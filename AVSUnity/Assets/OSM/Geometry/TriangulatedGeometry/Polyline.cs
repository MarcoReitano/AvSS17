using UnityEngine;
#if UNITY_EDITOR
//using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

/// <summary>
/// Polyline.
/// </summary>
public class Polyline
{
    public Polyline()
    { }
    public Polyline(params IEnumerable<Vertex>[] vertices)
    {
        for (int i = 0; i < vertices.Length; i++)
        {
            this.vertices.AddRange(vertices[i]);
        }
        calcLength();
    }

    public Polyline LevelUp()
    {
        List<Vertex> leveledVertices = new List<Vertex>();
        float maxHeight = float.MinValue;

        for (int i = 0; i < vertices.Count; i++)
        {
            maxHeight = Mathf.Max(maxHeight, vertices[i].Y);
        }

        for (int i = 0; i < vertices.Count; i++)
        {
            Vector3 position = vertices[i].Position;
            position.y = maxHeight;
            leveledVertices.Add(new Vertex(position));
        }
        return new Polyline(leveledVertices);
    }
    public Polyline LevelDown()
    {
        List<Vertex> leveledVertices = new List<Vertex>();
        float minHeight = float.MaxValue;

        for (int i = 0; i < vertices.Count; i++)
        {
            minHeight = Mathf.Min(minHeight, vertices[i].Y);
        }

        for (int i = 0; i < vertices.Count; i++)
        {
            Vector3 position = vertices[i].Position;
            position.y = minHeight;
            leveledVertices.Add(new Vertex(position));
        }

        return new Polyline(leveledVertices);
    }
    public Polyline Inset(float amount)
    {
        Polyline result = new Polyline();

        //Inset only works for Polylines with at least 2 Vertices
        if (vertices.Count < 2)
            return result;

        //Calculate First Vertex
        Vector2 firstEdgeVector = vertices[0].Position2D - vertices[1].Position2D;

        result.Add(
                new Vertex((vertices[0].Position2D + firstEdgeVector.NormalVector() * amount).ToXZVector3(vertices[0].Y))
                );

        Vector2 edgeVector1, edgeVector2, beforeVertex, beforeVertex2, afterVertex, afterVertex2, intersectionPoint;

        for (int i = 1; i < vertices.Count - 1; i++)
        {
            edgeVector1 = vertices[i - 1].Position2D - vertices[i].Position2D;
            edgeVector2 = vertices[i].Position2D - vertices[i + 1].Position2D;

            beforeVertex = vertices[i - 1].Position2D + edgeVector1.NormalVector() * amount;
            beforeVertex2 = vertices[i].Position2D + edgeVector1.NormalVector() * amount;

            afterVertex = vertices[i].Position2D + edgeVector2.NormalVector() * amount;
            afterVertex2 = vertices[i + 1].Position2D + edgeVector2.NormalVector() * amount;

            if (Math2D.LineLineIntersection(beforeVertex, beforeVertex2, afterVertex, afterVertex2, out intersectionPoint))
            {
                //Intersection found
                //Debug.Log("Intersection");
                result.Add(new Vertex(intersectionPoint.ToXZVector3(vertices[i].Y)));
            }
            else
            {
                //Debug.Log("Parallel?!");
                //Lines are parallel
                result.Add(new Vertex(beforeVertex2.ToXZVector3(vertices[i].Y)));
            }
        }
        //Calculate Last Vertex
        Vector2 lastEdgeVector = vertices[vertices.Count - 2].Position2D - vertices[vertices.Count - 1].Position2D;

        result.Add(
                new Vertex(
                    (vertices[vertices.Count - 1].Position2D
                    + lastEdgeVector.NormalVector() * amount).ToXZVector3(vertices[vertices.Count - 1].Y))
                );

        return result;
    }
    public Polyline Translate(Vector3 extrudeVector)
    {
        List<Vertex> translatedVertices = new List<Vertex>();
        for (int i = 0; i < vertices.Count; i++)
        {
            translatedVertices.Add(new Vertex(vertices[i].Position + extrudeVector));
        }

        return new Polyline(translatedVertices);
    }

    public Polyline InterpolateHeights()
    {
        Polyline result = new Polyline();

        if (vertices.Count == 0)
            return result;

        if (vertices.Count == 1)
        {
            result.Add(new Vertex(vertices[0].Position)); // Clone Vertex?!
            return result;
        }

        float startheight = vertices[0].Y;
        float endHeight = vertices[vertices.Count - 1].Y;

        calcLength();

        List<Vertex> translatedVertices = new List<Vertex>();

        translatedVertices.Add(new Vertex(vertices[0].Position));

        for (int i = 1; i < vertices.Count - 1; i++)
        {
            translatedVertices.Add(
                new Vertex(
                    new Vector3(
                        vertices[i].Position.x,
                        Mathf.Lerp(startheight, endHeight, ParameterAtIndex(i)),
                        vertices[i].Position.z)
                        )
                    );
        }
        translatedVertices.Add(new Vertex(vertices[vertices.Count - 1].Position));

        return new Polyline(translatedVertices);
    }

