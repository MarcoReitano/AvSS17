using System.Xml;
using System;
using System.IO;
using System.Collections.Generic;

public class OSMData
{


	public Dictionary<long, OSMNode> nodes = new Dictionary<long, OSMNode>();
	public Dictionary<long, OSMWay> ways = new Dictionary<long, OSMWay>();
	public Dictionary<long, OSMRelation> relations = new Dictionary<long, OSMRelation>();

	public OSMData(Stream osmXMLStream)
	{
		XmlTextReader xmlReader = new XmlTextReader(osmXMLStream);
		xmlReader.MoveToContent();
		while (xmlReader.Read())
		{
			if (xmlReader.NodeType.Equals(XmlNodeType.Element))
			{
				switch (xmlReader.LocalName)
				{
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
	}


	private OSMNode ParseOSMNode(XmlTextReader xmlReader)
	{
		OSMNode node = new OSMNode(long.Parse(xmlReader.GetAttribute("id")), float.Parse(xmlReader.GetAttribute("lat")), float.Parse(xmlReader.GetAttribute("lon")));
		string childs = xmlReader.ReadOuterXml();

		XmlTextReader outerXmlReader = new XmlTextReader(new System.IO.StringReader(childs));
		outerXmlReader.MoveToContent();
		while (outerXmlReader.Read())
		{
			if (outerXmlReader.NodeType.Equals(XmlNodeType.Element) && outerXmlReader.LocalName.Equals("tag"))
			{
				node.AddTag(outerXmlReader.GetAttribute("k"), outerXmlReader.GetAttribute("v"));
			}
		}
		return node;
	}

	private OSMWay ParseOSMWay(XmlTextReader xmlReader)
	{
		OSMWay way = new OSMWay(long.Parse(xmlReader.GetAttribute("id")));

		string childs = xmlReader.ReadOuterXml();

		XmlTextReader outerXmlReader = new XmlTextReader(new System.IO.StringReader(childs));
		outerXmlReader.MoveToContent();
		while (outerXmlReader.Read())
		{
			if (outerXmlReader.NodeType.Equals(XmlNodeType.Element) && outerXmlReader.LocalName.Equals("tag"))
			{
				way.Tags.Add(outerXmlReader.GetAttribute("k"), outerXmlReader.GetAttribute("v"));
			}
			if (outerXmlReader.NodeType.Equals(XmlNodeType.Element) && outerXmlReader.LocalName.Equals("nd"))
			{
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
		while (outerXmlReader.Read())
		{
			if (outerXmlReader.NodeType.Equals(XmlNodeType.Element) && outerXmlReader.LocalName.Equals("tag"))
			{
				relation.Tags.Add(outerXmlReader.GetAttribute("k"), outerXmlReader.GetAttribute("v"));
			}
			if (outerXmlReader.NodeType.Equals(XmlNodeType.Element) && outerXmlReader.LocalName.Equals("member"))
			{
				relation.Members.Add(new OSMMember(outerXmlReader.GetAttribute("type"), long.Parse(outerXmlReader.GetAttribute("ref")), outerXmlReader.GetAttribute("role")));
			}
		}
		return relation;
	}
}
