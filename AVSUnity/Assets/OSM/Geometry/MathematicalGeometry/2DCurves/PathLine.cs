using UnityEngine;
using System.Collections;

public class PathLine : AbstractPathCurve
{
    public PathLine(Vector3 start, Vector3 end)
    {
        this.Points.Add(start);
        this.Points.Add(end);
        
        this.ControlPoints.Add(start);
        this.ControlPoints.Add(end);
    }

    public PathLine(Vector3 start, Vector3 direction, float length)
    {
        this.Points.Add(start);
        this.Points.Add(start+direction*length);

        this.ControlPoints.Add(start);
        this.ControlPoints.Add(start + direction * length);
    }

    //public PathLine(Vector3 start, Vector3 direction)
    //{
    //    this.Points.Add(start);
    //    this.Points.Add(start + direction);

    //    this.ControlPoints.Add(start);
    //    this.ControlPoints.Add(start + direction);
    //}


    public override Vector3 StartDirection
    {
        get { return this.StartPoint- this.EndPoint; }
    }

    public override Vector3 EndDirection
    {
        get { return this.StartPoint - this.EndPoint; }
    }
}
