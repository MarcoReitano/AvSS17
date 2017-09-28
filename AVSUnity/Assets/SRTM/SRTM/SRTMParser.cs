//using System;
//using System.IO;
//using System.Net;
//using System.Text;
//using ICSharpCode.SharpZipLib.Zip;
//using UnityEngine;
//using UnityEditor;


///// <summary>
///// http://dds.cr.usgs.gov/srtm/version2_1/Documentation/
///// http://dds.cr.usgs.gov/srtm/version2_1/SRTM3/Eurasia/
///// </summary>
//public class SRTMParser
//{
//    private bool _DEBUG = true;

//    public volatile float progressBar;
//    public volatile bool isRunning;

//    private string editorpath;
//    public string Editorpath
//    {
//        get { return editorpath; }
//        set { editorpath = value; }
//    }

//    // TODO: Check if volatile necessary
//    private OSMBoundingBox bounds;
//    public OSMBoundingBox Bounds
//    {
//        get { return bounds; }
//        set { bounds = value; }
//    }

//    private bool reloadContent;
//    public bool ReloadContent
//    {
//        get { return reloadContent; }
//        set { reloadContent = value; }
//    }


//    private string sitePath = @"http://dds.cr.usgs.gov/srtm/version2_1/SRTM3/Eurasia/";
//    public string SitePath
//    {
//        get { return sitePath; }
//        set { sitePath = value; }
//    }


//    public float[][] floatMap;
//    public bool _finished = false;


//    /// <summary>
//    /// 
//    /// </summary>
//    public SRTMParser()
//    {

//    }

//    //public SRTMMapOld Parse()
//    //{
//    //    SRTMMapOld srtmMap = new SRTMMapOld();
//    //    srtmMap.Bounds = this.bounds;
//    //    srtmMap.InitialFloatmap = ParseToFloatMap();
//    //    srtmMap.CalculateSize();
//    //    return srtmMap;
//    //}


//    /// <summary>
//    /// 
//    /// </summary>
//    /// <param name="y"></param>
//    /// <param name="x"></param>
//    /// <returns></returns>
//    private static string GetChunkFileName(int y, int x)
//    {
//        StringBuilder fileName = new StringBuilder();
//        if (y > 0)
//            fileName.Append('N');
//        else
//            fileName.Append('S');
//        fileName.Append(String.Format("{0:00}", Math.Abs(y)));

//        if (x > 0)
//            fileName.Append('E');
//        else
//            fileName.Append('W');
//        fileName.Append(String.Format("{0:000}", Math.Abs(x)));

//        fileName.Append(".hgt.zip");

//        return fileName.ToString();
//    }


//    /// <summary>
//    /// 
//    /// </summary>
//    /// <param name="path"></param>
//    /// <returns></returns>
//    private string DownloadSRTMFile(string path, string filename)
//    {
//        string tmpFolder = EditorApplication.applicationContentsPath + @"/Temp/SRTM_TMP/";
//        string tmpFile = tmpFolder + filename;

//        if (!path.StartsWith("http"))
//        {
//            if (!File.Exists(tmpFile))
//            {
//                File.Copy(path + filename, tmpFile);
//            }
//        }
//        else
//        {
//            if (!File.Exists(tmpFile) || reloadContent)
//            {

//                // creating the file for the chunk
//                FileStream file = File.Create(tmpFile);

//                int buffersize = 1048576; // TODO: Buffersize selectable 524288 //16384;
//                byte[] buffer = new byte[buffersize];
//                int len;

//                // request file from local path or server
//                string url = path + "/" + filename;
//                Debug.Log(url);
//                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(path + filename);
//                // get response of request
//                HttpWebResponse res = (HttpWebResponse)req.GetResponse();
//                Stream stream = res.GetResponseStream();

//                // reading SRTMChunk from stream into file
//                do
//                {
//                    len = stream.Read(buffer, 0, buffersize);
//                    if (len < 1) break;
//                    file.Write(buffer, 0, len);
//                } while (len > 0);
//                file.Close();
//            }
//        }
//        return tmpFile;
//    }


//    /// <summary>
//    /// 
//    /// </summary>
//    /// <param name="downloadedFile"></param>
//    /// <returns></returns>
//    private string UnzipFile(string downloadedFile)
//    {
//        // open a temporary file
//        string tmpFile = Path.GetTempFileName();
//        FileStream outStream = File.Create(tmpFile);

