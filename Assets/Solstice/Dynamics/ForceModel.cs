using UnityEngine;

namespace Solstice.Dynamics
{
    // Asset-based force contribution: create instances via the Assets menu and
    // drag them into a Player's force model list to tune them per-instance.
    public abstract class ForceModel : ScriptableObject, IForceModel
    {
        public abstract Vector2 ComputeForce(VehicleFrame2D frame);
    }
}