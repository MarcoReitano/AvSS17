using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.IO;
using Ionic.Zip;
using System;
using System.Text;
using System.Globalization;
//using ICSharpCode.SharpZipLib.Zip;

/// <summary>
/// SRTMDataCell.
/// </summary>
public class SRTMDataCell
{
    public SRTMDataCell()
    {
    }

    public SRTMDataCell(int latIndex, int longIndex)
    {
        this.latIndex = latIndex;
        this.longIndex = longIndex;

        try
        {
            Debug.Log("SRTM Folder:" + SRTMHeightProvider.SRTMDataPath);
            if (!Directory.Exists(SRTMHeightProvider.SRTMDataPath))
                Directory.CreateDirectory(SRTMHeightProvider.SRTMDataPath);
        }
        catch (DirectoryNotFoundException)
        {
            throw new DirectoryNotFoundException("Couldnt read/write SRTM Path!");
        }

        string continent = "";


        if (GetChunkFileContinentFolder(longIndex, latIndex, out continent))
        {

            string chunkFileName = GetChunkFileName(longIndex, latIndex);
            string chunkFilePath = SRTMHeightProvider.SRTMDataPath + "/" + continent + "/" + chunkFileName;
            string chunkFileNameUnzipped = chunkFileName.Substring(0, chunkFileName.Length - 4);
            string chunkFilePathUnzipped = SRTMHeightProvider.SRTMDataPath + "/" + continent + "/" + chunkFileNameUnzipped;

            string zippedSRTMFile = chunkFilePath;

            if (!File.Exists(zippedSRTMFile))
            {
                if (!Directory.Exists(SRTMHeightProvider.SRTMDataPath + "/" + continent))
                {
                    Debug.Log("Folder Doesnt Exist: " + SRTMHeightProvider.SRTMDataPath + "/" + continent);
                    Directory.CreateDirectory(SRTMHeightProvider.SRTMDataPath + "/" + continent);
                }
                else
                {
                    Debug.Log("Folder Existed: " + SRTMHeightProvider.SRTMDataPath + "/" + continent);
                }
                string downloadURL = @"https://dds.cr.usgs.gov/srtm/version2_1/SRTM3/" + continent;
                //string downloadURL = @"F:\SRTM3\Eurasia\";

                if (!File.Exists(chunkFilePathUnzipped))
                {
                    if (!File.Exists(chunkFilePath))
                    {
                        Debug.Log("Download SRTMFIle " + downloadURL);
                        zippedSRTMFile = DownloadSRTMFile(downloadURL, chunkFileName, SRTMHeightProvider.SRTMDataPath + "/" + continent, false);
                    }
                        
                }
            }

            //this.ReadFromFile(sRTMFilename(latIndex, longIndex));
            this.ReadFromFile(zippedSRTMFile);
            //WriteMapToFile(SRTMHeightProvider.SRTMDataPath + "/" + continent + "/" + chunkFileName + ".txt", Data, 1201);
        }
        else
        {
            Debug.LogError("File is not Available");
            this.MakeOceanHeightCell();
        }
    }

    private void MakeOceanHeightCell()
    {
        for (int i = 0; i < 1201; i++)
        {
            for (int j = 0; j < 1201; j++)
            {
                Data[i, j] = 0;
            }
        }
    }

    private int latIndex = 0;
    private int longIndex = 0;

    public short[,] Data = new short[1201, 1201];

    public float GetHeight(double latitude, double longitude)
    {
        int indexX = (int)Math.Floor((latitude - (double)latIndex) * 1201);
        int indexY = (int)Math.Floor((longitude - (double)longIndex) * 1201);

        return (float)Data[indexX, indexY];
    }

    //public float GetInterpolatedHeight(double latitude, double longitude)
    //{
    //    int pixelIndexX = (int)Math.Floor((latitude - latIndex) * 1201);
    //    int pixelIndexY = (int)Math.Floor((longitude - longIndex) * 1201);

    //    float x = (float)(((latitude - latIndex) * 1201) % 1);
    //    float y = (float)(((longitude - longIndex) * 1201) % 1);

