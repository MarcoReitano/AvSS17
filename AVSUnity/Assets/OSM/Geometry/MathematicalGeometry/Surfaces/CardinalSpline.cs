using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CardinalSpline : MonoBehaviour
{

    public List<Vector3> controlPoints = new List<Vector3>();
    public Color color = Color.green;

    public CardinalSpline()
    {

    }


    // Use this for initialization
    public void Start()
    {
        controlPoints.Add(new Vector3(0f, 0f, 0f));
        controlPoints.Add(new Vector3(5f, 0f, 0f));
        controlPoints.Add(new Vector3(10f, 0f, 0f));
        controlPoints.Add(new Vector3(15f, 0f, 0f));

    }

    // Update is called once per frame
    public void Update()
    {

    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        foreach (Vector3 point in controlPoints)
            Gizmos.DrawSphere(point, 0.3f);

        DrawCurve(Color.green);


    }

    public List<Vector3> splinePoints;
    public int segments = 15;

    public void DrawCurve(Color color)
    {
        if (this.splinePoints != null)
        {
            if (this.splinePoints.Count > 1)
            {
                Gizmos.color = color;
                for (int i = 1; i < this.splinePoints.Count; i++)
                    Gizmos.DrawLine(this.splinePoints[i - 1], this.splinePoints[i]);
            }
        }
    }

    public static void DrawCurve(Vector3 P1, Vector3 P2, Vector3 P3, Vector3 P4, float s, int numberOfPoints, Color color)
    {
        List<Vector3> tmpSplinePoints = CardinalSpline.CalculateSplinePoints(P1, P2, P3, P4, s, numberOfPoints);
        Debug.Log(tmpSplinePoints.Count);
        Gizmos.color = color;
        for (int i = 1; i < tmpSplinePoints.Count; i++)
            Gizmos.DrawLine(tmpSplinePoints[i - 1], tmpSplinePoints[i]);

    }

    public static void DrawCurveGizmo(List<Vector3> points, float s, Color color)
    {
        List<Vector3> tmpSplinePoints = CardinalSpline.CalculateSplinePoints(points, s, points.Count * 10);
        //Debug.Log(tmpSplinePoints.Count);
        Gizmos.color = color;
        for (int i = 0; i < tmpSplinePoints.Count - 1; i++)
            Gizmos.DrawLine(tmpSplinePoints[i], tmpSplinePoints[i+1]);

    }


    [SerializeField]
    public float s = 0.5f;
    public Mesh mesh;
    public Material material;

    /// <summary>
    /// Fast Algorithm for calculating Points on the Cubic BezierCurve
    /// "[The points are calculated using] "forward difference" in combination with the Taylor series
    /// representation, to speed um the calculation significantly."
    /// (David Solomon, Curves and Surfaces for Computer Graphics, p.186)
    /// </summary>
    /// <returns>
    /// A <see cref="List<Vector3>"/>
    /// </returns>
    public static List<Vector3> CalculateSplinePoints(List<Vector3> controlPoints, float s, int numberOfSegments)
    {
        List<Vector3> splinePoints = new List<Vector3>();

        float delta_t = (float)1f / numberOfSegments;


        for (int segment = 0; segment < controlPoints.Count - 3; segment++)
        {
            for (float i = 0; i < numberOfSegments + 1; i++)
            {
                float t = i * delta_t;
                //float s = 1f;
                splinePoints.Add(P(controlPoints[segment], controlPoints[segment + 1], controlPoints[segment + 2], controlPoints[segment + 3], t, s));
            }
        }

        return splinePoints;
    }


    public List<Vector3> CalculateSplinePoints()
    {
        splinePoints = CalculateSplinePoints(controlPoints, s, segments);
        GenerateMesh();
        return splinePoints;
    }


    public static List<Vector3> CalculateSplinePoints(Vector3 P1, Vector3 P2, Vector3 P3, Vector3 P4, float s, int numberOfPoints)
    {
        List<Vector3> splinePoints = new List<Vector3>();

        float delta_t = (float)1f / numberOfPoints;
        
        for (float i = 0; i < numberOfPoints; i++)
        {
            float t = i * delta_t;
            splinePoints.Add(P(P1, P2, P3, P4, t, s));
        }

        return splinePoints;
    }


    public static Vector3 P(Vector3 P1, Vector3 P2, Vector3 P3, Vector3 P4, float t, float s)
    {
        float tPow3 = Mathf.Pow(t, 3);
        float tPow2 = Mathf.Pow(t, 2);

        float twoTimesTPow2 = 2 * tPow2;
        float twoTimesTPow3 = 2 * tPow3;

        Vector3 result =
            s * (-tPow3 + twoTimesTPow2 - t) * P1 +
            s * (-tPow3 + tPow2) * P2 + (twoTimesTPow3 - 3 * tPow2 + 1) * P2 +
            s * (tPow3 - twoTimesTPow2 + t) * P3 + (-twoTimesTPow3 + 3 * tPow2) * P3 +
            s * (tPow3 - tPow2) * P4;

        return result;
    }


    public float alpha = 0.5f;

    public void GenerateMesh()
    {
        //// clear the old one an set the new mesh
        if (this.mesh == null)
            this.mesh = new Mesh();

        mesh.Clear ();

        //if (generateMesh) {

        //    List<Vector3> vertices = new List<Vector3>();
        //    for (int u = 0; u < numberOfSegmentsU + 1; u++)
        //    {
        //        for (int w = 0; w < numberOfSegmentsW + 1; w++)
        //        {
        //            vertices.Add(surfacePoints[u,w] - transform.position);
        //        }
        //    }

        //    int numberOfpointsU = numberOfSegmentsU + 1;
        //    int numberOfpointsW = numberOfSegmentsW + 1;

        //    List<int> triangles = new List<int> ();
        //    for (int u = 0; u < numberOfSegmentsU; u++) {
        //        for (int w = 0; w < numberOfSegmentsW; w++) {
        //            triangles.Add (u * numberOfpointsW + w);
        //            triangles.Add (u * numberOfpointsW + w + 1);
        //            triangles.Add ((u + 1) * numberOfpointsW + w);

        //            triangles.Add ((u + 1) * numberOfpointsW + w);
        //            triangles.Add (u * numberOfpointsW + w + 1);
        //            triangles.Add ((u + 1) * numberOfpointsW + w + 1);
        //        }
        //    }


        //    List<Vector2> uvs = new List<Vector2> ();
        //    for (int u = 0; u < numberOfSegmentsU + 1; u++)
        //    {
        //        for (int w = 0; w < numberOfSegmentsW + 1; w++)
        //        {
        //            uvs.Add(new Vector2(surfacePoints[u, w].x, surfacePoints[u, w].z));
        //        }
        //    }



        //    mesh.vertices = vertices.ToArray ();
        //    mesh.triangles = triangles.ToArray ();
        //    mesh.uv = uvs.ToArray ();

        MeshFilter meshFilter = (MeshFilter)this.gameObject.GetComponent<MeshFilter>();
        if (meshFilter == null)
            meshFilter = this.gameObject.AddComponent<MeshFilter>();

        this.mesh = MeshHelper.GenerateWaveMesh(this.mesh, transform.position, splinePoints);

        MeshRenderer meshRenderer = (MeshRenderer)this.gameObject.GetComponent<MeshRenderer>();
        if (meshRenderer == null)
            meshRenderer = gameObject.AddComponent<MeshRenderer>();

        //if (this.material == null)
        //    this.material = new Material(Shader.Find("Transparent/Diffuse"));

        this.material = new Material(Shader.Find("Custom/BackfaceOn"));

        Material[] materials = new Material[1];
        materials[0] = this.material;


        this.color.a = alpha;
        materials[0].color = this.color;
        meshRenderer.materials = materials;

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        ;


        meshFilter.sharedMesh = this.mesh;

        //			DoMesh(ref meshFilter);
        //}
    }
}
