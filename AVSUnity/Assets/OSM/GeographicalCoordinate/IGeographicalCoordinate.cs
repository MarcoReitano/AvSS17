using UnityEngine;
#if UNITY_EDITOR
//using UnityEditor;
#endif
using System.Collections;

/// <summary>
/// IGeographicalCoordinate.
/// </summary>
public interface IGeographicalCoordinate
{
    float Longitude{get; set;}
    float Latitude{get; set;}
}
