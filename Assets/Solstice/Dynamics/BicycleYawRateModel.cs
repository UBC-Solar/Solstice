using UnityEngine;

namespace Solstice.Dynamics
{
    // Kinematic bicycle model: yaw rate = (speed / wheelBase) * tan(steering angle).
    [CreateAssetMenu(menuName = "Solstice/Dynamics/Bicycle Yaw Rate Model", fileName = "BicycleYawRateModel")]
    public class BicycleYawRateModel : YawRateModel
    {
        [SerializeField] private float wheelBase = 2.5f;

        public override float ComputeYawRate(VehicleFrame frame)
        {
            if (frame.Speed <= 0.01f) return 0f;

            float steeringAngleRad = frame.State.SteeringWheelAngle.Value * Mathf.Deg2Rad;
            return frame.Speed / wheelBase * Mathf.Tan(steeringAngleRad);
        }
    }
}