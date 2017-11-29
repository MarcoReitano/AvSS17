using System.Collections.Generic;
using System.Globalization;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;


public class CubicBezier : MonoBehaviour
{

    private Vector3 oldStartPosition = new Vector3(0, 0, 0);
    public Vector3 startPosition = new Vector3(0, 0, 0);
    public Vector3 StartPosition
    {
        get { return this.startPosition; }
        set
        {
            this.oldStartPosition = this.startPosition;
            this.startPosition = value;

            Vector3 positionDifference = this.startPosition - this.oldStartPosition;
            this.StartTangentPosition += positionDifference;
            this.startTangentVector = this.startTangentPosition - this.startPosition;

            RecalculateCurve();

        }
    }

    public Vector3 startTangentVector;
    private Vector3 oldStartTangentPosition = new Vector3(0, 0, 10);
    public Vector3 startTangentPosition = new Vector3(0, 0, 10);
    public Vector3 StartTangentPosition
    {
        get { return this.startTangentPosition; }
        set
        {
            this.oldStartTangentPosition = this.startTangentPosition;
            this.startTangentPosition = value;
            this.startTangentVector = this.startTangentPosition - this.startPosition;

            RecalculateCurve();

        }
    }


    private Vector3 oldEndPosition = new Vector3(10, 0, 10);
    public Vector3 endPosition = new Vector3(10, 0, 10);
    public Vector3 EndPosition
    {
        get { return this.endPosition; }
        set
        {
            this.oldEndPosition = this.endPosition;
            this.endPosition = value;

            Vector3 positionDifference = this.endPosition - this.oldEndPosition;
            this.EndTangentPosition += positionDifference;
            this.endTangentVector = this.endPosition - this.endTangentPosition;

            RecalculateCurve();

        }
    }

    public Vector3 endTangentVector;

    private Vector3 oldEndTangentPosition = new Vector3(10, 0, 0);
    public Vector3 endTangentPosition = new Vector3(10, 0, 0);
    public Vector3 EndTangentPosition
    {
        get { return this.endTangentPosition; }
        set
        {
            this.oldEndTangentPosition = this.endTangentPosition;
            this.endTangentPosition = value;
            this.endTangentVector = this.endPosition - this.endTangentPosition;

            RecalculateCurve();

        }
    }


    public List<Vector2> offsets = new List<Vector2>();



    public CubicBezier()
    {
        this.startPosition = Vector3.zero;
        this.oldStartPosition = Vector3.zero;
        this.oldStartTangentPosition = Vector3.zero;
        this.startTangentPosition = Vector3.zero;
        this.startTangentVector = Vector3.zero;

        this.endPosition = Vector3.zero;
        this.oldEndPosition = Vector3.zero;
        this.oldEndTangentPosition = Vector3.zero;
        this.endTangentPosition = Vector3.zero;
        this.endTangentVector = Vector3.zero;
    }

    void Reset()
    {
        RecalculateCurve();
        MeshFilter meshFilter = (MeshFilter)this.gameObject.GetComponent<MeshFilter>();
        if (meshFilter == null)
            meshFilter = this.gameObject.AddComponent<MeshFilter>();
        meshFilter.sharedMesh = null;
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(this.startPosition, 0.3f);
        Gizmos.DrawSphere(this.endPosition, 0.3f);

        DrawBezier(Color.green);

        //if (this.curves != null)
        //{
        //    foreach (List<Vector3> points in curves)
        //        DrawBezier(points, Color.yellow);
        //}
    }

    public Vector3 GetMinDistancePointOnCurve(Vector3 point)
    {
        Vector3 returnValue = new Vector3();
        if (bezierPoints.Count > 0)
        {
            float min = float.MaxValue;
            for (int i = 0; i < bezierPoints.Count; i++)
            {
                float squareDistance = (bezierPoints[i] - point).sqrMagnitude;
                if (squareDistance < min)
                {
                    min = squareDistance;
                    returnValue = bezierPoints[i];
                }
            }
        }
        return returnValue;
    }


    public float width = 1f;
    public bool isLine = false;

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(this.startPosition, this.StartTangentPosition);
        Gizmos.DrawLine(this.endPosition, this.EndTangentPosition);

