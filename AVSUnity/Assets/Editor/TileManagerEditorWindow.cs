using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;

/// <summary>
/// TileManagerEditorWindow.
/// </summary>
public class TileManagerEditorWindow : EditorWindow 
{
    [MenuItem("City/TileManager")]
    static void Init()
    {
        EditorWindow.GetWindow<TileManagerEditorWindow>();
    }

    void OnGUI()
    {
        TileManager.TileWidth = (double)EditorGUILayout.FloatField("TileWidth", (float)TileManager.TileWidth);
        TileManager.tileRadius = EditorGUILayout.IntField("TileRadius", TileManager.tileRadius);
        TileManager.LOD = EditorGUILayout.IntField("LOD", TileManager.LOD);
        TileManager.OriginLatitude = (double)EditorGUILayout.FloatField("OriginLatitude", (float)TileManager.OriginLatitude);
        TileManager.OriginLongitude = (double)EditorGUILayout.FloatField("OriginLongitude", (float)TileManager.OriginLongitude);

        if (GUILayout.Button("Create TileMap"))
            TileManager.CreateTileMap();
        if (GUILayout.Button("GetOriginFromBounds Cologne"))
        {
            OSMTileProviderBehaviour.mapBounds = MapBounds.Cologne;
            TileManager.OriginLatitude = OSMTileProviderBehaviour.mapBounds.CenterLatitude;
            TileManager.OriginLongitude = OSMTileProviderBehaviour.mapBounds.CenterLongitude;

            ReloadOSMTiles();
        }
        if (GUILayout.Button("GetOriginFromBounds Gummersbach"))
        {
            OSMTileProviderBehaviour.mapBounds = MapBounds.Gummersbach;
            TileManager.OriginLatitude = OSMTileProviderBehaviour.mapBounds.CenterLatitude;
            TileManager.OriginLongitude = OSMTileProviderBehaviour.mapBounds.CenterLongitude;

            ReloadOSMTiles();
        }
        if (GUILayout.Button("GetOriginFromBounds SanJose"))
        {
            OSMTileProviderBehaviour.mapBounds = MapBounds.SanJose;
            TileManager.OriginLatitude = OSMTileProviderBehaviour.mapBounds.CenterLatitude;
            TileManager.OriginLongitude = OSMTileProviderBehaviour.mapBounds.CenterLongitude;
            ReloadOSMTiles();
        }
        if (GUILayout.Button("GetOriginFromBounds Warszawa"))
        {
            OSMTileProviderBehaviour.mapBounds = MapBounds.Warszawa;
            TileManager.OriginLatitude = OSMTileProviderBehaviour.mapBounds.CenterLatitude;
            TileManager.OriginLongitude = OSMTileProviderBehaviour.mapBounds.CenterLongitude;
            ReloadOSMTiles();
        }


    }

    private static void ReloadOSMTiles()
    {
        GameObject osmTileProviderBehaviour = GameObject.Find("OSMTiles");
        osmTileProviderBehaviour.gameObject.DeleteChildren();
        OSMTileProvider.Clear();

        OSMTileProvider.GetOSMTileGameObjectsInBoundingBox(OSMTileProviderBehaviour.mapBounds, OSMTileProviderBehaviour.CurrentZoomLevel);
    }
}
