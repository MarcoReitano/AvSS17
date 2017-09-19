using UnityEngine;
#if UNITY_EDITOR
//using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Extensions.
/// </summary>
public static class Extensions
{
    public static Vector2 ToXZVector2(this Vector3 v)
    {
        return new Vector2(v.x, v.z);
    }	
	public static Vector3 ToXZPlaneVector3(this Vector3 v)
	{
		return new Vector3(v.x, 0f, v.z);
	}

    public static Vector3 ToXZVector3(this Vector2 v)
    {
        return new Vector3(v.x, 0f, v.y);
    }
    public static Vector3 ToXZVector3(this Vector2 v, float y)
    {
        return new Vector3(v.x, y, v.y);
    }

    public static Vector2 NormalVector(this Vector2 v)
    {
        return new Vector2(v.y, -v.x).normalized;
    }

    public static T GetOrAddComponent<T>(this GameObject gO) where T : Component
    {
        T component = gO.GetComponent<T>();
        if (component != null)
            return component;
        else
            return gO.AddComponent<T>();
    }
	
	
	public static Vector2 OrientationToVector2(this float f)
	{
		return new Vector2(Mathf.Sin(f), Mathf.Cos(f));
	}

    public static Vector3 OrientationToVector3(this float f)
    {
        return new Vector3(Mathf.Sin(f), 0f, Mathf.Cos(f));
    }

    public static float ToOrientation(this Vector2 v)
    {
        return Mathf.Atan2(v.x, v.y);
    }

    public static float ToOrientation(this Vector3 v)
    {
        return Mathf.Atan2(v.x, v.z);
    }

    public static float RandomBinomial()
    {
        return Random.Range(0f, 1f) - Random.Range(0f, 1f);
    }

    public static float MapToRotationRange(this float f)
    {
		if(f > Mathf.PI)
		{
			while(f > Mathf.PI)
				f -= Mathf.PI * 2;
		}
		else if (f < -Mathf.PI)
		{
			while(f < -Mathf.PI)
				f += Mathf.PI * 2;
		}
		return f;
    }

    public static T RandomElement<T>(this List<T> list)
    {
        if (list.Count == 0)
            return default(T);
        return list[Random.Range(0, list.Count - 1)];
    }

    public static Vector2 Rotate(this Vector2 v, float angle)
    {
        return (Quaternion.AngleAxis(angle, Vector3.up) * v.ToXZVector3()).ToXZVector2();
    }

    public static double Clamp(this double value, double minValue, double maxValue)
    {
        return System.Math.Min(System.Math.Max(value, minValue), maxValue);
    }
    
    public static int Clamp(this int value, int minValue, int maxValue)
    {
        return System.Math.Min(System.Math.Max(value, minValue), maxValue);
    }

    public static List<GameObject> GetAllChildGOs(this Transform trans)
    {
        List<GameObject> result = new List<GameObject>();
        for (int i = 0; i < trans.childCount; i++)
        {
            result.Add(trans.GetChild(i).gameObject);
        }
        return result;
    }
    public static List<Transform> GetAllChildTransforms(this Transform trans)
    {
        List<Transform> result = new List<Transform>();
        for (int i = 0; i < trans.childCount; i++)
        {
            result.Add(trans.GetChild(i));
        }
        return result;
    }
}
