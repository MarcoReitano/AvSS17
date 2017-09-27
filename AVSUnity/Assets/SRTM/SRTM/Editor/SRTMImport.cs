//using System;
//using UnityEditor;
//using UnityEngine;


//public class SRTMImport : EditorWindow
//{
//    //private GEODataFoldout window;
//    private EditorWindow parent;
//    private string label;

//    private float MAX_HEIGHTFACTOR = 25f;
//    private float heightFactor = 1.0f;

//    //private float MAX_SMOOTHITERATIONS = 100.0f;
//    //private float smoothIterations = 25.0f;
//    //private float smoothBlend = 0.5f;

//    private float MAX_MARGINWIDTHINKM = 20.0f;
//    private float marginWidthKm = 1.0f;

//    private float MAX_ROUGHNESS = 5f;
//    private float roughness = 1;
    

//    public EditorWindow Parent
//    {
//        get { return parent; }
//        set { parent = value; }
//    }

//    public delegate void FoldoutDelegate();
//    private bool _AdvancedSettings = true;
//    private bool reloadContentSRTM = false;


//    public SRTMParser srtmParser = new SRTMParser();

//    private int selectedSRTMDownloadSite;
    





//    /// <summary>
//    /// The Constructor
//    /// </summary>
//    /// <param name="parent">The parent-EditorWindow that created this window</param>
//    /// <param name="label">The foldout-label for this instance</param>
//    public SRTMImport(EditorWindow parent, string label)
//    {
//        this.parent = parent;
//        this.label = label;

//        // create SRTM-Parser
//        srtmParser = new SRTMParser();
//    }


//    /// <summary>
//    /// 
//    /// </summary>
//    public void OnGUI()
//    {
//        GUILayout.Label("SRTM - Shuttle Radar Topography Mission Data", EditorStyles.boldLabel);

//        // Show controls for downloading SRTM-Data
//        EditorGUILayout.Separator();
//        DownloadSRTMData();

//        // Show controls for setting the TerrainHeights
//        EditorGUILayout.Separator();
//        CalculateAndSetTerrainHeights();

//        //// Show controls for smoothing the Terrain
//        //EditorGUILayout.Separator();
//        //SmoothTerrain();

//        //// Show controls for resetting the to the downloaded data
//        //// After that, the Heights can be calculated with different settings
//        //EditorGUILayout.Separator();
//        //ResetTerrainToDownloadedData();

//        // Show the advanced Settings
//        EditorGUILayout.Separator();
//        ShowAdvancedSettings();
//    }


//    /// <summary>
//    /// 
//    /// </summary>
//    private void DownloadSRTMData()
//    {
        
//        //string[] srtmDownloadSites = new string[] { "asdf", "asdf", "asdf", "asdf", "asdf",
//        //    "http://dds.cr.usgs.gov/srtm/version2_1/SRTM3/Eurasia/", 
//        //    "http://dds.cr.usgs.gov/srtm/version2_1/SRTM3/Africa/",
//        //    "http://dds.cr.usgs.gov/srtm/version2_1/SRTM3/Australia/",
//        //    "http://dds.cr.usgs.gov/srtm/version2_1/SRTM3/Islands/",
//        //    "http://dds.cr.usgs.gov/srtm/version2_1/SRTM3/North_America/",
//        //    "http://dds.cr.usgs.gov/srtm/version2_1/SRTM3/South_America/"};

//        //selectedSRTMDownloadSite = EditorGUILayout.Popup(selectedSRTMDownloadSite, srtmDownloadSites); 
//        marginWidthKm = EditorGUILayout.Slider("Margin (in km)", marginWidthKm, 0.0f, MAX_MARGINWIDTHINKM);

//        if (GUILayout.Button("Download SRTM-Data"))
//        {
//            srtmParser = new SRTMParser();
//            srtmParser.Bounds = MapBounds.MaxBounds(CITY_Editor._GeoData._BoundsList).AddMargin(marginWidthKm);
//            srtmParser.Editorpath = CITY_Editor.Path; // UnityEditor.EditorApplication.applicationContentsPath + @"/CITY_Editor";
//            srtmParser.ReloadContent = reloadContentSRTM;
//            // TODO:   srtmParser.SitePath = srtmDownloadSites[selectedSRTMDownloadSite];
//            srtmParser.SitePath = @"http://dds.cr.usgs.gov/srtm/version2_1/SRTM3/Eurasia/";

