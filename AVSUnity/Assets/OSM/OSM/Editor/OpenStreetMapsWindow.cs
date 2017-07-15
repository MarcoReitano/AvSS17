using System.IO;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;
using System.Collections.Generic;
using System;
using System.Collections;
using System.Net;
using System.Globalization;
using System.Text;



public class OpenStreetMapsWindow : EditorWindow
{
    public static OpenStreetMapsWindow window;

    [SerializeField]
    private Vector2 _scrollPos;

    public static bool initializedAttributes = false;


    // Add menu named "StreetGraphEditor" to the Window menu
    [MenuItem("Window/OpenStreetMapsWindow")]
    static void Init()
    {
        //OpenStreetMapsWindow window = (OpenStreetMapsWindow)EditorWindow.GetWindow (typeof (OpenStreetMapsWindow));

        

        if (OpenStreetMapsWindow.window == null)
        {
            window = (OpenStreetMapsWindow)EditorWindow.GetWindow(typeof(OpenStreetMapsWindow));
            window.myGuiUtils = new MyGUIUtils();
            window.myGuiUtils.Initialize();
            Debug.Log("Window created");
        }
        else
        {
            Debug.Log("Window Available");
        }

        window.autoRepaintOnSceneChange = true;
        window.title = "OpenStreetMaps-Data";
        window.minSize = new Vector2(395f, 500f);
        window.Show();
        if (!OpenStreetMapsWindow.initializedAttributes)
        {
            Debug.Log("OpenStreetMaps-Data INIT()");
        }
        else
        {
            Debug.Log("OpenStreetMaps-Data INIT() - Already initialized");
        }

    }


    public void OnDestroy()
    {
        Debug.Log("Destroy Window");
        OpenStreetMapsWindow.initializedAttributes = false;
    }


