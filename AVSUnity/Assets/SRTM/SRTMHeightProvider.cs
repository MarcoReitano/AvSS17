using UnityEngine;
using System.Collections.Generic;
using System.Text;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// SRTMHeightProvider.
/// </summary>
#if UNITY_EDITOR
[InitializeOnLoad]
#endif
public static class SRTMHeightProvider
{
    static SRTMHeightProvider()
    {
#if UNITY_EDITOR
        if (EditorPrefs.HasKey("SRTMDataPath"))
        {
            string dataPath = EditorPrefs.GetString("SRTMDataPath");
            if (System.IO.Directory.Exists(dataPath))
            {
                SRTMDataPath = dataPath;
            }
            else
                Debug.Log("Couldnt find directory " + dataPath + " referenced as the SRTMDataPath! Please set the correct path in the C.I.T.Y. EditorWindow!");
        }
        else
        {
            Debug.Log("SRTMDataPath not set, please do so in the C.I.T.Y. EditorWindow!");
        }

        if (EditorPrefs.HasKey("OSMCachePath") && EditorPrefs.GetString("OSMCachePath") != "")
        {
            string dataPath = EditorPrefs.GetString("OSMCachePath");
            if (System.IO.Directory.Exists(dataPath))
            {
                OverpassQuery.oSMCachePath = dataPath;
            }
            else
                UnityEngine.Debug.Log("Couldnt find directory " + dataPath + " referenced as the OSMCachePath! Please set the correct path in the C.I.T.Y. EditorWindow!");
        }
        else
        {
            UnityEngine.Debug.Log("OSMCachePath not set, please do so in the C.I.T.Y. EditorWindow!");
        }
#endif
    }
    //public static string SRTMDataPath = "/Users/mreitano/Resources/SRTM/version21/eurasia"; // Büro Mac Marco
    //public static string SRTMDataPath = "C:/Users/MReitano/Documents/DEV/Resources/SRTM/version21/eurasia"; // Büro Win Marco
    //public static string SRTMDataPath = "E:/DEV/SRTM"; // Home Win Marco
    public static string SRTMDataPath = "C:/UnityWorkspace/GeoData/SRTM/";//eurasia"; // Dennis Büro


    public static void PrintDataCells()
    {
        StringBuilder sb = new StringBuilder();

        foreach (int lat in dataCells.Keys)
        {
            foreach (int lon in dataCells[lat].Keys)
            {
                sb.AppendLine(dataCells[lat][lon].ToString());
            }
        }

        Debug.Log(sb.ToString());
    }

        
    private static Dictionary<int, Dictionary<int, SRTMDataCell>> dataCells = new Dictionary<int, Dictionary<int, SRTMDataCell>>();

    public static SRTMDataCell GetSRTMDataCell(int latIndex, int longIndex)
    {
        if (dataCells == null)
            dataCells = new Dictionary<int, Dictionary<int, SRTMDataCell>>();

        if (!dataCells.ContainsKey(latIndex))
            dataCells[latIndex] = new Dictionary<int, SRTMDataCell>();

        if (!dataCells[latIndex].ContainsKey(longIndex))
        {
            dataCells[latIndex][longIndex] = new SRTMDataCell(latIndex, longIndex);
            Debug.Log("Created new SRTMDataCell " + dataCells[latIndex][longIndex].ToString());
        }
        return dataCells[latIndex][longIndex];
    }


    public static void LoadCells(OSMBoundingBox boundingBox)
    {
        int minLatIndex = (int)System.Math.Floor(boundingBox.MinLatitude);
        int maxLatIndex = (int)System.Math.Floor(boundingBox.MaxLatitude);
        int minLongIndex = (int)System.Math.Floor(boundingBox.MinLongitude);
        int maxLongIndex = (int)System.Math.Floor(boundingBox.MaxLongitude);

        for (int i = minLatIndex; i < maxLatIndex; i++)
        {
            for (int j = minLongIndex; j < maxLongIndex; j++)
            {
                GetSRTMDataCell(i, j);// new SRTMDataCell(i, j);
            }
        }
    }

    public static float GetHeight(double latitude, double longitude)
    {
        int latIndex = (int)System.Math.Floor(latitude);
        int longIndex = (int)System.Math.Floor(longitude);
         
        return GetSRTMDataCell(latIndex, longIndex).GetHeight(latitude, longitude);
    }

