using UnityEngine;

namespace Solstice.Dynamics
{
    // Kinematic bicycle model: yaw rate = (speed / wheelBase) * tan(road wheel angle).
    [CreateAssetMenu(menuName = "Solstice/Dynamics/Bicycle Yaw Rate Model", fileName = "BicycleYawRateModel")]
    public class BicycleYawRateModel : YawRateModel
    {
        [SerializeField] private float wheelBase = 2.8f;

        // Degrees of steering wheel per degree of road wheel. The state carries a
        // *steering wheel* angle, and feeding that straight into tan() as though
        // it were the road wheel angle made the car yaw about an order of
        // magnitude too fast (35 deg at 25 m/s works out to roughly 400 deg/s).
        [SerializeField] private float steeringRatio = 15f;

        public override float ComputeYawRate(VehicleFrame frame)
        {
            if (frame.Speed <= 0.01f) return 0f;

            float roadWheelAngleRad = frame.State.SteeringWheelAngle.Value / steeringRatio * Mathf.Deg2Rad;
            return frame.Speed / wheelBase * Mathf.Tan(roadWheelAngleRad);
        }
    }
}
