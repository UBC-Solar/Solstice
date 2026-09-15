using Solstice.Messages;
using UnityEngine;

namespace Solstice
{
    public class MechBrake : MonoBehaviour, IVehicleComponent
    {
        // Max total stopping force (N) at 100% input
        [field: SerializeField] private float MaxBrakingForceNewtons = 2000f;

        private MessageBus _messageBus;
        private VehicleState _vehicleState;

        private void OnMechBrakeMessage(MechBrakeMessage message)
        {
            var mechBrakePercent = Mathf.Clamp(message.MechBrakeValue, 0f, 100f);
            var speed = _vehicleState.ForwardVelocity.Value;
            
            var brakeInput = Mathf.Clamp01(mechBrakePercent / 100f);
    
            // Smooth transition envelope around zero velocity (v_threshold ~0.5 m/s)
            const float velocityThreshold = 0.5f;
            var velocityEnvelope = Unity.Mathematics.math.tanh(speed / velocityThreshold);
            var brakingForce = -velocityEnvelope * brakeInput * MaxBrakingForceNewtons;

            _vehicleState.MechBrakeForce.Set(brakingForce);
        }

        private void OnDisable()
        {
            _messageBus.Unsubscribe<MechBrakeMessage>(OnMechBrakeMessage);
        }

        public void Construct(MessageBus messageBus, VehicleState vehicleState)
        {
            _messageBus = messageBus;
            _vehicleState = vehicleState;
            
            _messageBus.Subscribe<MechBrakeMessage>(OnMechBrakeMessage);
        }
    }
}