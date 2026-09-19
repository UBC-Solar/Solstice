using UnityEngine;
using Unity.Mathematics;

public class WorldPoint : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The real world latitude of this object's position in game space")]
    private double latitude = 0.0f;
    
    [SerializeField]
    [Tooltip("The real world latitude of this object's position in game space")]
    private double longitude = 0.0f;
    
    public double Latitude => latitude;
    public double Longitude => longitude;
    
    /// <summary>
    /// Obtain the position of this WorldPoint 
    /// </summary>
    /// <param name="latitude"></param>
    /// <param name="longitude"></param>
    /// <returns></returns>
    public static double2 GetWorldPoint(double latitude, double longitude)
    {
        return new double2(latitude, longitude);
    }
}
