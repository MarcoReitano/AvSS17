using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(DragNDropUseInnerDrops))]
public class DragNDropEditor : Editor {

    private DragNDropUseInnerDrops dragNDropUse;
    private DND dnd;

    void Awake()
    {
        dragNDropUse = target as DragNDropUseInnerDrops;
        //dnd = new DND();

        dnd = dragNDropUse.dragNDrop;
        
    }

    void OnSceneGUI()
    {
        
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.BeginHorizontal();
        dragNDropUse.OnGUI();
        EditorGUILayout.EndHorizontal();
        //if (dnd.currentDragObject.IsDragging)
            Repaint();
    }
}
