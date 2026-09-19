using System.Collections.Generic;
using System.Linq;
using Solstice.Dynamics;
using UnityEngine;

namespace Solstice
{
    public class Player : MonoBehaviour
    {
        private InputSystem_Actions _controls;

        public VehicleState vehicleState;

        [field: SerializeField] public float steeringWheelAngleRatePerSecond { get; private set; } = 450f;
        [field: SerializeField] public float acceleratorPositionRatePerSecond { get; private set; } = 50f;
        [field: SerializeField] public float maxSteeringWheelAngle { get; private set; } = 450f;

        [SerializeField] private List<ForceModel> forceModels = new();
        [SerializeField] private List<YawRateModel> yawRateModels = new();

        private Rigidbody _rigidbody;
        private BoxCollider _boxCollider;

        private const float RaycastOffsetY = 1000f;

        private void ApplyAcceleration(float value)
        {
            // var newAcceleratorPosition =
            //     vehicleState.AcceleratorPosition.Value + value.y * acceleratorPositionRatePerSecond * deltaTime;
            // vehicleState.AcceleratorPosition.Set(Mathf.Clamp(newAcceleratorPosition, 0f, 100f));
            
            vehicleState.AcceleratorPosition.Set(value  * 100f);
        }
        
        private void ApplySteering(float value)
        {
            // var newSteeringWheelAngle =
            //     vehicleState.SteeringWheelAngle.Value + value * steeringWheelAngleRatePerSecond * deltaTime;
            // vehicleState.SteeringWheelAngle.Set(Mathf.Clamp(newSteeringWheelAngle, -maxSteeringWheelAngle, maxSteeringWheelAngle));

            vehicleState.SteeringWheelAngle.Set(value  * maxSteeringWheelAngle);
        }
        
        private void ApplyMechBrake(float value)
        {
            vehicleState.MechBrakePosition.Set(value  * 100f);
        }

        private void Update()
        {
            ApplySteering(_controls.Player.Steer.ReadValue<float>());
            ApplyMechBrake(_controls.Player.MechBrake.ReadValue<float>());
            ApplyAcceleration(_controls.Player.Accelerate.ReadValue<float>());
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

        private void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _boxCollider = GetComponent<BoxCollider>();
            
            _boxCollider.enabled = false;
        }

        private void FixedUpdate()
        {
            SnapToTrack();
            
            var frame = new VehicleFrame2D(
                vehicleState,
                new Vector2(_rigidbody.linearVelocity.x, _rigidbody.linearVelocity.z),
                new Vector2(transform.forward.x, transform.forward.z),
                new Vector2(transform.right.x, transform.right.z),
                _rigidbody.mass);

            var netForce = forceModels.Aggregate(
                Vector2.zero, (current, forceModel) => current + forceModel.ComputeForce(frame));
            vehicleState.AppliedForce.Set(netForce); // For debug
            var netForce3D = new Vector3(netForce.x, 0f, netForce.y);
            _rigidbody.AddForce(netForce3D, ForceMode.Force);

            var netYawRate = yawRateModels.Sum(model => model.ComputeYawRate(frame));
            _rigidbody.angularVelocity = new Vector3(0, netYawRate, 0);

            Vector3 flatVelocity = Vector3.ProjectOnPlane(_rigidbody.linearVelocity, Vector3.up);
            Vector3 flatForward = Vector3.ProjectOnPlane(_rigidbody.transform.forward, Vector3.up).normalized;
            vehicleState.ForwardVelocity.Set(Vector3.Dot(flatVelocity, flatForward));
        }

        private void SnapToTrack()
        {
            // Force pitch and roll to zero
            _rigidbody.angularVelocity = new Vector3(0f, _rigidbody.angularVelocity.y, 0f);
            var flatForward = Vector3.ProjectOnPlane(transform.forward, Vector3.up);

            if (flatForward.sqrMagnitude > 0.001f)
            {
                _rigidbody.rotation = Quaternion.LookRotation(flatForward, Vector3.up);
            }

            // Snap y-value to track
            var centerToGroundSetpoint = _boxCollider.size.y;
            var worldCenter = transform.TransformPoint(_boxCollider.center);
            var raycastOrigin = worldCenter + Vector3.up * RaycastOffsetY;
            if (Physics.Raycast(raycastOrigin, Vector3.down, out var hit, Mathf.Infinity))
            {
                var centerToGroundMeasured = hit.distance - RaycastOffsetY;
                var targetPosition = _rigidbody.position;
                targetPosition += Vector3.down * (centerToGroundMeasured - centerToGroundSetpoint);
                _rigidbody.position = targetPosition;
            }
        }
    }
}