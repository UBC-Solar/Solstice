
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BezierSpline), true)]
public class BezierSplineEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        BezierSpline spline = (BezierSpline)target;

        EditorGUILayout.Space();

        if (GUILayout.Button("Extend Spline"))
        {
            BezierCurve curve = spline.ExtendSpline();
        }

        if (spline is Track track)
        {
            if (GUILayout.Button("Enforce G1 Continuity"))
            {
                track.EnforceG1();

                EditorUtility.SetDirty(track);
                SceneView.RepaintAll();
            }
        }
    }
}

