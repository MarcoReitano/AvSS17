using UnityEngine;
#if UNITY_EDITOR
//using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Text;


/*
 * <!ELEMENT way (tag*,nd,tag*,nd,(tag|nd)*)>
 * <!ATTLIST way id                 CDATA #REQUIRED>
 * <!ATTLIST way changeset          CDATA #IMPLIED>
 * <!ATTLIST way visible            (true|false) #REQUIRED>
 * <!ATTLIST way user               CDATA #IMPLIED>
 * <!ATTLIST way timestamp          CDATA #IMPLIED>

 * <!ELEMENT nd EMPTY>
 * <!ATTLIST nd ref                 CDATA #REQUIRED>
 */

/// <summary>
/// OSMWay.
/// </summary>
[System.Serializable]
public class OSMWay
{
    public OSMWay(long id)
    {
        this.id = id;
    }

    public OSMWay()
    {
        // TODO: Complete member initialization
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
    public List<long> WayNodes
    {
        get { return wayNodes; }
    }
    public Dictionary<string, string> Tags
    {
        get { return tags; }
    }

    [SerializeField]
    private long id;
    [SerializeField]
    private int version;
    [SerializeField]
    private bool visible = true;

   

    [SerializeField]
    private List<long> wayNodes = new List<long>();
    private Dictionary<string, string> tags = new Dictionary<string, string>();

    public long MinWayNode
    {
        get
        {
            long min = long.MaxValue;
            for (int i = 0; i < WayNodes.Count; i++)
            {
                if (min >= WayNodes[i])
                    min = WayNodes[i];
            }
            return min;
        }
    }
    public bool FirstInBounds(OSMBoundingBox box, OSMData osm)
    {
        OSMNode node;
        if(osm.nodes.TryGetValue(WayNodes[0], out node))
            return node.IsInBounds(box);
        return false;
    }
    public bool ContainsNode(OSMNode node)
    {
        return WayNodes.Contains(node.Id);
    }
    public bool ContainsNode(long id)
    {
        return WayNodes.Contains(id);
    }
    public List<long> GetWayNodesFromToExclusive(long from, long to)
    {
        int fromIndex = WayNodes.IndexOf(from);
        int toIndex = WayNodes.IndexOf(to);

        if (fromIndex < toIndex)
            return WayNodes.GetRange(fromIndex, toIndex - fromIndex);
        if (toIndex < fromIndex)
        {
            List<long> result = WayNodes.GetRange(toIndex, fromIndex - toIndex);
            result.Reverse();
            return result;
        }
        UnityEngine.Debug.Log("GetWayNodesFromTo returned null");
        return null;
    }
    public List<long> GetWayNodesFromTo(long from, long to)
    {
        int fromIndex = WayNodes.IndexOf(from);
        int toIndex = WayNodes.IndexOf(to);

        if (fromIndex < toIndex)
            return WayNodes.GetRange(fromIndex, toIndex - fromIndex + 1);
        if (fromIndex > toIndex)
        {
            List<long> result = WayNodes.GetRange(toIndex, fromIndex - toIndex + 1);
            result.Reverse();
            return result;
        }
        UnityEngine.Debug.Log("GetWayNodesFromTo returned null");
        return null;
    }


    public string GetXMLString()
    {
        StringBuilder sb = new StringBuilder();

        sb.Append("  ");
        sb.Append("<way id=\"").Append(id)
            .Append("\" version=\"").Append(version.ToString()).Append("\">\n");

        // TODO: not using the nodeIDs but the actual references
        //foreach (OSMNode node in wayNodes)
        //{
        //    sb.Append("    ");
        //    sb.Append("<nd ref=\"").Append(node.Id).Append("\"/>\n");
        //}
        foreach (long node in wayNodes)
        {
            sb.Append("    ");
            sb.Append("<nd ref=\"").Append(node).Append("\"/>\n");
        }

        foreach (KeyValuePair<string, string> tag in tags)
        {
            sb.Append("    ");
            sb.Append("<tag k=\"").Append(tag.Key).Append("\" v=\"").Append(tag.Value).Append("\"/>\n");
        }

        sb.Append("  ");
        sb.Append("</way>\n");
        return sb.ToString();
    }




    /*
    * <!ELEMENT way (tag*,nd,tag*,nd,(tag|nd)*)>
    * <!ATTLIST way id                 CDATA #REQUIRED>
    * <!ATTLIST way changeset          CDATA #IMPLIED>
    * <!ATTLIST way visible            (true|false) #REQUIRED>
    * <!ATTLIST way user               CDATA #IMPLIED>
    * <!ATTLIST way timestamp          CDATA #IMPLIED>

    * <!ELEMENT nd EMPTY>
    * <!ATTLIST nd ref                 CDATA #REQUIRED>
    */
    public static OSMWay ParseWay(XmlNode node, ref OSMMap map)
    {
        OSMWay osmWay = new OSMWay();

        // TODO: Check import
        ////HashDictionary<int, OSMNode> tmpNodesDict = new HashDictionary<int, OSMNode>();
        //bool keep = false;

        //// parse node-attributes
        //foreach (XmlAttribute attribute in node.Attributes)
        //{
        //    string name = attribute.Name;
        //    switch (name)
        //    {
        //        case "id":
        //            osmWay.Id = int.Parse(attribute.Value);
        //            break;
        //        case "visible":
        //            osmWay.Visible = bool.Parse(attribute.Value);
        //            break;
        //        case "version":
        //            osmWay.version = int.Parse(attribute.Value);
        //            break;
        //        default:
        //            break;
        //    }
        //}

        //// parse <nd /> Nodes
        //foreach (XmlNode childnode in node.ChildNodes)
        //{
        //    switch (childnode.Name)
        //    {
        //        case "nd":
        //            long id = long.Parse(childnode.Attributes[0].Value);
        //            if (map.Nodes.Contains(id))
        //            {
        //                osmWay.Nodes.Add(map.Nodes[id]);
        //                //if (!tmpNodesDict.Contains(id))
        //                //    tmpNodesDict.Add(id, map.Nodes[id]);
        //            }
        //            break;
        //        case "tag":
        //            osmWay.AddTag(OSMParser.ReadKeyValue(childnode));
        //            KeyValuePair<string, string> tag = OSMParser.ReadKeyValue(childnode);

        //            switch (tag.Key)
        //            {
        //                case "highway":
        //                    {
        //                        switchWayHighwayValues(out keep, tag.Value);
        //                    }
        //                    break;
        //                case "lanes":
        //                    {
        //                        try
        //                        {
        //                            osmWay.Lanes = int.Parse(tag.Value);
        //                            //Console.Write("lanes={0}", osmWay.Lanes);
        //                        }
        //                        catch (FormatException e)
        //                        {
        //                            e.ToString();
        //                            //Console.Write("lanes={0}", value);
        //                        }

        //                    }
        //                    break;
        //                case "name":
        //                case "nam":
        //                case "name:de":
        //                case "int_name":
        //                case "ref":
        //                case "nat_ref":
        //                case "loc_ref":
        //                case "int_ref":
        //                    OSMStatistics.wayNameTagCount++;

        //                    break;
        //                default:
        //                    //Console.Write(key);
        //                    break;
        //            }
        //            break;
        //        default:
        //            break;
        //    }
        //}

        return osmWay;
    }



    private static void switchWayHighwayValues(out bool keep, string value)
    {
        keep = false;
        switch (value)
        {
            case "motorway":
                OSMStatistics.highway_motorway++;
                keep = true;
                break;
            case "motorway_link":
                OSMStatistics.highway_motorway_link++;
                keep = true;
                break;
            case "trunk":
                OSMStatistics.highway_trunk++;
                keep = true;
                break;
            case "trunk_link":
                OSMStatistics.highway_trunk_link++;
                keep = false;
                break;
            case "primary":
                OSMStatistics.highway_primary++;
                keep = true;
                break;
            case "primary_link":
                OSMStatistics.highway_primary_link++;
                keep = true;
                break;
            case "secondary":
                OSMStatistics.highway_secondary++;
                keep = true;
                break;
            case "secondary_link":
                OSMStatistics.highway_secondary_link++;
                keep = true;
                break;
            case "tertiary":
                OSMStatistics.highway_tertiary++;
                keep = true;
                break;
            case "residential":
                OSMStatistics.highway_residential++;
                keep = true;
                break;
            case "unclassified":
                OSMStatistics.highway_unclassified++;
                keep = true;
                break;
            default:

                break;
        }
    }


    public override string ToString()
    {
        System.Text.StringBuilder sB = new System.Text.StringBuilder();
        sB.AppendLine("Way " + id);
        sB.AppendLine("  Tags, Count: " + tags.Count);
        foreach (KeyValuePair<string, string> kV in tags)
        {
            sB.AppendLine("    " + kV.Key + " - " + kV.Value);
        }
        sB.AppendLine("  WayNodes, Count: " + wayNodes.Count);
        foreach(long node in wayNodes)
        {
            sB.AppendLine("    " +node.ToString());
        }
        return sB.ToString();

    }
}
