using UnityEngine;
#if UNITY_EDITOR
//using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Street.
/// </summary>

public class Street : IProceduralObjects
{
    public static Dictionary<Street, Tile> streets = new Dictionary<Street, Tile>();
    public static void Clear()
    {
        streets.Clear();
    }

    public static bool CreateStreets(Tile tile)
    {
        List<Intersection> allIntersectionsOnWay = new List<Intersection>();
        OSMWay currentWay;
        Intersection nextIntersection = null;
        Intersection previousIntersection = null;
        bool alreadyExisting = false;

        //Streets
        for (int i = 0; i < Intersection.intersectionsList.Count; i++)
        {
            for (int j = 0; j < Intersection.intersectionsList[i].Ways.Count; j++)
            {
                currentWay = Intersection.intersectionsList[i].Ways[j];
                nextIntersection = null;
                previousIntersection = null;
                allIntersectionsOnWay.Clear();

                //Find next/previous intersectionsList[i]
                for (int k = 0; k < Intersection.intersectionsList.Count; k++)
                {
                    if (Intersection.intersectionsList[k].Ways.Contains(currentWay) && Intersection.intersectionsList[k] != Intersection.intersectionsList[i])
                        allIntersectionsOnWay.Add(Intersection.intersectionsList[k]);
                }

                int currentIntersectionIndex = currentWay.WayNodes.IndexOf(Intersection.intersectionsList[i]);
                int indexNext = int.MaxValue;
                int indexPrevious = int.MinValue;



                for (int k = 0; k < allIntersectionsOnWay.Count; k++)
                {
                    int currentIndex = currentWay.WayNodes.IndexOf(allIntersectionsOnWay[k]);

                    if (currentIndex > currentIntersectionIndex && currentIndex < indexNext)
                    {
                        indexNext = currentIndex;
                        nextIntersection = allIntersectionsOnWay[k];
                    }
                    if (currentIndex < currentIntersectionIndex && currentIndex > indexPrevious)
                    {
                        indexPrevious = currentIndex;
                        previousIntersection = allIntersectionsOnWay[k];
                    }
                }

                //Is Street already existing? if not create 
                alreadyExisting = false;

                if (nextIntersection != null)
                {
                    foreach(KeyValuePair<Street, Tile> kV in streets)
                    {
                        if (kV.Key.From == Intersection.intersectionsList[i] && kV.Key.To == nextIntersection && kV.Key.Way == currentWay)
                            alreadyExisting = true;
                    }
                    if (!alreadyExisting)
                    {
                        if (currentWay.Tags.ContainsKey("bridge"))
                        {
                            Bridge newBridge = new Bridge(Intersection.intersectionsList[i], nextIntersection, currentWay, tile.Query.OSM);
                            Bridge.bridges.Add(newBridge, tile);
                            streets.Add(newBridge.Street, tile);
                        }
                        else
                        {
                            streets.Add(new Street(Intersection.intersectionsList[i], nextIntersection, currentWay, tile.Query.OSM), tile);
                        }

                    }
                }

                alreadyExisting = false;
                if (previousIntersection != null)
                {
                    foreach (KeyValuePair<Street, Tile> kV in streets)
                    {
                        if (kV.Key.From == previousIntersection && kV.Key.To == Intersection.intersectionsList[i] && kV.Key.Way == currentWay)
                            alreadyExisting = true;
                    }
                    if (!alreadyExisting)
                    {
                        if (currentWay.Tags.ContainsKey("bridge"))
                        {
                            Bridge newBridge = new Bridge(previousIntersection, Intersection.intersectionsList[i], currentWay, tile.Query.OSM);
                            Bridge.bridges.Add(newBridge, tile);
                            streets.Add(newBridge.Street, tile);
                        }
                        else
                        {
                            streets.Add(new Street(previousIntersection, Intersection.intersectionsList[i], currentWay, tile.Query.OSM), tile);
                        }
                    }
                }

            }
        }
        return true;
    }
    public static void CreateAllMeshes()
    {
        foreach (KeyValuePair<Street, Tile> kV in streets)
        {
            kV.Key.CreateMesh(kV.Value.TileMesh);
        }
    }
    public static void CreateAllMeshes(ModularMesh mesh)
    {
        foreach (KeyValuePair<Street, Tile> kV in streets)
        {
            kV.Key.CreateMesh(mesh);
        }
    }

