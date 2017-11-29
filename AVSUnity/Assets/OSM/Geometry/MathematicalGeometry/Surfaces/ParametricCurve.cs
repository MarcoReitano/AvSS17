using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class ParametricCurve : MonoBehaviour
{

	public Vector3 A = new Vector3();
	public Vector3 B = new Vector3();
	public Vector3 C = new Vector3();
	public Vector3 D = new Vector3();

	public Vector3 P = new Vector3();
	public Vector3 PN = new Vector3();
	public Vector3 dP = new Vector3();
	public Vector3 ddP = new Vector3();
	public Vector3 dddP = new Vector3();

	public Vector3 A_delta_cube = new Vector3();
	public Vector3 A_delta_cube_times_six = new Vector3();
	public Vector3 B_delta_quad = new Vector3();


	public List<Vector3> points;

	int steps = 20;

	public void CalculatePoints()
	{
		points = new List<Vector3>();

		float delta = 0.1f; //1/steps;

		P = this.D;
		A_delta_cube = A * Mathf.Pow(delta, 3f);
		A_delta_cube_times_six = A_delta_cube * 6;
		B_delta_quad = B * Mathf.Pow(delta, 2f);

		dP = A_delta_cube + B_delta_quad + C * delta;
		ddP = A_delta_cube_times_six + 2 * B_delta_quad;
		dddP = A_delta_cube_times_six;

		points.Add(P);
		for(float t = 0f; t < 1f; t += delta) {
			PN = P + dP;
			dP = dP + ddP;
			ddP = ddP + dddP;
			P = PN;
			points.Add(P);
		}
	}
	
	
	public void CalculatePointsSlow()
	{
		points = new List<Vector3>();

		float delta = 0.05f; //1/steps;

		P = D;

		points.Add(P);
		for(float t = 0f; t < 10f; t += delta) {
			points.Add(A*Mathf.Pow(t,3) + B*Mathf.Pow(t,2) + C * Mathf.Pow(t,2) + D);
		}
	}

//	public void CalculatePointsLong()
//	{
//		points = new List<Vector3>();
//
//		float delta = 0.1f; //1/steps;
//
//
//		for(float t = 0f; t < 1f; t += delta) {
//			points.Add(
//			           A * Mathf.Pow(t,3) +
//			           B * Mathf.Pow(t,2) +
//			           C * t +
//			           D);
//		}
//	}




	public void OnDrawGizmos()
	{
		
		if(points != null) {
#if UNITY_EDITOR
            Handles.Label(A, "A");
			Handles.Label(B, "B");
			Handles.Label(C, "C");
			Handles.Label(D, "D");
			Handles.color = Color.white;
			Handles.DrawPolyLine(A,B,C,D);
#endif

            Gizmos.color = Color.green;
			for(int i = 0; i < points.Count - 1; i++) {
				Gizmos.DrawLine(points[i], points[i + 1]);
			}
		}
	}


	// Use this for initialization
	void Start()
	{
		
	}

	// Update is called once per frame
	void Update()
	{
		
	}
}