    public static float GetInterpolatedHeight(double latitude, double longitude)
    {
        int latIndex = (int)System.Math.Floor(latitude);
        int longIndex = (int)System.Math.Floor(longitude);
        
        return GetSRTMDataCell(latIndex, longIndex).GetInterpolatedHeight(latitude, longitude);
    }

    public static float[,] GetTerrain(OSMBoundingBox boundingBox, out float height)
    {
        float[,] result = new float[257, 257];
        
        float minheight = float.MaxValue;
        float maxHeight = float.MinValue;

        for (int i = 0; i < 257; i++)
        {
            for (int j = 0; j < 257; j++)
            {
                result[i, j] = SRTMHeightProvider.GetHeight(boundingBox.MinLatitude + ((double)i / 257) * (boundingBox.MaxLatitude - boundingBox.MinLatitude),
                                                            boundingBox.MinLongitude + ((double)j / 257) * (boundingBox.MaxLongitude - boundingBox.MinLongitude));
                if (result[i, j] < -32767)
                {
                    continue;
                }
                minheight = Mathf.Min(result[i, j], minheight);
                maxHeight = Mathf.Max(result[i, j], maxHeight);
            }
        }

        for (int i = 0; i < 257; i++)
        {
            for (int j = 0; j < 257; j++)
            {
                if (result[i, j] < -32767)
                    result[i, j] = minheight;
                else
                    result[i, j] = result[i, j] / maxHeight;
            }
        }
        // Prevent zerodivision by TerrainComponent
        if (maxHeight > 0f)
            height = maxHeight;
        else
            height = 1f;

        return result;
    }
    public static float[,] GetInterpolatedTerrain(OSMBoundingBox boundingBox, out float height)
    {
        float[,] result = new float[257, 257];
               
        float minHeight = float.MaxValue;
        float maxHeight = float.MinValue;

        for (int i = 0; i < 257; i++)
        {
            for (int j = 0; j < 257; j++)
            {
                result[i, j] = SRTMHeightProvider.GetInterpolatedHeight(boundingBox.MinLatitude + ((double)i / 257) * (boundingBox.MaxLatitude - boundingBox.MinLatitude),
                                                            boundingBox.MinLongitude + ((double)j / 257) * (boundingBox.MaxLongitude - boundingBox.MinLongitude));
                if (result[i, j] < -32767)
                    continue;

                minHeight = Mathf.Min(result[i, j], minHeight);
                maxHeight = Mathf.Max(result[i, j], maxHeight);
            }
        }

        for (int i = 0; i < 257; i++)
        {
            for (int j = 0; j < 257; j++)
            {
                if (result[i, j] < -32767)
                    result[i, j] = minHeight;
                else
                    result[i, j] = result[i, j] / maxHeight;
            }
        }

        // Prevent zerodivision by TerrainComponent
        if (maxHeight > 0f)
            height = maxHeight;
        else
            height = 1f;

        TerrainLayer.MinTerrainHeight = minHeight;
        TerrainLayer.MaxTerrainHeight = maxHeight;

        return result;
    }
    public static float[,] GetInterpolatedTerrain(OSMBoundingBox boundingBox, out float height, out float minHeight, out float maxHeight)
    {
        float[,] result = new float[257, 257];
      
        minHeight = float.MaxValue;
        maxHeight = float.MinValue;

        for (int i = 0; i < 257; i++)
        {
            for (int j = 0; j < 257; j++)
            {
                result[i, j] = SRTMHeightProvider.GetInterpolatedHeight(boundingBox.MinLatitude + ((double)i / 257) * (boundingBox.MaxLatitude - boundingBox.MinLatitude),
                                                            boundingBox.MinLongitude + ((double)j / 257) * (boundingBox.MaxLongitude - boundingBox.MinLongitude));
                if (result[i, j] < -32767)
                    continue;

                minHeight = Mathf.Min(result[i, j], minHeight);
                maxHeight = Mathf.Max(result[i, j], maxHeight);
            }
        }

        for (int i = 0; i < 257; i++)
        {
            for (int j = 0; j < 257; j++)
            {
                if (result[i, j] < -32767)
                    result[i, j] = minHeight;
                else
                    result[i, j] = result[i, j] / maxHeight;
            }
        }

        Debug.Log("MaxHeight = " + maxHeight);
        // Prevent zerodivision by TerrainComponent
        if (maxHeight > 0f)
            height = maxHeight;
        else
            height = 1f;

        return result;
    }

}