    public Street(Intersection from, Intersection to, OSMWay way, OSMData osm)
    {
        this.from = from;
        this.to = to;
        this.way = way;
        //this.osm = osm;

        OSMNode node;
        pathway = new Polyline();

        foreach (long nodeId in way.GetWayNodesFromTo(from, to))
        {
            if (osm.nodes.TryGetValue(nodeId, out node))
            {
                pathway.Add(new Vertex(node + Vector3.up * 0.1f));
            }
        }

        centerLine = new Polyline(pathway.Vertices);


        //Needs centerLine, therefore after that
        from.AddStreet(this);
        to.AddStreet(this);

        /*Default is oneway=no, except for highway=motorway which implies oneway=yes. junction=roundabout also implies oneway=yes. highway=motorway_link has been a discussion a few times before, and it's best to add the oneway=* tag on those all the time, one way or not. --Eimai 12:59, 8 January 2009 (UTC)
        Should this be indicated in the page? --Seav 10:49, 14 January 2009 (UTC) 
        Other implicit oneway=yes cases are: highway=motorway_link, highway=trunk_link and highway=primary_link. --Beldin 21:55, 22 April 2009 (UTC)*/


        if (way.Tags.ContainsKey("oneway"))
        {
            if (way.Tags["oneway"] == "yes" || way.Tags["oneway"] == "1" || way.Tags["oneway"] == "true")
            {
                isOneWay = true;
            }
        }
        if(way.Tags.ContainsKey("highway"))
        {
            if (way.Tags["highway"] == "motorway")
            {
                isOneWay = true;
            } 
        }
        if (way.Tags.ContainsKey("junction"))
        {
            if (way.Tags["junction"] == "roundabout")
            {
                isOneWay = true;
            }
        }

        if (isOneWay)
        {
            carriageways[0] = new CarriageWay(way);
        }
        else
        {
            carriageways[0] = new CarriageWay(way);
            carriageways[1] = new CarriageWay(way);
        }
    }
    public Street(Intersection from, Intersection to, OSMWay way, OSMData osm, Polyline pathway)
    {
        this.from = from;
        this.to = to;
        this.way = way;
        //this.osm = osm;

        //Creation of the pathway is handled outside (by bridge)
        this.pathway = pathway;
        centerLine = new Polyline(pathway.Vertices);


        //Needs centerLine, therefore after that
        from.AddStreet(this);
        to.AddStreet(this);

        /*Default is oneway=no, except for highway=motorway which implies oneway=yes. junction=roundabout also implies oneway=yes. highway=motorway_link has been a discussion a few times before, and it's best to add the oneway=* tag on those all the time, one way or not. --Eimai 12:59, 8 January 2009 (UTC)
        Should this be indicated in the page? --Seav 10:49, 14 January 2009 (UTC) 
        Other implicit oneway=yes cases are: highway=motorway_link, highway=trunk_link and highway=primary_link. --Beldin 21:55, 22 April 2009 (UTC)*/

        if (way.Tags.ContainsKey("oneway"))
        {
            if (way.Tags["oneway"] == "yes" || way.Tags["oneway"] == "1" || way.Tags["oneway"] == "true")
            {
                isOneWay = true;
            }
        }
        if (way.Tags.ContainsKey("highway"))
        {
            if (way.Tags["highway"] == "motorway")
            {
                isOneWay = true;
            }
        }
        if (way.Tags.ContainsKey("junction"))
        {
            if (way.Tags["junction"] == "roundabout")
            {
                isOneWay = true;
            }
        }

        if (isOneWay)
        {
            carriageways[0] = new CarriageWay(way);
        }
        else
        {
            carriageways[0] = new CarriageWay(way);
            carriageways[1] = new CarriageWay(way);
        }
 
    }

