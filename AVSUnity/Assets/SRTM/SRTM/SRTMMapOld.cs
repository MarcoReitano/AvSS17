//using System.Globalization;
//using System.IO;
//using System.Text;
//using UnityEditor;
//using UnityEngine;


//public enum InterpolationAlgorithm
//{
//    DIAMOND_SQUARE,
//    NORMAL_MIDPOINT_DISPLACEMENT
//}

//public class SRTMMapOld
//{
    
//    public float[][] floatMap;

//    private float[][] initialFloatmap;
//    public float[][] InitialFloatmap
//    {
//        get { return initialFloatmap; }
//        set
//        {
//            initialFloatmap = value;
//            this.floatMap = value;
//            this.size = this.Size;
//        }
//    }

//    private OSMBoundingBox bounds;
//    public OSMBoundingBox Bounds
//    {
//        get { return bounds; }
//        set
//        {
//            bounds = value;
//        }
//    }

//    private double cellSize;
//    public double CellSize
//    {
//        get { return cellSize; }
//        set { cellSize = value; }
//    }

//    public int HeightmapHeight
//    {
//        get { return this.floatMap.GetLength(0); }
//        set { }
//    }

//    public int HeightmapWidth
//    {
//        get { return this.floatMap.GetLength(1); }
//        set { }
//    }

//    private float heightSmooth;
//    public float HeightFactor
//    {
//        get { return heightSmooth; }
//        set { heightSmooth = value; }
//    }

//    private float roughness;
//    public float Roughness
//    {
//        get { return roughness; }
//        set { roughness = value; }
//    }


//    private Vector3 size;
//    public Vector3 Size
//    {
//        get
//        {
//            return this.size;
//        }
//        set { }
//    }



//    /// <summary>
//    /// 
//    /// </summary>
//    public SRTMMapOld()
//    {
//        this.initialFloatmap = new float[1][];
//        this.initialFloatmap[0] = new float[1]; 

//        this.floatMap = new float[1][];
//        this.floatMap[0] = new float[1];

//        this.bounds = new OSMBoundingBox();
//        this.cellSize = GeographicCoordinates.arcsec3Meter;
//        this.size = Vector3.zero;
//    }


//    /// <summary>
//    /// 
//    /// </summary>
//    public void Reset()
//    {
//        this.floatMap = initialFloatmap;
//    }



//    /// <summary>
//    /// 
//    /// </summary>
//    /// <param name="ter"></param>
//    /// <returns></returns>
//    public float[,] CalculateTerrainHeights(Terrain ter)
//    {
//        TerrainData terrain = ter.terrainData;

//        // Interpolieren
//        //this.floatMap 
//        float[,] localFloatmap = SmartInterpolateHeightmap(
//            SRTMMapOld.ConvertJaggedArrayToFloatArray(this.floatMap),
//            terrain.heightmapResolution, roughness);

//        // Abmessungen
//        int height = localFloatmap.GetLength(0);
//        int width = localFloatmap.GetLength(1);

//        terrain.size = this.CalculateSize();

//        //EditorUtility.DisplayProgressBar("CalculateTerrainHeights", "Neue Heightmap anlegen", 0.0f);
//        int heightmapResolution = terrain.heightmapWidth;
//        float[,] heightmapData = terrain.GetHeights(0, 0, heightmapResolution, heightmapResolution);
//        //float[,] heightmapData = new float[heightmapResolution, heightmapResolution];

//        //EditorUtility.DisplayProgressBar("CalculateTerrainHeights", "hRatio und WRatio", 0.0f);
//        float wRatio = (float)width / (float)heightmapResolution;
//        float hRatio = (float)height / (float)heightmapResolution;

//        //EditorUtility.DisplayProgressBar("CalculateTerrainHeights", "ProgressbarWerte initialisieren", 0.0f);
//        int gesamt = heightmapResolution * heightmapResolution;
//        float prozent = 1f / gesamt;
//        float progressBar = 0.0f;

//        //EditorUtility.DisplayProgressBar("CalculateTerrainHeights", "Schleife beginnen", 0.0f);
//        for (int y = 0; y < heightmapResolution; y++)
//        {
//            for (int x = 0; x < heightmapResolution; x++)
//            {
//                float tempU = 0f;
//                float tempD = 0f;
//                float tempL = 0f;
//                float tempR = 0f;

