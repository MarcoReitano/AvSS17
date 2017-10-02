using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// StreetTester.
/// </summary>
[ExecuteInEditMode]
public class StreetTester : MonoBehaviour
{
    ModularMesh mesh = new ModularMesh();
    [SerializeField]
    [HideInInspector]
    private Polyline polyline = new Polyline();
    private Polyline secPolyline = new Polyline();

    //private Polyline preVertices = new Polyline();
    //private Polyline postVertices = new Polyline();
    //private Polyline midvertices = new Polyline();

    //private Vertex StartVertex;
    //private Vertex EndVertex;

    public float start = 0f;
    public float end = 0f;
    public float width = 5f;


    public void Reset()
    {
    }

    public void Update()
    {
        mesh.Clear();
        polyline.Clear();
        secPolyline.Clear();

        for (int i = 0; i < transform.childCount; i++)
        {
            polyline.Add(new Vertex(transform.GetChild(i).transform.position));
        }

        secPolyline = new Polyline(
            polyline.VerticesToParameter(start),
            new Vertex[1]{polyline.VertexAtParameter(start)},
            polyline.Inset(width).VerticesBetweenParameter(start, end),
            new Vertex[1]{polyline.VertexAtParameter(end)},
            polyline.VerticesFromParameter(end)
            );
        //preVertices = new Polyline(polyline.VerticesToParameter(start));
        //postVertices = new Polyline(polyline.VerticesFromParameter(end));
        //midvertices = new Polyline(polyline.VerticesBetweenParameter(start, end));

        //StartVertex = polyline.VertexAtParameter(start);
        //EndVertex = polyline.VertexAtParameter(end);

        new TriangleStrip(polyline, secPolyline, mesh, MaterialManager.GetMaterial("diffuseBlue"));

        mesh.FillMesh(transform, false);
    }

    public void CreateMesh()
    {
 
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        for (int i = 0; i < polyline.Count; i++)
            Gizmos.DrawCube(polyline[i].Position, Vector3.one);

        Gizmos.color = Color.red;
        for (int i = 0; i < secPolyline.Count; i++)
        {
            Gizmos.DrawCube(secPolyline[i].Position, Vector3.one);
            Handles.Label(secPolyline[i].Position, i.ToString());
        }

        //Gizmos.color = Color.green;
        //for (int j = 0; j < preVertices.Count; j++)
        //    Gizmos.DrawCube(preVertices[j].Position, Vector3.one);

        //Gizmos.color = Color.blue;
        //for (int j = 0; j < midvertices.Count; j++)
        //    Gizmos.DrawCube(midvertices[j].Position, Vector3.one);

        //Gizmos.color = Color.red;
        //for (int j = 0; j < postVertices.Count; j++)
        //    Gizmos.DrawCube(postVertices[j].Position, Vector3.one);

        Gizmos.color = Color.white;
        polyline.OnDrawGizmos();

        //Gizmos.color = Color.yellow;
        //if (StartVertex)
        //    Gizmos.DrawCube(StartVertex.Position, Vector3.one);
        //if (EndVertex)
        //    Gizmos.DrawCube(EndVertex.Position, Vector3.one);

        secPolyline.OnDrawGizmos();



    }
}
