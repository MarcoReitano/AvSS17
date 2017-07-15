using UnityEngine;
#if UNITY_EDITOR
//using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// River.
/// </summary>
public class River : IProceduralObjects
{
    public static Dictionary<River, Tile> rivers = new Dictionary<River, Tile>();
    public static void Clear()
    {
        rivers.Clear();
    }
    public static bool TryCreateFromOSM(OSMRelation relation, Tile tile)
    {
        if (relation.Tags.ContainsKey("waterway"))
         {
             if (relation.Tags["waterway"] == "riverbank")
             {
                 new River(relation, tile.Query.OSM, tile.TileMesh);
                 return true;
             }
         }
         return false; 
    }
    public static void CreateAllMeshes()
    {
        foreach (KeyValuePair<River, Tile> kV in rivers)
        {
            kV.Key.CreateMesh(kV.Value.TileMesh);
        }
    }

    public River(OSMRelation rel, OSMData osm, ModularMesh mesh)
    {
        Debug.Log("RIVERBANKS");
        //layout = new Polyline();

        //OSMNode node;

        ////Discard last waynode if its the same as first
        //if (way.WayNodes[0] == way.WayNodes[way.WayNodes.Count - 1])
        //{
        //    for (int i = 0; i < way.WayNodes.Count - 1; i++)
        //    {
        //        if (nodes.TryGetValue(way.WayNodes[i], out node))
        //        {
        //            Vector3 pos = new Vector3(node.X, SRTMHeightProvider.GetInterpolatedHeight(node.Latitude, node.Longitude), node.Z);
        //            layout.Add(new Vertex(pos, mesh));
        //        }

        //    }
        //}
        //else
        //{
        //    for (int i = 0; i < way.WayNodes.Count; i++)
        //    {
        //        if (nodes.TryGetValue(way.WayNodes[i], out node))
        //        {
        //            Vector3 pos = new Vector3(node.X, SRTMHeightProvider.GetInterpolatedHeight(node.Latitude, node.Longitude), node.Z);
        //            layout.Add(new Vertex(pos, mesh));
        //        }

        //    }
        //}

        ////layout.MakeClockwise();

        //leveledLayout = layout.LevelUp(null);
        //leftBorder = leveledLayout.Inset(-50f, mesh);
        //rightBorder = leveledLayout.Inset(50f, mesh);

        //area = new TriangleStrip(leftBorder, rightBorder, mesh, MaterialManager.GetMaterial("diffuseBlue"));
    }

    Polyline layout;
    Polyline leveledLayout;
    Polyline leftBorder;
    Polyline rightBorder;

    TriangleStrip area;

    public void CreateMesh(ModularMesh mesh)
    {
        throw new System.NotImplementedException();
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
