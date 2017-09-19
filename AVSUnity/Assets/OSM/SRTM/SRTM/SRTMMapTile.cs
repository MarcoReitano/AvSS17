using System.Globalization;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using System;
using System.Net;
using ICSharpCode.SharpZipLib.Zip;
using System.Collections.Generic;


public class SRTMMapTile : MonoBehaviour
{
    [SerializeField]
    public short[,] shortMap;

    [SerializeField]
    public short[,] floatMap;

    [SerializeField]
    public Vector3[,] verticesMap;

    [SerializeField]
    public int longitude = 7;

    [SerializeField]
    public int latitude = 51;

    [SerializeField]
    public bool reloadContent = false;
    private bool initialized = false;

    public MapBounds bounds;

    /// <summary>
    /// 
    /// </summary>
    public SRTMMapTile()
    {

    }


    public void ParseToCutMap(MapBounds bounds)
    {
        this.bounds = bounds;
        SRTMParser parser = new SRTMParser();

        parser.Bounds = this.bounds;
        SRTMMapOld map = parser.Parse();
        //map.printMapToFile(@"E://cutmap.txt", true);

        this.verticesMap = GenerateVertexMap(SRTMMapOld.ConvertJaggedArrayToFloatArray(map.floatMap));

        Debug.Log(verticesMap.GetLength(0) + "  " + verticesMap.GetLength(1));

        //for (int x = 0; x < verticesMap.GetLength(0); x++)
        //    for (int y = 0; y < verticesMap.GetLength(1); y++)
        //        Debug.Log(verticesMap[x, y]);

        CatmullRomSurfaceBehaviour catmullRomSurface = this.gameObject.AddComponent<CatmullRomSurfaceBehaviour>();
        catmullRomSurface.NumberOfControlPointsWidth = verticesMap.GetLength(0);
        catmullRomSurface.NumberOfControlPointsHeight = verticesMap.GetLength(1);
        catmullRomSurface.ControlPoints = new Vector3Grid(verticesMap.GetLength(0), verticesMap.GetLength(1), verticesMap);
       
        //LinkedList<Vector3> list = new LinkedList<Vector3>();
        


        initialized = true;
    }



    public bool isFileAvailable()
    {
        string downloadURL = @"http://dds.cr.usgs.gov/srtm/version2_1/SRTM3/Eurasia/";
        //string downloadURL = @"F:\SRTM3\Eurasia\";
        string chunkFileName = GetChunkFileName(longitude, latitude);
        //Debug.Log(downloadURL + chunkFileName + "  " + File.Exists(downloadURL + chunkFileName));
        return File.Exists(downloadURL + chunkFileName);
    }

    public void InitializeTile()
    {
        string downloadURL = @"http://dds.cr.usgs.gov/srtm/version2_1/SRTM3/Eurasia/";
        //string downloadURL = @"F:\SRTM3\Eurasia\";

        string chunkFileName = GetChunkFileName(longitude, latitude);
        string chunkFileNameUnzipped = chunkFileName.Substring(0, chunkFileName.Length - 4);

        string tmpFolder = EditorApplication.applicationContentsPath + @"/Temp/SRTM_TMP/";


        string zippedSRTMFile;
        string unzippedSRTMFile;

        if (!File.Exists(tmpFolder + chunkFileNameUnzipped))
        {
            if (!File.Exists(tmpFolder + chunkFileName))
                zippedSRTMFile = DownloadSRTMFile(downloadURL, chunkFileName);
            else
                zippedSRTMFile = tmpFolder + chunkFileName;

            unzippedSRTMFile = UnzipFile(zippedSRTMFile);
        }
        else
            unzippedSRTMFile = tmpFolder + chunkFileNameUnzipped;


        this.shortMap = ParseSRTMFile(unzippedSRTMFile);

        this.verticesMap = GenerateVertexMap(this.shortMap);

        //this.transform.position = this.verticesMap[0, 0];
        this.transform.position = Vector3.zero;

        //CatmullRomSurfaceBehaviour catmullRomSurface = this.gameObject.AddComponent<CatmullRomSurfaceBehaviour>();
        //catmullRomSurface.NumberOfControlPointsWidth = verticesMap.GetLength(0);
        //catmullRomSurface.NumberOfControlPointsHeight = verticesMap.GetLength(1);
        //catmullRomSurface.ControlPoints = new Vector3Grid(verticesMap.GetLength(0), verticesMap.GetLength(1), verticesMap);

        CatmullRomSurfaceBehaviour catmullRomSurface = this.gameObject.AddComponent<CatmullRomSurfaceBehaviour>();
        catmullRomSurface.NumberOfControlPointsWidth = 4;
        catmullRomSurface.NumberOfControlPointsHeight = 4;
        catmullRomSurface.ControlPoints = new Vector3Grid(verticesMap.GetLength(0), verticesMap.GetLength(1), verticesMap);

        this.initialized = true;
    }



