using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// QuadStrip.
/// </summary>
public class QuadStrip : ITriangleSurface, IQuadSurface
{
    public QuadStrip(Polyline poly1, Polyline poly2, ModularMesh mesh, Material material)
    {
        for (int i = 0; i < Mathf.Min(poly1.Count, poly2.Count) - 1; i++)
        {
            Quad newQuad = new Quad(poly1[i], poly2[i], poly2[i + 1], poly1[i + 1], mesh, material);
            quads.Add(newQuad);

            triangles.AddRange(newQuad.Triangles);
        }
    }
    public QuadStrip(Polygon poly1, Polygon poly2, ModularMesh mesh, Material material)
    {
        for (int i = 0; i < Mathf.Min(poly1.Count, poly2.Count) - 1; i++)
        {
            Quad newQuad = new Quad(poly1[i], poly2[i], poly2[i + 1], poly1[i + 1], mesh, material);
            quads.Add(newQuad);

            triangles.AddRange(newQuad.Triangles);
        }

        Quad lastNewQuad = new Quad(poly1[poly1.Count - 1], poly2[poly2.Count - 1], poly2[0], poly1[0], mesh, material);
        quads.Add(lastNewQuad);

        triangles.AddRange(lastNewQuad.Triangles);
    }


    public Quad this[int i]
    {
        get
        {
            if (i >= 0 && i < Quads.Count)
                return Quads[i];
            return null;
        }
    }
    public List<Quad> Quads
    {
        get { return quads; }
    }
    [SerializeField]
    [HideInInspector]
    protected List<Quad> quads = new List<Quad>();

    public List<Triangle> Triangles
    {
        get { return triangles; }
    }
    [SerializeField]
    [HideInInspector]
    protected List<Triangle> triangles = new List<Triangle>();

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

    public void OnDrawGizmos()
    {
        for (int i = 0; i < quads.Count; i++)
        {
            quads[i].OnDrawGizmos();
        }
    }
}
