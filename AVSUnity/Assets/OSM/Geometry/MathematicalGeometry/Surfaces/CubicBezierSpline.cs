using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEditor;


public class CubicBezierSpline : MonoBehaviour
{
	[SerializeField]
	public List<CurvePoint> points;


	public CubicBezierSpline()
	{
		
	}


	public void AppendPoint()
	{
		if(points == null)
			points = new List<CurvePoint>();
		
//		points.Add(ScriptableObject.CreateInstance<CurvePoint>());
		GameObject go = new GameObject();
		if(go != null){

			CurvePoint curvePoint = go.AddComponent<CurvePoint>();
			Debug.Log(curvePoint.transform.name);
			curvePoint.transform.parent = this.transform;
			points.Add(curvePoint);
		}
		Debug.Log("points.Count = " + points.Count);
	}


	public void OnDrawGizmos()
	{
		
		if(points != null) {
			Gizmos.color = Color.red;
			
			
			foreach(CurvePoint point in points) {
				Gizmos.DrawIcon(point.Position, "CurvesAndSurfaces/red8.png");
			}
			
			if(this.splinePoints != null) {
				if(this.splinePoints.Count > 1) {
					Gizmos.color = Color.gray;
					Vector3 previousPoint = this.splinePoints[0];
					
					for(int i = 1; i < this.splinePoints.Count; i++) {
						Vector3 currentPoint = this.splinePoints[i];
						Gizmos.DrawLine(currentPoint, previousPoint);
						previousPoint = currentPoint;
					}
				}
			}
		}
	}


	public void OnDrawGizmosSelected()
	{

	}



	public bool circle = true;

	[SerializeField]
	public List<Vector3> splinePoints;

	public void RecalculateCurve()
	{
		if(points != null) {
			if(points.Count > 1) {
				splinePoints = new List<Vector3>();
				
				if(points.Count > 1) {
					for(int i = 0; i < points.Count - 1; i++) {
						List<Vector3> bezPoints = CollectBezierPoints(points[i].Position, points[i].StartTangentPosition, points[i + 1].Position, points[i + 1].EndTangentPosition);
						splinePoints.AddRange(bezPoints);
					}
					
					// complete circle
					if(circle) {
						List<Vector3> bezPoints = CollectBezierPoints(points[points.Count - 1].Position, points[points.Count - 1].StartTangentPosition, points[0].Position, points[0].EndTangentPosition);
						splinePoints.AddRange(bezPoints);
						
					}
				}
			}
		}
	}





	public int segments = 20;

	/// <summary>
	/// Fast Algorithm for calculating Points on the Cubic BezierCurve
	/// "[The points are calculated using] "forward difference" in combination with the Taylor series
	/// representation, to speed um the calculation significantly."
	/// (David Solomon, Curves and Surfaces for Computer Graphics, p.186)
	/// </summary>
	/// <returns>
	/// A <see cref="List<Vector3>"/>
	/// </returns>
	public List<Vector3> CollectBezierPoints(Vector3 startPosition, Vector3 startTangentPosition, Vector3 endPosition, Vector3 endTangentPosition)
	{
		List<Vector3> bezierPoints = new List<Vector3>();
		
		float delta_t = (float) 1f / segments;
		float q1 = 3 * delta_t;
		float q2 = q1 * delta_t;
		// 3*delta_t²
		float q3 = Mathf.Pow(delta_t, 3f);
		float q4 = 2 * q2;
		float q5 = 6 * q3;
		
		Vector3 q6 = startPosition - 2 * startTangentPosition + endTangentPosition;
		Vector3 q7 = 3 * (startTangentPosition - endTangentPosition) - startPosition + endPosition;
		
		Vector3 B = startPosition;
		Vector3 dB = (startTangentPosition - startPosition) * q1 + q6 * q2 + q7 * q3;
		// (P1-P0) * Q1 + Q6xQ2 + Q7xQ3
		Vector3 dddB = q7 * q5;
		// Q7xQ5
		Vector3 ddB = q6 * q4 + dddB;
		// Q6xQ4 + Q7xQ5
		
		bezierPoints.Add(B);
		for(float i = 0; i < this.segments; i++) {
			B = B + dB;
			bezierPoints.Add(B);
			
			dB = dB + ddB;
			ddB = ddB + dddB;
		}
		
		return bezierPoints;
	}
	
	
	
}

