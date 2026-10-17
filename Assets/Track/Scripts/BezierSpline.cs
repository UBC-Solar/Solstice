
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class BezierSpline : MonoBehaviour
{   
    [Range(0.0f, 1.0f)]
    public float testT = 0;
    public List<BezierCurve> curves = new List<BezierCurve>();

    public float handleLength = 6f;

    public bool draw = true;

    public int CurveCount => curves.Count;

    public BezierCurve LastCurve =>
        curves.Count > 0 ? curves[curves.Count - 1] : null;

    
    public virtual BezierCurve ExtendSpline()
    {
        curves.RemoveAll(curve => curve == null);

        GameObject curveObject = new GameObject(
            $"Bezier Curve {curves.Count}"
        );

        // Parent first, then place the object
        curveObject.transform.SetParent(transform, false);

        BezierCurve curve = curveObject.AddComponent<BezierCurve>(); // Give object the bezier component to use
        curve.draw = draw;

        if (LastCurve != null)
        {
            BezierCurve previous = LastCurve;

            Transform start = previous.EndPoint;
            Vector3 tangent = previous.Tangent(1f);

            // Keep the tangent on the horizontal XZ plane
            tangent.y = 0f;

            tangent.Normalize();

<<<<<<< HEAD
            // Position the new curve object at the previous endpoint
=======
            // Position the new curve object at the previous endpoint.
>>>>>>> 6542ee1 (Implemented Bezier Curve Object)
            curveObject.transform.position = start.position;
            curveObject.transform.rotation =
                Quaternion.LookRotation(tangent, Vector3.up);

<<<<<<< HEAD
=======
            // The curve's control points are children, so set them
            // in local coordinates relative to the new curve object.
>>>>>>> 6542ee1 (Implemented Bezier Curve Object)
            Transform oldPoint0 = curve.point0;

            curve.point0 = start;
            curve.point0.SetParent(curveObject.transform, true);

            if (oldPoint0 != null && oldPoint0 != curve.point0)
                DestroyImmediate(oldPoint0.gameObject);

            curve.point1.localPosition = Vector3.forward * handleLength;
            curve.point2.localPosition = Vector3.forward * (handleLength * 2f);
            curve.point3.localPosition = Vector3.forward * (handleLength * 3f);
        }
        else
        {
<<<<<<< HEAD
            // For the first curve, use the spline's position and orientation
=======
            // For the first curve, use the spline's position and orientation.
>>>>>>> 6542ee1 (Implemented Bezier Curve Object)
            curveObject.transform.position = transform.position;
            curveObject.transform.rotation = transform.rotation;
        }

        curves.Add(curve);
        return curve;
    }

    public Vector3 Evaluate(float t)
    {
        curves.RemoveAll(curve => curve == null);

        if (curves.Count == 0)
            return transform.position;

        t = Mathf.Clamp01(t);

        float scaled = t * curves.Count;
        int index = Mathf.Min(
            Mathf.FloorToInt(scaled),
            curves.Count - 1
        );

        float localT = index == curves.Count - 1 && t >= 1f
            ? 1f : scaled - index;

        return curves[index].Evaluate(localT);
    }

    protected virtual void OnDrawGizmos()
    {
        if (!draw)
            return;

        foreach (BezierCurve curve in curves)
        {
            if (curve != null)
                curve.draw = draw;
        }

        Gizmos.color = Color.yellow;

        Gizmos.DrawSphere(Evaluate(testT), 1.0f);
    }
}
