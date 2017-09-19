using UnityEngine;
#if UNITY_EDITOR
//using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Console.
/// </summary>
[ExecuteInEditMode]
public class Console : Singleton<Console>
{
    
    public Rect boundingBox = new Rect(0, 0, Screen.width, Screen.height);

    //    private Texture2D icon;

    [SerializeField]
    private float _buttonHeight;

    //    [SerializeField]
    //    private int _buttonWidth;

    [SerializeField]
    private float margin;

    [SerializeField]
    private float boxWidth;

    [SerializeField]
    private float boxHeight;

    [SerializeField]
    private float rowHeight;

    [SerializeField]
    private float detailBoxHeight;

    public GUIStyle style;

    private static Dictionary<string, List<string>> messages = new Dictionary<string, List<string>>();

    
    //private static List<string> messages = new List<string>();
    public bool _toggleDebugWindow = false;
    //    private static int count = 0;
    private static string boxText = string.Empty;

    // Use this for initialization
    void Start()
    {
        //        icon = Resources.Load("debug", typeof(Texture2D)) as Texture2D;

        _buttonHeight = Screen.width / 20;
        //      _buttonWidth = Screen.width / 10;

        margin = 3;
        boxWidth = boundingBox.width - 2 * margin;
        boxHeight = boundingBox.height - _buttonHeight - margin;
        rowHeight = 23;

        detailBoxHeight = boxHeight / 3;

        style = new GUIStyle();
        style.alignment = TextAnchor.MiddleLeft;
        style.normal.textColor = Color.white;
        
        messages["default"] = new List<string>();
        
    }

    // Update is called once per frame
    void Update()
    {
        boundingBox = new Rect(0, 0, Screen.width, Screen.height);

        //if running on Android, check for Menu/Home and exit
        switch (Application.platform)
        {
            case RuntimePlatform.Android:
                Debug.Log("Android");
                if (Input.GetKeyUp(KeyCode.Home))
                {
                    // whatever the home-button should do...

                    return;
                }
                else if (Input.GetKeyUp(KeyCode.Escape))
                {
                    // whatever the back-button should do...
                }
                else if (Input.GetKey(KeyCode.Menu))
                {
                    _toggleDebugWindow = !_toggleDebugWindow;
                }
                break;

            case RuntimePlatform.IPhonePlayer:
                Debug.Log("iPhone");
                // Handle iPhone Input here
                break;
            default:

                if (Input.GetKeyUp(KeyCode.Home))
                {
                    // whatever the home-button should do...
                    Application.Quit();
                    return;
                }
                else if (Input.GetKeyUp(KeyCode.Escape))
                {
                    // whatever the back-button should do...
                }
                else if (Input.GetKeyUp(KeyCode.C))
                {
                    _toggleDebugWindow = !_toggleDebugWindow;
                }

                break;
        }
    }





    public void OnGUI()
    {
        //Rect buttonRect = new Rect((Screen.width / 8) * 7 + margin, margin, Screen.width / 8 - 3 * margin, _buttonHeight - margin);
        Rect buttonRect = new Rect((boundingBox.width / 8) * 7 + margin, margin, boundingBox.width / 8 - 3 * margin, _buttonHeight - margin);

        if (_toggleDebugWindow)
        {
            if (GUI.Button(buttonRect, "Hide"))
                _toggleDebugWindow = !_toggleDebugWindow;
        }
        else
        {
            //buttonRect = new Rect((boundingBox.width / 8) * 7 + margin, margin, boundingBox.width / 8 - 3 * margin, _buttonHeight - margin);

            if (GUI.Button(buttonRect, "Show"))
                _toggleDebugWindow = !_toggleDebugWindow;
        }

        if (_toggleDebugWindow)
        {
            Rect clearButtonRect = new Rect(2 * margin, margin, (boundingBox.width / 8) * 7 - 2 * margin, _buttonHeight - margin);
            if (GUI.Button(clearButtonRect, "Clear Log"))
                ClearLog();

            LogConsoleBox();
        }
    }




    public static void AddMessage(string message)
    {
        Console.AddMessage("default", message);
    }

    public static void AddMessage(string session, string message)
    {
        List<string> messageList;
        if(!messages.TryGetValue(session, out messageList)){
            messages[session] = new List<string>();
        }

        messages[session].Add(message);
    }

    public static void ClearLog()
    {
        messages.Clear();
        boxText = string.Empty;
    }


    private Vector2 scrollPosition;
    private Vector2 scrollPositionDetailBox;

    public void LogConsoleBox()
    {
        Rect box = new Rect(Screen.width - boxWidth, Screen.height - boxHeight, boxWidth - 10, boxHeight - 10);
        GUI.Box(box, "");

        GUI.contentColor = Color.white;
        float positionX = 0;

        float scrollViewHeight = rowHeight * (messages.Count + 3);
        scrollPosition = GUI.BeginScrollView(
            new Rect(boundingBox.width - boxWidth + margin,
                boundingBox.height - boxHeight + margin,
                boxWidth - 10 - 2 * margin,
                boxHeight - 10 - detailBoxHeight - 3 * margin),
            scrollPosition,
            new Rect(2 * margin,
                0,
                boxWidth - 10 - 6 * margin,
                scrollViewHeight),
            false,
            false);


        foreach (string log in messages.Keys)
        {
            for (int i = 0; i < messages[log].Count; i++)
            {
                GUI.color = Color.white;
                string message = i + ". " + messages[log];

                if (GUI.Button(new Rect(Screen.width - boxWidth, Screen.height - boxHeight - _buttonHeight + positionX, boxWidth - 8 * margin, rowHeight), "", style))
                {
                    boxText = message;
                    Debug.Log(boxText);
                }
                GUI.Label(new Rect(Screen.width - boxWidth + 10, Screen.height - boxHeight - _buttonHeight + positionX, boxWidth - 8 * margin, rowHeight), message, style);
                positionX += rowHeight + 2;
            }
        }
        GUI.EndScrollView();

        scrollPositionDetailBox = GUI.BeginScrollView(
            new Rect(Screen.width - boxWidth + 10, Screen.height - detailBoxHeight - 10, boxWidth - 6 * margin, detailBoxHeight - 2 * margin), scrollPositionDetailBox,
            new Rect(0, 0, boxWidth - 6 * margin, scrollViewHeight), false, false);

        GUI.Box(new Rect(0, 0, boxWidth - 6 * margin, detailBoxHeight * 5), "");
        GUI.Label(new Rect(margin, margin, boxWidth - 8 * margin, detailBoxHeight - 2 * margin), boxText);

        GUI.EndScrollView();
    }

}
