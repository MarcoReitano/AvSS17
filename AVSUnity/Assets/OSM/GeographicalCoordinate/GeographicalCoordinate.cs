using UnityEngine;
#if UNITY_EDITOR
//using UnityEditor;
#endif
using System.Collections;

/// <summary>
/// GeographicalCoordinate.
/// </summary>
public struct GeographicalCoordinate : IGeographicalCoordinate
{
    public GeographicalCoordinate(float latitude, float longitude)
    {
        this.latitude = latitude;
        this.longitude = longitude;
    }

    public float Latitude
    {
        get
        {
            return latitude;
        }
        set
        {
            if (value < -90f || value > 90f)
                throw new System.ArgumentOutOfRangeException();
            latitude = value;
        }
    }
    private float latitude;

    public float Longitude
    {
        get
        {
            return longitude;
        }
        set
        {
            if(value < -180f || value > 180f)
                throw new System.ArgumentOutOfRangeException();
            longitude = value;
        }
    }
    private float longitude;

    public override string ToString()
    {
        return string.Format("{0:0.000000}°, {1:0.000000}°", Latitude, Longitude);
    }
}
