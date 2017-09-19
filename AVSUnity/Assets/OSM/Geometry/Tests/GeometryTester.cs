using UnityEngine;
#if UNITY_EDITOR
//using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// GeometryTester.
/// </summary>
[ExecuteInEditMode]
public class GeometryTester : MonoBehaviour 
{
    public Polygon polygon = new Polygon();
    public Polygon insetPolygon;

    public PolygonSurface polygonSurface = new PolygonSurface();
	
    public float amount = 0.1f;
    //private float oldAmount = 0.1f;
	
	private GameObject[] vertexObjects;

    public void Reset()
    {
		polygon = new Polygon();
		vertexObjects = new GameObject[transform.childCount];
		
		for(int i = 0; i < transform.childCount; i++)
		{
			vertexObjects[i] = transform.GetChild(i).gameObject;			
		}
    }

    public void Update()
    {
//        if (amount != oldAmount)
//        {
//            insetPolygon = polygon.Inset(amount, null);
//            oldAmount = amount;
//        }
		
        polygon.Clear();
		for(int i = 0; i < vertexObjects.Length; i++)
		{
			polygon.Add(new Vertex(vertexObjects[i].transform.position));
		}

        polygon.MakeClockwise();
		
		polygonSurface = polygon.Triangulate(null, null);
        //insetPolygon = polygon.Inset(0.1f, null);
 
    }

    public void OnDrawGizmos()
    {
        //Debug.Log((polygon != null).ToString() + " " + polygon.polygonVertices.Count);

        polygonSurface.OnDrawGizmos();
		
        Gizmos.color = Color.blue;
        if(polygon != null)
            polygon.OnDrawGizmos();
		
		//Debug.Log(triangles.Count + " Triangles");

//        if(insetPolygon != null)
//            insetPolygon.OnDrawGizmos();
    }

}
