using System;
using System.Text;
using UnityEngine;
using System.Collections.Generic;


public class MapBounds// : ScriptableObject
{

    public static MapBounds Cologne = new MapBounds(50.9645, 6.9166, 7.0004, 50.9100); // Cologne
    public static MapBounds Gummersbach = new MapBounds(51.0395, 7.5359, 7.5904, 51.0075); // Gummersbach
    public static MapBounds SanJose= new MapBounds(37.4184, -121.9898, -121.7783, 37.2760); // San José
    public static MapBounds Warszawa = new MapBounds(52.3651, 20.7957, 21.2187, 52.1192); // Warszawa

 

    public void ClampMapBounds(){


    }


    public MapBounds()
    {
        this.north = 0.0f;
        this.east = 0.0f;
        this.south = 0.0f;
        this.west = 0.0f;
    }

    public MapBounds(double north, double west, double east, double south)
    {
        this.north = north;
        this.west = west;
        this.east = east;
        this.south = south;
        CheckCondition();
    }

    public void SetValues(double north, double west, double east, double south)
    {
        this.north = north;
        this.west = west;
        this.east = east;
        this.south = south;
        CheckCondition();
    }

    private void CheckCondition()
    {
        if (east < west)
        {
            double tmp = east;
            east = west;
            west = tmp;
        }

        if (north < south)
        {
            double tmp = north;
            north = south;
            south = tmp;
        }
    }

    public MapBounds Copy()
    {
        return new MapBounds(this.north, this.west, this.east, this.south);
    }

    // Latitude north
    private double north;
    public double North
    {
        get { return north; }
        set
        {
            north = value;
        }
    }

    // longitude East
    private double east;
    public double East
    {
        get { return east; }
        set
        {
            east = value;

        }
    }

    // Latitude south
    private double south;
    public double South
    {
        get { return south; }
        set
        {
            south = value;

        }
    }

    // longitude West
    private double west;
    public double West
    {
        get { return west; }
        set
        {
            west = value;

        }
    }

    
    public double EastWestDistance
    {
        get { return this.East - this.West; }
        set { }
    }

    public double NorthSouthDistance
    {
        get { return this.North - this.South; }
        set { }
    }


    public double CenterLongitude
    {
        get { return (east + west) / 2; }
    }


    public double CenterLatitude
    {
        get { return (north + south) / 2; }
    }


    public static MapBounds MaxBounds(List<MapBounds> list)
    {
        if (list == null)
            list = new List<MapBounds>();
        return MaxBounds(list.ToArray());
    }
    

    public static MapBounds MaxBounds(params MapBounds[] list)
    {
        MapBounds result = new MapBounds();

        // TODO: real Bounds are overflowing...   work with the real values
        if (list.Length > 0)
        {
            // start with first listitem so that the zero is no problem
            int count = 0;
            do
            {
                result = list[count++].Copy();
            } while (result.Equals(new MapBounds()));


            foreach (MapBounds bounds2 in list)
            {
                // sort out the (0,0,0,0) bounds
                if (!bounds2.Equals(new MapBounds()))
                {
                    // is northern ?
                    if (result.North < bounds2.North)
                    {
                        result.North = bounds2.North;
                    }

                    // is eastern ?
                    if (result.East < bounds2.East)
                    {
                        result.East = bounds2.East;
                    }

                    // is southern ?
                    if (result.South > bounds2.South)
                    {
                        result.South = bounds2.South;
                    }

                    // is western ?
                    if (result.West > bounds2.West)
                    {
                        result.West = bounds2.West;
                    }
                }
            }
        }
        return result;
    }



    public override bool Equals(System.Object obj)
    {
        // If parameter is null return false.
        if (obj == null)
        {
            return false;
        }

        // If parameter cannot be cast to Point return false.
        MapBounds p = obj as MapBounds;
        if ((System.Object)p == null)
        {
            return false;
        }

        // Return true if the fields match:
        return this == p;
    }

    public bool Equals(MapBounds p)
    {
        // If parameter is null return false:
        if ((object)p == null)
        {
            return false;
        }

        // Return true if the fields match:
        return this == p;
    }