    public Polyline TrimFront2D(float amount)
    {
        List<Vertex> vertices = new List<Vertex>(this.vertices);
        float dist;

        while (vertices.Count > 2)
        {
            dist = Vector2.Distance(vertices[0].Position2D, vertices[1].Position2D);
            if (amount >= dist)
            {
                vertices.RemoveAt(0);
                amount -= dist;
            }
            else
            {
                break;
            }
        }
        dist = Vector2.Distance(vertices[0].Position2D, vertices[1].Position2D);
        vertices[0] = new Vertex(Vector3.Lerp(vertices[0].Position, vertices[1].Position, amount / dist));
        return new Polyline(vertices);
    }
    public Polyline TrimEnd2D(float amount)
    {
        List<Vertex> vertices = new List<Vertex>(this.vertices);
        float dist;

        while (vertices.Count > 2)
        {
            dist = Vector2.Distance(vertices[vertices.Count - 1].Position2D, vertices[vertices.Count - 2].Position2D);
            if (amount >= dist)
            {
                vertices.RemoveAt(vertices.Count - 1);
                amount -= dist;
            }
            else
            {
                break;
            }
        }
        dist = Vector2.Distance(vertices[vertices.Count - 1].Position2D, vertices[vertices.Count - 2].Position2D);
        vertices[vertices.Count - 1] = new Vertex(Vector3.Lerp(vertices[vertices.Count - 1].Position, vertices[vertices.Count - 2].Position, amount / dist));
        return new Polyline(vertices);

    }

    #region Parametric Stuff
    public List<Vertex> VerticesToParameter(float t)
    {
        calcLength(); // This shouldnt be necessary every time, but if vertex positions are changed, there is no update of the lenght yet (updatesystem?!)
        List<Vertex> result = new List<Vertex>();
        if (length2D <= 0f || vertices.Count == 0)
            return result;

        result.Add(vertices[0]);

        float toLength = t * length2D;

        float counter = 0f;
        float dist = 0f;
        for (int i = 0; i < vertices.Count - 1; i++)
        {
            dist = Vector2.Distance(vertices[i].Position2D, vertices[i + 1].Position2D);

            if ((counter + dist) <= toLength)
            {
                result.Add(vertices[i + 1]);
                counter += dist;
            }
            else
                break;

        }
        return result;
    }
    public List<Vertex> VerticesFromParameter(float t)
    {
        calcLength(); // This shouldnt be necessary every time, but if vertex positions are changed, there is no update of the lenght yet (updatesystem?!)
        List<Vertex> result = new List<Vertex>();
        if (length2D <= 0f || vertices.Count == 0)
            return result;

        result.Add(vertices[vertices.Count - 1]);

        float fromLength = (1f - t) * length2D;

        float counter = 0f;
        float dist = 0f;
        for (int i = vertices.Count - 1; i > 0; i--)
        {
            dist = Vector2.Distance(vertices[i].Position2D, vertices[i - 1].Position2D);

            if ((counter + dist) <= fromLength)
            {
                result.Add(vertices[i - 1]);
                counter += dist;
            }
            else
                break;
        }
        result.Reverse();
        return result;
    }
    public List<Vertex> VerticesBetweenParameter(float startParameter, float endParameter)
    {
        calcLength(); // This shouldnt be necessary every time, but if vertex positions are changed, there is no update of the lenght yet (updatesystem?!)
        List<Vertex> result = new List<Vertex>();
        if (length2D <= 0f || vertices.Count == 0)
            return result;

        float startLength = startParameter * length2D;
        float endLength = endParameter * length2D;

        if (startParameter <= 0f)
            result.Add(vertices[0]);

        float counter = 0f;
        float dist = 0f;

        for (int i = 0; i < vertices.Count - 1; i++)
        {
            dist = Vector2.Distance(vertices[i].Position2D, vertices[i + 1].Position2D);

            if (counter + dist >= startLength)
            {
                if (counter + dist <= endLength)
                {
                    result.Add(vertices[i + 1]);
                }
                else
                    break;
            }
            counter += dist;
        }
        return result;
    }

