using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(SRTMTest))]
public class SRTMTestEditor : Editor {

    SRTMTest test;
	// Use this for initialization
	void Awake () {
        test = target as SRTMTest;
	}

    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Check File"))
        {
            SRTMDataCell cell = new SRTMDataCell((int)TileManager.OriginLatitude, (int)TileManager.OriginLongitude);

            
        }
    }
}
