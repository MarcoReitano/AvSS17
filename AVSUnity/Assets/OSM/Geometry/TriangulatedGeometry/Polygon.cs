using UnityEngine;
#if UNITY_EDITOR
//using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Polygon.
/// </summary>

public class Polygon
{
    public Polygon()
    {}
    public Polygon(List<Vertex> vertices)
    {
        this.vertices = vertices;
    }
    public Polygon(params Vertex[] vertices)
    {
        this.vertices.AddRange(vertices);
    }
	
	[SerializeField][HideInInspector]
    protected List<Vertex> vertices = new List<Vertex>();
		
    public float Area2D()
    {
        float result = 0f;

        for (int i = 0; i < vertices.Count; i++)
        {
            result += vertices[i].X * vertices[i + 1].Z - vertices[i + 1].X * vertices[i].Z;
        }
        result += vertices[vertices.Count - 1].X * vertices[0].Z - vertices[0].X * vertices[vertices.Count - 1].Z;

        result *= 0.5f;

        return result;
    }
    public Vector2 Centroid()
    {
        Vector2 result = Vector2.zero;
        for(int i = 0; i < vertices.Count; i++)
        {
            result += vertices[i].Position2D;
        }
        result /= vertices.Count;
        return result;
    }

    public bool IsSelfIntersecting()
	{
        int nexti;
        int nextj;
		for(int i = 0; i < vertices.Count; i++)
		{
            nexti = (i + 1) % vertices.Count;
			
			for(int j = 0; j < vertices.Count; j++)
			{
				nextj = (j + 1) % vertices.Count;
                if (nextj >= vertices.Count)
					nextj -= vertices.Count;

                if (i == j || nexti == j || i == nextj)
                    continue;

                if (Math2D.SegmentSegmentIntersection(
					vertices[i].Position2D, 
					vertices[nexti].Position2D, 
					vertices[j].Position2D, 
					vertices[nextj].Position2D))
				{
					//Debug.Log("Polygon is SelfIntersecting");
					return true;
				}
			}			
		}
		return false;		
	}

