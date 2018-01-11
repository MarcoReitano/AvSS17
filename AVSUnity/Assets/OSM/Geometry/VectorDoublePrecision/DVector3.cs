using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

[System.Serializable]
public struct DVector3
{
    public DVector3(double x, double y, double z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    [SerializeField]
    public double x;
    [SerializeField]
    public double y;
    [SerializeField]
    public double z;

    public double magnitude
    {
        get
        {
            return System.Math.Sqrt(x * x + y * y + z * z);
        }
    }
    public double sqrMagnitude
    {
        get
        {
            return (x * x + y * y + z * z);
        }
    }

    public DVector3 normalized
    {
        get
        {
            double magnitude = this.magnitude;
            return new DVector3(
                this.x / Math.Abs(magnitude),
                this.y / Math.Abs(magnitude),
                this.z / Math.Abs(magnitude));
        }
    }

    public static DVector3 zero
    {
        get { return new DVector3(0D, 0D, 0D); }
    }

    public static DVector3 one
    {
        get { return new DVector3(1D, 1D, 1D); }
    }

    public static DVector3 up
    {
        get { return new DVector3(0D, 1D, 0D); }
    }

    public static DVector3 down
    {
        get { return new DVector3(0D, -1D, 0D); }
    }

    public static DVector3 left
    {
        get { return new DVector3(-1D, 0D, 0D); }
    }

    public static DVector3 right
    {
        get { return new DVector3(1D, 0D, 0D); }
    }

    public static DVector3 forward
    {
        get { return new DVector3(0D, 0D, 1D); }
    }

    public static DVector3 backward
    {
        get { return new DVector3(0D, 0D, -1D); }
    }

    public static double Distance(DVector3 p, DVector3 B)
    {
        return (B - p).magnitude;
    }

    public DVector2 ToDVector2XZ()
    {
        return new DVector2(this.x, this.z);
    }

    public static double Dot(DVector3 v1, DVector3 v2)
    {
        return v1.x * v2.x + v1.y * v2.y + v1.z * v2.z;
    }

    public static DVector3 Cross(DVector3 v1, DVector3 v2)
    {
        return new DVector3(v1.y * v2.z - v1.z * v2.y,
                            v1.z * v2.x - v1.x * v2.z,
                            v1.x * v2.y - v1.y * v2.x);
    }

    public static DVector3 operator *(DVector3 v1, DVector3 v2)
    {
        return new DVector3(v1.x * v2.x, v1.y * v2.y, v1.z * v2.z);
    }
    public static DVector3 operator /(DVector3 v1, DVector3 v2)
    {
        return new DVector3(v1.x / v2.x, v1.y / v2.y, v1.z / v2.z);
    }
    public static DVector3 operator +(DVector3 v1, DVector3 v2)
    {
        return new DVector3(v1.x + v2.x, v1.y + v2.y, v1.z + v2.z);
    }
    public static DVector3 operator -(DVector3 v1, DVector3 v2)
    {
        return new DVector3(v1.x - v2.x, v1.y - v2.y, v1.z - v2.z);
    }

    public static DVector3 operator *(DVector3 v1, double s)
    {
        return new DVector3(v1.x * s, v1.y * s, v1.z * s);
    }
    public static DVector3 operator *(double s, DVector3 v1)
    {
        return new DVector3(v1.x * s, v1.y * s, v1.z * s);
    }

    public static DVector3 operator /(DVector3 v1, double s)
    {
        return new DVector3(v1.x / s, v1.y / s, v1.z / s);
    }

    public static bool operator ==(DVector3 a, DVector3 b)
    {
        // If both are null, or both are same instance, return true.
        if (System.Object.ReferenceEquals(a, b))
            return true;

        // If one is null, but not both, return false.
        if (((object)a == null) || ((object)b == null))
            return false;

        return a.x == b.x && a.y == b.y && a.z == b.z;
    }

    public static bool operator !=(DVector3 a, DVector3 b)
    {
        return !(a == b);
    }
    
    public static explicit operator DVector3(Vector3 v)
    {
        return new DVector3((double)v.x, (double)v.y, (double)v.z);
    }
    public static explicit operator Vector3(DVector3 dV) 
    {
        return new Vector3((float)dV.x, (float)dV.y, (float)dV.z);
    }
    
    //Cast to DVector2 (drops y component of the vector)
    public static implicit operator DVector2(DVector3 v)
    {
        return new DVector2(v.x, v.z);
    }
    
    public override bool Equals(System.Object obj)
    {
        // If parameter is null return false.
        if (obj == null)
        {
            return false;
        }

        //// If parameter cannot be cast to Point return false.
        DVector3 p = (DVector3) obj;
        //if ((System.Object)p == null)
        //{
        //    return false;
        //}

        // Return true if the fields match:
        return (x == p.x) && (y == p.y) && (z == p.z);
    }

    public bool Equals(DVector3 p)
    {
        // If parameter is null return false:
        if ((object)p == null)
        {
            return false;
        }

        // Return true if the fields match:
        return (x == p.x) && (y == p.y) && (z == p.z);
    }

    public override int GetHashCode()
    {
        // TODO: What is a good hash!?
        // http://www.partow.net/programming/hashfunctions/index.html#top
        return (int)(x * 10000) ^ (int)(y * 10000) ^ (int)(z * 10000); 
    }


    public static void OrthoNormalize(ref DVector3 normal, ref DVector3 biNormal)
    {
        // TODO: Implement --> not using Casts
        Vector3 norm = (Vector3)normal;
        Vector3 biNorm = (Vector3)biNormal;
        
        Vector3.OrthoNormalize(ref norm, ref biNorm);
       
        normal = (DVector3)norm;
        biNormal = (DVector3)biNorm;
    }

    public static void OrthoNormalize(ref DVector3 normal, ref DVector3 biNormal, ref DVector3 triNormal)
    {
        // TODO: Implement --> not using Casts
        Vector3 norm = (Vector3)normal;
        Vector3 biNorm = (Vector3)biNormal;
        Vector3 triNorm = (Vector3)triNormal;
        
        Vector3.OrthoNormalize(ref norm, ref biNorm, ref triNorm);

        normal = (DVector3)norm;
        biNormal = (DVector3)biNorm;
        triNormal = (DVector3)triNorm;
    }

    public override string ToString()
    {
        return "(" + x + ", " + y + ")";
    }
}
