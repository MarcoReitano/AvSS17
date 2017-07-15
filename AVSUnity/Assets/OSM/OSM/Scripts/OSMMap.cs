using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml;

using UnityEngine;
using System.Collections.Generic;


    /// <summary>
    ///  
    /// <?xml version='1.0' encoding='UTF-8'?>
    /// <osm version="0.6" generator="Osmosis 0.32">
    /// <bound box="50.32279,5.86674,51.25040,7.79179" origin="http://www.openstreetmap.org/api/0.6"/>
    /// </summary>
    public class OSMMap
    {

        private string xmlVersion;
        public string XmlVersion
        {
            get { return xmlVersion; }
            set { xmlVersion = value; }
        }

        private string encoding;
        public string Encoding
        {
            get { return encoding; }
            set { encoding = value; }
        }

        private string osmVersion;
        public string OsmVersion
        {
            get { return osmVersion; }
            set { osmVersion = value; }
        }
         
        private string generator;
        public string Generator
        {
            get { return generator; }
            set { generator = value; }
        }
         
        private string origin;
        public string Origin
        {
            get { return origin; }
            set { origin = value; }
        }


        public double Minlat // south
        {
            get { return box.South; }
            set { box.South = value; }
        }

        public double Minlon // west
        {
            get { return box.West; }
            set { box.West = value; }
        }

        public double Maxlat // north
        {
            get { return box.North; }
            set { box.North = value; }
        }

        public double Maxlon // east
        {
            get { return box.East; }
            set { box.East = value; }
        }


        private MapBounds box;
        public MapBounds Box
        {
            get { return this.box; }
            set { box = value; }
        }

        private Dictionary<long, OSMNode> nodes;
        public Dictionary<long, OSMNode> Nodes
        {
            get { return nodes; }
            set { nodes = value; }
        }



        private Dictionary<long, OSMWay> ways;
        public Dictionary<long, OSMWay> Ways
        {
            get { return ways; }
            set { ways = value; }
        }

        private Dictionary<long, OSMRelation> relations;
        internal Dictionary<long, OSMRelation> Relations
        {
            get { return relations; }
            set { relations = value; }
        }


        public OSMMap()
        {
            this.XmlVersion = "1.0";
            this.Encoding = "UTF-8";
            this.OsmVersion = "0.6";
            this.Generator = "CITY-OSM-Parser";
            //this.Box = ScriptableObject.CreateInstance<MapBounds>();
            this.Box = new MapBounds();

            this.Origin = "http://www.openstreetmap.org/api/0.6";
            nodes = new Dictionary<long, OSMNode>();
            ways = new Dictionary<long, OSMWay>();
            relations = new Dictionary<long, OSMRelation>();
        }


        public OSMMap(
            string xmlVersion,
            string encoding,
            string osmVersion,
            string generator,
            MapBounds box,
            string origin)
        {
            this.XmlVersion = xmlVersion;
            this.Encoding = encoding;
            this.OsmVersion = osmVersion;
            this.Generator = generator;
            this.Box = box;
            this.Origin = origin;

            nodes = new Dictionary<long, OSMNode>();
            ways = new Dictionary<long, OSMWay>();
            relations = new Dictionary<long, OSMRelation>();
        }


        public void AddNode(OSMNode node)
        {
            if (node != null)
                if (!nodes.Keys.Contains(node.Id))
                    nodes.Add(node.Id, node);
        }

        public void AddWay(OSMWay way)
        {
            if (way != null)
                if (!ways.Keys.Contains(way.Id))
                    ways.Add(way.Id, way);
        }

        public void AddRelation(OSMRelation relation)
        {
            if (relation != null)
                if (!relations.Keys.Contains(relation.Id))
                    relations.Add(relation.Id, relation);
        }

        public List<MapBounds> boundsList = new List<MapBounds>();
        internal void AddBounds(MapBounds bounds)
        {
            //this.boundsList.Add(bounds);
            Debug.Log(bounds);
            this.box = MapBounds.MaxBounds(this.box, bounds);
        }

        public XmlDocument GetXMLDocument()
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(GetXMLString());
            return doc;
        }





        public string GetXMLString()
        {
            StringBuilder sb = new StringBuilder();

            // Create OSM-Header
            //<?xml version='1.0' encoding='UTF-8'?>
            //<osm version="0.6" generator="Osmosis 0.32">
            //    <bound box="50.32279,5.86674,51.25040,7.79179" origin="http://www.openstreetmap.org/api/0.6"/>

            sb.Append("<?xml version='").Append(xmlVersion).Append("' encoding='").Append(encoding).Append("'?>\n");
            sb.Append("<osm version=\"").Append(osmVersion).Append("\" generator=\"").Append(generator).Append("\">\n");
            
            /* // Old Version... but still API 0.6
            sb.Append("  ").Append("<bound box=\"");
            sb.Append(box.North).Append(',');
            sb.Append(box.West).Append(',');
            sb.Append(box.South).Append(',');
            sb.Append(box.East);
            sb.Append("\" origin=\"").Append(origin).Append("\"/>");
            */

            sb.Append("  ").Append("<bounds ");
            sb.Append(" minlat=\"").Append(box.South.ToString("0.#######", CultureInfo.CreateSpecificCulture("en-US"))).Append("\" ");
            sb.Append(" minlon=\"").Append(box.West.ToString("0.#######", CultureInfo.CreateSpecificCulture("en-US"))).Append("\" ");
            sb.Append(" maxlat=\"").Append(box.North.ToString("0.#######", CultureInfo.CreateSpecificCulture("en-US"))).Append("\" ");
            sb.Append(" maxlon=\"").Append(box.East.ToString("0.#######", CultureInfo.CreateSpecificCulture("en-US"))).Append("\"/>\n");
             
            // Get all the OSMNode Strings
            foreach (OSMNode node in nodes.Values)
            {
                sb.Append(node.GetXMLString());
            }

            // Get all the OSMWay Strings
            foreach (OSMWay way in ways.Values)
            {
                sb.Append(way.GetXMLString());
            }

            // Get all the OSMRelation Strings
            foreach (OSMRelation relation in relations.Values)
            {
                sb.Append(relation.GetXMLString());
            }

            // Close the Document
            sb.Append("</osm>\n");

            return sb.ToString();
        }


        public static OSMMap ParseMap(XmlDocument doc, ref XmlTextReader xmlReader, ref OSMMap osmMap)
        {
            //osmMap = new OSMMap();

            XmlNode rootnode = doc.ReadNode(xmlReader);
            OSMMap.ParseOSMHeader(rootnode, ref osmMap);

            int nodeCount = 0;
            int wayCount = 0;
            int relationCount = 0;

            foreach (XmlNode node in rootnode.ChildNodes)
            {
                switch (node.Name)
                {
                    case "bounds":
                        osmMap.AddBounds(OSMMap.ParseBounds(node, ref osmMap));
                        break;
                    case "node":
                        nodeCount++;
                        osmMap.AddNode(OSMNode.ParseNode(node));
                        break;
                    case "way":
                        wayCount++;
                        osmMap.AddWay(OSMWay.ParseWay(node, ref osmMap));
                        break;
                    case "relation":
                        relationCount++;
                        osmMap.AddRelation(OSMRelation.ParseRelation(node, ref osmMap));
                        break;
                    default:
                        break;
                }
            }
            //Console.WriteLine("nodeCount = {0} = {1}", nodeCount, osmMap.nodes.Count);
            //Console.WriteLine("wayCount = {0} = {1}", wayCount, osmMap.ways.Count);
            //Console.WriteLine("relationCount = {0} = {1}", relationCount, osmMap.relations.Count);

            return osmMap;
        }

        public static MapBounds ParseBounds(XmlNode node, ref OSMMap map)
        {
            MapBounds bounds = new MapBounds();
            foreach (XmlAttribute attribute in node.Attributes)
            {
                String name = attribute.Name;
                switch (name)
                {
                    case "minlat": // south
                        bounds.South = float.Parse(attribute.Value, CultureInfo.CreateSpecificCulture("en-US"));
                        break;
                    case "minlon": // west
                        bounds.West = float.Parse(attribute.Value, CultureInfo.CreateSpecificCulture("en-US"));
                        break;
                    case "maxlat": // north
                        bounds.North = float.Parse(attribute.Value, CultureInfo.CreateSpecificCulture("en-US"));
                        break;
                    case "maxlon": // east
                        bounds.East = float.Parse(attribute.Value, CultureInfo.CreateSpecificCulture("en-US"));
                        break;
                    case "box":
                        string tmp = attribute.Value;
                        string[] splitStr = tmp.Split(',');
                        bounds.South = float.Parse(splitStr[0], CultureInfo.CreateSpecificCulture("en-US")); //south
                        bounds.East = float.Parse(splitStr[1], CultureInfo.CreateSpecificCulture("en-US")); //east
                        bounds.North = float.Parse(splitStr[2], CultureInfo.CreateSpecificCulture("en-US")); //north
                        bounds.West = float.Parse(splitStr[3], CultureInfo.CreateSpecificCulture("en-US")); //west
                        break;
                    case "origin":
                        map.Origin = attribute.Value;
                        break;
                    default:
                        break;
                }
            }
            return bounds;
        }

        private static void ParseOSMHeader(XmlNode node, ref OSMMap map)
        {
            foreach (XmlAttribute attribute in node.Attributes)
            {
                String name = attribute.Name;
                switch (name)
                {
                    case "version":
                        map.OsmVersion = attribute.Value;
                        break;
                    case "generator":
                        map.Generator = attribute.Value;
                        break;
                    default:
                        break;
                }
            }
        }


        //public void StitchImages()
        //{
        //    Texture texture = new Texture();
        //    texture.
        //    Bitmap bitmap_Bild1 = new Bitmap("1.jpg");
        //    Bitmap bitmap_Bild2 = new Bitmap("2.jpg");
        //    Bitmap bitmap_Bild3 = new Bitmap("3.jpg");
        //    Bitmap bitmap_Bild4 = new Bitmap("4.jpg");

        //    Bitmap bitmap_GesamtBild = new Bitmap(bitmap_Bild1.Width + bitmap_Bild2.Width, bitmap_Bild1.Height + bitmap_Bild3.Height);
        //    Graphics g = Graphics.FromImage(bitmap_GesamtBild);
        //    g.DrawImage(bitmap_Bild1, new Rectangle(0, 0, bitmap_Bild1.Width, bitmap_Bild1.Height));
        //    g.DrawImage(bitmap_Bild2, new Rectangle(bitmap_Bild1.Width, 0, bitmap_Bild2.Width, bitmap_Bild2.Height));
        //    g.DrawImage(bitmap_Bild3, new Rectangle(0, bitmap_Bild1.Height, bitmap_Bild3.Width, bitmap_Bild3.Height));
        //    g.DrawImage(bitmap_Bild4, new Rectangle(bitmap_Bild1.Width, bitmap_Bild1.Height, bitmap_Bild4.Width, bitmap_Bild4.Height));

        //    bitmap_GesamtBild.Save("neuesbild.png", System.Drawing.Imaging.ImageFormat.Png);
        //}



    }

