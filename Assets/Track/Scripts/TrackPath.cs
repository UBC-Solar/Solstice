
using UnityEngine;

public class Track : BezierSpline
{
    [Tooltip("Automatically align handles when extending the track.")]
    public bool enforceG1WhenExtending = true;
    public bool alwaysEnforceG1 = true;

    public override BezierCurve ExtendSpline()
    {
        BezierCurve curve = base.ExtendSpline();

        if (enforceG1WhenExtending)
            EnforceG1();

        return curve;
    }

    private void Update() {
        if(alwaysEnforceG1){EnforceG1();}
    }
    public void EnforceG1()
    {
        curves.RemoveAll(curve => curve == null);

        for (int i = 0; i < curves.Count - 1; i++)
        {
            BezierCurve previous = curves[i];
            BezierCurve next = curves[i + 1];

            if (previous == null || next == null)
                continue;

            // The incoming tangent direction at the join
            Vector3 direction =
                previous.point3.position -
                previous.point2.position;

            if (direction.sqrMagnitude < 0.0001f)
                continue;

            direction.Normalize();

            // Preserve the next handle's distance from the join
            float handleLength = Vector3.Distance(
                next.point0.position,
                next.point1.position
            );

            next.point1.position =
                next.point0.position +
                direction * handleLength;
        }
    }
}
