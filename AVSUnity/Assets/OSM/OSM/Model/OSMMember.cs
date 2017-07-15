using UnityEngine;
#if UNITY_EDITOR
//using UnityEditor;
#endif
using System.Collections;
using System.Text;
using System.Xml;

/// <summary>
/// OSMMember.
/// </summary>
public class OSMMember
{
    public OSMMember(string type, long reference, string role)
    {
        if (type == "node")
            this.type = OSMMemberType.Node;
        else if (type == "way")
            this.type = OSMMemberType.Way;

        this.reference = reference;
        this.role = role;
    }

    public OSMMember()
    {
        // TODO: Complete member initialization
    }

    public OSMMemberType Type
    {
        get { return type; }
        set { type = value; }
    }
    public long Reference
    {
        get { return reference; }
        set { reference = value; }
    }
    public string Role
    {
        get { return role; }
        set { role = value; }
    }

    private OSMMemberType type;
    private long reference;
    private string role;


    /// <summary>
    ///  <member type="way" ref="50905280" role=""/>
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    public static OSMMember ParseRelationMember(XmlNode node)
    {
        OSMMember osmRelationMember = new OSMMember();

        // parse node-attributes
        foreach (XmlAttribute attribute in node.Attributes)
        {
            string name = attribute.Name;
            switch (name)
            {
                case "type":
                    osmRelationMember.Type = (OSMMemberType) int.Parse(attribute.Value);
                    break;
                case "ref":
                    osmRelationMember.Reference = long.Parse(attribute.Value);
                    break;
                case "role":
                    osmRelationMember.Role = attribute.Value;
                    break;
                default:
                    break;
            }
        }



        return osmRelationMember;
    }


    public string GetXMLString()
    {
        StringBuilder sb = new StringBuilder();

        //  <member type="way" ref="8066611" role="forward"/>
        sb.Append("  ");
        sb.Append("<member type=\"").Append(Type).Append("\" ref=\"").Append(Reference).Append("\" role=\"").Append(Role).Append("\"/>\n");

        return sb.ToString();
    }


}
public enum OSMMemberType
{
    Node,
    Way
}