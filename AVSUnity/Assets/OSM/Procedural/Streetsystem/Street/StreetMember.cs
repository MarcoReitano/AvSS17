using UnityEngine;
#if UNITY_EDITOR
//using UnityEditor;
#endif
using System.Collections;
using System;

/// <summary>
/// StreetMember.
/// </summary>
public interface IStreetMember : IProceduralObjects
{
    Type Type { get; }
    Polyline LeftOutline { get; set; }
    Polyline RightOutline { get; }
    float StartParameter { get; set; }
    float EndParameter { get; set; }
}
