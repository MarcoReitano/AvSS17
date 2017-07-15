using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(GUIAreasDebug))]
public class GUIAreaDebugEditor : Editor {


	void Awake () {
        
	}
	
	void OnSceneGUI() {
	
	}

    public override void OnInspectorGUI()
    {
        Color oldColor = GUIAreas.color;
        Color newColor = EditorGUILayout.ColorField(GUIAreas.color);
        if (oldColor != newColor)
            GUIAreas.color = newColor;
    }
}
