using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// TileEditor.
/// </summary>
[CustomEditor (typeof(Tile))]
public class TileEditor : Editor 
{
    Tile tile;

    void Awake()
    {
        this.tile = (Tile)target; 
    }

    static bool drawDefaultInspector = true;

    public override void OnInspectorGUI()
    {
        drawDefaultInspector = EditorGUILayout.Foldout(drawDefaultInspector, "DefaultInspector");
        if (drawDefaultInspector)
        {
            EditorGUI.indentLevel++;
            DrawDefaultInspector();
            EditorGUI.indentLevel--;
        }

        if (GUILayout.Button("UpdateTile"))
            tile.StartQuery();
    }

    public void OnSceneGUI()
    {
        if (OSMMapTools.DrawAsHandles) //Only draw Handles when control down, otherwise draw Gizmos from Tile.OnDrawGizmos
        {
            if (LayerTool.OSMGizmoMapOnlyOnSelected)
            {
                if (tile.Query != null)
                {
                    if (tile.Query.OSM != null)
                        tile.Query.OSM.OnSceneGUI();
                }
            }
            else
            {
                foreach (KeyValuePair<string, Tile> kV in TileManager.tiles)
                {
                    if (kV.Value.Query != null)
                    {
                        if (kV.Value.Query.OSM != null)
                        {
                            kV.Value.Query.OSM.OnSceneGUI();
                        }
                    }
                }
            }
        }
    }


}
