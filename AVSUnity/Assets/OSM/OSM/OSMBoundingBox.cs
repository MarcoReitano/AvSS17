using UnityEngine;
#if UNITY_EDITOR
//using UnityEditor;
#endif
using System.Collections;

/// <summary>
/// OSMBoundingBox.
/// </summary>
[System.Serializable]
public struct OSMBoundingBox
{
    public OSMBoundingBox(double minLong, double minLat, double maxLong, double maxLat)
    {
        this.minLongitude = minLong;
        this.minLatitude = minLat;
        this.maxLongitude = maxLong;
        this.maxLatitude = maxLat;
    }

    public double MinLongitude
    {
        get { return minLongitude; }
        set { minLongitude = value; }
    }
    public double MinLatitude
    {
        get { return minLatitude; }
        set { minLatitude = value; }
    }
    public double MaxLongitude
    {
        get { return maxLongitude; }
        set { maxLongitude = value; }
    }
    public double MaxLatitude
    {
        get { return maxLatitude; }
        set { maxLatitude = value; }
    }

    [SerializeField]
    private double minLongitude; // left
    [SerializeField]
    private double minLatitude; // bottom
    [SerializeField]
    private double maxLongitude; // right
    [SerializeField]
    private double maxLatitude; // top

    public string ToQLString()
    {
        return string.Format("({0},{1},{2},{3})", minLatitude, minLongitude, maxLatitude, maxLongitude);
    }
}
