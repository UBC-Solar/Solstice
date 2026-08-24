using Solstice.Messages;
using UnityEngine;

namespace Solstice
{
    public class DriverDashboard : FixedRateBehaviour, IVehicleComponent
    {
        private MessageBus _messageBus;
        private VehicleState _vehicleState;

        protected override void OnTick()
        {
            float accelerationValue = _vehicleState.AcceleratorPosition.Value;
            
            _messageBus.Publish(new AccelerationMessage(accelerationValue));
        }

        public void Construct(MessageBus messageBus, VehicleState vehicleState)
        {
            _messageBus = messageBus;
            _vehicleState = vehicleState;
        }
    }
}
