using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Line : MonoBehaviour
{
    [SerializeField]
    private Vector3 p0;
    public Vector3 P0
    {
        get { return this.p0; }
        set
        {
            this.p0 = value;

            RecalculateLineSegmentation();
        }
    }

    [SerializeField]
    private Vector3 p1;
    public Vector3 P1
    {
        get { return this.p1; }
        set
        {
            this.p1 = value;
            RecalculateLineSegmentation();
        }
    }

    public Vector3 Vector
    {
        get { return p1 - p0; }
    }


    public Ray Ray
    {
        get { return new Ray(p0, Vector * 100f); }
    }



    /// <summary>
    /// Returns the Point at parameter t on the line
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public Vector3 PointOnLine(float t)
    {
        if (t < 0f)
            return this.p0;

        if (t > 1f)
            return this.p1;

        return (1f - t) * this.P0 + t * this.P1;
    }



    public Vector3 GetOrthogonalNormalizedVector()
    {
        Vector3 normal = this.p1 - this.p0;
        Vector3 zielVect = Vector3.zero;
        Vector3 tangent = Vector3.up;

        Vector3.OrthoNormalize(ref normal, ref tangent, ref zielVect);
        return zielVect;
    }


    public Vector3 GetOrthogonalNormalizedVectorUp()
    {
        Vector3 normal = this.p1 - this.p0;
        Vector3 zielVect = Vector3.zero;
        Vector3 tangent = GetOrthogonalNormalizedVector();

        Vector3.OrthoNormalize(ref normal, ref tangent, ref zielVect);
        return zielVect;
    }


    public int numberOfSegments = 20;

    public List<Vector3> lineSegmentPoints = new List<Vector3>();
    public float width = 5f;


    /// <summary>
    /// Calculate Points on the Line at certain values of t.
    /// </summary>
    public void RecalculateLineSegmentation()
    {
        float delta = 1f / numberOfSegments;
        lineSegmentPoints = new List<Vector3>();

        for (int i = 0; i < numberOfSegments; i++)
        {
            lineSegmentPoints.Add(PointOnLine(i * delta));
        }
        lineSegmentPoints.Add(this.P1);

        transform.position = PointOnLine(0.5f);
        GenerateMesh();
    }



    /// <summary>
    /// 
    /// </summary>
    /// <param name="point">Point from where the Point with the minimal distance is calculated.</param>
    /// <returns>Point on the line with shortest distance to the given point.</returns>
    public Vector3 GetMinDistanceSegmentPointOnLine(Vector3 point)
    {
        Vector3 returnValue = new Vector3();

        float min = float.MaxValue;
        for (int i = 0; i < lineSegmentPoints.Count; i++)
        {
            float squareDistance = (lineSegmentPoints[i] - point).sqrMagnitude;
            if (squareDistance < min)
            {
                min = squareDistance;
                returnValue = lineSegmentPoints[i];
            }
        }

        return returnValue;
    }


    /// <summary>
    /// http://softsurfer.com/Archive/algorithm_0102/algorithm_0102.htm#Distance%20to%20Parametric%20Line
    /// </summary>
    /// <param name="point">Point from where the Point with the minimal distance is calculated.</param>
    /// <returns>Point on the line with shortest distance to the given point.</returns>
    public Vector3 GetMinDistancePointOnLine(Vector3 point)
    {
        Vector3 v = P1 - P0;
        Vector3 w = point - P0;

        float t = Vector3.Dot(w, v) / Vector3.Dot(v, v);

        return PointOnLine(t);
    }



    void OnDrawGizmos()
    {
        Gizmos.color = Color.grey;
        Gizmos.DrawLine(this.P0, this.P1);
    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(this.P0, this.P1);
    }

    public Mesh mesh;
    public Material material;
    public Color color = new Color(0f,1f,0f,42f/255f);

    public void GenerateMesh()
    {

        // clear the old one an set the new mesh
        if (this.mesh == null)
            this.mesh = new Mesh();

        mesh.Clear();

        /*                  triangles:
        //     *      1-----3      [0] = 0 
        //     *      |\    |      [1] = 1
        //     *      | \   |      [2] = 2
        //     * from o-----o to   [3] = 2
        //     *      |   \ |      [4] = 1
        //     *      |    \|      [5] = 3
        //     *      0-----2      
        //     */

        int[] triangles = new int[6];

        // triangle 1
        triangles[0] = 0;
        triangles[1] = 1;
        triangles[2] = 2;

        // triangle 2
        triangles[3] = 2;
        triangles[4] = 1;
        triangles[5] = 3;


        // calculate verticepositions
        Vector3[] vertices = new Vector3[4];
        vertices[0] = this.p0 - GetOrthogonalNormalizedVector() * this.width - transform.position;
        vertices[1] = this.p0 + GetOrthogonalNormalizedVector() * this.width - transform.position;
        vertices[2] = this.p1 - GetOrthogonalNormalizedVector() * this.width - transform.position;
        vertices[3] = this.p1 + GetOrthogonalNormalizedVector() * this.width - transform.position;

        // calculate uvs
        float height = (vertices[1] - vertices[0]).magnitude;
        float width = (vertices[2] - vertices[0]).magnitude;
        Vector2[] uvs = new Vector2[4];
        uvs[0] = new Vector2(0f, 0f);
        uvs[1] = new Vector2(0f, height);
        uvs[2] = new Vector2(width, 0f);
        uvs[3] = new Vector2(width, height);

        // clear the old one an set the new mesh
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        

        MeshRenderer meshRenderer = (MeshRenderer)this.gameObject.GetComponent<MeshRenderer>();
        if (meshRenderer == null)
            meshRenderer = gameObject.AddComponent<MeshRenderer>();

        if (this.material == null)
        {
            this.material = new Material(Shader.Find("Transparent/Diffuse"));
            this.material.color = this.color;
        }

        Material[] materials = new Material[1];
        materials[0] = this.material;
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

