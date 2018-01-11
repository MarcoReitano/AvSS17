using UnityEngine;
using System.Collections;

[System.Serializable]
public struct DVector2
{
    public DVector2(double x, double y)
    {
        this.x = x;
        this.y = y;
    }

    [SerializeField]
    public double x;
    [SerializeField]
    public double y;

    public double Magnitude
    {
        get
        {
            return System.Math.Sqrt(x * x + y * y);
        }
    }
    public double SqrMagnitude
    {
        get
        {
            return (x * x + y * y);
        }
    }

    public DVector3 ToXZPlaneDVector3()
    {
        return new DVector3(this.x, 0d, this.y);
    }

    public static double Dot(DVector2 v1, DVector2 v2)
    {
        return v1.x * v2.x + v1.y * v2.y;
    }

    public static DVector2 operator *(DVector2 v1, DVector2 v2)
    {
        return new DVector2(v1.x * v2.x, v1.y * v2.y);
    }
    public static DVector2 operator /(DVector2 v1, DVector2 v2)
    {
        return new DVector2(v1.x / v2.x, v1.y / v2.y);
    }
    public static DVector2 operator +(DVector2 v1, DVector2 v2)
    {
        return new DVector2(v1.x + v2.x, v1.y + v2.y);
    }
    public static DVector2 operator -(DVector2 v1, DVector2 v2)
    {
        return new DVector2(v1.x - v2.x, v1.y - v2.y);
    }

    public static DVector2 operator *(DVector2 v1, double s)
    {
        return new DVector2(v1.x * s, v1.y * s);
    }
    public static DVector2 operator *(double s, DVector2 v1)
    {
        return new DVector2(v1.x * s, v1.y * s);
    }

    public static implicit operator DVector2(Vector2 v)
    {
        return new DVector2((double)v.x, (double)v.y);
    }
    public static explicit operator Vector2(DVector2 dV)
    {
        return new Vector3((float)dV.x, (float)dV.y);
    }

    //Cast to DVector3 (x -> v.x, y -> 0d, z -> v.y)
    public static implicit operator DVector3(DVector2 v)
    {
        return new DVector3(v.x, 0d, v.y);
    }

    public override string ToString()
    {
        return "(" + x + ", " + y + ")";
    }
}
