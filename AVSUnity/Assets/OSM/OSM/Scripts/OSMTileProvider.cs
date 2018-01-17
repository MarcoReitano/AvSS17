using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;




public static class OSMTileProvider
{
    [SerializeField]
    public static Dictionary<int, Dictionary<Vector2, OSMTile>> tiles = new Dictionary<int, Dictionary<Vector2, OSMTile>>();
    [SerializeField]
    public static Dictionary<int, Dictionary<Vector2, OSMTileBehaviour>> tileBehaviours = new Dictionary<int, Dictionary<Vector2, OSMTileBehaviour>>();

    [SerializeField]
    public static Dictionary<int, Dictionary<Vector2, Texture2D>> tileTextures = new Dictionary<int, Dictionary<Vector2, Texture2D>>();

    [SerializeField]
    private static double[] degreesPerPixel = new double[30];
    [SerializeField]
    private static double[] metersPerPixel = new double[30];

    public static readonly int TileSizeInPixels = 256;  // Für mobile auch 64 möglich

    // Static constructor for the initialisation of the degreesPerPixel Array
    static OSMTileProvider()
    {
        double degrees = 360;
        double meters = 156412;
        for (int i = 0; i < 30; i++)
        {
            degreesPerPixel[i] = degrees;
            metersPerPixel[i] = meters;
            degrees /= 2;
            meters /= 2;
        }
    }

    public static int TileCountForZoomLevel(int zoomLevel)
    {
        return (int)Mathf.Pow(2f, zoomLevel);
    }

    public static int ClampToZoomLevel(int index, int zoomLevel)
    {
        return (int)Mathf.Clamp(index, 0f, TileCountForZoomLevel(zoomLevel));
    }

    public static GameObject GetOSMTileGameObject(int xIndex, int yIndex, int zoomLevel)
    {
        return GetOSMTileGameObject(OSMTile.GetOSMTile(xIndex, yIndex, zoomLevel));
    }

    public static GameObject GetOSMTileGameObject(OSMTile tile)
    {
        GameObject tilesParent = GameObject.Find("OSMTiles");
        if (tilesParent == null)
            tilesParent = new GameObject("OSMTiles");

        Vector2 coords = new Vector2(tile.X, tile.Y);

        if (!OSMTileProvider.tileBehaviours.ContainsKey(tile.ZoomLevel))
            OSMTileProvider.tileBehaviours.Add(tile.ZoomLevel, new Dictionary<Vector2, OSMTileBehaviour>());

        GameObject zoomLevelParent;
        if (!tilesParent.HasChildNamed(tile.ZoomLevel.ToString(), out zoomLevelParent))
            zoomLevelParent = tilesParent.CreateChild(tile.ZoomLevel.ToString());


        if (!OSMTileProvider.tileBehaviours[tile.ZoomLevel].ContainsKey(coords))
        {
            GameObject tileGO = zoomLevelParent.CreateRenderableChild("osmTile_" + tile.ZoomLevel + "_" + tile.X + "_" + tile.Y);
            OSMTileBehaviour osmTileBehaviour = tileGO.AddComponent<OSMTileBehaviour>();
            osmTileBehaviour.tile = GetOSMTile(tile.X, tile.Y, tile.ZoomLevel);

            OSMTileProvider.tileBehaviours[tile.ZoomLevel].Add(coords, osmTileBehaviour);
            osmTileBehaviour.Initialize();
        }

        return OSMTileProvider.tileBehaviours[tile.ZoomLevel][coords].gameObject;
    }

    public static List<GameObject> GetOSMTileGameObjects(List<OSMTile> tiles)
    {
        List<GameObject> tileGOs = new List<GameObject>();
        foreach (OSMTile tile in tiles)
            tileGOs.Add(GetOSMTileGameObject(tile));

        return tileGOs;
    }

