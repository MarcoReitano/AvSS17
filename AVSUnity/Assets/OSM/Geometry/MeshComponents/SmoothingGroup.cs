using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SmoothingGroup
{
    public SmoothingGroup()
    { }
    public SmoothingGroup(ITriangleSurface tS)
    {
        memberTriangles.AddRange(tS.Triangles);

        for (int i = 0; i < memberTriangles.Count; i++)
        {
            Vertex[] vertices = tS.Triangles[i].Vertices;
            for (int j = 0; j < 3; j++)
            {
                if (!vertexTriangles.ContainsKey(vertices[j]))
                    vertexTriangles.Add(vertices[j], new List<Triangle>());
                vertexTriangles[vertices[j]].Add(tS.Triangles[i]);
            }  
            memberTriangles[i].AddSmoothingGroup(this);
        }
    }
    public void Add(IEnumerable<Triangle> triangles)
    {
        memberTriangles.AddRange(triangles);
        foreach(Triangle t in triangles)
        {
            Vertex[] vertices = t.Vertices;
            for (int j = 0; j < 3; j++)
            {
                if (!vertexTriangles.ContainsKey(vertices[j]))
                    vertexTriangles.Add(vertices[j], new List<Triangle>());
                vertexTriangles[vertices[j]].Add(t);
            }  

            t.AddSmoothingGroup(this);
        }
    }
    public void Add(Triangle triangle)
    {
        memberTriangles.Add(triangle);

        Vertex[] vertices = triangle.Vertices;
        for (int i = 0; i < 3; i++)
        {
            if (!vertexTriangles.ContainsKey(vertices[i]))
                vertexTriangles.Add(vertices[i], new List<Triangle>());
            vertexTriangles[vertices[i]].Add(triangle);
        }        

        triangle.AddSmoothingGroup(this);
    }
    public void Add(ITriangleSurface tS)
    {
        memberTriangles.AddRange(tS.Triangles);
        for (int i = 0; i < tS.Triangles.Count; i++)
        {
            Vertex[] vertices = tS.Triangles[i].Vertices;
            for (int j = 0; j < 3; j++)
            {
                if (!vertexTriangles.ContainsKey(vertices[j]))
                    vertexTriangles.Add(vertices[j], new List<Triangle>());
                vertexTriangles[vertices[j]].Add(tS.Triangles[i]);
            }  

            tS.Triangles[i].AddSmoothingGroup(this);
        }
    }
    public void Remove(IEnumerable<Triangle> triangles)
    {
        foreach(Triangle t in triangles)
        {
            memberTriangles.Remove(t);

            Vertex[] vertices = t.Vertices;
            for (int j = 0; j < 3; j++)
            {
                if (vertexTriangles.ContainsKey(vertices[j]))
                    vertexTriangles[vertices[j]].Remove(t);
            }  

            t.RemoveSmoothingGroup(this);
        }
    }
    public bool Remove(Triangle triangle)
    {
        triangle.RemoveSmoothingGroup(this);

        Vertex[] vertices = triangle.Vertices;
        for (int j = 0; j < 3; j++)
        {
            if (vertexTriangles.ContainsKey(vertices[j]))
                vertexTriangles[vertices[j]].Remove(triangle);
        } 

        return memberTriangles.Remove(triangle);   
    }

    public Vector3 Normal(Vertex vertex)
    {
        List<Triangle> triangles;
        if (vertexTriangles.ContainsKey(vertex))
        {
            triangles = vertexTriangles[vertex]; 
        }
        else
            return Vector3.zero;

        Vector3 result = Vector3.zero;

        for (int i = 0; i < triangles.Count; i++)
        {
            result += triangles[i].Normal;
        }
        if (triangles.Count > 0)
            result /= triangles.Count;
        return result;
    }

    private List<Triangle> memberTriangles = new List<Triangle>();
    private Dictionary<Vertex, List<Triangle>> vertexTriangles = new Dictionary<Vertex, List<Triangle>>();
}
