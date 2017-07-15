using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;

/// <summary>
/// StreetTesterEditor.
/// </summary>
[CustomEditor(typeof(StreetTester))]
public class StreetTesterEditor : Editor
{
    //StreetTester sT;

    void Awake()
    {
        //sT = (StreetTester)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

    }

}