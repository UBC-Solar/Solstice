using System;
using UnityEngine;

namespace Solstice
{
    public class VehicleState : MonoBehaviour
    {
        [field: SerializeField] public Signal<float> AcceleratorPosition { get; private set; } = new(0f);
        [field: SerializeField] public Signal<float> MechBrakePosition { get; private set; } = new(0f);
        [field: SerializeField] public Signal<float> SteeringWheelAngle { get; private set; } = new(0f);

        [field: SerializeField] public Signal<float> MotorForce { get; private set; } = new(0f);
        [field: SerializeField] public Signal<float> MechBrakeForce { get; private set; } = new(0f);

        [field: SerializeField] public Signal<float> ForwardVelocity { get; private set; } = new(0f);
        
        [field: SerializeField] public Signal<Vector2> AppliedForce { get; private set; } = new(new Vector2(0f, 0f));
    }
}
