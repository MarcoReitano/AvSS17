using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RectExtension
{
    public static bool Overlaps(this Rect a, Rect b)
    {
        return RectanglesOverlap(a, b);
    }
   
    public static bool RectanglesOverlap(Rect a, Rect b)
    {
        if (a.xMin < b.xMax && a.xMax > b.xMin && a.yMin < b.yMax && a.yMax > b.yMin)
            return true;
        return false;
    }


    public static Rect Crop(this Rect a, Rect cropRect)
    {
        Rect result = new Rect(a);
        result.xMin = Math.Max(a.xMin, cropRect.xMin);
        result.xMax = Math.Min(a.xMax, cropRect.xMax);

        result.yMin = Math.Max(a.yMin, cropRect.yMin);
        result.yMax = Math.Min(a.yMax, cropRect.yMax);

        return result;
    }

    public static Rect Crop(this Rect a, Rect cropRect, out Rect parametricRect)
    {
        Rect result = new Rect(a);
        parametricRect = new Rect(0, 0, 1, 1);

        if (a.xMin > cropRect.xMin)
        {
            result.xMin = a.xMin;
            parametricRect.xMin = 0f;
        }
        else
        {
            result.xMin = cropRect.xMin;
            parametricRect.xMin = a.xMin.Distance(cropRect.xMin) / a.width;
        }

        if (a.xMax < cropRect.xMax)
        {
            result.xMax = a.xMax;
            parametricRect.xMax = 1f;
        }
        else
        {
            result.xMax = cropRect.xMax;
            parametricRect.xMax = a.xMin.Distance(cropRect.xMax) / a.width;
        }

        if (a.yMin > cropRect.yMin)
        {
            result.yMin = a.yMin;
            parametricRect.yMin = 0f;
        }
        else
        {
            result.yMin = cropRect.yMin;
            parametricRect.yMin = a.yMin.Distance(cropRect.yMin) / a.height;
        }


        if (a.yMax < cropRect.yMax)
        {
            result.yMax = a.yMax;
            parametricRect.yMax = 1f;
        }
        else
        {
            result.yMax = cropRect.yMax;
            parametricRect.yMax = a.yMin.Distance(cropRect.yMax) / a.height;
        }

        return result;
    }

    public static Texture2D Crop(this Texture2D texture, Rect cropRect)
    {
        int x = Mathf.FloorToInt(texture.width * cropRect.xMin);
        int y = texture.height - Mathf.FloorToInt(texture.height * cropRect.yMax);

        int blockWidth = Mathf.Clamp((int)(texture.width * cropRect.width), 1, texture.width);
        int blockHeight = Mathf.Clamp((int)(texture.height * cropRect.height), 1, texture.height);

        Texture2D result = new Texture2D(blockWidth, blockHeight);
        if (x + blockWidth <= texture.width && y + blockHeight <= texture.height)
        {
            Color[] pixels = texture.GetPixels(x, y, blockWidth, blockHeight);
            if (pixels.Length == blockWidth * blockHeight)
            {
                result.SetPixels(pixels);
                result.Apply();
            }
        }
        return result;
    }

}
