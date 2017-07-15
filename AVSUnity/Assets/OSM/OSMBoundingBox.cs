using System;
using OSM;

public class OSMBoundingBox
{
    public OSMBoundingBox(double minLat, double minLong, double maxLat, double maxLong)
    {
        if (minLat < maxLat && minLong < maxLong)
        {
            this.minLatitude = minLat;
            this.maxLatitude = maxLat;
            this.minLongitude = minLong;
            this.maxLongitude = maxLong;
        }
        else
            throw new OSMBoundingBoxException();
    }
    private double minLatitude;
    private double maxLatitude;
    private double minLongitude;
    private double maxLongitude;

    public double MinLatitude
    {
        get
        {
            return minLatitude;
        }

        set
        {
            minLatitude = value;
        }
    }

    public double MaxLatitude
    {
        get
        {
            return maxLatitude;
        }

        set
        {
            maxLatitude = value;
        }
    }

    public double MinLongitude
    {
        get
        {
            return minLongitude;
        }

        set
        {
            minLongitude = value;
        }
    }

    public double MaxLongitude
    {
        get
        {
            return maxLongitude;
        }

        set
        {
            maxLongitude = value;
        }
    }

    public string toQueryLanguage()
    {
        return string.Format("({0},{1},{2},{3})", minLatitude, minLongitude, maxLatitude, maxLongitude);
    }

}
