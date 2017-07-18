using UnityEngine;
#if UNITY_EDITOR
//using UnityEditor;
#endif
using System.Collections;

/// <summary>
/// SmoothingGroup.
/// </summary>
public abstract class Surface
{
    public abstract Vector3 this[float t, float s] { get; }
}
