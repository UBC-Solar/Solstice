using UnityEngine;

namespace Solstice.Dynamics
{
    // Snapshot of the physical inputs a dynamics model needs for one physics
    // step. Keeps models decoupled from Rigidbody/Transform so they stay
    // plain, testable C#.
    public readonly struct VehicleFrame
    {
        public VehicleState State { get; }
        public Vector3 Velocity { get; }
        public Vector3 Forward { get; }
        public Vector3 Right { get; }
        public float Mass { get; }

        // Fraction of the vehicle's wheels currently touching the ground, 0..1.
        // Models that represent a force transmitted through the tires (grip,
        // rolling resistance) must scale by this — a car in mid-air has nothing
        // to push against, and a model that ignores that will happily corner
        // off the top of a jump.
        public float GroundedFraction { get; }

        public Vector3 Down => Vector3.down;
        public float Speed => Velocity.magnitude;

        public VehicleFrame(VehicleState state, Vector3 velocity, Vector3 forward, Vector3 right, float mass, float groundedFraction)
        {
            State = state;
            Velocity = velocity;
            Forward = forward;
            Right = right;
            Mass = mass;
            GroundedFraction = groundedFraction;
        }
    }

    public interface IForceModel
    {
        Vector3 ComputeForce(VehicleFrame frame);
    }

    public interface IYawRateModel
    {
        float ComputeYawRate(VehicleFrame frame);
    }
}
