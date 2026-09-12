using UnityEngine;

namespace Solstice.Dynamics
{
    // Force of gravity: F = mg.
    //
    // The vehicle's Rigidbody has Use Gravity switched off so that weight comes
    // through this model like every other force, which keeps it visible and
    // tunable alongside the rest of the dynamics.
    [CreateAssetMenu(menuName = "Solstice/Dynamics/Gravity Model", fileName = "GravityModel")]
    public class GravityModel : ForceModel
    {
        // Magnitude, not a signed component: the direction comes from frame.Down.
        // A negative value here points gravity at the sky.
        [SerializeField] private float gravityAcceleration = 9.81f;

        // Mass comes from the frame (i.e. the Rigidbody) rather than a field of
        // our own, so there is only one place to change it. Carrying a second
        // copy here meant a Rigidbody mass edit silently left weight behind.
        public override Vector3 ComputeForce(VehicleFrame frame) => frame.Mass * gravityAcceleration * frame.Down;
    }
}
