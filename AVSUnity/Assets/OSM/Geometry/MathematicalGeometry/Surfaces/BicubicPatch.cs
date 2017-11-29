using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class BicubicPatch : MonoBehaviour
{
    public BicubicPatch()
    {

    }

    [SerializeField]
    public Vector3[,] points = new Vector3[4, 4];


    //public Vector3 P (float u, float w)
    //{

    //    float wPow3 = Mathf.Pow(w, 3);
    //    float wPow2 = w * w;

    //    Vector3 A = points[0, 0] * wPow3 + points[0, 1] * wPow2 + points[0, 2] * w + points[0, 3];
    //    Vector3 B = points[1, 0] * wPow3 + points[1, 1] * wPow2 + points[1, 2] * w + points[1, 3];
    //    Vector3 C = points[2, 0] * wPow3 + points[2, 1] * wPow2 + points[2, 2] * w + points[2, 3];
    //    Vector3 D = points[3, 0] * wPow3 + points[3, 1] * wPow2 + points[3, 2] * w + points[3, 3];

    //    //Vector3 A = points[0, 0] * wPow3 + points[1, 0] * wPow2 + points[2, 0] * w + points[3, 0];
    //    //Vector3 B = points[0, 1] * wPow3 + points[1, 1] * wPow2 + points[2, 1] * w + points[3, 1];
    //    //Vector3 C = points[0, 2] * wPow3 + points[1, 2] * wPow2 + points[2, 2] * w + points[3, 2];
    //    //Vector3 D = points[0, 3] * wPow3 + points[1, 3] * wPow2 + points[2, 3] * w + points[3, 3];

    //    Vector3 result = A * Mathf.Pow(u, 3) + B * Mathf.Pow(u, 2) + C * u + D;

    //    return result;
    //}


    //public Vector3 P(float u, float w)
    //{

    //    float wPow3 = Mathf.Pow(w, 3);
    //    float wPow2 = w * w;

    //    // blendingMatrix
    //    Matrix4x4 B = new Matrix4x4();
    //    B.SetRow(0, new Vector4(-0.5f, 1.5f, -1.5f, 0.5f));
    //    B.SetRow(2, new Vector4(1f, -2.5f, 2f, -0.5f));
    //    B.SetRow(3, new Vector4(-0.5f, 0f, 0.5f, 0f));
    //    B.SetRow(4, new Vector4(0f, 1f, 0f, 0f));

    //    // transposedBlendingMatrix
    //    Matrix4x4 BT = B.transpose;

    //    Matrix4x4 P = new Matrix4x4();
    //    int i, j;
    //    P.SetRow(0, new Vector4(points[i + 3, j], points[i + 3, j+1], points[i + 3, j+2], points[i + 3, j+3]));
    //    P.SetRow(0, new Vector4(points[i + 2, j], points[i + 2, j+1], points[i + 2, j+2], points[i + 2, j+3]));
    //    P.SetRow(0, new Vector4(points[i + 1, j], points[i + 1, j+1], points[i + 1, j+2], points[i + 1, j+3]));
    //    P.SetRow(0, new Vector4(points[i + 0, j], points[i + 0, j+1], points[i + 0, j+2], points[i + 0, j+3]));

    //    Vector3 point = new Vector4(Mathf.Pow(u, 3), Mathf.Pow(u, 2), u, 1) * B;


    //    Vector3 A = points[0, 0] * wPow3 + points[0, 1] * wPow2 + points[0, 2] * w + points[0, 3];
    //    Vector3 B = points[1, 0] * wPow3 + points[1, 1] * wPow2 + points[1, 2] * w + points[1, 3];
    //    Vector3 C = points[2, 0] * wPow3 + points[2, 1] * wPow2 + points[2, 2] * w + points[2, 3];
    //    Vector3 D = points[3, 0] * wPow3 + points[3, 1] * wPow2 + points[3, 2] * w + points[3, 3];

    //    //Vector3 A = points[0, 0] * wPow3 + points[1, 0] * wPow2 + points[2, 0] * w + points[3, 0];
    //    //Vector3 B = points[0, 1] * wPow3 + points[1, 1] * wPow2 + points[2, 1] * w + points[3, 1];
    //    //Vector3 C = points[0, 2] * wPow3 + points[1, 2] * wPow2 + points[2, 2] * w + points[3, 2];
    //    //Vector3 D = points[0, 3] * wPow3 + points[1, 3] * wPow2 + points[2, 3] * w + points[3, 3];

    //    Vector3 result = A * Mathf.Pow(u, 3) + B * Mathf.Pow(u, 2) + C * u + D;

    //    return result;
    //}

    public Vector3 P(float u, float w)
    {

        Vector3 P0 = CubicBezier.P(points[0, 0], points[0, 1], points[0, 2], points[0, 3], w);
        Vector3 P1 = CubicBezier.P(points[1, 0], points[1, 1], points[1, 2], points[1, 3], w);
        Vector3 P2 = CubicBezier.P(points[2, 0], points[2, 1], points[2, 2], points[2, 3], w);
        Vector3 P3 = CubicBezier.P(points[3, 0], points[3, 1], points[3, 2], points[3, 3], w);

        Vector3 result = CubicBezier.P(P0, P1, P2, P3, u);

        return result;
    }


    public void Start()
    {

    }


    [SerializeField]
    private int numberOfSegmentsU = 10;
    public int NumberOfSegmentsU
    {
        get
        {
            return numberOfSegmentsU;
        }
        set
        {
            this.numberOfSegmentsU = value;
            RecalculateSurface();
        }
    }


    [SerializeField]
    private int numberOfSegmentsW = 10;
    public int NumberOfSegmentsW
    {
        get
        {
            return numberOfSegmentsW;
        }
        set
        {
            this.numberOfSegmentsW = value;
            RecalculateSurface();
        }
    }

    //	public Vector3[,] surfacePoints;
    public List<Vector3> surfacePoints;

    public void RecalculateSurface()
    {
        //		this.transform.position = this.P00;

        float deltaU = 1f / numberOfSegmentsU;
        float deltaW = 1f / numberOfSegmentsW;

        //		surfacePoints = new Vector3[numberOfSegments + 1, numberOfSegments + 1];
        surfacePoints = new List<Vector3>();

        //List<Vector3> P0 = CubicBezier.CollectBezierPoints(points[0, 0], points[0, 1], points[0, 2], points[0, 3], numberOfSegmentsW);
        //List<Vector3> P1 = CubicBezier.CollectBezierPoints(points[1, 0], points[1, 1], points[1, 2], points[1, 3], numberOfSegmentsW);
        //List<Vector3> P2 = CubicBezier.CollectBezierPoints(points[2, 0], points[2, 1], points[2, 2], points[2, 3], numberOfSegmentsW);
        //List<Vector3> P3 = CubicBezier.CollectBezierPoints(points[3, 0], points[3, 1], points[3, 2], points[3, 3], numberOfSegmentsW);

        List<Vector3> P0 = CubicBezier.CollectBezierPoints(points[0, 0], points[1, 0], points[2, 0], points[3, 0], numberOfSegmentsU);
        List<Vector3> P1 = CubicBezier.CollectBezierPoints(points[0, 1], points[1, 1], points[2, 1], points[3, 1], numberOfSegmentsU);
        List<Vector3> P2 = CubicBezier.CollectBezierPoints(points[0, 2], points[1, 2], points[2, 2], points[3, 2], numberOfSegmentsU);
        List<Vector3> P3 = CubicBezier.CollectBezierPoints(points[0, 3], points[1, 3], points[2, 3], points[3, 3], numberOfSegmentsU);

        for (int u = 0; u < numberOfSegmentsU + 1; u++)
        {
            List<Vector3> curveW = CubicBezier.CollectBezierPoints(P0[u], P1[u], P2[u], P3[u], numberOfSegmentsW);
            surfacePoints.AddRange(curveW);
        }

        //
        //{
        //    for (int w = 0; w < numberOfSegmentsW + 1; w++)
        //    {
        //        surfacePoints.Add(P(u * deltaU, w * deltaW));
        //    }
        //}

        GenerateMesh();
    }

    public bool drawGizmos = true;
    public bool drawMeshGizmo = false;


    public void OnDrawGizmos()
    {
#if UNITY_EDITOR
        Gizmos.color = Color.grey;

        //if (surfacePoints == null) {
        //    surfacePoints = new List<Vector3> ();
        //    RecalculateSurface ();
        //}

        //if (drawMeshGizmo) {
        //    for (int i = 0; i < numberOfSegments + 1; i++) {
        //        int offset = i * (numberOfSegments + 1);
        //        Gizmos.DrawLine (this.surfacePoints[offset], this.surfacePoints[offset + numberOfSegments]);
        //        Gizmos.DrawLine (this.surfacePoints[i], this.surfacePoints[i + (numberOfSegments + 1) * (numberOfSegments + 1) - (numberOfSegments + 1)]);
        //    }
        //}

        for (int u = 0; u < 4; u++)
            for (int w = 0; w < 3; w++)
                Gizmos.DrawLine(this.points[u, w], this.points[u, w + 1]);

        for (int u = 0; u < 3; u++)
            for (int w = 0; w < 4; w++)
                Gizmos.DrawLine(this.points[u, w], this.points[u + 1, w]);
#endif
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
#if UNITY_EDITOR
        for (int u = 0; u < 4; u++)
            for (int w = 0; w < 4; w++)
                Handles.Label(this.points[u, w], "P" + u + w);
#endif
    }

    public Mesh mesh;
    public bool generateMesh = true;
    public Color color = Color.green;
    public float alpha = 0.3f;
    public Material material;

    public void GenerateMesh()
    {
        // clear the old one an set the new mesh
        if (this.mesh == null)
            this.mesh = new Mesh();

        mesh.Clear();

        if (generateMesh)
        {

            List<Vector3> vertices = new List<Vector3>();
            foreach (Vector3 vertex in surfacePoints)
                vertices.Add(vertex - transform.position);

            int numberOfpointsU = numberOfSegmentsU + 1;
            int numberOfpointsW = numberOfSegmentsW + 1;

            List<int> triangles = new List<int>();
            for (int u = 0; u < numberOfSegmentsU; u++)
            {
                for (int w = 0; w < numberOfSegmentsW; w++)
                {

                    triangles.Add(u * numberOfpointsW + w);
                    triangles.Add(u * numberOfpointsW + w + 1);
                    triangles.Add((u + 1) * numberOfpointsW + w);

                    triangles.Add((u + 1) * numberOfpointsW + w);
                    triangles.Add(u * numberOfpointsW + w + 1);
                    triangles.Add((u + 1) * numberOfpointsW + w + 1);
                }
            }


            List<Vector2> uvs = new List<Vector2>();
            foreach (Vector3 vertex in surfacePoints)
                uvs.Add(new Vector2(vertex.x, vertex.z));



            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.uv = uvs.ToArray();


            MeshRenderer meshRenderer = (MeshRenderer)this.gameObject.GetComponent<MeshRenderer>();
            if (meshRenderer == null)
                meshRenderer = gameObject.AddComponent<MeshRenderer>();

            if (this.material == null)
                this.material = new Material(Shader.Find("Transparent/Diffuse"));

            Material[] materials = new Material[1];
            materials[0] = this.material;


            this.color.a = alpha;
            materials[0].color = this.color;
            meshRenderer.materials = materials;

            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            ;

            MeshFilter meshFilter = (MeshFilter)this.gameObject.GetComponent<MeshFilter>();
            if (meshFilter == null)
                meshFilter = this.gameObject.AddComponent<MeshFilter>();
            meshFilter.sharedMesh = this.mesh;

        }
    }

    public void Initialize()
    {
        for (int u = 0; u < 4; u++)
        {
            for (int w = 0; w < 4; w++)
            {
                points[u, w] = new Vector3(u * 10f, 0f, w * 10f);
            }
        }
        RecalculateSurface();
    }
}

