using UnityEngine;
#if UNITY_EDITOR
//using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Vertex.
/// </summary>
public class Vertex
{
	//Testcomment for git presentation
    public Vertex(Vector3 position)
    {
        this.position = position;
    }
    public Vertex(GameObject gameObject)
    {
        this.position = gameObject.transform.position;
    }
    public Vertex(Transform transform)
    {
        this.position = transform.position;
    }

    public Vector2 Position2D
    {
        get { return new Vector2(position.x, position.z); }
    }
    public Vector3 Position
    {
        get { return position; }
        set { position = value; }
    }
    public float X
    {
        get { return position.x; }
    }
    public float Y
    {
        get { return position.y; }
    }
    public float Z
    {
        get { return position.z; }
    }

    private Vector3 position;

    private List<HalfEdge> edges = new List<HalfEdge>();
    private List<Triangle> triangles = new List<Triangle>();

    public void AddHalfEdge(HalfEdge edge)
    {
        edges.Add(edge);
        triangles.Add(edge.Left);
    }
    public void RemoveHalfEdge(HalfEdge edge)
    {
        edges.Remove(edge);
        triangles.Remove(edge.Left);
    }

    public HalfEdge GetHalfEdgeToVertex(Vertex vertex)
    {
        for (int i = 0; i < edges.Count; i++)
        {
            if (edges[i].Next != null)
            {
                if (edges[i].Next.Vertex == vertex)
                    return edges[i];
            }
        }
        return null;
    }
    public List<Triangle> Triangle
    {
        get
        {
            return triangles;
        }
    }

    public static implicit operator Vector3(Vertex v)
    {
        return v.Position;
    }
    public static implicit operator Vector2(Vertex v)
    {
        return new Vector2(v.position.x, v.position.z);
    }
    public static implicit operator bool(Vertex exists)
    {
        if (exists != null)
            return true;
        return false;
    }

    public static Vector3 operator +(Vertex v1, Vertex v2)
    {
        return v1.Position + v2.Position;
    }
    public static Vector3 operator -(Vertex v1, Vertex v2)
    {
        return v1.Position - v2.Position;
    }

}
