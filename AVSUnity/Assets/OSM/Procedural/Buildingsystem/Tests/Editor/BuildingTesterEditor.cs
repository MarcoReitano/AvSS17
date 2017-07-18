using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;

/// <summary>
/// BuildingTesterEditor.
/// </summary>
[CustomEditor (typeof(BuildingTester))]
public class BuildingTesterEditor : Editor 
{
    BuildingTester bT;

    void Awake()
    {
        bT = (BuildingTester)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
		if(GUILayout.Button("Generate"))
        //if (GUI.changed)
            bT.Generate();
    }

}
