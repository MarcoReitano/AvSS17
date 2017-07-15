using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Triangle.
/// </summary>
[System.Serializable]
public class Triangle: ITriangleSurface
{
    public Triangle(Vertex v1, Vertex v2, Vertex v3)
    {
        edges = new HalfEdge[3];

        edges[0] = new HalfEdge(v1, this);
        edges[1] = new HalfEdge(v2, this);
        edges[2] = new HalfEdge(v3, this);

        edges[0].Next = edges[1];
        edges[1].Next = edges[2];
        edges[2].Next = edges[0];
    }
    public Triangle(Vertex v1, Vertex v2, Vertex v3, ModularMesh mesh, Material material)
    {
        edges = new HalfEdge[3];

        edges[0] = new HalfEdge(v1, this);
        edges[1] = new HalfEdge(v2, this);
        edges[2] = new HalfEdge(v3, this);

        edges[0].Next = edges[1];
        edges[1].Next = edges[2];
        edges[2].Next = edges[0];

        AddToMesh(mesh, material);
    }


    public HalfEdge[] Edges
    {
        get { return edges; }
        set { edges = value; }
    }
    public List<SmoothingGroup> SmoothingGroups
    {
        get { return smoothingGroups; }
    }
    public List<MeshParticipant> Meshes
    {
        get { return meshes; }
    }

    private HalfEdge[] edges;
    private List<SmoothingGroup> smoothingGroups = new List<SmoothingGroup>();
    private List<MeshParticipant> meshes = new List<MeshParticipant>();

    public Vertex this[int i]
    {
        get
        {
            if (i >= 0 && i < 2)
                return edges[i].Vertex;
            return null;
        }
    }
    public Vertex[] Vertices
    {
        get 
        {
            return new Vertex[] { edges[0].Vertex, edges[1].Vertex, edges[2].Vertex };
        }
    }
    
    public Vector3 Normal
    {
        get
        {
            return Vector3.Cross(edges[0], edges[1]).normalized;
        }
    }
    public Vector3 Centroid
    {
        get 
        {
            return (edges[0].Vertex + edges[1].Vertex + edges[2].Vertex)/3;
        }
    }
    public bool IsClockwise()
    {
        return Math2D.IsClockwise(Vertices);
    }

    public void AddToMesh(ModularMesh mesh, Material m)
    {
        if (mesh != null && edges[0].Vertex && edges[1].Vertex && edges[2].Vertex)
        {
            mesh.AddTriangle(this, m);
            meshes.Add(new MeshParticipant(mesh, m));
            //meshes.Add(mesh);
        }
        //else
            //Debug.Log("Triangle has null vertices and will not be added to its Mesh");
    }
    public void RemoveFromMesh(ModularMesh mesh, Material m)
    {
        mesh.RemoveTriangle(this, m);
    }
    public void RemoveFromAllMeshes()
    {
        for (int i = meshes.Count-1; i >= 0; i--)
        {
            Debug.Log(i);
            meshes[i].Mesh.RemoveTriangle(this);
        }
    }

    public void AddSmoothingGroup(SmoothingGroup sG)
    {
        if (!smoothingGroups.Contains(sG))
        {
            smoothingGroups.Add(sG);
            sG.Add(this);
        }
    }
    public void RemoveSmoothingGroup(SmoothingGroup sG)
    {
        smoothingGroups.Remove(sG);
        sG.Remove(this);
    }

    public void OnDrawGizmos()
    {
        if (edges != null)
        {
            Gizmos.DrawLine(edges[0].Vertex, edges[1].Vertex);
            Gizmos.DrawLine(edges[1].Vertex, edges[2].Vertex);
            Gizmos.DrawLine(edges[2].Vertex, edges[0].Vertex);

            Gizmos.DrawLine(Centroid, Centroid + Normal);            
        }
    }

    public List<Triangle> Triangles
    {
        get { return new List<Triangle>() { this }; }
    }

