using System;
using System.IO;
using System.Xml;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System.Diagnostics;

using Debug = UnityEngine.Debug;


public class OSMParser
{
    

    public volatile float progressBar;

    public volatile string lastMessage = string.Empty;

    public volatile bool isRunning;

    private OSMMap map;
    public OSMMap Map
    {
        get { return map; }
        set { map = value; }
    }

    private List<MapBounds> boundsList;
    public List<MapBounds> BoundsList
    {
        get { return boundsList; }
        set { boundsList = value; }
    }

    // The Filepath of the XML-OSMmap-file
    private string filePath;
    public String FilePath
    {
        get { return filePath; }
        set { this.filePath = value; }
    }

    public OSMSourceType sourceType = OSMSourceType.Server;

    // ServerURL
    private string serverURL;
    public String ServerURL
    {
        get { return serverURL; }
        set { this.serverURL = value; }
    }

    private string outfilePath;
    public string OutfilePath
    {
        get { return outfilePath; }
        set { outfilePath = value; }
    }

    private XmlTextReader xmlReader;
    public bool finished = false;


    /// <summary>
    /// The default Constructor
    /// </summary>
    public OSMParser()
    {
        map = new OSMMap();
        isRunning = false;
        finished = false;
    }


    /// <summary>
    /// The Constructor 
    /// </summary>
    /// <param name="strPath"></param>
    public OSMParser(string filePath, string outFilePath)
    {
        this.FilePath = filePath;
        this.OutfilePath = outFilePath;
        map = new OSMMap();
        isRunning = false;
        finished = false;
    }

    /// <summary>
    /// 
    /// </summary>
    public void StartParsing(bool reloadContent)
    {
        isRunning = true;
        finished = false;
        Stopwatch timer = new Stopwatch();
        timer.Start();


        string tmpFolder = EditorApplication.applicationContentsPath + @"/OSM_TMP/";
        string tmpFile = tmpFolder + MapBounds.MaxBounds(this.boundsList).GetHashCode();


        if (!File.Exists(tmpFile) || reloadContent)
        {
            if (!Directory.Exists(tmpFolder))
            {
                Debug.Log("!Directory.Exists(tmpFolder): " + !Directory.Exists(tmpFolder));
                string newPath = Path.Combine(EditorApplication.applicationContentsPath, "OSM_TMP");
                Directory.CreateDirectory(newPath);
            }

            this.Map.Box = MapBounds.MaxBounds(boundsList);
            Debug.Log("\n* this.Map.Box " + this.Map.Box);

            Debug.Log("\n* Calculating Chunksize... ");
            double north = boundsList[0].North;
            double south = boundsList[0].South;
            double west = boundsList[0].West;
            double east = boundsList[0].East;

            int xSteps = 0;
            int ySteps = 0;
            double chunkWidth = 0.0f;
            double chunkHeight = 0.0f;
            MapBounds.calculateChunksize(
                ref north, ref south,
                ref west, ref east,
                ref xSteps, ref ySteps,
                ref chunkWidth, ref chunkHeight);
            Debug.Log("done.");

            // statistics and estimation
            int gesamt = ySteps * xSteps;
            float prozent = 1f / gesamt;

            this.progressBar = 0.0f;

            Debug.Log("\n* Retrieving and Parsing Chunks...\n");
            int counter = 0;

            double yPointer = north;
            for (int y = 0; y < ySteps; y++)
            {
                double xPointer = west;
                for (int x = 0; x < xSteps; x++)
                {
                    this.sourceType = OSMSourceType.Server;
                    this.FilePath = this.serverURL +
                        xPointer.ToString().Replace(",", ".") + "," +
                        (yPointer - chunkHeight).ToString().Replace(",", ".") + "," +
                        (xPointer + chunkWidth).ToString().Replace(",", ".") + "," +
                        yPointer.ToString().Replace(",", ".");

                    Debug.Log("Chunk Bounds kontrolle: " + this.FilePath);
                    counter++;
                    //timer.NextLap();
                    //string estimatedTime = timer.formatTimeSpan(timer.estimateCompleteTime(counter, gesamt));

                    //progressBar = prozent * counter;
                    //lastMessage = "\t* Chunk " + counter + "/" + gesamt + " | " + (prozent * counter) + "% | estimated time left: " + estimatedTime.ToString();
                    //Debug.Log("\t* Chunk " + counter + "/" + gesamt + " | " + (prozent * counter) + "% | estimated time left: " + estimatedTime.ToString());
                    //Debug.Log("\t* map statistics: nodes=" + map.Nodes.Count + " ways=" + map.Ways.Count + " relations=" + map.Relations.Count);
                    //EditorUtility.DisplayProgressBar("OSM-Data", lastMessage, this.progressBar);

                    this.ParseOSM();
                    xPointer += chunkWidth;

                }
                yPointer -= chunkHeight;
            }
            Debug.Log("\r* Parsing completed...\n");
            EditorUtility.DisplayProgressBar("OSM-Data", "Writing to XML-File... ", this.progressBar);

            //this.WriteXMLFile(@"e:\dennis\XML-Chunks\Unity-Test.osm");
            this.WriteXMLFile(tmpFile);
            Debug.Log("done " + tmpFile + ".\n\n");

            Debug.Log("\n* ");
        }
        else
        {
            Debug.Log("Datei bereits vorhanden: " + tmpFile);
            this.FilePath = tmpFile;
            this.ParseOSM();
        }

        timer.Stop();
        isRunning = false;
        finished = true;
    }


