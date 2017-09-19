using UnityEngine;
#if UNITY_EDITOR
//using UnityEditor;
#endif
using System.Collections;
using System.IO;
using Ionic.Zip;
using System;

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

        this.ReadFromFile(sRTMFilename(latIndex, longIndex));
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

    public float GetInterpolatedHeight(double latitude, double longitude)
    {
        int indexX = (int)Math.Floor((latitude - (double)latIndex) * 1201);
        int indexY = (int)Math.Floor((longitude - (double)longIndex) * 1201);

        float x = (float)((latitude - (double)latIndex) * 1201d) % 1;
        float y = (float)((longitude - (double)longIndex) * 1201d) % 1;

        if (indexX >= 1200 || indexY >= 1200)
            return 0f;

        float f00 = (float)Data[indexX, indexY];
        float f10 = (float)Data[indexX + 1, indexY];
        float f01 = (float)Data[indexX, indexY + 1];
        float f11 = (float)Data[indexX + 1, indexY + 1];

        //Bilinear Interpolation
        return f00 * (1f - x) * (1f - y) + f10 * x * (1f - y) + f01 * (1 - x) * y + f11 * x * y;
    }

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

        short max = 0;
        int[] pos = new int[2];

        for (int i = 0; i < 1201; i++)
        {
            for (int j = 0; j < 1201; j++)
            {
                byte[] buffer = new byte[2];

                s.Read(buffer, 1, 1);
                s.Read(buffer, 0, 1);

                Data[i, j] = BitConverter.ToInt16(buffer, 0);
                if (Data[i, j] > max)
                {
                    max = Data[i, j];
                    pos[0] = i;
                    pos[1] = j;
                }
            }
        }


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

    private string sRTMFilename(int latIndex, int longIndex)
    {
        //Debug.Log("DataCell loaded: " + latIndex + " " + longIndex);
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        if (latIndex > 0)
            sb.Append("N");
        else
            sb.Append("S");

        int abs = System.Math.Abs(latIndex);

        sb.Append(abs.ToString("00"));

        if (longIndex < 0)
            sb.Append("W");
        else
            sb.Append("E");
        abs = System.Math.Abs(longIndex);

        sb.Append(abs.ToString("000"));

        return SRTMHeightProvider.SRTMDataPath + "/" + sb + ".hgt.zip";
    }
}