//            CITY_Editor._GeoData.SetSRTMMap(srtmParser.Parse());
//        }
//    }


//    /// <summary>
//    /// 
//    /// </summary>
//    private void CalculateAndSetTerrainHeights()
//    {
//        GUI.enabled = CITY_Editor._GeoData.SRTMMapSet;
        
//        if (GUILayout.Button("Calculate and set TerrainHeights"))
//        {
//            SetNewTerrainHeights();
//        }

//        float oldHeightFactor = heightFactor;
//        heightFactor = EditorGUILayout.Slider("Heightfactor", heightFactor, 0.0f, MAX_HEIGHTFACTOR);

//        float oldRoughness = roughness;
//        roughness = EditorGUILayout.Slider("Roughness", roughness, 0.0f, MAX_ROUGHNESS);

//        if ((heightFactor != oldHeightFactor) || (roughness != oldRoughness))
//            SetNewTerrainHeights();

//        GUI.enabled = true;
//    }

//    private void SetNewTerrainHeights()
//    {
//        CITY_Editor._GeoData._SRTMMap.Reset();
//        Terrain terr = Terrain.activeTerrain;
//        CITY_Editor._GeoData._SRTMMap.HeightFactor = heightFactor;
//        CITY_Editor._GeoData._SRTMMap.Roughness = roughness;

//        float[,] map = CITY_Editor._GeoData._SRTMMap.CalculateTerrainHeights(terr);
//        terr.terrainData.SetHeights(0, 0, map);

//        //float startindexNorth = (int)(Math.Floor(GeoData._SRTMMap.Bounds.South / MapBounds.arcsec3) - (int)(Math.Ceiling(Math.Floor(GeoData._SRTMMap.Bounds.South) / MapBounds.arcsec3)));
//        //float startindexEast = (int)(Math.Floor(GeoData._SRTMMap.Bounds.West / MapBounds.arcsec3) - (int)(Math.Ceiling(Math.Floor(GeoData._SRTMMap.Bounds.West) / MapBounds.arcsec3)));
//        //GeoData._OFFSET = new Vector3(startindexNorth * MapBounds.arcsec3Meter, 0.0f, startindexEast * MapBounds.arcsec3Meter);
//        terr.transform.position = Vector3.zero;
//    }


//    ///// <summary>
//    ///// 
//    ///// </summary>
//    //private void SmoothTerrain()
//    //{
//    //    GUILayout.Label("Smooth Terrain:", EditorStyles.boldLabel);
//    //    smoothIterations = EditorGUILayout.Slider("Iterations", (int)smoothIterations, 0.0f, MAX_SMOOTHITERATIONS);
//    //    smoothBlend = EditorGUILayout.Slider("Blend", smoothBlend, 0.0f, 1.0f);

//    //    if (GUILayout.Button("Smooth Terrain"))
//    //    {
//    //        GameObject go = GameObject.Find("Terrain");
//    //        if (go != null)
//    //        {
//    //            TerrainToolkit toolkit = (TerrainToolkit)go.GetComponent(typeof(TerrainToolkit));
//    //            if (toolkit != null)
//    //            {
//    //                toolkit.SmoothTerrain((int)smoothIterations, smoothBlend);
//    //            }
//    //        }
//    //    }
//    //}


//    ///// <summary>
//    ///// 
//    ///// </summary>
//    //private void ResetTerrainToDownloadedData()
//    //{
//    //    GUI.enabled = CITY_Editor._GeoData.SRTMMapSet;
//    //    if (GUILayout.Button("Reset Terrain to Downloaded Data"))
//    //    {
//    //        CITY_Editor._GeoData._SRTMMap.Reset();
//    //    }
//    //    GUI.enabled = true;
//    //}


//    /// <summary>
//    /// 
//    /// </summary>
//    private void ShowAdvancedSettings()
//    {
//        if (_AdvancedSettings = EditorGUILayout.Foldout(_AdvancedSettings, "Advanced Settings"))
//        {
//            reloadContentSRTM = EditorGUILayout.Toggle("reload content", reloadContentSRTM);
//        }
//    }
//}

