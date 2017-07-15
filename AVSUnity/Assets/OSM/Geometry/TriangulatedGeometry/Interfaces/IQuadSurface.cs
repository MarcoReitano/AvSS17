using UnityEngine;
#if UNITY_EDITOR
//using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// IQuadSurface.
/// </summary>
public interface IQuadSurface
{
    List<Quad> Quads { get; }
}
