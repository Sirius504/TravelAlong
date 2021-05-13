using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SplineBehaviour))]
public class SplineEditor: Editor
{
    private const int stepsPerSegment = 10;

    private SplineBehaviour splineBehaviour;
    private Spline spline;

    private void OnSceneGUI()
    {
        Draw();
    }

    void Draw()
    {
        DrawBezier();
        DrawHandles();
    }

    private void DrawBezier()
    {
        for (int i = 0; i <  spline.SegmentsCount; i++)
        {
            Vector3[] points = spline.GetSegment(i);
            Handles.DrawBezier(points[0], points[3], points[1], points[2], Color.green, null, 2f);
            Handles.color = Color.black;
            Handles.DrawLine(points[0], points[1]);
            Handles.DrawLine(points[3], points[2]);
        }
    }

    private void DrawHandles()
    {
        Handles.color = Color.red;
        for (int i = 0; i < spline.PointsCount; i++)
        {
            Vector3 newPos = Handles.FreeMoveHandle(spline[i], Quaternion.identity, .1f, Vector3.zero, Handles.SphereHandleCap);
            if (spline[i] != newPos)
            {
                Undo.RecordObject(splineBehaviour, "Move point");
                spline.MovePoint(i, newPos);
            }
        }
    }

    private void OnEnable()
    {
        splineBehaviour = target as SplineBehaviour;
        if (splineBehaviour.Spline == null)
            splineBehaviour.CreateSpline();
        spline = splineBehaviour.Spline;
    }
}
