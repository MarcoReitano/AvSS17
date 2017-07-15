using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class GizmoUtils
{
    public static void DrawPathGizmo(List<Vector3> points)
    {
        if (points != null)
        {
            if (points.Count > 1)
            {
                Gizmos.color = Color.gray;
                Vector3 previousPoint = points[0];

                for (int i = 1; i < points.Count; i++)
                {
                    Vector3 currentPoint = points[i];
                    Gizmos.DrawLine(currentPoint, previousPoint);
                    previousPoint = currentPoint;
                }
            }
        }
    }

}

