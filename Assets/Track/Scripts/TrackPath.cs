
using System;
using UnityEngine;

public class Track : BezierSpline
{
    [Tooltip("Automatically align handles when extending the track")]
    public bool enforceG1WhenExtending = true;
    public bool alwaysEnforceG1 = true;

    public bool closeLoop = false;

    public Transform trackedObject;
    public override BezierCurve ExtendSpline()
    {
        BezierCurve curve = base.ExtendSpline();

        if (enforceG1WhenExtending)
            EnforceG1();

        return curve;
    }

    private void Update() {
        if(alwaysEnforceG1){EnforceG1();}
        if(closeLoop){CloseLoop();}
    }
    public void EnforceG1()
    {
        curves.RemoveAll(curve => curve == null);

        int curveCount = curves.Count;

        for (int i = 0; i < curves.Count; i++)
        {
            if (curveCount == 1)
                continue;
            
            BezierCurve previous = curves[i % curveCount];
            BezierCurve next = curves[(i + 1) % curveCount];

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

    public void CloseLoop()
    {
        BezierCurve finalCurve = curves[curves.Count - 1];
        BezierCurve firstCurve = curves[0];
        
        Vector3 finalPointPos = new Vector3(0f, 0f, 0f);

        finalPointPos.Set(finalCurve.point3.position.x,
                          finalCurve.point3.position.y,
                          finalCurve.point3.position.z);

        firstCurve.point0.position = finalPointPos;
    }
}
