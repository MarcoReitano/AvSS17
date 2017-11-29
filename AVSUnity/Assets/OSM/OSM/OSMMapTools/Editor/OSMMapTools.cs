
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;

/// <summary>
/// OSMColors.
/// </summary>

[InitializeOnLoad]
public static class OSMMapTools
{
    static OSMMapTools()
    {
        defaultColor = HasColorKey("OSMdefaultColor") ? GetColor("OSMdefaultColor") : SetColor("OSMdefaultColor", defaultColor);
        highwayColor = HasColorKey("OSMhighwayColor") ? GetColor("OSMhighwayColor") : SetColor("OSMhighwayColor", highwayColor);
        highwayFootwayColor = HasColorKey("OSMhighwayFootwayColor") ? GetColor("OSMhighwayFootwayColor") : SetColor("OSMhighwayFootwayColor", highwayFootwayColor);
        highwayServiceColor = HasColorKey("OSMhighwayServiceColor") ? GetColor("OSMhighwayServiceColor") : SetColor("OSMhighwayServiceColor", highwayServiceColor);
        highwaySecondaryColor = HasColorKey("OSMhighwaySecondaryColor") ? GetColor("OSMhighwaySecondaryColor") : SetColor("OSMhighwaySecondaryColor", highwaySecondaryColor);
        highwayTertiaryColor = HasColorKey("OSMhighwayTertiaryColor") ? GetColor("OSMhighwayTertiaryColor") : SetColor("OSMhighwayTertiaryColor", highwayTertiaryColor);
        buildingColor = HasColorKey("OSMbuildingColor") ? GetColor("OSMbuildingColor") : SetColor("OSMbuildingColor", buildingColor);
        waterwayColor = HasColorKey("OSMwaterwayColor") ? GetColor("OSMwaterwayColor") : SetColor("OSMwaterwayColor", waterwayColor);

        highlightColor = HasColorKey("OSMhighlightColor") ? GetColor("OSMhighlightColor") : SetColor("OSMhighlightColor", highlightColor);

        if(EditorPrefs.HasKey("OSMKeySearchTerm"))
            keySearchTerm = EditorPrefs.GetString("OSMKeySearchTerm");
        if (EditorPrefs.HasKey("OSMValueSearchTerm"))
            valueSearchTerm = EditorPrefs.GetString("OSMValueSearchTerm");
    }

    public static Color DefaultColor
    {
        get { return defaultColor; }
        set
        {
            if (defaultColor != value)
            {
                defaultColor = value;
                SetColor("OSMdefaultColor", value);
                SceneView.RepaintAll();
            }
        }
 
    }
    public static Color HighwayColor
    {
        get { return highwayColor; }
        set 
        {
            if (highwayColor != value)
            {
                highwayColor = value;
                SetColor("OSMhighwayColor", value);
                SceneView.RepaintAll();
            }
        }
    }
    public static Color HighwayFootwayColor
    {
        get { return highwayFootwayColor; }
        set
        {
            if (highwayFootwayColor != value)
            {
                highwayFootwayColor = value;
                SetColor("OSMhighwayFootwayColor", value);
                SceneView.RepaintAll();
            }
        }
    }
    public static Color HighwayServiceColor
    {
        get { return highwayServiceColor; }
        set
        {
            if (highwayServiceColor != value)
            {
                highwayServiceColor = value;
                SetColor("OSMhighwayServiceColor", value);
                SceneView.RepaintAll();
            }
        }
    }
    public static Color HighwaySecondaryColor
    {
        get { return highwaySecondaryColor; }
        set
        {
            if (highwaySecondaryColor != value)
            {
                highwaySecondaryColor = value;
                SetColor("OSMhighwaySecondaryColor", value);
                SceneView.RepaintAll();
            }
        }
    }
    public static Color HighwayTertiaryColor
    {
        get { return highwayTertiaryColor; }
        set
        {
            if (highwayTertiaryColor != value)
            {
                highwayTertiaryColor = value;
                SetColor("OSMhighwayTertiaryColor", value);
                SceneView.RepaintAll();
            }
        }
    }
    public static Color BuildingColor
    {
        get { return buildingColor; }
        set
        {
            if (buildingColor != value)
            {
                buildingColor = value;
                SetColor("OSMbuildingColor", value);
                SceneView.RepaintAll();
            }
        }
    }
    public static Color WaterwayColor
    {
        get { return waterwayColor; }
        set
        {
            if (waterwayColor != value)
            {
                waterwayColor = value;
                SetColor("OSMwaterwayColor", value);
                SceneView.RepaintAll();
            }
        }
    }

