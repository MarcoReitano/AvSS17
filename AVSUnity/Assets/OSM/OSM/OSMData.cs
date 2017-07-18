using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.IO;
using System.Text;
using System.Xml;
using System.Collections.Generic;

/// <summary>
/// OSM.
/// </summary>
[System.Serializable]
public class OSMData
{	
	public OSMData(string osmXMLpath)
	{		
		XmlTextReader xmlReader = new XmlTextReader(osmXMLpath); 
        xmlReader.MoveToContent();
		while(xmlReader.Read()){
			if (xmlReader.NodeType.Equals(XmlNodeType.Element)) {
                    switch (xmlReader.LocalName) {
                        case "node":
							OSMNode n = ParseOSMNode(xmlReader);
            				nodes.Add(n.Id, n);
                            break;
                        case "way":
							OSMWay way = ParseOSMWay(xmlReader);
            				ways.Add(way.Id, way);                          
                            break;
                        case "relation":
                            OSMRelation relation = ParseOSMRelation(xmlReader);
            				relations.Add(relation.Id, relation);
                            break;
                        default:
                            //ignore other nodes
                            break;
                    }
                }
		}
		UnityEngine.Debug.Log("New OSMDATA: " + nodes.Count + " Nodes " + ways.Count + " Ways " + relations.Count + " Relations");		
        Console.AddMessage("New OSMDATA: " + nodes.Count + " Nodes " + ways.Count + " Ways " + relations.Count + " Relations");
	}
	
	public OSMData(Stream osmXMLStream)
	{		
        UnityEngine.Debug.Log(osmXMLStream);
		XmlTextReader xmlReader = new XmlTextReader(osmXMLStream); 
        xmlReader.MoveToContent();
		while(xmlReader.Read()){
			if (xmlReader.NodeType.Equals(XmlNodeType.Element)) {
                    switch (xmlReader.LocalName) {
                        case "node":
							OSMNode n = ParseOSMNode(xmlReader);
            				nodes.Add(n.Id, n);
                            break;
                        case "way":
							OSMWay way = ParseOSMWay(xmlReader);
            				ways.Add(way.Id, way);                          
                            break;
                        case "relation":
                            OSMRelation relation = ParseOSMRelation(xmlReader);
            				relations.Add(relation.Id, relation);
                            break;
                        default:
                            //ignore other nodes
                            break;
                    }
                }
		}
		UnityEngine.Debug.Log("New OSMDATA: " + nodes.Count + " Nodes " + ways.Count + " Ways " + relations.Count + " Relations");
		Console.AddMessage("New OSMDATA: " + nodes.Count + " Nodes " + ways.Count + " Ways " + relations.Count + " Relations");
	}
	
	private OSMNode ParseOSMNode(XmlTextReader xmlReader)
	{
		OSMNode node = new OSMNode(long.Parse(xmlReader.GetAttribute("id")), float.Parse(xmlReader.GetAttribute("lat")), float.Parse(xmlReader.GetAttribute("lon")));		
		string childs = xmlReader.ReadOuterXml();
		
		XmlTextReader outerXmlReader = new XmlTextReader(new System.IO.StringReader(childs)); 
        outerXmlReader.MoveToContent();
		while(outerXmlReader.Read()){
			if (outerXmlReader.NodeType.Equals(XmlNodeType.Element) && outerXmlReader.LocalName.Equals("tag")) {
				node.AddTag(outerXmlReader.GetAttribute("k"),outerXmlReader.GetAttribute("v"));
			}
		}		
		return node;
	}
	
