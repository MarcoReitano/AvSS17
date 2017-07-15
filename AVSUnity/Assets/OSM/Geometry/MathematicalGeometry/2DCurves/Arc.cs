using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Arc : MonoBehaviour {


    public float startWinkel;
    public float StartWinkel
    {
        get { return startWinkel; }
        set { startWinkel = value; }
    }

    public float endWinkel;
    public float EndWinkel
    {
        get { return endWinkel; }
        set { endWinkel = value; }
    }

    public float radius;
    public float Radius
    {
        get { return radius; }
        set { radius = value; }
    }

    public Vector3 MidPoint
    {
        get { return transform.position; }
    }



	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    
	}


    public List<Vector3> points = new List<Vector3>();

    void OnDrawGizmos()
    {
        this.points = CalculateArcPoints(MidPoint, Radius, StartWinkel, EndWinkel, 90);

        if (this.points.Count > 1)
        {
            drawPathGizmo(this.points);
            //Debug.Log("DarwArc");

            Gizmos.color = Color.green;
            Gizmos.DrawSphere(this.points[0], Radius / 10);

            Gizmos.color = Color.red;
            Gizmos.DrawSphere(this.points[this.points.Count - 1], Radius / 10);
        }
    }


    public List<Vector3> CalculateArcPoints(Vector3 midPoint, float radius, float startWinkel, float endWinkel, int punktanzahl)
    {
        List<Vector3> points = new List<Vector3>();

        float step = (2*Mathf.PI) / punktanzahl;
        
        for (float i = startWinkel; i < endWinkel; i += step)
        {
            Vector3 first = PointOnCircle(i, radius, midPoint);
            //Vector3 second = PointOnCircle(i + step, radius, midPoint);
            points.Add(first);
            
        }

        return points;
    }

    private static Vector3 PointOnCircle(float winkel, float radius, Vector3 midPoint)
    {
        return new Vector3(
                Mathf.Cos(winkel * Mathf.Deg2Rad) * radius + midPoint.x,
                midPoint.y, 
                Mathf.Sin(winkel * Mathf.Deg2Rad) * radius + midPoint.z);
    }

    public static void drawPathGizmo(List<Vector3> points)
    {
        if (points != null)
        {
            if (points.Count > 1)
            {
                Gizmos.color = Color.gray;
                Vector3 previousPoint = points[0];

                for (int i = 1; i < points.Count; i++)
                {
                    Vector3 currentPoint = points[i];
                    Gizmos.DrawLine(currentPoint, previousPoint);
                    previousPoint = currentPoint;
                }
            }
        }
    }
}
