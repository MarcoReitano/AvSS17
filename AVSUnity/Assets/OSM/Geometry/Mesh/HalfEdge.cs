using UnityEngine;
#if UNITY_EDITOR
//using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// HalfEdge.
/// </summary>
[System.Serializable]
public class HalfEdge
{
    public HalfEdge(Vertex vertex, Triangle triangle)
    {
        this.vertex = vertex;
        vertex.AddHalfEdge(this);
        this.left = triangle;
    }

    public Vertex Vertex
    {
        get { return vertex; }
        set { vertex = value; }
    }
    public HalfEdge Next
    {
        get { return next; }
        set 
        {
            if (value == null)
                Debug.Log("Next.Set null");
            next = value;
            next.prev = this;

            pair = next.Vertex.GetHalfEdgeToVertex(vertex);
        }
    }
    public HalfEdge Prev
    {
        get { return prev; }
    }

    public HalfEdge Pair
    {
        get { return pair; }
        set { pair = value; }
    }
    public Triangle Left
    {
        get { return left; }
        set { Left = value; }
    }

    private Vertex vertex;
    private HalfEdge next;
    private HalfEdge prev;

    private HalfEdge pair;
    private Triangle left;

    public static implicit operator Vector3(HalfEdge edge)
    {
        return edge.next.vertex - edge.vertex;
    }

    public void Split(Vertex p)
    {
        //Split leftTriangle
        //Split pair
    }

    public void Delete()
    {
        vertex.RemoveHalfEdge(this);

        vertex = null;
        next = null;
        prev = null;

        pair = null;
        left = null;
    }
}
