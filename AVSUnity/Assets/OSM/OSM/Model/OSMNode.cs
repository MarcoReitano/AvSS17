using UnityEngine;
#if UNITY_EDITOR
//using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Text;
using System.Globalization;

///*
// * <!ELEMENT node (tag*)>
// * <!ATTLIST node id                CDATA #REQUIRED>
// * <!ATTLIST node lat               CDATA #REQUIRED>
// * <!ATTLIST node lon               CDATA #REQUIRED>
// * <!ATTLIST node changeset         CDATA #IMPLIED>             <---  NOT REQUIRED
// * <!ATTLIST node visible           (true|false) #REQUIRED>
// * <!ATTLIST node user              CDATA #IMPLIED>             <---  NOT REQUIRED
// * <!ATTLIST node timestamp         CDATA #IMPLIED>             <---  NOT REQUIRED
// * */

/// <summary>
/// OSMNode.
/// </summary>
[System.Serializable]
public class OSMNode
{
    public OSMNode(long id, double latitude, double longitude)
    {
        this.id = id;
        this.latitude = latitude;
        this.longitude = longitude;
    }

    public OSMNode()
    {
        // Needed for Parsing
    }

    public double Latitude
    {
        get { return latitude; }
        set { latitude = value; }
    }
    public double Longitude
    {
        get { return longitude; }
        set { longitude = value; }
    }
    public long Id
    {
        get { return id; }
        set { id = value; }
    }
    public int Version
    {
        get { return version; }
        set { version = value; }
    }
    public bool Visible
    {
        get { return visible; }
        set { visible = value; }
    }

    public Dictionary<string, string> Tags = new Dictionary<string, string>();

    [SerializeField]
    private double longitude = 0d;
    [SerializeField]
    private double latitude = 0d;
    [SerializeField]
    private long id = 0;
    [SerializeField]
    private int version = 0;
    [SerializeField]
    private bool visible = true;
       

    public float X
    {
        get { return (float)((longitude - TileManager.OriginLongitude) * TileManager.Scaling); } 
    }
    public float Z
    {
        get { return (float)((latitude - TileManager.OriginLatitude) * TileManager.Scaling); }
    }

    public Vector3 Position2D
    {
        get { return new Vector3(X, 0f, Z); }
    }
    public bool IsInBounds(OSMBoundingBox box)
    {
        if (latitude > box.MaxLatitude)
            return false;
        if (latitude < box.MinLatitude)
            return false;
        if (longitude > box.MaxLongitude)
            return false;
        if (longitude < box.MinLongitude)
            return false;
        return true;
    }

    public static implicit operator Vector3(OSMNode node)
    {
        return new Vector3(node.X, 0, node.Z);
    }
    public static implicit operator Vector2(OSMNode node)
    {
        return new Vector2(node.X, node.Z);
    }
    public static implicit operator long(OSMNode node)
    {
        return node.id;
    }


    public void AddTag(string k, string v)
    {
        Tags.Add(k, v);
    }


    public bool RemoveTag(string k, string v)
    {
        if (Tags.ContainsKey(k))
        {
            return Tags.Remove(k);
        }
        return false;
    }


    /// <summary>
    /// <node id="313576" lat="50.9500261" lon="6.9663865" user="Raymond" uid="16478" visible="true" version="4" changeset="4862326" timestamp="2010-05-31T11:57:12Z"/>

    /// </summary>
    /// <returns></returns>
    public string GetXMLString()
    {
        StringBuilder sb = new StringBuilder();

        sb.Append("  ");
        sb.Append("<node id=\"").Append(id)
            .Append("\" lat=\"").Append(latitude.ToString("0.#######", CultureInfo.CreateSpecificCulture("en-US")))
            .Append("\" lon=\"").Append(longitude.ToString("0.#######", CultureInfo.CreateSpecificCulture("en-US")))
            .Append("\" visible=\"").Append(visible.ToString())
            .Append("\" version=\"").Append(version.ToString())
            .Append("\">");

        if (Tags.Count > 0)
            sb.Append('\n');
        foreach (KeyValuePair<string, string> tag in Tags)
        {
            sb.Append("    ");
            sb.Append("<tag k=\"").Append(tag.Key.ToString()).Append("\" v=\"").Append(tag.Value.ToString()).Append("\"/>\n");
        }

        if (Tags.Count > 0)
            sb.Append("  ");
        sb.Append("</node>\n");
        return sb.ToString();
    }




    /*
     * <!ELEMENT node (tag*)>
     * <!ATTLIST node id                CDATA #REQUIRED>
     * <!ATTLIST node lat               CDATA #REQUIRED>
     * <!ATTLIST node lon               CDATA #REQUIRED>
     * <!ATTLIST node changeset         CDATA #IMPLIED>
     * <!ATTLIST node visible           (true|false) #REQUIRED>
     * <!ATTLIST node user              CDATA #IMPLIED>
     * <!ATTLIST node timestamp         CDATA #IMPLIED>
     * */
    public static OSMNode ParseNode(XmlNode node)
    {
        OSMNode osmNode = new OSMNode();

        //bool keep = false;

        // parse node-attributes
        foreach (XmlAttribute attribute in node.Attributes)
        {
            //string name = ;
            switch (attribute.Name)
            {
                case "id":
                    osmNode.Id = long.Parse(attribute.Value);
                    break;
                case "lat":
                    osmNode.Latitude = float.Parse(attribute.Value, CultureInfo.CreateSpecificCulture("en-US"));
                    break;
                case "lon":
                    osmNode.Longitude = float.Parse(attribute.Value, CultureInfo.CreateSpecificCulture("en-US"));
                    break;
                case "visible":
                    osmNode.Visible = bool.Parse(attribute.Value);
                    //keep = osmNode.visible;
                    break;
                case "version":
                    osmNode.version = int.Parse(attribute.Value);
                    break;
                default:
                    break;
            }
        }

        foreach (XmlNode child in node.ChildNodes)
        {
            if (child.Name == "tag")
            {
                //osmNode.AddTag(OSMParser.ReadKeyValue(child));
                osmNode.AddTag(node.Attributes[0].Value, node.Attributes[1].Value);
                
            }
        }
        return osmNode;
    }
}