    public static Color HighlightColor
    {
        get { return highlightColor; }
        set 
        {
            if (highlightColor != value)
            {
                highlightColor = value;
                SetColor("OSMhighlightColor", value);
                SceneView.RepaintAll();
            }
 
        }
    }

    private static Color defaultColor = Color.gray;
    private static Color highwayColor = Color.green;
    private static Color highwayFootwayColor = Color.magenta;
    private static Color highwayServiceColor = Color.green * 0.5f;
    private static Color highwaySecondaryColor = Color.green * 0.8f;
    private static Color highwayTertiaryColor = Color.green * 0.9f;
    private static Color buildingColor = Color.yellow;
    private static Color waterwayColor= Color.blue;

    private static Color highlightColor = Color.red;

    public static void ResetToDefaultColors()
    {
        defaultColor = SetColor("OSMdefaultColor", Color.gray);
        highwayColor = SetColor("OSMhighwayColor", Color.green);
        highwayFootwayColor = SetColor("OSMhighwayFootwayColor", Color.magenta);
        highwayServiceColor = SetColor("OSMhighwayServiceColor", Color.green * 0.5f);
        highwaySecondaryColor = SetColor("OSMhighwaySecondaryColor", Color.green * 0.8f);
        highwayTertiaryColor = SetColor("OSMhighwayTertiaryColor", Color.green * 0.9f);
        buildingColor = SetColor("OSMbuildingColor", Color.yellow);
        waterwayColor = SetColor("OSMwaterwayColor", Color.blue);

        highlightColor = SetColor("OSMhighlightColor", Color.red);
    }

    static Color SetColor(string name, Color color)
    {
        EditorPrefs.SetFloat(name + "_R", color.r);
        EditorPrefs.SetFloat(name + "_G", color.g);
        EditorPrefs.SetFloat(name + "_B", color.b);
        EditorPrefs.SetFloat(name + "_A", color.a);
        return color;
    }
    static Color GetColor(string name)
    {
        return new Color(
        EditorPrefs.GetFloat(name + "_R", 0f),
        EditorPrefs.GetFloat(name + "_G", 0f),
        EditorPrefs.GetFloat(name + "_B", 0f),
        EditorPrefs.GetFloat(name + "_A", 1f));
    }
    static bool HasColorKey(string name)
    {
        if (EditorPrefs.HasKey(name + "_R") &&
            EditorPrefs.HasKey(name + "_G") &&
            EditorPrefs.HasKey(name + "_B") &&
            EditorPrefs.HasKey(name + "_A"))
            return true;
        else
            return false;
    }

    #region Key Value Search
    public static bool KeySearch
    {
        get { return keySearchTerm != ""; }
    }
    public static bool ValueSearch
    {
        get { return valueSearchTerm != ""; }
    }

    public static string KeySearchTerm
    {
        get { return keySearchTerm; }
        set 
        {
            if (keySearchTerm != value)
            {
                keySearchTerm = value;
                EditorPrefs.SetString("OSMKeySearchTerm", value);
                SceneView.RepaintAll();
            }
        }
    }
    public static string ValueSearchTerm
    {
        get{return valueSearchTerm;}
        set
        {
            if (valueSearchTerm != value)
            {
                valueSearchTerm = value;
                EditorPrefs.SetString("OSMValueSearchTerm", value);
                SceneView.RepaintAll();
            }
        }
    }

    private static string keySearchTerm = "";
    private static string valueSearchTerm = "";
    #endregion

    public static OSMWay SelectedWay;

    public static bool DrawOnTerrainHeight
    {
        get { return drawOnTerrainHeight; }
        set 
        {
            if (drawOnTerrainHeight != value)
            {
                drawOnTerrainHeight = value;
                SceneView.RepaintAll();
            }
        }
    }
    private static bool drawOnTerrainHeight = false;
    public static bool DrawAsHandles
    {
        get { return drawAsHandles; }
        set 
        {
            if (drawAsHandles != value)
            {
                drawAsHandles = value;
                SceneView.RepaintAll();
            }
        }
    }
    private static bool drawAsHandles = false;
}

