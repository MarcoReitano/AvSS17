using UnityEngine;
using System;
using System.Text;
using System.IO;
using System.Globalization;

public class GeographicCoordinates : MonoBehaviour
{
    public static double arcmin = 60f / 3600f;  // 0,016... grad
    public static double arcsec = 1f / 3600f;   // 0,00027.. grad
    public static double arcsec3 = GeographicCoordinates.arcsec * 3; // 0,00083... grad

    public static double arcminMeter = 1850f;
    public static double arcsecMeter = (arcminMeter / 60);  // 30,83... meter
    public static double arcsec3Meter = (arcminMeter / 60) * 3; // 92,6... meter

    public static double meterArc = arcmin / 1850f;  // 0,000009
    public static double kmArc = meterArc * 1000f;

    public static double EarthRadius = 6378.137d; //6378137;

    public static double MinLatitude = -85.05112878;
    public static double MaxLatitude = 85.05112878;
    public static double MinLongitude = -180;
    public static double MaxLongitude = 180;


    [SerializeField]
    private float longitude;
    public float Longitude
    {
        get { return longitude; }
        set { longitude = value; }
    }

    [SerializeField]
    private float latitude;
    public float Latitude
    {
        get { return latitude; }
        set { latitude = value; }
    }

    private float height;
    public float Height
    {
        get { return height; }
        set { height = value; }
    }
    
    //public Vector2 Coords2D
    //{
    //    get
    //    {
    //        return ConvertLonLatToXY(this.longitude, this.latitude, GeographicCoordinates.MapCenter);
    //    }
    //    set
    //    {
    //        Vector2 convert = GeographicCoordinates.ConvertXYtoLonLat(value.x, value.y, GeographicCoordinates.MapCenter);
    //        this.longitude = convert.x;
    //        this.latitude = convert.y;
    //    }
    //}

    public Vector3 Coords3D
    {
        get
        {
            Vector2 convert = GeographicCoordinates.ConvertLonLatToXY(this.longitude, this.latitude, 0f);
            return new Vector3(convert.x, 0f, convert.y);
        }
        set
        {
            //Debug.Log("Before: " + this.longitude + ", " +  this.latitude);
            Vector2 convert = GeographicCoordinates.ConvertXYtoLonLat(value.x, value.z, 0f);
            this.longitude = convert.x;
            this.latitude = convert.y;
            //Debug.Log("After: " + this.longitude + ", " + this.latitude);
        }
    }


  


    /** Nach: http://code.google.com/p/osmdroid/source/browse/trunk/osmdroid-android/src/microsoft/mappoint/TileSystem.java?r=854
     * Determines the map width and height (in pixels) at a specified level of detail.
     *
     * @param levelOfDetail
     *            Level of detail, from 1 (lowest detail) to 23 (highest detail)
     * @return The map width and height in pixels
     */

