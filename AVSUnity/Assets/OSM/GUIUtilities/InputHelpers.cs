using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
#if UNITY_EDITOR
using UnityEditor;
#endif


public class InputHelpers
{

    private static Plane plane = new Plane(Vector3.up, Vector3.zero);

    public static Vector3 GetXZPlaneCollisionInEditor(UnityEngine.Event currentEvent)
    {
        Vector3 returnCollision = Vector3.zero;

        if (Camera.current != null)
        {
            Camera sceneCamera = Camera.current;

            Vector2 correctedMousePosition = new Vector2(currentEvent.mousePosition.x, Screen.height - currentEvent.mousePosition.y - 35);
            Ray terrainRay = sceneCamera.ScreenPointToRay(correctedMousePosition);

            float enter;
            if (plane.Raycast(terrainRay, out enter))
            {
                returnCollision = sceneCamera.transform.position + terrainRay.direction.normalized * enter;
            }
        }
        return returnCollision;
    }


    public static Vector3 GetXZPlaneCollisionInEditorFixedHeight(float y, UnityEngine.Event currentEvent)
    {
        Vector3 returnCollision = Vector3.zero;

        if (Camera.current != null)
        {
            Camera sceneCamera = Camera.current;

            //Vector2 correctedMousePosition = new Vector2(currentEvent.mousePosition.x, Screen.height - currentEvent.mousePosition.y - 35);
            //Ray terrainRay = sceneCamera.ScreenPointToRay(correctedMousePosition);

            //Vector2 correctedMousePosition = new Vector2(currentEvent.mousePosition.x, Screen.height - currentEvent.mousePosition.y - 35);
            Ray terrainRay = sceneCamera.ScreenPointToRay(Input.mousePosition);

            float enter;
            Plane planeWithHeight = new Plane(Vector3.up, new Vector3(0f, y, 0f));
            if (planeWithHeight.Raycast(terrainRay, out enter))
            {
                returnCollision = sceneCamera.transform.position + terrainRay.direction.normalized * enter;
            }
        }
        returnCollision.y = y;
        return returnCollision;
    }



    public static bool GetPointOnPlane(Vector2 point, Plane plane, out Vector3 hitPoint)
    {
        if (Camera.main != null)
        {
            Vector3 screenPoint = new Vector3(point.x, point.y, Camera.main.transform.position.y);


            hitPoint = Camera.main.ScreenToWorldPoint(screenPoint);
            hitPoint.y = plane.distance;
            return true;
        }
        hitPoint = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
        return false;
    }




