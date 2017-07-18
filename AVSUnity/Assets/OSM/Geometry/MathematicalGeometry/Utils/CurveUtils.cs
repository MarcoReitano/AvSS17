using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CurveUtils {

    public static void DrawCurve(List<Vector3> points)
    {
        for (int i = 0; i < points.Count - 1; i++)
        {
            Gizmos.DrawLine(points[i], points[i + 1]);
        }
    }

    public static void DrawCurve(List<Vertex> points)
    {
        for (int i = 0; i < points.Count - 1; i++)
        {
            Gizmos.DrawLine(points[i], points[i + 1]);
        }
    }

    

}
