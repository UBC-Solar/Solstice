using UnityEngine;

namespace Solstice
{
    public class SteeringWheel : MonoBehaviour
    {
        [field: SerializeField] private VehicleState State { get; set; }

        // Steering column axis in the parent's (car's) space
        [SerializeField] private Vector3 parentSpinAxis = Vector3.forward;

        private Vector3 restPosition;     // local position at angle = 0
        private Quaternion restRotation;  // local rotation at angle = 0 (e.g. -90° X)
        private Vector3 pivotCenter;      // visual center in the parent's space

        void Awake()
        {
            restPosition = transform.localPosition;
            restRotation = transform.localRotation;

            Vector3 worldCenter = GetComponentInChildren<Renderer>().bounds.center;
            pivotCenter = transform.parent != null
                ? transform.parent.InverseTransformPoint(worldCenter)
                : worldCenter;
        }

        void Update()
        {
            // Spin about a fixed axis of the car, applied on top of the rest rotation
            Quaternion spin = Quaternion.AngleAxis(-State.SteeringWheelAngle.Value, parentSpinAxis);

            Quaternion rotation = spin * restRotation;
            Vector3 position = pivotCenter + spin * (restPosition - pivotCenter);

            transform.SetLocalPositionAndRotation(position, rotation);
        }
    }
}