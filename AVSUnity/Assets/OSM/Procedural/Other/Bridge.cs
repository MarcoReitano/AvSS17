using UnityEngine;
#if UNITY_EDITOR
//using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Bridge.
/// </summary>
public class Bridge : IProceduralObjects
{
    public static Dictionary<Bridge, Tile> bridges = new Dictionary<Bridge, Tile>();
    public static void Clear()
    {
        bridges.Clear();
    }
    public static void TryCreateFromOSM()
    {
        throw new System.NotImplementedException();
    }
    public static void CreateAllMeshes()
    {
        foreach (KeyValuePair<Bridge, Tile> kV in bridges)
        {
            kV.Key.CreateMesh(kV.Value.TileMesh);
        }
    }
    public static void CreateAllMeshes(ModularMesh mesh)
    {
        foreach (KeyValuePair<Bridge, Tile> kV in bridges)
        {
            kV.Key.CreateMesh(mesh);
        }
    }

    public Bridge(Intersection from, Intersection to, OSMWay way, OSMData osm)
    {
        this.from = from;
        this.to = to;
        this.way = way;
        //this.osm = osm;


        //Calculate pathway for street

        //Polyline pathway = new Polyline();

        //OSMNode node;

        //foreach (long nodeId in way.GetWayNodesFromTo(from, to))
        //{
        //    if (osm.nodes.TryGetValue(nodeId, out node))
        //    {
        //        pathway.Add(new Vertex(node + Vector3.up * 0.1f));
        //    }
        //}

        //pathway = pathway.InterpolateHeights();

        //this.street = new Street(from, to, way, osm, pathway);
        this.street = new Street(from, to, way, osm);
    }

    public OSMWay Way
    {
        get { return way; }
    }
    public Intersection From
    {
        get { return from; }
    }
    public Intersection To
    {
        get { return to; }
    }
    public Street Street 
    { 
        get { return street; } 
    }

    [SerializeField]
    private OSMWay way;
    [SerializeField]
    private Intersection from;
    [SerializeField]
    private Intersection to;
    //[SerializeField]
    //private OSMData osm;
    [SerializeField]
    private Street street;

    public void CreateMesh(ModularMesh mesh)
    {

        Polyline leftTopEdge;
        Polyline leftBottomEdge;
        Polyline rightTopEdge;
        Polyline rightBottomEdge;

        leftTopEdge = street.LeftOutline.Inset(-2f);
        leftBottomEdge = leftTopEdge.Translate(Vector3.down * 2f);
        rightTopEdge = street.RightOutline.Inset(2f);
        rightBottomEdge = rightTopEdge.Translate(Vector3.down * 2f);

        new QuadStrip(leftTopEdge, street.LeftOutline, mesh, MaterialManager.GetMaterial("diffuseGray"));
        new QuadStrip(leftBottomEdge, leftTopEdge, mesh, MaterialManager.GetMaterial("diffuseGray"));
        new QuadStrip(street.RightOutline, rightTopEdge, mesh, MaterialManager.GetMaterial("diffuseGray"));
        new QuadStrip(rightTopEdge, rightBottomEdge, mesh, MaterialManager.GetMaterial("diffuseGray"));
        new QuadStrip(rightBottomEdge, leftBottomEdge, mesh, MaterialManager.GetMaterial("diffuseGray"));        
    }
    public void UpdateMesh(ModularMesh mesh)
    {
        throw new System.NotImplementedException();
    }
    public void Destroy()
    {
        throw new System.NotImplementedException();
    }
}
