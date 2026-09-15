using UnityEngine;

namespace Solstice.Dynamics
{
    // Lateral tire grip: bleeds off the sideways component of velocity so the
    // vehicle's momentum follows where it's pointing.
    //
    // Without this there is no sideways force anywhere in the simulation — the
    // raycast suspension only pushes along the chassis's up axis, and the yaw
    // model rotates the body without touching linear velocity — so steering
    // changes the car's heading while it keeps sliding along its original path.
    // (Before the raycast suspension existed, the chassis collider rested on the
    // ground and PhysX contact friction was doing this job implicitly.)
    //
    // The clamp is what keeps this from being a rail: below the limit the tires
    // hold and the car tracks its heading, above it they let go and the car
    // slides, which is roughly how a real tire behaves past its friction circle.
    [CreateAssetMenu(menuName = "Solstice/Dynamics/Lateral Grip Model", fileName = "LateralGripModel")]
    public class LateralGripModel : ForceModel
    {
        // Time to null out sideways velocity if grip were unlimited. Smaller is
        // sharper/more darty; larger lets the back end wander.
        [SerializeField] private float gripTimeConstant = 0.15f;

        // Friction circle ceiling. ~8 m/s^2 is about 0.8g, a decent road tire.
        [SerializeField] private float maxLateralAcceleration = 8f;

        public override Vector2 ComputeForce(VehicleFrame2D frame)
        {

            float lateralSpeed = Vector2.Dot(frame.Velocity, frame.Right);
            float acceleration = Mathf.Clamp(-lateralSpeed / gripTimeConstant, -maxLateralAcceleration, maxLateralAcceleration);

            // Applied at the center of mass, so cornering produces no body roll.
            // Getting roll out of this means moving to per-wheel tire forces
            // applied at each contact patch, with the friction clamp scaled by
            // that wheel's own suspension normal force.
            return frame.Right * (acceleration * frame.Mass);
        }
    }
}