//                if (Mathf.Floor(x * wRatio) > 0
//                    && Mathf.Floor(x * wRatio) < width - 1
//                    && Mathf.Floor(y * hRatio) > 0
//                    && Mathf.Floor(y * hRatio) < height - 1)
//                {
//                    tempL = (localFloatmap[
//                            (int)Mathf.Floor(y * hRatio),
//                            (int)Mathf.Floor(x * wRatio) - 1]
//                            - localFloatmap[
//                            (int)Mathf.Floor(y * hRatio),
//                            (int)Mathf.Floor(x * wRatio)]) * wRatio;

//                    tempR = (localFloatmap[
//                            (int)Mathf.Floor(y * hRatio),
//                            (int)Mathf.Floor(x * wRatio)]
//                            - localFloatmap[
//                            (int)Mathf.Floor(y * hRatio),
//                            (int)Mathf.Floor(x * wRatio) + 1]) * wRatio;

//                    tempU = (localFloatmap[
//                            (int)Mathf.Floor(y * hRatio) - 1,
//                            (int)Mathf.Floor(x * wRatio)]
//                            - localFloatmap[
//                            (int)Mathf.Floor(y * hRatio),
//                            (int)Mathf.Floor(x * wRatio)]) * hRatio;

//                    tempD = (localFloatmap[
//                            (int)Mathf.Floor(y * hRatio),
//                            (int)Mathf.Floor(x * wRatio)]
//                            - localFloatmap[
//                            (int)Mathf.Floor(y * hRatio) + 1,
//                            (int)Mathf.Floor(x * wRatio)]) * hRatio;
//                }

//                var avg = (localFloatmap[
//                    (int)(y * hRatio),
//                    (int)(x * wRatio)])
//                    + (tempL + tempR + tempU + tempD);

//                heightmapData[x, y] = (avg * heightSmooth) / this.size.y;

//                progressBar += prozent;
//            }
//            //EditorUtility.DisplayProgressBar("Calculating Heightmap", "calculating...", progressBar);
//        }

//        this.floatMap = SRTMMapOld.ConvertFloatArrayToJaggedArray(heightmapData);

//        //EditorUtility.ClearProgressBar();
//        return heightmapData;
//    }


//    public static float GetMaxHeight(float[][] floatArray)
//    {
//        float max = floatArray[0][0];

//        for (int y = 0; y < floatArray.Length; y++)
//        {
//            for (int x = 0; x < floatArray[0].Length; x++)
//            {
//                if (floatArray[y][x] > max)
//                    max = floatArray[y][x];
//            }
//        }

//        return max;
//    }

//    public static float GetMinHeight(float[][] floatArray)
//    {
//        float min = floatArray[0][0];

//        for (int y = 0; y < floatArray.Length; y++)
//        {
//            for (int x = 0; x < floatArray[0].Length; x++)
//            {
//                if (floatArray[y][x] < min)
//                    min = floatArray[y][x];
//            }
//        }

//        return min;
//    }


//    /// <summary>
//    ///
//    /// </summary>
//    /// <returns></returns>
//    public static float GetHeight(float[][] floatArray)
//    {
//        /*
//         // 
//         //                           max = 4____    _                               <
//         //                        3____    /    \   | 
//         //        minValue = 2____/    \__/      \  |
//         //                                          | height = 4
//         //   min = 0-------------------------------_|_-----------------------------
//         //   
//         //   
//         //                  max = 4____    _
//         //                3___    /    \   | 
//         //                /   \__/      \  |
//         //               /                 | height = 6
//         //   min = 0----/------------------|---------------------------------
//         //             /                   |
//         //   min = -2_/                   _|_
//         //      
//         */
//        float min = SRTMMapOld.GetMinHeight(floatArray);
//        if (min > 0f)
//            min = 0f;
//        else
//            min *= -1;

//        return min + SRTMMapOld.GetMaxHeight(floatArray);
//    }



//    /// <summary>
//    /// 
//    /// </summary>
//    /// <param name="localFloatMap"></param>
//    /// <param name="heightmapResolution"></param>
//    /// <returns></returns>
//    private float[,] SmartInterpolateHeightmap(float[,] localFloatMap, int heightmapResolution, float roughness)
//    {
//        // TODO: Ausreißer rausfiltern


