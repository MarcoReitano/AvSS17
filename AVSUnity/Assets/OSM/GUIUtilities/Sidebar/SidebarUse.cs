using UnityEngine;
using System.Collections;

public class SidebarUse : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

        //SideBar sidebar = GetComponent<SideBar>();

        //Rect openRect = new Rect(0, 0 + MenuSettings.MenuBarFrameThickness, Screen.width / 4, Screen.height - MenuSettings.MenuBarHeight - 3 * MenuSettings.MenuBarFrameThickness);
        //Rect closedRect = new Rect(0, 0, 5f, Screen.height - MenuSettings.MenuBarHeight - 3 * MenuSettings.MenuBarFrameThickness);
        //sidebar.Initialize(
        //    "mySideBar",
        //    openRect,
        //    closedRect,
        //    GUIMethod);
    }

    private void GUIMethod()
    {
        //if (GUILayout.Button("Button"))
        //{
        //    Debug.Log("Button clicked");
        //}
    }


    // Update is called once per frame
    void Update()
    {

    }




    void OnGUI()
    {

        CustomGUIUtils.BeginSideBar("sidebarLeft", SidebarType.LEFT, new Rect(0f, 100f, Screen.width / 4f, 300f), false, 2f, EasingCurves.EaseType.linear, 2f);

        GUILayout.BeginHorizontal();
        GUILayout.Label("WallHeight:", GUILayout.Width(70f), GUILayout.Height(30f));
        GUILayout.HorizontalSlider(2f, 0f, 10f, GUILayout.Height(30f));
        GUILayout.EndHorizontal();

        CustomGUIUtils.EndSideBar();

        CustomGUIUtils.BeginSideBar("sidebarRight",
            SidebarType.RIGHT,
            new Rect(0f, 100f, 200f, 300f),
            false,
            5f,
            EasingCurves.EaseType.easeInBounce,
            EasingCurves.EaseType.easeInOutBounce,
            2f);
        GUILayout.Button("Button");




        CustomGUIUtils.EndSideBar();

        CustomGUIUtils.BeginSideBar("sidebarTop",
            SidebarType.TOP,
            new Rect(250f, 100f, 200f, 300f),
            false,
            7f,
            EasingCurves.EaseType.easeInElastic,
            EasingCurves.EaseType.easeOutElastic,
            2f);
        GUILayout.Button("Button");
        CustomGUIUtils.EndSideBar();

        CustomGUIUtils.BeginSideBar("sidebarBottom",
            SidebarType.BOTTOM,
            new Rect(500f, 100f, 200f, 300f),
            false,
            10f,
            EasingCurves.EaseType.easeInSine,
            EasingCurves.EaseType.easeOutSine,
            2f);
        GUILayout.Button("Button");
        CustomGUIUtils.EndSideBar();


        CustomGUIUtils.BeginSideBar("sidebarMiddleTOP",
            SidebarType.TOP,
            new Rect(130, 200, 400, 200),
            new Rect(0, 0, 100f, 200f),
            false,
            10f,
            EasingCurves.EaseType.easeInSine,
            EasingCurves.EaseType.easeOutSine,
            2f);
        GUILayout.Button("Button");
        GUILayout.Button("Button");

        GUILayout.Button("Button");

        CustomGUIUtils.EndSideBar();

        CustomGUIUtils.BeginSideBar("sidebarMiddleRIGHT",
            SidebarType.RIGHT,
            new Rect(130, 200, 400, 200),
            new Rect(0, 0, 200f, 100f),
            false,
            10f,
            EasingCurves.EaseType.easeInSine,
            EasingCurves.EaseType.easeOutSine,
            2f);
        GUILayout.Button("Button");
        CustomGUIUtils.EndSideBar();

        CustomGUIUtils.BeginSideBar("sidebarMiddleLEFT",
            SidebarType.LEFT,
            new Rect(130, 200, 400, 200),
            new Rect(0, 0, 100f, 200f),
            false,
            10f,
            EasingCurves.EaseType.easeInSine,
            EasingCurves.EaseType.easeOutSine,
            2f);

        GUILayout.Button("Button");

        CustomGUIUtils.EndSideBar();

        CustomGUIUtils.BeginSideBar("sidebarMiddleBOTTOM",
            SidebarType.BOTTOM,
            new Rect(130, 200, 400, 200),
            new Rect(100, 0, 200f, 100f),
            false,
            10f,
            EasingCurves.EaseType.easeInSine,
            EasingCurves.EaseType.easeOutSine,
            2f);
        GUILayout.Button("Button");
        CustomGUIUtils.EndSideBar();




        //float lineHeight = 50f;
        //float lineSeparator = 3f;

        //Rect parent = CustomGUIUtils.BeginSideBar("sidebarLeft", SidebarType.LEFT, new Rect(0f, 100f, Screen.width / 4f, 400f), false, 2f, EasingCurves.EaseType.easeInQuart, EasingCurves.EaseType.easeOutQuart, 2f);

        //////GUILayout.BeginHorizontal();
        //////GUILayout.Label("WallHeight:", GUILayout.Width(70f), GUILayout.Height(30f));
        //////GUILayout.HorizontalSlider(2f, 0f, 10f, GUILayout.Height(30f));
        //////GUILayout.EndHorizontal();

        ////Debug.Log(parent);
        //for (int i = 0; i < 10; i++)
        //{
        //    CustomGUIUtils.BeginSideBar("sidebarLeft" + i, SidebarType.LEFT, parent, new Rect(0f, lineHeight * i + lineSeparator * i, parent.width - 20, lineHeight), false, 2f, EasingCurves.EaseType.easeInQuart, EasingCurves.EaseType.easeOutQuart, 2f);
        //    GUILayout.Label("sidebarLeft" + i);

        //    CustomGUIUtils.EndSideBar();
        //}

        //////CustomGUIUtils.BeginSideBar("sidebarMiddleBOTTOM",
        //////    SidebarType.LEFT,
        //////    parent,
        //////    new Rect(0, 0, 200f, 100f),
        //////    false,
        //////    10f,
        //////    EasingCurves.EaseType.easeInSine,
        //////    EasingCurves.EaseType.easeOutSine,
        //////    2f);
        //////GUILayout.Button("Button");
        //////CustomGUIUtils.EndSideBar();


        //if (GUILayout.Button("showDialog"))
        //{
        //    Debug.Log("clicked");
        //    showDialog = true;

        //}

        //CustomGUIUtils.EndSideBar();

        //DialogResultType result = DialogResultType.UNDEFINED;

        //if (showDialog)
        //{
        //    result = CustomGUIUtils.Dialog(DialogType.OK, "Bleh", "bleh bleh bleh...", new Rect(Screen.width / 4f, Screen.height / 4f, Screen.width / 2f, Screen.height / 2f), 1f, ref showDialog);
        //}

        //switch (result)
        //{
        //    case DialogResultType.YES:
        //        break;
        //    case DialogResultType.NO:
        //        break;
        //    case DialogResultType.CANCEL:
        //        break;
        //    case DialogResultType.OK:
        //        break;
        //    case DialogResultType.UNDEFINED:
        //        break;
        //    default:
        //        break;
        //}
    }

    bool showDialog = false;
}
