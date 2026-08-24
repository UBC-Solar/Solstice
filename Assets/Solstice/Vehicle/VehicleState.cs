using System;
using UnityEngine;

namespace Solstice
{
    public class VehicleState : MonoBehaviour
    {
        [field: SerializeField] public Signal<float> AcceleratorPosition { get; private set; } = new(0f);
        [field: SerializeField] public Signal<float> SteeringWheelAngle { get; private set; } = new(0f);

        [field: SerializeField] public Signal<float> TractiveForce { get; private set; } = new(0f);
        [field: SerializeField] public Signal<float> Speed { get; private set; } = new(0f);
    }
}
