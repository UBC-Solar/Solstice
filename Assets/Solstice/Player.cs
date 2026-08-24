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

        [field: SerializeField] public float steeringWheelAnglePerPress { get; private set; }= 10;
        [field: SerializeField] public float acceleratorPositionPerPress { get; private set; } = 10;
        [field: SerializeField] public float maxSteeringWheelAngle { get; private set; } = 35f;

        [SerializeField] private List<ForceModel> forceModels = new();
        [SerializeField] private List<YawRateModel> yawRateModels = new();

        private Rigidbody _rigidbody;

        private void OnMove(InputAction.CallbackContext ctx)
        {
            Vector2 value = ctx.ReadValue<Vector2>();

            float steeringWheelAngleChange = value.x * steeringWheelAnglePerPress;
            float acceleratorPositionChange = value.y * acceleratorPositionPerPress;

            float newSteeringWheelAngle = vehicleState.SteeringWheelAngle.Value + steeringWheelAngleChange;
            vehicleState.SteeringWheelAngle.Set(Mathf.Clamp(newSteeringWheelAngle, -maxSteeringWheelAngle, maxSteeringWheelAngle));

            float newAcceleratorPosition = vehicleState.AcceleratorPosition.Value + acceleratorPositionChange;
            vehicleState.AcceleratorPosition.Set(Mathf.Clamp(newAcceleratorPosition, 0f, 100f));
        }

        private void OnEnable()
        {
            _controls ??= new InputSystem_Actions();
            _controls.Player.Move.performed += OnMove;
            _controls.Enable();
        }

        private void OnDisable()
        {
            _controls.Player.Move.performed -= OnMove;
            _controls.Disable();
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        // FixedUpdate just moves data between VehicleState and the Rigidbody: build
        // this step's frame, sum the dynamics models' contributions, push them into
        // physics, then report the resulting speed back for other components to consume.
        void FixedUpdate()
        {
            var frame = new VehicleFrame(vehicleState, _rigidbody.linearVelocity, transform.forward);

            Vector3 netForce = Vector3.zero;
            foreach (ForceModel model in forceModels)
                netForce += model.ComputeForce(frame);

            _rigidbody.AddForce(netForce, ForceMode.Force);

            float netYawRate = 0f;
            foreach (YawRateModel model in yawRateModels)
                netYawRate += model.ComputeYawRate(frame);

            _rigidbody.angularVelocity = new Vector3(0f, netYawRate, 0f);

            vehicleState.Speed.Set(_rigidbody.linearVelocity.magnitude);
        }
    }
}
