//using UnityEditor;
//using UnityEngine;
//using System.Collections.Generic;
//using System;
//using System.IO;





//[CustomEditor(typeof(SRTMMapTile))]
//public class SRTMMapTileEditor : Editor
//{

//    [SerializeField]
//    public SRTMMapTile srtmMapTile;



//    public void Awake()
//    {
//        srtmMapTile = (SRTMMapTile)target as SRTMMapTile;
//    }


//    public void OnSceneGUI()
//    {
//        //Event currentEvent = Event.current;

//        //bool changed = false;

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
//        //    currentEvent.Use();
//        //}

//    }


//    public override void OnInspectorGUI()
//    {
//        //EditorGUIUtility.LookLikeInspector();

//        GUILayout.Label("SRTMMapTile-Editor", EditorStyles.boldLabel);

//        EditorGUILayout.Separator();
//        EditorGUILayout.Separator();

//        bool changed = false;
//        int oldIntValue = srtmMapTile.longitude;
//        srtmMapTile.longitude = EditorGUILayout.IntSlider("Longitude (West-East)", srtmMapTile.longitude, -180, 180);
//        if (srtmMapTile.longitude != oldIntValue)
//            changed = true;

//        oldIntValue = srtmMapTile.latitude;
//        srtmMapTile.latitude = EditorGUILayout.IntSlider("Latitude (North-South)", srtmMapTile.latitude, -180, 180);
//        if (srtmMapTile.latitude != oldIntValue)
//            changed = true;

//        if (changed)
//        {
//            if (srtmMapTile.isFileAvailable())
//            {
//                GUILayout.Label("The requested File is available.");
//            }
//            else
//            {
//                GUILayout.Label("The requested File is not available. Sorry...");
//            }
//        }

//        EditorGUILayout.Separator();
//        EditorGUILayout.Separator();

//        srtmMapTile.reloadContent = EditorGUILayout.Toggle("Re-Download", srtmMapTile.reloadContent);

//        EditorGUILayout.Separator();
//        EditorGUILayout.Separator();

//        //if (srtmMapTile.isFileAvailable())
//        //{
//        if (GUILayout.Button("Initialize"))
//        {
//            srtmMapTile.CheckFolders();
//            srtmMapTile.InitializeTile();
//        }
//        if (changed)
//            if (srtmMapTile.isFileAvailable())
//                srtmMapTile.InitializeTile();
//        //}
//        //else
//        //{
//        //    GUILayout.Label("The requested File is not available. Sorry...");
//        //}

//        GUILayout.BeginVertical();
//        GUILayout.BeginHorizontal();
//        GUILayout.Space(25f);
//        if (GUILayout.Button("^", GUILayout.Width(25f)))
//        {
//            srtmMapTile.latitude += 1;
//            srtmMapTile.InitializeTile();
//        }
//        GUILayout.EndHorizontal();

//        GUILayout.BeginHorizontal();
//        if (GUILayout.Button("<", GUILayout.Width(25f)))
//        {
//            srtmMapTile.longitude -= 1;
//            srtmMapTile.InitializeTile();
//        }
//        GUILayout.Space(20f);

//        if (GUILayout.Button(">", GUILayout.Width(25f)))
//        {
//            srtmMapTile.longitude += 1;
//            srtmMapTile.InitializeTile();
//        }
//        GUILayout.EndHorizontal();

//        GUILayout.BeginHorizontal();
//        GUILayout.Space(25f);

//        if (GUILayout.Button("v", GUILayout.Width(25f)))
//        {
//            srtmMapTile.latitude -= 1;
//            srtmMapTile.InitializeTile();
//        }
//        GUILayout.EndHorizontal();
//        GUILayout.EndVertical();


//        if (GUI.changed)
//        {
//            EditorUtility.SetDirty(srtmMapTile);
//        }
//    }
//}
