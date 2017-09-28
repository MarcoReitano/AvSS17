using UnityEngine;

#if UNITY_EDITOR
//using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// TestBuilding.
/// </summary>
public class Building : IProceduralObjects
{
    public static Dictionary<Building, Tile> Buildings = new Dictionary<Building, Tile>();
    public static void Clear()
    {
        Buildings.Clear();
    }
    public static bool TryCreateFromOSM(OSMWay way, Tile tile)
    {
        if (way.Tags.ContainsKey("building"))
        {
            Buildings.Add(new Building(way, tile.Query.OSM), tile);
            return true;
        }
        return false;         
    }
    public static void CreateAllMeshes()
    {
        foreach (KeyValuePair<Building, Tile> kV in Buildings)
        {
            kV.Key.CreateMesh(kV.Value.TileMesh);
        }
    }
    public static void CreateAllMeshes(ModularMesh mesh)
    {
        foreach (KeyValuePair<Building, Tile> kV in Buildings)
        {
            kV.Key.CreateMesh(mesh);
        }
    }

    public Building(OSMWay way, OSMData osm)
    {
        if (way.Tags.ContainsKey("building:height"))
            float.TryParse(way.Tags["building:height"], out height);
        else
            height = Random.Range(20f, 40f);

        layout = new Polygon();

        OSMNode node;

        //Discard last waynode if its the same as first
        if (way.WayNodes[0] == way.WayNodes[way.WayNodes.Count - 1])
        {
            for (int i = 0; i < way.WayNodes.Count - 1; i++)
            {
                if (osm.nodes.TryGetValue(way.WayNodes[i], out node))
                {
                    Vector3 pos = new Vector3(node.X, SRTMHeightProvider.GetInterpolatedHeight(node.Latitude, node.Longitude), node.Z);
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
                    Vector3 pos = new Vector3(node.X, SRTMHeightProvider.GetInterpolatedHeight(node.Latitude, node.Longitude), node.Z);
                    layout.Add(new Vertex(pos));
                }

            }
        }

        layout.MakeClockwise();
    }

    private Polygon layout;
    private float height;

    public void CreateMesh(ModularMesh mesh)
    {
        Polygon leveledLayout;
        Polygon topLayout1;
        Polygon topLayout2;

        leveledLayout = layout.LevelUp();

        new QuadStrip(leveledLayout, layout, mesh, MaterialManager.GetMaterial("diffuseGray"));
        topLayout1 = leveledLayout.Translate(Vector3.up * height / 2f);
        new QuadStrip(topLayout1, leveledLayout, mesh, MaterialManager.GetMaterial("diffuseGray"));
        topLayout2 = topLayout1.Translate(Vector3.up * height / 2f);
        new QuadStrip(topLayout2, topLayout1, mesh, MaterialManager.GetMaterial("diffuseGray"));
        topLayout2.Triangulate(mesh, MaterialManager.GetMaterial("diffuseGray"));
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
