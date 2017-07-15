#if UNITY_EDITOR
using System;
using UnityEngine;
using System.Reflection;
using UnityEditorInternal;
using UnityEditor;

public class CustomEditorUtils
{

    /// <summary>
    /// 
    /// </summary>
    /// <param name="changed"></param>
    /// <param name="position"></param>
    /// <returns></returns>
    public static bool FreeMoveHandleChangedPosition(ref bool changed, ref Vector3 position)
    {
        Vector3 oldPosition = position;
        position = Handles.FreeMoveHandle(position, Quaternion.identity, 0.2f, Vector3.zero, Handles.SphereCap);
        if (oldPosition != position)
            changed = true;
        return changed;
    }



    /// <summary>
    /// 
    /// </summary>
    /// <param name="changed"></param>
    /// <param name="position"></param>
    /// <returns></returns>
    public static Vector3 FreeMoveHandleChangedPosition(ref bool changed, Vector3 position)
    {
        Vector3 oldPosition = position;
        position = Handles.FreeMoveHandle(position, Quaternion.identity, 0.2f, Vector3.zero, Handles.SphereCap);
        if (oldPosition != position)
            changed = true;
        return position;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="changed"></param>
    /// <param name="position"></param>
    /// <returns></returns>
    public static Vector3 SimpleFreeMoveHandle(Vector3 position)
    {
        position = Handles.FreeMoveHandle(position, Quaternion.identity, 0.2f, Vector3.zero, Handles.SphereCap);
        return position;
    }


    /// <summary>
    /// 
    /// </summary>
    //public static void ClearLog()
    //{
    //    Assembly assembly = Assembly.GetAssembly(typeof(Macros));
    //    Type type = assembly.GetType("UnityEditorInternal.LogEntries");
    //    MethodInfo method = type.GetMethod("Clear");
    //    method.Invoke(new object(), null);
    //}



    private static int[] selectionIDs;
    /// <summary>
    /// 
    /// </summary>
    public static void OnSelectionChange()
    {
        selectionIDs = Selection.instanceIDs;
    }

    /// <summary>
    /// 
    /// </summary>
    public static void SaveSelection()
    {
        selectionIDs = Selection.instanceIDs;
        string saveStr = "";
        foreach (int i in selectionIDs)
            saveStr += i.ToString() + ";";
        saveStr = saveStr.TrimEnd(char.Parse(";"));
        EditorPrefs.SetString("SelectedIDs", saveStr);
    }

    /// <summary>
    /// 
    /// </summary>
    public static void LoadLastSavedSelection()
    {
        string[] strIDs = EditorPrefs.GetString("SelectedIDs").Split(char.Parse(";"));

        int[] ids = new int[strIDs.Length];
        for (int i = 0; i < strIDs.Length; i++)
            ids[i] = int.Parse(strIDs[i]);
        Selection.instanceIDs = ids;
    }


     //if (GUILayout.Button("ClearLog"))
     //       {
     //           Assembly assembly = Assembly.GetAssembly(typeof(Macros));
                
     //           Type type = assembly.GetType("UnityEditorInternal.LogEntries");
     //           MemberInfo[] members = type.FindMembers(MemberTypes.All, BindingFlags.Public, null, null);
     //           Debug.Log(members.Length);
     //           foreach (MemberInfo member in members)
     //           {
     //               Debug.Log(member.Name);
     //           }

     //           // just members which are methods beginning with Get
     //           MemberInfo[] mbrInfoArray =
     //              type.FindMembers(MemberTypes.All,
     //                 BindingFlags.Public |
     //                 BindingFlags.Static |
     //                 BindingFlags.NonPublic |
     //                 BindingFlags.Instance |
     //                 BindingFlags.DeclaredOnly,
     //                 Type.FilterName, "*");
     //           foreach (MemberInfo mbrInfo in mbrInfoArray)
     //           {
     //               Debug.Log(mbrInfo.ToString() + "  :  " + mbrInfo.MemberType.ToString());
     //           }
     //           //MethodInfo method = type.GetMethod("Clear");
     //           //method.Invoke(new object(), null);
     //       }

}
#endif
