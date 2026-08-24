using UnityEngine;

namespace Solstice.Dynamics
{
    // Linear drag opposing velocity: F = -k * v.
    [CreateAssetMenu(menuName = "Solstice/Dynamics/Drag Model", fileName = "DragModel")]
    public class DragModel : ForceModel
    {
        [SerializeField] private float dragCoefficient = 0.5f;

        public override Vector3 ComputeForce(VehicleFrame frame) => -dragCoefficient * frame.Velocity;
    }
}