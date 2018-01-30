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
using System.Threading;



public class OSMMapScroll : EditorWindow
{
    private static int MAXZOOMLEVEL = 21;
    private static OSMMapScroll window;
    private static Rect mapRect;
    private float sidebarWidth = 300;

    private Vector2 origin;
    private Vector2 centerOffset;

    private Vector2 scroll;
    private static Texture2D here;

    [MenuItem("OSM/Map")]
    static void Init()
    {
        here = Resources.Load("here") as Texture2D;
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
    private int _zoomLevel = 1;
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
            return (int)Mathf.Pow(2, ZoomLevel);
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
            double yDeg = (Math.Atan(Math.Sinh(Math.PI)) * (180d / Math.PI)) * 2d;
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
            // Berechne Geoposition der Maus
            Vector2 mouse = Event.current.mousePosition;
            Vector2 center = GUIToCoordinateSystemPosition(mouse);
            DVector2 geoMouse = CalculateGeoCoords(center);

            if (Event.current.delta.y < 0)
            {
                if (ZoomLevel < MAXZOOMLEVEL)
                    ZoomLevel++;
            }
            else
            {
                if (mapRect.width < TileSizePixels * (int)Mathf.Pow(2, ZoomLevel - 1))
                    ZoomLevel--;
            }

          

            // Berechne TileIndex im neuen Zoomlevel
            Vector2 tileIndex = CalculateGridIndex((Vector2)geoMouse);
            //Debug.Log("GeoMouse: " + geoMouse + "  -->  TileIndex: " + tileIndex);

            Vector2 pixelPositionAfterZoom = WorldToPixelPosition(geoMouse.x, geoMouse.y, ZoomLevel);
            pixelOrigin = pixelPositionAfterZoom - mouse;
            PerformMove(Vector2.zero);

            Event.current.Use();
            Repaint();
        }

