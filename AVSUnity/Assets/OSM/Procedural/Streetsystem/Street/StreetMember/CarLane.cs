using UnityEngine;
#if UNITY_EDITOR
//using UnityEditor;
#endif
using System.Collections;

/// <summary>
/// CarLane.
/// </summary>
public class CarLane : IStreetMember, IProceduralObjects
{
    public System.Type Type
    {
        get { return typeof(CarLane); }
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

    private float width = 3.25f;

    public void CreateMesh(ModularMesh mesh)
    {
        if (leftOutline)
        {
            rightOutline = leftOutline.Inset(width);
            laneSurface = new TriangleStrip(leftOutline, rightOutline, mesh, MaterialManager.GetMaterial("diffuseDarkGray"));
        }
    }

    public void CreateMeshNew(ModularMesh mesh)
    {
        if (leftOutline)
        {
            rightOutline = new Polyline(
                leftOutline.VerticesToParameter(startParameter),
                new Vertex[1]{leftOutline.VertexAtParameter(startParameter)},
                leftOutline.Inset(width).VerticesBetweenParameter(startParameter, endParameter),
                new Vertex[1]{leftOutline.VertexAtParameter(endParameter)},
                leftOutline.VerticesFromParameter(endParameter));

            laneSurface = new TriangleStrip(leftOutline, rightOutline, mesh, MaterialManager.GetMaterial("diffuseDarkGray"));
        }
        else
            Debug.Log("LeftOutline missing");
    }
    public void UpdateMesh(ModularMesh mesh)
    {
        if (laneSurface != null)
            laneSurface.RemoveFromMesh(mesh, MaterialManager.GetMaterial("diffuseDarkGray"));

        rightOutline = leftOutline.Inset(width);
        laneSurface = new TriangleStrip(leftOutline, rightOutline, mesh, MaterialManager.GetMaterial("diffuseDarkGray"));
    }
    public void Destroy()
    {
        throw new System.NotImplementedException();
    }
}
