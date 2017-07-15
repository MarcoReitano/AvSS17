using UnityEngine;
#if UNITY_EDITOR
//using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// HexagonGrid.
/// </summary>
public class HexagonGrid : ITriangleSurface
{
//    private Vector3 origin;
//    private Quaternion rotation;
//
//    private float height;
//    private float width;
//
//
//    public Vertex[] HexVertices(int indexI, int indexJ)
//    {
//        Vector3 hexMid = indexI * width * 2 * Vector3.right
//                        + indexYJ
//        return null;
//    }

    public List<Triangle> Triangles
    {
        get { return triangles; }
    }
    [SerializeField][HideInInspector]
    protected List<Triangle> triangles = new List<Triangle>();

    public virtual void OnDrawGizmos()
    {
        if (triangles != null)
        {
            for (int i = 0; i < triangles.Count; i++)
            {
                triangles[i].OnDrawGizmos();
            }
        }
    }

    public void AddToMesh(ModularMesh mesh, Material m)
    {
        for (int i = 0; i < triangles.Count; i++)
            triangles[i].AddToMesh(mesh, m);
    }

    public void RemoveFromMesh(ModularMesh mesh, Material m)
    {
        for (int i = 0; i < triangles.Count; i++)
            triangles[i].RemoveFromMesh(mesh, m);
    }

    public void AddSmoothingGroup(SmoothingGroup sG)
    {
        for (int i = 0; i < triangles.Count; i++)
        {
            triangles[i].AddSmoothingGroup(sG);
        }
    }
    public void RemoveSmoothingGroup(SmoothingGroup sG)
    {
        for (int i = 0; i < triangles.Count; i++)
        {
            triangles[i].RemoveSmoothingGroup(sG);
        }
    }
}
