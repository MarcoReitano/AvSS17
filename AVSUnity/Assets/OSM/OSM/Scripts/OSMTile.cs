using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[Serializable]
public class OSMTile
{
    [SerializeField]
    private int x;
    [SerializeField]
    private int y;
    [SerializeField]
    private int zoomLevel;
    [SerializeField]
    private Texture2D image;

    [SerializeField]
    private double longitude;
    [SerializeField]
    private double latitude;
    [SerializeField]
    private MapBounds mapBounds;
    [SerializeField]
    public OverpassQuery query;


    public int X
    {
        get { return x; }
    }

    public int Y
    {
        get { return y; }
    }

    public double Longitude
    {
        get { return longitude; }
        set { longitude = value; }
    }

    public double Latitude
    {
        get { return latitude; }
        set { latitude = value; }
    }

    public int ZoomLevel
    {
        get { return zoomLevel; }
    }

    public Texture2D Image
    {
        get { return image; }
    }

    public MapBounds MapBounds
    {
        get { return mapBounds; }
    }


    public OSMTile North
    {
        get { return GetOSMTile(x, y - 1, zoomLevel); }
    }
    public OSMTile East
    {
        get { return GetOSMTile(x + 1, y, zoomLevel); }
    }
    public OSMTile South
    {
        get { return GetOSMTile(x, y + 1, zoomLevel); }
    }
    public OSMTile West
    {
        get { return GetOSMTile(x - 1, y, zoomLevel); }
    }

    public OSMTile NorthEast
    {
        get { return GetOSMTile(x + 1, y - 1, zoomLevel); }
    }
    public OSMTile NorthWest
    {
        get { return GetOSMTile(x - 1, y - 1, zoomLevel); }
    }
    public OSMTile SouthEast
    {
        get { return GetOSMTile(x + 1, y + 1, zoomLevel); }
    }
    public OSMTile SouthWest
    {
        get { return GetOSMTile(x - 1, y + 1, zoomLevel); }
    }

    /// <summary>
    /// Get the Neighbours as List (clockwise: N,NE,E,SE,S,SW,W,NW)
    /// If not allready done the Tile will be downloaded.
    /// </summary>
    /// <returns>List of neighbouring OSM Tiles</returns>
    public List<OSMTile> GetNeightbours()
    {
        List<OSMTile> neighbours = new List<OSMTile>();
        
        neighbours.Add(North);
        neighbours.Add(NorthEast);
        neighbours.Add(East);
        neighbours.Add(SouthEast);
        neighbours.Add(South);
        neighbours.Add(SouthWest);
        neighbours.Add(West);
        neighbours.Add(NorthWest);

        return neighbours;
    }



    public static OSMTile GetOSMTile(int xIndex, int yIndex, int zoomLevel)
    {
        Vector2 coords = new Vector2(xIndex, yIndex);

        if (!OSMTileProvider.tiles.ContainsKey(zoomLevel))
            OSMTileProvider.tiles.Add(zoomLevel, new Dictionary<Vector2, OSMTile>());

        if (!OSMTileProvider.tiles[zoomLevel].ContainsKey(coords))
            OSMTileProvider.tiles[zoomLevel].Add(coords, new OSMTile(xIndex, yIndex, zoomLevel));

        return OSMTileProvider.tiles[zoomLevel][coords];
    }
      

    public static OSMTile GetOSMTile(double longitude, double latitude, int zoomLevel)
    {
        Vector2 tilePosition = OSMTileProvider.LonLat2TileIndex(longitude, latitude, zoomLevel);
        return GetOSMTile((int)tilePosition.x, (int)tilePosition.y, zoomLevel);
    }

    private OSMTile(int x, int y, int zoomLevel)
    {
        this.zoomLevel = zoomLevel;

        this.x = x;
        this.y = y;

        this.longitude = OSMTileProvider.Tile2Longitude(x, zoomLevel);
        this.latitude = OSMTileProvider.Tile2Latitude(y, zoomLevel);

        this.image = OSMTileProvider.DownloadTileTexture(x, y, zoomLevel);
        this.mapBounds = OSMTileProvider.Tile2MapBounds(x, y, zoomLevel);

        query = new OverpassQuery();
        query.BoundingBox = OSMTileProvider.Tile2OSMBoundingBox(x, y, zoomLevel);
    }