//        // anzahl durchläufe prüfen
//        int requiredInterpolCount = 0;
//        int floatmapsize = localFloatMap.GetLength(0);
//        while (floatmapsize < heightmapResolution)
//        {
//            requiredInterpolCount++;
//            floatmapsize = floatmapsize * 2 - 1;
//        }

//        // Progressbar-Werte initialaisieren
//        int gesamt = requiredInterpolCount;
//        float prozent = 1f / gesamt;
//        float progressBar = 0.0f;

//        int interpolCount = 0;
//        while (localFloatMap.GetLength(0) < heightmapResolution) // War terrain.heightmapWidth/2
//        {
//            localFloatMap = InterpolateHeightmapDiamondSquare(localFloatMap, roughness);
//            interpolCount++;

//            progressBar += prozent;
//            //EditorUtility.DisplayProgressBar(interpolCount + ". Interpolation of Heightmap", "interpolating...", progressBar);

//        }
//        EditorUtility.ClearProgressBar();
//        return localFloatMap;
//    }


//    /// <summary>
//    /// 
//    /// </summary>
//    /// <param name="localFloatMap"></param>
//    /// <returns></returns>
//    private float[,] InterpolateHeightmap(float[,] localFloatMap)
//    {
//        // Array doppelter größe anlegen
//        float[,] newArray = new float[localFloatMap.GetLength(0) * 2 - 1, localFloatMap.GetLength(1) * 2 - 1];

//        // altes array in neues übertragen
//        for (int y = 0; y < localFloatMap.GetLength(0); y++)
//            for (int x = 0; x < localFloatMap.GetLength(1); x++)
//                newArray[y * 2, x * 2] = localFloatMap[y, x];

//        // Zeilen-zwischenwerte berechnen
//        for (int y = 0; y < newArray.GetLength(0); y++)
//            for (int x = 1; x < newArray.GetLength(1) - 1; x++)
//                if (x % 2 == 1)
//                    newArray[y, x] = (newArray[y, x - 1] + newArray[y, x + 1]) / 2;

//        // zwischenzeilen zwischenwerte berechnen
//        for (int y = 1; y < newArray.GetLength(0) - 1; y++)
//            if (y % 2 == 1)
//                for (int x = 0; x < newArray.GetLength(1); x++)
//                    newArray[y, x] = (newArray[y - 1, x] + newArray[y + 1, x]) / 2;

//        //// mitte
//        for (int y = 1; y < newArray.GetLength(0) - 1; y++)
//            if (y % 2 == 1)
//                for (int x = 1; x < newArray.GetLength(1) - 1; x++)
//                    if (x % 2 == 1)
//                        newArray[y, x] =
//                            (newArray[y - 1, x - 1] +
//                             newArray[y - 1, x] +
//                             newArray[y - 1, x + 1] +
//                             newArray[y + 1, x - 1] +
//                             newArray[y + 1, x] +
//                             newArray[y + 1, x + 1] +
//                             newArray[y, x - 1] +
//                             newArray[y, x + 1]) / 8;

//        // calculate new CellSize
//        this.cellSize *= 2f / 3f;

//        return newArray;
//    }




//    /// <summary>
//    /// 
//    /// </summary>
//    /// <param name="localFloatMap"></param>
//    /// <returns></returns>
//    private float[,] InterpolateHeightmapDiamondSquare(float[,] localFloatMap, float roughness)
//    {
//        // TODO: Roughness einbeziehen

//        // Array doppelter größe anlegen
//        float[,] newArray = new float[localFloatMap.GetLength(0) * 2 - 1, localFloatMap.GetLength(1) * 2 - 1];

//        // altes array in neues übertragen
//        for (int y = 0; y < localFloatMap.GetLength(0); y++)
//            for (int x = 0; x < localFloatMap.GetLength(1); x++)
//                newArray[y * 2, x * 2] = localFloatMap[y, x];

//        //// mitte
//        for (int y = 0; y < newArray.GetLength(0); y++)
//            if (y % 2 == 1)
//                for (int x = 1; x < newArray.GetLength(1); x++)
//                    if (x % 2 == 1)
//                        newArray[y, x] =
//                            (newArray[y - 1, x - 1] +
//                             newArray[y - 1, x + 1] +
//                             newArray[y + 1, x - 1] +
//                             newArray[y + 1, x + 1]) / 4;


