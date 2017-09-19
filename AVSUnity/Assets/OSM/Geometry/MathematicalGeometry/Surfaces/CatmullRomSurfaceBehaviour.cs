using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

public class CatmullRomSurfaceBehaviour : MonoBehaviour
{

    public CatmullRomSurface surface = new CatmullRomSurface();

    [SerializeField]
    public int NumberOfSegmentsWidth
    {
        get { return surface.NumberOfSegmentsWidth; }
        set { surface.NumberOfSegmentsWidth = value; }
    }


    [SerializeField]
    public int NumberOfSegmentsHeight
    {
        get { return surface.NumberOfSegmentsHeight; }
        set { surface.NumberOfSegmentsHeight = value; }
    }


    [SerializeField]
    public int NumberOfControlPointsWidth
    {
        get { return surface.NumberOfControlPointsWidth; }
        set { surface.NumberOfControlPointsWidth = value; }
    }

    [SerializeField]
    public int NumberOfControlPointsHeight
    {
        get { return surface.NumberOfControlPointsHeight; }
        set { surface.NumberOfControlPointsHeight = value; }
    }



    public Vector3Grid ControlPoints
    {
        get { return surface.controlPoints; }
        set { surface.controlPoints = value; }
    }


    public Vector3Grid SurfacePoints
    {
        get { return surface.surfacePoints; }
    }


    private Vector3 oldPosition;
    void Start()
    {
        //oldPosition = transform.position;
        //this.surface.Position = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        //if (transform.position != oldPosition)
        //{
        //    oldPosition = transform.position;
        //    this.surface.Position = transform.position;
        //}

    }

    public bool drawGizmos = true;
    public bool drawMeshGizmo = false;

    public void OnDrawGizmos()
    {
        DrawSurface();
        DrawControlGridSplines();
    }

    public void DrawControlGrid()
    {
        if (surface.controlPoints == null){
            //surface.controlPoints = new Vector3[surface.NumberOfControlPointsWidth, ]; //, surface.NumberOfControlPointsHeight, new Vector3());
            //for (int i = 0; i < surface.NumberOfControlPointsWidth; i++)
            //{
            //    surf
            //}
            return;
        }

        Gizmos.color = Color.grey;
        for (int x = 0; x < surface.NumberOfControlPointsWidth; x++)
            for (int y = 0; y < surface.NumberOfControlPointsHeight - 1; y++)
                Gizmos.DrawLine(surface.controlPoints[x, y],surface.controlPoints[x, y + 1]);

        for (int x = 0; x < surface.NumberOfControlPointsWidth - 1; x++)
            for (int y = 0; y < surface.NumberOfControlPointsHeight; y++)
                Gizmos.DrawLine(surface.controlPoints[x, y], surface.controlPoints[x + 1, y]);
    }


    public void DrawControlGridSplines()
    {
        if (surface.controlPoints == null)
        {
            //surface.controlPoints = new Array2D<Vector3>(surface.NumberOfControlPointsWidth, surface.NumberOfControlPointsHeight, new Vector3());
            return;
        }

        for (int x = 1; x < surface.NumberOfControlPointsWidth - 1; x++)
        {
            List<Vector3> points = new List<Vector3>();
            for (int y = 0; y < surface.NumberOfControlPointsHeight; y++)
            {
                points.Add(surface.controlPoints[x, y]);
            }
            CardinalSpline.DrawCurveGizmo(points, 0.5f, Color.red);
        }

        for (int y = 1; y < surface.NumberOfControlPointsHeight - 1; y++)
        {
            List<Vector3> points = new List<Vector3>();
            for (int x = 0; x < surface.NumberOfControlPointsWidth; x++)
            {
                points.Add(surface.controlPoints[x, y]);
            }
            CardinalSpline.DrawCurveGizmo(points, 0.5f, Color.red);
        }
    }


    void OnDrawGizmosSelected()
    {

        DrawControlGrid();
        Gizmos.color = Color.green;

        if (surface.controlPoints == null)
            return;

        for (int u = 0; u < surface.NumberOfControlPointsWidth; u++)
            for (int w = 0; w < surface.NumberOfControlPointsHeight; w++)
            {
                Handles.Label(surface.controlPoints[u, w], "P" + u + w);
                Gizmos.DrawIcon(surface.controlPoints[u, w], "CurvesAndSurfaces/circle_green_8.png", false);
            }

        DrawSurface();
        DrawControlGridSplines();
    }

    private void DrawSurface()
    {
        if (surface.surfacePoints == null)
        {
            //surface.surfacePoints = new Vector3[(surface.NumberOfSegmentsWidth) * (surface.NumberOfControlPointsWidth - 3) + 1, (surface.NumberOfSegmentsHeight) * (surface.NumberOfControlPointsHeight - 3) + 1];
            return;
        }

        Gizmos.color = Color.gray;
        for (int u = 0; u < surface.surfacePoints.Width; u++)
            for (int w = 0; w < surface.surfacePoints.Height - 1; w++)
                Gizmos.DrawLine(surface.surfacePoints[u, w], surface.surfacePoints[u, w + 1]);

        for (int u = 0; u < surface.surfacePoints.Width - 1; u++)
            for (int w = 0; w < surface.surfacePoints.Height; w++)
                Gizmos.DrawLine(surface.surfacePoints[u, w], surface.surfacePoints[u + 1, w]);
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

        this.mesh = MeshHelper.GenerateGridMesh(this.mesh, this.transform.position, surface.surfacePoints);
        

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


    public void RecalculateSurface()
    {
        this.surface.RecalculateSurface();
    }

    public void Initialize()
    {
        this.surface.Initialize();
    }

    public float GridSize
    {
        get { return this.surface.GridSize; }
        set { this.surface.GridSize = value; }
    }
}