    public static void SetZoomLevelVisible(int zoomLevel, bool visible)
    {
        if (tileBehaviours.ContainsKey(zoomLevel))
        {
            foreach (OSMTileBehaviour tileBehaviour in tileBehaviours[zoomLevel].Values)
            {
                tileBehaviour.gameObject.SetVisible(visible);
            }
        }
    }

    public static double DegreesPerPixel(int zoomLevel)
    {
        return degreesPerPixel[zoomLevel.Clamp(0, 30)];
    }

    public static double MetersPerPixel(int zoomLevel)
    {
        return metersPerPixel[zoomLevel.Clamp(0, 30)];
    }


    public static OSMTile GetOSMTile(int xIndex, int yIndex, int zoomLevel)
    {
        return OSMTile.GetOSMTile(xIndex, yIndex, zoomLevel);
    }

    public static OSMTile GetOSMTile(double longitude, double latitude, int zoomLevel)
    {
        Vector2 tilePosition = LonLat2TileIndex(longitude, latitude, zoomLevel);
        return GetOSMTile((int)tilePosition.x, (int)tilePosition.y, zoomLevel);
    }

    public static List<OSMTile> GetOSMTilesInBoundingBox(MapBounds bounds, int zoomLevel)
    {
        Vector2 indexNW = LonLat2TileIndex(bounds.West, bounds.North, zoomLevel);
        Vector2 indexNE = LonLat2TileIndex(bounds.East, bounds.North, zoomLevel);
        Vector2 indexSW = LonLat2TileIndex(bounds.West, bounds.South, zoomLevel);

        List<OSMTile> tilesInBounds = new List<OSMTile>();
        Debug.Log(bounds);
        Debug.Log("NW: " + indexNW);
        Debug.Log("NE: " + indexNE);
        Debug.Log("SW: " + indexSW);

        for (int x = (int)indexNW.x; x <= (int)indexNE.x; x++)
        {
            for (int y = (int)indexNW.y; y <= (int)indexSW.y; y++)
            {
                tilesInBounds.Add(GetOSMTile(x, y, zoomLevel));
                //Debug.Log(x + " / " + y + " --> " + zoomLevel);
            }
        }

        return tilesInBounds;
    }

    public static List<GameObject> GetOSMTileGameObjectsInBoundingBoxCutting(MapBounds bounds, int zoomLevel)
    {
        List<GameObject> tileGOs = new List<GameObject>();
        List<OSMTile> tiles = GetOSMTilesInBoundingBox(bounds, zoomLevel);

        foreach (OSMTile tile in tiles)
        {
            GameObject tileGO = GetOSMTileGameObject(tile);
            OSMTileBehaviour tileBehaviour = tileGO.GetComponent<OSMTileBehaviour>();
            tileBehaviour.InitializeCuttingBounds(bounds);
            tileGOs.Add(tileGO);
        }

        return tileGOs;
    }


    public static List<GameObject> GetOSMTileGameObjectsInBoundingBox(MapBounds bounds, int zoomLevel)
    {
        List<GameObject> tileGOs = new List<GameObject>();
        List<OSMTile> tiles = GetOSMTilesInBoundingBox(bounds, zoomLevel);

        foreach (OSMTile tile in tiles)
            tileGOs.Add(GetOSMTileGameObject(tile));

        return tileGOs;
    }




    public static void PrepareZoomGameObjects(int zoomLevel)
    {
        if (tileBehaviours.ContainsKey(zoomLevel))
        {
            foreach (OSMTileBehaviour tileBehaviour in tileBehaviours[zoomLevel].Values)
            {
                tileBehaviour.tile.PrepareZoomGameObjects();
            }
        }
    }

    public static void PrepareZoom(int zoomLevel)
    {
        if (tiles.ContainsKey(zoomLevel))
        {
            foreach (OSMTile tile in tiles[zoomLevel].Values)
            {
                tile.PrepareZoom();
            }
        }
    }




