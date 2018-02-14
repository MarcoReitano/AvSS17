using System;
using UnityEngine;

public class OSMMapRect 
{
    #region Fields
    private static int MAXZOOMLEVEL = 21;
    private static Rect mapRect;
   
    private Vector2 pixelOrigin;

    private Vector2 scroll;
    private static Texture2D here;
    private static GUIStyle lightGreen;

    private int top = 0;
    private int bottom = 0;
    private int left = 0;
    private int right = 0;

    private bool drawGrid = true;
    private bool drawOrigin = true;
    private bool drawMouse = true;

    private LocationQuery locationQuery;
    private string searchString = String.Empty;
    #endregion // Fields

    static void Init()
    {
        here = Resources.Load("here") as Texture2D;
        lightGreen = CustomGUIUtils.GetColorBackgroundStyle(XKCDColors.LightGreen);
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

    #region Attributes
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
    #endregion // Attributes

    #region Trigger repaint
    public bool dirty = false;
    public void SetDirty()
    {
        dirty = true;
    }
    public bool ShouldRepaint()
    {
        bool shouldRepaint = dirty;
        dirty = false;
        return shouldRepaint;
    }
    #endregion //Trigger repaint

    #region Interaction Handling
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
            Vector2 pixelPositionAfterZoom = WorldToPixelPosition(geoMouse.x, geoMouse.y, ZoomLevel);
            pixelOrigin = pixelPositionAfterZoom - mouse;
            PerformMove(Vector2.zero);

            Event.current.Use();
            SetDirty();
        }

        FitZoomLevel();
    }

    /// <summary>
    /// Corrects the Zoom-Level, no borders ar visible
    /// </summary>
    private void FitZoomLevel()
    {
        if (mapRect.width > TileSizePixels * (int)Mathf.Pow(2, _zoomLevel))
        {
            while (mapRect.width > (TileSizePixels * (int)Mathf.Pow(2, _zoomLevel)))
                _zoomLevel++;

            SetDirty();
        }
    }

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
        SetDirty();
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
    #endregion //Interaction Handling

    #region Draw OSM-Map as Rectangle
    public void DrawOSMMapRect(Rect rect)
    {
        GUILayout.BeginArea(rect);
        mapRect = new Rect(0, 0, rect.width, rect.height);
        DrawOSMMap(mapRect);
        GUILayout.EndArea();
    }

    private void DrawOSMMap(Rect mapRect)
    {
        // Handle Interaction Mouse-Actions
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
       
        // Draw Location Search-Bar
        DrawSearchBar();
        DrawSearchResults();

        CustomGUIUtils.DrawOuterFrame(mapRect, 2, XKCDColors.Black);
    }
    #endregion // Draw OSM-Map as Rectangle

    #region Selection using TileManager-Atttributes
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
                OSMBoundingBox box = new OSMBoundingBox(
                    TileManager.OriginLongitude - TileManager.TileWidth / 2d + TileManager.TileWidth * i,
                    TileManager.OriginLatitude - TileManager.TileWidth / 2d + TileManager.TileWidth * j,
                    TileManager.OriginLongitude + TileManager.TileWidth / 2d + TileManager.TileWidth * i,
                    TileManager.OriginLatitude + TileManager.TileWidth / 2d + TileManager.TileWidth * j
                );
                Vector2 pixelPositionMin = WorldToPixelPosition(box.MinLongitude, box.MinLatitude, ZoomLevel);
                Vector2 pixelPositionMax = WorldToPixelPosition(box.MaxLongitude, box.MaxLatitude, ZoomLevel);
                Vector2 pixelSize = pixelPositionMax - pixelPositionMin;
                Rect rect = new Rect(pixelPositionMin.x - pixelOrigin.x - 1, pixelPositionMin.y - pixelOrigin.y + 1, pixelSize.x - 2, pixelSize.y + 2);
                GUI.DrawTexture(rect, CustomGUIUtils.GetSimpleColorTexture(redAlpha), ScaleMode.StretchToFill, true);
            }
        }
    }
    #endregion // Selection using TileManager-Atttributes

    #region Location Search-Bar
    private void LocationQuery_QueryDone(object sender, EventArgs e)
    {
        Debug.Log("OSMMapRect: Found Location");
    }

    private void DrawSearchResults()
    {
        Rect resultLine = new Rect(0, 0, 300, 40);
        if (locationQuery != null)
        {
            if (locationQuery.searchResults.Count > 0)
            {
                if (lightGreen == null)
                {
                    lightGreen = CustomGUIUtils.GetColorBackgroundStyle(XKCDColors.LightGreen);
                }
                for (int i = 0; i < locationQuery.searchResults.Count; i++)
                {
                    Location location = locationQuery.searchResults[i];
                    resultLine.y = (resultLine.height * (i + 1));
                    GUILayout.BeginArea(resultLine);
                    GUILayout.Label(location.ToString(), lightGreen);
                    GUILayout.EndArea();
                    if (resultLine.Contains(Event.current.mousePosition))
                    {
                        Debug.Log("mouseOver " + location);
                        if (Event.current.type == EventType.MouseUp)
                        {
                            TileManager.OriginLatitude = location.lat;
                            TileManager.OriginLongitude = location.lon;
                            Debug.Log("Set YouAreHere to " + location.ToString());
                            SetDirty();
                        }
                    }
                }
            }
        }
    }

    private void DrawSearchBar()
    {
        searchString = GUI.TextField(new Rect(0, 0, 300, 40), searchString);
        if (GUI.Button(new Rect(305, 0, 100, 40), "Search"))
        {
            locationQuery = new LocationQuery();
            locationQuery.QueryDone += LocationQuery_QueryDone;
            locationQuery.SearchLocation(searchString);
            SetDirty();
        }
    }
    #endregion // Location Search-Bar

    #region Debug-Draw-Methods
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
    #endregion // Debug-Draw-Methods

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

    private Rect CartesianGrid(Rect mapRect)
    {
        // Grid on offsetCenter with tileSize
        for (int x = (int)-pixelOrigin.x % TileSizePixels; x < mapRect.xMax; x += TileSizePixels)
        {
            DrawVerticalLine(mapRect, x, Color.grey);
        }

        for (int y = (int)-pixelOrigin.y % TileSizePixels; y < mapRect.yMax; y += TileSizePixels)
        {
            DrawHorizontalLine(mapRect, y, Color.grey);
        }
        return mapRect;
    }
    #endregion // Tile-, Coords- Converstion-Methods

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
        CustomGUIUtils.DrawBox(new Rect(rect.xMin, y, rect.width, 1), color);
    }

    private static void DrawVerticalLine(Rect rect, float x, Color color)
    {
        CustomGUIUtils.DrawBox(new Rect(x, rect.yMin, 1, rect.height), color);
    }
    #endregion // Drawing Utils
}