    #region HilfsMethoden
    /// <summary>
    /// start a new element-Group
    /// </summary>
    public static void BeginGroup()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Box("", GUIStyle.none, GUILayout.Width(12f));
        GUILayout.BeginVertical();
    }

    /// <summary>
    /// end last element-group
    /// </summary>
    public static void EndGroup()
    {
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
    }
    #endregion


    /// <summary>
    /// 
    /// </summary>
    void OnSceneGUI()
    {

    }
    
    public string[] servers = {"www.overpass-api.de",
                                  "xapi.openstreetmap.org",
                                  "www.informationfreeway.org",
                                  "api06.dev.openstreetmap.org",
                                  "osmxapi.hypercube.telascience.org",
                                  "Custom"};

    public string[] serverLinks = { "http://www.overpass-api.de/api/xapi?map?bbox=",
                                      "http://xapi.openstreetmap.org/api/0.6/map?bbox=",
                                      "http://www.informationfreeway.org/api/0.6/map?bbox=",
                                      "http://api06.dev.openstreetmap.org/api/0.6/map?bbox=",
                                      "http://osmxapi.hypercube.telascience.org/api/0.6/map?bbox=",
                                      "http://www.overpass-api.de/api/xapi?map?bbox="};


    [SerializeField]
    public List<bool> moving;
    [SerializeField]
    public List<Vector2> points;
    [SerializeField]
    public List<Vector2> pointsCoords;
    public Vector2 point;

    public bool initialized = false;

    public float sidebarWidth = 350f;
    public Vector2 mouseTilePosition;
    public Vector2 mouseTilePositionRelativeToCenter;

    [SerializeField]
    MyGUIUtils myGuiUtils;

    public Texture2D zoomIn;
    public Texture2D zoomOut;
    [SerializeField]
    public Texture2D titleTexture;
    public Texture2D pointTexture;
    public Texture2D selectionTexture;
    public Texture2D navigationTexture;
    public Texture2D downloadTexture;

    [SerializeField]
    public static OSMTileMap tileMap;

    [SerializeField]
    public static OSMMap osmMap;

    [SerializeField]
    public int editorMode;


    public int selectedOSMServerIndex;
    public string theSelectedOSMServerLink;
    public string customOSMServerLink;


    public Vector2 scrollView;

    public MapBounds maxBounds;

    [SerializeField]
    public MapBounds mybounds; // Köln Groß 
    //private MapBounds mybounds = new MapBounds(51.024f, 6.826f, 7.115f, 50.844f);
    //private MapBounds mybounds = new MapBounds(51.0222f, 6.8635f, 7.073f, 50.8866f);
    //private MapBounds mybounds = new MapBounds(50.9432f, 6.9574f, 6.9758f, 50.9334f); // Köln Zentrum
    //private MapBounds mybounds = new MapBounds(51.0535f, 7.5115f, 7.6028f, 50.9915f); // Gummersbach Groß 
    //private MapBounds mybounds = new MapBounds(50.937195f, 6.962967f, 6.963852f, 50.936752f);  // klein Kölnpegel
    //private MapBounds mybounds = new MapBounds(51.0453f, 7.5277f, 7.6043f, 50.9946f);

    //public bool BoundsListSet = false;
    //public List<MapBounds> boundsList = new List<MapBounds>();
    public bool navigationMode = true;



    public GameObject nodePrefab;
    public GameObject entrancePrefab;
    public GameObject theOSMNodesParent;

    //public SRTMMapOld srtmMap;

    public bool reloadContentOSM = false;

    public OSMParser osmParser;


    public GameObject edgePrefab;
    public string osmFilename;



    /// <summary>
    /// Update the GUI
    /// </summary>
    public void OnGUI()
    {
        // Initialize GUI-Utils

        if (!OpenStreetMapsWindow.initializedAttributes)
        {
            myGuiUtils = new MyGUIUtils();
            myGuiUtils.Initialize();

            //mybounds = new MapBounds(50.9517f, 6.9258f, 6.9902f, 50.9202f); // Köln Groß 
            //mybounds = new MapBounds(50.23513f, 8.04716f, 8.07123f, 50.22266f); // Michelbach
            mybounds = MapBounds.Cologne;


            tileMap = new OSMTileMap();
            //tileMap.centerLatitude = mybounds.CenterLatitude;
            //tileMap.centerLongitude = mybounds.CenterLongitude;
            tileMap.centerLatitude = 50.9390d;
            tileMap.centerLongitude = 6.9605d; //7.02744f; //6.9605d;//7.56211f;  Basti 6.93728f;
    
            tileMap.zoomLevel = 13;


            osmMap = new OSMMap();
            osmParser = new OSMParser();

            moving = new List<bool>();
            points = new List<Vector2>();
            pointsCoords = new List<Vector2>();
            point = new Vector2();


            zoomIn = Resources.Load("OSMWindowIcons/zoomIn", typeof(Texture2D)) as Texture2D;
            zoomOut = Resources.Load("OSMWindowIcons/zoomOut", typeof(Texture2D)) as Texture2D;
            titleTexture = Resources.Load("OSMWindowIcons/title", typeof(Texture2D)) as Texture2D;
            //Texture2D titleTexture = Resources.Load("title_small", typeof(Texture2D)) as Texture2D;
            pointTexture = Resources.Load("OSMWindowIcons/marker", typeof(Texture2D)) as Texture2D;
            selectionTexture = Resources.Load("OSMWindowIcons/selection", typeof(Texture2D)) as Texture2D;
            navigationTexture = Resources.Load("OSMWindowIcons/navigation", typeof(Texture2D)) as Texture2D;
            downloadTexture = Resources.Load("OSMWindowIcons/download", typeof(Texture2D)) as Texture2D;


            //nodePrefab = GameObject.Find("BCNodePrefab");
            //if (nodePrefab == null)
            //{
            //    nodePrefab = new GameObject("BCNodePrefab");
            //    nodePrefab.AddComponent(typeof(Node));
            //    nodePrefab.AddComponent<BCIntersection>();
            //}

            //edgePrefab = GameObject.Find("BCEdgePrefab");
            //if (edgePrefab == null)
            //{
            //    edgePrefab = new GameObject("BCEdgePrefab");
            //    edgePrefab.AddComponent<Edge>();
            //    edgePrefab.AddComponent<MeshFilter>();
            //    edgePrefab.AddComponent<MeshRenderer>();
            //    edgePrefab.AddComponent<MeshCollider>();
            //    edgePrefab.AddComponent<BCRoad>();
            //}

            //entrancePrefab = GameObject.Find("BCEntrancePrefab");
            //if (entrancePrefab == null)
            //{
            //    entrancePrefab = new GameObject("BCEntrancePrefab");
            //    entrancePrefab.AddComponent(typeof(BCEntrance));
            //    entrancePrefab.AddComponent<MeshFilter>();
            //    entrancePrefab.AddComponent<MeshRenderer>();
            //    entrancePrefab.AddComponent<MeshCollider>();

            //}


            OpenStreetMapsWindow.initializedAttributes = true;
        }


        // Calculate Center Tile, Position and Coords
        CaculateValues();
        tileMap.DisplayTileMap();

        //osmParser = new OSMParser();
        _scrollPos = GUILayout.BeginScrollView(_scrollPos, GUILayout.ExpandWidth(true));

        GUILayout.BeginVertical("Box", GUILayout.Width(sidebarWidth), GUILayout.ExpandHeight(true));
        GUILayout.Space(100f);

        // Display Title-Texture
        DisplayTitle();

        // Display Zoom controls and execute zoomControl-handling
        ZoomControl();

        // Scroll-Control
        ScrollControl();

        GUILayout.BeginVertical(GUILayout.Width(sidebarWidth), GUILayout.Height(300f));
        EditorGUILayout.BeginHorizontal();
        GUIContent[] toolbarOptions = new GUIContent[2];
        toolbarOptions[0] = new GUIContent(navigationTexture, "Navigation Mode");
        toolbarOptions[1] = new GUIContent(selectionTexture, "Selection Mode");
        editorMode = GUILayout.Toolbar(editorMode, toolbarOptions, GUILayout.Width(zoomButtonWidth), GUILayout.Height(zoomButtonHeight), GUILayout.ExpandWidth(true));
        EditorGUILayout.EndHorizontal();

        HandleNavigationOrSelection();
        GUILayout.EndVertical();

        GUILayout.BeginVertical(GUILayout.Width(sidebarWidth), GUILayout.Height(80f));
        OSMServerSelection();
        GUILayout.EndVertical();
        EditorGUILayout.Separator();

        GUILayout.BeginVertical(GUILayout.Width(sidebarWidth));

        //TileManager.OriginLatitude = this.mybounds.CenterLatitude;
        //TileManager.OriginLongitude = this.mybounds.CenterLongitude;

        if (GUILayout.Button(downloadTexture, GUILayout.Width(zoomButtonWidth), GUILayout.Height(zoomButtonHeight * 2), GUILayout.ExpandWidth(true)))
        {
            //TileManager.OriginLatitude = this.mybounds.CenterLatitude;
            //TileManager.OriginLongitude = this.mybounds.CenterLongitude;

            OSMTileProviderBehaviour.mapBounds = mybounds;

            OSMTileProvider.GetOSMTileGameObjectsInBoundingBoxCutting(OSMTileProviderBehaviour.mapBounds, OSMTileProviderBehaviour.CurrentZoomLevel);

            //osmParser.BoundsList = new List<MapBounds>();
            //osmParser.BoundsList.Add(this.mybounds);

            //osmParser.StartParsingNoChunks(reloadContentOSM);

            //MapBounds.CenterOffset = GeographicCoordinates.ConvertLonLatToXY(
            //           osmParser.Map.Box.CenterLongitude,
            //           osmParser.Map.Box.CenterLatitude,
            //           GeographicCoordinates.MapCenter);

            //GameObject srtmMap = new GameObject();
            //srtmMap.name = "SRTMMap";

            
            //mapTile.bounds = this.mybounds;
            //mapTile.latitude = this.mybounds.South;
            //mapTile.longitude = this.mybounds.West;

            //SRTMMapTile mapTile = srtmMap.AddComponent<SRTMMapTile>();
            //mapTile.ParseToCutMap(this.mybounds);
            //mapTile.transform.position = mapTile.verticesMap[0, 0];
            //srtmMap.active = false;

            //GenerateGraph();

            //foreach (Node tmpNode in Editor.FindSceneObjectsOfType(typeof(Node)))
            //{
            //    tmpNode.FirePositionChanged(NodeEvent.Dirty, 1);
            //}

            EditorUtility.ClearProgressBar();
        }
        GUILayout.BeginHorizontal(GUILayout.Width(sidebarWidth));
        //GUILayout.Label(osmParser.FilePath, GUILayout.Width(sidebarWidth - 30f));
        if (GUILayout.Button("...", GUILayout.Width(25f)))
        {
            osmParser.FilePath = EditorUtility.OpenFilePanel("Open OSM-XML-File...", EditorApplication.applicationContentsPath, "");
            this.osmParser.StartParsingNoChunks(true);
            Debug.Log("Done");
            //if(File.Exists(@osmFilename))
            //    osmParser.FilePath = osmFilename;
        }
        GUILayout.EndHorizontal();


        GUILayout.EndVertical();

        GUILayout.EndScrollView();
        GUILayout.EndVertical();
        Event.current.Use();
    }

    /// <summary>
    /// 
    /// </summary>
    private void HandleNavigationOrSelection()
    {
        switch (editorMode)
        {
            case 0:
                //DisplaySelection();
                points.Clear();
                moving.Clear();
                pointsCoords.Clear();

                LonLatInputBox();
                break;
            case 1:
                if (points.Count == 0)
                {
                    moving.Add(false);
                    points.Add(new Vector2(tileMap.centerTileCoordsOnWindow.x - 100f, tileMap.centerTileCoordsOnWindow.y - 100f));
                    pointsCoords.Add(tileMap.PixelXYToLonLat((int)(tileMap.centerTileCoordsOnWindow.x - 100f), (int)(tileMap.centerTileCoordsOnWindow.y - 100f)));

                    moving.Add(false);
                    points.Add(new Vector2(tileMap.centerTileCoordsOnWindow.x + 100f, tileMap.centerTileCoordsOnWindow.y + 100f));
                    pointsCoords.Add(tileMap.PixelXYToLonLat((int)(tileMap.centerTileCoordsOnWindow.x + 100f), (int)(tileMap.centerTileCoordsOnWindow.y + 100f)));
                }

                for (int i = 0; i < points.Count; i++)
                {
                    if (moving[i])
                    {
                        if (tileMap.isOnMap(Event.current.mousePosition))
                        {
                            points[i] = new Vector2(Event.current.mousePosition.x, Event.current.mousePosition.y);
                            pointsCoords[i] = tileMap.PixelXYToLonLat((int)points[i].x, (int)points[i].y);
                        }
                    }

                    Vector2 markerCoords = tileMap.LonLatToPixelXY(pointsCoords[i].x, pointsCoords[i].y);
                    if (GUI.Button(new Rect(markerCoords.x - 10f, markerCoords.y - 10f, 25f, 25f), pointTexture, EditorStyles.label))
                    {
                        moving[i] = !moving[i];
                    }
                }

                mybounds.SetValues(pointsCoords[0].y, pointsCoords[0].x, pointsCoords[1].x, pointsCoords[1].y);

                DisplaySelection();
                BoundsInputBox();
                break;
        }
    }

    
    /// <summary>
    /// 
    /// </summary>
    private void OSMServerSelection()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("OSM-Server:", MyGUIUtils.boldCenterLabel16, GUILayout.Width(120f));

        selectedOSMServerIndex = EditorGUILayout.Popup(selectedOSMServerIndex, servers, MyGUIUtils.myPopup, GUILayout.ExpandWidth(true));
        GUILayout.EndHorizontal();
        //GUILayout.Label(osmParser.ServerURL, GUILayout.ExpandWidth(true)); // Debug

        if (servers[selectedOSMServerIndex] == "Custom")
        {
            GUILayout.BeginVertical();
            customOSMServerLink = GUILayout.TextField(customOSMServerLink, MyGUIUtils.myTextfield, GUILayout.ExpandWidth(true));
            theSelectedOSMServerLink = customOSMServerLink;
            GUILayout.EndVertical();
        }
        else
        {
            theSelectedOSMServerLink = serverLinks[selectedOSMServerIndex];
        }

        osmParser.ServerURL = theSelectedOSMServerLink;
    }

    /// <summary>
    /// 
    /// </summary>
    private void DisplaySelection()
    {
        if (points.Count == 2)
        {
            Color color = Color.red;
            color.a = 0.3f;
            GUI.color = color;
            tileMap.PrepareMouseTileTexture(Color.red);

            Vector2 nortWest = tileMap.LonLatToPixelXY(mybounds.West, mybounds.North);
            Vector2 southEast = tileMap.LonLatToPixelXY(mybounds.East, mybounds.South);

            GUI.DrawTexture(
                    new Rect(
                        nortWest.x,
                        nortWest.y,
                        southEast.x - nortWest.x,
                        southEast.y - nortWest.y),
                        tileMap.mouseTexture);
            GUI.color = Color.white;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void DisplayTitle()
    {
        float factor = 0.45f; // sidebarWidth / titleTexture.width;
        float titleTextureWidth = titleTexture.width * factor;
        float titleTextureHeight = titleTexture.height * factor;
        GUI.DrawTexture(new Rect(10f, 10f, titleTextureWidth, titleTextureHeight), titleTexture, ScaleMode.ScaleToFit);
    }

    /// <summary>
    /// 
    /// </summary>
    private void CaculateValues()
    {
        tileMap.CaculateValues(this.position, sidebarWidth + 20f, 0f);

        mouseTilePositionRelativeToCenter = tileMap.MouseToTilePositionRelativeToCenter((int)Event.current.mousePosition.x, (int)Event.current.mousePosition.y);
        mouseTilePosition = tileMap.MouseToTilePosition((int)Event.current.mousePosition.x, (int)Event.current.mousePosition.y);
    }

    /// <summary>
    /// 
    /// </summary>
    private void DisplayCoords()
    {
        GUILayout.BeginVertical("Box", GUILayout.Width(sidebarWidth));
        GUILayout.Label(Event.current.mousePosition.x + ", " + Event.current.mousePosition.y);
        GUILayout.Label("CenterTileCoords : " + tileMap.centerTileCoordsOnWindow.x + ", " + tileMap.centerTileCoordsOnWindow.y);
        GUILayout.Label("MousePosition: " + Event.current.mousePosition.x + ", " + Event.current.mousePosition.y);
        GUILayout.Label("CenterTilePosition : " + tileMap.centerTileNumbers.x + ", " + tileMap.centerTileNumbers.y);
        GUILayout.Label("MouseTilePosition: " + mouseTilePosition.x + ", " + mouseTilePosition.y);
        GUILayout.Label("MouseTilePositionRelativeToCenter: " + (mouseTilePositionRelativeToCenter.x) + ", " + (mouseTilePositionRelativeToCenter.y));

        Vector2 mousePositionLonLat = tileMap.PixelXYToLonLat((int)Event.current.mousePosition.x, (int)Event.current.mousePosition.y);
        GUILayout.Label("MouseLonLat: " + mousePositionLonLat.x.ToString("0.0####", CultureInfo.CreateSpecificCulture("en-US")) +
            " / " + mousePositionLonLat.y.ToString("0.0####", CultureInfo.CreateSpecificCulture("en-US")));

        GUILayout.Label("MousePosition:           " + Event.current.mousePosition);
        GUILayout.Label("MouseXY -> LonLat -> XY: " + tileMap.LonLatToPixelXY(mousePositionLonLat.x, mousePositionLonLat.y));

        GUILayout.Label("TileSize: " + tileMap.tileSize);
        GUILayout.Label("DegreesPerPixel: " + tileMap.degreesPerPixel);

        GUILayout.Label("zoom: " + tileMap.zoomLevel + "  lon: " + tileMap.centerLongitude + "  lat: " + tileMap.centerLatitude + "  x=" + (int)tileMap.centerTileNumbers.x + "  y=" + (int)tileMap.centerTileNumbers.y);
        GUILayout.EndVertical();
    }


    #region Zoom Control-Display and Handling

    float zoomButtonHeight = 80f;
    float zoomButtonWidth = 80f;

    /// <summary>
    /// 
    /// </summary>
    private void ZoomControl()
    {
        // Display Zoom-Controls

        GUILayout.BeginHorizontal();
        GUILayout.Label("Zoom-Level \n" + tileMap.zoomLevel.ToString() + "/18", MyGUIUtils.boldCenterLabel, GUILayout.Width(150f), GUILayout.Height(zoomButtonHeight), GUILayout.ExpandWidth(true));
        if (GUILayout.Button(zoomOut, GUILayout.Width(zoomButtonWidth), GUILayout.Height(zoomButtonHeight), GUILayout.ExpandWidth(false)))
        {
            if (tileMap.zoomLevel > 0)
                tileMap.zoomLevel--;
            tileMap.changed = true;
        }

        if (GUILayout.Button(zoomIn, GUILayout.Width(zoomButtonWidth), GUILayout.Height(zoomButtonHeight), GUILayout.ExpandWidth(false)))
        {
            if (tileMap.zoomLevel < 18)
                tileMap.zoomLevel++;
            tileMap.changed = true;

        }
        GUILayout.EndHorizontal();
        GUILayout.Space(30f);

        // Handle MouseWheel-Zoom
        HandleMouseWheelZoom();

        // Handle Keypad-Zoom-Change
        HandleKeypadZoom();
    }

    /// <summary>
    /// 
    /// </summary>
    private void HandleKeypadZoom()
    {
        if (Event.current.isKey)
        {
            switch (Event.current.keyCode)
            {
                case KeyCode.KeypadPlus:
                    if (tileMap.zoomLevel < 18)
                        tileMap.zoomLevel++;
                    break;
                case KeyCode.KeypadMinus:
                    if (tileMap.zoomLevel > 0)
                        tileMap.zoomLevel--;
                    break;

                default:
                    break;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void HandleMouseWheelZoom()
    {
        if (Event.current.type == EventType.ScrollWheel)
        {
            if (Event.current.delta.y < 0)
            {
                if (tileMap.zoomLevel < 18)
                    tileMap.zoomLevel++;
                tileMap.changed = true;
            }
            else
            {
                if (tileMap.zoomLevel > 0)
                    tileMap.zoomLevel--;
                tileMap.changed = true;
            }

        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void ScrollControl()
    {
        if (editorMode == 0)
        {
            if (Event.current.isMouse)
            {
                if (Event.current.button == 0 && Event.current.type == EventType.MouseUp)
                {
                    if (tileMap.isOnMap(Event.current.mousePosition))
                    {
                        tileMap.centerTileNumbers = tileMap.MouseToTilePosition((int)Event.current.mousePosition.x, (int)Event.current.mousePosition.y);
                        Vector2 newPos = GeographicCoordinates.TileToWorldPos((int)tileMap.centerTileNumbers.x, (int)tileMap.centerTileNumbers.y, tileMap.zoomLevel);
                        tileMap.centerLatitude = newPos.y;
                        tileMap.centerLongitude = newPos.x;
                        tileMap.changed = true;
                    }
                }
            }
        }

        if (Event.current.isKey && Event.current.type == EventType.KeyUp)
        {
            // Handle KeyScrolling
            Vector2 newPos = GeographicCoordinates.TileToWorldPos((int)tileMap.centerTileNumbers.x, (int)tileMap.centerTileNumbers.y, tileMap.zoomLevel);
            switch (Event.current.keyCode)
            {
                case KeyCode.UpArrow:
                    newPos = GeographicCoordinates.TileToWorldPos((int)tileMap.centerTileNumbers.x, (int)tileMap.centerTileNumbers.y - 1, tileMap.zoomLevel);
                    break;
                case KeyCode.DownArrow:
                    newPos = GeographicCoordinates.TileToWorldPos((int)tileMap.centerTileNumbers.x, (int)tileMap.centerTileNumbers.y + 1, tileMap.zoomLevel);
                    break;
                case KeyCode.LeftArrow:
                    newPos = GeographicCoordinates.TileToWorldPos((int)tileMap.centerTileNumbers.x - 1, (int)tileMap.centerTileNumbers.y, tileMap.zoomLevel);
                    break;
                case KeyCode.RightArrow:
                    newPos = GeographicCoordinates.TileToWorldPos((int)tileMap.centerTileNumbers.x + 1, (int)tileMap.centerTileNumbers.y, tileMap.zoomLevel);
                    break;
                default:
                    break;
            }
            tileMap.centerLatitude = newPos.y;
            tileMap.centerLongitude = newPos.x;
            tileMap.changed = true;
            //Event.current.Use();
        }

    }
    #endregion


    /// <summary>
    /// Box with control-elements to enter the map-bounds
    /// </summary>
    private void BoundsInputBox()
    {
        EditorGUILayout.Separator();
        //GUILayout.Label("Enter Bounds:", EditorStyles.boldLabel);

        float offset = 10f;
        float lineWidth = 328f;
        float floatFieldWidth = 65f;
        float midSeparatorWidth = 20f;
        float preSeparatorWidth = (lineWidth / 2f) - (floatFieldWidth / 2f) + 5f;
        float labelWidth = (lineWidth - midSeparatorWidth - (2f * floatFieldWidth)) / 2f - offset;


        // The Bounds-Floatfields
        EditorGUILayout.BeginVertical();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(floatFieldWidth * 2 + offset);
        GUILayout.Label("N", MyGUIUtils.boldCenterLabel, GUILayout.Width(floatFieldWidth));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(floatFieldWidth * 2 + offset);
        mybounds.North = (double) EditorGUILayout.FloatField((float)mybounds.North, MyGUIUtils.centerTextFieldStyle, GUILayout.Width(floatFieldWidth), GUILayout.Height(30f));
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(6f);

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("W", MyGUIUtils.boldCenterLabel, GUILayout.Width(floatFieldWidth));
        mybounds.West = (double)EditorGUILayout.FloatField((float)mybounds.West, MyGUIUtils.centerTextFieldStyle, GUILayout.Width(floatFieldWidth), GUILayout.Height(30f));
        GUILayout.Space(floatFieldWidth);
        mybounds.East = (double)EditorGUILayout.FloatField((float)mybounds.East, MyGUIUtils.centerTextFieldStyle, GUILayout.Width(floatFieldWidth), GUILayout.Height(30f));
        GUILayout.Label("E", MyGUIUtils.boldCenterLabel, GUILayout.ExpandWidth(true));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(floatFieldWidth * 2 + offset);
        mybounds.South = (double)EditorGUILayout.FloatField((float)mybounds.South, MyGUIUtils.centerTextFieldStyle, GUILayout.Width(floatFieldWidth), GUILayout.Height(30f));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(floatFieldWidth * 2 + offset);
        GUILayout.Label("S", MyGUIUtils.boldCenterLabel, GUILayout.Width(floatFieldWidth));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();

        EditorGUILayout.Separator();

    }

    /// <summary>
    /// Box with control-elements to enter the map-bounds
    /// </summary>
    private void LonLatInputBox()
    {
        EditorGUILayout.Separator();
        EditorGUILayout.Separator();
        EditorGUILayout.Separator();

        float offset = 20f;
        float lineWidth = 328f;
        float floatFieldWidth = 70f;
        float midSeparatorWidth = 20f;
        float preSeparatorWidth = (lineWidth / 2f) - (floatFieldWidth / 2f) + 5f;
        float labelWidth = (lineWidth - midSeparatorWidth - (2f * floatFieldWidth)) / 2f - offset;


        EditorGUILayout.BeginVertical();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Center-Latitude", MyGUIUtils.boldLeftLabel, GUILayout.ExpandWidth(true));
        double oldValue = tileMap.centerLatitude;
        tileMap.centerLatitude = (double) EditorGUILayout.FloatField((float)tileMap.centerLatitude, MyGUIUtils.centerTextFieldStyle, GUILayout.Width(floatFieldWidth), GUILayout.Height(30f));
        if (oldValue != tileMap.centerLatitude)
            tileMap.changed = true;
        GUILayout.Space(20f);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Center-Longitude", MyGUIUtils.boldLeftLabel, GUILayout.ExpandWidth(true));
        oldValue = tileMap.centerLongitude;
        tileMap.centerLongitude = (double)EditorGUILayout.FloatField((float)tileMap.centerLongitude, MyGUIUtils.centerTextFieldStyle, GUILayout.Width(floatFieldWidth), GUILayout.Height(30f));
        if (oldValue != tileMap.centerLongitude)
            tileMap.changed = true;
        GUILayout.Space(20f);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();
    }

    /// <summary>
    /// 
    /// </summary>
    private static void DisplayOSMBrowserButton()
    {
        // Button to open OpenStreetMap-page in a browser window
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Lookup Bounds:", EditorStyles.boldLabel);
        if (GUILayout.Button("Open OpenStreetMap in browser"))
        {
            if (EditorUtility.DisplayDialog(
                "Open OpenStreetMap in browser?",
                "Do you really wish to open the OpenStreetMap-Page in your browser?",
                "Yes", "No"))
                Application.OpenURL("http://www.openstreetmap.org/export");
        }
        EditorGUILayout.EndHorizontal();
    }

    /// <summary>
    /// 
    /// </summary>
    private void CheckPrefabs()
    {
        theOSMNodesParent = GameObject.Find("OSM-Data");
        if (theOSMNodesParent == null)
            theOSMNodesParent = new GameObject("OSM-Data");
    }

    private class MyGUIUtils
    {

        #region HilfsMethoden
        /// <summary>
        /// start a new element-Group
        /// </summary>
        public static void BeginGroup()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Box("", GUIStyle.none, GUILayout.Width(12f));
            GUILayout.BeginVertical();
        }

        /// <summary>
        /// end last element-group
        /// </summary>
        public static void EndGroup()
        {
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }
        #endregion


        public static Vector2 scrollView;
        public static GUIStyle labelStyle;
        public static GUIStyle leftAlignLabelStyle;
        public static GUIStyle rightAlignLabelStyle;
        public static GUIStyle centerAlignLabelStyle;
        public static GUIStyle centerTextFieldStyle;
        public static GUIStyle boldCenterLabel;
        public static GUIStyle boldLeftLabel;
        public static GUIStyle boldCenterLabel16;
        public static GUIStyle myPopup;
        public static GUIStyle myTextfield;

        public void Initialize()
        {
            labelStyle = new GUIStyle(EditorStyles.boldLabel);
            labelStyle.alignment = TextAnchor.MiddleCenter;

            leftAlignLabelStyle = new GUIStyle(EditorStyles.label);
            leftAlignLabelStyle.alignment = TextAnchor.MiddleLeft;

            rightAlignLabelStyle = new GUIStyle(EditorStyles.label);
            rightAlignLabelStyle.alignment = TextAnchor.MiddleRight;

            centerAlignLabelStyle = new GUIStyle(EditorStyles.label);
            centerAlignLabelStyle.alignment = TextAnchor.MiddleCenter;

            centerTextFieldStyle = new GUIStyle(EditorStyles.textField);
            centerTextFieldStyle.alignment = TextAnchor.MiddleCenter;

            boldCenterLabel = new GUIStyle(EditorStyles.largeLabel);
            boldCenterLabel.fontSize = 22;
            boldCenterLabel.fontStyle = FontStyle.Bold;
            boldCenterLabel.alignment = TextAnchor.MiddleCenter;

            boldLeftLabel = new GUIStyle(EditorStyles.largeLabel);
            boldLeftLabel.fontSize = 22;
            boldLeftLabel.fontStyle = FontStyle.Bold;
            boldLeftLabel.alignment = TextAnchor.MiddleLeft;

            boldCenterLabel16 = new GUIStyle(EditorStyles.largeLabel);
            boldCenterLabel16.fontSize = 16;
            boldCenterLabel16.fontStyle = FontStyle.Bold;
            boldCenterLabel16.alignment = TextAnchor.MiddleLeft;

            myTextfield = new GUIStyle(EditorStyles.textField);
            myTextfield.fontSize = 12;
            myTextfield.alignment = TextAnchor.MiddleLeft;
            myTextfield.fixedHeight = 26f;

            myPopup = new GUIStyle(EditorStyles.popup);
            myPopup.fontSize = 12;
            myPopup.alignment = TextAnchor.MiddleCenter;
            myPopup.fixedHeight = 26f;

        }

    }



}
