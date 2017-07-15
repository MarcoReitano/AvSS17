
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEditor;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class BilinearSurface : MonoBehaviour
{

    public BilinearSurface()
    {

    }


    [SerializeField]
    private Vector3 p00 = new Vector3(0, 0, 0);
    public Vector3 P00
    {
        get { return this.p00; }
        set
        {
            this.p00 = value;
            RecalculateSurface();
        }
    }

    [SerializeField]
    private Vector3 p01 = new Vector3(0, 0, 10);
    public Vector3 P01
    {
        get { return this.p01; }
        set
        {
            this.p01 = value;
            RecalculateSurface();
        }
    }


    [SerializeField]
    private Vector3 p10 = new Vector3(10, 0, 0);
    public Vector3 P10
    {
        get { return this.p10; }
        set
        {
            this.p10 = value;
            RecalculateSurface();
        }
    }

    [SerializeField]
    private Vector3 p11 = new Vector3(10, 0, 10);
    public Vector3 P11
    {
        get { return this.p11; }
        set
        {
            this.p11 = value;
            RecalculateSurface();
        }
    }



    public Vector3 P(float u, float w)
    {
        Vector3 result = P00 * (1 - u) * (1 - w) + P01 * (1 - u) * w + P10 * u * (1 - w) + P11 * u * w;

        return result;
    }


    public void Start()
    {
        //		surfacePoints = new Vector3[numberOfSegments + 1, numberOfSegments + 1];
    }


    [SerializeField]
    private int numberOfSegmentsU = 10;
    public int NumberOfSegmentsU
    {
        get { return numberOfSegmentsU; }
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
        get { return numberOfSegmentsW; }
        set
        {
            this.numberOfSegmentsW = value;
            RecalculateSurface();
        }
    }


    public Vector3[,] surfacePoints;
    //public Array2D<Vector3> surfacePoints;
    //public List<Vector3> surfacePoints;

    public void RecalculateSurface()
    {
        //		this.transform.position = this.P00;

        float deltaU = 1f / numberOfSegmentsU;
        float deltaW = 1f / numberOfSegmentsW;

        surfacePoints = new Vector3[numberOfSegmentsU + 1, numberOfSegmentsW + 1];
        //surfacePoints = new Array2D<Vector3>(numberOfSegmentsU + 1, numberOfSegmentsW + 1, Vector3.zero);
        //surfacePoints = new List<Vector3> ();

        for (int u = 0; u < numberOfSegmentsU + 1; u++)
        {
            for (int w = 0; w < numberOfSegmentsW + 1; w++)
            {
                surfacePoints[u, w] = P(u * deltaU, w * deltaW);
                //surfacePoints.Add (P (u * deltaU, w * deltaW));
                //surfacePoints.Set(u, w, P(u * deltaU, w * deltaW));
            }
        }

        GenerateMesh();
    }

    public bool drawGizmos = true;
    public bool drawMeshGizmo = false;

    public void OnDrawGizmos()
    {

        Gizmos.color = Color.grey;
        Gizmos.DrawLine(this.P00, this.P10);
        Gizmos.DrawLine(this.P01, this.P11);

        if (surfacePoints == null)
        {
            //surfacePoints = new Array2D<Vector3>(numberOfSegmentsU + 1, numberOfSegmentsW + 1, Vector3.zero);
            surfacePoints = new Vector3[numberOfSegmentsU + 1, numberOfSegmentsW + 1];
            //surfacePoints = new List<Vector3> ();
            RecalculateSurface();
        }

        if (drawMeshGizmo)
        {
            //for (int i = 0; i < numberOfSegments + 1; i++) {
            //    int offset = i * (numberOfSegments + 1);
            //    Gizmos.DrawLine (this.surfacePoints[offset], this.surfacePoints[offset + numberOfSegments]);
            //    Gizmos.DrawLine (this.surfacePoints[i], this.surfacePoints[i + (numberOfSegments + 1) * (numberOfSegments + 1) - (numberOfSegments + 1)]);
            //}

            for (int u = 0; u < numberOfSegmentsU + 1; u++)
            {
                //Gizmos.DrawLine(this.surfacePoints.Get(u, 0), this.surfacePoints.Get(u, numberOfSegmentsW));
                Gizmos.DrawLine(this.surfacePoints[u, 0], this.surfacePoints[u, numberOfSegmentsW]);

                for (int w = 0; w < numberOfSegmentsW + 1; w++)
                {
                    //Gizmos.DrawLine(this.surfacePoints.Get(0, w), this.surfacePoints.Get(numberOfSegmentsU, w));
                    Gizmos.DrawLine(this.surfacePoints[0, w], this.surfacePoints[numberOfSegmentsU, w]);
                }
            }

        }
        //		

    }


    void Reset()
    {
        GenerateMesh();

    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(this.P00, this.P10);
        Gizmos.DrawLine(this.P01, this.P11);

        Handles.Label(this.P00, "P00");
        Handles.Label(this.P01, "P01");
        Handles.Label(this.P10, "P10");
        Handles.Label(this.P11, "P11");
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
            for (int u = 0; u < numberOfSegmentsU + 1; u++)
            {
                for (int w = 0; w < numberOfSegmentsW + 1; w++)
                {
                    vertices.Add(surfacePoints[u, w] - transform.position);
                }
            }

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
            for (int u = 0; u < numberOfSegmentsU + 1; u++)
            {
                for (int w = 0; w < numberOfSegmentsW + 1; w++)
                {
                    uvs.Add(new Vector2(surfacePoints[u, w].x, surfacePoints[u, w].z));
                }
            }



            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.uv = uvs.ToArray();

            MeshFilter meshFilter = (MeshFilter)this.gameObject.GetComponent<MeshFilter>();
            if (meshFilter == null)
                meshFilter = this.gameObject.AddComponent<MeshFilter>();

            this.mesh = MeshHelper.GenerateGridMesh(this.mesh, transform.position, surfacePoints);

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


            meshFilter.sharedMesh = this.mesh;

            //			DoMesh(ref meshFilter);
            //}
        }






        //	public void DoMesh (ref MeshFilter mf)
        //	{
        //		mesh = mf.sharedMesh;
        //		var uvs = new Vector2[mesh.vertices.Length];
        //		var tris = mesh.triangles;
        //		var go = mf.gameObject;
        //		for (var i = 0; i < tris.Length; i += 3) {
        //			Vector3 a = go.transform.TransformPoint (mesh.vertices[tris[i]]);
        //			Vector3 b = go.transform.TransformPoint (mesh.vertices[tris[i + 1]]);
        //			Vector3 c = go.transform.TransformPoint (mesh.vertices[tris[i + 2]]);
        //			Vector3 n = Vector3.Cross (a - c, b - c).normalized;
        //			
        //			if (Vector3.Dot (Vector3.up, n) > .5 || Vector3.Dot (-Vector3.up, n) > .5) {
        //				uvs[tris[i]] = new Vector2 (a.x, a.z);
        //				uvs[tris[i + 1]] = new Vector2 (b.x, b.z);
        //				uvs[tris[i + 2]] = new Vector2 (c.x, c.z);
        //			} else if (Vector3.Dot (Vector3.right, n) > .5 || Vector3.Dot (Vector3.left, n) > .5) {
        //				uvs[tris[i]] = new Vector2 (a.y, a.z);
        //				uvs[tris[i + 1]] = new Vector2 (b.y, b.z);
        //				uvs[tris[i + 2]] = new Vector2 (c.y, c.z);
        //			} else {
        //				uvs[tris[i]] = new Vector2 (a.y, a.x);
        //				uvs[tris[i + 1]] = new Vector2 (b.y, b.x);
        //				uvs[tris[i + 2]] = new Vector2 (c.y, c.x);
        //			}
        //		}
        //		mesh.uv = uvs;
        //	}

    }

}