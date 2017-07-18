using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public static class GUIAreas
{
    [SerializeField]
    private static Dictionary<string, Rect> insideGUIRects = new Dictionary<string, Rect>();
    
    [SerializeField]
    public static Color color = new Color(1f, 0f, 0f, 0.3f); // red highlight

    /// <summary>
    /// Reset the Dictionary. Removes all GUIAras (Rects)
    /// </summary>
    public static void Reset()
    {
        insideGUIRects = new Dictionary<string, Rect>();
    }

    /// <summary>
    /// Adds a Rect to the Dictionary
    /// </summary>
    /// <param name="name">name/key of the Rect</param>
    /// <param name="rect">value - the Rect</param>
    public static void Add(string name, Rect rect)
    {
        insideGUIRects[name] = rect;
    }

    /// <summary>
    /// Removes a Rect from the Dictionary
    /// </summary>
    /// <param name="name">name/key of the Rect</param>
    public static void Remove(string name){
        insideGUIRects.Remove(name);
    }

    /// <summary>
    /// Checks if the mousposition on GUI is inside of one of the GUIAreas
    /// </summary>
    /// <returns></returns>
    public static bool MouseInsideGUIArea()
    {
        return PointInsideGUIArea(InputHelpers.MouseOnGUI());
    }

    /// <summary>
    /// Checks if the mousposition on GUI is outside of the GUIAreas
    /// </summary>
    /// <returns></returns>
    public static bool MouseOutsideGUIArea()
    {
        return !MouseInsideGUIArea();
    }
    
    /// <summary>
    /// Check if a Point is insice of one of the Rects
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    public static bool PointInsideGUIArea(Vector3 point)
    {
        foreach (Rect rect in insideGUIRects.Values)
        {
            if (rect.Contains(point))
                return true;
        }
        return false;
    }
    
    /// <summary>
    /// Draws the GUIAreas with the set Color. Only use in OnGUI()
    /// </summary>
    public static void DrawGUIAreas()
    {
        foreach (Rect rect in insideGUIRects.Values)
        {
            CustomGUIUtils.DrawBox(rect, color);
        }
    }

}