    /// <summary>
    /// 
    /// </summary>
    public void StartParsingNoChunks(bool reloadContent)
    {
        isRunning = true;
        finished = false;
        Stopwatch timer = new Stopwatch();
        timer.Start();


        string tmpFolder = EditorApplication.applicationContentsPath + @"/OSM_TMP/";
        string tmpFile = tmpFolder + MapBounds.MaxBounds(this.boundsList).GetHashCode();


        if (!File.Exists(tmpFile) || reloadContent)
        {
            if (!Directory.Exists(tmpFolder))
            {
                Debug.Log("!Directory.Exists(tmpFolder): " + !Directory.Exists(tmpFolder));
                string newPath = Path.Combine(EditorApplication.applicationContentsPath, "OSM_TMP");
                Directory.CreateDirectory(newPath);
            }

            this.Map.Box = MapBounds.MaxBounds(boundsList);
            Debug.Log("\n* this.Map.Box " + this.Map.Box);

            Debug.Log("\n* Calculating Chunksize... ");
            double north = boundsList[0].North;
            double south = boundsList[0].South;
            double west = boundsList[0].West;
            double east = boundsList[0].East;


            // statistics and estimation


            int counter = 0;

            this.sourceType = OSMSourceType.Server;
            this.FilePath = this.serverURL +
                west.ToString().Replace(",", ".") + "," +
                south.ToString().Replace(",", ".") + "," +
                east.ToString().Replace(",", ".") + "," +
                north.ToString().Replace(",", ".");


            this.ParseOSM();

            Debug.Log("\r* Parsing completed...\n");
            EditorUtility.DisplayProgressBar("OSM-Data", "Writing to XML-File... ", this.progressBar);

            //this.WriteXMLFile(@"e:\dennis\XML-Chunks\Unity-Test.osm");
            this.WriteXMLFile(tmpFile);
            Debug.Log("done " + tmpFile + ".\n\n");

            Debug.Log("\n* ");
        }
        else
        {
            Debug.Log("Datei bereits vorhanden: " + tmpFile);
            this.FilePath = tmpFile;
            this.ParseOSM();
        }

        timer.Stop();
        isRunning = false;
        finished = true;
    }




    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public OSMMap ParseOSM()
    {
        xmlReader = new XmlTextReader(this.filePath);
        XmlDocument doc = new XmlDocument();

        while (xmlReader.Read())
        {
            if (xmlReader.NodeType == XmlNodeType.Element)
            {
                if (xmlReader.Name == "osm")
                {
                    OSMMap.ParseMap(doc, ref xmlReader, ref map);
                }
            }
        }
        return this.map;
    }


    public void WriteXMLFile(string pathAndFilename)
    {
        StreamWriter writer = File.CreateText(pathAndFilename);
        writer.Write(map.GetXMLString());
        writer.Close();
    }

    //public static C5.KeyValuePair<string, string> ReadKeyValue(XmlNode node)
    //{
    //    return new C5.KeyValuePair<string, string>(System.Security.SecurityElement.Escape(node.Attributes[0].Value), System.Security.SecurityElement.Escape(node.Attributes[1].Value));
    //}

}
