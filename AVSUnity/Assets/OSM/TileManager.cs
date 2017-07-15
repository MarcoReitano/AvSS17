using UnityEngine;
#if UNITY_EDITOR
//using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// TileManager.
/// </summary>
/// 
[ExecuteInEditMode] // For OnEnable
public class TileManager : Singleton<TileManager>
{

    public static int tileRadius = 1;
    public static int LOD = 5;

    public static double OriginLongitude
    {
        get { return originLongitude; }
        set 
        {
            if (originLongitude != value)
            {
                originLongitude = value;
                //TerrainLayer.Reset();
            }
        }
    }
    public static double OriginLatitude
    {
        get { return originLatitude; }
        set 
        {
            if (originLongitude != value)
            {
                originLatitude = value;
                //TerrainLayer.Reset();
            }
        }
    }

    public static double TileWidth
    {
        get { return tileWidth; }
        set { tileWidth = value; }
    }
    public static double Scaling
    {
        get { return scaling; }
        set { scaling = value; }
    }

    private static double originLongitude = 6.9605d;//7.02744f; //6.9605d;//7.56211f;  Basti 6.93728f;
    private static double originLatitude = 50.9390d;//51.03304f; //50.9390d;//51.02344f;      50.93959f;
    private static double tileWidth = 0.01d;
    private static double scaling = 50000d;    


    [UnityEditor.MenuItem("City/CalculateScale")]
    private static void calculateScaling()
    {
        //abgeleitet von haversine formel... könnte auch einfacher gehn?

        double R = 6378.137d; //Radius of the earth in meters
        //double lat1 = originLatitude;
        //double lat2 = originLatitude;

        //double dLat = deg2rad(lat2 - lat1);
        double dLon = deg2rad(1d);

        double a =
            Math.Cos(deg2rad(originLatitude)) * Math.Cos(deg2rad(originLatitude)) *
            Math.Sin(dLon / 2d) * Math.Sin(dLon / 2d);

        double c = 2d * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        double d = R * c * 1000d;
        //Debug.Log("Scaling: " + d);
        scaling = d;
        //return d; 
    }
    private static double deg2rad(double deg)
    {
        return deg * (Math.PI / 180d);
    }

}
