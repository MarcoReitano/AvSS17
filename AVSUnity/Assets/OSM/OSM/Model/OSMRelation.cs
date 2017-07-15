using UnityEngine;
#if UNITY_EDITOR
//using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Text;

/// <summary>
/// OSMRelation.
/// </summary>
[System.Serializable]
public class OSMRelation
{
    public OSMRelation(long id)
    {
        this.id = id;
    }

    public OSMRelation()
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

    [SerializeField]
    private long id;
    [SerializeField]
    private int version;
    [SerializeField]
    private bool visible = false;

    
    
    public List<OSMMember> Members = new List<OSMMember>();
    public Dictionary<string, string> Tags = new Dictionary<string, string>();


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
       
    // TODO: Ist ein RelationMember immer ein OSM
    //public void AddMember(OSMRelationMember member)
    //{
    //    Members.Add(member);
    //}

    //public bool RemoveMember(OSMRelationMember member)
    //{
    //    if (members.Contains(member))
    //    {
    //        return members.Remove(member);
    //    }
    //    return false;
    //}


    public string GetXMLString()
    {
        StringBuilder sb = new StringBuilder();

        sb.Append("  ");
        sb.Append("<relation id=\"").Append(id)
            .Append("\" version=\"").Append(version.ToString()).Append("\">\n");

        foreach (OSMMember member in Members)
        {
            sb.Append(member.GetXMLString());
        }

        foreach (KeyValuePair<string, string> tag in Tags)
        {
            sb.Append("    ");
            sb.Append("<tag k=\"").Append(tag.Key).Append("\" v=\"").Append(tag.Value).Append("\"/>\n");
        }

        sb.Append("  ");
        sb.Append("</relation>\n");
        return sb.ToString();
    }


    //<relation id="448540" user="OSMicha" uid="172947" visible="true" version="21" changeset="4699466" timestamp="2010-05-14T20:09:09Z">
    public static OSMRelation ParseRelation(XmlNode node, ref OSMMap map)
    {
        OSMRelation osmRelation = new OSMRelation();

        //HashDictionary<int, OSMNode> tmpNodesDict = new HashDictionary<int, OSMNode>();
        //bool keep = false;

        // parse node-attributes
        foreach (XmlAttribute attribute in node.Attributes)
        {
            string name = attribute.Name;
            switch (name)
            {
                case "id":
                    osmRelation.Id = int.Parse(attribute.Value);
                    break;
                case "visible":
                    osmRelation.Visible = bool.Parse(attribute.Value);
                    break;
                case "version":
                    osmRelation.version = int.Parse(attribute.Value);
                    break;
                default:
                    break;
            }
        }

        // parse <nd /> Nodes
        foreach (XmlNode childnode in node.ChildNodes)
        {
            switch (childnode.Name)
            {
                case "member":
                    //osmRelation.AddMember(OSMRelationMember.ParseRelationMember(childnode));
                    osmRelation.Members.Add(OSMMember.ParseRelationMember(childnode));
                    break;
                case "tag":
                    //osmRelation.AddTag(OSMParser.ReadKeyValue(childnode));
                    osmRelation.AddTag(childnode.Attributes[0].Value, childnode.Attributes[0].Value);
                    //KeyValuePair<string, string> tag = OSMParser.ReadKeyValue(childnode);
                    break;
                default:
                    break;
            }
        }

        return osmRelation;
    }

}
