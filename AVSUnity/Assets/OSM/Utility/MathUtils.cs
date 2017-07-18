using UnityEngine;
using System.Collections;

public static class MathUtils
{

    /// <summary>
    /// 
    /// </summary>
    /// <param name="vector"></param>
    /// <returns></returns>
    public static Vector3 GetOrthogonalNormalizedVector(Vector3 vector)
    {
        Vector3 normal = vector;
        Vector3 zielVect = Vector3.zero;
        Vector3 tangent = Vector3.up;

        Vector3.OrthoNormalize(ref normal, ref tangent, ref zielVect);
        return zielVect;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <returns></returns>
    public static Vector3 GetOrthogonalNormalizedVector(Vector3 start, Vector3 end)
    {
        Vector3 normal = end - start;
        Vector3 zielVect = Vector3.zero;
        Vector3 tangent = Vector3.up;

        Vector3.OrthoNormalize(ref normal, ref tangent, ref zielVect);
        return zielVect;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="edge"></param>
    /// <returns></returns>
    public static Vector3 GetOrthogonalNormalizedVector(Edge edge)
    {
        return GetOrthogonalNormalizedVector(edge.From.Position, edge.To.Position);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="vector"></param>
    /// <returns></returns>
    public static Vector3 GetOrthogonalNormalizedVectorUp(Vector3 vector)
    {
        Vector3 normal = vector;
        Vector3 zielVect = Vector3.zero;
        Vector3 tangent = GetOrthogonalNormalizedVector(vector);

        Vector3.OrthoNormalize(ref normal, ref tangent, ref zielVect);
        return -zielVect;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <returns></returns>
    public static Vector3 GetOrthogonalNormalizedVectorUp(Vector3 start, Vector3 end)
    {
        Vector3 normal = end - start;
        Vector3 zielVect = Vector3.zero;
        Vector3 tangent = GetOrthogonalNormalizedVector(start, end);

        Vector3.OrthoNormalize(ref normal, ref tangent, ref zielVect);
        return -zielVect;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="edge"></param>
    /// <returns></returns>
    public static Vector3 GetOrthogonalNormalizedVectorUp(Edge edge)
    {
        return GetOrthogonalNormalizedVectorUp(edge.From.Position, edge.To.Position);
    }



    /// <summary>
    /// 
    /// </summary>
    /// <param name="NodeBehaviour"></param>
    /// <returns></returns>
    public static float AngleToZAxis(Edge edge)
    {
        return AngleToZAxis(edge, edge.From);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="NodeBehaviour"></param>
    /// <returns></returns>
    public static float AngleToZAxis(Edge edge, Node node)
    {
        if (node == edge.From)
            return AngleToZAxis(edge.GetLookDirectionFromNode(edge.From));
        else if (node == edge.To)
            return AngleToZAxis(edge.GetLookDirectionFromNode(edge.To));

        return float.NaN;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="targetDir"></param>
    /// <returns></returns>
    public static float AngleToZAxis(Vector3 targetDir)
    {
        if (targetDir.x >= 0)
            return Vector3.Angle(targetDir, Vector3.forward);
        else
            return 360f - Vector3.Angle(targetDir, Vector3.forward);
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="targetDir"></param>
    /// <returns></returns>
    public static float AngleToYAxis(Vector2 targetDir)
    {
        if (targetDir.x >= 0)
            return Vector2.Angle(targetDir, Vector2.up);
        else
            return 360f - Vector2.Angle(targetDir, Vector2.up);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    private static Vector2 Min(Vector2 a, Vector2 b)
    {
        if (a.x <= b.x)
            if (a.y <= b.y)
                return a;
            else
                return b;
        else
            if (a.y <= b.y)
                return a;
            else
                return b;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    private static Vector2 Max(Vector2 a, Vector2 b)
    {
        if (a.x <= b.x)
            if (a.y <= b.y)
                return b;
            else
                return a;
        else
            if (a.y <= b.y)
                return b;
            else
                return a;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="t"></param>
    /// <returns></returns>
    public static Vector3 PointOnLine(Vector3 start, Vector3 end, float t)
    {
        if (t < 0f)
            return start;

        if (t > 1f)
            return end;

        return (1f - t) * start + t * end;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="t"></param>
    /// <returns></returns>
    public static Vector3 PointOnRay(Vector3 start, Vector3 end, float t)
    {
        return (1f - t) * start + t * end;
    }




    /// <summary>
    /// 
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="t"></param>
    /// <param name="width"></param>
    /// <returns></returns>
    public static Vector3 PointOnLine(Vector3 start, Vector3 end, float t, float width)
    {
        Vector3 pointOnLine = PointOnLine(start, end, t);
        Vector3 direction = (end - start).normalized;

        return pointOnLine + (direction * width);
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="t"></param>
    /// <param name="width"></param>
    /// <returns></returns>
    public static Vector3 PointOnRay(Vector3 start, Vector3 end, float t, float width)
    {
        Vector3 pointOnLine = PointOnRay(start, end, t);
        Vector3 direction = (end - start).normalized;

        return pointOnLine + (direction * width);
    }


    /// <summary>
    /// http://softsurfer.com/Archive/algorithm_0102/algorithm_0102.htm#Distance%20to%20Parametric%20Line
    /// </summary>
    /// <param name="point">Point from where the Point with the minimal distance is calculated.</param>
    /// <returns>Point on the line with shortest distance to the given point.</returns>
    public static Vector3 GetMinDistancePointOnLine(Vector3 P0, Vector3 P1, Vector3 point)
    {
        Vector3 v = P1 - P0;
        Vector3 w = point - P0;

        float t = Vector3.Dot(w, v) / Vector3.Dot(v, v);

        return MathUtils.PointOnLine(P0, P1, t);
    }

    /// <summary>
    /// http://softsurfer.com/Archive/algorithm_0102/algorithm_0102.htm#Distance%20to%20Parametric%20Line
    /// </summary>
    /// <param name="point">Point from where the Point with the minimal distance is calculated.</param>
    /// <returns>Point on the line with shortest distance to the given point.</returns>
    public static Vector3 GetMinDistancePointOnLine(Vector3 P0, Vector3 P1, Vector3 point, out float t)
    {
        Vector3 v = P1 - P0;
        Vector3 w = point - P0;

        t = Vector3.Dot(w, v) / Vector3.Dot(v, v);

        return MathUtils.PointOnLine(P0, P1, t);
    }


    /// <summary>
    /// Based on:
    /// http://softsurfer.com/Archive/algorithm_0102/algorithm_0102.htm#Distance%20to%20Parametric%20Line
    /// but this time on a ray
    /// </summary>
    /// <param name="point">Point from where the Point with the minimal distance is calculated.</param>
    /// <returns>Point on the line with shortest distance to the given point.</returns>
    public static Vector3 GetMinDistancePointOnRay(Vector3 P0, Vector3 P1, Vector3 point)
    {
        Vector3 v = P1 - P0;
        Vector3 w = point - P0;

        float t = Vector3.Dot(w, v) / Vector3.Dot(v, v);

        return MathUtils.PointOnRay(P0, P1, t);
    }

    /// <summary>
    /// http://softsurfer.com/Archive/algorithm_0102/algorithm_0102.htm#Distance%20to%20Parametric%20Line
    /// </summary>
    /// <param name="point">Point from where the Point with the minimal distance is calculated.</param>
    /// <returns>Point on the line with shortest distance to the given point.</returns>
    public static float GetMinDistancePointOnLineParameter(Vector3 P0, Vector3 P1, Vector3 point)
    {
        Vector3 v = P1 - P0;
        Vector3 w = point - P0;

        float t = Vector3.Dot(w, v) / Vector3.Dot(v, v);

        return t;
    }


    /// <summary>
    /// Based on 
    /// http://softsurfer.com/Archive/algorithm_0102/algorithm_0102.htm#Distance%20to%20Parametric%20Line
    /// </summary>
    /// <param name="point">Point from where the Point with the minimal distance is calculated.</param>
    /// <returns>Point on the line with shortest distance to the given point.</returns>
    public static float GetMinDistancePointOnRayParameter(Vector3 P0, Vector3 P1, Vector3 point)
    {
        Vector3 v = P1 - P0;
        Vector3 w = point - P0;

        float t = Vector3.Dot(w, v) / Vector3.Dot(v, v);

        return t;
    }



    /// <summary>
    /// Converts Vector3 to Vector2 ignoring y-value
    /// </summary>
    /// <param name="vect3"></param>
    /// <returns></returns>
    public static Vector2 Vector3ToVector2IgnoringY(Vector3 vect3)
    {
        return new Vector2(vect3.x, vect3.z);
    }


    /// <summary>
    /// Converts Vector2 to Vector3 with (vect3.y = 0.0f) and (vect3.z = vect2.y)
    /// </summary>
    /// <param name="vect2"></param>
    /// <returns></returns>
    public static Vector3 Vector2ToVector3XZ(Vector2 vect2)
    {
        return new Vector3(vect2.x, 0.0f, vect2.y);
    }


    /// <summary>
    /// Converts Vector2 to Vector3 with (vect3.y = 0.0f) and (vect3.z = vect2.y)
    /// </summary>
    /// <param name="vect2"></param>
    /// <returns></returns>
    public static Vector3 Vector2ToVector3WithHeight(Vector2 vect2, float height)
    {
        return new Vector3(vect2.x, height, vect2.y);
    }


    /// <summary>
    /// Custom Rounding Method
    /// 
    /// </summary>
    /// <param name="toRound"></param>
    /// <param name="digits">Floating point digits to round to </param>
    /// <returns></returns>
    public static float Round(this float toRound, int digits)
    {
        //return (float)System.Math.Round(toRound, digits, System.MidpointRounding.ToEven);

        float factor = Mathf.Pow(10, -digits);
        return  Mathf.RoundToInt(toRound / factor) * factor;
    }

    /// <summary>
    /// Custom Rounding Method
    /// 
    /// </summary>
    /// <param name="toRound"></param>
    /// <param name="digits">Floating point digits to round to </param>
    /// <returns></returns>
    public static int Round(this int toRound, int digits)
    {
        //return (float)System.Math.Round(toRound, digits, System.MidpointRounding.ToEven);

        float factor = Mathf.Pow(10, -digits);
        return (int) (Mathf.RoundToInt(toRound / factor) * factor);
    }


    public static Vector3 PointOnCircle(Vector3 center,  float radius, float angleDegree)
    {
        return new Vector3(
                Mathf.Cos(angleDegree * Mathf.Deg2Rad) * radius + center.x,
                Mathf.Sin(angleDegree * Mathf.Deg2Rad) * radius + center.y,
                0f);
    }


    public static Vector3 PointOnCircleXZ(Vector3 center, float radius, float angleDegree)
    {
        return new Vector3(
                Mathf.Cos(angleDegree * Mathf.Deg2Rad) * radius + center.x,
                center.y,
                Mathf.Sin(angleDegree * Mathf.Deg2Rad) * radius + center.z);
    }



    public static bool AlmostEqual(float a, float b, float epsilon)
    {
        return Mathf.Abs(a - b) < epsilon;
    }

    //public static bool AlmostEqual(this float a, float b, float epsilon)
    //{
    //    return Mathf.Abs(a - b) < epsilon;
    //}


    public static bool AlmostEqual(Vector3 a, Vector3 b, float epsilon)
    {
        if (Mathf.Abs(a.x - b.x) < epsilon)
        {
            if (Mathf.Abs(a.x - b.x) < epsilon)
            {
                if (Mathf.Abs(a.x - b.x) < epsilon)
                {
                    return true;
                }
            }
        }
                
        return false;
    }


    public static bool Between(this float value, float start, float end)
    {
        if (value >= start && value <= end)
            return true;
        return false;
    }


    public static float ParameterRay(float start, float end, float value)
    {
        return GetMinDistancePointOnRayParameter(start.ToVector3X(), end.ToVector3X(), value.ToVector3X());
    }

    public static float ParameterLine(float start, float end, float value)
    {
        return GetMinDistancePointOnLineParameter(start.ToVector3X(), end.ToVector3X(), value.ToVector3X());
    }


    public static bool CheckPointInCircleGUI(Vector3 pointPosition, Vector2 circlePosition, float radius)
    {
        if ((pointPosition.x - circlePosition.x) * (pointPosition.x - circlePosition.x) + (pointPosition.y - circlePosition.y) * (pointPosition.y - circlePosition.y) < (radius * radius))
            return true;

        return false;
    }



    public static Vector2 ToVector2X(this float point)
    {
        return new Vector2(point, 0f);
    }

    public static Vector2 ToVector2Y(this float point)
    {
        return new Vector2(0f, point);
    }

    public static Vector3 ToVector3X(this float point)
    {
        return new Vector3(point, 0f, 0f);
    }

    public static Vector3 ToVector3Y(this float point)
    {
        return new Vector3(0f, point, 0f);
    }

    public static Vector3 ToVector3Z(this float point)
    {
        return new Vector3(0f, 0f, point);
    }



    public static Vector2 ResetX(this Vector2 vector)
    {
        return new Vector2(0f, vector.y);
    }

    public static Vector2 ResetY(this Vector2 vector)
    {
        return new Vector2(vector.x, 0f);
    }

    
    public static Vector3 ResetX(this Vector3 vector)
    {
        return new Vector3(0f, vector.y, vector.z);
    }

    public static Vector3 ResetY(this Vector3 vector)
    {
        return new Vector3(vector.x, 0f, vector.z);
    }

    public static Vector3 ResetZ(this Vector3 vector)
    {
        return new Vector3(vector.x, vector.y, 0f);
    }

    
    public static Vector3 ResetXY(this Vector3 vector)
    {
        return new Vector3(0f, 0f, vector.z);
    }

    public static Vector3 ResetYZ(this Vector3 vector)
    {
        return new Vector3(vector.x, 0f, 0f);
    }

    public static Vector3 ResetXZ(this Vector3 vector)
    {
        return new Vector3(0f, vector.y, 0f);
    }


    public static float Gon2Degree(float angleInGon)
    {
        return angleInGon * 0.9f;
    }
}