    public static int MapSize(int zoomLevel)
    {
        return 256 << zoomLevel;
    }



    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }





    public void OpenStreetView()
    {

        StringBuilder streetView = new StringBuilder();
        streetView.Append("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Strict//EN\"\n");
        streetView.Append("\"http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd\">\n");
        streetView.Append("<html xmlns=\"http://www.w3.org/1999/xhtml\" xmlns:v=\"urn:schemas-microsoft-com:vml\">\n");
        streetView.Append("    <head>\n");
        streetView.Append("        <meta http-equiv=\"content-type\" content=\"text/html; charset=utf-8\"/>\n");
        streetView.Append("        <title>SeriousGames VerkehrsnetzFramework Google StreetView</title>\n\n");

        streetView.Append("        <script src=\"http://maps.google.com/maps?file=api&amp;v=2.x&amp;key=ABQIAAAAzr2EBOXUKnm_jVnk0OJI7xSosDVG8KKPE1-m51RBrvYughuyMxQ-i1QfUnH94QxWIa6N4U6MouMmBA\"\n");
        streetView.Append("            type=\"text/javascript\">\n");
        streetView.Append("        </script>\n\n");

        streetView.Append("        <script type=\"text/javascript\">\n");
        streetView.Append("            var myPano;\n");
        streetView.Append("            function initialize() {\n");
        streetView.Append("                var location = new GLatLng(");
        streetView.Append(latitude.ToString("0.0####", CultureInfo.CreateSpecificCulture("en-US")));
        streetView.Append(",");
        streetView.Append(longitude.ToString("0.0####", CultureInfo.CreateSpecificCulture("en-US")));
        streetView.Append(");\n");

        streetView.Append("                panoramaOptions = { latlng:location };\n");
        streetView.Append("                myPano = new GStreetviewPanorama(document.getElementById(\"pano\"), panoramaOptions);\n");
        streetView.Append("                GEvent.addListener(myPano, \"error\", handleNoFlash);\n");
        streetView.Append("            }\n\n");

        streetView.Append("            function handleNoFlash(errorCode) {\n");
        streetView.Append("                if (errorCode == FLASH_UNAVAILABLE) {\n");
        streetView.Append("                    alert(\"Error: Flash doesn\'t appear to be supported by your browser\");\n");
        streetView.Append("                    return;\n");
        streetView.Append("                }\n");
        streetView.Append("            }\n");
        streetView.Append("        </script>\n");
        streetView.Append("    </head>\n");
        streetView.Append("    <body onload=\"initialize()\" onunload=\"GUnload()\">\n");
        streetView.Append("        <div name=\"pano\" id=\"pano\" style=\"width: 1024px; height: 768px\"></div>\n");
        streetView.Append("    </body>\n");
        streetView.Append("</html>");


        string tempFile = Path.GetTempFileName() + ".html";
        TextWriter htmlFile = new StreamWriter(tempFile);

        // write a line of text to the file
        htmlFile.WriteLine(streetView.ToString());

        // close the stream
        htmlFile.Close();
        Application.OpenURL(tempFile);

        WWW www = new WWW("http://maps.googleapis.com/maps/api/streetview?size=640x480&location=" + latitude.ToString("0.0####", CultureInfo.CreateSpecificCulture("en-US")) + "," + longitude.ToString("0.0####", CultureInfo.CreateSpecificCulture("en-US")) + "&heading=151.78&pitch=-0.76&sensor=false");
        Texture2D streetViewTextur = new Texture2D(640, 480);

        while (!www.isDone)
        { // wait;

        }
        www.LoadImageIntoTexture(streetViewTextur);
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Plane);
        go.GetComponent<Renderer>().sharedMaterial.mainTexture = streetViewTextur;


    }




    public static Vector2 ConvertLonLatToXY(double lon, double lat, double centerLon)
    {
        Vector2 result = new Vector2();

        //double earthRadius = EarthRadius;// 637813.70f; //6378.100f;
        //result.x = (float)((lon - centerLon) * earthRadius);

        //double sinLat = Math.Sin(lat);
        //result.y = (float)(((double)0.5f * (double)Math.Log((double)(1 + sinLat) / (1 - sinLat))) * earthRadius);


        result.x = (float)((lon - TileManager.OriginLongitude) * TileManager.Scaling);
        result.y = (float)((lat - TileManager.OriginLatitude) * TileManager.Scaling); 
        return result;
    }


    public static Vector2 ConvertXYtoLonLat(double x, double y, double centerLon)
    {
        Vector2 result = new Vector2();
        //double earthRadius = 637813.70f; //6378.100f;

        //// longitude
        //result.x = (float)(x / earthRadius + centerLon);

        //// latitude
        //result.y = (float)Math.Atan(Math.Sinh(y / earthRadius));


        result.x = (float) (x / TileManager.Scaling + TileManager.OriginLongitude);
        result.y = (float)(y / TileManager.Scaling + TileManager.OriginLatitude);
        return result;
    }

    //public static Vector2 ConvertLonLatToXY(float lon, float lat, float centerLon)
    //{

    //    return GeographicCoordinates.geo_utm(lat, lon, GeographicCoordinates.MapCenter);
    //}


    //public static Vector2 ConvertXYZToLonLat(Vector3 point, float centerLon)
    //{
    //    //Vector2 result = new Vector2();

    //    return GeographicCoordinates.utm_geo(point.x, point.y, 0f);

    //    //return result;
    //}



    //public void CalculateTileXY(float OSMZoom)
    //{
    //    OSMtileX = Mathf.FloorToInt((lon + 180) / 360 * Mathf.Pow(2, OSMZoom));
    //    OSMtileY = Mathf.FloorToInt((1 - Mathf.Log(Mathf.Tan(lat * Mathf.PI / 180) + 1 / Mathf.Cos(lat * Mathf.PI / 180)) / Mathf.PI) / 2 * Mathf.Pow(2, OSMZoom));

    //    // calculate the position ON the tile itself - (due to variable "TileSize" this is ready for a yet to be done new smooth zoom)
    //    PositionOnTileX = Mathf.FloorToInt((((lon + 180) / 360 * Mathf.Pow(2, OSMZoom)) - OSMtileX) * TileSize);
    //    PositionOnTileY = Mathf.FloorToInt((((1 - Mathf.Log(Mathf.Tan(lat * Mathf.PI / 180) + 1 / Mathf.Cos(lat * Mathf.PI / 180)) / Mathf.PI) / 2 * Mathf.Pow(2, OSMZoom)) - OSMtileY) * TileSize);

    //}




    // http://wiki.openstreetmap.org/wiki/Slippy_map_tilenames
    public static Vector2 WorldToTilePos(double lon, double lat, int zoom)
    {
        Vector2 p = new Vector2();
        p.x = (float)((lon + 180.0) / 360.0 * (1 << zoom));
        p.y = (float)((1.0 - Math.Log(Math.Tan(lat * Math.PI / 180.0) +
            1.0 / Math.Cos(lat * Math.PI / 180.0)) / Math.PI) / 2.0 * (1 << zoom));

        return p;
    }


    public static Vector2 TileToWorldPos(int tile_x, int tile_y, int zoom)
    {
        Vector2 p = new Vector2();
        double n = Math.PI - ((2.0 * Math.PI * tile_y) / Math.Pow(2.0, zoom));

        p.x = (float)((tile_x / Math.Pow(2.0, zoom) * 360.0) - 180.0);
        p.y = (float)(180.0 / Math.PI * Math.Atan(Math.Sinh(n)));

        return p;
    }




    #region Old Stuff

    //// http://www.whoi.edu/marine/ndsf/utility/NDSFref.txt
    ///*---------------------------------------------------------*/
    ///* parameters and functions for UTM<->Lat/Long conversions */
    ///* UTM conversion is based on WGS84 ellipsoid parameters   */
    ///*---------------------------------------------------------*/

    //public static float RADIUS = 6378137.0f;
    //public static float FLATTENING = 0.00335281068f; /* GRS80 or WGS84 */
    //public static float K_NOT = 0.9996f;     /* UTM scale factor */
    //public static float FALSE_EASTING = 500000.0f;
    //public static float FALSE_NORTHING = 10000000.0f;
    //public bool showCoords = false;

    ///*--------------------------------------------------------*/
    ///* These Java functions were converted from original      */
    ///* C fucntions provided by Dana Yoerger, Steve Gegg,      */
    ///* and Louis Whitcomb at WHOI                             */
    ///*--------------------------------------------------------*/

    //public static Vector2 geo_utm(float lat, float lon, float zone)
    //{
    //    zone = (int)((lon + 180f) / 6f);
    //    /* first compute the necessary geodetic parameters and constants */

    //    float lambda_not = ((-180.0f + zone * 6.0f) - 3.0f) / Mathf.Deg2Rad;
    //    float e_squared = 2.0f * FLATTENING - FLATTENING * FLATTENING;
    //    float e_fourth = e_squared * e_squared;
    //    float e_sixth = e_fourth * e_squared;
    //    float e_prime_sq = e_squared / (1.0f - e_squared);
    //    float sin_phi = Mathf.Sin(lat);
    //    float tan_phi = Mathf.Tan(lat);
    //    float cos_phi = Mathf.Cos(lat);
    //    float N = RADIUS / Mathf.Sqrt(1.0f - e_squared * sin_phi * sin_phi);
    //    float T = tan_phi * tan_phi;
    //    float C = e_prime_sq * cos_phi * cos_phi;
    //    float M = RADIUS * (
    //        (1.0f - e_squared * 0.25f - 0.046875f * e_fourth - 0.01953125f * e_sixth)
    //        * lat - (0.375f * e_squared + 0.09375f * e_fourth + 0.043945313f * e_sixth)
    //        * Mathf.Sin(2.0f * lat) + (0.05859375f * e_fourth + 0.043945313f * e_sixth)
    //        * Mathf.Sin(4.0f * lat) - (0.011393229f * e_sixth)
    //        * Mathf.Sin(6.0f * lat));
    //    float A = (lon - lambda_not) * cos_phi;
    //    float A_sq = A * A;
    //    float A_fourth = A_sq * A_sq;

    //    /* now go ahead and compute X and Y */

    //    float x_utm = K_NOT * N * (A + (1.0f - T + C) * A_sq * A / 6.0f +
    //         (5.0f - 18.0f * T + T * T + 72.0f * C -
    //          58.0f * e_prime_sq) * A_fourth * A / 120.0f);

    //    /* note:  specific to UTM, vice general trasverse mercator.  
    //       since the origin is at the equator, M0, the M at phi_0, 
    //       always equals zero, and I won't compute it   */

    //    float y_utm = K_NOT * (M + N * tan_phi * (A_sq / 2.0f +
    //             (5.0f - T + 9.0f * C + 4.0f * C * C) * A_fourth / 24.0f +
    //             (61.0f - 58.0f * T + T * T + 600.0f * C -
    //              330.0f * e_prime_sq) * A_fourth * A_sq / 720.0f));

    //    /* now correct for false easting and northing */

    //    //if (lat < 0)
    //    //{
    //    //    y_utm += 10000000.0f;
    //    //}
    //    //x_utm += 500000f;

    //    return new Vector2(x_utm, y_utm);
    //}


    //public static Vector2 utm_geo(float x_utm, float y_utm, float zone)
    //{
    //    /* first, subtract the false easting */
    //    x_utm = x_utm - FALSE_EASTING;

    //    /* compute the necessary geodetic parameters and constants */

    //    float e_squared = 2.0f * FLATTENING - FLATTENING * FLATTENING;
    //    float e_fourth = e_squared * e_squared;
    //    float e_sixth = e_fourth * e_squared;
    //    float oneminuse = Mathf.Sqrt(1.0f - e_squared);

    //    /* compute the footpoint latitude */

    //    float M = y_utm / K_NOT;
    //    float mu = M / (RADIUS * (1.0f - 0.25f * e_squared -
    //                0.046875f * e_fourth - 0.01953125f * e_sixth));
    //    float e1 = (1.0f - oneminuse) / (1.0f + oneminuse);
    //    float e1sq = e1 * e1;
    //    float footpoint = mu + (1.5f * e1 - 0.84375f * e1sq * e1) * Mathf.Sin(2.0f * mu) +
    //            (1.3125f * e1sq - 1.71875f * e1sq * e1sq) * Mathf.Sin(4.0f * mu) +
    //            (1.57291666667f * e1sq * e1) * Mathf.Sin(6.0f * mu) +
    //            (2.142578125f * e1sq * e1sq) * Mathf.Sin(8.0f * mu);


    //    /* compute the other necessary terms */

    //    float e_prime_sq = e_squared / (1.0f - e_squared);
    //    float sin_phi = Mathf.Sin(footpoint);
    //    float tan_phi = Mathf.Tan(footpoint);
    //    float cos_phi = Mathf.Cos(footpoint);
    //    float N = RADIUS / Mathf.Sqrt(1.0f - e_squared * sin_phi * sin_phi);
    //    float T = tan_phi * tan_phi;
    //    float Tsquared = T * T;
    //    float C = e_prime_sq * cos_phi * cos_phi;
    //    float Csquared = C * C;
    //    float denom = Mathf.Sqrt(1.0f - e_squared * sin_phi * sin_phi);
    //    float R = RADIUS * oneminuse * oneminuse / (denom * denom * denom);
    //    float D = x_utm / (N * K_NOT);
    //    float Dsquared = D * D;
    //    float Dfourth = Dsquared * Dsquared;

    //    float lambda_not = ((-180.0f + zone * 6.0f) - 3.0f) * Mathf.Deg2Rad;


    //    /* now, use the footpoint to compute the real latitude and longitude */

    //    float lat = footpoint - (N * tan_phi / R) * (0.5f * Dsquared - (5.0f + 3.0f * T + 10.0f * C -
    //                         4.0f * Csquared - 9.0f * e_prime_sq) * Dfourth / 24.0f +
    //                         (61.0f + 90.0f * T + 298.0f * C + 45.0f * Tsquared -
    //                          252.0f * e_prime_sq -
    //                          3.0f * Csquared) * Dfourth * Dsquared / 720.0f);
    //    float lon = lambda_not + (D - (1.0f + 2.0f * T + C) * Dsquared * D / 6.0f +
    //                       (5.0f - 2.0f * C + 28.0f * T - 3.0f * Csquared + 8.0f * e_prime_sq +
    //                        24.0f * Tsquared) * Dfourth * D / 120.0f) / cos_phi;

    //    return new Vector2(lon, lat);
    //}



    ///* Nach: http://code.google.com/p/osmdroid/source/browse/trunk/osmdroid-android/src/microsoft/mappoint/TileSystem.java?r=854
    // * 
    // * Converts a point from latitude/longitude WGS-84 coordinates (in degrees) into pixel XY
    // * coordinates at a specified level of detail.
    // *
    // * @param latitude
    // *            Latitude of the point, in degrees
    // * @param longitude
    // *            Longitude of the point, in degrees
    // * @param levelOfDetail
    // *            Level of detail, from 1 (lowest detail) to 23 (highest detail)
    // * @param reuse
    // *            An optional Point to be recycled, or null to create a new one automatically
    // * @return Output parameter receiving the X and Y coordinates in pixels
    // */
    //public static Vector2 LatLongToPixelXY(double latitude, double longitude, int levelOfDetail)
    //{
    //    Vector2 result = new Vector2();

    //    latitude.Clamp(MinLatitude, MaxLatitude);
    //    longitude.Clamp(MinLongitude, MaxLongitude);

    //    double x = (longitude + 180) / 360;
    //    double sinLatitude = Math.Sin(latitude * Math.PI / 180);
    //    double y = 0.5 - Math.Log((1 + sinLatitude) / (1 - sinLatitude)) / (4 * Math.PI);

    //    int mapSize = MapSize(levelOfDetail);
    //    double resultXUnclamped = x * mapSize + 0.5;
    //    double resultYUnclamped = y * mapSize + 0.5;
    //    result.x = (int)resultXUnclamped.Clamp(0, mapSize - 1);
    //    result.y = (int)resultYUnclamped.Clamp(0, mapSize - 1);
    //    return result;
    //}


    ///**
    //     * Converts a pixel from pixel XY coordinates at a specified level of detail into
    //     * latitude/longitude WGS-84 coordinates (in degrees).
    //     *
    //     * @param pixelX
    //     *            X coordinate of the point, in pixels
    //     * @param pixelY
    //     *            Y coordinate of the point, in pixels
    //     * @param levelOfDetail
    //     *            Level of detail, from 1 (lowest detail) to 23 (highest detail)
    //     * @param reuse
    //     *            An optional GeoPoint to be recycled, or null to create a new one automatically
    //     * @return Output parameter receiving the latitude and longitude in degrees.
    //     */
    //    public static Vector2 PixelXYToLatLong(int pixelX, int pixelY, int levelOfDetail) {
    //           Vector2 result = new Vector2();

    //            double mapSize = MapSize(levelOfDetail); 
    //            double x = (pixelX.Clamp(0, mapSize - 1) / mapSize) - 0.5;
    //            double y = 0.5 - (Clip(pixelY, 0, mapSize - 1) / mapSize);

    //            double latitude = 90 - 360 * Math.Atan(Math.Exp(-y * 2 * Math.PI)) / Math.PI;
    //            double longitude = 360 * x;

    //            result.setLatitudeE6((int) (latitude * 1E6));
    //            result.setLongitudeE6((int) (longitude * 1E6));
    //            return result;
    //    }

#endregion // Old Stuff

    #region Gizmo-related Methods

    /// <summary>
    /// 
    /// </summary>
    public void OnDrawGizmos()
    {
        //Vector2 result = GeographicCoordinates.ConvertXYtoLonLat(transform.position.x, transform.position.z, GeographicCoordinates.MapCenter);
        //this.longitude = result.x;
        //this.latitude = result.y;
        //Handles.Label(this.transform.position, "lon: " + result.x + "\nlat: " + result.y);
        
        //if (showCoords)
        //    Handles.Label(this.transform.position, "lon: " + longitude + "\nlat: " + latitude);
    }



    /// <summary>
    /// 
    /// </summary>
    public void OnDrawGizmosSelected()
    {

    }
    #endregion

}
