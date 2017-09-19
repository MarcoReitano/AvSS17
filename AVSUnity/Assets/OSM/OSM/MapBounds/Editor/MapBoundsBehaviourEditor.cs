//using UnityEditor;
//using UnityEngine;
//using System.Collections.Generic;
//using System;
//using System.IO;





//[CustomEditor(typeof(MapBoundsBehaviour))]
//public class MapBoundsBehaviourEditor : Editor
//{

//    [SerializeField]
//    public MapBoundsBehaviour mapBoundsBehaviour;

//    [SerializeField]
//    public MapBounds mapBounds;

//    public void Awake()
//    {
//        mapBoundsBehaviour = (MapBoundsBehaviour)target as MapBoundsBehaviour;
//        mapBounds = mapBoundsBehaviour.mapBounds;
//    }


//    public void OnSceneGUI()
//    {
//        Event currentEvent = Event.current;

//        bool changed = false;

//        //if (currentEvent.isKey)
//        //{
//        //    if (currentEvent.keyCode == KeyCode.UpArrow)
//        //    {
//        //        srtmMapTile.latitude++;
//        //        changed = true;
//        //    }
//        //    else if (currentEvent.keyCode == KeyCode.DownArrow)
//        //    {
//        //        srtmMapTile.latitude--;
//        //        changed = true;
//        //    }
//        //    else if (currentEvent.keyCode == KeyCode.RightArrow)
//        //    {
//        //        srtmMapTile.longitude--;
//        //        changed = true;
//        //    }
//        //    else if (currentEvent.keyCode == KeyCode.LeftArrow)
//        //    {
//        //        srtmMapTile.longitude++;
//        //        changed = true;
//        //    }
//        //}
        
//        //if (changed)
//        //{
//        //    if (srtmMapTile.isFileAvailable())
//        //        srtmMapTile.InitializeTile();
            
//        //}
        
//    }


//    public override void OnInspectorGUI()
//    {
//        EditorGUIUtility.LookLikeInspector();

//        GUILayout.Label("SRTMMapTile-Editor", EditorStyles.boldLabel);

//        EditorGUILayout.Separator();
//        EditorGUILayout.Separator();

//        bool changed = false;
//        int oldIntValue = mapBoundsBehaviour.longitude;
//        mapBoundsBehaviour.longitude = EditorGUILayout.IntSlider("Longitude (West-East)", mapBoundsBehaviour.longitude, -180, 180);
//        if (mapBoundsBehaviour.longitude != oldIntValue)
//            changed = true;

//        oldIntValue = mapBoundsBehaviour.latitude;
//        mapBoundsBehaviour.latitude = EditorGUILayout.IntSlider("Latitude (North-South)", mapBoundsBehaviour.latitude, -180, 180);
//        if (mapBoundsBehaviour.latitude != oldIntValue)
//            changed = true;

//        //if (changed)
//        //{
//        if (mapBoundsBehaviour.isFileAvailable())
//        {
//            GUILayout.Label("The requested File is available.");
//        }
//        else
//        {
//            GUILayout.Label("The requested File is not available. Sorry...");
//        }
//        //}

//        EditorGUILayout.Separator();
//        EditorGUILayout.Separator();

//        mapBoundsBehaviour.reloadContent = EditorGUILayout.Toggle("Re-Download", mapBoundsBehaviour.reloadContent);

//        EditorGUILayout.Separator();
//        EditorGUILayout.Separator();

//        //if (srtmMapTile.isFileAvailable())
//        //{
//        if (GUILayout.Button("Initialize"))
//        {
//            mapBoundsBehaviour.CheckFolders();
//            mapBoundsBehaviour.InitializeTile();
//        }
//        if(changed)
//            if (mapBoundsBehaviour.isFileAvailable())
//                mapBoundsBehaviour.InitializeTile();
//        //}
//        //else
//        //{
//        //    GUILayout.Label("The requested File is not available. Sorry...");
//        //}

//        if (GUI.changed)
//        {
//            EditorUtility.SetDirty(mapBoundsBehaviour);
//        }
//    }
//}