//        // Zeilen-zwischenwerte berechnen
//        for (int y = 0; y < newArray.GetLength(0); y++)
//            for (int x = 0; x < newArray.GetLength(1); x++)
//            {
//                if ((y % 2 == 0) && (x % 2 == 1))
//                {
//                    if (y == 0) // oberkante
//                    {
//                        newArray[y, x] =
//                            (newArray[y + 1, x] +
//                             newArray[y, x - 1] +
//                             newArray[y, x + 1]) / 3;
//                    }
//                    else if (y == newArray.GetLength(0) - 1) // unterkante
//                    {
//                        newArray[y, x] =
//                            (newArray[y - 1, x] +
//                             newArray[y, x - 1] +
//                             newArray[y, x + 1]) / 3;
//                    }
//                    else
//                    {
//                        newArray[y, x] =
//                            (newArray[y + 1, x] +
//                             newArray[y - 1, x] +
//                             newArray[y, x + 1] +
//                             newArray[y, x - 1]) / 4;
//                    }
//                }


//                else if ((x % 2 == 0) && (y % 2 == 1))
//                {
//                    if (x == 0) // linke kante
//                    {
//                        newArray[y, x] =
//                            (newArray[y - 1, x] +
//                             newArray[y + 1, x] +
//                             newArray[y, x + 1]) / 3;
//                    }
//                    else if (x == newArray.GetLength(1) - 1) // rechte kante
//                    {
//                        newArray[y, x] =
//                            (newArray[y - 1, x] +
//                             newArray[y + 1, x] +
//                             newArray[y, x - 1]) / 3;
//                    }
//                    else
//                    {
//                        newArray[y, x] =
//                            (newArray[y + 1, x] +
//                             newArray[y - 1, x] +
//                             newArray[y, x + 1] +
//                             newArray[y, x - 1]) / 4;
//                    }
//                }
//            }

//        //// zwischenzeilen zwischenwerte berechnen
//        //for (int y = 1; y < newArray.GetLength(0) - 1; y++)
//        //    if (y % 2 == 1)
//        //        for (int x = 0; x < newArray.GetLength(1); x++)
//        //        {
//        //            newArray[y, x] = (newArray[y, x] + newArray[y - 1, x] + newArray[y + 1, x]) / 2;
//        //        }


//        // calculate new CellSize
//        //this.cellSize *= 2f / 3f;

//        return newArray;
//    }



//    /// <summary>
//    /// Returns a String with the map-Values
//    /// </summary>
//    /// <returns></returns>
//    public string printMap()
//    {
//        StringBuilder sb = new StringBuilder();

//        for (int y = 0; y < HeightmapHeight; y++)
//        {
//            for (int x = 0; x < HeightmapWidth; x++)
//            {
//                sb.Append(floatMap[x][y].ToString("00.00", CultureInfo.CreateSpecificCulture("en-US"))).Append(" ");
//            }
//            sb.Append("\n");
//        }
//        return sb.ToString();
//    }

//    /// <summary>
//    /// 
//    /// </summary>
//    public void printMapToFile(string file, bool intmode)
//    {
//        try
//        {
//            TextWriter writer = File.CreateText(file);
//            int gesamt = HeightmapHeight * HeightmapHeight;
//            float prozent = 1f / gesamt;
//            float progressBar = 0.0f;

//            for (int y = 0; y < HeightmapHeight; y++)
//            {
//                for (int x = 0; x < HeightmapWidth; x++)
//                {
//                    if (intmode)
//                        writer.Write(floatMap[x][y].ToString("00", CultureInfo.CreateSpecificCulture("en-US")) + " ");
//                    else
//                        writer.Write(floatMap[x][y].ToString("00.0000", CultureInfo.CreateSpecificCulture("en-US")) + " ");

//                    progressBar += prozent;

//                }
//                //EditorUtility.DisplayProgressBar("Writing to File: " + file, "writing", progressBar);
//                writer.WriteLine();
//            }
//            writer.Close();
//            EditorUtility.ClearProgressBar();
//        }
//        catch
//        {
//            // TODO: good catch...  YEAH!!
//        }

//    }

