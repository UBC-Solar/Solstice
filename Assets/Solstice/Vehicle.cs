using UnityEngine;

namespace Solstice
{
    public class Vehicle : MonoBehaviour
    {
        [SerializeField] private DriverDashboard _driverDashboard;
        [SerializeField] private Motor _motor;
        [SerializeField] private VehicleState _vehicleState;

        private MessageBus _messageBus;

        private void Awake()
        {
            _messageBus = new MessageBus();

            _driverDashboard.Construct(_messageBus, _vehicleState);
            _motor.Construct(_messageBus, _vehicleState);
        }
    }
}
