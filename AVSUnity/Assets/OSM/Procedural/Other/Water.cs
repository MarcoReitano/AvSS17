using UnityEngine;
#if UNITY_EDITOR
//using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Water.
/// </summary>
public class Water : IProceduralObjects
{
    public static Dictionary<Water, Tile> waters = new Dictionary<Water, Tile>();
    public static void Clear()
    {
        waters.Clear();
    }
    public static bool TryCreateFromOSM(OSMWay way, Tile tile)
    {
        if (way.Tags.ContainsKey("natural"))
        {
            if (way.Tags["natural"] == "water")
            {
                waters.Add(new Water(way, tile.Query.OSM), tile);
                return true;
            }
        }
        return false;
    }
    public static void CreateAllMeshes()
    {
        foreach (KeyValuePair<Water, Tile> kV in waters)
        {
            kV.Key.CreateMesh(kV.Value.TileMesh);
        }
    }
    public static void CreateAllMeshes(ModularMesh mesh)
    {
        foreach (KeyValuePair<Water, Tile> kV in waters)
        {
            kV.Key.CreateMesh(mesh);
        }
    }

    public Water(OSMWay way, OSMData osm)
    {
        this.way = way;
        this.osm = osm;
    }
    OSMWay way;
    OSMData osm;

    Polygon layout;
    Polygon leveledLayout;

    public PolygonSurface Area
    {
        get { return area; }

    }
    PolygonSurface area;


    public void CreateMesh(ModularMesh mesh)
    {
        layout = new Polygon();

        OSMNode node;

        //Discard last waynode if its the same as first
        if (way.WayNodes[0] == way.WayNodes[way.WayNodes.Count - 1])
        {
            for (int i = 0; i < way.WayNodes.Count - 1; i++)
            {
                if (osm.nodes.TryGetValue(way.WayNodes[i], out node))
                {
                    Vector3 pos = new Vector3(node.X, 0, node.Z);
                    layout.Add(new Vertex(pos));
                }
            }
        }
        else
        {

            for (int i = 0; i < way.WayNodes.Count; i++)
            {
                if (osm.nodes.TryGetValue(way.WayNodes[i], out node))
                {
                    Vector3 pos = new Vector3(node.X, 0, node.Z);
                    layout.Add(new Vertex(pos));
                }

            }
        }

        layout.MakeClockwise();

        leveledLayout = layout.LevelUp();

        area = leveledLayout.Triangulate(mesh, MaterialManager.GetMaterial("diffuseBlue"));
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
