using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;


public class DropArea
{

    private string id;
    public string Id
    {
        get { return id; }
        set { id = value; }
    }


    private Rect rect;
    public Rect Rect
    {
        get { return rect; }
        set { rect = value; }
    }

    private List<Type> types = new List<Type>();
    public List<Type> Types
    {
        get { return types; }
        set { types = value; }
    }

    public bool ContainsMouse()
    {
        return Rect.Contains(InputHelpers.MouseOnGUI());
    }




    private object obj;
    public object Object
    {
        get { return obj; }
        set
        {
            if (IsValidType(value) || value == null)
                obj = value;
        }
    }

    public V GetObject<V>() where V : class
    {
        if (obj != null)
        {
            if (obj.GetType().IsSubclassOf(typeof(V)) || obj.GetType() == typeof(V))
            {
                return (V)obj;
            }
        }

        return (V)null;
    }

    private bool IsValidType(object value)
    {
        bool valid = false;
        if (value == null)
            return false;

        foreach (Type type in types)
        {
            if (value.GetType().IsSubclassOf(type) || value.GetType() == type)
            {
                valid = true;
                break;
            }
        }

        return valid;
    }

    public DropArea(string id, object obj, Rect rect, params Type[] types)
    {
        this.id = id;
        this.rect = rect;
        this.types.AddRange(types);
        this.Object = obj;
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("DropArea ("+this.id+") [");
        sb.AppendLine("\tEmpty: " + (this.obj == null));
        sb.AppendLine("\tTypes: ");
        foreach (Type type in this.types)
        {
            sb.AppendLine("\t\t" + type.ToString());
        }
        return sb.ToString();
    }
}
