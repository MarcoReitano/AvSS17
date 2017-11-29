using System.Collections.Generic;
using UnityEngine;


public class CurvePath : MonoBehaviour {


    public List<AbstractPathCurve> curves = new List<AbstractPathCurve>();

    public Vector3 start = new Vector3(10f, 0f, 10f);


    public Vector3 GetLastPoint()
    {
        if (this.curves.Count > 0)
            return this.curves[this.curves.Count - 1].EndPoint;
        return start;
    }

    public Vector3 GetLastDirection()
    {
        if (this.curves.Count > 0)
            return this.curves[this.curves.Count - 1].EndDirection;
        return start;
    }


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}


    void OnDrawGizmos()
    {
        foreach (AbstractPathCurve curve in curves)
        {
            GizmoUtils.DrawPathGizmo(curve.Points);
        }
    }
}
