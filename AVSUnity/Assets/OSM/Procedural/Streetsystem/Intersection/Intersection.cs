using UnityEngine;
#if UNITY_EDITOR
//using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

/// <summary>
/// Intersection.
/// </summary>
public class Intersection : IProceduralObjects
{
    static Dictionary<Intersection, Tile> intersectionsTileDict = new Dictionary<Intersection, Tile>();
    public static Dictionary<long, Intersection> intersections = new Dictionary<long, Intersection>();
    public static List<Intersection> intersectionsList = new List<Intersection>();

    public static void Clear()
    {
        intersections.Clear();
        intersectionsList.Clear();
    }
    public static bool TryCreateFromOSM(OSMWay way, Tile tile)
    {
        if (way.Tags.ContainsKey("highway"))
        {
            if (
                way.Tags["highway"] == "residential" ||
                way.Tags["highway"] == "motorway" ||
                way.Tags["highway"] == "primary" ||
                way.Tags["highway"] == "secondary" ||
                way.Tags["highway"] == "tertiary" ||
                way.Tags["highway"] == "unclassified" ||
                way.Tags["highway"] == "service" ||
                way.Tags["highway"] == "path" ||
                way.Tags["highway"] == "footway")
            {
                //Intersections
                for (int i = 0; i < way.WayNodes.Count; i++)
                {
                    //Check if there is already an intersection for this node
                    if (!intersections.ContainsKey(way.WayNodes[i]))
                    {
                        List<OSMWay> intersectionsWays = new List<OSMWay>();

                        foreach (KeyValuePair<long, OSMWay> wayKV in tile.Query.OSM.ways)
                        {
                            if (wayKV.Value.ContainsNode(way.WayNodes[i]))
                            {
                                if (wayKV.Value.Tags.ContainsKey("highway"))
                                    intersectionsWays.Add(wayKV.Value);
                            }
                        }

                        //Create Intersection, if node is part of more then one way
                        if (intersectionsWays.Count > 1)
                        {

                            OSMNode node;
                            if (tile.Query.OSM.nodes.TryGetValue(way.WayNodes[i], out node))
                            {
                                Intersection newIntersection = new Intersection(node, intersectionsWays);
                                intersectionsTileDict.Add(newIntersection, tile);
                                intersections.Add(newIntersection.Node.Id, newIntersection);
                                intersectionsList.Add(newIntersection);

                            }
                        }
                        //Also create Intersections if it the start/end of a way
                        else if (i == 0 || i == way.WayNodes.Count - 1)
                        {
                            OSMNode node;
                            if (tile.Query.OSM.nodes.TryGetValue(way.WayNodes[i], out node))
                            {
                                Intersection newIntersection = new Intersection(node, intersectionsWays);
                                intersectionsTileDict.Add(newIntersection, tile);
                                intersections.Add(newIntersection.Node.Id, newIntersection);
                                intersectionsList.Add(newIntersection);
                            }
                        }
                    }
                }
                return true;
            }
        }
        return false; 
    }
    public static void CreateAllMeshes()
    {
        foreach (KeyValuePair<Intersection, Tile> kV in intersectionsTileDict)
        {
            kV.Key.CreateMesh(kV.Value.TileMesh);
        }
    }
    public static void CreateAllMeshes(ModularMesh mesh)
    {
        foreach (KeyValuePair<Intersection, Tile> kV in intersectionsTileDict)
        {
            kV.Key.CreateMesh(mesh);
        }
    }

    public Intersection(OSMNode node, List<OSMWay> ways)
    {
        this.node = node;
        this.ways = ways;
    }

    public OSMNode Node
    {
        get { return node; }
    }
    public List<OSMWay> Ways
    {
        get { return ways; }
    }

    public IntersectionType Type
    {
        get 
        {
            if (intersectionMembers.Count > 2)
                return IntersectionType.Intersection;
            if (intersectionMembers.Count == 2)
                return IntersectionType.Connection;
            return IntersectionType.DeadEnd;
        }
    }

    private OSMNode node;
    private List<OSMWay> ways;

    public void AddStreet(Street street)
    {
        IntersectionMember iM = new IntersectionMember(street, this);
        if (iM.IsValid())
        {
            intersectionMembers.Add(iM);
            intersectionMembers.Sort(new IntersectionMemberComparer());
        }
    }
    public bool TryGetRightStreet(Street street, out Street rightStreet)
    {
        int streetIndex = intersectionMembers.FindIndex(x => x.Street == street);
        if (streetIndex < 0)
        {
            rightStreet = null;
            return false;
        }
        int rightIndex = streetIndex + 1;
        if(rightIndex >= intersectionMembers.Count)
            rightIndex -= intersectionMembers.Count;
        rightStreet = intersectionMembers[rightIndex].Street;
        return true;
    }
    public bool TryGetLeftStreet(Street street, out Street leftStreet)
    {
        int streetIndex = intersectionMembers.FindIndex(x => x.Street == street);
        if (streetIndex < 0)
        {
            leftStreet = null;
            return false;
        }

        int leftIndex = streetIndex - 1;
        if (leftIndex < 0)
            leftIndex += intersectionMembers.Count;
        leftStreet = intersectionMembers[leftIndex].Street;
        return true;
    }

