using UnityEngine;
using System.Threading;
using System.Globalization;


#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Xml;
using System.Xml.Serialization;
using System.Net;
using System;
using System.Text;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.IO;

/// <summary>
/// OverpassQuery.
/// </summary>
[System.Serializable]
public class OverpassQuery
{
	
    public OverpassQuery()
    {
		
        logPath = Application.dataPath + "/Logs/OSM.xml";
#if UNITY_STANDALONE
        if(!Directory.Exists(Application.dataPath + "/OSMQueries/"))
        {
            Directory.CreateDirectory(Application.dataPath + "/OSMQueries/");
        }
        oSMCachePath = Application.dataPath + "/OSMQueries/";
#endif
#if UNITY_EDITOR

        //oSMCachePath = "C:/Users/MReitano/Documents/DEV/Resources/OSMQueries/"; //Marco Büro
        //oSMCachePath = "C:/Users/Marco/Documents/DEV/Resources/OSMQueries/"; //Marco Home
        //oSMCachePath = "D:/UnityWorkspace/GeoData/OSM/"; // Dennis Büro
#endif
    }
    private string logPath;
    public static string oSMCachePath;

#if UNITY_EDITOR
    [MenuItem("City/Clear OSM Cache")]
    static void ClearOSMCache()
    {
        DirectoryInfo osmCache = new DirectoryInfo(oSMCachePath);

        foreach (FileInfo file in osmCache.GetFiles("*.xml"))
        {
            file.Delete();
        }
    }
#endif

    public readonly string URL = //"http://overpass.osm.rambler.ru/cgi/interpreter?data=";
                                    "http://overpass-api.de/api/interpreter?data=";

    public OSMBoundingBox BoundingBox
    {
        get { return boundingBox; }
        set { boundingBox = value; }
    }
    [SerializeField]
    private OSMBoundingBox boundingBox;

    public OSMData OSM;

    //private static string querystring = "(node{0};way{0};node(w)->.x;);out;rel{0};out;";
    //private static string querystring = "(node{0};rel(bn)->.x; way(bn); node(w); rel(bw)->.y; rel.y(br););out;";
    //private static string querystring = "(node{0};<;>;);out;"; // Läd zuviel... 
    private static string querystring = "node{0};way[\"highway\"](bn)->.a;way[\"building\"](bn)->.b;way[\"waterway\"](bn)->.c;(.b;.b >;.a;.a >;.c;.c >;)->.e;.e out;";

    public event EventHandler QueryDone;
    protected void OnQueryDone()
    {
        if (QueryDone != null)
            QueryDone(this, new EventArgs());
    }

    public void DownloadOSMString()
    {
        Console.AddMessage("Webclient");
        WebClient client = new WebClient();

        UnityEngine.Debug.Log(URL + string.Format(querystring, boundingBox.ToQLString()));
        string url = URL + string.Format(querystring, boundingBox.ToQLString());
		client.Encoding=Encoding.UTF8;
        client.DownloadStringCompleted += ParseOSMXml;
        client.DownloadStringAsync(new Uri(url));
    }

    public void ParseOSMXml(object sender, DownloadStringCompletedEventArgs args)
    {
        
		string result = args.Result;
        if (result == null) return;
        UnityEngine.Debug.Log(result);

#if UNITY_EDITOR
		//File.WriteAllBytes(logPath, Encoding.UTF8.GetBytes(result));
#endif

		OSM = new OSMData(new MemoryStream(Encoding.UTF8.GetBytes(result)));
        OnQueryDone();
    }
}