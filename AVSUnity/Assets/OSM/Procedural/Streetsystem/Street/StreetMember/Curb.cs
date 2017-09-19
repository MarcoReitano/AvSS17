using UnityEngine;
#if UNITY_EDITOR
//using UnityEditor;
#endif
using System.Collections;

/// <summary>
/// Curb.
/// </summary>
public class Curb : IStreetMember
{
    public System.Type Type
    {
        get { return typeof(Curb); }
    }
    public Polyline LeftOutline
    {
        get
        {
            return leftOutline;
        }
        set
        {
            leftOutline = value;
        }
    }
    public Polyline RightOutline
    {
        get { return rightOutline; }
    }

    public float StartParameter
    {
        get
        {
            return startParameter;
        }
        set
        {
            startParameter = value;
        }
    }
    public float EndParameter
    {
        get
        {
            return endParameter;
        }
        set
        {
            endParameter = value;
        }
    }

    private Polyline leftOutline;
    private Polyline rightOutline;

    private float startParameter = 0f;
    private float endParameter = 1f;

    public Polyline CurbEdge
    {
        get { return curbedge; }
    }
    public TriangleStrip CurbFace
    {
        get { return curbFace; }
    }
    public TriangleStrip CurbSurface
    {
        get { return curbSurface; }
    }

    private Polyline curbedge;
    private TriangleStrip curbFace;
    private TriangleStrip curbSurface;

    private float width = 0.1f;
    private float height = 0.1f;

    public void CreateMesh(ModularMesh mesh)
    {
        if (leftOutline)
        {
            curbedge = leftOutline.Translate(Vector3.up * height);
            rightOutline = curbedge.Inset(width);

            curbFace = new TriangleStrip(leftOutline, curbedge, mesh, MaterialManager.GetMaterial("diffuseCheckerboard"));
            curbSurface = new TriangleStrip(curbedge, rightOutline, mesh, MaterialManager.GetMaterial("diffuseCheckerboard"));
        }
        else
            Debug.Log("LeftOutline missing");
    }
    public void UpdateMesh(ModularMesh mesh)
    {
        if (curbFace != null)
            curbFace.RemoveFromMesh(mesh, MaterialManager.GetMaterial("diffuseCheckerboard"));
        if (curbSurface != null)
            curbSurface.RemoveFromMesh(mesh, MaterialManager.GetMaterial("diffuseCheckerboard"));

        curbedge = leftOutline.Translate(Vector3.up * height);
        rightOutline = curbedge.Inset(width);

        curbFace = new TriangleStrip(leftOutline, curbedge, mesh, MaterialManager.GetMaterial("diffuseCheckerboard"));
        curbSurface = new TriangleStrip(curbedge, rightOutline, mesh, MaterialManager.GetMaterial("diffuseCheckerboard"));
    }
    public void Destroy()
    {
        throw new System.NotImplementedException();
    }
}
