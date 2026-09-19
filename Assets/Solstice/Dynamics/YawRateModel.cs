using UnityEngine;

namespace Solstice.Dynamics
{
    // Asset-based yaw rate contribution: create instances via the Assets menu and
    // drag them into a Player's yaw rate model list to tune them per-instance.
    public abstract class YawRateModel : ScriptableObject, IYawRateModel
    {
        public abstract float ComputeYawRate(VehicleFrame2D frame);
    }
}