//    /// <summary>
//    /// 
//    /// </summary>
//    public static void printMapToFile(string file, float[,] floatMap, int HeightmapResolution, bool intmode)
//    {
//        try
//        {
//            TextWriter writer = File.CreateText(file);
//            int gesamt = HeightmapResolution * HeightmapResolution;
//            float prozent = 1f / gesamt;
//            float progressBar = 0.0f;

//            for (int y = 0; y < HeightmapResolution; y++)
//            {
//                for (int x = 0; x < HeightmapResolution; x++)
//                {
//                    if (intmode)
//                        writer.Write(floatMap[x, y].ToString("00", CultureInfo.CreateSpecificCulture("en-US")) + " ");
//                    else
//                        writer.Write(floatMap[x, y].ToString("00.0000", CultureInfo.CreateSpecificCulture("en-US")) + " ");

//                    progressBar += prozent;

//                }
//                EditorUtility.DisplayProgressBar("Writing to File: " + file,
//                        "writing",
//                        progressBar);
//                writer.WriteLine();
//            }
//            writer.Close();
//            EditorUtility.ClearProgressBar();
//        }
//        catch
//        {
//            // TODO: good catch...  YEAH!!
//        }

//    }



//    public override string ToString()
//    {
//        StringBuilder sb = new StringBuilder();

//        sb.Append("floatmap [height=" + HeightmapHeight + ", width=" + HeightmapWidth + "] \n");
//        sb.Append("MapBounds [" + this.bounds.ToString() + "] \n");
//        sb.Append("Size [" + this.Size + "] \n");
//        sb.Append("CellSize [" + this.CellSize + "] \n");

//        sb.Append("GetMinHeight() [" + GetMinHeight(this.floatMap) + "] \n");
//        sb.Append("GetMaxHeight() [" + GetMaxHeight(this.floatMap) + "] \n");
//        sb.Append("GetHeight() [" + GetHeight(this.floatMap) + "] \n");
//        //sb.Append(printMap());
//        //printMapToFile();
//        return sb.ToString();
//    }

//    public Vector3 CalculateSize()
//    {
//        this.size = new Vector3(
//                (float)(bounds.EastWestDistance / GeographicCoordinates.meterArc),
//                GetHeight(this.floatMap) * HeightFactor,
//                (float)(bounds.NorthSouthDistance / GeographicCoordinates.meterArc));
//        return this.size;
//    }





//    public static float[][] ConvertFloatArrayToJaggedArray(float[,] floatarray)
//    {

//        float[][] jaggedArray = new float[floatarray.GetLength(0)][];
//        for (int y = 0; y < floatarray.GetLength(0); y++)
//        {
//            jaggedArray[y] = new float[floatarray.GetLength(1)];
//            for (int x = 0; x < floatarray.GetLength(1); x++)
//                jaggedArray[y][x] = floatarray[y, x];
//        }

//        return jaggedArray;
//    }

//    public static float[,] ConvertJaggedArrayToFloatArray(float[][] jaggedArray)
//    {

//        float[,] floatArray = new float[jaggedArray.Length, jaggedArray[0].Length];
//        for (int y = 0; y < jaggedArray.Length; y++)
//            for (int x = 0; x < jaggedArray[0].Length; x++)
//                floatArray[y, x] = jaggedArray[y][x];

//        return floatArray;
//    }


//    public static short[][] ConvertShortArrayToJaggedArray(short[,] shortArray)
//    {

//        short[][] jaggedArray = new short[shortArray.GetLength(0)][];
//        for (int y = 0; y < shortArray.GetLength(0); y++)
//        {
//            jaggedArray[y] = new short[shortArray.GetLength(1)];
//            for (int x = 0; x < shortArray.GetLength(1); x++)
//                jaggedArray[y][x] = shortArray[y, x];
//        }

//        return jaggedArray;
//    }

//    public static short[,] ConvertJaggedArrayToShortArray(short[][] jaggedArray)
//    {

//        short[,] shortArray = new short[jaggedArray.GetLength(0), jaggedArray.GetLength(1)];
//        for (int y = 0; y < jaggedArray.GetLength(0); y++)
//            for (int x = 0; x < jaggedArray.GetLength(1); x++)
//                shortArray[y, x] = jaggedArray[y][x];

//        return shortArray;
//    }

//}

