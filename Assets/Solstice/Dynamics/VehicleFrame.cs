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
        public float Speed => Velocity.magnitude;

        public VehicleFrame(VehicleState state, Vector3 velocity, Vector3 forward)
        {
            State = state;
            Velocity = velocity;
            Forward = forward;
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