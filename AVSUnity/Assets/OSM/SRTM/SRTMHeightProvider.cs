using UnityEngine;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;

/// <summary>
/// SRTMHeightProvider.
/// </summary>
[InitializeOnLoad]
public static class SRTMHeightProvider
{
    static SRTMHeightProvider()
    {
#if UNITY_EDITOR
        if (EditorPrefs.HasKey("SRTMDataPath"))
        {
            //string dataPath = EditorPrefs.GetString("SRTMDataPath");
            string dataPath = Application.dataPath + @"/SCTM_Data_TMP";
            if (System.IO.Directory.Exists(dataPath))
            {
                SRTMDataPath = dataPath;
            }
            else
                Directory.CreateDirectory(Application.dataPath + @"/SCTM_Data_TMP");
                Debug.Log("Couldnt find directory " + dataPath + " referenced as the SRTMDataPath! Was generated automatically.");
        }
        else 
        {
            Directory.CreateDirectory(Application.dataPath + @"/SCTM_Data_TMP");
            Debug.Log("SRTMDataPath not set, created default folder.");
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
    public static string SRTMDataPath = "C:/Users/MReitano/Documents/DEV/Resources/SRTM/version21/eurasia"; // Büro Win Marco
    //public static string SRTMDataPath = "C:/Users/Marco/Documents/DEV/Resources/SRTMDaten/v21srtm3/eurasia"; // Home Win Marco
    //public static string SRTMDataPath = "C:/UnityWorkspace/GeoData/SRTM/version21/eurasia"; // Dennis Büro


    private static SRTMDataCell[,] dataCells = new SRTMDataCell[120, 360];
    private static SRTMDataCell nullCell = new SRTMDataCell();

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
                dataCells[i, j] = new SRTMDataCell(i, j);
            }
        }
    }

    public static float GetHeight(double latitude, double longitude)
    {
        if (latitude < 0d || longitude < 0d)
            return 0f;
        int latIndex = (int)System.Math.Floor(latitude);
        int longIndex = (int)System.Math.Floor(longitude);

        if (dataCells[latIndex, longIndex] == null)
            dataCells[latIndex, longIndex] = new SRTMDataCell(latIndex, longIndex);

        return dataCells[latIndex, longIndex].GetHeight(latitude, longitude);
    }
    public static float GetInterpolatedHeight(double latitude, double longitude)
    {
        if (latitude < 0d || longitude < 0d)
            return 0f;

        //return GetHeight(latitude, longitude);
        int latIndex = (int)System.Math.Floor(latitude);
        int longIndex = (int)System.Math.Floor(longitude);

        if (dataCells[latIndex, longIndex] == null)
        {
            //Debug.Log("DataCell loaded: " + latIndex + " " + longIndex);
            dataCells[latIndex, longIndex] = new SRTMDataCell(latIndex, longIndex);
        }
        return dataCells[latIndex, longIndex].GetInterpolatedHeight(latitude, longitude);
    }

    public static float[,] GetTerrain(OSMBoundingBox boundingBox, out float height)
    {
        float[,] result = new float[257, 257];

        if (boundingBox.MinLatitude < 0d || boundingBox.MaxLatitude < 0d || boundingBox.MinLongitude < 0d || boundingBox.MaxLongitude < 0d)
        {
            height = 1f;
            return result;
        }

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

        if (boundingBox.MinLatitude < 0d || boundingBox.MaxLatitude < 0d || boundingBox.MinLongitude < 0d || boundingBox.MaxLongitude < 0d)
        {
            height = 1f;
            return result;
        }

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

        if (boundingBox.MinLatitude < 0d || boundingBox.MaxLatitude < 0d || boundingBox.MinLongitude < 0d || boundingBox.MaxLongitude < 0d)
        {
            height = 1f;
            minHeight = 0f;
            maxHeight = 1f;
            return result;
        }

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

        // Prevent zerodivision by TerrainComponent
        if (maxHeight > 0f)
            height = maxHeight;
        else
            height = 1f;

        return result;
    }
}
