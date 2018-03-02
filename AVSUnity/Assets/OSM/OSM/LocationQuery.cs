using UnityEngine;
using System.Net;
using System;
using System.Text;
using System.Xml;
using System.Collections.Generic;

public struct Location
{
    public string name;
    public double lon;
    public double lat;


    public Location(XmlNode name, XmlNode lon, XmlNode lat):
        this(name.InnerText, lon.InnerText, lat.InnerText)
    {
            
    }

    public Location(string name, string lon, string lat)
    {
        this.name = name;
        this.lon = double.Parse(lon);
        this.lat = double.Parse(lat);
    }

    public override string ToString()
    {
        return name + " (" + lon + ", " + lat + ")";
    }
}


/// <summary>
/// OverpassQuery.
/// </summary>
[System.Serializable]
public class LocationQuery
{
    

    //int retry = 3;
    public LocationQuery()
    {
        //retry = 3;
    }

    public event EventHandler QueryDone;
    protected void OnQueryDone()
    {
        if (QueryDone != null)
            QueryDone(this, new EventArgs());
    }

    WebClient client;
    public void SearchLocation(string searchString)
    {
        try
        {
            if (client == null)
                client = new WebClient();

            string url = "http://nominatim.openstreetmap.org/search/" + searchString + "?format=xml";

            client.Encoding = Encoding.UTF8;
            client.DownloadStringCompleted += ParseLocation;
            client.DownloadStringAsync(new Uri(url));
        }
        catch (WebException e)
        {
            UnityEngine.Debug.Log("i got this exception: " + e.Message);
            //retry--;
            //if (retry > 0)
            //{
                UnityEngine.Debug.Log("retry... ");// + retry);
                SearchLocation(searchString);
            //}
        }

    }


    public List<Location> searchResults = new List<Location>();
    public void ParseLocation(object sender, DownloadStringCompletedEventArgs args)
    {
        string result = args.Result;
        if (result == null)
        {
            Debug.Log("No Result");
            foreach (string item in client.QueryString.AllKeys)
            {
                Debug.Log(item);
            }

            return;
        }
        Debug.Log(result);

        XmlDocument doc = new XmlDocument();
        doc.LoadXml(result);
        Debug.Log("outerXML" + doc.OuterXml);
        XmlNodeList places = doc.GetElementsByTagName("place");
        searchResults = new List<Location>();
        int count = 0;
        foreach (XmlNode item in places)
        {
            XmlAttributeCollection attributes = item.Attributes;
            XmlNode displayNameNode = attributes.GetNamedItem("display_name");
            XmlNode latNode = attributes.GetNamedItem("lat");
            XmlNode lonNode = attributes.GetNamedItem("lon");
            searchResults.Add(new Location(displayNameNode, lonNode, latNode));
            Debug.Log(count++ +": " +displayNameNode.InnerText + " ("+ latNode.InnerText + "," + lonNode.InnerText + ")");
        }
        // select nodes
        // get lon/lat of first element

        OnQueryDone();
    }
}