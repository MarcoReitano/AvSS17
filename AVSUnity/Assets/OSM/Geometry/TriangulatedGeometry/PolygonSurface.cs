using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PolygonSurface : ITriangleSurface
{
    public PolygonSurface()
    { }
    public PolygonSurface(IEnumerable<Triangle> triangles)
    {
        this.triangles.AddRange(triangles);
    }
    public PolygonSurface(ITriangleSurface tS)
    {
        this.triangles.AddRange(tS.Triangles);

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

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        for (int i = 0; i < triangles.Count; i++)
        {
            Vertex[] triangleVerices = triangles[i].Vertices;

            Gizmos.DrawLine(triangleVerices[0].Position, triangleVerices[1].Position);
            Gizmos.DrawLine(triangleVerices[1].Position, triangleVerices[2].Position);
            Gizmos.DrawLine(triangleVerices[2].Position, triangleVerices[0].Position);
        }
    }



}