    //    float f00 = Data[pixelIndexX, pixelIndexY];
    //    float f01 = Data[pixelIndexX, pixelIndexY + 1];
    //    float f10 = Data[pixelIndexX + 1, pixelIndexY];
    //    float f11 = Data[pixelIndexX + 1, pixelIndexY + 1];

    //    //Bilinear Interpolation
    //    //return f00 * (1f - x) * (1f - y) + f10 * x * (1f - y) + f01 * (1 - x) * y + f11 * x * y;
    //    return BilinearInterpolation(f00, f01, f10, f11, x, y);
    //}

    //private float BilinearInterpolation(float f00, float f01, float f10, float f11, float u, float w)
    //{
    //    return f00 * (1 - u) * (1 - w) + f01 * (1 - u) * w + f10 * u * (1 - w) + f11 * u * w;
    //}

    public float GetInterpolatedHeight(double latitude, double longitude)
    {
        int pixelIndexX = (int)Math.Floor((latitude - latIndex) * 1200);
        int pixelIndexY = (int)Math.Floor((longitude - longIndex) * 1200);

        float xParam = (float)(((latitude - latIndex) * 1200) % 1);
        float yParam = (float)(((longitude - longIndex) * 1200) % 1);

        float[,] controlPoints = new float[4, 4];

        int counterX = 0;
        int counterY = 0;
        for (int x = pixelIndexX - 1; x <= pixelIndexX + 2; x++)
        {
            counterY = 0;
            for (int y = pixelIndexY - 1; y <= pixelIndexY + 2; y++)
            {
                if (x == -1)
                {

                    //       |
                    //     * * * *
                    //     * * * *
                    // ----*-*-*-*-----
                    //     X * * *
                    //       |
                    if (y == -1)
                    {
                        SRTMDataCell cell = SRTMHeightProvider.GetSRTMDataCell(latIndex - 1, longIndex - 1);
                        controlPoints[counterX, counterY] = cell.Data[1199, 1199];

                        if (xParam >= 0.5f || yParam >= 0.5f)
                            Debug.Log("Parameter: " + x + ", " + y);
                    }
                    //       |
                    //     X * * *
                    // ----*-*-*-*-----
                    //     * * * *
                    //     * * * *
                    //       |
                    else if (y == 1201)
                    {
                        SRTMDataCell cell = SRTMHeightProvider.GetSRTMDataCell(latIndex - 1, longIndex + 1);
                        controlPoints[counterX, counterY] = cell.Data[1199, 1];
                    }
                    //       |
                    //     X * * *
                    //     * * * *
                    // ----*-*-*-*-----
                    //     * * * *
                    //       |
                    else if (y == 1202)
                    {
                        SRTMDataCell cell = SRTMHeightProvider.GetSRTMDataCell(latIndex - 1, longIndex + 1);
                        controlPoints[counterX, counterY] = cell.Data[1199, 2];
                    }
                         //  |
                         //X * **
                         //X * **
                         //X * **
                         //X * **
                         //  |
                    else
                    {
                        SRTMDataCell cell = SRTMHeightProvider.GetSRTMDataCell(latIndex - 1, longIndex);
                        controlPoints[counterX, counterY] = cell.Data[1199, y];
                    }
                }
                else if (x == 1201)
                {
                    //         |
                    //     * * * *
                    //     * * * *
                    // ----*-*-*-*-----
                    //     * * * X
                    //         |
                    if (y == -1)
                    {
                        SRTMDataCell cell = SRTMHeightProvider.GetSRTMDataCell(latIndex + 1, longIndex - 1);
                        controlPoints[counterX, counterY] = cell.Data[1, 1199];
                    }
                    //         |
                    //     * * * x
                    // ----*-*-*-*-----
                    //     * * * *
                    //     * * * *
                    //         |
                    else if (y == 1201)
                    {
                        SRTMDataCell cell = SRTMHeightProvider.GetSRTMDataCell(latIndex + 1, longIndex + 1);
                        controlPoints[counterX, counterY] = cell.Data[1, 1];
                    }
                    ////         |
                    ////     * * * x
                    ////     * * * *
                    //// ----*-*-*-*-----
                    ////     * * * *
                    ////         |
                    //else if (y == 1202)
                    //{
                    //    SRTMDataCell cell = SRTMHeightProvider.GetSRTMDataCell(latIndex + 1, longIndex + 1);
                    //    controlPoints[counterX, counterY] = cell.Data[1, 2];
                    //}
                    //         |
                    //     * * * x
                    //     * * * x
                    //     * * * x
                    //     * * * x
                    //         |
                    else
                    {
                        SRTMDataCell cell = SRTMHeightProvider.GetSRTMDataCell(latIndex + 1, longIndex);
                        controlPoints[counterX, counterY] = Data[1, y];
                    }
                }
                //else if (x == 1202)
                //{
                //    //       |
                //    //     * * * *
                //    //     * * * *
                //    // ----*-*-*-*-----
                //    //     * * * X
                //    //       |
                //    if (y == -1)
                //    {
                //        SRTMDataCell cell = SRTMHeightProvider.GetSRTMDataCell(latIndex + 1, longIndex - 1);
                //        controlPoints[counterX, counterY] = cell.Data[2, 1199];
                //    }
                //    //       |
                //    //     * * * x
                //    // ----*-*-*-*-----
                //    //     * * * *
                //    //     * * * *
                //    //       |
                //    else if (y == 1201)
                //    {
                //        SRTMDataCell cell = SRTMHeightProvider.GetSRTMDataCell(latIndex + 1, longIndex + 1);
                //        controlPoints[counterX, counterY] = cell.Data[2, 1];
                //    }
                //    //       |
                //    //     * * * x
                //    //     * * * *
                //    // ----*-*-*-*-----
                //    //     * * * *
                //    //       |
                //    else if (y == 1202)
                //    {
                //        SRTMDataCell cell = SRTMHeightProvider.GetSRTMDataCell(latIndex + 1, longIndex + 1);
                //        controlPoints[counterX, counterY] = cell.Data[2, 2];
                //    }
                //    //       |
                //    //     * * * x
                //    //     * * * x
                //    //     * * * x
                //    //     * * * x
                //    //       |
                //    else
                //    {
                //        SRTMDataCell cell = SRTMHeightProvider.GetSRTMDataCell(latIndex + 1, longIndex);
                //        controlPoints[counterX, counterY] = Data[2, y];
                //    }
                //}
                else
                {
                    //         
                    //     * * * *
                    //     * * * *
                    // ----*-*-*-*-----
                    //     X X X X
                    //         
                    if (y == -1)
                    {
                        SRTMDataCell cell = SRTMHeightProvider.GetSRTMDataCell(latIndex, longIndex - 1);
                        controlPoints[counterX, counterY] = cell.Data[x, 1199];
                    }
                    //         
                    //     X X X X
                    // ----*-*-*-*-----
                    //     * * * *
                    //     * * * *
                    //         
                    else if (y == 1201)
                    {
                        SRTMDataCell cell = SRTMHeightProvider.GetSRTMDataCell(latIndex, longIndex + 1);
                        controlPoints[counterX, counterY] = cell.Data[x, 1];
                    }
                    ////         
                    ////     X X X X
                    ////     * * * *
                    //// ----*-*-*-*-----
                    ////     * * * *
                    ////         
                    //else if (y == 1202)
                    //{
                    //    SRTMDataCell cell = SRTMHeightProvider.GetSRTMDataCell(latIndex, longIndex + 1);
                    //    controlPoints[counterX, counterY] = cell.Data[x, 1];
                    //}
                    //         
                    //     X X X X
                    //     X X X X
                    //     X X X X
                    //     X X X X
                    //         
                    else
                    {
                        controlPoints[counterX, counterY] = Data[x, y];
                    }
                }
                counterY++;
            }
            counterX++;
        }



        //Bilinear Interpolation
        //return f00 * (1f - x) * (1f - y) + f10 * x * (1f - y) + f01 * (1 - x) * y + f11 * x * y;
        return CardinalSplinePatchPoint(controlPoints, xParam, yParam);
    }