    public override int GetHashCode()
    {
        // TODO: Besserer Hash
        StringBuilder sb = new StringBuilder();
        sb.Append(this.North.ToString());
        sb.Append(this.East.ToString());
        sb.Append(this.South.ToString());
        sb.Append(this.West.ToString());
        return sb.ToString().GetHashCode();
    }



    public static bool operator ==(MapBounds a, MapBounds b)
    {
        // If both are null, or both are same instance, return true.
        if (System.Object.ReferenceEquals(a, b))
        {
            return true;
        }

        // If one is null, but not both, return false.
        if (((object)a == null) || ((object)b == null))
        {
            return false;
        }

        // Return true if the fields match:
        return a.North == b.North && a.East == b.East && a.South == b.South && a.West == b.West;
    }

    public static bool operator !=(MapBounds a, MapBounds b)
    {
        return !(a == b);
    }


    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();

        sb.Append("[").Append("north").Append("=").Append(this.North).Append(", ");
        sb.Append("west").Append("=").Append(this.West).Append(", ");
        sb.Append("east").Append("=").Append(this.East).Append(", ");
        sb.Append("south").Append("=").Append(this.South).Append("]");

        return sb.ToString();
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="north"></param>
    /// <param name="south"></param>
    /// <param name="west"></param>
    /// <param name="east"></param>
    /// <param name="stepsX"></param>
    /// <param name="stepsY"></param>
    /// <param name="chunkSizeWidth"></param>
    /// <param name="chunkSizeHeight"></param>
    public static void calculateChunksize(
        ref double north, ref double south,
        ref double west, ref double east,
        ref int stepsX, ref int stepsY,
        ref double chunkSizeWidth, ref double chunkSizeHeight)
    {

        if (east < west)
        {
            double tmp = east;
            east = west;
            west = tmp;
        }

        if (north < south)
        {
            double tmp = north;
            north = south;
            south = tmp;
        }

        double width = east - west;
        double height = north - south;

        // höhe 
        int count = 1;
        while ((height / count) > 0.01f)
        {
            count++;
        }
        stepsY = count;
        chunkSizeHeight = height / stepsY;


        // breite 
        count = 1;
        while ((width / count) > 0.015f)
        {
            count++;
        }
        stepsX = count;
        chunkSizeWidth = width / stepsX;

    }




    public MapBounds AddMargin(float marginWidthKm)
    {
        double marginGrad = marginWidthKm * GeographicCoordinates.kmArc;

        double northSouthDistance = NorthSouthDistance;
        double eastWestDistance = EastWestDistance;

        double west = 0.0f;
        double east = 0.0f;
        double north = 0.0f;
        double south = 0.0f;

        if (eastWestDistance > northSouthDistance)
        {
            double width = ((eastWestDistance + 2 * marginGrad) - northSouthDistance) / 2;
            north = this.North + width;
            south = this.South - width;
            west = this.West - marginGrad;
            east = this.East + marginGrad;
        }
        else
        {
            double width = ((northSouthDistance + 2 * marginGrad) - eastWestDistance) / 2;
            north = this.North + marginGrad;
            south = this.South - marginGrad;
            west = this.West - width;
            east = this.East + width;
        }

        return new MapBounds(north, west, east, south);
    }



    /// <summary>
    ///  1    3
    ///  
    ///  0    2
    /// </summary>
    /// <returns></returns>
    public List<Vector3> WorldCoordsCorners()
    {
        List<Vector3> corners = new List<Vector3>();

        corners.Add(GeographicCoordinates.ConvertLonLatToXY(west, south, 0).Vector3FromVector2XZ());
        corners.Add(GeographicCoordinates.ConvertLonLatToXY(west, north, 0).Vector3FromVector2XZ());
        corners.Add(GeographicCoordinates.ConvertLonLatToXY(east, south, 0).Vector3FromVector2XZ());
        corners.Add(GeographicCoordinates.ConvertLonLatToXY(east, north, 0).Vector3FromVector2XZ());

        return corners;
    }


    public string ToQLString()
    {
        return string.Format("({0},{1},{2},{3})", south, west, north, east);
    }

}
