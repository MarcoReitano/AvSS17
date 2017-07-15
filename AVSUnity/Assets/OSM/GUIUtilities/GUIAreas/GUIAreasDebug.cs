using UnityEngine;
using System.Collections;

public class GUIAreasDebug : MonoBehaviour {


    void OnGUI()
    {
        // Just draw the GUIArea Rects
        GUIAreas.DrawGUIAreas();
    }
}