    public void StartQuery()
    {
        this.query.QueryDone += QueryDone;
        this.query.DownloadOSMString();
    }

    public void QueryDone(object sender, System.EventArgs e)
    {
        this.query.QueryDone -= QueryDone;
        Debug.Log("Loaded Tile Data");
    }


    public List<OSMTile> GetSubTiles()
    {
        return OSMTileProvider.GetSubTilesAsList(this.x, this.y, this.zoomLevel);
    }

    public OSMTile GetSuperTile()
    {
        return OSMTileProvider.GetSupertile(this);
    }


    public List<GameObject> GetSubTilesGameObjects()
    {
		 return OSMTileProvider.GetOSMTileGameObjects(GetSubTiles());
    }

    public GameObject GetSuperTileGameObject()
    {
        return OSMTileProvider.GetOSMTileGameObject(GetSuperTile());
    }


    public Vector2 TileToRealWorldPosition
    {
        get { return OSMTileProvider.Tile2GeoCoordinates(x, y, zoomLevel); }
    }

    public Vector2 PixelToParameter(int x, int y)
    {
        Vector2 result = new Vector2();

        result.x = Mathf.Clamp01(x / 256f);
        result.y = Mathf.Clamp01(y / 256f);
        return result;
    }


    public Vector2 PixelToParameter(Vector2 pixelCoords)
    {
        return PixelToParameter((int)pixelCoords.x, (int)pixelCoords.y);
    }


    public Vector2 ParameterToPixel(float x, float y)
    {
        Vector2 result = new Vector2();

        result.x = Mathf.RoundToInt(256f*x);
        result.y = Mathf.RoundToInt(256f*y);
        return result;
    }


    //public Vector2 PixelXYToLonLat(int x, int y)
    //{
    //    Vector2 result = new Vector2();


    //    result.x = (float)this.centerLongitude - ((this.centerTileCoordsOnWindow.x - (float)x) * this.degreesPerPixel);
    //    result.y = (float)this.centerLatitude + ((this.centerTileCoordsOnWindow.y - (float)y) * this.degreesPerPixel);

    //    return result;
    //}


    public Vector2 LonLatToParameter(double lon, double lat)
    {
        Vector2 result = new Vector2();

        double latStep = this.mapBounds.NorthSouthDistance / 256d;
        double lonStep = this.mapBounds.EastWestDistance / 256d;

        
        float correctedLongitude = Mathf.Clamp((float)lon, (float)this.mapBounds.West, (float)this.mapBounds.East);
        float correctedLatitude = Mathf.Clamp((float)lat, (float)this.mapBounds.North, (float)this.mapBounds.West);

        float westDistance = Mathf.Abs(correctedLongitude - (float)mapBounds.West);
        //float eastDistance = Mathf.Abs((float)mapBounds.East - correctedLongitude);
        //float northDistance = Mathf.Abs(correctedLatitude - (float)mapBounds.North);
        float southDistance = Mathf.Abs(correctedLatitude - (float)mapBounds.South);


        result.x =  westDistance / (float)this.mapBounds.EastWestDistance;
        result.y = southDistance / (float)this.mapBounds.NorthSouthDistance; ;

        return result;
    }




    //public Vector2 PixelToParameter(Vector2 pixelCoords)
    //{
    //    return PixelToParameter((int)pixelCoords.x, (int)pixelCoords.y);
    //}


    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();

        sb.Append("OSMTile[")
            .Append("\nX = " + X).Append(" -->  Longitude = " + this.longitude)
            .Append("\nY = " + Y).Append(" -->  Latitude = " + this.latitude)
            .Append("]");

        return sb.ToString();
    }

    public List<GameObject> GetNeightboursGameObjects()
    {
        return OSMTileProvider.GetOSMTileGameObjects(this.GetNeightbours());
    }


    public void PrepareZoom()
    {
        PrepareZoomIn();
        PrepareZoomOut();
    }

    public void PrepareZoomIn()
    {
        GetSubTiles();
    }

    public void PrepareZoomOut()
    {
        GetSuperTile();
    }


    public void PrepareZoomGameObjects()
    {
        PrepareZoomInGameObjects();
        PrepareZoomOutGameObjects();
    }

    public void PrepareZoomInGameObjects()
    {
        GetSubTilesGameObjects();
    }

    public void PrepareZoomOutGameObjects()
    {
        GetSuperTileGameObject();
    }

}
