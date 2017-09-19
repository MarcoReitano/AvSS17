using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Math2D
{
    public static bool LineLineIntersection(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, out Vector2 p)
    {
        float denominator = (p1.x - p2.x) * (p3.y - p4.y) - (p1.y - p2.y) * (p3.x - p4.x);
        //if denominator == 0 -> parallel!
        if (denominator <= Mathf.Epsilon && denominator >= -Mathf.Epsilon) // <- wie besser?!
        {
            p = Vector2.zero;
            return false;
        }

        float px = ((p1.x * p2.y - p1.y * p2.x) * (p3.x - p4.x) - (p1.x - p2.x) * (p3.x * p4.y - p3.y * p4.x)) / denominator;
        float py = ((p1.x * p2.y - p1.y * p2.x) * (p3.y - p4.y) - (p1.y - p2.y) * (p3.x * p4.y - p3.y * p4.x)) / denominator;

        p = new Vector2(px, py);
        return true;
    }

    public static bool LineLineIntersection(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4)
    {
        float denominator = (p1.x - p2.x) * (p3.y - p4.y) - (p1.y - p2.y) * (p3.x - p4.x);
        //if denominator == 0 -> parallel!
        if (denominator <= Mathf.Epsilon && denominator >= -Mathf.Epsilon) // <- wie besser?!
        {
            return false;
        }
        return true;
    }

    //Optimise by boundingbox test?
    public static bool SegmentSegmentIntersection(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4)
    {
        float d = (p4.y - p3.y) * (p2.x - p1.x) - (p4.x - p3.x) * (p2.y - p1.y);

        float ua = ((p4.x - p3.x) * (p1.y - p3.y) - (p4.y - p3.y) * (p1.x - p3.x)) / d;
        float ub = ((p2.x - p1.x) * (p1.y - p3.y) - (p2.y - p1.y) * (p1.x - p3.x)) / d;

        if (ua >= 0 && ua <= 1 && ub >= 0 && ub <= 1)
            return true;
        return false;
    }
    public static bool SegmentSegmentIntersection(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, out Vector2 p)
    {
        throw new System.NotImplementedException();
    }

    public static float DistancePointToSegment(Vector2 SegPoint1, Vector2 SegPoint2, Vector2 Point, out float parameter, out Vector2 projectionPoint)
    {
        Vector2 Seg12 = SegPoint2 - SegPoint1;
        float t = Vector2.Dot(Point - SegPoint1, Seg12) / (Seg12).sqrMagnitude;

        if (t <= 0)
        {
            parameter = 0f;
            projectionPoint = SegPoint1;
        }
        else if (t >= 1)
        {
            parameter = 1f;
            projectionPoint = SegPoint2;
        }
        else
        {
            parameter = t;
            projectionPoint = SegPoint1 + t * Seg12;
        }

        return Vector2.Distance(projectionPoint, Point);
    }

    public static bool IsClockwise(Vertex v1, Vertex v2, Vertex v3)
    {
        return IsClockwise(new Vertex[] { v1, v2, v3 });
    }
    public static bool IsClockwise(Vertex[] vertices)
    {
        if (vertices.Length == 0)
            return false;
        float area = vertices[vertices.Length - 1].X * vertices[0].Z - vertices[0].X * vertices[vertices.Length - 1].Z;
        for (int i = 0; i < vertices.Length - 1; i++)
        {
            area += vertices[i].X * vertices[i + 1].Z - vertices[i + 1].X * vertices[i].Z;
        }
        if (area >= 0f)
            return true;
        else
            return false;
    }

    public static bool PointInTriangle(Vertex v1, Vertex v2, Vertex v3, Vertex point)
    {
        //Barycentic
        float Area = 0.5f * (-v2.Z * v3.X + v1.Z * (-v2.X + v3.X) + v1.X * (v2.Z - v3.Z) + v2.X * v3.Z);
        float s = 1f / (2f * Area) * (v1.Z * v3.X - v1.X * v3.Z + (v3.Z - v1.Z) * point.X + (v1.X - v3.X) * point.Z);
        float t = 1f / (2f * Area) * (v1.X * v2.Z - v1.Z * v2.X + (v1.Z - v2.Z) * point.X + (v2.X - v1.X) * point.Z);

        if (s >= 0 && t >= 0 && 1 - s - t >= 0)
            return true;
        return false;
    }

    public static bool AnyPointInTriangle(Vertex v1, Vertex v2, Vertex v3, List<Vertex> points)
    {
        List<Vertex> pointsWithoutTriangle = new List<Vertex>(points);
        pointsWithoutTriangle.Remove(v1);
        pointsWithoutTriangle.Remove(v2);
        pointsWithoutTriangle.Remove(v3);
        for (int i = 0; i < pointsWithoutTriangle.Count; i++)
        {
            if (PointInTriangle(v1, v2, v3, pointsWithoutTriangle[i]))
                return true;
        }
        return false;
    }

}