    // http://wiki.openstreetmap.org/wiki/Slippy_map_tilenames
    /// <summary>
    /// 
    /// </summary>
    /// <param name="longitude"></param>
    /// <param name="latitude"></param>
    /// <param name="zoomLevel"></param>
    /// <returns></returns>
    public static Vector2 LonLat2TileIndex(double longitude, double latitude, int zoomLevel)
    {
        Vector2 p = new Vector2();
        p.x = (float)((longitude + 180.0) / 360.0 * (1 << zoomLevel));
        p.y = (float)((1.0 - Math.Log(Math.Tan(latitude * Math.PI / 180.0) +
            1.0 / Math.Cos(latitude * Math.PI / 180.0)) / Math.PI) / 2.0 * (1 << zoomLevel));

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
*   ------------------------------
*   |     0/0 	    |    1/0      |
*   ------------------------------
*   |     0/1      |    1/1      |
*   ------------------------------  
* 
* Similarly, zoom out by halving x and y (in the previous zoom level) 
* */
    public static Vector2[,] GetSubtileIndices(int xIndex, int yIndex, int zoomLevel)
    {
        Vector2[,] subTileIndices = new Vector2[2, 2];
        subTileIndices[0, 0] = new Vector2(2 * xIndex, 2 * yIndex);
        subTileIndices[0, 1] = new Vector2(2 * xIndex, 2 * yIndex + 1);
        subTileIndices[1, 0] = new Vector2(2 * xIndex + 1, 2 * yIndex);
        subTileIndices[1, 1] = new Vector2(2 * xIndex + 1, 2 * yIndex + 1);
        return subTileIndices;
    }

    public static Vector2[,] GetSubtileIndices(OSMTile superTile)
    {
        return GetSubtileIndices(superTile.X, superTile.Y, superTile.ZoomLevel);
    }

    public static OSMTile[,] GetSubTiles(int xIndex, int yIndex, int zoomLevel)
    {
        OSMTile[,] subTiles = new OSMTile[2, 2];
        Vector2[,] subTileIndices = GetSubtileIndices(xIndex, yIndex, zoomLevel);

        subTiles[0, 0] = GetOSMTile((int)subTileIndices[0, 0].x, (int)subTileIndices[0, 0].y, zoomLevel + 1);
        subTiles[0, 1] = GetOSMTile((int)subTileIndices[0, 1].x, (int)subTileIndices[0, 1].y, zoomLevel + 1);
        subTiles[1, 0] = GetOSMTile((int)subTileIndices[1, 0].x, (int)subTileIndices[1, 0].y, zoomLevel + 1);
        subTiles[1, 1] = GetOSMTile((int)subTileIndices[1, 1].x, (int)subTileIndices[1, 1].y, zoomLevel + 1);

        return subTiles;
    }

    public static List<OSMTile> GetSubTilesAsList(int xIndex, int yIndex, int zoomLevel)
    {
        List<OSMTile> subTiles = new List<OSMTile>();
        Vector2[,] subTileIndices = GetSubtileIndices(xIndex, yIndex, zoomLevel);

        subTiles.Add(GetOSMTile((int)subTileIndices[0, 0].x, (int)subTileIndices[0, 0].y, zoomLevel + 1));
        subTiles.Add(GetOSMTile((int)subTileIndices[0, 1].x, (int)subTileIndices[0, 1].y, zoomLevel + 1));
        subTiles.Add(GetOSMTile((int)subTileIndices[1, 0].x, (int)subTileIndices[1, 0].y, zoomLevel + 1));
        subTiles.Add(GetOSMTile((int)subTileIndices[1, 1].x, (int)subTileIndices[1, 1].y, zoomLevel + 1));

        return subTiles;
    }

    public static OSMTile[,] GetSubTiles(OSMTile superTile)
    {
        return GetSubTiles(superTile.X, superTile.Y, superTile.ZoomLevel);
    }

    public static List<OSMTile> GetSubTilesAsList(OSMTile superTile)
    {
        return GetSubTilesAsList(superTile.X, superTile.Y, superTile.ZoomLevel);
    }


    public static Vector3 GetSupertileIndex(int xIndex, int yIndex, int zoomLevel)
    {
        return new Vector3(Mathf.FloorToInt(xIndex / 2f), Mathf.FloorToInt(yIndex / 2f), zoomLevel - 1);
    }

    public static Vector3 GetSupertileIndex(OSMTile subTile)
    {
        return new Vector3(Mathf.FloorToInt(subTile.X / 2f), Mathf.FloorToInt(subTile.Y / 2f), subTile.ZoomLevel - 1);
    }

    public static OSMTile GetSupertile(int xIndex, int yIndex, int zoomLevel)
    {
        Vector3 superTile = GetSupertileIndex(xIndex, yIndex, zoomLevel);
        return GetOSMTile((int)superTile.x, (int)superTile.y, (int)superTile.z);
    }

    public static OSMTile GetSupertile(OSMTile subTile)
    {
        Vector3 superTile = GetSupertileIndex(subTile);
        return GetOSMTile((int)superTile.x, (int)superTile.y, (int)superTile.z);
    }


    public static Texture2D DownloadTileTexture(double longitude, double latitude, int zoomLevel)
    {
        Vector2 tileIndices = LonLat2TileIndex(longitude, latitude, zoomLevel);
        return DownloadTileTexture(tileIndices.x, tileIndices.y, zoomLevel);
    }


    public static Texture2D DownloadTileTexture(int xIndex, int yIndex, int zoomLevel)
    {
        if (zoomLevel >= 0 && zoomLevel <= 18 &&
            xIndex >= 0 && xIndex < Mathf.Pow(4, 18) &&
            yIndex >= 0 && yIndex < Mathf.Pow(4, 18))
        {

            string tilePath = "/" + zoomLevel + "/" + xIndex + "/" + yIndex + ".png";
            string url = "http://tile.openstreetmap.org" + tilePath;
            //string url = "http://tile.stamen.com/toner" + tilePath;
            //string url = "http://tile.stamen.com/toner" + tilePath;
            //string url = "http://tile.stamen.com/watercolor" + tilePath;

            //<<<<<<< HEAD
            string tmpFolder;
#if UNITY_EDITOR
            tmpFolder = EditorPrefs.GetString("OSMTileCachePath");
#else
            tmpFolder = Application.dataPath + @"/OSM_TILE_TMP";
#endif
            //=======
            //            string tmpFolder = string.Empty;
            //#if UNITY_EDITOR
            //            tmpFolder = EditorPrefs.GetString("OSMTileCachePath");
            //#elif STANDALONE
            //            //tmpFolder = EditorPrefs.GetString("OSMTileCachePath");
            //#endif
            //            if (tmpFolder == null || tmpFolder == "") {
            //                tmpFolder = Application.dataPath + @"/OSM_TILE_TMP";
            //            }
            //>>>>>>> scenemanager
            //string tmpFolder = EditorApplication.applicationContentsPath + @"/OSM_TILE_TMP";
            string tmpFile = tmpFolder + tilePath;

            if (!Directory.Exists(tmpFolder + @"/" + zoomLevel + "/" + xIndex))
            {
                string newPath = tmpFolder + "/" + zoomLevel + "/" + xIndex;
                Directory.CreateDirectory(@newPath);
            }

            if (!tileTextures.ContainsKey(zoomLevel))
            {
                tileTextures[zoomLevel] = new Dictionary<Vector2, Texture2D>();
            }


            Vector2 coords = new Vector2(xIndex, yIndex);
            if (!tileTextures[zoomLevel].ContainsKey(coords))
            {
                if (!File.Exists(tmpFile))
                    DownloadFile(url, tmpFile);

                if (File.Exists(tmpFile))
                {
                    byte[] file = File.ReadAllBytes(tmpFile);
                    Texture2D texture = new Texture2D(2, 2, TextureFormat.RGB24, false);
                    texture.LoadImage(file);
                    tileTextures[zoomLevel][coords] = texture;
                }
            }
            else
            {
                return tileTextures[zoomLevel][coords];
            }
        }
        else
        {
            Texture2D texture = new Texture2D(2, 2, TextureFormat.RGB24, false);
            texture = DownloadTileTexture(0, 0, 0);
            return texture;
        }

        return null;
    }

    private static FileStream fileStream;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="url"></param>
    /// <param name="tmpFile"></param>
    private static void DownloadFile(string url, string tmpFile)
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
        stream.Close();
    }

