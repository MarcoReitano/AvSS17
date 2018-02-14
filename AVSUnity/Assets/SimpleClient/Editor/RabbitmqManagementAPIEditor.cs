using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RabbitmqManagementAPI))]
public class RabbitmqManagementAPIEditor : Editor
{
    RabbitmqManagementAPI api;

    void Awake()
    {
        api = target as RabbitmqManagementAPI;
        overview = api.Overview();
    }


    RabbitmqWebRequest overview;
    public override void OnInspectorGUI()
    {
        if (overview != null)
        {
            EditorGUILayout.LabelField("Overview", overview.GetResult());
        }

    }
}
