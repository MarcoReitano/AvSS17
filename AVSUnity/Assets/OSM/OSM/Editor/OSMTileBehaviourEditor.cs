using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;





[CustomEditor(typeof(OSMTileBehaviour))]
public class OSMTileBehaviourEditor : Editor
{

    [SerializeField]
    public OSMTileBehaviour osmTileBehaviour;

    public void Awake()
    {
        osmTileBehaviour = (OSMTileBehaviour)target as OSMTileBehaviour;

    }


    public void OnSceneGUI()
    {
        Event currentEvent = Event.current;

        //bool changed = false;

        //if (currentEvent.isKey)
        //{
        //    if (currentEvent.keyCode == KeyCode.UpArrow)
        //    {
        //        srtmMapTile.latitude++;
        //        changed = true;
        //    }
        //    else if (currentEvent.keyCode == KeyCode.DownArrow)
        //    {
        //        srtmMapTile.latitude--;
        //        changed = true;
        //    }
        //    else if (currentEvent.keyCode == KeyCode.RightArrow)
        //    {
        //        srtmMapTile.longitude--;
        //        changed = true;
        //    }
        //    else if (currentEvent.keyCode == KeyCode.LeftArrow)
        //    {
        //        srtmMapTile.longitude++;
        //        changed = true;
        //    }
        //}

        //if (changed)
        //{
        //    if (srtmMapTile.isFileAvailable())
        //        srtmMapTile.InitializeTile();
        //    currentEvent.Use();
        //}

    }

    int zoomlevel = 13;
    int currentIndexX = 0;
    int currentIndexY = 0;

    public override void OnInspectorGUI()
    {
        EditorGUIUtility.LookLikeInspector();

        GUILayout.Label("OSMMapTile-Editor", EditorStyles.boldLabel);

        int oldZoomLevel = OSMTileProviderBehaviour.CurrentZoomLevel;
        OSMTileProviderBehaviour.CurrentZoomLevel = EditorGUILayout.IntSlider("ZoomLevel", OSMTileProviderBehaviour.CurrentZoomLevel, 0, 18);
        if (OSMTileProviderBehaviour.CurrentZoomLevel != oldZoomLevel)
        {
            //OSMTileProvider.PrepareZoomGameObjects(oldZoomLevel);
            OSMTileProvider.SetZoomLevelVisible(oldZoomLevel, false);
            OSMTileProvider.SetZoomLevelVisible(OSMTileProviderBehaviour.CurrentZoomLevel, true);
        }


        EditorGUILayout.IntSlider("X", osmTileBehaviour.tile.X, 0, OSMTileProvider.TileCountForZoomLevel(zoomlevel) - 1);
        EditorGUILayout.IntSlider("Y", osmTileBehaviour.tile.Y, 0, OSMTileProvider.TileCountForZoomLevel(zoomlevel) - 1);

        //if (GUILayout.Button("Download Tile"))
        //{
        //    OSMTileProvider.GetOSMTileGameObject(currentIndexX, currentIndexY, zoomlevel);
        //}

        if (GUILayout.Button("Download SuperTile"))
            osmTileBehaviour.tile.GetSuperTileGameObject();

        if (GUILayout.Button("Download SubTile"))
            osmTileBehaviour.tile.GetSubTilesGameObjects();

        if (GUILayout.Button("Load Neighbours"))
            osmTileBehaviour.tile.GetNeightboursGameObjects();


        EditorGUILayout.Separator();
        EditorGUILayout.Separator();

        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();
        GUILayout.Space(25f);
        if (GUILayout.Button("^", GUILayout.Width(25f)))
            OSMTileProvider.GetOSMTileGameObject(osmTileBehaviour.tile.North);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("<", GUILayout.Width(25f)))
            OSMTileProvider.GetOSMTileGameObject(osmTileBehaviour.tile.West);
        GUILayout.Space(20f);

        if (GUILayout.Button(">", GUILayout.Width(25f)))
            OSMTileProvider.GetOSMTileGameObject(osmTileBehaviour.tile.East);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Space(25f);

        if (GUILayout.Button("v", GUILayout.Width(25f)))
            OSMTileProvider.GetOSMTileGameObject(osmTileBehaviour.tile.South);
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();



        if (GUI.changed)
        {
            EditorUtility.SetDirty(osmTileBehaviour);
        }

        DrawDefaultInspector();
    }
}
