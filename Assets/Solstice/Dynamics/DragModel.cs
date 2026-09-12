using UnityEngine;

namespace Solstice.Dynamics
{
    // The resistive forces that set the vehicle's top speed: aerodynamic drag,
    // which is quadratic in speed, plus rolling resistance, which is roughly
    // constant once the wheels are turning.
    //
    // This used to be a single linear -k*v term, which is the wrong shape: with
    // k = 0.5 it produced about 12 N at 25 m/s, so the car effectively had no
    // top speed at all.
    [CreateAssetMenu(menuName = "Solstice/Dynamics/Drag Model", fileName = "DragModel")]
    public class DragModel : ForceModel
    {
        [Header("Aerodynamic")]
        [SerializeField] private float airDensity = 1.225f; // kg/m^3 at sea level
        [SerializeField] private float dragArea = 0.1f;     // Cd*A in m^2; solar cars are very slippery

        [Header("Rolling")]
        [SerializeField] private float rollingResistanceCoefficient = 0.008f; // Crr, good tires on asphalt
        [SerializeField] private float gravityAcceleration = 9.81f;           // for the Crr*m*g normal load

        // Rolling resistance has constant magnitude, so applying it at a standstill
        // would shove a parked car backwards and jitter it around zero. Fade it in
        // over the first fraction of a m/s instead.
        [SerializeField] private float rollingResistanceFadeInSpeed = 0.5f;

        public override Vector3 ComputeForce(VehicleFrame frame)
        {
            float speed = frame.Speed;
            if (speed < 1e-4f) return Vector3.zero;

            Vector3 direction = frame.Velocity / speed;

            float aerodynamicDrag = 0.5f * airDensity * dragArea * speed * speed;

            // Rolling resistance is transmitted through the tires, so it only
            // exists for wheels that are actually on the ground.
            float rollingResistance = rollingResistanceCoefficient * frame.Mass * gravityAcceleration
                                      * frame.GroundedFraction
                                      * Mathf.Clamp01(speed / rollingResistanceFadeInSpeed);

            return -direction * (aerodynamicDrag + rollingResistance);
        }
    }
}
