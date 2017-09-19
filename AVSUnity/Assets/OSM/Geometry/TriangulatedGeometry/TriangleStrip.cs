using UnityEngine;
#if UNITY_EDITOR
//using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// TriangleStrip.
/// </summary>
[System.Serializable]
public class TriangleStrip : ITriangleSurface
{
    public TriangleStrip(Polyline poly1, Polyline poly2, ModularMesh mesh, Material material)
    {
        for (int i = 0; i < Mathf.Min(poly1.Count, poly2.Count) - 1; i++)
        {
            triangles.Add(new Triangle(poly1[i], poly2[i], poly1[i + 1], mesh, material));
            triangles.Add(new Triangle(poly1[i + 1], poly2[i], poly2[i + 1], mesh, material));
        }
    }
    public TriangleStrip(Polygon poly1, Polygon poly2, ModularMesh mesh, Material material)
    {
        for (int i = 0; i < Mathf.Min(poly1.Count, poly2.Count) - 1; i++)
        {
            triangles.Add(new Triangle(poly1[i], poly2[i], poly1[i + 1], mesh, material));
            triangles.Add(new Triangle(poly1[i + 1], poly2[i], poly2[i + 1], mesh, material));
        }

        triangles.Add(new Triangle(poly1[poly1.Count - 1], poly2[poly2.Count - 1], poly1[0], mesh, material));
        triangles.Add(new Triangle(poly1[0], poly2[poly2.Count - 1], poly2[0], mesh, material));
    }

    public List<Triangle> Triangles
    {
        get { return triangles; }
    }
    private List<Triangle> triangles = new List<Triangle>();

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
