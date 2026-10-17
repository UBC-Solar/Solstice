
<<<<<<< HEAD
using System;
=======
>>>>>>> 6542ee1 (Implemented Bezier Curve Object)
using UnityEngine;

public class Track : BezierSpline
{
<<<<<<< HEAD
    [Tooltip("Automatically align handles when extending the track")]
    public bool enforceG1WhenExtending = true;
    public bool alwaysEnforceG1 = true;

    public bool closeLoop = false;

    public Transform trackedObject;
=======
    [Tooltip("Automatically align handles when extending the track.")]
    public bool enforceG1WhenExtending = true;
    public bool alwaysEnforceG1 = true;

>>>>>>> 6542ee1 (Implemented Bezier Curve Object)
    public override BezierCurve ExtendSpline()
    {
        BezierCurve curve = base.ExtendSpline();

        if (enforceG1WhenExtending)
            EnforceG1();

        return curve;
    }

    private void Update() {
        if(alwaysEnforceG1){EnforceG1();}
<<<<<<< HEAD
        if(closeLoop){CloseLoop();}
=======
>>>>>>> 6542ee1 (Implemented Bezier Curve Object)
    }
    public void EnforceG1()
    {
        curves.RemoveAll(curve => curve == null);

<<<<<<< HEAD
        int curveCount = curves.Count;

        for (int i = 0; i < curves.Count; i++)
        {
            if (curveCount == 1)
                continue;
            
            BezierCurve previous = curves[i % curveCount];
            BezierCurve next = curves[(i + 1) % curveCount];
=======
        for (int i = 0; i < curves.Count - 1; i++)
        {
            BezierCurve previous = curves[i];
            BezierCurve next = curves[i + 1];
>>>>>>> 6542ee1 (Implemented Bezier Curve Object)

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
<<<<<<< HEAD

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
=======
>>>>>>> 6542ee1 (Implemented Bezier Curve Object)
}
