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
    public void OnEnable()
    {
        calculateScaling();
        tiles.Clear();
        foreach (Tile tile in FindObjectsOfType(typeof(Tile)) as Tile[])
        {
            tiles.Add(tile.TileIndex[0] + ":" + tile.TileIndex[1], tile);
        }
    }

    public static int tileRadius = 0;
    public static int LOD = 5;

    public static double OriginLongitude
    {
        get { return originLongitude; }
        set 
        {
            if (originLongitude != value)
            {
                originLongitude = value;
                TerrainLayer.Reset();
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
                TerrainLayer.Reset();
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

    
    public static Dictionary<string, Tile> tiles = new Dictionary<string, Tile>();

#if UNITY_EDITOR
    [UnityEditor.MenuItem("City/CalculateScale")]
#endif
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
        Debug.Log("Scaling: " + d);
        scaling = d;
        //return d; 
    }
    private static double deg2rad(double deg)
    {
        return deg * (Math.PI / 180d);
    }
#if UNITY_EDITOR
    [UnityEditor.MenuItem("City/CreateTileMap")]
    public static void CreateTileMap()
    {
        //Find all existing and destroy
        //Tile[] tileGOs = FindObjectsOfType(typeof(Tile)) as Tile[];
        //for (int i = 0; i < tileGOs.Length; i++)
        //{
        //    DestroyImmediate(tileGOs[i].gameObject);            
        //}
        foreach (KeyValuePair<string, Tile> kV in tiles)
        {
            try
            {
                DestroyImmediate(kV.Value.gameObject);
            }
            catch (MissingReferenceException)
            {
                Debug.LogWarning("Trying to access dead object...");
            }
           
        }

        tiles.Clear();

        //Create new Tiles
        for (int i = -tileRadius; i <= tileRadius; i++)
        {
            for (int j = -tileRadius; j <= tileRadius; j++)
            {
                Tile newTile = Tile.CreateTileGO(i, j, LOD);
                tiles.Add(i + ":" + j, newTile);
            }
        }

        foreach (KeyValuePair<string, Tile> kV in tiles)
        {
            kV.Value.StartQuery();
        }
    }
#endif

    public GameObject playerGO;

    int[] currentTileIndex = new int[2] { 0, 0 };

    void Update()
    {
        if (playerGO != null)
        {
            Vector3 playerPosition = playerGO.transform.position;

            float tileUnityWidth = (float)(TileWidth * Scaling);

            currentTileIndex[0] = Mathf.RoundToInt(playerPosition.x / (tileUnityWidth));
            currentTileIndex[1] = Mathf.RoundToInt(playerPosition.z / (tileUnityWidth));

            //Debug.Log("x: " + currentTileIndex[0].ToString() + " y: " + currentTileIndex[1].ToString());

            if (!tiles.ContainsKey(currentTileIndex[0] + ":"+ currentTileIndex[1]))
            {
                Console.AddMessage("Creating new Tile: " + currentTileIndex.ToString() + " with LOD" + LOD);
                Tile newTile = Tile.CreateTileGO(currentTileIndex[0], currentTileIndex[1], LOD);
                tiles.Add(currentTileIndex[0] + ":" + currentTileIndex[1], newTile);
                newTile.StartQuery();
            }
        }
    }

    void OnGUI()
    {
        GUILayout.Label("Current TileIndex : " + currentTileIndex[0] + " : " + currentTileIndex[1]);
        GUILayout.Label("PlayerObject: " + playerGO);
    }
}