    public static float CardinalSplinePoint(float P1, float P2, float P3, float P4, float t, float s)
    {
        float tPow3 = Mathf.Pow(t, 3);
        float tPow2 = Mathf.Pow(t, 2);

        float twoTimesTPow2 = 2 * tPow2;
        float twoTimesTPow3 = 2 * tPow3;

        float result =
            s * (-tPow3 + twoTimesTPow2 - t) * P1 +
            s * (-tPow3 + tPow2) * P2 + (twoTimesTPow3 - 3 * tPow2 + 1) * P2 +
            s * (tPow3 - twoTimesTPow2 + t) * P3 + (-twoTimesTPow3 + 3 * tPow2) * P3 +
            s * (tPow3 - tPow2) * P4;

        return result;
    }

    private float CardinalSplinePatchPoint(float[,] controlPoints, float x, float y)
    {

        float P3 = CardinalSplinePoint(controlPoints[3, 0], controlPoints[3, 1], controlPoints[3, 2], controlPoints[3, 3], y, 0.5f);
        float P2 = CardinalSplinePoint(controlPoints[2, 0], controlPoints[2, 1], controlPoints[2, 2], controlPoints[2, 3], y, 0.5f);
        float P1 = CardinalSplinePoint(controlPoints[1, 0], controlPoints[1, 1], controlPoints[1, 2], controlPoints[1, 3], y, 0.5f);
        float P0 = CardinalSplinePoint(controlPoints[0, 0], controlPoints[0, 1], controlPoints[0, 2], controlPoints[0, 3], y, 0.5f);

        float result = CardinalSplinePoint(P0, P1, P2, P3, x, 0.5f);

        return result;
    }


