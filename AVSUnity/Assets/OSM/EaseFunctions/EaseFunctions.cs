using UnityEngine;
#if UNITY_EDITOR
//using UnityEditor;
#endif
using System.Collections;

/// <summary>
/// EaseFunctions.
/// </summary>
public static class EaseFunctions
{
    public static IEaseFunction Linear{get { return null; }}
}

public interface IEaseFunction
{
    float GetValue(float t);
}

public class EaseLinear : IEaseFunction
{
    public float GetValue(float t)
    {
        return Mathf.Clamp01(t);
    }
}
