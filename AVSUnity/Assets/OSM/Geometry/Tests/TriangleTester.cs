using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// TriangleTester
/// </summary>
public class TriangleTester : MonoBehaviour
{
    public float t = 0f;
    public float s = 0f;

    public GameObject PointToTest;
    public List<GameObject> vertexObjects = new List<GameObject>();

    Vertex pointToTest = new Vertex(Vector3.zero);
    List<Vertex> vertices = new List<Vertex>();

    public List<Triangle> triangles = new List<Triangle>();

    public List<Triangle> splitresult = new List<Triangle>();

    public void Reset()
    {
        vertices = new List<Vertex>();
        triangles = new List<Triangle>();
        vertexObjects = new List<GameObject>();

        PointToTest = GameObject.Find("PointToTest");

        gameObject.DeleteChildren();
        for (int i = 0; i < 4; i++)
        {
            vertexObjects.Add(gameObject.CreateChild());
            vertexObjects[i].name = "Vertex" + i.ToString();
        }
        vertexObjects[0].transform.position = new Vector3(0, 0, 0);
        vertexObjects[1].transform.position = new Vector3(10, 0, 0);
        vertexObjects[2].transform.position = new Vector3(0, 0, 10);
        vertexObjects[3].transform.position = new Vector3(10, 0, 10);

        for (int i = 0; i < 4; i++)
        {
            vertices.Add(new Vertex(vertexObjects[i].transform.position));
        }


        triangles.Add(new Triangle(vertices[0], vertices[1], vertices[2]));
        triangles.Add(new Triangle(vertices[2], vertices[1], vertices[3]));

        if (PointToTest != null)
            pointToTest.Position = PointToTest.transform.position;
    }

    void OnDrawGizmos()
    {
        //Update Positions;
        for (int i = 0; i < Mathf.Min(vertexObjects.Count, vertices.Count); i++)
        {
            vertices[i].Position = vertexObjects[i].transform.position; 
        }

        if (PointToTest != null)
        {
            pointToTest.Position = PointToTest.transform.position;
            Gizmos.DrawCube(pointToTest.Position, Vector3.one * 0.1f);

            for (int i = 0; i < triangles.Count; i++)
            {
                if (triangles[i].IsPointInTriangle2DXZ(pointToTest))
                    Gizmos.color = Color.green;
                else
                    Gizmos.color = Color.red;
                triangles[i].OnDrawGizmos();
            }
        }
        else
        {
            Gizmos.color = Color.blue;
            for (int i = 0; i < triangles.Count; i++)
            {
                if(triangles[i] != null)
                    triangles[i].OnDrawGizmos();                
            }
        }

#if UNITY_EDITOR
        for (int i = 0; i < vertices.Count; i++)
        {
            Handles.Label(vertices[i].Position, "v" + i.ToString());
        }
#endif
    }

    public void SplitTriangle()
    {
        splitresult = new List<Triangle>();

        pointToTest.Position = PointToTest.transform.position;

        for(int i = 0; i < triangles.Count; i++)
        {
            Vertex newVertex = new Vertex(pointToTest);

            bool didSplit;
            splitresult.AddRange(triangles[i].Split2DXZ(newVertex, out didSplit));

            if (didSplit)
            {
                vertices.Add(newVertex);
                GameObject newChild = gameObject.CreateChild();
                vertexObjects.Add(newChild);
                newChild.name = "Vertex" + (vertexObjects.Count - 1).ToString();
                newChild.transform.position = newVertex;
            }
            
            triangles[i] = null;
        }

        triangles = splitresult;
#if UNITY_EDITOR
        SceneView.RepaintAll();
#endif
    }
}