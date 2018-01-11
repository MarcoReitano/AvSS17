using System;

using System.IO;
using System.Net;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class OSMTileMap
{

    public OSMTileMap()
    {

    }

    public double mapWidth = 0f;
    public double mapHeight = 0f;

    public double xOffset = 0f;
    public double yOffset = 0f;

    public Vector2 centerTileNumbers;
    public Vector2 centerTileCoordsOnWindow;

    public double centerLongitude = 0f;
    public double centerLatitude = 0;
    public int tileSize = 256;
    public int zoomLevel = 13;
    public Texture2D[,] textureArray;
    public Texture2D mouseTexture;

    public float degreesPerPixel;

    public bool changed = true;




    public void DisplayTileMap()
    {
        // Load new Tiles if something changed since last time
        if (changed)
            LoadTileTextures();

        // Display the Map
#if UNITY_EDITOR
        DisplayTileTextures();
#endif

    }

    public void SetCenterTile(int x, int y)
    {
        this.centerTileNumbers = new Vector2(x, y);
        Vector2 newCenterCoords = GeographicCoordinates.TileToWorldPos(x, y, this.zoomLevel);
        this.centerLongitude = newCenterCoords.x;
        this.centerLatitude = newCenterCoords.y;

    }

    /// <summary>
    /// 
    /// </summary>
    public void CaculateValues(Rect windowRect, float xOffset, float yOffset)
    {

        this.xOffset = xOffset;
        this.yOffset = yOffset;
        this.mapWidth = windowRect.width - this.xOffset;
        this.mapHeight = windowRect.height - this.yOffset;
        this.tileSize = (int)(this.mapWidth / 5f);

        this.centerTileNumbers = GeographicCoordinates.WorldToTilePos(this.centerLongitude, this.centerLatitude, this.zoomLevel);

        this.centerTileCoordsOnWindow = new Vector2((float)(this.mapWidth / 2f + this.xOffset), (float)(this.mapHeight / 2f + this.yOffset));

        CalculateDegreesPerPixel();

        // Methode --> pixelKoordinaten --> LonLat


        // Methode --> LonLat --> Pixelkoordinaten 

    }

    private void CalculateDegreesPerPixel()
    {
        // TileToWorldPosition vom CenterTile
        Vector2 tileToWorldCenterTile = GeographicCoordinates.TileToWorldPos(Mathf.RoundToInt(centerTileNumbers.x), Mathf.RoundToInt(centerTileNumbers.y), this.zoomLevel);

        // TileToWorldPosition vom ein Tile weiter
        Vector2 tileToWorldCenterNextTile = GeographicCoordinates.TileToWorldPos(Mathf.RoundToInt(centerTileNumbers.x + 1), Mathf.RoundToInt(centerTileNumbers.y), this.zoomLevel);

        // Distanz zwischen Punkten geteilt durch TileSize in Pixeln = degreesPerPixel
        this.degreesPerPixel = (tileToWorldCenterNextTile.x - tileToWorldCenterTile.x) / this.tileSize;
    }


    public Vector2 PixelXYToLonLat(int x, int y)
    {
        Vector2 result = new Vector2();


        result.x = (float)this.centerLongitude - ((this.centerTileCoordsOnWindow.x - (float)x) * this.degreesPerPixel);
        result.y = (float)this.centerLatitude + ((this.centerTileCoordsOnWindow.y - (float)y) * this.degreesPerPixel);

        return result;
    }


    public Vector2 LonLatToPixelXY(double lon, double lat)
    {
        Vector2 result = new Vector2();

        int resultLon = Mathf.RoundToInt((float)(this.centerLongitude - lon) / degreesPerPixel);
        int resultLat = Mathf.RoundToInt((float)(this.centerLatitude - lat) / degreesPerPixel);

        result.x = this.centerTileCoordsOnWindow.x - resultLon;
        result.y = this.centerTileCoordsOnWindow.y + resultLat;

        return result;
    }



    /// <summary>
    /// 
    /// </summary>
    /// <param name="color"></param>
    public void PrepareMouseTileTexture(Color color)
    {
        if (mouseTexture == null)
        {
            mouseTexture = new Texture2D(tileSize, tileSize, TextureFormat.ARGB32, true);

            int y = 0;
            while (y < mouseTexture.height)
            {
                int x = 0;
                while (x < mouseTexture.width)
                {
                    mouseTexture.SetPixel(x, y, color);
                    ++x;
                }
                ++y;
            }
            mouseTexture.Apply();
        }
    }
#if UNITY_EDITOR
    /// <summary>
    /// 
    /// </summary>
    public void DisplayTileTextures()
    {
        int xIndex = -2;
        int yIndex = -2;

        for (int x = 0; x < 5; x++)
        {
            yIndex = -2;
            for (int y = 0; y < 5; y++)
            {
#if UNTIY_EDITOR
                EditorGUI.DrawPreviewTexture(
                    new Rect(
                        centerTileCoordsOnWindow.x + xIndex * tileSize - tileSize / 2,
                        centerTileCoordsOnWindow.y + yIndex * tileSize - tileSize / 2,
                        tileSize, tileSize),
                        textureArray[y, x]);
#endif
                yIndex++;
            }
            xIndex++;
        }
    }
#endif
    /// <summary>
    /// 
    /// </summary>
    public void LoadTileTextures()
    {
        this.textureArray = new Texture2D[5, 5];
        int xIndex = (int)centerTileNumbers.x - 2;
        int yIndex = (int)centerTileNumbers.y - 2;

        for (int x = 0; x < 5; x++)
            for (int y = 0; y < 5; y++)
                this.textureArray[y, x] = GetTileTexture(xIndex + x, yIndex + y, this.zoomLevel);

        changed = false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="xpos"></param>
    /// <param name="ypos"></param>
    /// <param name="zoom"></param>
    /// <returns></returns>
    public Texture2D GetTileTexture(int xpos, int ypos, int zoom)
    {
        Texture2D texture = new Texture2D(2, 2, TextureFormat.RGB24, false);

        if (zoom >= 0 && zoom <= 18 &&
            xpos >= 0 && xpos < Mathf.Pow(4, 18) &&
            ypos >= 0 && ypos < Mathf.Pow(4, 18))
        {

            string tilePath = "/" + zoom + "/" + xpos + "/" + ypos + ".png";

            string url = "http://tile.openstreetmap.org" + tilePath;


            string tmpFolder = string.Empty;
            string tmpFile = string.Empty;

//#if UNTIY_EDITOR
//            tmpFolder = EditorApplication.applicationContentsPath + @"/OSM_TILE_TMP";
//            tmpFile = tmpFolder + tilePath;
//#elif STANDALONE
            tmpFolder = UnityEngine.Application.dataPath + @"/OSM_TILE_TMP";
            tmpFile = tmpFolder + tilePath;
//#endif


            if (!Directory.Exists(tmpFolder + @"/" + zoom + "/" + xpos))
            {
                string newPath = tmpFolder + "/" + zoom + "/" + xpos;
                Directory.CreateDirectory(@newPath);
            }

            if (!File.Exists(tmpFile))
                DownloadFile(url, tmpFile);

            if (File.Exists(tmpFile))
            {
                byte[] file = File.ReadAllBytes(tmpFile);
                texture.LoadImage(file);
            }
        }
        else
        {
            texture = GetTileTexture(0, 0, 0);

        }

        return texture;
    }

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


    public Vector2 MouseToTilePositionRelativeToCenter(int x, int y)//, Vector2 centerScreenCoords, Vector2 centerTilePosition, int tileSize)
    {
        Vector2 p = new Vector2();

        p.x = Mathf.Round(-(centerTileCoordsOnWindow.x - x) / tileSize);
        p.y = Mathf.Round(-(centerTileCoordsOnWindow.y - y) / tileSize);

        return p;
    }


    public Vector2 MouseToTilePosition(int x, int y)//, Vector2 centerScreenCoords, Vector2 centerTilePosition, int tileSize)
    {
        Vector2 p = new Vector2();

        Vector2 mouseRelative = MouseToTilePositionRelativeToCenter(x, y);

        p.x = centerTileNumbers.x + mouseRelative.x;
        p.y = centerTileNumbers.y + mouseRelative.y;

        return p;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="centerScreenCoords"></param>
    /// <param name="centerTilePosition"></param>
    /// <param name="tileSize"></param>
    /// <returns></returns>
    public Vector2 MouseToWorldPosition(int x, int y, Vector2 centerScreenCoords, Vector2 centerTilePosition, int tileSize)
    {
        Vector2 p = new Vector2();

        int xDistance = x - (int)centerScreenCoords.x;
        int yDistance = y - (int)centerScreenCoords.y;

        p.x = (int)(x / tileSize);
        p.y = (int)(y / tileSize);

        return p;
    }





    public bool isOnMap(Vector2 point)
    {
        if (point.x > xOffset && point.x < xOffset + mapWidth
            && point.y > yOffset && point.y < yOffset + mapHeight)
        {
            return true;
        }
        return false;
    }



    // http://wiki.openstreetmap.org/wiki/Slippy_map_tilenames
    /// <summary>
    /// 
    /// </summary>
    /// <param name="lon"></param>
    /// <param name="lat"></param>
    /// <param name="zoom"></param>
    /// <returns></returns>
    public static Vector2 RealWorldToTilePos(double lon, double lat, int zoom)
    {
        Vector2 p = new Vector2();
        p.x = (float)((lon + 180.0) / 360.0 * (1 << zoom));
        p.y = (float)((1.0 - Math.Log(Math.Tan(lat * Math.PI / 180.0) +
            1.0 / Math.Cos(lat * Math.PI / 180.0)) / Math.PI) / 2.0 * (1 << zoom));

        return p;
    }



    /// <summary>
    /// 
    /// </summary>
    /// <param name="tile_x"></param>
    /// <param name="tile_y"></param>
    /// <param name="zoom"></param>
    /// <returns> A Vector with longitude and Latitude of the lower</returns>
    public static Vector2 TileToRealWorldPos(int tile_x, int tile_y, int zoom)
    {
        Vector2 p = new Vector2();
        double n = Math.PI - ((2.0 * Math.PI * tile_y) / Math.Pow(2.0, zoom));

        p.x = (float)((tile_x / Math.Pow(2.0, zoom) * 360.0) - 180.0);
        p.y = (float)(180.0 / Math.PI * Math.Atan(Math.Sinh(n)));

        return p;
    }

    /*
     * Subtiles
     * If you're looking at tile x,y and want to zoom in, the subtiles are (in the next zoom-level's coordinate system):
     *   
     *   -----------------------------------
     *   |  2x, 2y 	    |  2x + 1, 2y      |
     *   -----------------------------------
     *   |  2x, 2y + 1 	|  2x + 1, 2y + 1  |
     *   -----------------------------------
     *   
     * Similarly, zoom out by halving x and y (in the previous zoom level) 
     * */

    public static MapBounds Tile2MapBounds(int x, int y, int zoom)
    {
        MapBounds bb = new MapBounds();
        bb.North = Tile2Latitude(y, zoom);
        bb.South = Tile2Latitude(y + 1, zoom);
        bb.West = Tile2Longitude(x, zoom);
        bb.East = Tile2Longitude(x + 1, zoom);
        return bb;
    }

    public static double Tile2Longitude(int x, int zoom)
    {
        return x / Math.Pow(2.0, zoom) * 360.0 - 180;
    }

    static double Tile2Latitude(int y, int zoom)
    {
        double n = Math.PI - (2.0 * Math.PI * y) / Math.Pow(2.0, zoom);
        return (Math.Atan(Math.Sinh(n))) * Mathf.Rad2Deg;
    }


}
