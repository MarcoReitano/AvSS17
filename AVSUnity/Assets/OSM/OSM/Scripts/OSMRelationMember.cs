//using System;
//using System.Linq;
//using System.Text;
//using System.Xml;


//public class OSMRelationMember
//{


//    private string type;
//    public string Type
//    {
//        get { return type; }
//        set { type = value; }
//    }

//    private long reference;
//    public long Ref
//    {
//        get { return reference; }
//        set { reference = value; }
//    }


//    private string role;
//    public string Role
//    {
//        get { return role; }
//        set { role = value; }
//    }

//    /// <summary>
//    ///  <member type="way" ref="50905280" role=""/>
//    /// </summary>
//    /// <param name="node"></param>
//    /// <returns></returns>
//    public static OSMRelationMember ParseRelationMember(XmlNode node)
//    {
//        OSMRelationMember osmRelationMember = new OSMRelationMember();

//        // parse node-attributes
//        foreach (XmlAttribute attribute in node.Attributes)
//        {
//            string name = attribute.Name;
//            switch (name)
//            {
//                case "type":
//                    osmRelationMember.Type = attribute.Value;
//                    break;
//                case "ref":
//                    osmRelationMember.Ref = long.Parse(attribute.Value);
//                    break;
//                case "role":
//                    osmRelationMember.Role = attribute.Value;
//                    break;
//                default:
//                    break;
//            }
//        }



//        return osmRelationMember;
//    }


//    public string GetXMLString()
//    {
//        StringBuilder sb = new StringBuilder();

//        //  <member type="way" ref="8066611" role="forward"/>
//        sb.Append("  ");
//        sb.Append("<member type=\"").Append(Type).Append("\" ref=\"").Append(Ref).Append("\" role=\"").Append(Role).Append("\"/>\n");

//        return sb.ToString();
//    }
//}