    public short max;
    public int average;
    public void ReadFromFile(string sRTMFilePath)
    {

        ZipFile zip;
        try
        {
            zip = ZipFile.Read(sRTMFilePath);
        }
        catch (Exception ex)
        {
            Console.AddMessage("The requested SRTM File doesnt seem to exist at the specified path! /n Initialized the SRTMData with one-heightmap! \n" + ex.Message);
            for (int i = 0; i < 1201; i++)
            {
                for (int j = 0; j < 1201; j++)
                {
                    Data[i, j] = 0;
                }
            }
            return;
        }

        ZipEntry e = zip[0];
        Stream s = e.OpenReader();

        //short max = 0;
        max = 0;
        int[] pos = new int[2];

        for (int i = 0; i < 1201; i++)
        {
            for (int j = 0; j < 1201; j++)
            {
                byte[] buffer = new byte[2];

                s.Read(buffer, 1, 1);
                s.Read(buffer, 0, 1);

                Data[i, j] = BitConverter.ToInt16(buffer, 0);
                if (Data[i, j] == -32768)
                    Data[i, j] = 0;

                average += Data[i, j];
                if (Data[i, j] > max)
                {
                    max = Data[i, j];
                    pos[0] = i;
                    pos[1] = j;
                }
            }
        }
        average = average / (1201 * 1201);


        // TODO: really necessary???  mirroring the values
        int i2 = 1201;
        short[,] map2 = new short[1201, 1201];

        for (var i = 0; i < 1201; i++)
        {
            i2--;
            for (var j = 0; j < 1201; j++)
            {
                map2[i, j] = Data[i2, j];
            }
        }

        Data = map2;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="longitude"></param>
    /// <param name="latitude"></param>
    /// <returns></returns>
    public static string GetChunkFileName(int longitude, int latitude)
    {
        // TODO: aufräumen
        StringBuilder fileName = new StringBuilder();
        if (latitude > 0)
            fileName.Append('N');
        else
            fileName.Append('S');
        fileName.Append(String.Format("{0:00}", Math.Abs(latitude)));

        if (longitude > 0)
            fileName.Append('E');
        else
            fileName.Append('W');
        fileName.Append(String.Format("{0:000}", Math.Abs(longitude)));
        fileName.Append(".hgt.zip");

        return fileName.ToString();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="longitude"></param>
    /// <param name="latitude"></param>
    /// <param name="continent"></param>
    /// <returns></returns>
    public static bool GetChunkFileContinentFolder(int longitude, int latitude, out string continent)
    {
        string filename = GetChunkFileName(longitude, latitude);
        Debug.Log("Filename: " + filename);
        continent = "";

        if (CheckFileExists("https://dds.cr.usgs.gov/srtm/version2_1/SRTM3/Africa/" + filename))
            continent = "Africa";
        else if (CheckFileExists("https://dds.cr.usgs.gov/srtm/version2_1/SRTM3/Australia/" + filename))
            continent = "Australia";
        else if (CheckFileExists("https://dds.cr.usgs.gov/srtm/version2_1/SRTM3/Eurasia/" + filename))
            continent = "Eurasia";
        else if (CheckFileExists("https://dds.cr.usgs.gov/srtm/version2_1/SRTM3/Islands/" + filename))
            continent = "Islands";
        else if (CheckFileExists("https://dds.cr.usgs.gov/srtm/version2_1/SRTM3/North_America/" + filename))
            continent = "North_America";
        else if (CheckFileExists("https://dds.cr.usgs.gov/srtm/version2_1/SRTM3/South_America/" + filename))
            continent = "South_America";
        else
        {
            continent = "Doesn't exist";
            return false;
        }
        Debug.Log("Kontinent: " + continent);
        return true;
    }

    public static bool CheckFileExists(string url)
    {
        // Start a download of the given URL
        WWW www = new WWW(url);

        while (!www.isDone)
        {
            if (www.error != null)
            {
                //Debug.LogError(www.error);
                return false;
            }
        }
       
        return true;
    }

    //public static bool URLExists(string url)
    //{
    //    bool result = true;

    //    WebRequest webRequest = WebRequest.Create(url);
    //    webRequest.Timeout = 5000; // miliseconds
    //    webRequest.Method = "HEAD";

    //    try
    //    {
    //        webRequest.GetResponse();
    //    }
    //    catch
    //    {
    //        result = false;
    //    }

    //    return result;
    //}

    /// <summary>
    /// 
    /// </summary>
    /// <param name="downloadPath"></param>
    /// <returns></returns>
    public static string DownloadSRTMFile(string downloadPath, string downloadFilename, string srtmDataPath, bool reloadContent)
    {
        // TODO: Auf Settings in DataCell anpassen
        string tmpFolder = srtmDataPath;
        string tmpFile = tmpFolder + "/" + downloadFilename;
        Debug.Log(tmpFile);
        if (!downloadPath.StartsWith("http"))
        {
            if (!File.Exists(tmpFile))
            {
                File.Copy(downloadPath + downloadFilename, tmpFile);
            }
        }
        else
        {
            if (!File.Exists(tmpFile) || reloadContent)
            {
                // creating the file for the chunk
                FileStream file = File.Create(tmpFile);

                // request file from local path or server
                string url = downloadPath + "/" + downloadFilename;
                //Debug.Log(url);
               
                WWW www = new WWW(url);
                while (!www.isDone)
                {
                    if (www.error != null)
                        Debug.LogError(www.error);
                }

                file.Write(www.bytes, 0, www.bytes.Length);
                file.Close();
            }
        }
        return tmpFile;
    }


    public override string ToString()
    {
        return "SRTMDataCell[lat:" + this.latIndex + ", long:" + longIndex + "]" + "max=" + max + " average=" + average;
    }


    /// <summary>
    /// 
    /// </summary>
    public static void WriteMapToFile(string file, short[,] shortMap, int HeightmapResolution)
    {
        try
        {
            TextWriter writer = File.CreateText(file);
            int gesamt = HeightmapResolution * HeightmapResolution;
            float prozent = 1f / gesamt;
            float progressBar = 0.0f;

            for (int y = 0; y < HeightmapResolution; y++)
            {
                for (int x = 0; x < HeightmapResolution; x++)
                {

                    writer.Write(shortMap[x, y].ToString("000000", CultureInfo.CreateSpecificCulture("en-US")) + " ");

                    progressBar += prozent;

                }
#if UNITY_EDITOR
                UnityEditor.EditorUtility.DisplayProgressBar("Writing to File: " + file,
                        "writing",
                        progressBar);
#endif
                writer.WriteLine();
            }
            writer.Close();
#if UNITY_EDITOR
            UnityEditor.EditorUtility.ClearProgressBar();
#endif
        }
        catch
        {
            // TODO: good catch...  YEAH!!
        }

    }
}
