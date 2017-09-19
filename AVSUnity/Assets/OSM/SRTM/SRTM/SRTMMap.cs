using System.Globalization;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;



public class SRTMMap
{
    

    private MapBounds bounds;
    public MapBounds Bounds
    {
        get { return bounds; }
        set
        {
            bounds = value;
        }
    }

    double cellSize = GeographicCoordinates.arcsec3Meter;
    

    /// <summary>
    /// 
    /// </summary>
    public SRTMMap()
    {
        
    }

}

