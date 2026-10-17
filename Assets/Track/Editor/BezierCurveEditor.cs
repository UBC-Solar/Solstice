
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BezierCurve))]
public class BezierCurveEditor : Editor
{
    private void OnSceneGUI()
    {
        BezierCurve curve = (BezierCurve)target;

        if (curve == null || !curve.draw)
            return;

        DrawPoint(curve, curve.point0, "P0 - Start", Color.white, 0.13f);
        DrawPoint(curve, curve.point1, "P1 - Handle", curve.handleColor, 0.11f);
        DrawPoint(curve, curve.point2, "P2 - Handle", curve.handleColor, 0.11f);
        DrawPoint(curve, curve.point3, "P3 - End", Color.white, 0.13f);
    }

    private void DrawPoint(
        BezierCurve curve,
        Transform point,
        string label,
        Color color,
        float size)
    {
        if (point == null)
            return;

        Vector3 position = point.position;

        // Scale the handle to make it easy to grab
        float handleSize = HandleUtility.GetHandleSize(position) * size;

        Handles.color = color;

        // Big handle for moving the points
        if (Handles.Button(
            position,
            Quaternion.identity,
            handleSize,
            handleSize * 1.5f,
            Handles.SphereHandleCap))
        {
            Selection.activeTransform = point;
        }

        // Draw the move gizmo when this point is selected.
        if (Selection.activeTransform == point)
        {
            EditorGUI.BeginChangeCheck();

            Vector3 newPosition = Handles.PositionHandle(
                position,
                Quaternion.identity
            );

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(point, "Move Bezier Point");
                point.position = newPosition;
                EditorUtility.SetDirty(point);
            }
        }

        Handles.color = Color.white;
        Handles.Label(
            position + Vector3.up * handleSize * 1.5f,
            label
        );
    }
}
