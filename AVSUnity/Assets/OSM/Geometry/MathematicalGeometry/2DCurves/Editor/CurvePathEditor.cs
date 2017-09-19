using UnityEngine;
using System.Collections;
using UnityEditor;


[CustomEditor(typeof(CurvePath))]
public class CurvePathEditor : Editor
{

    public CurvePath path;


    void Awake()
    {
        path = (CurvePath)target;
    }

    void OnSceneGUI()
    {

    }


    private bool arcFoldout = false;
    private bool lineFoldout = false;


    public override void OnInspectorGUI()
    {

        if (arcFoldout = EditorGUILayout.Foldout(lineFoldout, "Add Line"))
        {
            EditorGUILayout.Vector3Field("Start", path.GetLastPoint());
            
            Vector3 direction = EditorGUILayout.Vector3Field("Direction", path.GetLastDirection());

            if (GUILayout.Button("Add Line"))
            {
                path.curves.Add(new PathLine(path.GetLastPoint(), direction));
            }
        }

        //if (arcFoldout = EditorGUILayout.Foldout(arcFoldout, "Add Arc"))
        //{
            

        //    if (GUILayout.Button("Add Arc"))
        //    {

        //    }
        //}


        
        

    }

}