	private OSMWay ParseOSMWay(XmlTextReader xmlReader){
    	OSMWay way = new OSMWay(long.Parse(xmlReader.GetAttribute("id")));
		
		string childs = xmlReader.ReadOuterXml();

		XmlTextReader outerXmlReader = new XmlTextReader(new System.IO.StringReader(childs)); 
	    outerXmlReader.MoveToContent();
		while(outerXmlReader.Read()){
			if (outerXmlReader.NodeType.Equals(XmlNodeType.Element) && outerXmlReader.LocalName.Equals("tag")) {
				way.Tags.Add(outerXmlReader.GetAttribute("k"),outerXmlReader.GetAttribute("v"));
			}
			if (outerXmlReader.NodeType.Equals(XmlNodeType.Element) && outerXmlReader.LocalName.Equals("nd")) {
				way.WayNodes.Add(long.Parse(outerXmlReader.GetAttribute("ref")));
			}
		}
		
		return way;
	}

	public OSMRelation ParseOSMRelation(XmlTextReader xmlReader)
	{
        OSMRelation relation = new OSMRelation(long.Parse(xmlReader.GetAttribute("id")));
		
		string childs = xmlReader.ReadOuterXml();

		XmlTextReader outerXmlReader = new XmlTextReader(new System.IO.StringReader(childs)); 
	    outerXmlReader.MoveToContent();
		while(outerXmlReader.Read()){
			if (outerXmlReader.NodeType.Equals(XmlNodeType.Element) && outerXmlReader.LocalName.Equals("tag")) {
				relation.Tags.Add(outerXmlReader.GetAttribute("k"),outerXmlReader.GetAttribute("v"));
			}
			if (outerXmlReader.NodeType.Equals(XmlNodeType.Element) && outerXmlReader.LocalName.Equals("member")) {
				relation.Members.Add(new OSMMember(outerXmlReader.GetAttribute("type"),long.Parse(outerXmlReader.GetAttribute("ref")),outerXmlReader.GetAttribute("role")));
			}
		}	
		return relation;
	}	
	
	
    public OSMData(XmlDocument doc)
    {
        XmlNodeList xmlNodes = doc.GetElementsByTagName("node");
        XmlNodeList xmlWays = doc.GetElementsByTagName("way");
        XmlNodeList xmlRelations = doc.GetElementsByTagName("relation");

        long id;
        float longitude;
        float latitude;

        foreach (XmlNode node in xmlNodes)
        {
            if (!long.TryParse(node.Attributes["id"].Value, out id))
                continue;
            if (!float.TryParse(node.Attributes["lon"].Value, out longitude))
                continue;
            if (!float.TryParse(node.Attributes["lat"].Value, out latitude))
                continue;
            OSMNode n = new OSMNode(id, latitude, longitude);

            nodes.Add(id, n);

            XmlNodeList childNodes = node.ChildNodes;

            foreach (XmlNode tagNode in childNodes)
            {
                if (tagNode.Name == "tag")
                    n.Tags.Add(tagNode.Attributes["k"].Value, tagNode.Attributes["v"].Value);
            }
        }

        foreach (XmlNode wayNode in xmlWays)
        {
            if (!long.TryParse(wayNode.Attributes["id"].Value, out id))
                continue;
            OSMWay way = new OSMWay(id);
            ways.Add(id, way);

            XmlNodeList childNodes = wayNode.ChildNodes;

            foreach (XmlNode childNode in childNodes)
            {
                if (childNode.Name == "nd")
                {
                    long referenceId;
                    if (long.TryParse(childNode.Attributes["ref"].Value, out referenceId))
                        way.WayNodes.Add(referenceId);
                }

                if (childNode.Name == "tag")
                    way.Tags.Add(childNode.Attributes["k"].Value, childNode.Attributes["v"].Value);
            }

        }

        foreach (XmlNode relNode in xmlRelations)
        {
            if (!long.TryParse(relNode.Attributes["id"].Value, out id))
                continue;

            OSMRelation relation = new OSMRelation(id);

            relations.Add(id, relation);

            XmlNodeList childNodes = relNode.ChildNodes;

            foreach (XmlNode childNode in childNodes)
            {
                if (childNode.Name == "member")
                {
                    if (!long.TryParse(childNode.Attributes["ref"].Value, out id))
                        continue;

                    relation.Members.Add(new OSMMember(childNode.Attributes["type"].Value, id, childNode.Attributes["role"].Value));
                }
                if (childNode.Name == "tag")
                    relation.Tags.Add(childNode.Attributes["k"].Value, childNode.Attributes["v"].Value);
            }

        }

        Console.AddMessage("New OSMDATA: " + nodes.Count + " Nodes " + ways.Count + " Ways " + relations.Count + " Relations");
    }

