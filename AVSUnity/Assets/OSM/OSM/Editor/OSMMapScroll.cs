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



public class OSMMapScroll : EditorWindow
{
    private static int MAXZOOMLEVEL = 21;
    private static OSMMapScroll window;
    private static Rect mapRect;
    private float sidebarWidth = 300;

    private Vector2 origin;
    private Vector2 centerOffset;

    private Vector2 scroll;

    [MenuItem("OSM/Map")]
    static void Init()
    {
        OSMMapScroll.window = (OSMMapScroll)EditorWindow.GetWindow(typeof(OSMMapScroll));

        if (OSMMapScroll.window == null)
            window = (OSMMapScroll)EditorWindow.GetWindow(typeof(OSMMapScroll));

        window.autoRepaintOnSceneChange = true;
        window.title = "Open Street Map";
        window.Show();
    }

    #region Server-Locations
    [SerializeField]
    private int OSMServerIndex;
    [SerializeField]
    private string OSMServerURL;
    [SerializeField]
    private string customOSMServerURL;

    private string[] servers = {"www.overpass-api.de",
                                  "xapi.openstreetmap.org",
                                  "www.informationfreeway.org",
                                  "api06.dev.openstreetmap.org",
                                  "osmxapi.hypercube.telascience.org",
                                  "Custom"};
    private string[] serverLinks = { "http://www.overpass-api.de/api/xapi?map?bbox=",
                                      "http://xapi.openstreetmap.org/api/0.6/map?bbox=",
                                      "http://www.informationfreeway.org/api/0.6/map?bbox=",
                                      "http://api06.dev.openstreetmap.org/api/0.6/map?bbox=",
                                      "http://osmxapi.hypercube.telascience.org/api/0.6/map?bbox=",
                                      "http://www.overpass-api.de/api/xapi?map?bbox="};
    #endregion // Server-Locations

    #region Unity Methods
    public void OnDestroy()
    {

    }

    void OnSceneGUI()
    {

    }
    #endregion // Unity Methods

    #region Zoom
    private int _zoomLevel = 2;
    public int ZoomLevel
    {
        get
        {
            return _zoomLevel;
        }
        set
        {
            _zoomLevel = value;
        }
    }

    public int TilesPerRow
    {
        get
        {
            return (int)Mathf.Pow(2f, (float)ZoomLevel);
        }
    }

    private int _tileSizePixels = 256;
    public int TileSizePixels
    {
        get
        {
            return _tileSizePixels;
        }
        set
        {
            _tileSizePixels = value;
            _tileSizePixelsHalf = (int)(_tileSizePixels * 0.5f);
        }
    }

    private int _tileSizePixelsHalf = 128;
    public int TileSizePixelsHalf
    {
        get
        {
            return _tileSizePixelsHalf;
        }
    }

    public DVector2 DegreesPerTile
    {
        get
        {
            double tilesPerRow = TilesPerRow;
            double yDeg = (Math.Atan(Math.Sinh(Math.PI)) * (180d / Math.PI)) * 2;
            return new DVector2(360d / tilesPerRow, yDeg / tilesPerRow);
        }
    }

    public DVector2 DegreesPerPixel
    {
        get
        {
            return new DVector2(DegreesPerTile.x / (double)_tileSizePixels, DegreesPerTile.y / (double)_tileSizePixels);
        }
    }
    #endregion // Zoom

    private void Zoom()
    {
        if (Event.current.type == EventType.ScrollWheel)
        {
            if (Event.current.delta.y < 0)
            {
                // Berechne Geoposition der Maus
                Vector2 center = GUIToCoordinateSystemPosition(Event.current.mousePosition);
                DrawDot(center, Color.green);
                DVector2 geoMouse = CalculateGeoCoords(center);
               
                DrawDot(center, Color.red);
                // Berechne TileIndex im neuen Zoomlevel
                Vector2 tileIndex = CalculateOSMGridIndex((Vector2)geoMouse);
                Debug.Log("GeoMouse: " + geoMouse + "  -->  TileIndex: " + tileIndex);

                origin = (Vector2)geoMouse;

                if (ZoomLevel < MAXZOOMLEVEL)
                    ZoomLevel++;
            }
            else
            {
                if (ZoomLevel > 0)
                    ZoomLevel--;
            }
            Event.current.Use();
        }
    }

    private void Move()
    {
        if (Event.current.type == EventType.MouseDrag)
        {
            centerOffset += Event.current.delta;
        }
    }


    private bool drawGrid = true;
    private bool drawOrigin = true;
    private bool drawMouse = true;

