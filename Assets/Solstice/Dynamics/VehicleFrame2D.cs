using UnityEngine;

namespace Solstice.Dynamics
{
    // Snapshot of the physical inputs a dynamics model needs for one physics
    // step. Keeps models decoupled from Rigidbody/Transform so they stay
    // plain, testable C#.
    //
    // Models operate in two dimensions (X and Z linear velocity, Y angular velocity i.e., yaw rate).
    // Linear velocity in Y and angular velocity in X and Z (pitch/roll) are purely cosmetic.
    public readonly struct VehicleFrame2D
    {
        public VehicleState State { get; }
        public Vector2 Velocity { get; }
        public Vector2 Forward { get; }
        public Vector2 Right { get; }
        public float Mass { get; }

        public VehicleFrame2D(VehicleState state, Vector2 velocity, Vector2 forward, Vector2 right, float mass)
        {
            State = state;
            Velocity = velocity;
            Forward = forward;
            Right = right;
            Mass = mass;
        }
    }

    public interface IForceModel
    {
        Vector2 ComputeForce(VehicleFrame2D frame);
    }

    public interface IYawRateModel
    {
        float ComputeYawRate(VehicleFrame2D frame);
    }
}