    public Dictionary<long, OSMNode> nodes = new Dictionary<long, OSMNode>();
    public Dictionary<long, OSMWay> ways = new Dictionary<long, OSMWay>();
    public Dictionary<long, OSMRelation> relations = new Dictionary<long, OSMRelation>();

    public void OnDrawGizmos(Vector3 osmPos)
    {
        //if (nodes != null)
        //{
        //    //Debug.Log("nodes != null");
        //    foreach (KeyValuePair<long, OSMNode> kV in nodes)
        //    {
        //        Debug.Log("nodeposition: " + (kV.Value - osmPos) * Tile.Scaling);
        //        Gizmos.DrawCube((kV.Value - osmPos) * Tile.Scaling, Vector3.one * 1f);
        //    }
        //}

        if (ways != null)
        {
            foreach (KeyValuePair<long, OSMWay> kV in ways)
            {
                if (kV.Value.Tags.ContainsKey("highway"))
                {
                    Gizmos.color = OSMMapTools.HighwayColor;
                    if (kV.Value.Tags["highway"] == "footway")
                        Gizmos.color = OSMMapTools.HighwayFootwayColor;
                    if (kV.Value.Tags["highway"] == "service")
                        Gizmos.color = OSMMapTools.HighwayServiceColor;
                    if (kV.Value.Tags["highway"] == "secondary")
                        Gizmos.color = OSMMapTools.HighwaySecondaryColor;
                    if (kV.Value.Tags["highway"] == "tertiary")
                        Gizmos.color = OSMMapTools.HighwayTertiaryColor;

                }
                else if (kV.Value.Tags.ContainsKey("building"))
                    Gizmos.color = OSMMapTools.BuildingColor;
                else if (kV.Value.Tags.ContainsKey("waterway"))
                    Gizmos.color = OSMMapTools.WaterwayColor;
                else
                    Gizmos.color = OSMMapTools.DefaultColor;

                if (OSMMapTools.KeySearch && OSMMapTools.ValueSearch)
                {
                    if (kV.Value.Tags.ContainsKey(OSMMapTools.KeySearchTerm))
                    {
                        if (kV.Value.Tags[OSMMapTools.KeySearchTerm] == OSMMapTools.ValueSearchTerm)
                            Gizmos.color = OSMMapTools.HighlightColor;
                        else
                            Gizmos.color = OSMMapTools.DefaultColor;
                    }
                    else
                        Gizmos.color = OSMMapTools.DefaultColor;
                }
                else if (OSMMapTools.KeySearch)
                {
                    if (kV.Value.Tags.ContainsKey(OSMMapTools.KeySearchTerm))
                        Gizmos.color = OSMMapTools.HighlightColor;
                    else
                        Gizmos.color = OSMMapTools.DefaultColor;
                }
                else if (OSMMapTools.ValueSearch)
                {
                    if (kV.Value.Tags.ContainsValue(OSMMapTools.ValueSearchTerm))
                        Gizmos.color = OSMMapTools.HighlightColor;
                    else
                        Gizmos.color = OSMMapTools.DefaultColor;
                }




                for (int i = 0; i < kV.Value.WayNodes.Count - 1; i++)
                {
                    OSMNode node1;
                    OSMNode node2;

                    if (!nodes.TryGetValue(kV.Value.WayNodes[i], out node1))
                    {
                        continue;
                    }
                    if (!nodes.TryGetValue(kV.Value.WayNodes[i + 1], out node2))
                    {
                        continue;
                    }
                    if (OSMMapTools.DrawOnTerrainHeight)
                        Gizmos.DrawLine(node1, node2);
                    else
                        Gizmos.DrawLine(node1.Position2D, node2.Position2D);
                }
            }
        }
    }

#if UNITY_EDITOR
    public void OnSceneGUI()
    {
        //if (nodes != null)
        //{
        //    //Debug.Log("nodes != null");
        //    foreach (KeyValuePair<long, OSMNode> kV in nodes)
        //    {
        //        Debug.Log("nodeposition: " + (kV.Value - osmPos) * Tile.Scaling);
        //        Gizmos.DrawCube((kV.Value - osmPos) * Tile.Scaling, Vector3.one * 1f);
        //    }
        //}

        if (ways != null)
        {
            foreach (KeyValuePair<long, OSMWay> kV in ways)
            {
                if (kV.Value.Tags.ContainsKey("highway"))
                {
                    Handles.color = OSMMapTools.HighwayColor;
                    if (kV.Value.Tags["highway"] == "footway")
                        Handles.color = OSMMapTools.HighwayFootwayColor;
                    if (kV.Value.Tags["highway"] == "service")
                        Handles.color = OSMMapTools.HighwayServiceColor;

                    if (kV.Value.Tags["highway"] == "secondary")
                        Handles.color = OSMMapTools.HighwaySecondaryColor;


                    if (kV.Value.Tags["highway"] == "tertiary")
                        Handles.color = OSMMapTools.HighwayTertiaryColor;

                }
                else if (kV.Value.Tags.ContainsKey("building"))
                    Handles.color = OSMMapTools.BuildingColor;
                else if (kV.Value.Tags.ContainsKey("waterway"))
                    Handles.color = OSMMapTools.WaterwayColor;
                else
                    Handles.color = OSMMapTools.DefaultColor; 

                for (int i = 0; i < kV.Value.WayNodes.Count - 1; i++)
                {
                    OSMNode node1;
                    OSMNode node2;

                    if (!nodes.TryGetValue(kV.Value.WayNodes[i], out node1))
                    {
                        continue;
                    }
                    if (!nodes.TryGetValue(kV.Value.WayNodes[i + 1], out node2))
                    {
                        continue;
                    }

                    Vector3 direction = (Vector3)node2 - (Vector3)node1;
                    if (direction != Vector3.zero)
                    {
                        if (OSMMapTools.DrawOnTerrainHeight)
                        {
                            if (Handles.Button(node1, Quaternion.LookRotation(direction), direction.magnitude, 2f, DrawLine))
                            {
                                OSMMapTools.SelectedWay = kV.Value;
                                Debug.Log(kV.Value.ToString());
                            }
                        }
                        else
                        {
                            if (Handles.Button(node1.Position2D, Quaternion.LookRotation(direction.ToXZPlaneVector3()), direction.magnitude, 2f, DrawLine))
                            {
                                OSMMapTools.SelectedWay = kV.Value;
                                Debug.Log(kV.Value.ToString());
                            } 
                        }
                    }
                }
            }
        }
    }

    static  Material lineMaterial;
    static void CreateLineMaterial() {
	    if( !lineMaterial ) {
		    lineMaterial = new Material( "Shader \"Lines/Colored Blended\" {" +
			    "SubShader { Pass { " +
			    "    Blend SrcAlpha OneMinusSrcAlpha " +
			    "    ZWrite Off Cull Off Fog { Mode Off } " +
			    "    BindChannels {" +
			    "      Bind \"vertex\", vertex Bind \"color\", color }" +
			    "} } }" );
		    lineMaterial.hideFlags = HideFlags.HideAndDontSave;
		    lineMaterial.shader.hideFlags = HideFlags.HideAndDontSave;
	    }
    }

    void DrawLine(int controlID, Vector3 position, Quaternion rotation, float size)
    {
        CreateLineMaterial();

        lineMaterial.SetPass(0);

        GL.Begin(GL.LINES);
        GL.Color(Handles.color);
        GL.Vertex3(position.x, position.y, position.z);
        Vector3 endPosition = position + size * (rotation * Vector3.forward);
        GL.Vertex3(endPosition.x, endPosition.y, endPosition.z);
        GL.End();
    }
#endif
}
