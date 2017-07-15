using System;
using UnityEngine;


/// <summary>
/// 
/// The Golden Ration Class
/// 
/// The Golden Ratio is defined 
///  
///     a+b  is to  a    as   a  is to  b  
/// 
///      o----------a---------o------b-----o
///      |                    |            |
///      |                    |            |
///      |                    |            |
///      |                    |            |
///      a                    |            |
///      |                    |            |
///      |                    |            |
///      |                    |            |
///      o--------------------o------------o
///       \_______________  ______________/
///                       \/
///                 length = a + b
/// 
/// </summary>
public static class GoldenRatio
{
    /// <summary>
    /// phi = 1,6180339887...
    /// </summary>
    public static readonly float phi = (1 + Mathf.Sqrt(5f)) * 0.5f;

    /// <summary>
    /// Phi = 0,6180339887...
    /// </summary>
    public static readonly float Phi = (1 + Mathf.Sqrt(5f)) * 0.5f - 1;



    /// <summary>
    /// 
    /// <para> o--------given-------o------------o </para>
    /// <para> |-----------------?---------------| </para>
    /// 
    /// </summary>
    /// <param name="length"></param>
    /// <returns></returns>
    public static float LengthForLongSide(float longSide)
    {
        return longSide + longSide.ShortSideForLongSideGoldenRatio();
    }

    /// <summary>
    /// <para> o--------given-------o------------o </para>
    /// <para> |-----------------?---------------| </para>
    /// </summary>
    /// <param name="length"></param>
    /// <returns></returns>
    public static float LengthForLongSideGoldenRatio(this float longSide)
    {
        return GoldenRatio.LengthForLongSide(longSide);
    }


    /// <summary>
    /// <para> o--------------------o----given---o </para>
    /// <para> |-----------------?---------------| </para>
    /// </summary>
    /// <param name="length"></param>
    /// <returns></returns>
    public static float LengthForShortSide(float shortSide)
    {
        return shortSide + shortSide.LongSideForShortSideGoldenRatio();
    }

    /// <summary>
    /// <para> o--------------------o----given---o </para>
    /// <para> |-----------------?---------------| </para>
    /// </summary>
    /// <param name="length"></param>
    /// <returns></returns>
    public static float LengthForShortSideGoldenRatio(this float shortSide)
    {
        return GoldenRatio.LengthForShortSide(shortSide);
    }


    /// <summary>
    /// <para> o----------?---------o------------o </para>
    /// <para> |-------------length--------------| </para>
    /// </summary>
    /// <param name="length"></param>
    /// <returns></returns>
    public static float LongSideOf(float length)
    {
        return length * Phi;
    }

    /// <summary>
    /// <para> o----------?---------o------------o </para>
    /// <para> |-------------length--------------| </para>
    /// </summary>
    /// <param name="length"></param>
    /// <returns></returns>
    public static float LongSideOfLengthGoldenRatio(this float length)
    {
        return GoldenRatio.LongSideOf(length);
    }


    /// <summary>
    /// <para> o--------------------o------?-----o </para>
    /// <para> |-------------length--------------| </para>
    /// </summary>
    /// <param name="length"></param>
    /// <returns></returns>
    public static float ShortSideOf(float length)
    {
        return length - LongSideOf(length);
    }


    /// <summary>
    /// <para> o--------------------o------?-----o </para>
    /// <para> |-------------length--------------| </para>
    /// </summary>
    /// <param name="length"></param>
    /// <returns></returns>
    public static float ShortSideOfLengthGoldenRatio(this float length)
    {
        return GoldenRatio.ShortSideOf(length);
    }


    /// <summary>
    /// <para> o----------?---------o----given---o </para>
    /// </summary>
    /// <param name="shortSide"></param>
    /// <returns></returns>
    public static float LongSideFor(float shortSide)
    {
        return shortSide * phi;
    }


    /// <summary>
    /// <para> o----------?---------o----given---o </para>
    /// </summary>
    /// <param name="shortSide"></param>
    /// <returns></returns>
    public static float LongSideForShortSideGoldenRatio(this float shortSide)
    {
        return GoldenRatio.LongSideFor(shortSide);
    }


    /// <summary>
    /// <para> o--------given-------o------?-----o </para>
    /// </summary>
    /// <param name="longSide"></param>
    /// <returns></returns>
    public static float ShortSideFor(float longSide)
    {
        return longSide * Phi;
    }


    /// <summary>
    /// <para> o--------given-------o------?-----o </para>
    /// </summary>
    /// <param name="longSide"></param>
    /// <returns></returns>
    public static float ShortSideForLongSideGoldenRatio(this float longSide)
    {
        return GoldenRatio.ShortSideFor(longSide);
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="side1"></param>
    /// <param name="side2"></param>
    /// <returns></returns>
    public static bool IsGoldenRatio(float side1, float side2)
    {
        // Due to FloatingPointError not even the Golden Ratio Constants result true.  :o/  
        // --> so the approximation is taken everytime
        if (Mathf.Approximately(side1, LongSideFor(side2)))
            return true;
        else if (Mathf.Approximately(side2, LongSideFor(side1)))
            return true;
        else
            return false;
    }

    /// <summary>
    /// Checks the rounded given values for the Golden Ratio
    /// If the Precision of the golden-ratio-matching i
    /// </summary>
    /// <param name="side1"></param>
    /// <param name="side2"></param>
    /// <param name="digits">Number of floating Point digits to taken in account</param>
    /// <returns></returns>
    public static bool IsGoldenRatio(float side1, float side2, int precision)
    {
        if (side1.Round(precision) == LongSideFor(side2).Round(precision))
            return true;
        else if (side2.Round(precision) == LongSideFor(side1).Round(precision))
            return true;
        else
            return false;
    }


    /// <summary>
    /// Crop the Rect to golden ratio (minimizing the area)
    /// </summary>
    /// <param name="rect"></param>
    /// <returns></returns>
    public static Rect CropToGoldenRatio(this Rect rect)
    {
        if (rect.width == rect.height)
            rect.height = rect.width.LongSideOfLengthGoldenRatio();
        else if (rect.width > rect.height)
		    rect.width = rect.height.LengthForLongSideGoldenRatio();
	    else
            rect.height = rect.width.LengthForShortSideGoldenRatio();

        return rect;
    }

    /// <summary>
    /// Extend the Rect to golden ratio (maximizing the area)
    /// </summary>
    /// <param name="rect"></param>
    /// <returns></returns>
    public static Rect ExtendToGoldenRatio(this Rect rect)
    {
        if (rect.width == rect.height)
            rect.width = rect.height.LengthForLongSideGoldenRatio();
        else if (rect.width > rect.height)
            rect.height = rect.width.LongSideOfLengthGoldenRatio();
        else
            rect.width = rect.height.LongSideOfLengthGoldenRatio();

        return rect;
    }


}
