using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Quad.
/// </summary>
public class Quad : ITriangleSurface, IQuadSurface
{
    //1--2
    //| /|
    //|/ |
    //0--3
    //Clockwise!
    public Quad(Vertex[] vertices)
    {
        triangles[0] = new Triangle(vertices[0], vertices[1], vertices[2]);
        triangles[1] = new Triangle(vertices[2], vertices[3], vertices[0]);
    }
    public Quad(Vertex[] vertices, ModularMesh mesh, Material material)
    {
        triangles[0] = new Triangle(vertices[0], vertices[1], vertices[2], mesh, material);
        triangles[1] = new Triangle(vertices[2], vertices[3], vertices[0], mesh, material);
    }

    public Quad(Vertex v0, Vertex v1, Vertex v2, Vertex v3)
    {
        triangles[0] = new Triangle(v0, v1, v2);
        triangles[1] = new Triangle(v2, v3, v0);
    }
    public Quad(Vertex v0, Vertex v1, Vertex v2, Vertex v3, ModularMesh mesh, Material material)
    {
        triangles[0] = new Triangle(v0, v1, v2, mesh, material);
        triangles[1] = new Triangle(v2, v3, v0, mesh, material);
    }

    public List<Quad> Quads
    {
        get { return new List<Quad>() { this }; }
    }
    public List<Triangle> Triangles
    {
        get { return new List<Triangle>(triangles); }
    }
    [SerializeField][HideInInspector]
    private Triangle[] triangles = new Triangle[2];

    public void AddToMesh(ModularMesh mesh, Material m)
    {
            triangles[0].AddToMesh(mesh, m);
            triangles[1].AddToMesh(mesh, m);
    }

    public void RemoveFromMesh(ModularMesh mesh, Material m)
    {
            triangles[0].RemoveFromMesh(mesh, m);
            triangles[1].RemoveFromMesh(mesh, m);
    }

    public void AddSmoothingGroup(SmoothingGroup sG)
    {
        triangles[0].AddSmoothingGroup(sG);
    }

    public void RemoveSmoothingGroup(SmoothingGroup sG)
    {
        triangles[0].RemoveSmoothingGroup(sG);
    }

    public void OnDrawGizmos()
    {
        triangles[0].OnDrawGizmos();
        triangles[1].OnDrawGizmos();

        Gizmos.color = Color.green;

        Gizmos.DrawLine(triangles[0].Vertices[0], triangles[0].Vertices[1]);
        Gizmos.DrawLine(triangles[0].Vertices[1], triangles[0].Vertices[2]);

        Gizmos.DrawLine(triangles[1].Vertices[0], triangles[1].Vertices[1]);
        Gizmos.DrawLine(triangles[1].Vertices[1], triangles[1].Vertices[2]);
    }
}
