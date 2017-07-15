using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// LayerTool.
/// </summary>

[InitializeOnLoad]
public static class LayerTool
{
    public static bool Terrain
    {
        get { return terrain; }
        set 
        {
            if (terrain != value)
            {
                setTerrainVisibility(value);
                terrain = value;
            }
        }
    }
    public static bool ShowOSMTileMap
    {
        get { return showOSMTileMap; }
        set 
        {
            if (showOSMTileMap != value)
            {
                showOSMTileMap = value;
                setOSMTileMapVisibility(value);
            }
        }
    }
    public static bool OSMGizmoMap
    {
        get { return oSMGizmoMap; }
        set {
#if UNITY_EDITOR
            if (oSMGizmoMap != value)
            {
                oSMGizmoMap = value;
                SceneView.RepaintAll();
                return;
            }
#endif
            oSMGizmoMap = value; 
        }
    }
    public static bool OSMGizmoMapOnlyOnSelected
    {
        get { return oSMGizmoMApOnlyOnSelected; }
        set 
        { 
#if UNITY_EDITOR
            if (OSMGizmoMapOnlyOnSelected != value)
            {
                oSMGizmoMApOnlyOnSelected = value;
                SceneView.RepaintAll();
                return;
            }
#endif
            oSMGizmoMApOnlyOnSelected = value;
        }
 
    }
    public static bool ProceduralMeshes
    {
        get { return proceduralMeshes; }
        set {
            if (proceduralMeshes != value)
            {
                proceduralMeshes = value;
                setProceduralMeshVisibility(value);
            }
        }
    }
    public static bool BackgroundMeshes
    {
        get { return backgroundMeshes; }
        set 
        {
            if (backgroundMeshes != value)
            {
                backgroundMeshes = value;
                setBackgroundMeshVisibility(value);
            }
        }
    }
    public static bool BuildingSystem
    {
        get { return buildingSystem; }
        set 
        {
            if (buildingSystem != value)
            {
                buildingSystem = value;
                setBuildingMeshVisibility(value);
            }
        }
    }
    public static bool StreetSystem
    {
        get { return streetSystem; }
        set {
            if (streetSystem != value)
            {
                streetSystem = value;
                setStreetMeshVisibility(value);
            }
        }
    }
    public static bool OtherProcMesh
    {
        get { return otherProcMesh; }
        set 
        {
            if (otherProcMesh != value)
            {
                otherProcMesh = value;
                setOtherProcMeshVisibility(value);
            }
        }
    }

    private static bool terrain = true;
    private static bool showOSMTileMap = true;
    private static bool oSMGizmoMap = true;
    private static bool oSMGizmoMApOnlyOnSelected = true;
    private static bool proceduralMeshes = true;
    private static bool backgroundMeshes = true;
    private static bool buildingSystem = true;
    private static bool streetSystem = true;
    private static bool otherProcMesh = true;

    private static void setTerrainVisibility(bool value)
    {
        foreach (KeyValuePair<string, Tile> kV in TileManager.tiles)
        {
            Terrain t = kV.Value.GetComponent<Terrain>();
            if (t != null)
                t.enabled = value;
        }        
    }
    private static void setOSMTileMapVisibility(bool value)
    {
        OSMTileProvider.SetZoomLevelVisible(OSMTileProviderBehaviour.CurrentZoomLevel, value); 
    }
    private static void setProceduralMeshVisibility(bool value)
    {
        foreach (KeyValuePair<string, Tile> kV in TileManager.tiles)
        {
            for (int i = 0; i < kV.Value.transform.childCount; i++)
            {
                kV.Value.transform.GetChild(i).GetComponent<MeshRenderer>().enabled = value;
                kV.Value.transform.GetChild(i).GetComponent<MeshCollider>().enabled = value;
            }
        }
    }
    private static void setBackgroundMeshVisibility(bool value)
    {
        foreach(KeyValuePair<string, Tile> kV in TileManager.tiles)
        {
            for (int i = 0; i < kV.Value.transform.childCount; i++)
            {
                if (kV.Value.transform.GetChild(i).name == "BackgroundMesh")
                    kV.Value.transform.GetChild(i).gameObject.SetVisibleRecursively(value);
            }
        } 
    }
    private static void setBuildingMeshVisibility(bool value)
    {
        foreach (KeyValuePair<string, Tile> kV in TileManager.tiles)
        {
            for (int i = 0; i < kV.Value.transform.childCount; i++)
            {
                if (kV.Value.transform.GetChild(i).name == "BuildingMesh")
                {
                    kV.Value.transform.GetChild(i).gameObject.SetVisibleRecursively(value);

                    //Also deactivate Collider
                    MeshCollider[] meshColliders = kV.Value.transform.GetChild(i).gameObject.GetComponentsInChildren<MeshCollider>();
                    foreach (MeshCollider meshCollider in meshColliders)
                        meshCollider.enabled = value;
                }
            }
        } 
    }
    private static void setStreetMeshVisibility(bool value)
    {
        foreach (KeyValuePair<string, Tile> kV in TileManager.tiles)
        {
            for (int i = 0; i < kV.Value.transform.childCount; i++)
            {
                if (kV.Value.transform.GetChild(i).name == "StreetMesh")
                    kV.Value.transform.GetChild(i).gameObject.SetVisibleRecursively(value);
            }
        } 
    }
    private static void setOtherProcMeshVisibility(bool value)
    {
        foreach (KeyValuePair<string, Tile> kV in TileManager.tiles)
        {
            for (int i = 0; i < kV.Value.transform.childCount; i++)
            {
                if (kV.Value.transform.GetChild(i).name == "OtherMesh")
                    kV.Value.transform.GetChild(i).gameObject.SetVisibleRecursively(value);
            }
        }
 
    }
}
