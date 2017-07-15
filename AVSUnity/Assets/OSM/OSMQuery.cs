using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

public class OSMQuery : MonoBehaviour {

	// Use this for initialization
	void Start () {
		OSMQuery.DownloadOSMString();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	//generiere die Query
	//kontrolliere, ob bereich bereits heruntergeladen wurde
	//nutze die datei und parse diese ODER lade daten herunter, schreibe datei und parse diese.

		private static string API_URI = "http://overpass-api.de/api/interpreter?data=";
		private static string query = "node{0};way[\"highway\"](bn)->.a;way[\"building\"](bn)->.b;way[\"waterway\"](bn)->.c;(.b;.b >;.a;.a >;.c;.c >;)->.e;.e out;"; // {0} wird später mit der boundingbox per regex ersetztersetzt
        public OSMData OSM;

		public static void DownloadOSMString()
		{
			Console.WriteLine("Webclient");
			WebClient client = new WebClient();
			//OSMBoundingBox boundingBox = new OSMBoundingBox (51.0157,7.5522,51.0338,7.5801); //Gummersbach
			OSMBoundingBox boundingBox = new OSMBoundingBox(51.0100, 7.5500, 51.0200, 7.5600);

			Console.WriteLine(API_URI + string.Format(query, boundingBox.toQueryLanguage()));
			string url = API_URI + string.Format(query, boundingBox.toQueryLanguage());

			client.Encoding = Encoding.UTF8;
			client.DownloadStringCompleted += ParseOSMXml;
			client.DownloadStringAsync(new Uri(url));

		}
		private static void ParseOSMXml(object sender, DownloadStringCompletedEventArgs args)
		{
			string result = args.Result;
            Debug.Log(result);
			Console.Write(result);
            OSMData OSM = new OSMData(new MemoryStream(Encoding.UTF8.GetBytes(result)));

		}

	}