    public Polygon LevelUp()
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
        return new Polygon(leveledVertices); 
    }
    public Polygon LevelDown()
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

        return new Polygon(leveledVertices); 
    }
    public Polygon Translate(Vector3 extrudeVector)
    {
        List<Vertex> translatedVertices = new List<Vertex>();
        for (int i = 0; i < vertices.Count; i++)
        {
            translatedVertices.Add(new Vertex(vertices[i].Position + extrudeVector));
        }

        return new Polygon(translatedVertices);
    }

    public Polygon Rotate(float angle)
    {
        Vector2 centroid = Centroid();
        List<Vertex> rotatedVertices = new List<Vertex>();
        for (int i = 0; i < vertices.Count; i++)
        {
            Vector2 rotatedPos = (vertices[i].Position2D - centroid).Rotate(angle) + centroid;
            Vector3 rotatedPos3d = new Vector3(rotatedPos.x, vertices[i].Y, rotatedPos.y);
            rotatedVertices.Add(new Vertex(rotatedPos3d)); 
        }
        return new Polygon(rotatedVertices);
    }
    public Polygon RotateAround(float angle, Vector2 point)
    {
        List<Vertex> rotatedVertices = new List<Vertex>();
        for (int i = 0; i < vertices.Count; i++)
        {
            Vector2 rotatedPos = (vertices[i].Position2D - point).Rotate(angle) + point;
            Vector3 rotatedPos3d = new Vector3(rotatedPos.x, vertices[i].Y, rotatedPos.y);
            rotatedVertices.Add(new Vertex(rotatedPos3d));
        }
        return new Polygon(rotatedVertices);
 
    }

    public Polygon Union(Polygon polygon)
    {
        throw new System.NotImplementedException();
    }
    public Polygon Intersect(Polygon polygon)
    {
        throw new System.NotImplementedException();
    }
    public Polygon Difference(Polygon polygon)
    {
        throw new System.NotImplementedException();
    }

    public Polygon Inset(float amount)
    {
        Polygon result = new Polygon();

        int before = 0;
        int after;

        Vector2 edgeVector1, edgeVector2, beforeVertex, beforeVertex2, afterVertex, afterVertex2, intersectionPoint;

        for (int i = 0; i < vertices.Count; i++)
        {
            before = i - 1;

            if (before < 0)
                before += vertices.Count;
            after = i + 1;

            if (after >= vertices.Count)
                after -= vertices.Count;


            edgeVector1 = vertices[before].Position2D - vertices[i].Position2D;
            edgeVector2 = vertices[i].Position2D - vertices[after].Position2D;

            beforeVertex = vertices[before].Position2D + edgeVector1.NormalVector() * amount;
            beforeVertex2 = vertices[i].Position2D + edgeVector1.NormalVector() * amount;

            afterVertex = vertices[i].Position2D + edgeVector2.NormalVector() * amount;
            afterVertex2 = vertices[after].Position2D + edgeVector2.NormalVector() * amount;

            if (Math2D.LineLineIntersection(beforeVertex, beforeVertex2, afterVertex, afterVertex2, out intersectionPoint))
            {
                //Intersection found
                result.vertices.Add(new Vertex(intersectionPoint.ToXZVector3(vertices[i].Y)));
            }
            else
            {
                //Debug.Log("Parallel?!");
                //Lines are parallel
                result.vertices.Add(new Vertex(beforeVertex2.ToXZVector3(vertices[i].Y)));
            }
        }
        return result;
    }

    public Polygon Split(float amount, ModularMesh mesh)
    {
        List<Vertex> newVertices = new List<Vertex>();
        float distance;

        newVertices.Add(vertices[0]);
        int i;
        for (i = 1;  i < vertices.Count; i++)
        {
            distance = Vector2.Distance(vertices[i - 1].Position2D, vertices[i].Position2D);
            amount -= distance;

            if (amount <= 0)
            {

            }

            newVertices.Add(vertices[i]);
        }

        for (int j = i; j < vertices.Count; j++)
        {
            newVertices.Add(vertices[j]);
        }
        return new Polygon(newVertices);
    }

    public void MakeClockwise()
    {
        if(Math2D.IsClockwise(vertices.ToArray()))
            vertices.Reverse();
    }

    /*
    public void TriangulateEarClipping()
    {
        int count = 0;
        int countSameTriangle = 0;

        polygonTriangles = new List<Triangle>();
        //  Polygon als Liste von Punkten
        List<Vector3> worklist = new List<Vector3>(polygonPoints);

        // Prüfe ob das Polygon im  Uhrzeigersinn angeordnet ist
        bool polygonIsClockwise = IsClockwise(polygonPoints);
        bool triangleIsClockwise = false;

        int current;
        int next;
        int nextnext;

        // Beginne mit i = 0
        int i = 0;
        // Solange mehr als 2 Punkte in der Liste
        while (worklist.Count > 2)
        {
            count++;
            if (count > 20)
            {
                Debug.Log("Emergency Exit");
                return;
            }
           

            // Indizes der Points
            // Hole die nächsten 3 Punkte current, next, nextnext
            current = i % worklist.Count;
            next = (i + 1) % worklist.Count;
            nextnext = (i + 2) % worklist.Count;

            // Ist das Triangle in Clockwise-Order 
            triangleIsClockwise = IsClockwise(
                worklist[current],
                worklist[next],
                worklist[nextnext]);

            if (countSameTriangle == 3)
            {
                polygonTriangles.Add(
                       new Triangle(
                           worklist[current],
                           worklist[nextnext],
                           worklist[next]));

                worklist.RemoveAt(next);
                Debug.Log("Finished");
            }

            // Wenn Polygon und Triangle im Uhrzeigersinn angeordnet sind
            if (polygonIsClockwise == triangleIsClockwise)
            {
                // Liegt keiner der Punkte im Triangle?
                if (!AnyPointInTriangle(
                    worklist[current],
                    worklist[next],
                    worklist[nextnext],
                    worklist))
                {
                    //    Füge das Triangle hinzu
                    polygonTriangles.Add(
                        new Triangle(
                            worklist[current],
                            worklist[next],
                            worklist[nextnext]));
                    //    Lösche next aus der Liste
                    worklist.RemoveAt(next);

                    // Gehe zum nächsten Point
                    i++;
                }
                else
                    i++;
            }
            else
            {
                i++;
                if (worklist.Count == 3)
                {
                    countSameTriangle++;
                    // TODO: Check if self-intersecting
                    //Triangle tri = new Triangle(
                    //       worklist[current],
                    //       worklist[next],
                    //       worklist[nextnext]);

                    //foreach (Triangle tri2 in polygonTriangles)
                    //{


                    //}

                }
            }
                
        }
    }
        */

    //public PolygonSurface Triangulate(ModularMesh mesh, Material material)
    //{
    //    List<Triangle> triangles = new List<Triangle>();
    //    List<Vertex> worklist = new List<Vertex>(vertices);

    //    bool polygonIsClockwise = Math2D.IsClockwise(vertices.ToArray());
    //    bool triangleIsClockwise = false;

    //    //Cant triangulate selfIntersecting Polygons
    //    //if (IsSelfIntersecting())
    //    //{
    //    //    //Debug.Log("Is self intersecting");
    //    //    return new PolygonSurface(triangles);
    //    //}

    //    int current;
    //    int next;
    //    int nextnext;

    //    // Beginne mit i = 0
    //    int i = 0;
    //    int count = 0;
    //    int countSameTriangle = 0;
    //    // Solange mehr als 2 Punkte in der Liste
    //    while (worklist.Count > 2)
    //    {
    //        count++;

    //        if (count > 500)
    //            return new PolygonSurface(triangles);
    //        // Indizes der Points
    //        // Hole die nächsten 3 Punkte current, next, nextnext
    //        current = i % worklist.Count;
    //        next = (i + 1) % worklist.Count;
    //        nextnext = (i + 2) % worklist.Count;

    //        // Ist das Triangle in Clockwise-Order 
    //        triangleIsClockwise = Math2D.IsClockwise(
    //            worklist[current],
    //            worklist[next],
    //            worklist[nextnext]);

    //        if (countSameTriangle == 3)
    //        {
    //            triangles.Add(
    //                   new Triangle(
    //                       worklist[current],
    //                       worklist[nextnext],
    //                       worklist[next]));

    //            worklist.RemoveAt(next);
    //            break;
    //        }

    //        // Wenn Polygon und Triangle im Uhrzeigersinn angeordnet sind
    //        if (polygonIsClockwise == triangleIsClockwise)
    //        {
    //            // Liegt keiner der Punkte im Triangle?
    //            if (!Math2D.AnyPointInTriangle(
    //                worklist[current],
    //                worklist[next],
    //                worklist[nextnext],
    //                worklist))
    //            {
    //                //    Füge das Triangle hinzu
    //                triangles.Add(
    //                    new Triangle(
    //                        worklist[current],
    //                        worklist[next],
    //                        worklist[nextnext]));
    //                //    Lösche next aus der Liste
    //                worklist.RemoveAt(next);

    //                // Gehe zum nächsten Point
    //                i++;
    //            }
    //            else
    //                i++;
    //        }
    //        else
    //        {
    //            i++;
    //            if (worklist.Count == 3)
    //                countSameTriangle++;
    //        }

    //    }
    //    return new PolygonSurface(triangles);
    //}

    public PolygonSurface Triangulate(ModularMesh mesh, Material material)
    {
        List<Triangle> triangles = new List<Triangle>();
        List<Vertex> workList = new List<Vertex>(vertices);

        bool polygonIsClockwise = Math2D.IsClockwise(vertices.ToArray());
        bool triangleIsClockwise = false;
        int next;
        int nextnext;

        //Cant triangulate selfIntersecting Polygons
        if (IsSelfIntersecting())
        {
            //Debug.Log("Is self intersecting");
            return new PolygonSurface(triangles);
        }

        int count = 0;
        int helper = 0;
        while (workList.Count > 2)
        {
            if (helper < 0)
                helper = 0;
            if (helper > workList.Count - 1)
                helper = 0;

            if(count++ > 500)
                return new PolygonSurface(triangles);

            for (int i = helper; i < workList.Count; i++)
            {
                helper = i + 1;
                next = i + 1;
                if (next >= workList.Count)
                    next -= workList.Count;
                nextnext = i + 2;
                if (nextnext >= workList.Count)
                    nextnext -= workList.Count;

                triangleIsClockwise = Math2D.IsClockwise(new Vertex[] { workList[i], workList[next], workList[nextnext] });

                if (triangleIsClockwise == polygonIsClockwise)
                {
                    if (!Math2D.AnyPointInTriangle(workList[i], workList[next], workList[nextnext], workList))
                    {
                        triangles.Add(new Triangle(workList[i], workList[next], workList[nextnext], mesh, material));
                        workList.RemoveAt(next);
                        break;
                    }
                }
            }
        }
        return new PolygonSurface(triangles);
    }

    public void OnDrawGizmos()
    {
        for (int i = 0; i < vertices.Count - 1; i++)
        {
            Gizmos.DrawLine(vertices[i].Position, vertices[i + 1].Position);
        }
        if(vertices.Count > 1)
            Gizmos.DrawLine(vertices[0].Position, vertices[vertices.Count - 1].Position);
    }

    #region Polygon Listlike 
    public int Count
    {
        get { return vertices.Count; }
    }
    public Vertex this[int i]
    {
        get { return vertices[i]; }
    }
    public void Add(Vertex v)
    {
        vertices.Add(v);
    }
    public void AddRange(IEnumerable<Vertex> vertices)
    {
        this.vertices.AddRange(vertices);
    }
    public void Add(Polyline polyline)
    {
        vertices.AddRange(polyline.Vertices);
    }
    public bool Remove(Vertex v)
    {
        return vertices.Remove(v);
    }
    public Polygon Reversed()
    {
        List<Vertex> reversedPolylineVertices = new List<Vertex>(vertices);
        reversedPolylineVertices.Reverse();
        return new Polygon(reversedPolylineVertices);
    }
    public void Clear()
    {
        vertices.Clear();
    }
    #endregion

    //    public Polygon AlsoInterestingInset(float amount, ModularMesh mesh)
    //    {
    //
    //        Polygon result = new Polygon();
    //
    //
    //        int before = 0;
    //        int after;
    //
    //
    //        for (int i = 0; i < polygonVertices.Count; i++)
    //        {
    //
    //            before = i - 1;
    //
    //            if (before < 0)
    //                before += polygonVertices.Count;
    //
    //            after = i + 1;
    //            if (after >= polygonVertices.Count)
    //                after -= polygonVertices.Count;
    //
    //
    //            Vector2 edgeVector1 = polygonVertices[before].Position2D - polygonVertices[i].Position2D;
    //            Vector2 edgeVector2 = polygonVertices[i].Position2D - polygonVertices[after].Position2D;
    //
    //
    //            Vector2 beforeVertex = polygonVertices[before].Position2D + edgeVector1.NormalVector() * amount;
    //            Vector2 beforeVertex2 = polygonVertices[i].Position2D + edgeVector1.NormalVector() * amount;
    //
    //
    //            Vector2 afterVertex = polygonVertices[i].Position2D + edgeVector2.NormalVector() * amount;
    //            Vector2 afterVertex2 = polygonVertices[after].Position2D + edgeVector2.NormalVector() * amount;
    //
    //
    //            result.polygonVertices.Add(new Vertex(beforeVertex2.ToXZVector3(polygonVertices[i].Y), mesh));
    //            result.polygonVertices.Add(new Vertex(afterVertex.ToXZVector3(polygonVertices[i].Y), mesh));
    //        }
    //
    //        return result;
    //    }
}