        //		Handles.color = Color.yellow;
        //		Handles.ArrowCap(0, this.startPosition, Quaternion.LookRotation(this.startTangentVector), this.startTangentVector.magnitude);
        //		Handles.ArrowCap(0, this.endTangentPosition, Quaternion.LookRotation(this.endTangentVector), this.endTangentVector.magnitude);

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(this.StartTangentPosition, 0.2f);
        Gizmos.DrawSphere(this.EndTangentPosition, 0.2f);

#if UNITY_EDITOR
        Handles.Label(this.midPoint, "" + this.arcLength.ToString("0.##", CultureInfo.CreateSpecificCulture("en-US")));

        Handles.Label(this.startPosition, "P0");
        Handles.Label(this.startTangentPosition, "T0");
        Handles.Label(this.endPosition, "P1");
        Handles.Label(this.endTangentPosition, "T1");
#endif
    }


    public void DrawBezier(Color color)
    {
        if (this.bezierPoints != null)
        {
            if (this.bezierPoints.Count > 1)
            {
                Gizmos.color = color;
                Vector3 previousPoint = this.bezierPoints[0];
                Vector3 currentPoint;

                for (int i = 1; i < this.bezierPoints.Count; i++)
                {
                    currentPoint = this.bezierPoints[i];
                    Gizmos.DrawLine(currentPoint, previousPoint);
                    previousPoint = currentPoint;
                }
            }
        }
    }


    public void DrawBezier(List<Vector3> points, Color color)
    {
        if (points != null)
        {
            if (points.Count > 1)
            {
                Gizmos.color = color;
                Vector3 previousPoint = points[0];
                Vector3 currentPoint;

                for (int i = 1; i < points.Count; i++)
                {
                    currentPoint = points[i];
                    Gizmos.DrawLine(currentPoint, previousPoint);
                    previousPoint = currentPoint;
                }
            }
        }
    }


    public void DrawBezierOffset(Color color, float offset, float heightOffset)
    {
        List<Vector3> points = CollectBezierPointsWithOffset(offset, heightOffset);
        DrawBezier(points, color);

    }

    //public void DrawBezierOffset(Color color, float offset, float heightOffset)
    //{
    //    if (this.bezierPoints != null)
    //    {
    //        if (this.bezierPoints.Count > 1)
    //        {
    //            Gizmos.color = color;
    //            Vector3 previousPoint = this.bezierPoints[0];


    //            Vector3 previousOffsetPoint = GetPointWithOffset(this.bezierPoints[0], this.bezierPoints[1], offset, heightOffset);
    //            Gizmos.DrawLine(previousPoint, previousOffsetPoint);

    //            Vector3 currentPoint;
    //            Vector3 currentOffsetPoint;
    //            for (int i = 1; i < this.bezierPoints.Count - 1; i++)
    //            {
    //                currentOffsetPoint = GetPointWithOffset(this.bezierPoints[i], this.bezierPoints[i + 1], offset, heightOffset);

    //                Gizmos.DrawLine(this.bezierPoints[i], currentOffsetPoint);
    //                Gizmos.DrawLine(currentOffsetPoint, previousOffsetPoint);
    //                previousOffsetPoint = currentOffsetPoint;
    //            }

    //            currentPoint = this.bezierPoints[this.bezierPoints.Count - 1];
    //            currentOffsetPoint = GetPointWithOffset(this.bezierPoints[this.bezierPoints.Count - 2], this.bezierPoints[this.bezierPoints.Count - 1], offset, heightOffset);

    //            Gizmos.DrawLine(currentPoint, currentOffsetPoint);
    //            Gizmos.DrawLine(currentOffsetPoint, previousOffsetPoint);
    //            previousOffsetPoint = currentOffsetPoint;

    //        }
    //    }
    //}


    private Vector3 GetPointWithOffset(Vector3 point0, Vector3 point1, float offset, float heightOffset)
    {
        Vector3 offsetVector = -GetOrthogonalNormalizedVector(point0, point1) * offset;
        Vector3 heightOffsetVector = GetOrthogonalNormalizedVectorUp(point0, (point0 + offsetVector)) * heightOffset;

        return point0 + offsetVector + heightOffsetVector;
    }


    private static Vector3 GetOrthogonalNormalizedVector(Vector3 point0, Vector3 point1)
    {
        Vector3 normal = point1 - point0;
        Vector3 zielVect = Vector3.zero;
        Vector3 tangent = Vector3.up;

        Vector3.OrthoNormalize(ref normal, ref tangent, ref zielVect);
        return zielVect;
    }

    public static Vector3 GetOrthogonalNormalizedVectorUp(Vector3 point0, Vector3 point1)
    {
        Vector3 normal = point1 - point0;
        Vector3 zielVect = Vector3.zero;
        Vector3 tangent = GetOrthogonalNormalizedVector(point0, point1);

        Vector3.OrthoNormalize(ref normal, ref tangent, ref zielVect);

        return -zielVect;
    }


    public List<Vector3> CollectBezierPointsWithOffset(float offset, float heightOffset)
    {
        List<Vector3> resultList = new List<Vector3>();

        if (this.bezierPoints != null)
        {
            if (this.bezierPoints.Count > 1)
            {
                //Vector3 previousPoint = this.bezierPoints[0];
                Vector3 previousOffsetPoint;// = GetPointWithOffset(this.bezierPoints[0], this.bezierPoints[1], offset, heightOffset);

                //resultList.Add(previousOffsetPoint);

                Vector3 currentOffsetPoint;
                for (int i = 1; i < this.bezierPoints.Count; i++)
                {
                    currentOffsetPoint = GetPointWithOffset(this.bezierPoints[i - 1], this.bezierPoints[i], offset, heightOffset);
                    previousOffsetPoint = currentOffsetPoint;
                    resultList.Add(previousOffsetPoint);
                }

                currentOffsetPoint = GetPointWithOffset(this.bezierPoints[this.bezierPoints.Count - 1], this.bezierPoints[this.bezierPoints.Count - 1] + this.endTangentVector, offset, heightOffset);
                resultList.Add(currentOffsetPoint);
            }
        }
        return resultList;
    }


    public Vector3 midPoint;
    public float arcLength;

    public float CalculateLength(List<Vector3> points)
    {
        float length = 0f;

        for (int i = 1; i < points.Count; i++)
            length += (points[i] - points[i - 1]).magnitude;

        return length;
    }


    [SerializeField]
    public CubicBezier left;
    public float leftStartOffset = 3f;
    public float leftEndOffset = 8f;


    public int method = 0;

    public void RecalculateCurve()
    {
        switch (method)
        {
            case 0: CollectBezierPoints();
                break;
            case 1: CollectBezierPoints2();
                break;
            default:
                CollectBezierPoints();
                break;
        }

        this.arcLength = CalculateLength(bezierPoints);
        this.midPoint = P(0.5f);
        this.transform.position = this.midPoint;

        if (left != null)
        {
            Vector3 offsetVector = -GetOrthogonalNormalizedVector(bezierPoints[0], bezierPoints[1]) * leftStartOffset;
            left.startPosition = this.startPosition + offsetVector;
            left.startTangentPosition = left.startPosition + this.startTangentVector;

            offsetVector = -GetOrthogonalNormalizedVector(bezierPoints[bezierPoints.Count - 2], bezierPoints[bezierPoints.Count - 1]) * leftEndOffset;
            left.endPosition = this.endPosition + offsetVector;
            left.endTangentPosition = left.endPosition - this.endTangentVector;
            left.RecalculateCurve();
        }
        GenerateMesh();
    }

    public List<Vector3> tangents;



    public List<Vector3> bezierPoints = new List<Vector3>();
    public int segments = 15;

    /// <summary>
    /// Fast Algorithm for calculating Points on the Cubic BezierCurve
    /// "[The points are calculated using] "forward difference" in combination with the Taylor series
    /// representation, to speed um the calculation significantly."
    /// (David Solomon, Curves and Surfaces for Computer Graphics, p.186)
    /// </summary>
    /// <returns>
    /// A <see cref="List<Vector3>"/>
    /// </returns>
    public List<Vector3> CollectBezierPoints()
    {
        bezierPoints = new List<Vector3>();

        float delta_t = (float)1f / segments;
        float q1 = 3 * delta_t;
        float q2 = q1 * delta_t; // 3*delta_t²

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

        this.bezierPoints.Add(B);

        for (float i = 0; i < this.segments; i++)
        {
            B += dB;
            this.bezierPoints.Add(B);

            dB += ddB;
            ddB += dddB;
        }

        return bezierPoints;
    }







    /// <summary>
    /// Fast Algorithm for calculating Points on the Cubic BezierCurve
    /// "[The points are calculated using] "forward difference" in combination with the Taylor series
    /// representation, to speed um the calculation significantly."
    /// (David Solomon, Curves and Surfaces for Computer Graphics, p.186)
    /// </summary>
    /// <returns>
    /// A <see cref="List<Vector3>"/>
    /// </returns>
    public static List<Vector3> CollectBezierPoints(Vector3 startPosition, Vector3 startTangentPosition, Vector3 endTangentPosition, Vector3 endPosition, int segments)
    {
        List<Vector3> bezierPoints = new List<Vector3>();

        float delta_t = (float)1f / segments;
        float q1 = 3 * delta_t;
        float q2 = q1 * delta_t; // 3*delta_t²

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

        for (float i = 0; i < segments; i++)
        {
            B += dB;
            bezierPoints.Add(B);

            dB += ddB;
            ddB += dddB;
        }

        return bezierPoints;
    }


    public Vector3 P(float t)
    {
        float tPow3 = Mathf.Pow(t, 3f);
        float tPow2 = Mathf.Pow(t, 2f);
        float oneMinusT = 1f - t;

        Vector3 point = Mathf.Pow(oneMinusT, 3) * startPosition +
            3 * t * Mathf.Pow(oneMinusT, 2) * startTangentPosition +
            3 * tPow2 * oneMinusT * endTangentPosition +
            tPow3 * endPosition;

        return point;
    }


    public static Vector3 P(Vector3 startPosition, Vector3 startTangentPosition, Vector3 endTangentPosition, Vector3 endPosition, float t)
    {
        float tPow3 = Mathf.Pow(t, 3f);
        float tPow2 = Mathf.Pow(t, 2f);
        float oneMinusT = 1f - t;

        Vector3 point = Mathf.Pow(oneMinusT, 3) * startPosition +
            3 * t * Mathf.Pow(oneMinusT, 2) * startTangentPosition +
            3 * tPow2 * oneMinusT * endTangentPosition +
            tPow3 * endPosition;

        return point;
    }



    /// <summary>
    /// Different Method for calculating points on the BezierCurve (slower method)
    /// </summary>
    /// <returns>
    /// A <see cref="List<Vector3>"/>
    /// </returns>
    public List<Vector3> CollectBezierPoints2()
    {
        bezierPoints = new List<Vector3>();

        Vector3 previousPoint = this.startPosition;
        this.bezierPoints.Add(previousPoint);

        for (int i = 1; i <= segments; i++)
        {
            float pm = (float)i / segments;
            float d = 1f - pm;
            Vector3 currentPoint =
                Mathf.Pow(d, 3) * this.startPosition +
                3f * Mathf.Pow(d, 2) * pm * this.startTangentPosition +
                3f * d * Mathf.Pow(pm, 2) * this.endTangentPosition +
                Mathf.Pow(pm, 3) * this.endPosition;

            previousPoint = currentPoint;
            this.bezierPoints.Add(previousPoint);
        }

        return bezierPoints;
    }


    //	public void RefreshWithNewValues(Vector3 start1, Vector3 direction1, Vector3 start2, Vector3 direction2, Vector3 intersectionPoint)
    //    {
    //        this.start1 = start1;
    //        this.direction1 = direction1;
    //
    //        this.start2 = start2;
    //        this.direction2 = direction2;
    //        this.intersectionPoint = intersectionPoint;
    //
    //        this.end1 = CalculateTangentEndpoint(start1, direction1, this.intersectionPoint);
    //        this.end2 = CalculateTangentEndpoint(start2, direction2, this.intersectionPoint);
    //
    //        Refresh();
    //    }
    //
    //
    //	public void Refresh()
    //    {
    //        CalculateTangents();
    //        collectBezierPoints(this.segments);
    //    }
    //
    //    /// <summary>
    //    ///
    //    /// </summary>
    //    public void CalculateTangents()
    //    {
    //        if ((this.direction1 == Vector3.zero) || (this.direction2 == Vector3.zero))
    //        {
    //            //Debug.Log(this.name + ": one of the directions is zero...");
    //            this.intersection = ProceduralMeshUtils.Ray2RayIntersect(
    //            this.start1,
    //            this.end1,
    //            this.start2,
    //            this.end2);
    //        }
    //        else
    //        {
    //            this.intersection = ProceduralMeshUtils.Ray2RayIntersect(
    //            this.start1,
    //            this.start1 + this.direction1,
    //            this.start2,
    //            this.start2 + this.direction2);
    //        }
    //
    //        if (intersection.points.Count > 0)
    //        {
    //            this.intersectionPoint = intersection.points[0];
    //            this.inverseIntersectionPoint = this.intersectionPoint + (this.start1 - this.intersectionPoint) + (this.start2 - this.intersectionPoint);
    //
    //            this.end1 = CalculateTangentEndpoint(start1, direction1, this.intersectionPoint);
    //            this.end2 = CalculateTangentEndpoint(start2, direction2, this.intersectionPoint);
    //        }
    //        else
    //        {
    //            //Debug.Log(this.name + ": No TangentIntersection Found...");
    //            if (this.end1 == Vector3.zero)
    //                this.end1 = this.start1;
    //
    //            if (this.end2 == Vector3.zero)
    //                this.end2 = this.start2;
    //        }
    //    }
    //
    //	public static Vector3 CalculateTangentEndpoint(Vector3 tangent_start, Vector3 tangent_direction, Vector3 intersection)
    //    {
    //        // alpha = Winkel des Vectors zur x Achse
    //        float gamma = 90f;
    //        float alpha = Mathf.Abs(90f - Vector3.Angle(Vector3.up, tangent_direction));
    //        float beta = 180f - alpha - gamma;
    //
    //        // Ankathete b:
    //        // Berechnung über y-genullten Anfangspunkt der Bezierkurve
    //        float b = Mathf.Abs(Vector3.Distance(
    //            new Vector3(tangent_start.x, 0f, tangent_start.z),
    //            new Vector3(intersection.x, 0f, intersection.z)));
    //
    //        float c = Mathf.Abs(b / (Mathf.Sin(beta * Mathf.Deg2Rad) / Mathf.Sin(gamma * Mathf.Deg2Rad)));
    //
    //        return tangent_start + tangent_direction.normalized * c;
    //    }
    //
    //
    //	public static Intersection Bezier2BezierIntersection(CubicBezier bezier, CubicBezier intersectingBezier)
    //    {
    //        List<Vector3> bezier1points = bezier.points;
    //        List<Vector3> bezier2points = intersectingBezier.points;
    //        Intersection intersectionPoints = new Intersection();
    //
    //        for (int i = 0; i < bezier1points.Count - 1; i++)
    //        {
    //            Vector3 bezier1_p1 = bezier1points[i];
    //            Vector3 bezier1_p2 = bezier1points[i + 1];
    //
    //            for (int j = 0; j < bezier2points.Count - 1; j++)
    //            {
    //                Vector3 bezier2_p1 = bezier2points[j];
    //                Vector3 bezier2_p2 = bezier2points[j + 1];
    //                intersectionPoints.AppendPoints(ProceduralMeshUtils.Line2LineIntersect(bezier1_p1, bezier1_p2, bezier2_p1, bezier2_p2).points);
    //            }
    //        }
    //
    //        if (intersectionPoints.points.Count == 0)
    //            intersectionPoints.Status = IntersectionStatus.NO_INTERSECTION;
    //        if (intersectionPoints.points.Count == 1)
    //            intersectionPoints.Status = IntersectionStatus.INTERSECTION;
    //        if (intersectionPoints.points.Count > 1)
    //            intersectionPoints.Status = IntersectionStatus.MULTIPLE_INTERSECTIONS;
    //
    //        return intersectionPoints;
    //    }



    public Mesh mesh;
    public bool generateMesh = true;
    public Color color = Color.green;
    public float alpha = 0.3f;
    public Material material;

    List<Vector3>[] curves;

    public void GenerateMesh()
    {
        // clear the old one an set the new mesh
        if (this.mesh == null)
            this.mesh = new Mesh();

        mesh.Clear();

        if (generateMesh && offsets.Count > 0)
        {
            // jeweils mit der nächsten Kurve verbinden
            List<int>[] submeshes = new List<int>[offsets.Count - 1];
            List<Vector2> uvs = new List<Vector2>();

            List<Vector3> vertices = new List<Vector3>();

            // Array für all Kurven
            curves = new List<Vector3>[offsets.Count];

            // alle Kurven berechnen
            int count = 0;
            foreach (Vector2 offset in offsets)
                curves[count++] = CollectBezierPointsWithOffset(offset.x, offset.y);


            int indexOffset = 0;
            int numberOfPoints = segments + 1;


            for (int i = 0; i < offsets.Count - 1; i++)
            {
                //      Knoten beider Kurven hinzufügen
                float x = 0f;
                float y = 0f;

                vertices.Add(curves[i][0] - transform.position);
                uvs.Add(new Vector2(x, y));

                for (int u = 1; u < numberOfPoints; u++)
                {
                    vertices.Add(curves[i][u] - transform.position);

                    y += (vertices[u + indexOffset] - vertices[u + indexOffset - 1]).magnitude;
                    uvs.Add(new Vector2(x, y));
                }

                vertices.Add(curves[i + 1][0] - transform.position);
                x = (vertices[numberOfPoints - 1 + indexOffset] - vertices[indexOffset]).magnitude;
                y = 0f;
                uvs.Add(new Vector2(x, y));

                for (int u = 1; u < numberOfPoints; u++)
                {
                    vertices.Add(curves[i + 1][u] - transform.position);
                    y += (vertices[u * 2 + indexOffset] - vertices[u * 2 + indexOffset - 1]).magnitude;
                    uvs.Add(new Vector2(x, y));
                }


                submeshes[i] = new List<int>();

                //      Indices berechnen
                for (int u = 0 + indexOffset; u < segments + indexOffset; u++)
                {
                    submeshes[i].Add(u);
                    submeshes[i].Add(u + 1);
                    submeshes[i].Add(numberOfPoints + u);

                    submeshes[i].Add(numberOfPoints + u);
                    submeshes[i].Add(u + 1);
                    submeshes[i].Add(numberOfPoints + u + 1);
                }

                //      Offset der Indices berechnen
                indexOffset += 2 * numberOfPoints;



            }

            mesh.vertices = vertices.ToArray();
            mesh.subMeshCount = submeshes.Length;
            for (int i = 0; i < submeshes.Length; i++)
            {
                mesh.SetTriangles(submeshes[i].ToArray(), i);
            }

            mesh.uv = uvs.ToArray();


            MeshRenderer meshRenderer = (MeshRenderer)this.gameObject.GetComponent<MeshRenderer>();
            if (meshRenderer == null)
                meshRenderer = gameObject.AddComponent<MeshRenderer>();

            if (this.material == null)
                this.material = new Material(Shader.Find("Transparent/Diffuse"));

            Material[] materials = new Material[submeshes.Length];
            this.color.a = 1;
            for (int i = 0; i < submeshes.Length; i++)
            {
                materials[i] = new Material(Shader.Find("Transparent/Diffuse"));
                materials[i].color = this.color;
            }
            meshRenderer.materials = materials;

            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            ;

            MeshFilter meshFilter = (MeshFilter)this.gameObject.GetComponent<MeshFilter>();
            if (meshFilter == null)
                meshFilter = this.gameObject.AddComponent<MeshFilter>();
            meshFilter.sharedMesh = this.mesh;
            //			DoMesh(ref meshFilter);
        }
    }
}

