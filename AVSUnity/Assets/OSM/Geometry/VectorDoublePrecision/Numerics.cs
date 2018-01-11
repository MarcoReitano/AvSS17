//using System;


///// <summary>
///// Based on:
///// http://www.mycsharp.de/wbb2/thread.php?threadid=98035
///// </summary>
//public static class Comparison
//{
//    //private static readonly double _epsilon = Math.Pow(2, -52);
//    private static readonly double _epsilon = Math.Pow(2, -6);

//    public static double Epsilon
//    {
//        get { return _epsilon; }
//    }
//    //---------------------------------------------------------------------
//    public static bool IsEqualTo(this double a, double b)
//    {
//        return IsEqualTo(a, b, _epsilon);
//    }
//    //---------------------------------------------------------------------
//    public static bool IsEqualTo(this double a, double b, double eps_a)
//    {
//        if (CheckSpecialCases(a, b)) return true;
//        if (Math.Abs(a - b) < eps_a) return true;

//        if (Math.Abs(b) > Math.Abs(a))
//            return Math.Abs((a - b) / b) < _epsilon;

//        return Math.Abs((a - b) / a) < _epsilon;
//    }
//    //---------------------------------------------------------------------
//    public static bool IsEqualTo(this double a, double b, long precision)
//    {
//        if (CheckSpecialCases(a, b)) return true;

//        if (precision < 0)
//            throw new ArgumentOutOfRangeException("precision");

//        double test = Math.Pow(10d, precision);
//        if (double.IsInfinity(test) || test > long.MaxValue)
//            throw new ArgumentOutOfRangeException("precision");

//        precision = (long)test;
//        return (long)(a * precision) == (long)(b * precision);
//    }
//    //---------------------------------------------------------------------
//    private static bool CheckSpecialCases(double a, double b)
//    {
//        if (double.IsPositiveInfinity(a)) return double.IsPositiveInfinity(b);
//        if (double.IsNegativeInfinity(a)) return double.IsNegativeInfinity(b);
//        if (double.IsNaN(a)) return double.IsNaN(b);
//        if (a == b) return true;

//        return false;
//    }

//    #region ourCases
//    private static bool GreaterThanOne(this double value)
//    {
//        return (value + MathGeometry.Epsilon) > 1D;
//    }

//    private static bool LessThanZero(this double value)
//    {
//        return (value - _epsilon) < 0D;
//    }

//    private static bool IsZero(this double value)
//    {
//        //return Math.Abs(value) < MathGeometry.Epsilon;
//        return value.IsEqualTo(0D);
//    }

//    private static bool IsOne(this double value)
//    {
//        //return Math.Abs(value) < MathGeometry.Epsilon;
//        return value.IsEqualTo(1D);
//    }

//    #endregion // ourCases
//}
