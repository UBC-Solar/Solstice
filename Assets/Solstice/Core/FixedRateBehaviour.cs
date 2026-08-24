using UnityEngine;

namespace Solstice
{
    /// <summary>
    /// Base class for MonoBehaviours that need to run logic at a fixed rate,
    /// independent of frame rate (e.g. emulating a periodic CAN broadcast).
    /// </summary>
    public abstract class FixedRateBehaviour : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("How many times per second OnTick() should be called.")]
        private float frequencyHz = 10f;

        private float _timer;

        // Exposed in case something needs to change rate at runtime
        // (e.g. simulating a sensor whose polling rate is configurable).
        protected float FrequencyHz
        {
            get => frequencyHz;
            set => frequencyHz = Mathf.Max(0.0001f, value); // guard div-by-zero
        }

        private float Interval => 1f / frequencyHz;

        protected virtual void Update()
        {
            _timer += Time.deltaTime;

            // while loop, not if — catches up properly if multiple
            // intervals elapsed in one frame (e.g. after a hitch)
            while (_timer >= Interval)
            {
                _timer -= Interval;
                OnTick();
            }
        }

        /// <summary>
        /// Called at the configured frequency. Implement your periodic logic here.
        /// </summary>
        protected abstract void OnTick();
    }
}

/*
 * Usage:
 *
 *   public class AcceleratorPublisher : FixedRateBehaviour
 *   {
 *       private MessageBus _bus;
 *       private VehicleState _vehicleState;
 *
 *       public void Construct(MessageBus bus, VehicleState vehicleState)
 *       {
 *           _bus = bus;
 *           _vehicleState = vehicleState;
 *       }
 *
 *       protected override void OnTick()
 *       {
 *           _bus.Publish(new AcceleratorPositionMessage(_vehicleState.AcceleratorPosition.Value));
 *       }
 *   }
 *
 * frequencyHz is exposed in the Inspector by default (10 Hz), so you can
 * tune per-instance rates per component without touching code.
 */