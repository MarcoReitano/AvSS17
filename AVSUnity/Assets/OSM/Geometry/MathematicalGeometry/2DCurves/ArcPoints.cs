using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class ArcPoints : MonoBehaviour
{

    public Vector3 startPoint = Vector3.zero;
    public Vector3 startDirection = Vector3.forward;
    public float angle = 120;
    public float radius = 1;
    public int segments = 15;
    public List<Vector3> points = new List<Vector3>();

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnDrawGizmos()
    {

        Vector3 center;
        float startAngle;
        //this.points = CalculateArcPointsCW(this.startPoint + transform.position, this.startDirection, this.radius, this.angle, this.segments, out center, out startAngle);
        //this.points = CalculateArcPointsCW(transform.position, this.startDirection, this.radius, this.angle, this.segments, out center, out startAngle);
        this.points = CalculateArcSequence(transform.position, this.startDirection, this.radius, this.angle, this.segments, out center, out startAngle);
        
        // Given a Startpoint...
        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(transform.position, this.radius / 10f);

        // ... and the startDirection --> calculate the center of the circle:
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(center, radius / 8);

        if (this.points.Count > 1)
        {
            Gizmos.color = Color.gray;
            Gizmos.DrawLine(transform.position, center);
            Gizmos.DrawLine(points[points.Count - 1], center);

            ////Gizmos.color = Color.yellow;
            ////Gizmos.DrawSphere(zeroPoint + transform.position, Radius / 10);
            //Gizmos.color = Color.yellow;
            //Gizmos.DrawLine(startPoint + transform.position, (startPoint + startDirection.normalized) + transform.position);

            //StartAngle = 270f - MathUtils.AngleToZAxis(Center - startPoint);
            //Handles.Label(startPoint + new Vector3(0f, 0f, -0.3f), StartAngle + "");


            GizmoUtils.DrawPathGizmo(this.points);
            //Debug.Log("DarwArc");

            Gizmos.color = Color.green;
            Gizmos.DrawSphere(this.points[0], radius / 10);

            Gizmos.color = Color.red;
            Gizmos.DrawSphere(this.points[this.points.Count - 1], radius / 10);
        }

    }

    public static List<Vector3> CalculateArcSequence(Vector3 startPoint, Vector3 startDirection, float radius, float angle, int segments, out Vector3 center, out float startAngle)
    {
        List<Vector3> points = new List<Vector3>();

        points.Add(startPoint - startDirection * radius*2);

        points.AddRange(CalculateArcPointsCW(startPoint, startDirection, radius*2, MathUtils.Gon2Degree(17.5f), segments, out center, out startAngle));

        Vector3 newdirection = MathUtils.GetOrthogonalNormalizedVector(points[points.Count - 1], center);
        points.AddRange(CalculateArcPointsCW(points[points.Count - 1], newdirection, radius, angle, segments, out center, out startAngle));

        newdirection = MathUtils.GetOrthogonalNormalizedVector(points[points.Count - 1], center);
        points.AddRange(CalculateArcPointsCW(points[points.Count - 1], newdirection, radius * 3, MathUtils.Gon2Degree(22.5f), segments, out center, out startAngle));
        
        newdirection = MathUtils.GetOrthogonalNormalizedVector(points[points.Count - 1], center);
        //points.Add(points[points.Count - 1] + newdirection * radius*2);

        return points;
    }
    

    public static List<Vector3> CalculateArcPointsCW(Vector3 startPoint, Vector3 startDirection, float radius, float angle, int segments, out Vector3 center, out float startAngle)
    {
        center = (startPoint - MathUtils.GetOrthogonalNormalizedVector(startDirection) * radius);
        startAngle = 270f - MathUtils.AngleToZAxis(center - startPoint);
        return CalculateArcPoints(center, radius, startAngle, angle, segments);
    }



    public static List<Vector3> CalculateArcPoints(Vector3 center, float radius, float startAngle, float angle, int segments)
    {
        List<Vector3> points = new List<Vector3>();

        //float step = (2 * Mathf.PI) / punktanzahl;
        float endAngle = startAngle - angle;

        if (angle != 0)
        {
            float angleStep = angle / (float)segments;
            Debug.Log(angle + " / " + segments + " = " + angleStep + " --> " + (angleStep * segments));


            float currentAngle = startAngle;
            for (int i = 0; i <= segments; i++)
            {
                Vector3 first = PointOnCircle(startAngle - i * angleStep, radius, center);
                points.Add(first);
            }
        }

        return points;
    }

    private static Vector3 PointOnCircle(float angle, float radius, Vector3 center)
    {
        return new Vector3(
                Mathf.Cos(angle * Mathf.Deg2Rad) * radius + center.x,
                center.y,
                Mathf.Sin(angle * Mathf.Deg2Rad) * radius + center.z);
    }

}
