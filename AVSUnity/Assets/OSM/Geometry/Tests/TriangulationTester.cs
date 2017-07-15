using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;

public class TriangulationTester : MonoBehaviour 
{
    public int amount = 100;
    public float borders = 100f;

    public List<Vertex> vertices = new List<Vertex>();
    public List<Triangle> triangles = new List<Triangle>();

    public void GenerateRandomPoints()
    {
        vertices.Clear();
        triangles.Clear();

        //Generate BoundingBoxTriangles
        vertices.Add(new Vertex(new Vector3(-borders, 0, -borders)));
        vertices.Add(new Vertex(new Vector3(borders, 0, -borders)));
        vertices.Add(new Vertex(new Vector3(-borders, 0, borders)));
        vertices.Add(new Vertex(new Vector3(borders, 0, borders)));


        triangles.Add(new Triangle(vertices[0], vertices[1], vertices[2]));
        triangles.Add(new Triangle(vertices[2], vertices[1], vertices[3]));

        Vertex newRandomVertex = new Vertex(new Vector3(Random.Range(-borders, borders), 0f, Random.Range(-borders, borders)));
        for (int i = 0; i < triangles.Count; i++)
        {
            triangles[i].Split2DXZ(newRandomVertex);
            triangles.RemoveAt(i);
        }

        //List<Triangle> splitresults = new List<Triangle>();
        //for (int i = 0; i < amount; i++)
        //{
        //    splitresults.Clear();
        //    Vertex newRandomVertex = new Vertex(new Vector3(Random.Range(-borders, borders), 0f, Random.Range(-borders, borders)));
        //    vertices.Add(newRandomVertex);

        //    for (int j = 0; j < triangles.Count; i++)
        //    {
        //        splitresults.AddRange(triangles[j].Split2DXZ(newRandomVertex)); 
        //    }

        //    triangles.Clear();
        //    triangles.AddRange(splitresults);
        //}
    }

    public void OnDrawGizmos()
    {
        for (int i = 0; i < triangles.Count; i++)
        {
            triangles[i].OnDrawGizmos();
        }

#if UNITY_EDITOR
        for (int i = 0; i < vertices.Count; i++)
        {
            Handles.Label(vertices[i].Position, "v" + i.ToString());
        }
#endif
    }

    public void Triangulate()
    {
        Debug.Log("Not implemented yet");
    }
}
