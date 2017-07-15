using UnityEngine;
#if UNITY_EDITOR
//using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// ITriangleSurface.
/// </summary>
public interface ITriangleSurface
{
    List<Triangle> Triangles { get; }
    void AddToMesh(ModularMesh mesh, Material m);
    void RemoveFromMesh(ModularMesh mesh, Material m);
    void AddSmoothingGroup(SmoothingGroup sG);
    void RemoveSmoothingGroup(SmoothingGroup sG);
}
