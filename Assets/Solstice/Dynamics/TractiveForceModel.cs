using UnityEngine;

namespace Solstice.Dynamics
{
    // Pushes the vehicle forward with whatever tractive force the motor is currently reporting.
    [CreateAssetMenu(menuName = "Solstice/Dynamics/Tractive Force Model", fileName = "TractiveForceModel")]
    public class TractiveForceModel : ForceModel
    {
        public override Vector2 ComputeForce(VehicleFrame2D frame) =>
            frame.Forward * (frame.State.MotorForce.Value + frame.State.MechBrakeForce.Value);
    }
}