    //private static void DownloadUsingWWW(string url, string tmpfile)
    //{
    //    // Start a download of the given URL
    //    WWW www = new WWW(url);

    //    WWW.LoadFromCacheOrDownload(url, Hash128.Parse(url));
    //    while (!www.isDone)
    //    {
    //        if (www.error != null)
    //        {
    //            //Debug.LogError(www.error);
    //            return false;
    //        }
    //    }

    //    return true;
    //}


    public static OSMBoundingBox Tile2OSMBoundingBox(int xIndex, int yIndex, int zoomLevel)
    {
        OSMBoundingBox bb = new OSMBoundingBox();
        bb.MinLatitude = Tile2Latitude(yIndex, zoomLevel);
        bb.MaxLatitude = Tile2Latitude(yIndex + 1, zoomLevel);
        bb.MinLongitude = Tile2Longitude(xIndex, zoomLevel);
        bb.MaxLongitude = Tile2Longitude(xIndex + 1, zoomLevel);
        return bb;
    }

    /// <summary>
    ///  Nach: http://code.google.com/p/osmdroid/source/browse/trunk/osmdroid-android/src/microsoft/mappoint/TileSystem.java?r=854
    /// </summary>
    /// <param name="xIndex"></param>
    /// <param name="yIndex"></param>
    /// <param name="zoomLevel"></param>
    /// <returns></returns>
    public static MapBounds Tile2MapBounds(int xIndex, int yIndex, int zoomLevel)
    {
        MapBounds bb = new MapBounds();
        bb.North = Tile2Latitude(yIndex, zoomLevel);
        bb.South = Tile2Latitude(yIndex + 1, zoomLevel);
        bb.West = Tile2Longitude(xIndex, zoomLevel);
        bb.East = Tile2Longitude(xIndex + 1, zoomLevel);

        return bb;
    }