    public Vector3 PositionAtLength(float length)
    {
        calcLength();

        float dist = 0f;
        if (length >= length2D)
            return vertices[vertices.Count - 1];

        for (int i = 0; i < vertices.Count - 1; i++)
        {
            dist = Vector2.Distance(vertices[i].Position2D, vertices[i + 1].Position2D);

            if (dist < length)
            {
                length -= dist;
            }
            else
                return Vector3.Lerp(vertices[i].Position, vertices[i + 1].Position, length / dist);
        }
        return vertices[vertices.Count - 1];
    }
    public Vector3 PositionAtParameter(float t)
    {
        calcLength();

        float length = t * length2D;

        float dist = 0f;
        if (length >= length2D)
            return vertices[vertices.Count - 1];

        for (int i = 0; i < vertices.Count - 1; i++)
        {
            dist = Vector2.Distance(vertices[i].Position2D, vertices[i + 1].Position2D);

            if (dist < length)
            {
                length -= dist;
            }
            else
                return Vector3.Lerp(vertices[i].Position, vertices[i + 1].Position, length / dist);
        }
        return vertices[vertices.Count - 1];
    }
    public Vertex VertexAtLength(float length)
    {
        return new Vertex(PositionAtLength(length));
    }
    public Vertex VertexAtParameter(float t)
    {
        return new Vertex(PositionAtParameter(t));
    }

    //Better Hashset?! Getting the index faster?!
    //Maybe hold parmeters for each vertex in seperate array/list/whatever?!
    public float GetParameterOfVertex(Vertex v)
    {
        calcLength();
        float tmpLength = 0f;

        if (v == vertices[0] || this.length2D == 0f)
            return tmpLength;

        for (int i = 1; i < vertices.Count; i++)
        {
            tmpLength += Vector2.Distance(vertices[i - 1].Position2D, vertices[i].Position2D);

            if (v == vertices[i])
                return tmpLength / this.length2D;
        }
        return tmpLength / this.length2D;
    }
    public float ParameterAtIndex(int index)
    {
        calcLength();
        float tmpLength = 0f;

        if (index == 0 || this.length2D == 0f)
            return tmpLength;

        for (int i = 0; i < index - 1; i++)
        {
            tmpLength += Vector2.Distance(vertices[i].Position2D, vertices[i + 1].Position2D);
        }

        return tmpLength / this.length2D;
    }

    #endregion

    public ReadOnlyCollection<Vertex> Vertices
    {
        get { return vertices.AsReadOnly(); }
    }
    public float Length2D
    {
        get
        {
            return length2D;
        }
    }

    protected List<Vertex> vertices = new List<Vertex>();
    private float length2D = -1f;

    private void calcLength()
    {
        length2D = 0f;
        for (int i = 0; i < vertices.Count - 1; i++)
        {
            length2D += Vector2.Distance(vertices[i].Position2D, vertices[i + 1].Position2D);
        }
    }
    #region Polyline Listlike
    public int Count
    {
        get { return vertices.Count; }
    }
    public Vertex this[int i]
    {
        get { return vertices[i]; }
    }
    public Vertex First
    {
        get
        {
            if (vertices.Count > 0)
                return vertices[0];
            return null;
        }
    }
    public Vertex Last
    {
        get
        {
            if (vertices.Count > 0)
                return vertices[vertices.Count - 1];
            return null;
        }
    }
    public void Add(Vertex v)
    {
        vertices.Add(v);
        calcLength();
    }
    public void AddRange(IEnumerable<Vertex> vertices)
    {
        this.vertices.AddRange(vertices);
        calcLength();
    }
    public bool Remove(Vertex v)
    {
        bool removed = vertices.Remove(v);
        if (removed)
            calcLength();
        return removed;
    }
    public Polyline Reversed()
    {
        List<Vertex> reversedPolylineVertices = new List<Vertex>(vertices);
        reversedPolylineVertices.Reverse();
        return new Polyline(reversedPolylineVertices);
    }
    public void Clear()
    {
        vertices.Clear();
        length2D = -1f;
    }
    #endregion

    public void OnDrawGizmos()
    {
        for (int i = 0; i < vertices.Count - 1; i++)
        {
            Gizmos.DrawLine(vertices[i], vertices[i + 1]);
        }
    }
    public static implicit operator bool(Polyline exists)
    {
        if (exists != null)
            return true;
        return false;
    }

    public static bool Intersection(Polyline polyline1, Polyline polyline2, out Vector2 p)
    {
        for (int i = 0; i < polyline1.Count - 1; i++)
        {
            for (int j = 0; j < polyline2.Count - 1; j++)
            {
                if (Math2D.SegmentSegmentIntersection(polyline1[i], polyline1[i + 1], polyline2[j], polyline2[j + 1]))
                    return Math2D.LineLineIntersection(polyline1[i], polyline1[i + 1], polyline2[j], polyline2[j + 1], out p);
            }
        }
        p = Vector2.zero;
        return false;
    }
}