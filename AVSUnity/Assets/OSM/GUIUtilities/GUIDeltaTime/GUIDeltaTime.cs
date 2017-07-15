using UnityEngine;
using System.Collections;
using System;
using System.Diagnostics;


/// <summary>
/// This class Provides the deltaTime
/// </summary>
public class GUIDeltaTime : Singleton<GUIDeltaTime>
{
    public static float deltaTimeGUI = 0f;
    public static float deltaTime = 0f;
    private static long lastTimeGUI;
    private static long lastTime;
    private Stopwatch watch = new Stopwatch();

    void Awake()
    {
        watch.Start();
        lastTimeGUI = watch.ElapsedTicks;
        lastTime = watch.ElapsedTicks;
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        deltaTime = ((float)(watch.ElapsedTicks - lastTimeGUI) / (float)TimeSpan.TicksPerSecond);
        lastTime = watch.ElapsedTicks;
    }

    void OnGUI()
    {
        deltaTimeGUI = ((float)(watch.ElapsedTicks - lastTimeGUI) / (float)TimeSpan.TicksPerSecond);
        //DebugCheck();
        lastTimeGUI = watch.ElapsedTicks;
    }

    private void DebugCheck()
    {
        Rect drawRect = new Rect(0, Screen.height - 20, 300, 20);
        Rect drawRect2 = new Rect(0, Screen.height - 40, 300, 20);
        //if (Mathf.Abs(deltaTimeGUI - Time.deltaTime) < 0.000001f)
        //{
        //    CustomGUIUtils.DrawBox(drawRect, Color.green);
        //}
        //else
        //{
        //    CustomGUIUtils.DrawBox(drawRect, Color.red);
        //}

        //if (watch.ElapsedTicks > lastTimeGUI)
        //{
        //    CustomGUIUtils.DrawBox(drawRect2, Color.green);
        //}
        //else
        //{
        //    CustomGUIUtils.DrawBox(drawRect2, Color.red);
        //}

        GUI.Label(drawRect,  "DeltaTime:    " + deltaTime + "  =?=  " + Time.deltaTime);
        GUI.Label(drawRect2, "DeltaTimeGUI: " + deltaTimeGUI + "  =?=  " + Time.deltaTime);
        //GUI.Label(drawRect2, "DeltaTimeGUI: " + watch.ElapsedTicks + "   " + deltaTimeGUI + "   ?   " + Time.deltaTime);
    }
}
