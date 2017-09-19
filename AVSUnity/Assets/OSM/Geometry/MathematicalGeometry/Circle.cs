using UnityEngine;
#if UNITY_EDITOR
//using UnityEditor;
#endif
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;

/// <summary>
/// Circle.
/// </summary>

public class Circle : Polygon
{
    public Circle(Vertex mid, float radius)
    {
        this.mid = mid;
        this.radius = radius;
        this.normal = Vector3.up;
    }
    public Circle(Vertex mid, float radius, Vector3 normal)
    {
        this.mid = mid;
        this.radius = radius;
        this.normal = normal; 
    }

    public Vertex Mid
    {
        get { return mid; }
        set { mid = value; }
    }
    public float Radius
    {
        get { return radius; }
        set { radius = value; }
    }
    public Vector3 Normal
    {
        get { return normal; }
        set { normal = value; }
    }

    private Vertex mid;
    private float radius;
    private Vector3 normal;

    private int segmentCount = 8;

    private void calculateVertices()
    {
        vertices.Clear();

        float delta = 2 * Mathf.PI / segmentCount;
        float x;
        float y;
        float radians;

        for (int i = 0; i < segmentCount; i++)
        {
            radians = i * delta;

            x = radius * Mathf.Cos(radians);
            y = radius * Mathf.Sin(radians);
        }
    }
}