    /// <summary>
    /// Update the GUI
    /// </summary>
    public void OnGUI()
    {
        GUILayout.BeginHorizontal(GUILayout.Width(sidebarWidth));
        GUILayout.Label("TileSizePixels: " + TileSizePixels);
        TileSizePixels = (int)GUILayout.HorizontalSlider(TileSizePixels, 1, position.width);
        GUILayout.EndHorizontal();

        GUILayout.Space(10f);
        GUILayout.Label("DegreesPerTileX: " + DegreesPerTile.x);
        GUILayout.Label("DegreesPerTileY: " + DegreesPerTile.y);
        GUILayout.Label("TilesPerRow: " + TilesPerRow);
        GUILayout.Space(10f);
        GUILayout.Label("DegreesPerPixelX: " + DegreesPerPixel.x);
        GUILayout.Label("DegreesPerPixelY: " + DegreesPerPixel.y);
        GUILayout.Label("ZoomLevel: " + ZoomLevel);
        GUILayout.Space(10f);
        //drawGrid = GUILayout.Toggle(drawGrid, "Draw Tile-Grid");
        //drawOrigin = GUILayout.Toggle(drawOrigin, "Draw Origin");
        //drawMouse = GUILayout.Toggle(drawMouse, "Draw Mouse");

        mapRect = new Rect(400, 100, 640, 480);
        DrawOSMMap(mapRect);

        DisplayCoords();
        Repaint();
    }


    private void DrawOSMMap(Rect mapRect)
    {
        CustomGUIUtils.DrawBox(mapRect, XKCDColors.LightGreen);

        Rect tileRect = new Rect(
            (int)mapRect.xMin + (int)((origin.x - mapRect.xMin) % TileSizePixels),// - TileSizePixels,
            (int)mapRect.yMin + (int)((origin.y - mapRect.yMin) % TileSizePixels) - TileSizePixels,
            TileSizePixels,
            TileSizePixels);

        for (int x = (int)tileRect.xMin; x < mapRect.xMax; x += TileSizePixels)
        {
            for (int y = (int)tileRect.yMin; y < mapRect.yMax; y += TileSizePixels)
            {
                Rect tileRect2 = new Rect(x, y, TileSizePixels, TileSizePixels);
               
                Vector2 tileIndex = CalculateOSMGridIndex(CalculateGridIndex(tileRect2.center));
                

                Rect textureCropRectParameters;
                Rect cropped = CropRectangle(tileRect2, mapRect, out textureCropRectParameters);

                Texture2D tileTexture = OSMTileProvider.DownloadTileTexture((int)tileIndex.x, (int)tileIndex.y, ZoomLevel);
                tileTexture = CropTexture(tileTexture, textureCropRectParameters);
                EditorGUI.DrawPreviewTexture(cropped, tileTexture);
                GUI.Label(tileRect2, tileIndex.ToString());
            }
        }

        if (drawGrid)
            DrawGrid(mapRect);
        if (drawOrigin)
            DrawOrigin(mapRect);
        if (drawMouse)
            DrawMouse(mapRect);

        Vector2 center = GUIToCoordinateSystemPosition(mapRect.center);
        DrawDot(mapRect.center);
        GUI.Label(new Rect(mapRect.center.x, mapRect.center.y, TileSizePixelsHalf, 300), "center:\n" + CalculateGeoCoords(center));

        CustomGUIUtils.DrawOuterFrame(mapRect, 2, XKCDColors.Black);
        Move();
        Zoom();
    }

    private void DrawGrid(Rect mapRect)
    {
        // Grid on offsetCenter with tileSize
        //for (int x = (int)mapRect.xMin + (int)(mapRect.xMin + origin.x % tileSize); x < mapRect.xMax; x += tileSize)
        for (int x = (int)mapRect.xMin + (int)((origin.x - mapRect.xMin) % TileSizePixels); x < mapRect.xMax; x += TileSizePixels)
        {
            //if (x + Event.current.mousePosition.x) % tileSize == 0)
            DrawVerticalLine(mapRect, x, Color.grey);
        }

        for (int y = (int)mapRect.yMin + (int)((origin.y - mapRect.yMin) % TileSizePixels); y < mapRect.yMax; y += TileSizePixels)
        {
            //if (y % tileSize == 0)
            DrawHorizontalLine(mapRect, y, Color.grey);
        }

    }

    private void DrawOrigin(Rect mapRect)
    {
        origin = mapRect.center + centerOffset;
        if (mapRect.Contains(origin))
        {
            DrawDot(origin);
            GUI.Label(new Rect(origin.x, origin.y, 200, 20), "origin (0/0)");
            DrawVerticalLine(mapRect, origin.x, Color.black);
            DrawHorizontalLine(mapRect, origin.y, Color.black);
        }
    }

