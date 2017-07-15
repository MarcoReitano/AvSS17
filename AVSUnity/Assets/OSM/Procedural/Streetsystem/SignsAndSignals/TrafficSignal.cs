using UnityEngine;
#if UNITY_EDITOR
//using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// TrafficSignal.
/// </summary>
public class TrafficSignal :IProceduralObjects
{
    public static Dictionary<TrafficSignal, Tile> TrafficSignals = new Dictionary<TrafficSignal, Tile>();

    public static bool TryCreateFromOSM(OSMNode node, Tile tile)
    {
        if (node.Tags.ContainsKey("highway"))
        {
            if (node.Tags["highway"] == "traffic_signals")
            {
                TrafficSignals.Add(new TrafficSignal(node, tile.Query.OSM), tile);
                return true;
            }
        }
        return false;
    }
    public static void CreateAllMeshes(ModularMesh mesh)
    {
        foreach (KeyValuePair<TrafficSignal, Tile> kV in TrafficSignals)
        {
            kV.Key.CreateMesh(mesh);
        }
    }

    public TrafficSignal(OSMNode node, OSMData osm)
    {
        position = new Vertex(node);
    }

    Vertex position;
    Polygon layout;
    Polygon topLayout;
    Polygon bottomSignalCase;
    Polygon topSignalCase;


    public void CreateMesh(ModularMesh mesh)
    {
        Vertex a = new Vertex(position + Vector3.forward * 0.1f + Vector3.left * 0.1f);
        Vertex b = new Vertex(position + Vector3.forward * 0.1f - Vector3.left * 0.1f);
        Vertex c = new Vertex(position - Vector3.forward * 0.1f - Vector3.left * 0.1f);
        Vertex d = new Vertex(position - Vector3.forward * 0.1f + Vector3.left * 0.1f);
 
        layout = new Polygon(d,c,b,a);
        layout = layout.LevelDown();

        topLayout = layout.Translate(Vector3.up * 2f);

        bottomSignalCase = topLayout.Inset(0.3f);
        topSignalCase = bottomSignalCase.Translate(Vector3.up * 0.9f);

        QuadStrip qS = new QuadStrip(layout, topLayout, mesh, MaterialManager.GetMaterial("diffuseDarkGrey"));
        QuadStrip qS2 = new QuadStrip(topLayout, bottomSignalCase, mesh, MaterialManager.GetMaterial("diffuseDarkGrey"));
        QuadStrip qS3 = new QuadStrip(bottomSignalCase, topSignalCase, mesh, MaterialManager.GetMaterial("diffuseDarkGrey"));
        topSignalCase.Triangulate(mesh, MaterialManager.GetMaterial("diffuseDarkGrey"));
 
        
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
