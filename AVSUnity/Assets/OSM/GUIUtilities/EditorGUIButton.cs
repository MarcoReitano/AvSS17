using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public enum ButtonType
{
    Label,
    Icon,
    IconAndLabel
}

public enum LabelPositonType
{
    TopLeft,
    TopCenter,
    TopRight,

    CenterLeft,
    Center,
    CenterRight,

    BottomLeft,
    BottomCenter,
    BottomRight
}


public class EditorGUIButton
{
    [SerializeField]
    private string label;
    public string Label
    {
        get { return label; }
        set { label = value; }
    }

    [SerializeField]
    private Texture2D icon;
    public Texture2D Icon
    {
        get { return icon; }
        set { icon = value; }
    }

    [SerializeField]
    private bool enabled;
    public bool Enabled
    {
        get { return enabled; }
        set { enabled = value; }
    }

    [SerializeField]
    private LabelPositonType labelPosition;
    public LabelPositonType LabelPosition
    {
        get { return labelPosition; }
        set { labelPosition = value; }
    }


    [SerializeField]
    private Rect rect = new Rect();
    public Rect Rect
    {
        get { return rect; }
        set { rect = value; }
    }

    public float X
    {
        get { return rect.x; }
        set { rect.x = value; }
    }

    public float y
    {
        get { return rect.y; }
        set { rect.y = value; }
    }


    [SerializeField]
    public float Width
    {
        get { return rect.width; }
        set { rect.width = value; }
    }

    [SerializeField]
    public float Height
    {
        get { return rect.height; }
        set { rect.height = value; }
    }


    public EditorGUIButton()
    {
        this.label = "Do Something";
        this.icon = null;

    }

    public EditorGUIButton(string label)
    {
        this.label = label;
    }

    public EditorGUIButton(string label, LabelPositonType labelPosition)
        : this(label)
    {
        this.labelPosition = labelPosition;
    }

    public EditorGUIButton(string label, Texture2D icon)
        : this(label)
    {
        this.icon = icon;
    }

    public EditorGUIButton(string label, Texture2D icon, LabelPositonType labelPosition)
        : this(label, icon)
    {
        this.labelPosition = labelPosition;
    }

    public void Draw()
    {
        GUI.Button(this.rect, this.label);
 
        // hmm... how can i put this delicately...  ?   this does not the trick...  should return a bool...
    }

}

