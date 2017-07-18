using UnityEngine;
using System.Collections;

public class PathStreetMember : IStreetMember
{
    public System.Type Type
    {
        get { return typeof(PathStreetMember); }
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

    public TriangleStrip LaneSurface
    {
        get { return laneSurface; }
    }
    private TriangleStrip laneSurface;

    private float width = 1f;
    public void CreateMesh(ModularMesh mesh)
    {
        if (leftOutline)
        {
            rightOutline = leftOutline.Inset(width);
            laneSurface = new TriangleStrip(leftOutline, rightOutline, mesh, MaterialManager.GetMaterial("diffuseBrown"));
        }
        else
            Debug.Log("LeftOutline missing!");
    }
    public void CreateMeshNew(ModularMesh mesh)
    {
        if (leftOutline)
        {
            rightOutline = new Polyline(
                leftOutline.VerticesToParameter(startParameter),
                leftOutline.Inset(width).VerticesBetweenParameter(startParameter, endParameter),
                leftOutline.VerticesFromParameter(endParameter));
            laneSurface = new TriangleStrip(leftOutline, rightOutline, mesh, MaterialManager.GetMaterial("diffuseBrown"));
        }
    }
    public void UpdateMesh(ModularMesh mesh)
    {
        if (laneSurface != null)
            laneSurface.RemoveFromMesh(mesh, MaterialManager.GetMaterial("diffuseBrown"));

        rightOutline = leftOutline.Inset(width);
        laneSurface = new TriangleStrip(leftOutline, rightOutline, mesh, MaterialManager.GetMaterial("diffuseBrown"));
    }
    public void Destroy()
    {
        throw new System.NotImplementedException();
    }
}
