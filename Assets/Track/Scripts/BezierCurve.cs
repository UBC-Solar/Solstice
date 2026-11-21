
using UnityEngine;

[ExecuteAlways]
public class BezierCurve : MonoBehaviour
{
    [Header("Control Points")]
    public Transform point0;
    public Transform point1;
    public Transform point2;
    public Transform point3;
    public bool draw = true;
    public Color curveColor = Color.cyan;

    public Color handleColor = Color.blue;

    [Min(4)]
    public int gizmoSegments = 24;

    private void OnEnable()
    {
        EnsurePointsExist();
    }

    private void Update()
    {
        EnsurePointsExist();
        flattenY();
    }

/// <summary>
/// Helper method to set points to be on the same vertical plane
/// </summary>
    private void flattenY()
    {
        setY(point0, 0);
        setY(point1, 0);
        setY(point2, 0);
        setY(point3, 0);
    }

    private void setY(Transform point, float y)
    {
        point.localPosition = new Vector3(point.localPosition.x, y, point.localPosition.z);
    }

/// <summary>
/// Helper method to make sure that all points exist and are edittable, since beziers are created without points
/// </summary>
    private void EnsurePointsExist()
    {
        float length = 6f;

        point0 = EnsurePoint(point0, "Point 0 - Start", Vector3.zero);
        point1 = EnsurePoint(point1, "Point 1 - Handle", Vector3.forward * (length / 3f));
        point2 = EnsurePoint(point2, "Point 2 - Handle", Vector3.forward * (length * 2f / 3f));
        point3 = EnsurePoint(point3, "Point 3 - End", Vector3.forward * length);
    }

/// <summary>
/// Helper method to make sure that all points exist and are edittable, since beziers are created without points
/// 
/// Checks if a point is valid, if not, create a new point
/// </summary>
/// <param name="existing"></param>
/// <param name="pointName"></param>
/// <param name="localPosition"></param>
/// <returns></returns>
    private Transform EnsurePoint(
        Transform existing,
        string pointName,
        Vector3 localPosition)
    {
        if (existing != null)
            return existing;

        GameObject pointObject = new GameObject(pointName);
        Transform point = pointObject.transform;

        point.SetParent(transform, false);
        point.localPosition = localPosition;

        return point;
    }

/// <summary>
/// Finds point on bezier which corresponds to parameter t
/// </summary>
/// <param name="t"></param>
/// <returns></returns>
    public Vector3 Evaluate(float t)
    {
        t = Mathf.Clamp01(t);
        float u = 1f - t;

        Vector3 p0 = point0.position;
        Vector3 p1 = point1.position;
        Vector3 p2 = point2.position;
        Vector3 p3 = point3.position;

        return u * u * u * p0
             + 3f * u * u * t * p1
             + 3f * u * t * t * p2
             + t * t * t * p3;
    }

/// <summary>
/// Finds the tangent vector of any given point on the bezier from parameter t
/// </summary>
/// <param name="t"></param>
/// <returns></returns>
    public Vector3 Tangent(float t)
    {
        t = Mathf.Clamp01(t);
        float u = 1f - t;

        Vector3 p0 = point0.position;
        Vector3 p1 = point1.position;
        Vector3 p2 = point2.position;
        Vector3 p3 = point3.position;

        return 3f * u * u * (p1 - p0)
             + 6f * u * t * (p2 - p1)
             + 3f * t * t * (p3 - p2);
    }

    public Transform StartPoint =>
        point0 != null ? point0 : transform; // Ternary operators are the most useless notation, however, I like them

    public Transform EndPoint =>
        point3 != null ? point3 : transform;

    private void OnDrawGizmos()
    {
        if (!draw ||
            point0 == null || point1 == null ||
            point2 == null || point3 == null)
            return;

        Gizmos.color = curveColor;

        Vector3 previous = Evaluate(0f);
        int segments = Mathf.Max(4, gizmoSegments);

        for (int i = 1; i <= segments; i++)
        {
            Vector3 next = Evaluate(i / (float)segments);
            Gizmos.DrawLine(previous, next);
            previous = next;
        }

        Gizmos.color = handleColor;
        Gizmos.DrawLine(point0.position, point1.position);

        Gizmos.color = (handleColor + Color.red)/2;
        Gizmos.DrawLine(point2.position, point3.position);

        Gizmos.color = Color.white;
        Gizmos.DrawSphere(point0.position, 0.3f);
        Gizmos.DrawSphere(point3.position, 0.3f);

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(point1.position, 0.2f);
        Gizmos.DrawSphere(point2.position, 0.2f);
    }
}
