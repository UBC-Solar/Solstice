using System.Collections.Generic;
using Solstice.Dynamics;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Solstice
{
    public class Player : MonoBehaviour
    {
        private InputSystem_Actions _controls;

        public VehicleState vehicleState;

        // Rates, not per-event increments. The Move action is a Value action, so
        // its `performed` callback fires once per *change* in the stick or key
        // state — holding a key down is a single event, not one per frame. Driving
        // the controls off that callback meant one tap of the accelerator moved the
        // pedal by a single increment and left it there, so the throttle sat at a
        // fraction of a percent no matter how long you held the key, and the motor
        // had to be given absurd force-per-amp numbers to produce any movement at
        // all. We poll the action's value each frame and integrate it instead.
        [field: SerializeField] public float steeringWheelAngleRatePerSecond { get; private set; } = 450f;
        [field: SerializeField] public float acceleratorPositionRatePerSecond { get; private set; } = 50f;
        [field: SerializeField] public float maxSteeringWheelAngle { get; private set; } = 450f;

        [SerializeField] private List<ForceModel> forceModels = new();
        [SerializeField] private List<YawRateModel> yawRateModels = new();
        [SerializeField] private List<Wheel> wheels = new();

        private Rigidbody _rigidbody;

        // Held input moves the controls at a constant rate, the way a driver winds
        // a steering wheel round or squeezes a pedal down. Both controls latch:
        // let go and they stay where you left them, which is what the accumulating
        // behaviour here has always intended.
        private void ApplyMoveInput(Vector2 value, float deltaTime)
        {
            float newSteeringWheelAngle =
                vehicleState.SteeringWheelAngle.Value + value.x * steeringWheelAngleRatePerSecond * deltaTime;
            vehicleState.SteeringWheelAngle.Set(Mathf.Clamp(newSteeringWheelAngle, -maxSteeringWheelAngle, maxSteeringWheelAngle));

            float newAcceleratorPosition =
                vehicleState.AcceleratorPosition.Value + value.y * acceleratorPositionRatePerSecond * deltaTime;
            vehicleState.AcceleratorPosition.Set(Mathf.Clamp(newAcceleratorPosition, 0f, 100f));
        }

        private void Update()
        {
            ApplyMoveInput(_controls.Player.Move.ReadValue<Vector2>(), Time.deltaTime);
        }

        private void OnEnable()
        {
            _controls ??= new InputSystem_Actions();
            _controls.Enable();
        }

        private void OnDisable()
        {
            _controls.Disable();
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        // FixedUpdate just moves data between VehicleState and the Rigidbody: run
        // the suspension, build this step's frame, sum the dynamics models'
        // contributions, push them into physics, then report the resulting speed
        // back for other components to consume.
        void FixedUpdate()
        {
            // Suspension runs first. As well as producing the per-wheel forces it
            // tells us how many wheels are actually touching down, which the frame
            // carries so that tire-borne models (grip, rolling resistance) know
            // whether there is any ground to push against.
            int groundedWheels = 0;
            foreach (Wheel wheel in wheels)
            {
                if (!wheel.TryComputeForce(transform, Time.fixedDeltaTime, out Vector3 suspensionForce, out Vector3 contactPoint))
                    continue;

                // Applied at the contact point rather than the center of mass, so
                // asymmetric compression produces the roll/pitch torque that keeps
                // the chassis level. That's why these aren't folded into netForce.
                _rigidbody.AddForceAtPosition(suspensionForce, contactPoint, ForceMode.Force);
                groundedWheels++;
            }

            float groundedFraction = wheels.Count > 0 ? (float)groundedWheels / wheels.Count : 0f;

            var frame = new VehicleFrame(
                vehicleState,
                _rigidbody.linearVelocity,
                transform.forward,
                transform.right,
                _rigidbody.mass,
                groundedFraction);

            Vector3 netForce = Vector3.zero;
            foreach (ForceModel model in forceModels)
                netForce += model.ComputeForce(frame);

            _rigidbody.AddForce(netForce, ForceMode.Force);

            float netYawRate = 0f;
            foreach (YawRateModel model in yawRateModels)
                netYawRate += model.ComputeYawRate(frame);

            // Only yaw is prescribed by the models; roll and pitch are left to the
            // physics solver. Overwriting all three components zeroed the roll and
            // pitch rate every single step, so the suspension's leveling torque was
            // wiped out about as fast as it was applied and the chassis barely
            // reacted to terrain.
            Vector3 angularVelocity = _rigidbody.angularVelocity;
            angularVelocity.y = netYawRate;
            _rigidbody.angularVelocity = angularVelocity;

            vehicleState.Speed.Set(_rigidbody.linearVelocity.magnitude);
        }

        // Only when selected (not always-on) so four wheels' worth of gizmos don't
        // clutter the scene view while you're working on unrelated objects.
        private void OnDrawGizmosSelected()
        {
            foreach (Wheel wheel in wheels)
                wheel?.DrawGizmos(transform);
        }
    }
}