    /// <summary>
    ///  Nach: http://code.google.com/p/osmdroid/source/browse/trunk/osmdroid-android/src/microsoft/mappoint/TileSystem.java?r=854
    /// </summary>
    /// <param name="xIndex"></param>
    /// <param name="zoomLevel"></param>
    /// <returns></returns>
    public static double Tile2Longitude(int xIndex, int zoomLevel)
    {
        return xIndex / Math.Pow(2.0, zoomLevel) * 360.0 - 180;
    }

    /// <summary>
    ///  Nach: http://code.google.com/p/osmdroid/source/browse/trunk/osmdroid-android/src/microsoft/mappoint/TileSystem.java?r=854
    /// </summary>
    /// <param name="yIndex"></param>
    /// <param name="zoomLevel"></param>
    /// <returns></returns>
    public static double Tile2Latitude(int yIndex, int zoomLevel)
    {
        double n = Math.PI - (2.0 * Math.PI * yIndex) / Math.Pow(2.0, zoomLevel);
        return (Math.Atan(Math.Sinh(n))) * Mathf.Rad2Deg;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="zoomLevel"></param>
    /// <returns> A Vector with longitude and Latitude of the lower-left corner</returns>
    public static Vector2 Tile2GeoCoordinates(int x, int y, int zoomLevel)
    {
        Vector2 p = new Vector2();

        double maxTiles = Math.Pow(2.0f, zoomLevel);
        double n = Math.PI - ((2.0f * Math.PI * y) / maxTiles);

        p.x = (float)((x / maxTiles * 360.0f) - 180.0f);
        p.y = (float)(180.0f / Math.PI * Math.Atan(Math.Sinh(n)));

        return p;
    }


    public static void Clear()
    {
        tiles.Clear();

        tileBehaviours.Clear();
    }
}