//        // unzipping the downloaded file
//        using (ZipInputStream s = new ZipInputStream(File.OpenRead(downloadedFile)))
//        {
//            //ZipEntry theEntry;
//            //while ((theEntry = s.GetNextEntry()) != null)
//            while ((s.GetNextEntry()) != null)
//            {
//                // writing unzipped stream-data into outfile-stream
//                using (FileStream streamWriter = outStream)
//                {
//                    int size = 2048;
//                    byte[] data = new byte[2048];
//                    while (true)
//                    {
//                        size = s.Read(data, 0, data.Length);
//                        if (size > 0)
//                            streamWriter.Write(data, 0, size);
//                        else
//                            break;
//                    }
//                }
//            }
//        }
//        // closing the stream, thus writing the outfile
//        outStream.Close();
//        return tmpFile;
//    }


//    /// <summary>
//    /// 
//    /// </summary>
//    /// <returns></returns>
//    public short[,] ParseSRTMFile(string file)
//    {
//        byte[] bytes = new byte[1];

//        // unzip the file
//        string strTempFile = UnzipFile(file);

//        // read Values from file into byte-Array
//        bytes = File.ReadAllBytes(strTempFile);
//        File.Delete(strTempFile);

//        // swap byteorder from big-endian to little-endian
//        ConvertBigEndianToLittleEndian(ref bytes);

//        // the height and width of one chunk
//        int height = 1201;
//        int width = 1201;

//        // create short-array and transfer values
//        short[,] map = new short[height, width];

//        for (var i = 0; i < height; i++)
//        {
//            for (var j = 0; j < width; j++)
//            {
//                map[i, j] = BitConverter.ToInt16(bytes, i * width * 2 + j * 2);
//            }
//        }

//        // TODO: really necessary???  mirroring the values
//        int i2 = 1201;
//        short[,] map2 = new short[height, width];

//        for (var i = 0; i < height; i++)
//        {
//            i2--;
//            for (var j = 0; j < width; j++)
//            {
//                map2[i, j] = map[i2, j];
//            }
//        }

//        return map2;
//    }



//    /// <summary>
//    /// 
//    /// </summary>
//    /// <param name="bytes"></param>
//    private static void ConvertBigEndianToLittleEndian(ref byte[] bytes)
//    {
//        for (int i = 0; i < bytes.Length; i += 2)
//        {
//            // swaping byte-order
//            byte tmp = bytes[i];
//            bytes[i] = bytes[i + 1];
//            bytes[i + 1] = tmp;
//        }
//    }



//    private void Print(string logmessage)
//    {
//        if (_DEBUG)
//        {
//            Debug.Log(logmessage);
//        }
//    }


//    /// <summary>
//    /// 
//    /// </summary>
//    public float[][] ParseToFloatMap()
//    {

//        isRunning = true;
//        _finished = false;

//        // TODO: Eurasia --> Ordnerzuweisung nach http://dds.cr.usgs.gov/srtm/version2_1/Documentation/Continent_def.gif

//        int northSouthDistance = (int)Math.Floor(bounds.North) - (int)Math.Floor(bounds.South) + 1;
//        int eastWestDistance = (int)Math.Floor(bounds.East) - (int)Math.Floor(bounds.West) + 1;

//        Print("northSouthDistance" + northSouthDistance);
//        Print("eastWestDistance" + eastWestDistance);

//        //int gesamt = northSouthDistance * eastWestDistance;
//        //float prozent = 1 / gesamt;
//        //this.progressBar = 0.0f;
//        //this.progressBar += prozent;


//        Print("chunkMap zum einlesen der einzelnen Platten initialisieren");
//        // chunkMap zum einlesen der einzelnen Platten initialisieren
//        short[,][,] chunkMap = new short[northSouthDistance, eastWestDistance][,];
//        for (int y = 0; y < northSouthDistance; y++)
//            for (int x = 0; x < eastWestDistance; x++)
//                chunkMap[y, x] = new short[1200, 1200];


//        Print("parsen der einzelnen Platten");
//        // parsen der einzelnen Platten
//        int yPointer = 0;
//        for (int y = (int)Math.Floor(bounds.South); y <= (int)Math.Floor(bounds.North); y++)
//        {
//            int xPointer = 0;
//            for (int x = (int)Math.Floor(bounds.West); x <= (int)Math.Floor(bounds.East); x++)
//            {
//                string fileName = GetChunkFileName(y, x);
//                //CITY_Editor.progressDelegate("SRTM-Data", "Downloading & Parsing map-chunk... " + sb.ToString() + "  Please wait.", this.progressBar);
//                Print("Downloading & Parsing map-chunk... " + fileName);
//                string file = DownloadSRTMFile(sitePath, fileName);

