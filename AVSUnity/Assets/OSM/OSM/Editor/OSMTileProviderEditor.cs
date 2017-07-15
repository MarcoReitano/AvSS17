using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;

[CustomEditor(typeof(OSMTileProviderBehaviour))]
public class OSMTileProviderEditor : Editor
{

    [SerializeField]
    public OSMTileProviderBehaviour osmTileProviderBehaviour;

    public void Awake()
    {
        osmTileProviderBehaviour = (OSMTileProviderBehaviour)target as OSMTileProviderBehaviour;

        Vector2 tilePosition = OSMTileProvider.LonLat2TileIndex(TileManager.OriginLongitude, TileManager.OriginLatitude, 13);
        currentIndexX = (int)tilePosition.x;
        currentIndexY = (int)tilePosition.y;
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


    [SerializeField]
    int zoomlevel = 13;
    [SerializeField]
    int currentIndexX = 0;
    [SerializeField]
    int currentIndexY = 0;

    public override void OnInspectorGUI()
    {
        EditorGUIUtility.LookLikeInspector();

        GUILayout.Label("OSMTileProvider", EditorStyles.boldLabel);

        int oldZoomLevel = OSMTileProviderBehaviour.CurrentZoomLevel;
        OSMTileProviderBehaviour.CurrentZoomLevel = EditorGUILayout.IntSlider("ZoomLevel", OSMTileProviderBehaviour.CurrentZoomLevel, 0, 18);
        if (OSMTileProviderBehaviour.CurrentZoomLevel != oldZoomLevel)
        {
            //OSMTileProvider.PrepareZoomGameObjects(oldZoomLevel);
            OSMTileProvider.SetZoomLevelVisible(oldZoomLevel, false);
            OSMTileProvider.SetZoomLevelVisible(OSMTileProviderBehaviour.CurrentZoomLevel, true);
        }


        currentIndexX = EditorGUILayout.IntSlider("X", currentIndexX, 0, OSMTileProvider.TileCountForZoomLevel(zoomlevel) - 1);
        currentIndexY = EditorGUILayout.IntSlider("Y", currentIndexY, 0, OSMTileProvider.TileCountForZoomLevel(zoomlevel) - 1);

        if (GUILayout.Button("Download Tile"))
        {
            OSMTileProvider.GetOSMTileGameObject(currentIndexX, currentIndexY, zoomlevel);
        }


        if (GUILayout.Button("Download Tile (with Neighbours)"))
        {
            OSMTileProvider.GetOSMTileGameObject(currentIndexX, currentIndexY, zoomlevel);

            OSMTileProvider.GetOSMTileGameObject(currentIndexX - 1, currentIndexY - 1, zoomlevel);
            OSMTileProvider.GetOSMTileGameObject(currentIndexX - 1, currentIndexY, zoomlevel);
            OSMTileProvider.GetOSMTileGameObject(currentIndexX - 1, currentIndexY + 1, zoomlevel);

            OSMTileProvider.GetOSMTileGameObject(currentIndexX + 1, currentIndexY - 1, zoomlevel);
            OSMTileProvider.GetOSMTileGameObject(currentIndexX + 1, currentIndexY, zoomlevel);
            OSMTileProvider.GetOSMTileGameObject(currentIndexX + 1, currentIndexY + 1, zoomlevel);

            OSMTileProvider.GetOSMTileGameObject(currentIndexX, currentIndexY - 1, zoomlevel);
            OSMTileProvider.GetOSMTileGameObject(currentIndexX, currentIndexY + 1, zoomlevel);
        }


        EditorGUILayout.Separator();
        EditorGUILayout.Separator();

        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();
        GUILayout.Space(25f);
        if (GUILayout.Button("^", GUILayout.Width(25f)))
        {
            currentIndexY -= 1;
            OSMTileProvider.GetOSMTileGameObject(currentIndexX, currentIndexY, zoomlevel);
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("<", GUILayout.Width(25f)))
        {
            currentIndexX -= 1;
            OSMTileProvider.GetOSMTileGameObject(currentIndexX, currentIndexY, zoomlevel);
        }
        GUILayout.Space(20f);

        if (GUILayout.Button(">", GUILayout.Width(25f)))
        {
            currentIndexX += 1;
            OSMTileProvider.GetOSMTileGameObject(currentIndexX, currentIndexY, zoomlevel);
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Space(25f);

        if (GUILayout.Button("v", GUILayout.Width(25f)))
        {
            currentIndexY += 1;
            OSMTileProvider.GetOSMTileGameObject(currentIndexX, currentIndexY, zoomlevel);
        }
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();


        if (GUILayout.Button("ResetProvider --> Delete Children"))
        {
            osmTileProviderBehaviour.gameObject.DeleteChildren();
            OSMTileProvider.Clear();
        }


        if (GUILayout.Button("Download Bounds"))
        {
            OSMTileProvider.GetOSMTileGameObjectsInBoundingBoxCutting(OSMTileProviderBehaviour.mapBounds, OSMTileProviderBehaviour.CurrentZoomLevel);
        }

        //if (GUI.changed)
        //{
        //    EditorUtility.SetDirty(osmTileProviderBehaviour);
        //}

        DrawDefaultInspector();
    }
}
