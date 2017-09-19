using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;

/// <summary>
/// TriangleTesterEditor
/// </summary>
[CustomEditor(typeof(TriangulationTester))]
public class TriangulationTesterEditor : Editor
{
    TriangulationTester tT;

    void Awake()
    {
        tT = (TriangulationTester)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (GUILayout.Button("GenerateRandomPoints"))
        {
            tT.GenerateRandomPoints();
        }
        if (GUILayout.Button("Triangulate"))
        {
            tT.Triangulate();
        }
    }

}