//                chunkMap[yPointer, xPointer] = this.ParseSRTMFile(file);
//                //this.progressBar += prozent;
//                xPointer++;
//            }
//            yPointer++;
//        }

//        Print("übertragen in ein floatArray");
//        // übertragen in ein floatArray
//        float[,] tmpMap = new float[1201 * northSouthDistance, 1201 * eastWestDistance];

//        //this.progressBar = 0.0f;
//        //gesamt = (1200 * northSouthDistance);
//        //prozent = 1 / gesamt;

//        for (int i = 0; i < northSouthDistance; i++)
//        {
//            for (int y = 0; y < 1200; y++)
//            {
//                for (int j = 0; j < eastWestDistance; j++)
//                {
//                    for (int x = 0; x < 1200; x++)
//                    {
//                        tmpMap[i * 1200 + y, j * 1200 + x] = chunkMap[i, j][y, x];
//                        //Debug.Log("[" + i * 1200 + y + "," + j * 1200 + x + "]" + chunkMap[i, j][y, x]);
//                        //this.progressBar += prozent;
//                    }
//                }
//            }
//        }


//        Debug.Log("CutMap" + tmpMap.GetLength(0) + " " + tmpMap.GetLength(1));
//        this.floatMap = SRTMMapOld.ConvertFloatArrayToJaggedArray(CutMap(this.bounds, tmpMap));


//        isRunning = false;
//        _finished = true;


//        return this.floatMap;
//    }


//    /// <summary>
//    /// 
//    /// </summary>
//    /// <param name="bounds"></param>
//    /// <param name="floatMap"></param>
//    /// <returns></returns>
//    private float[,] CutMap(OSMBoundingBox bounds, float[,] floatMap)
//    {
//        // Abmessungen der benötigten map berechnen
//        // aus maxbounds die anzahl der steps in ns und ow richtung berechnen
//        // ns-Richtung
//        int northSouthDistance = (int)Math.Floor((bounds.North - bounds.South) / GeographicCoordinates.arcsec3);
//        int eastWestDistance = (int)Math.Floor((bounds.East - bounds.West) / GeographicCoordinates.arcsec3);
//        Print("northSouthDistance " + northSouthDistance);
//        Print("eastWestDistance " + eastWestDistance);


//        int step;
//        if (northSouthDistance > eastWestDistance)
//            step = northSouthDistance;
//        else
//            step = eastWestDistance;

//        float[,] cutMap = new float[step, step];

//        Print("1201 = " + Math.Ceiling(Math.Ceiling(bounds.North) / GeographicCoordinates.arcsec3));
//        Print("1201 = " + Math.Ceiling(Math.Floor(bounds.North) / GeographicCoordinates.arcsec3));

//        Print("Math.Ceiling(bounds.North) " + Math.Ceiling(bounds.North));
//        int startindexNorth = (int)(Math.Floor(bounds.South / GeographicCoordinates.arcsec3) - (int)(Math.Ceiling(Math.Floor(bounds.South) / GeographicCoordinates.arcsec3)));
//        int startindexEast = (int)(Math.Floor(bounds.West / GeographicCoordinates.arcsec3) - (int)(Math.Ceiling(Math.Floor(bounds.West) / GeographicCoordinates.arcsec3)));

//        Print("startindexNorth " + startindexNorth);
//        Print("startindexEast " + startindexEast);
//        Print("endindexNorth " + (startindexNorth + step));
//        Print("endindexEast " + (startindexEast + step));

//        Print("floatMap " + floatMap.GetLength(0));
//        Print("floatMap " + floatMap.GetLength(1));

//        int yPointer = 0;
//        int xPointer = 0;
//        for (int y = startindexNorth; y < startindexNorth + step; y++)
//        {
//            xPointer = 0;
//            for (int x = startindexEast; x < startindexEast + step; x++)
//            {

//                cutMap[xPointer, yPointer] = floatMap[y, x];
//                //Debug.Log("[" + y + "," + x + "] = " + floatMap[y, x] + " = " + cutMap[xPointer, yPointer]);
//                xPointer++;

//            }
//            yPointer++;
//        }


//        return cutMap;
//    }



//}