    private void DrawMouse(Rect mapRect)
    {
        // MousePosition
        DrawVerticalLine(mapRect, Event.current.mousePosition.x, Color.red);
        DrawHorizontalLine(mapRect, Event.current.mousePosition.y, Color.red);

        if (mapRect.Contains(Event.current.mousePosition))
        {
            Vector2 mouseRelative = GUIToCoordinateSystemPosition(Event.current.mousePosition);
            DVector2 geoMouse = CalculateGeoCoords(mouseRelative);
            //DVector2 tileIndex = WorldToTilePos(geoMouse.x, geoMouse.y, ZoomLevel);
            //tileIndex = new Vector2(Mathf.Floor(tileIndex.x), Mathf.Floor(tileIndex.y)); 
            //Vector2 grid = CalculateGridIndex(mouseRelative);

            DrawDot(Event.current.mousePosition);
            GUI.Label(new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y - 12, 200, 100),
                //"mouse\n\tmouseRelative(" + mouseRelative.x + "," + mouseRelative.y + ")\n" +
                //"\tTileIndex   (" + tileIndex.x.ToString("0.00") + "," + tileIndex.y.ToString("0.00") + ")\n" +
                //"\tTileIndex   (" + Math.Floor(tileIndex.x).ToString("0.00") + "," + Math.Ceiling(tileIndex.y).ToString("0.00") + ")\n" +
                "(" + geoMouse.x.ToString("0.00") + "," + geoMouse.y.ToString("0.00") + ")");
            //"\tgrid        (" + grid.x.ToString("0") + "," + grid.y.ToString("0") + ")\n" +
            //"\t            (" + grid.x.ToString("0") + "," + grid.y.ToString("0") + ")");

            //GUI.Label(new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y - 12, 200, 60),
            //    tileIndex.x.ToString("###") + "/" + tileIndex.y.ToString("###"));
        }
    }

    #region Rectangle Methods
    public bool RectanglesOverlap(Rect a, Rect b)
    {
        if (a.xMin < b.xMax && a.xMax > b.xMin && a.yMin < b.yMax && a.yMax > b.yMin)
            return true;
        return false;
    }

    public Rect CropRectangle(Rect a, Rect cropRect)
    {
        Rect result = new Rect(a);
        result.xMin = Math.Max(a.xMin, cropRect.xMin);
        result.xMax = Math.Min(a.xMax, cropRect.xMax);

        result.yMin = Math.Max(a.yMin, cropRect.yMin);
        result.yMax = Math.Min(a.yMax, cropRect.yMax);

        return result;
    }

    public Rect CropRectangle(Rect a, Rect cropRect, out Rect parametricRect)
    {
        Rect result = new Rect(a);
        parametricRect = new Rect(0, 0, 1, 1);

        if (a.xMin > cropRect.xMin)
        {
            result.xMin = a.xMin;
            parametricRect.xMin = 0f;
        }
        else
        {
            result.xMin = cropRect.xMin;
            parametricRect.xMin = a.xMin.Distance(cropRect.xMin) / a.width;
        }

        if (a.xMax < cropRect.xMax)
        {
            result.xMax = a.xMax;
            parametricRect.xMax = 1f;
        }
        else
        {
            result.xMax = cropRect.xMax;
            parametricRect.xMax = a.xMin.Distance(cropRect.xMax) / a.width;
        }


        if (a.yMin > cropRect.yMin)
        {
            result.yMin = a.yMin;
            parametricRect.yMin = 0f;
        }
        else
        {
            result.yMin = cropRect.yMin;
            parametricRect.yMin = a.yMin.Distance(cropRect.yMin) / a.height;
        }


        if (a.yMax < cropRect.yMax)
        {
            result.yMax = a.yMax;
            parametricRect.yMax = 1f;
        }
        else
        {
            result.yMax = cropRect.yMax;
            parametricRect.yMax = a.yMin.Distance(cropRect.yMax) / a.height;
        }

        return result;
    }

    public Texture2D CropTexture(Texture2D texture, Rect cropRect)
    {
        int x = Mathf.FloorToInt(texture.width * cropRect.xMin);
        int y = texture.height - Mathf.FloorToInt(texture.height * cropRect.yMax);

        int blockWidth = Mathf.Clamp((int)(texture.width * cropRect.width), 1, texture.width);
        int blockHeight = Mathf.Clamp((int)(texture.height * cropRect.height), 1, texture.height);

        Texture2D result = new Texture2D(blockWidth, blockHeight);
        if (x + blockWidth <= texture.width && y + blockHeight <= texture.height)
        {
            Color[] pixels = texture.GetPixels(x, y, blockWidth, blockHeight);
            if (pixels.Length == blockWidth * blockHeight)
            {
                result.SetPixels(pixels);
                result.Apply();
            }
        }
        return result;
    }
    #endregion Rectangle Methods

    #region Tile-, Coords- Converstion-Methods
    // http://wiki.openstreetmap.org/wiki/Slippy_map_tilenames
    /// <summary>
    /// 
    /// </summary>
    /// <param name="lon"></param>
    /// <param name="lat"></param>
    /// <param name="zoom"></param>
    /// <returns></returns>
    public static DVector2 WorldToTilePos(double lon, double lat, int zoom)
    {
        DVector2 p = new DVector2();
        p.x = (lon + 180.0d) / 360.0d * (1 << zoom);
        p.y = (1.0d - Math.Log(Math.Tan(lat * Math.PI / 180.0d) +
            1.0d / Math.Cos(lat * Math.PI / 180.0d)) / Math.PI) / 2.0d * (1 << zoom);

        p.x = (int)p.x.Clamp(0, Math.Pow(2, MAXZOOMLEVEL));
        p.y = (int)p.y.Clamp(0, Math.Pow(2, MAXZOOMLEVEL));
        return p;
    }

    public static DVector2 TileToWorldPos(double tile_x, double tile_y, int zoom)
    {
        DVector2 p = new DVector2();
        double n = Math.PI - ((2.0 * Math.PI * tile_y) / Math.Pow(2.0, zoom));

        p.x = (double)((tile_x / Math.Pow(2.0, zoom) * 360.0) - 180.0);
        p.y = (double)(180.0 / Math.PI * Math.Atan(Math.Sinh(n)));

        return p;
    }

    private Vector2 GUIToCoordinateSystemPosition(Vector2 position)
    {
        return new Vector2(position.x - origin.x, -1 * (position.y - origin.y));
    }

    private DVector2 CalculateGeoCoords(DVector2 coords)
    {
        DVector2 geoCoords = new DVector2(coords.x * DegreesPerPixel.x, coords.y * DegreesPerPixel.y);

        DVector2 p = new DVector2();
        double n = Math.PI - ((2.0 * Math.PI * coords.y) / (TileSizePixels * Math.Pow(2.0, ZoomLevel)));
        p.x = (Math.Abs(geoCoords.x) % 360d) - 180d;
        p.y = (double)-(180.0 / Math.PI * Math.Atan(Math.Sinh(n)));

        return p;
    }

    private Vector2 CalculateGridIndex(Vector2 position)
    {

        // TODO: Handle negative values
        float x = -(origin.x - position.x) / (float)TileSizePixels;
        float y = (origin.y - position.y) / (float)TileSizePixels;

        //if (position.x > 0)
        //    x = Mathf.Ceil(x);
        //else
        x = Mathf.Floor(x);

        //if (position.y > 0)
        //    y = Mathf.Ceil(y);
        //else
        y = Mathf.Floor(y);

        return new Vector2(x, y);
    }

    private Vector2 CalculateOSMGridIndex(Vector2 position)
    {
        float x, y;
        if (position.x < 0)
        {
            x = Mathf.Abs(TilesPerRow - Mathf.Abs(position.x) % TilesPerRow);
        }
        else
        {
            x = Mathf.Abs(Mathf.Abs(position.x) % TilesPerRow);
        }


        if (position.y < 0)
        {
            y = Mathf.Abs(Mathf.Abs(position.y) % TilesPerRow - 1);
        }
        else
        {
            y = Mathf.Abs(TilesPerRow - Mathf.Abs(position.y) % TilesPerRow - 1);
        }
        return new Vector2(x, y);
    }

    private Rect CartesianGrid(Rect mapRect)
    {
        // Grid on offsetCenter with tileSize
        for (int x = (int)origin.x % TileSizePixels; x < mapRect.xMax; x += TileSizePixels)
        {
            //if (x + Event.current.mousePosition.x) % tileSize == 0)
            DrawVerticalLine(mapRect, x, Color.grey);
        }

        for (int y = (int)origin.y % TileSizePixels; y < mapRect.yMax; y += TileSizePixels)
        {
            //if (y % tileSize == 0)
            DrawHorizontalLine(mapRect, y, Color.grey);
        }
        return mapRect;
    }
    #endregion // Tile-, Coords- Converstion-Methods




    #region Download Tile Image
    FileStream fileStream;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="url"></param>
    /// <param name="tmpFile"></param>
    public void DownloadFile(string url, string tmpFile)
    {
        fileStream = File.Create(tmpFile);

        // request file from server
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        Stream stream = response.GetResponseStream();

        int buffersize = 65536 * 2; // TODO: Buffersize selectable 524288 //16384;
        byte[] buffer = new byte[buffersize];
        int len;
        do
        {
            len = stream.Read(buffer, 0, buffersize);
            if (len < 1)
                break;
            fileStream.Write(buffer, 0, len);
        } while (len > 0);
        fileStream.Close();
    }
    #endregion // Download Tile Image

    /// <summary>
    /// 
    /// </summary>
    private void DisplayCoords()
    {
        //GUILayout.BeginVertical("Box", GUILayout.Width(mapRect.width));
        //MousePosition
        //if (mapRect.Contains(Event.current.mousePosition))
        //{
        //    DrawDot(new Vector2(Event.current.mousePosition.x, Event.current.mousePosition.y));
        //    GUI.Label(new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y - 12, 100, 20), Event.current.mousePosition.x + ", " + Event.current.mousePosition.y);
        //}

        // Mouseposition relative to center
        //if (mapRect.Contains(Event.current.mousePosition))
        //{
        //    Vector2 mouseRelativeToCenter = new Vector2(Event.current.mousePosition.x - origin.x, -1 * (Event.current.mousePosition.y - origin.y));
        //    DrawDot(Event.current.mousePosition);
        //    GUI.Label(new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y - 12, 100, 20), mouseRelativeToCenter.x + ", " + mouseRelativeToCenter.y);
        //}


        //GUILayout.Label("CenterTileCoords : " + tileMap.centerTileCoordsOnWindow.x + ", " + tileMap.centerTileCoordsOnWindow.y);
        //GUILayout.Label("MousePosition: " + Event.current.mousePosition.x + ", " + Event.current.mousePosition.y);
        //GUILayout.Label("CenterTilePosition : " + tileMap.centerTileNumbers.x + ", " + tileMap.centerTileNumbers.y);
        //GUILayout.Label("MouseTilePosition: " + mouseTilePosition.x + ", " + mouseTilePosition.y);
        //GUILayout.Label("MouseTilePositionRelativeToCenter: " + (mouseTilePositionRelativeToCenter.x) + ", " + (mouseTilePositionRelativeToCenter.y));

        //Vector2 mousePositionLonLat = tileMap.PixelXYToLonLat((int)Event.current.mousePosition.x, (int)Event.current.mousePosition.y);
        //GUILayout.Label("MouseLonLat: " + mousePositionLonLat.x.ToString("0.0####", CultureInfo.CreateSpecificCulture("en-US")) +
        //    " / " + mousePositionLonLat.y.ToString("0.0####", CultureInfo.CreateSpecificCulture("en-US")));

        //GUILayout.Label("MousePosition:           " + Event.current.mousePosition);
        //GUILayout.Label("MouseXY -> LonLat -> XY: " + tileMap.LonLatToPixelXY(mousePositionLonLat.x, mousePositionLonLat.y));

        //GUILayout.Label("TileSize: " + tileMap.tileSize);
        //GUILayout.Label("DegreesPerPixel: " + tileMap.degreesPerPixel);

        //GUILayout.Label("zoom: " + tileMap.zoomLevel + "  lon: " + tileMap.centerLongitude + "  lat: " + tileMap.centerLatitude + "  x=" + (int)tileMap.centerTileNumbers.x + "  y=" + (int)tileMap.centerTileNumbers.y);
        //GUILayout.EndVertical();
    }

    #region Drawing Utils
    private static void DrawDot(Vector2 center, Color color)
    {
        CustomGUIUtils.DrawBox(new Rect(center.x - 1, center.y - 1, 3, 3), color);
    }

    private static void DrawDot(Vector2 center)
    {
        CustomGUIUtils.DrawBox(new Rect(center.x - 1, center.y - 1, 3, 3), Color.black);
    }

    private static void DrawHorizontalLine(Rect rect, float y, Color color)
    {
        //if (y > rect.yMin && y < rect.yMax)
        CustomGUIUtils.DrawBox(new Rect(rect.xMin, y, rect.width, 1), color);
    }

    private static void DrawVerticalLine(Rect rect, float x, Color color)
    {
        //if (x > rect.xMin && x < rect.xMax)
        CustomGUIUtils.DrawBox(new Rect(x, rect.yMin, 1, rect.height), color);
    }
    #endregion // Drawing Utils

}