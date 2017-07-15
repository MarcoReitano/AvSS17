using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public abstract class AbstractPathCurve
{
    private List<Vector3> points = new List<Vector3>();
    public List<Vector3> Points
    {
        get { return points; }
        set { points = value; }
    }

    private List<Vector3> controlPoints = new List<Vector3>();
    public List<Vector3> ControlPoints
    {
        get { return controlPoints; }
        set { controlPoints = value; }
    }

    public Vector3 StartPoint
    {
        get
        {
            if (points.Count > 0)
                return points[0];
            return Vector3.zero;
        }
    }

    public abstract Vector3 StartDirection
    {
        get;
    }

    public Vector3 EndPoint
    {
        get
        {
            if (points.Count > 0)
                return points[points.Count - 1];
            return Vector3.zero;
        }
    }

    public abstract Vector3 EndDirection
    {
        get;
    }

}