    public static bool GetPointOnPlane(Plane plane, out Vector3 hitPoint)
    {
        if (Camera.main != null)
        {
            if (Camera.main.orthographic)
            {
                Vector3 screenPoint;
                if (Event.current != null)
                    screenPoint = new Vector3(Event.current.mousePosition.x, Event.current.mousePosition.y, Camera.main.transform.position.y);
                else
                    screenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.transform.position.y);

                hitPoint = Camera.main.ScreenToWorldPoint(screenPoint);
                hitPoint.y = plane.distance;
                return true;
            }
            else
            {
                //Debug.Log("MousePosition: " + Input.mousePosition);
                Ray terrainRay = Camera.main.ScreenPointToRay(Input.mousePosition);

                float enter;
                if (plane.Raycast(terrainRay, out enter))
                {
                    hitPoint = Camera.main.transform.position + terrainRay.direction.normalized * enter;
                    return true;
                }
            }
        }
        hitPoint = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
        return false;
    }




    //public static bool GetPointOnPlane2(Plane plane, out Vector3 hitPoint)
    //{
    //    if (Camera.main != null)
    //    {
    //        //Vector3 screenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.transform.position.y);
    //        Vector3 screenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.transform.position.y);

    //        hitPoint = Camera.main.ScreenToWorldPoint(screenPoint);
    //        hitPoint.y = plane.distance;
    //        return true;
    //    }
    //    hitPoint = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
    //    return false;
    //}


    //public static bool GetXZPlaneCollision(out Vector3 hitPoint, int layerMask)
    //{
    //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    //    RaycastHit hit;

    //    if (Physics.Raycast(ray, out hit, layerMask))
    //    {
    //        hitPoint = hit.point;
    //        return true;
    //    }
    //    hitPoint = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
    //    return false;
    //}




    ///// <summary>
    ///// 
    ///// </summary>
    ///// <returns></returns>
    //public static bool ClickedLeft()
    //{
    //    if (Input.GetMouseButtonUp(0))
    //    {
    //        clickTime = Time.realtimeSinceStartup;
    //        return true;
    //    }


    //    //if (Input.GetMouseButtonUp(0))
    //    //{
    //    //    clickTime = EditorApplication.timeSinceStartup;
    //    //    return true;
    //    //    //double timeSinceLastClick = Time.realtimeSinceStartup - clickTime;
    //    //    //AndroidDebug.Log("time since last click: " + timeSinceLastClick);
    //    //    //if (timeSinceLastClick > minClicktime)
    //    //    //{

    //    //    //    clickTime = Time.realtimeSinceStartup;
    //    //    //    //Input.ResetInputAxes();
    //    //    //    AndroidDebug.Log("time since last click: " + timeSinceLastClick + " > " + minClicktime + "  = true");
    //    //    //    return true;
    //    //    //}
    //    //    //AndroidDebug.Log("time since last click: " + timeSinceLastClick + " > " + minClicktime + "  = false");
    //    //    ////clickTime = Time.realtimeSinceStartup;
    //    //}

    //    #region click varianten
    //    //switch (Application.platform)
    //    //{
    //    //    case RuntimePlatform.Android:
    //    //        if (Input.GetMouseButtonUp(0))
    //    //        {
    //    //            if ((Time.realtimeSinceStartup - clickTime) > minClicktime)
    //    //            {
    //    //                return true;
    //    //            }
    //    //            clickTime = Time.realtimeSinceStartup;
    //    //        }
    //    //        break;
    //    //    case RuntimePlatform.FlashPlayer:
    //    //        break;
    //    //    case RuntimePlatform.IPhonePlayer:
    //    //        break;
    //    //    case RuntimePlatform.LinuxPlayer:
    //    //        break;
    //    //    case RuntimePlatform.NaCl:
    //    //        break;
    //    //    case RuntimePlatform.OSXDashboardPlayer:
    //    //        break;
    //    //    case RuntimePlatform.OSXEditor:
    //    //        break;
    //    //    case RuntimePlatform.OSXPlayer:
    //    //        break;
    //    //    case RuntimePlatform.OSXWebPlayer:
    //    //        break;
    //    //    case RuntimePlatform.PS3:
    //    //        break;
    //    //    case RuntimePlatform.WiiPlayer:
    //    //        break;
    //    //    case RuntimePlatform.WindowsEditor:
    //    //        break;
    //    //    case RuntimePlatform.WindowsPlayer:
    //    //        break;
    //    //    case RuntimePlatform.WindowsWebPlayer:
    //    //        break;
    //    //    case RuntimePlatform.XBOX360:
    //    //        break;
    //    //    default:
    //    //        break;
    //    //}


    //    //#if UNITY_EDITOR
    //    //        Event currentEvent = UnityEngine.Event.current;
    //    //        if (currentEvent != null)
    //    //        {
    //    //            if (currentEvent.type == EventType.MouseDown && currentEvent.button == 0)
    //    //            {
    //    //                return true;
    //    //            }
    //    //        }
    //    //#else
    //    //        if (Input.GetMouseButtonUp(0))
    //    //        {
    //    //            if ((Time.realtimeSinceStartup - clickTime) > minClicktime)
    //    //            {
    //    //                return true;
    //    //            }
    //    //            clickTime = Time.realtimeSinceStartup;
    //    //        }

    //    ////        if (Input.GetMouseButtonUp(0)){
    //    ////            return true;
    //    ////        }
    //    ////        //switch (Application.platform)
    //    ////        //{
    //    ////        //    case RuntimePlatform.Android:
    //    ////        //        Debug.Log("Android");
    //    ////        //        if (Input.GetMouseButtonUp(0))
    //    ////        //            return true;
    //    ////        //        break;
    //    ////        //    case RuntimePlatform.IPhonePlayer:
    //    ////        //        // Handle iPhone Input here
    //    ////        //        if (Input.GetMouseButtonUp(0))
    //    ////        //            return true;
    //    ////        //        break;
    //    ////        //    default:
    //    ////        //       if (Input.GetMouseButtonUp(0))
    //    ////        //            return true;
    //    ////        //        break;
    //    ////        //}

    //    //#endif
    //    #endregion // click varianten 
    //    return false;
    //}

    // /// <summary>
    ///// 
    ///// </summary>
    ///// <returns></returns>
    //public static bool ClickedLeft(out bool doubleClicked)
    //{
    //    doubleClicked = false;

    //    if (Input.GetMouseButtonUp(0))
    //    {
    //        //clickTime = Time.realtimeSinceStartup;
    //        return true;
    //    }

    //    //TODO: richtiges doubleClick testen
    //    //if (Input.GetMouseButtonUp(0))
    //    //{

    //    //    double timeDifference = Time.time - clickTime;
    //    //    Debug.Log("Click " + clickTime +  "  difference: " + timeDifference);

    //    //    if (clickTime > 0 && timeDifference < doubleClickTime)
    //    //    {
    //    //        Debug.Log("Double Click " + timeDifference);
    //    //        doubleClicked = true;
    //    //        clickTime = -1;
    //    //    }
    //    //    else
    //    //    {
    //    //        Debug.Log("Single Click " + timeDifference);
    //    //        clickTime = Time.time;
    //    //        Debug.Log("Set Clicktime: " + clickTime);
    //    //    }
    //    //    return true;
    //    //}
    //    return false;
    //}


    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public static bool ClickedLeft()
    {
#if UNITY_EDITOR
        Event currentEvent = UnityEngine.Event.current;
        if (currentEvent != null)
        {
            if (currentEvent.type == EventType.MouseUp && currentEvent.button == 0)
                return true;
        }
#else
         if (Input.GetMouseButtonUp(0))
            return true;
#endif
        return false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public static bool ClickedRight()
    {
#if UNITY_EDITOR
        Event currentEvent = UnityEngine.Event.current;
        if (currentEvent != null)
        {
            if (currentEvent.type == EventType.MouseUp && currentEvent.button == 1)
                return true;
        }
#else
         if (Input.GetMouseButtonUp(1))
            return true;
#endif
        return false;
    }



    private static double clickTime;// = -1;
    private static double doubleClickTime = 0.4f;

    private static double minClicktime = 0.5f;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="button"></param>
    /// <returns></returns>
    public static bool DoubleClicked(int button)
    {
#if UNITY_EDITOR
        Event currentEvent = UnityEngine.Event.current;
        if (currentEvent != null)
        {
            if (currentEvent.type == EventType.MouseUp
                && currentEvent.button == button)
            {
                if ((EditorApplication.timeSinceStartup - clickTime) < doubleClickTime)
                {
                    return true;
                }
                clickTime = EditorApplication.timeSinceStartup;
            }
        }
#else
         if(Input.GetMouseButtonUp(button))
        {
            if ((Time.realtimeSinceStartup - clickTime) < doubleClickTime)
            {
                return true;
            }
            clickTime = Time.realtimeSinceStartup;
        }
#endif
        return false;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public static bool DoubleLeftClicked()
    {
        return DoubleClicked(0);
    }


    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public static bool DoubleRightClicked()
    {
        return DoubleClicked(1);
    }


    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public static bool DoubleMiddleClicked()
    {
        return DoubleClicked(2);
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public static bool PressedControl(KeyCode key)
    {
#if UNITY_EDITOR
        Event currentEvent = UnityEngine.Event.current;
        if (currentEvent != null)
        {
            if (currentEvent.control && currentEvent.keyCode == key && currentEvent.type == EventType.KeyUp)
                return true;
        }
#else
        if ((Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl))
            && Input.GetKeyUp(key))
            return true;
#endif
        return false;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public static bool IsPressingControl(KeyCode key)
    {
#if UNITY_EDITOR
        Event currentEvent = UnityEngine.Event.current;
        if (currentEvent != null)
        {
            if (currentEvent.control && currentEvent.keyCode == key && currentEvent.type == EventType.KeyDown)
                return true;
        }
#else
        if ((Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl))
            && Input.GetKeyDown(key))
            return true;
#endif
        return false;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public static bool Pressed(KeyCode key)
    {
#if UNITY_EDITOR
        Event currentEvent = UnityEngine.Event.current;
        if (currentEvent != null)
        {
            if (currentEvent.isKey && currentEvent.keyCode == key && currentEvent.type == EventType.KeyUp)
                return true;
        }
#else
        if(Input.GetKeyUp(key))
            return true;
       
#endif
        return false;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public static bool IsPressing(KeyCode key)
    {
#if UNITY_EDITOR
        Event currentEvent = UnityEngine.Event.current;
        if (currentEvent != null)
        {
            if (currentEvent.isKey && currentEvent.keyCode == key && currentEvent.type == EventType.KeyDown)
                return true;
        }
#else
        if(Input.GetKeyDown(key))
            return true;
#endif
        return false;
    }





    /// <summary>
    /// 
    /// </summary>
    /// <param name="button"></param>
    /// <returns></returns>
    public static bool MouseUp(int button)
    {
#if UNITY_EDITOR
        Event currentEvent = UnityEngine.Event.current;
        if (currentEvent != null)
        {
            if (currentEvent.type == EventType.mouseUp && currentEvent.button == button)
                return true;
        }
#else
        if (Input.GetMouseButtonUp(button))
            return true;
#endif
        return false;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="button"></param>
    /// <returns></returns>
    public static bool MouseDown(int button)
    {
#if UNITY_EDITOR
        Event currentEvent = UnityEngine.Event.current;
        if (currentEvent != null)
        {
            if (currentEvent.type == EventType.mouseDown && currentEvent.button == button)
                return true;
        }
#else
        if (Input.GetMouseButtonDown(button))
            return true;
#endif
        return false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="button"></param>
    /// <returns></returns>
    public static bool MouseMoved(int button)
    {
#if UNITY_EDITOR
        Event currentEvent = UnityEngine.Event.current;
        if (currentEvent != null)
        {
            if (currentEvent.type == EventType.mouseDrag && currentEvent.button == button)
                return true;
        }
#else
        if (Input.GetMouseButton(button))
            return true;
#endif
        return false;
    }



    private static Dictionary<string, bool> isRunningDict = new Dictionary<string, bool>();
    private static Dictionary<string, Vector2> oldMousePositionDict = new Dictionary<string, Vector2>();
    private static Dictionary<string, float> startTimeDict = new Dictionary<string, float>();
    private static Dictionary<string, Rect> rectDict = new Dictionary<string, Rect>();

    public static bool MouseHasNotMovedForSeconds(string sessionID, float seconds)
    {
        bool result = false;
        bool isRunning;

        if (!isRunningDict.TryGetValue(sessionID, out isRunning))
        {
            isRunningDict.Add(sessionID, true);
            startTimeDict.Add(sessionID, Time.realtimeSinceStartup);
            oldMousePositionDict.Add(sessionID, Event.current.mousePosition);
        }
        else
        {
            if (isRunning)
            {
                float startTime;
                if (!startTimeDict.TryGetValue(sessionID, out startTime))
                    EndRunMouseMoved(sessionID);

                Vector2 oldMousePosition;
                if (!oldMousePositionDict.TryGetValue(sessionID, out oldMousePosition))
                    EndRunMouseMoved(sessionID);


                if (oldMousePosition == Event.current.mousePosition)
                {
                    float elapsedTime = Time.realtimeSinceStartup - startTime;
                    if (elapsedTime >= seconds)
                    {
                        EndRunMouseMoved(sessionID);
                        return true;
                    }
                }
                else
                {
                    EndRunMouseMoved(sessionID);
                }
            }
        }
        return result;
    }

    private static void EndRunMouseMoved(string sessionID)
    {
        isRunningDict.Remove(sessionID);
        oldMousePositionDict.Remove(sessionID);
        startTimeDict.Remove(sessionID);
    }



    public static bool MouseInRectForSeconds(string sessionID, Rect rect, float seconds)
    {
        bool result = false;
        bool isRunning;

        if (!isRunningDict.TryGetValue(sessionID, out isRunning))
        {
            isRunningDict.Add(sessionID, true);
            startTimeDict.Add(sessionID, Time.realtimeSinceStartup);
            rectDict.Add(sessionID, rect);
        }
        else
        {
            if (isRunning)
            {
                float startTime;
                if (!startTimeDict.TryGetValue(sessionID, out startTime))
                    EndRunMouseInRect(sessionID);

                Rect theRect;
                if (!rectDict.TryGetValue(sessionID, out theRect))
                    EndRunMouseInRect(sessionID);


                if (theRect.Contains(Event.current.mousePosition))
                {
                    float elapsedTime = Time.realtimeSinceStartup - startTime;
                    if (elapsedTime >= seconds)
                    {
                        EndRunMouseInRect(sessionID);
                        return true;
                    }
                }
                else
                {
                    EndRunMouseInRect(sessionID);
                }
            }
        }
        return result;
    }

    private static void EndRunMouseInRect(string sessionID)
    {
        isRunningDict.Remove(sessionID);
        rectDict.Remove(sessionID);
        startTimeDict.Remove(sessionID);
    }



    //public static void DrawMousePositionWorldPoint(float height){

    //    Vector3 worldpoint = Camera.main.ScreenToWorldPoint(new Vector3(touch.x, Screen.height - touch.y, 0f));

    //}


    public static Vector3 MouseOnGUI()
    {
        Vector3 mousePosition;
        Vector3 mousePositionGUI;
#if UNITY_EDITOR

        mousePosition = Event.current.mousePosition;
        mousePositionGUI = mousePosition;
#else
        mousePosition = Input.mousePosition;
        mousePositionGUI = new Vector3(mousePosition.x, Screen.height - mousePosition.y, mousePosition.z);
#endif

        //Debug.Log(mousePositionGUI);
        return mousePositionGUI;
    }


    public static List<Vector3> TouchesToWorldpoints(float height)
    {
        List<Vector3> touches = new List<Vector3>();

        for (int i = 0; i < Input.touchCount; i++)
        {
            Vector2 touch = Input.touches[i].position;
            Vector3 worldpoint = Camera.main.ScreenToWorldPoint(new Vector3(touch.x, Screen.height - touch.y, 0f));
            touches.Add(worldpoint);
        }

        return touches;
    }


    public static string PrintInputInfos()
    {
        StringBuilder sb = new StringBuilder();
        if (Input.GetMouseButtonDown(0))
            sb.Append("########  MouseButtonDown(0)\n");
        if (Input.GetMouseButton(0))
            sb.Append("########  MouseButton(0)\n");
        if (Input.GetMouseButtonUp(0))
            sb.Append("########  MouseButtonUp(0)\n");

        if (Event.current != null)
            sb.Append("Event.current.mouseposition = ").AppendLine(Event.current.mousePosition.ToString());
        else
            sb.Append("Event.current.mouseposition = ").AppendLine("not available");

        sb.Append("Input.mouseposition = ").AppendLine(Input.mousePosition.ToString());

        sb.Append("\nTouchCount = " + Input.touchCount + "\n");
        for (int i = 0; i < Input.touchCount; i++)
        {
            sb.Append("Input.touches[" + i + "].position = ").Append(Input.touches[i].position.ToString()).Append("  delta = ").Append(Input.touches[i].deltaPosition.ToString()).Append("\n");
        }

        return sb.ToString();
    }


    private static bool tapping;
    private static float LastTap;
    private static float tapTime;

    public static bool DoubleTap()
    {
        bool doubleTap = false;
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Stationary)
            {
                if (!tapping)
                {
                    tapping = true;
                    SingleTap();
                }

                if ((Time.time - LastTap) < tapTime)
                {
                    Debug.Log("DoubleTap");
                    doubleTap = true;
                    tapping = false;
                }
                LastTap = Time.time;

            }
        }
        return doubleTap;
    }

    public static void SingleTap()
    {
        if (tapping)
        {
            Debug.Log("SingleTap");
            tapping = false;
        }
    }



}
