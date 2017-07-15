using UnityEngine;
using System.Collections;

public class Bezier
{
    public static Vector3 Point(float t, Vector3 P0, Vector3 P1, Vector3 P2, Vector3 P3)
    {
        //Without Optimization
        //		return Mathf.Pow(1 - t,3) * P0 
        //				+ 3*t*Mathf.Pow(1-t, 2) * P1 
        //				+ 3* Mathf.Pow(t,2) * (1-t) * P2 
        //				+ Mathf.Pow(t,3) * P3;

        //Optimized
        float a = 1 - t;
        float b = a * a;
        float c = b * a;
        float d = t * t;
        float e = d * t;
        return c * P0 + 3 * t * b * P1 + 3 * d * a * P2 + e * P3;

        //Test
        //return Mathf.Pow(t,3) * (3 * P1 - P0 - 3 * P2 + P3) + Mathf.Pow(t,2) * (3 * P2 - 6 * P1 + 3 * P0) + (3 * P1 - 3 * P0) * t + P0;
    }
}
