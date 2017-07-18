using UnityEngine;
#if UNITY_EDITOR
//using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// QuadStripTester.
/// </summary>
public class QuadStripTester : MonoBehaviour 
{

    public GameObject[] polyObjects = new GameObject[2];
    public GameObject[] poly1VerticesGO;
    public GameObject[] poly2VerticesGO;

    QuadStrip quadStrip;

    Polyline poly1;
    Polyline poly2;
    Vertex[] poly1Vertices;
    Vertex[] poly2Vertices;


    public void Reset()
    {
        //Collect GameObjects
        polyObjects[0] = transform.GetChild(0).gameObject;
        polyObjects[1] = transform.GetChild(1).gameObject;

        poly1VerticesGO = new GameObject[polyObjects[0].transform.childCount];
        poly2VerticesGO = new GameObject[polyObjects[1].transform.childCount];
        poly1Vertices = new Vertex[poly1VerticesGO.Length];
        poly2Vertices = new Vertex[poly2VerticesGO.Length];

        for (int i = 0; i < polyObjects[0].transform.childCount; i++)
        {
            poly1VerticesGO[i] = polyObjects[0].transform.GetChild(i).gameObject;
            poly1Vertices[i] = new Vertex(poly1VerticesGO[i]);
        }
        for (int i = 0; i < polyObjects[1].transform.childCount; i++)
        {
            poly2VerticesGO[i] = polyObjects[1].transform.GetChild(i).gameObject;
            poly2Vertices[i] = new Vertex(poly2VerticesGO[i]);
        }

        poly1 = new Polyline(new List<Vertex>(poly1Vertices));
        poly2 = new Polyline(new List<Vertex>(poly2Vertices));

        quadStrip = new QuadStrip(poly1, poly2, null, null);
        
    }

    void OnDrawGizmos()
    {
        if (quadStrip != null)
        {
            quadStrip.OnDrawGizmos();
        }
    }
}
