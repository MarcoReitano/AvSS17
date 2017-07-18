using UnityEngine;
#if UNITY_EDITOR
//using UnityEditor;
#endif
using System.Collections;

/// <summary>
/// QuadTester.
/// </summary>
public class QuadTester : MonoBehaviour 
{
    public float t = 0f;
    public float s = 0f;

    public GameObject[] vertexObjects;
    Quad quad;

    public void Reset()
    {
        vertexObjects = new GameObject[transform.childCount];
        Vertex[] vertices = new Vertex[4];

        for (int i = 0; i < transform.childCount; i++)
        {
            vertexObjects[i] = transform.GetChild(i).gameObject;
        }

        if (vertexObjects.Length >= 4)
        {
            for (int i = 0; i < 4; i++)
            {
                vertices[i] = new Vertex(vertexObjects[i]);
            }
            quad = new Quad(vertices);         
        }
    }

    void OnDrawGizmos()
    {
        if (quad != null)
        {
            quad.OnDrawGizmos();
            //Gizmos.DrawCube(quad[t, s], Vector3.one * 0.1f);
        }
    }
}
