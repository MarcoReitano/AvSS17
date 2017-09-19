using UnityEngine;
#if UNITY_EDITOR
//using UnityEditor;
#endif
using System.Collections;

/// <summary>
/// IProceduralObjects.
/// </summary>
public interface IProceduralObjects
{
    void CreateMesh(ModularMesh mesh);
    void UpdateMesh(ModularMesh mesh);
    void Destroy();
}
