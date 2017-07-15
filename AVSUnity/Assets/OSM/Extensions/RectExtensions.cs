using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class RectExtensions
{
    public static Rect AddOffset(this Rect rect, Rect offsetRect)
    {
        return new Rect(rect.xMin + offsetRect.xMin, rect.yMin + offsetRect.yMin, rect.width, rect.height);
    }
    
   
    public static Rect BoundingRect(Rect rectA, Rect rectB)
    {
        return  Rect.MinMaxRect(
            Mathf.Min(rectA.xMin, rectB.xMin),
            Mathf.Min(rectA.yMin, rectB.yMin),
            Mathf.Max(rectA.xMax, rectB.xMax),
            Mathf.Max(rectA.yMax, rectB.yMax));
    }


    public static Rect BoundingRect(params Vector2[] points)
    {
        return Rect.MinMaxRect(
            Mathf.Min(points.Select(point => point.x).ToArray<float>()),
            Mathf.Min(points.Select(point => point.y).ToArray<float>()),
            Mathf.Max(points.Select(point => point.x).ToArray<float>()),
            Mathf.Max(points.Select(point => point.y).ToArray<float>()));
    }

    public static bool isNull(this Rect rect){
        return rect.Equals(new Rect());
    }


    public static Vector2 Position(this Rect rect)
    {
        return new Vector2(rect.xMin, rect.yMin);
    }

    public static Rect AddFrame(this Rect rect, float frameThickness)
    {
        rect = new Rect(rect.xMin - frameThickness, rect.yMin - frameThickness, rect.width + 2 * frameThickness, rect.height + 2 * frameThickness);
        return rect;
    }
}