    public Vector3[,] GenerateVertexMap(short[,] p)
    {
        Vector3[,] verticesMap = new Vector3[1201, 1201];


        double step = GeographicCoordinates.arcsec3Meter;

        for (int i = 0; i < 1201; i++)
        {
            for (int j = 0; j < 1201; j++)
            {
                Vector3 vertex = new Vector3();

                //Vector2 position = MapBounds.ConvertLonLatToXY(
                //    (float)longitude + j * step,
                //    (float)latitude + i * step, latitude);
                ////(float)longitude + MapBounds.arcsec3 * 1201 / 2);

                //Vector2 position = GeographicCoordinates.ConvertLonLatToXY(
                //    (float)longitude + j * step,
                //    (float)latitude + i * step,
                //    GeographicCoordinates.MapCenter);

                Vector2 position = new Vector2((float)(j * step), (float)(i * step));

                //(float)longitude + MapBounds.arcsec3 * 1201 / 2);

                vertex.x = position.x;
                vertex.y = (float) shortMap[i, j];
                vertex.z = position.y;


                verticesMap[i, j] = vertex;
            }
        }

        return verticesMap;
    }


    public Vector3[,] GenerateVertexMap(float[,] p)
    {
        Vector3[,] verticesMap = new Vector3[p.GetLength(0), p.GetLength(1)];

        Debug.Log(" BoundsControll.. " + this.bounds);
        //Debug.Log(" MapBounds.CenterOffset.. " + GeographicCoordinates.CenterOffset);
        //this.longitude = bounds.West;
        //this.latitude = bounds.South;

        double step = GeographicCoordinates.arcsec3Meter;

        for (int i = 0; i < p.GetLength(0); i++)
        {
            for (int j = 0; j < p.GetLength(1); j++)
            {
                Vector3 vertex = new Vector3();

                //Vector2 position = MapBounds.ConvertLonLatToXY(
                //    (float)longitude + j * step,
                //    (float)latitude + i * step, latitude);
                ////(float)longitude + MapBounds.arcsec3 * 1201 / 2);

                //Vector2 position = GeographicCoordinates.ConvertLonLatToXY(
                //    (float)bounds.West + j * step,
                //    (float)bounds.South + i * step,
                //    GeographicCoordinates.MapCenter);
                //////(float)longitude + MapBounds.arcsec3 * 1201 / 2);
                
                Vector2 position = new Vector2((float)(j * step), (float)(i * step));

                //vertex.x = position.x - MapBounds.CenterOffset.x;
                //vertex.y = (float)p[i, j];
                //vertex.z = position.y - MapBounds.CenterOffset.z;
                //vertex -= MapBounds.CenterOffset;

                verticesMap[i, j] = vertex;
            }
        }

        return verticesMap;
    }



