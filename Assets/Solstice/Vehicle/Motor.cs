using Solstice.Messages;
using UnityEngine;

namespace Solstice
{
    public class Motor : MonoBehaviour, IVehicleComponent
    {
        private const float MaxCurrentAmps = 40f;
        private const float ForcePerAmp = 1f; // N of forward thrust per amp of motor current

        private MessageBus _messageBus;
        private VehicleState _vehicleState;

        private void OnAccelerationMessage(AccelerationMessage message)
        {
            float acceleratorPercent = Mathf.Clamp(message.AccelerationValue, 0f, 100f);
            float motorCurrent = acceleratorPercent / 100f * MaxCurrentAmps;
            float tractiveForce = motorCurrent * ForcePerAmp;

            _vehicleState.TractiveForce.Set(tractiveForce);
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
