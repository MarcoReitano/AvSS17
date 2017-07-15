using UnityEngine;
using System.Collections;



public enum SidebarAncor
{
    TOP,
    BOTTOM,
    LEFT,
    RIGHT
}

public class SideBar : MonoBehaviour
{

    string sidebarName = string.Empty;
    Rect sidebarRect;
    Rect openRect;
    Rect closedRect;

    bool sidebarOpen = true;

    Vector2 scrollPosition;

    public delegate void SidebarGUI();

    private SidebarGUI guiMethod;

    private bool initialized = false;
    public void Initialize(string sidebarName, Rect openRect, Rect closedRect, SidebarGUI guiMethod)
    {
        this.sidebarName = sidebarName;
        this.openRect = openRect;
        this.closedRect = closedRect;
        this.guiMethod = guiMethod;

        this.initialized = true;
    }


    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnGUI()
    {
        if (initialized)
        {
            #region sidebar

            if (sidebarOpen)
                sidebarRect = openRect;
            else
                sidebarRect = closedRect;


            CustomGUIUtils.DrawFrameBox(sidebarRect, Color.gray, 3f, Color.black);
            if (GUI.Button(new Rect(sidebarRect.xMax - 10, sidebarRect.height / 2 - 10, 20f, 20f), (sidebarOpen ? DefaultIconSet.ArrowsLeft : DefaultIconSet.ArrowsRight)))
                sidebarOpen = !sidebarOpen;
            

            if (sidebarOpen)
            {
                scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(openRect.width), GUILayout.Height(openRect.height));

                this.guiMethod();

                GUILayout.EndScrollView();
            }
            else
            {

            }



            #endregion // sidebar
        }
    }
}