    /// <summary>
    /// 
    /// </summary>
    /// <param name="longitude"></param>
    /// <param name="latitude"></param>
    /// <returns></returns>
    private static string GetChunkFileName(int longitude, int latitude)
    {
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



    public void CheckFolders()
    {
        string myTMP = EditorApplication.applicationContentsPath + @"/Temp";

        if (!Directory.Exists(myTMP))
            Directory.CreateDirectory(myTMP);

        if (!Directory.Exists(myTMP + @"/SRTM_TMP"))
        {
            Debug.Log("SRTM-Temp-Folder does not exist... creating it...");
            Directory.CreateDirectory(myTMP + @"/SRTM_TMP");
            Debug.Log("SRTM-Temp-Folder created... " + myTMP);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    private string DownloadSRTMFile(string path, string filename)
    {
        // TODO: Auf Settings in DataCell anpassen
        string tmpFolder = EditorApplication.applicationContentsPath + @"/Temp/SRTM_TMP/";
        string tmpFile = tmpFolder + filename;

        if (!path.StartsWith("http"))
        {
            if (!File.Exists(tmpFile))
            {
                File.Copy(path + filename, tmpFile);
            }
        }
        else
        {
            if (!File.Exists(tmpFile) || reloadContent)
            {

                // creating the file for the chunk
                FileStream file = File.Create(tmpFile);

                int buffersize = 1048576; // TODO: Buffersize selectable 524288 //16384;
                byte[] buffer = new byte[buffersize];
                int len;

                // request file from local path or server
                string url = path + "/" + filename;
                Debug.Log(url);
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(path + filename);
                // get response of request
                HttpWebResponse res = (HttpWebResponse)req.GetResponse();
                Stream stream = res.GetResponseStream();

                // reading SRTMChunk from stream into file
                do
                {
                    len = stream.Read(buffer, 0, buffersize);
                    if (len < 1) break;
                    file.Write(buffer, 0, len);
                } while (len > 0);
                file.Close();
            }
        }
        return tmpFile;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="downloadedFile"></param>
    /// <returns></returns>
    private string UnzipFile(string downloadedFile)
    {
        // open a temporary file
        string tmpFile = downloadedFile.Substring(0, downloadedFile.Length - 4);//Path.GetTempFileName();
        FileStream outStream = File.Create(tmpFile);

        // unzipping the downloaded file
        using (ZipInputStream s = new ZipInputStream(File.OpenRead(downloadedFile)))
        {
            try
            {
                ZipEntry theEntry;
                while ((theEntry = s.GetNextEntry()) != null)
                {
                    // writing unzipped stream-data into outfile-stream
                    using (FileStream streamWriter = outStream)
                    {
                        int size = 2048;
                        byte[] data = new byte[2048];
                        while (true)
                        {
                            size = s.Read(data, 0, data.Length);
                            if (size > 0)
                                streamWriter.Write(data, 0, size);
                            else
                                break;
                        }
                    }
                }
            }
            catch (ZipException e)
            {
                Debug.Log("Caught ZIP-Exception...");
            }
            finally
            {
                outStream.Close();
            }
        }
        // closing the stream, thus writing the outfile
        outStream.Close();
        return tmpFile;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public short[,] ParseSRTMFile(string file)
    {
        byte[] bytes = new byte[1];

        // unzip the file
        string strTempFile = file; // UnzipFile(file);

        // read Values from file into byte-Array
        bytes = File.ReadAllBytes(strTempFile);
        //File.Delete(strTempFile);

        // swap byteorder from big-endian to little-endian
        ConvertBigEndianToLittleEndian(ref bytes);

        // the height and width of one chunk
        int height = 1201;
        int width = 1201;

        // create short-array and transfer values
        short[,] map = new short[height, width];

        for (var i = 0; i < height; i++)
        {
            for (var j = 0; j < width; j++)
            {
                map[i, j] = BitConverter.ToInt16(bytes, i * width * 2 + j * 2);
            }
        }

        // TODO: really necessary???  mirroring the values
        int i2 = 1201;
        short[,] map2 = new short[height, width];

        for (var i = 0; i < height; i++)
        {
            i2--;
            for (var j = 0; j < width; j++)
            {
                map2[i, j] = map[i2, j];
                //Debug.Log(map2[i, j]);
            }
        }
        Debug.Log("Done Parsing");
        return map2;
    }



    /// <summary>
    /// 
    /// </summary>
    /// <param name="bytes"></param>
    private static void ConvertBigEndianToLittleEndian(ref byte[] bytes)
    {
        for (int i = 0; i < bytes.Length; i += 2)
        {
            // swaping byte-order
            byte tmp = bytes[i];
            bytes[i] = bytes[i + 1];
            bytes[i + 1] = tmp;
        }
    }


    public static short[][] ConvertShortArrayToJaggedArray(short[,] shortArray)
    {

        short[][] jaggedArray = new short[shortArray.GetLength(0)][];
        for (int y = 0; y < shortArray.GetLength(0); y++)
        {
            jaggedArray[y] = new short[shortArray.GetLength(1)];
            for (int x = 0; x < shortArray.GetLength(1); x++)
                jaggedArray[y][x] = shortArray[y, x];
        }

        return jaggedArray;
    }

    public static short[,] ConvertJaggedArrayToShortArray(short[][] jaggedArray)
    {

        short[,] shortArray = new short[jaggedArray.GetLength(0), jaggedArray.GetLength(1)];
        for (int y = 0; y < jaggedArray.GetLength(0); y++)
            for (int x = 0; x < jaggedArray.GetLength(1); x++)
                shortArray[y, x] = jaggedArray[y][x];

        return shortArray;
    }


    public void OnDrawGizmos()
    {
        //if (initialized)
        //    DrawControlGrid();
    }


    public void DrawControlGrid()
    {
        if (verticesMap != null)
        {
            Gizmos.color = Color.yellow;
            for (int x = 0; x < verticesMap.GetLength(0); x += 10)
                for (int y = 0; y < verticesMap.GetLength(1) - 10; y += 10)
                    Gizmos.DrawLine(this.verticesMap[x, y], this.verticesMap[x, y + 10]);

            for (int x = 0; x < verticesMap.GetLength(0) - 10; x += 10)
                for (int y = 0; y < verticesMap.GetLength(1); y += 10)
                    Gizmos.DrawLine(this.verticesMap[x, y], this.verticesMap[x + 10, y]);
        }
    }


    void OnDrawGizmosSelected()
    {

    }

}

