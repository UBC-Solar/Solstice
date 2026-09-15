using Solstice.Messages;
using UnityEngine;

namespace Solstice
{
    public class Motor : MonoBehaviour, IVehicleComponent
    {
        private const float MaxCurrentAmps = 40f;
        // N of forward thrust per amp of motor current. This was briefly cranked
        // to 1000 (40 kN at full throttle, over 11 m/s^2 on a 350 kg car) to get
        // the vehicle moving at all, but the real problem was that nothing was
        // generating grip, so thrust was fighting terrain gradients with no tire
        // forces to help. 12 N/A gives ~480 N flat out, roughly 0.14 g, which is
        // in the right neighbourhood for a ~4 kW solar car.
        [field: SerializeField] private float ForcePerAmp = 12f;

        private MessageBus _messageBus;
        private VehicleState _vehicleState;

        private void OnAccelerationMessage(AccelerationMessage message)
        {
            float acceleratorPercent = Mathf.Clamp(message.AccelerationValue, 0f, 100f);
            float motorCurrent = acceleratorPercent / 100f * MaxCurrentAmps;
            float motorForce = motorCurrent * ForcePerAmp;

            _vehicleState.MotorForce.Set(motorForce);
        }

        void OnDisable()
        {
            _messageBus.Unsubscribe<AccelerationMessage>(OnAccelerationMessage);
        }

        public void Construct(MessageBus messageBus, VehicleState vehicleState)
        {
            _messageBus = messageBus;
            _vehicleState = vehicleState;
            
            _messageBus.Subscribe<AccelerationMessage>(OnAccelerationMessage);
        }
    }
}
