using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;

public static class CustomHandles
{
    private static float fingerFactor = 0.25f;

    private static float handlePixelSize = 10;
    public static float HandlePixelSize
    {
        get { return handlePixelSize; }
    }

    static CustomHandles()
    {
        if (Screen.dpi != 0f)
        {
            handlePixelSize = Screen.dpi * fingerFactor;
            Debug.Log("Screen.dpi = " + Screen.dpi + " " + handlePixelSize);
        }
        else
        {
            handlePixelSize = Screen.height / 40;
            Debug.Log("Screen.dpi = " + Screen.dpi + " " + handlePixelSize);
        }
    }


    public static bool InsideHandleOnGUI(Vector3 handlePosition, float pixelDistance, out Vector3 newPosition)
    {
        Vector2 screenPoint = Camera.main.WorldToScreenPoint(handlePosition);
        Vector2 guiPosition = Input.mousePosition;

        newPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        return guiPosition.InDistance(screenPoint, pixelDistance);
    }

    public static bool InsideHandleOnGUI(Vector3 handlePosition, float pixelDistance)
    {
        Vector2 screenPoint = Camera.main.WorldToScreenPoint(handlePosition);
        Vector2 guiPosition = Input.mousePosition;

        return guiPosition.InDistance(screenPoint, pixelDistance);
    }


    public static bool InsideHandleOnGUI(Vector3 handlePosition)
    {
        return InsideHandleOnGUI(handlePosition, HandlePixelSize);
    }

    public static void DrawSphereHandle(Vector3 position, float size, Color inactiveColor, Color activeColor)
    {
        if (CustomHandles.InsideHandleOnGUI(position))
            DebugDraw.DrawSphere(position, size, activeColor);
        else
            DebugDraw.DrawSphere(position, size, inactiveColor);
    }


    public static void DrawMouseDebug()
    {
        Vector3 mouse = Input.mousePosition.SwapScreenToWorldPoint();
        CustomGUIUtils.DrawBox(
            new Rect(
                mouse.x - CustomHandles.HandlePixelSize,
                mouse.y - CustomHandles.HandlePixelSize,
                CustomHandles.HandlePixelSize * 2,
                CustomHandles.HandlePixelSize * 2), new Color(0f, 1f, 0f, 0.3f));
    }
}