    public OSMWay Way
    {
        get { return way; }
    }
    public Intersection From
    {
        get { return from; }
    }
    public Intersection To
    {
        get { return to; }
    }

    public float FromOffset
    {
        get { return fromOffset; }
        set { fromOffset = value; }
    }
    public float ToOffset
    {
        get { return toOffset; }
        set { toOffset = value; }
    }

    private OSMWay way;
    private Intersection from;
    private Intersection to;

    //the full pathway provided by the OSMData
    private Polyline pathway;
    //the trimmed centerline 
    private Polyline centerLine;
    //trim offsets
    private float fromOffset = 5f;
    private float toOffset = 5f;

    public bool IsOnewWay
    {
        get { return isOneWay; }
    }
    private bool isOneWay = false;

    public Vertex Beginning
    {
        get 
        {
            if(centerLine)
                return centerLine.First;
            return null;
        }
 
    }
    public Vertex End
    {
        get 
        {
            if(centerLine)
                return centerLine.Last;
            return null;
        }
 
    }

    public Polyline CenterLine
    {
        get { return centerLine; }
    }
    public Polyline LeftOutline
    {
        get 
        {
            if (carriageways[1])
                return carriageways[1].RightOutline.Reversed();
            if (carriageways[0])
                return carriageways[0].LeftOutline;
            return centerLine;
        }
    }
    public Polyline RightOutline
    {
        get 
        {
            if (carriageways[0])
                return carriageways[0].RightOutline;
            if (carriageways[1])
                return carriageways[1].LeftOutline.Reversed();
            return centerLine; 
        }
    }

    public Vertex BeginningLeft
    {
        get
        {
            if (LeftOutline)
                return LeftOutline.First;
            return null;
        }
    }
    public Vertex BeginningRight
    {
        get
        {
            if (RightOutline)
                return RightOutline.First;
            return null;
        }
    }
    public Vertex EndLeft
    {
        get
        {
            if (LeftOutline)
                return LeftOutline.Last;
            return null;
        }
    }
    public Vertex EndRight
    {
        get
        {
            if (RightOutline)
                return RightOutline.Last;
            return null;
        }
    }

    public CarriageWay ForwardCarriageWay
    {
        get { return carriageways[0]; }
    }
    public CarriageWay BackwardCarriageWay
    {
        get { return carriageways[1]; }
    }

    private CarriageWay[] carriageways = new CarriageWay[2];

    public void CreateMesh(ModularMesh mesh)
    {
        //centerLine = pathway;
        centerLine = pathway.TrimEnd2D(10f).TrimFront2D(10f);

        if (carriageways[0])
        {
            carriageways[0].LeftOutline = centerLine;
            carriageways[0].CreateMesh(mesh);
        }
        if (carriageways[1])
        {
            carriageways[1].LeftOutline = centerLine.Reversed();
            carriageways[1].CreateMesh(mesh);
        }
    }
    public void UpdateMesh(ModularMesh mesh)
    {
        centerLine = pathway.TrimEnd2D(toOffset).TrimFront2D(fromOffset);

        if (carriageways[0])
        {
            carriageways[0].LeftOutline = centerLine;
            carriageways[0].UpdateMesh(mesh);
        }
        if (carriageways[1])
        {
            carriageways[1].LeftOutline = centerLine.Reversed();
            carriageways[1].UpdateMesh(mesh);
        }
    }
    public void Destroy()
    {
        if (carriageways[0] != null)
            carriageways[0].Destroy();
        if (carriageways[1] != null)
            carriageways[1].Destroy();
    }

    public void OnDrawGizmos()
    {
        centerLine.OnDrawGizmos();
        LeftOutline.OnDrawGizmos();
        RightOutline.OnDrawGizmos();
    }
}
