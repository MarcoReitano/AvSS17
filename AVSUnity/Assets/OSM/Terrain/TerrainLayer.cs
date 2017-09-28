using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;

/// <summary>
/// TerrainLayer.
/// </summary>

public static class TerrainLayer
{
    public static void Reset()
    {
        minTerrainHeightSet = false;
        minTerrainHeightSet = false;
        minTerrainHeight = 0f;
        maxTerrainHeight = 1f;
        //Debug.Log("RESET");
    }

    public static float MinTerrainHeight
    {
        get { return minTerrainHeight; }
        set
        {
            if (!minTerrainHeightSet)
            {
                minTerrainHeight = value;
                minTerrainHeightSet = true;
            }
        }
    }
    public static float MaxTerrainHeight
    {
        get { return maxTerrainHeight; }
        set
        {
            if (!maxTerrainHeightSet)
            {
                maxTerrainHeight = value;
                maxTerrainHeightSet = true;
            }
        }
    }

    private static float minTerrainHeight;
    private static float maxTerrainHeight;
    private static bool minTerrainHeightSet = false;
    private static bool maxTerrainHeightSet = false;
}