    public bool IsPointInTriangle2DXZ(Vertex p)
    {
        Vertex p0 = edges[0].Vertex;
        Vertex p1 = edges[1].Vertex;
        Vertex p2 = edges[2].Vertex;

        float area = 0.5f *(-p1.Z*p2.X + p0.Z*(-p1.X + p2.X) + p0.X*(p1.Z - p2.Z) + p1.X*p2.Z);

        float s = 1 / (2 * area) * (p0.X * p1.Z - p0.Z * p1.X + (p0.Z - p1.Z) * p.X + (p1.X - p0.X) * p.Z);
        float t = 1 / (2 * area) * (p1.X * p2.Z - p1.Z * p2.X + (p1.Z - p2.Z) * p.X + (p2.X - p1.X) * p.Z);
        float u = 1 / (2 * area) * (p2.Z * p0.X - p2.X * p0.Z + (p2.Z - p0.Z) * p.X + (p0.X - p2.X) * p.Z);

        //UnityEngine.Debug.Log("s: " + s + "\n" + "t: " + t + "\n" + "u: " + u);
        return (s >= 0 && t >= 0 && 1 - s - t >= 0);
    }

    public bool IsPointInTriangle2DXZ(Vertex p, out float s, out float t, out float u)
    {
        Vertex p0 = edges[0].Vertex;
        Vertex p1 = edges[1].Vertex;
        Vertex p2 = edges[2].Vertex;

        float area = 0.5f * (-p1.Z * p2.X + p0.Z * (-p1.X + p2.X) + p0.X * (p1.Z - p2.Z) + p1.X * p2.Z);

        s = 1 / (2 * area) * (p0.X * p1.Z - p0.Z * p1.X + (p0.Z - p1.Z) * p.X + (p1.X - p0.X) * p.Z);
        t = 1 / (2 * area) * (p1.X * p2.Z - p1.Z * p2.X + (p1.Z - p2.Z) * p.X + (p2.X - p1.X) * p.Z);
        u = 1 / (2 * area) * (p2.Z * p0.X - p2.X * p0.Z + (p2.Z - p0.Z) * p.X + (p0.X - p2.X) * p.Z);

        //UnityEngine.Debug.Log("s: " + s + "\n" + "t: " + t + "\n" + "u: " + u);
        return (s >= 0 && t >= 0 && 1 - s - t >= 0);
    }

    public List<Triangle> Split2DXZ(Vertex p)
    {
        bool didSplit;
        return Split2DXZ(p, out didSplit);
    }
    public List<Triangle> Split2DXZ(Vertex p, out bool didSplit)
    {
        float s;
        float t;
        float u; 
        // Point in not inside Triangle, return this instance
        if (!IsPointInTriangle2DXZ(p, out s, out t, out u))
        {
            didSplit = false;
            return new List<Triangle>() { this };
        }

        // Is Point equivalent to one of the Trianglecorners? return this instance then
        if ((s == 0 && t == 0) || (s == 0 && t == 1) || (s == 1 && t == 0))
        {
            didSplit = false;
            return new List<Triangle>() { this };
        }
        
        //Create new Triangles
        List<Triangle> result = new List<Triangle>();

            //is on edge -> split into 2 new triangles
        if (s == 0)
        {
            result.Add(new Triangle(edges[2].Vertex, edges[0].Vertex, p));
            result.Add(new Triangle(edges[1].Vertex, edges[2].Vertex, p));
        }
            //p is on edge -> split into 2 new triangles
        else if (t == 0)
        {
            result.Add(new Triangle(edges[0].Vertex, edges[1].Vertex, p));
            result.Add(new Triangle(edges[2].Vertex, edges[0].Vertex, p));
        }
            //p is on edge -> split into 2 new triangles
        else if (u == 0)
        {
            result.Add(new Triangle(edges[0].Vertex, edges[1].Vertex, p));
            result.Add(new Triangle(edges[1].Vertex, edges[2].Vertex, p));
        }
            //p is fully inside triangle -> split into 3 new triangles
        else
        {
            result.Add(new Triangle(edges[0].Vertex, edges[1].Vertex, p));
            result.Add(new Triangle(edges[1].Vertex, edges[2].Vertex, p));
            result.Add(new Triangle(edges[2].Vertex, edges[0].Vertex, p));
        }
        this.Delete();
        didSplit = true;
        return result;
    }

    public void Delete()
    {
        for (int i = smoothingGroups.Count-1; i >= 0; i--)
        {
            RemoveSmoothingGroup(smoothingGroups[i]);
        }
        RemoveFromAllMeshes();

        edges[0].Delete();
        edges[1].Delete();
        edges[2].Delete();
    }

    public struct MeshParticipant
    {
        public MeshParticipant(ModularMesh mesh, Material material)
        {
            this.Mesh = mesh;
            this.Material = material;
        }
        public ModularMesh Mesh;
        public Material Material;
    }
}