    public ReadOnlyCollection<IntersectionMember> IntersectionMembers
    {
        get { return intersectionMembers.AsReadOnly(); }
    }
    private List<IntersectionMember> intersectionMembers = new List<IntersectionMember>();

    public void CreateMesh(ModularMesh mesh)
    {
        Vector3 nodePos = node;

        Vertex intersectionMid = new Vertex(nodePos);
        //Create Intersection Quads (for testing)
        Vector2 intersectionPoint;


        //"PushBack"
        if(intersectionMembers.Count >= 2)
        {
            for (int i = 0; i < intersectionMembers.Count-1; i++)
            {
                if (Polyline.Intersection(intersectionMembers[i].LeftOutline, intersectionMembers[i + 1].RightOutline, out intersectionPoint))
                {
                    float neededFromOffset0 = 5f;
                    float neededFromOffset1 = 5f;

                    //Calculate
                    //TODO

                    intersectionMembers[i].FromOffset = Mathf.Min(intersectionMembers[i].FromOffset, neededFromOffset0);
                    intersectionMembers[i+1].FromOffset = Mathf.Min(intersectionMembers[i+1].FromOffset, neededFromOffset1); 
                }
            }

            if (Polyline.Intersection(intersectionMembers[intersectionMembers.Count - 1].LeftOutline, intersectionMembers[0].RightOutline, out intersectionPoint))
            {
                float neededFromOffset0 = 5f;
                float neededFromOffset1 = 5f;

                //Calculate
                //TODO

                intersectionMembers[intersectionMembers.Count - 1].FromOffset = Mathf.Min(intersectionMembers[intersectionMembers.Count - 1].FromOffset, neededFromOffset0);
                intersectionMembers[0].FromOffset = Mathf.Min(intersectionMembers[0].FromOffset, neededFromOffset1); 
            }
        }

        if (Type == IntersectionType.Intersection)
        {
            for (int i = 0; i < intersectionMembers.Count-1; i++)
            {
                CarriageWay forwardCarriageWay = intersectionMembers[i].ForwardCarriageWay;
                CarriageWay forwardCarriageWay2 = intersectionMembers[i+1].ForwardCarriageWay;

                if (forwardCarriageWay != null && forwardCarriageWay2 != null)
                {
                    for (int j = 0; j < Mathf.Min(forwardCarriageWay.Count, forwardCarriageWay2.Count); j++)
                    {
                        CarLane carlane1 = (CarLane)forwardCarriageWay.GetLastStreetMemberOfType(typeof(CarLane));
                        CarLane carlane2 = (CarLane)forwardCarriageWay2.GetLastStreetMemberOfType(typeof(CarLane));

                        if (carlane1 != null && carlane2 != null)
                        {
                            //Quad newQuad = new Quad(carlane1.RightOutline.First, carlane1.LeftOutline.First, carlane2.LeftOutline.First, carlane2.RightOutline.First, mesh, MaterialManager.GetMaterial("error"));
                            //Debug.Log("Creating LaneQuad");
                        }
                        else
                        {
                            //Debug.Log("Carlanes null?!");
                        }
                    }
                }
                else
                {
                    //Debug.Log("CarriageWays nul!?");
                }
            }

            //for (int i = 0; i < intersectionMembers.Count; i++)
            //{
            //    if (intersectionMembers[i].BeginningLeft && intersectionMembers[i].BeginningRight)
            //    {
            //        new Triangle(intersectionMid, intersectionMembers[i].BeginningRight, intersectionMembers[i].BeginningLeft, mesh, MaterialManager.GetMaterial("diffuseDarkGray"));
            //    }  
            //}

            //Triangle tri;
            //for (int i = 0; i < intersectionMembers.Count-1; i++)
            //{
            //    tri = new Triangle(intersectionMid, intersectionMembers[i+1].BeginningLeft, intersectionMembers[i].BeginningRight);
            //    if(!tri.IsClockwise())
            //        tri.AddToMesh(mesh, MaterialManager.GetMaterial("error"));
            //}
            //tri = new Triangle(intersectionMid, intersectionMembers[0].BeginningLeft, intersectionMembers[intersectionMembers.Count - 1].BeginningRight);
            //if (!tri.IsClockwise())
            //    tri.AddToMesh(mesh, MaterialManager.GetMaterial("error"));
        }

        if (Type == IntersectionType.Connection)
        {
            new Quad(intersectionMembers[0].BeginningRight, intersectionMembers[0].BeginningLeft, intersectionMembers[1].BeginningRight, intersectionMembers[1].BeginningLeft, mesh, MaterialManager.GetMaterial("diffuseGrey"));
        }

        if (Type == IntersectionType.DeadEnd)
        {
 
        }
    }
    public void UpdateMesh(ModularMesh mesh)
    {
        throw new System.NotImplementedException();
    }
    public void Destroy()
    {
        throw new System.NotImplementedException();
    }

    public override string ToString()
    {
        return "Intersection " + node.Id;
    }
    public static implicit operator long(Intersection intersection)
    {
        return intersection.node.Id;
    }
    public static implicit operator OSMNode(Intersection intersection)
    {
        return intersection.node;
    }
}