        if (mapRect.width < TileSizePixels * (int)Mathf.Pow(2, _zoomLevel))
        {
            while (mapRect.width > (TileSizePixels * (int)Mathf.Pow(2, _zoomLevel)))
                _zoomLevel++;
            
            Repaint();
        } 
    }

    int top = 0;
    int bottom = 0;
    int left = 0;
    int right = 0;

    private void Move()
    {
        if (Event.current.type == EventType.MouseDrag)
        {
            PerformMove(Event.current.delta);
        }
    }

    private void PerformMove(Vector2 delta)
    {
        int pixelSize = (TilesPerRow * TileSizePixels);

        int freeSpaceHeight = pixelSize - (int)mapRect.height;
        int freeSpaceWidth = pixelSize - (int)mapRect.width;

        if (mapRect.height >= TilesPerRow * TileSizePixels)
        {
            top = (int)(mapRect.height - (TilesPerRow * TileSizePixels)) / 2;
            bottom = top + (TilesPerRow * TileSizePixels);
            pixelOrigin.y = top;
        }
        else
        {
            top = -(int)pixelOrigin.y % TileSizePixels;
            bottom = top + (TilesPerRow * TileSizePixels);
            pixelOrigin.y = Mathf.Clamp(pixelOrigin.y - delta.y, top, freeSpaceHeight);
        }

        if (mapRect.width >= TilesPerRow * TileSizePixels)
        {
            left = (int)(mapRect.width - (TilesPerRow * TileSizePixels)) / 2;
            right = left + (TilesPerRow * TileSizePixels);
            pixelOrigin.x = left;
        }
        else
        {
            left = -(int)pixelOrigin.x % TileSizePixels;
            right = left + (TilesPerRow * TileSizePixels);
            pixelOrigin.x = Mathf.Clamp(pixelOrigin.x - delta.x, left, freeSpaceWidth);
        }
        Repaint();
    }

    private void SetYouAreHere()
    {
        if (Event.current.type == EventType.ContextClick)
        {
            // Berechne Geoposition der Maus
            Vector2 mouse = Event.current.mousePosition;
            Vector2 center = GUIToCoordinateSystemPosition(mouse);
            DVector2 geoMouse = CalculateGeoCoords(center);

            TileManager.OriginLatitude = geoMouse.y;
            TileManager.OriginLongitude = geoMouse.x;
        }
    }

    private bool drawGrid = true;
    private bool drawOrigin = true;
    private bool drawMouse = true;

    private Vector2 pixelOrigin;
    private Vector2 geoOriginInPixels;

    int mapRectWidth = 800;
    int mapRectHeight = 600;
    /// <summary>
    /// Update the GUI
    /// </summary>
    public void OnGUI()
    {
        mapRectWidth = (int)GUILayout.HorizontalSlider(mapRectWidth, 100, 1000);
        mapRectHeight = (int)GUILayout.HorizontalSlider(mapRectHeight, 100, 1000);
        mapRect = new Rect(300, 100, mapRectWidth, mapRectHeight);

        //GUILayout.Space(10f);
        //GUILayout.Label("DegreesPerTileX: " + DegreesPerTile.x);
        //GUILayout.Label("DegreesPerTileY: " + DegreesPerTile.y);
        //GUILayout.Label("TilesPerRow: " + TilesPerRow);
        //GUILayout.Space(10f);
        //GUILayout.Label("DegreesPerPixelX: " + DegreesPerPixel.x);
        //GUILayout.Label("DegreesPerPixelY: " + DegreesPerPixel.y);
        //GUILayout.Label("ZoomLevel: " + ZoomLevel);
        //GUILayout.Space(10f);
        //GUILayout.Label("pixelOrigin: " + pixelOrigin);
        //GUILayout.Space(10f);

        GUILayout.BeginArea(mapRect);
        mapRect = new Rect(0, 0, mapRect.width, mapRect.height);

        DrawOSMMap(mapRect);
        GUILayout.EndArea();

        //Repaint();
    }


    private void DrawOSMMap(Rect mapRect)
    {
        // Mouse-Actions
        Move();
        Zoom();
        SetYouAreHere();

        // Draw Map
        for (int x = left % TileSizePixels; x < left + mapRect.width + TileSizePixels; x += TileSizePixels)
        {
            for (int y = top % TileSizePixels; y < top + mapRect.height + TileSizePixels; y += TileSizePixels)
            {
                Rect tileRect = new Rect(x, y, TileSizePixels, TileSizePixels);
                Vector2 tileIndex = CalculateGridIndex(GUIToCoordinateSystemPosition(tileRect.center));
                Texture2D tileTexture = OSMTileProvider.DownloadTileTexture((int)tileIndex.x, (int)tileIndex.y, ZoomLevel);

                GUI.DrawTexture(tileRect, tileTexture, ScaleMode.ScaleToFit);
                //GUI.Label(tileRect, "TileIndex: " + tileIndex.ToString());
            }
        }

        // Draw TileManager-Selection/Boundings-Settings
        DrawSelection();
        DrawYouAreHere();

        #region Debug-Draw-Methods
        //if (drawGrid)
        //    DrawGrid(mapRect);
        //if (drawOrigin)
        //    DrawOrigin(mapRect);
        //if (drawMouse)
        //    DrawMouse(mapRect);
        #endregion // Debug-Draw-Methods

        CustomGUIUtils.DrawOuterFrame(mapRect, 2, XKCDColors.Black);
    }

    private void DrawYouAreHere()
    {
        if (here == null)
            here = Resources.Load("here") as Texture2D;
        Vector2 point = WorldToPixelPosition(TileManager.OriginLongitude, TileManager.OriginLatitude, ZoomLevel);
        Vector2 correctedPoint = point - pixelOrigin;
        GUI.DrawTexture(new Rect(correctedPoint.x - 20, correctedPoint.y - 39, 41, 41), here, ScaleMode.ScaleToFit, true);
    }

    private void DrawSelection()
    {
        Color red = Color.black;
        red.a = 0.6f;

        Color redAlpha = Color.red;
        redAlpha.a = 0.3f;


        int tileRadius = TileManager.tileRadius;
        for (int i = -tileRadius; i <= tileRadius; i++)
        {
            for (int j = -tileRadius; j <= tileRadius; j++)
            {
                OSMBoundingBox box = new OSMBoundingBox (
                    TileManager.OriginLongitude - TileManager.TileWidth / 2d + TileManager.TileWidth * i,
                    TileManager.OriginLatitude - TileManager.TileWidth / 2d + TileManager.TileWidth * j,
                    TileManager.OriginLongitude + TileManager.TileWidth / 2d + TileManager.TileWidth * i,
                    TileManager.OriginLatitude + TileManager.TileWidth / 2d + TileManager.TileWidth * j
                );
                Vector2 pixelPositionMin = WorldToPixelPosition(box.MinLongitude, box.MinLatitude, ZoomLevel);
                Vector2 pixelPositionMax = WorldToPixelPosition(box.MaxLongitude, box.MaxLatitude, ZoomLevel);
                Vector2 pixelSize = pixelPositionMax - pixelPositionMin;
                Rect rect = new Rect(pixelPositionMin.x - pixelOrigin.x - 1, pixelPositionMin.y - pixelOrigin.y + 1, pixelSize.x -2, pixelSize.y + 2);
                //CustomGUIUtils.DrawFrameBox(rect, red, 1f, redAlpha);
                GUI.DrawTexture(rect, CustomGUIUtils.GetSimpleColorTexture(redAlpha), ScaleMode.StretchToFill, true);
                //Debug.Log(rect);
            }
        }

    }

    private void DrawGrid(Rect mapRect)
    {
        for (int x = (int)-pixelOrigin.x % TileSizePixels; x < mapRect.xMax; x += TileSizePixels)
            DrawVerticalLine(mapRect, x, Color.grey);

        for (int y = (int)-pixelOrigin.y % TileSizePixels; y < mapRect.yMax; y += TileSizePixels)
            DrawHorizontalLine(mapRect, y, Color.grey);
    }

    private void DrawOrigin(Rect mapRect)
    {
        if (mapRect.Contains(-pixelOrigin))
        {
            DrawDot(-pixelOrigin);
            DrawVerticalLine(mapRect, -pixelOrigin.x, Color.black);
            DrawHorizontalLine(mapRect, -pixelOrigin.y, Color.black);
        }
    }

    private void DrawMouse(Rect mapRect)
    {
        DrawVerticalLine(mapRect, Event.current.mousePosition.x, Color.red);
        DrawHorizontalLine(mapRect, Event.current.mousePosition.y, Color.red);

        if (mapRect.Contains(Event.current.mousePosition))
        {
            Vector2 mouse = Event.current.mousePosition;
            Vector2 mouseRelative = GUIToCoordinateSystemPosition(mouse);
            DVector2 geoMouse = CalculateGeoCoords(mouseRelative);
            Vector2 grid = CalculateGridIndex(mouseRelative);

            DrawDot(Event.current.mousePosition);
            GUI.Label(new Rect(Event.current.mousePosition.x-100, Event.current.mousePosition.y - 12, 300, 100),
                //"\n\tmouse(" + mouse.x + "," + mouse.y + ")" +
                //"\n\trelative(" + mouseRelative.x + "," + mouseRelative.y + ")" +
                //"\tTileIndex   (" + tileIndex.x.ToString("0.00") + "," + tileIndex.y.ToString("0.00") + ")\n" +
                //"\tTileIndex   (" + Math.Floor(tileIndex.x).ToString("0.00") + "," + Math.Ceiling(tileIndex.y).ToString("0.00") + ")\n" +
                //"\n\tgeoMouse(" + geoMouse.x.ToString("0.00") + "," + geoMouse.y.ToString("0.00") + ")");// +
                "\n   (" + geoMouse.x.ToString("0.00") + "," + geoMouse.y.ToString("0.00") + ")");// +
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

        p.x = (int)p.x.Clamp(0d, Math.Pow(2d, MAXZOOMLEVEL));
        p.y = (int)p.y.Clamp(0d, Math.Pow(2d, MAXZOOMLEVEL));
        return p;
    }

    public static DVector2 TileToWorldPos(double tile_x, double tile_y, int zoom)
    {
        DVector2 p = new DVector2();
        double n = Math.PI - ((2.0d * Math.PI * tile_y) / Math.Pow(2.0d, zoom));

        p.x = (double)((tile_x / Math.Pow(2.0, zoom) * 360.0d) - 180.0d);
        p.y = (double)(180.0d / Math.PI * Math.Atan(Math.Sinh(n)));

        return p;
    }

    private Vector2 GUIToCoordinateSystemPosition(Vector2 position)
    {
        if (mapRect.height >= TilesPerRow * TileSizePixels || mapRect.width >= TilesPerRow * TileSizePixels)
            return -pixelOrigin + position;

        return pixelOrigin + position;
    }

    /*
    public PointF TileToWorldPos(double tile_x, double tile_y, int zoom) 
{
	PointF p = new Point();
	double n = Math.PI - ((2.0 * Math.PI * tile_y) / Math.Pow(2.0, zoom));

	p.X = (float)((tile_x / Math.Pow(2.0, zoom) * 360.0) - 180.0);
	p.Y = (float)(180.0 / Math.PI * Math.Atan(Math.Sinh(n)));

	return p;
}*/


    public Vector2 WorldToPixelPosition(double longitude, double latitude, int zoom)
    {
        double x = ((longitude + 180) / 360) * (TileSizePixels << zoom);

        double sinLatitude = Math.Sin(latitude * Math.PI / 180);
        double y = (0.5 - Math.Log((1 + sinLatitude) / (1 - sinLatitude)) / (4 * Math.PI)) * (TileSizePixels << zoom) + 0.5;

        return new Vector2((int)x, (int)y);
    }


    private DVector2 CalculateGeoCoords(DVector2 coords)
    {
        DVector2 geoCoords = new DVector2(coords.x / (double)TileSizePixels, coords.y / (double)TileSizePixels);

        DVector2 p = new DVector2();
        double n = Math.PI - ((2.0d * Math.PI * geoCoords.y) / Math.Pow(2.0d, ZoomLevel));

        p.x = ((geoCoords.x / Math.Pow(2.0d, ZoomLevel)) * 360d) - 180d;
        p.y = (double)(180.0d / Math.PI * Math.Atan(Math.Sinh(n)));

        return p;
    }

    private Vector2 CalculateGridIndex(Vector2 position)
    {
        float x = position.x / TileSizePixels;
        float y = position.y / TileSizePixels;

        x = (int)Mathf.Clamp(Mathf.Floor(x), 0, TilesPerRow - 1);
        y = (int)Mathf.Clamp(Mathf.Floor(y), 0, TilesPerRow - 1);

        return new Vector2(x, y);
    }

    /*
    From:  https://wiki.openstreetmap.org/wiki/Slippy_map_tilenames#Resolution_and_Scale

    Exact length of the equator (according to wikipedia) is 40075.016686 km in WGS-84. 
    At zoom 0, one pixel would equal 156543.03 meters (assuming a tile size of 256 px): 

    40075.016686 * 1000 / 256 --> 6378137.0 * 2 * pi / 256  --> 156543.03

    Which gives us a formula to calculate resolution at any given zoom:

    resolution = 156543.03 meters/pixel * cos(latitude) / (2 ^ zoomlevel)


    From: https://wiki.openstreetmap.org/wiki/Zoom_levels
    Metres per pixel math

    The distance represented by one pixel (S) is given by

        S=C*cos(y)/2^(z+8)

    where...

        C is the (equatorial) circumference of the Earth
        z is the zoom level
        y is the latitude of where you're interested in the scale.

    Make sure your calculator is in degrees mode, unless you want to express latitude in radians for some reason. 
    C should be expressed in whatever scale unit you're interested in (miles, meters, feet, smoots, whatever). 
    Since the earth is actually ellipsoidal, there will be a slight error in this calculation. 
    But it's very slight. (0.3% maximum error) 
    */



    //private Vector2 CalculateOSMGridIndex(Vector2 position)
    //{
    //    float x, y;
    //    if (position.x < 0)
    //    {
    //        x = Mathf.Abs(TilesPerRow - Mathf.Abs(position.x) % TilesPerRow);
    //    }
    //    else
    //    {
    //        x = Mathf.Abs(Mathf.Abs(position.x) % TilesPerRow);
    //    }


    //    if (position.y < 0)
    //    {
    //        y = Mathf.Abs((Mathf.Abs(position.y)-1) % TilesPerRow);
    //    }
    //    else
    //    {
    //        y = Mathf.Abs(TilesPerRow - 1 - Mathf.Abs(position.y) % TilesPerRow - 1);
    //    }
    //    return new Vector2(x, y);
    //}

    private Rect CartesianGrid(Rect mapRect)
    {
        // Grid on offsetCenter with tileSize
        for (int x = (int)-pixelOrigin.x % TileSizePixels; x < mapRect.xMax; x += TileSizePixels)
        {
            //if (x + Event.current.mousePosition.x) % tileSize == 0)
            DrawVerticalLine(mapRect, x, Color.grey);
        }

        for (int y = (int)-pixelOrigin.y % TileSizePixels; y < mapRect.yMax; y += TileSizePixels)
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
