using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;

/// <summary>
/// TriangleTesterEditor
/// </summary>
[CustomEditor(typeof(TriangleTester))]
public class TriangleTesterEditor : Editor
{
    TriangleTester tT;

    void Awake()
    {
        tT = (TriangleTester) target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (GUILayout.Button("Split"))
        {
            tT.SplitTriangle();
        }
    }

}