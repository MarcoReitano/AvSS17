using UnityEngine;
#if UNITY_EDITOR
//using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Spline2D.
/// </summary>
public class Spline2D
{
    public Spline2D()
    {
        splineVertices = new List<Vertex>();
    }
    public Spline2D(IEnumerable<Vertex> vertices)
    {
        splineVertices = new List<Vertex>(vertices);         
    }
    public Spline2D(Polyline polyline)
    {
        splineVertices = new List<Vertex>(polyline.Vertices);
    }

    public List<Vertex> SplineVertices
    {
        get { return splineVertices; }
    }
    private List<Vertex> splineVertices;
}
