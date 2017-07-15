using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;

public static class CustomGizmos
{
#if UNITY_EDITOR
	public static void DrawCircle(Vector3 center, float radius)
	{
		Color c = Handles.color;
		Matrix4x4 m = Handles.matrix;
		
		Handles.color = Gizmos.color;
		Handles.matrix = Gizmos.matrix;
		
		Handles.DrawWireDisc(center, Vector3.up, radius);
		
		Handles.color = c;
		Handles.matrix = m;
	}
	
	public static void DrawDisc(Vector3 center, float radius)
	{
		Color c = Handles.color;
		Matrix4x4 m = Handles.matrix;
		
		Handles.color = Gizmos.color;
		Handles.matrix = Gizmos.matrix;
		
		Handles.DrawSolidDisc(center, Vector3.up, radius);
		
		Handles.color = c;
		Handles.matrix = m;
	}
	
	public static void DrawWireArc(Vector3 center, Vector3 from, float angle, float radius)
	{
		Color c = Handles.color;
		Matrix4x4 m = Handles.matrix;
		
		Handles.color = Gizmos.color;
		Handles.matrix = Gizmos.matrix;
		
		Handles.DrawWireArc(center, Vector3.up, from, angle, radius);
		
		Handles.color = c;
		Handles.matrix = m;
	}
	
	public static void DrawRing(Vector3 center, float innerRadius, float outerRadius)
	{
		DrawRing(center, innerRadius, outerRadius, 40);
	}
	
	public static void DrawRing(Vector3 center, float innerRadius, float outerRadius, int resolution)
	{
				Color c = Handles.color;
		Matrix4x4 m = Handles.matrix;
		
		Handles.color = Gizmos.color;
		Handles.matrix = Gizmos.matrix;
		
		List<Vector3> innerPoints = new List<Vector3>();
		List<Vector3> outerPoints = new List<Vector3>();
		
		for(int i = 0; i < resolution; i++)
		{
			innerPoints.Add(center + innerRadius * 
								(Vector3.forward * Mathf.Sin(Mathf.PI * 2 * i / (float)resolution) 
								- Vector3.right * Mathf.Cos(Mathf.PI * 2 * i / (float)resolution)));
			outerPoints.Add(center + outerRadius * 
								(Vector3.forward * Mathf.Sin(Mathf.PI * 2 * i / (float)resolution) 
								- Vector3.right * Mathf.Cos(Mathf.PI * 2 * i / (float)resolution)));	
		}
		
		Vector3[] recPoints = new Vector3[4];
		for(int i = 0; i < innerPoints.Count-1; i++)
		{
			recPoints[0] = innerPoints[i];
			recPoints[1] = innerPoints[i+1];
			recPoints[2] = outerPoints[i+1];
			recPoints[3] = outerPoints[i];
			
			Handles.DrawSolidRectangleWithOutline(recPoints, Gizmos.color, Gizmos.color);			
		}
		
		recPoints[0] = innerPoints[innerPoints.Count-1];
		recPoints[1] = innerPoints[0];
		recPoints[2] = outerPoints[0];
		recPoints[3] = outerPoints[outerPoints.Count-1];
		
		Handles.DrawSolidRectangleWithOutline(recPoints, Gizmos.color, Gizmos.color);
		
		Handles.color = c;
		Handles.matrix = m;
		
	}
	
#endif
}
