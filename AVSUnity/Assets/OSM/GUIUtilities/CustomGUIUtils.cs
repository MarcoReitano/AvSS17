using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum FadeType { In, Out }

public static class CustomGUIUtils
{

    public static Texture2D BlackTransparentGradient;

    static CustomGUIUtils()
    {
        BlackTransparentGradient = GetGradientTexture(255, 1, new Color(0, 0, 0, 1f), Color.black);
    }


    /// <summary>
    /// start a new element-Group
    /// </summary>
    public static void BeginGroup(GUIStyle color)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Box("", color, GUILayout.Width(12f));
        GUILayout.BeginVertical();

    }

    public static void BeginGroup(float width)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Box("", GUIStyle.none, GUILayout.Width(width));
        GUILayout.BeginVertical();
        GUILayout.Box("", GUIStyle.none, GUILayout.Height(width));

    }

    /// <summary>
    /// start a new element-Group
    /// </summary>
    public static void BeginGroup()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Box("", GUIStyle.none, GUILayout.Width(12f));
        GUILayout.BeginVertical();

    }

    /// <summary>
    /// end last element-group
    /// </summary>
    public static void EndGroup()
    {
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
    }


    /// <summary>
    /// start a new element-Group
    /// </summary>
    public static void BeginArea()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Box("", GUIStyle.none, GUILayout.Width(12f));
        GUILayout.BeginVertical();

    }

    /// <summary>
    /// end last element-group
    /// </summary>
    public static void EndArea()
    {
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
    }




    private static Dictionary<Color, Texture2D> simpleColorTextures = new Dictionary<Color, Texture2D>();
    public static Dictionary<Color, Texture2D> SimpleColorTextures
    {
        get { return simpleColorTextures; }
    }

    private static Dictionary<Color, GUIStyle> textColorStyles = new Dictionary<Color, GUIStyle>();
    public static Dictionary<Color, GUIStyle> TextColorStyles
    {
        get { return textColorStyles; }
    }

    private static Dictionary<Color, GUIStyle> backgroundColorStyles = new Dictionary<Color, GUIStyle>();
    public static Dictionary<Color, GUIStyle> BackgroundColorStyles
    {
        get { return backgroundColorStyles; }
    }

    public static GUIStyle GetColorBackgroundStyle(Color color, GUIStyle style)
    {
        GUIStyle colorStyle;
        if (!backgroundColorStyles.TryGetValue(color, out colorStyle))
        {
            colorStyle = new GUIStyle(style);
            colorStyle.normal.background = GetSimpleColorTexture(color);
            backgroundColorStyles.Add(color, colorStyle);
        }
        return colorStyle;
    }

    public static GUIStyle GetColorBackgroundStyle(Color color)
    {
        GUIStyle colorStyle;
        if(!backgroundColorStyles.TryGetValue(color, out colorStyle)){
            colorStyle = new GUIStyle();
            colorStyle.normal.background = GetSimpleColorTexture(color);
            backgroundColorStyles.Add(color, colorStyle);
        }
        return colorStyle;
    }


    public static GUIStyle GetColorTextStyle(Color color)
    {

        GUIStyle colorStyle;
        if (!TextColorStyles.TryGetValue(color, out colorStyle))
        {
            colorStyle = new GUIStyle();
            colorStyle.normal.textColor = color;
            TextColorStyles.Add(color, colorStyle);
        }
        return colorStyle;
    }

    public static GUIStyle GetColorTextStyle(Color color, int size)
    {

        GUIStyle colorStyle;
        if (!TextColorStyles.TryGetValue(color, out colorStyle))
        {
            colorStyle = new GUIStyle();
            colorStyle.normal.textColor = color;
            TextColorStyles.Add(color, colorStyle);
        }
        colorStyle.fontSize = size;
        return colorStyle;
    }

    public static Texture2D GetSimpleColorTexture(Color color)
    {
        Texture2D texture;
        if (!SimpleColorTextures.TryGetValue(color, out texture))
        {
            texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, color);
            texture.Apply();
            texture.filterMode = FilterMode.Point;
            SimpleColorTextures.Add(color, texture);
        }
        return texture;
    }



    //public static void Render_Colored_Rectangle(Rect rect, Color rgb_color)
    //{
    //    //Texture2D rgb_texture = new Texture2D((int)rect.width, (int) rect.height);
    //    Texture2D rgb_texture = GetSimpleColorTexture(rgb_color);
    //    //int i, j;
    //    //for (i = 0; i < rect.width; i++)
    //    //{
    //    //    for (j = 0; j < rect.height; j++)
    //    //    {
    //    //        rgb_texture.SetPixel(i, j, rgb_color);
    //    //    }
    //    //}
    //    //rgb_texture.Apply();
    //    GUIStyle generic_style = new GUIStyle();
    //    GUI.skin.box = generic_style;
    //    GUI.Box(rect, rgb_texture);
    //}


    public static void Render_Colored_Rectangle(int x, int y, int w, int h, float r, float g, float b)
    {
        Texture2D rgb_texture = new Texture2D(w, h);
        Color rgb_color = new Color(r, g, b);
        int i, j;
        for (i = 0; i < w; i++)
        {
            for (j = 0; j < h; j++)
            {
                rgb_texture.SetPixel(i, j, rgb_color);
            }
        }
        rgb_texture.Apply();
        GUIStyle generic_style = new GUIStyle();
        GUI.skin.box = generic_style;
        GUI.Box(new Rect(x, y, w, h), rgb_texture);
    }






    //// Fade in the GUITexture, wait a couple of seconds, then fade it out
    //public void Start () {
    //   yield FadeGUITexture(guiObject, fadeTime, FadeType.In);
    //   yield WaitForSeconds(2.0);
    //   yield FadeGUITexture(guiObject, fadeTime, FadeType.Out);
    //   Application.LoadLevel (Application.loadedLevel+1);
    //   }


    public static IEnumerator FadeGUITexture(this Texture2D texture, float seconds, FadeType fadeType)
    {
        float start = (float)(fadeType == FadeType.In ? 0.0 : 1.0);
        float end = (float)(fadeType == FadeType.In ? 1.0 : 0.0);
        float i = 0.0f;
        float step = 1.0f / seconds;

        while (i < 1.0)
        {
            i += step * Time.deltaTime;
            Color[] newColors = new Color[texture.GetPixels().Length];

            int x = 0;
            foreach (Color color in texture.GetPixels())
            {
                newColors[x] = new Color(color.r, color.g, color.b, Mathf.Lerp(start, end, i) * .5f);
            }
            texture.SetPixels(newColors);
            texture.Apply();
            yield return null;
        }
    }


    /// <summary>
    /// Draws a framed Rectangle
    /// </summary>
    /// <param name="rect"></param>
    /// <param name="backgroundColor"></param>
    /// <param name="frameThickness"></param>
    /// <param name="frameColor"></param>
    public static void DrawFrameBox(Rect rect, Color backgroundColor, float frameThickness, Color frameColor)
    {
        Rect frameRect = new Rect(
            rect.xMin - frameThickness,
            rect.yMin - frameThickness,
            rect.width + 2 * frameThickness,
            rect.height + 2 * frameThickness);

        DrawBox(frameRect, frameColor);
        DrawBox(rect, backgroundColor);
    }

    /// <summary>
    /// Draws a framed Rectangle
    /// </summary>
    /// <param name="rect"></param>
    /// <param name="backgroundColor"></param>
    /// <param name="frameThickness"></param>
    /// <param name="frameColor"></param>
    public static void DrawOuterFrame(Rect rect, float frameThickness, Color frameColor)
    {
        Rect frameRect = new Rect(
            rect.xMin - frameThickness,
            rect.yMin - frameThickness,
            rect.width + 2 * frameThickness,
            rect.height + 2 * frameThickness);

        DrawBox(new Rect(frameRect.xMin, frameRect.yMin, frameThickness, frameRect.height), frameColor);
        DrawBox(new Rect(frameRect.xMin, frameRect.yMin, frameRect.width, frameThickness), frameColor);
        DrawBox(new Rect(frameRect.xMin, frameRect.yMax - frameThickness, frameRect.width, frameThickness), frameColor);

        DrawBox(new Rect(rect.xMax, frameRect.yMin, frameThickness, frameRect.height), frameColor);
        //DrawBox(new Rect(rect.xMax - frameThickness, rect.yMin, frameThickness, frameRect.height), frameColor);
    }

    /// <summary>
    /// Draws a colored Box
    /// </summary>
    /// <param name="rect"></param>
    /// <param name="backgroundColor"></param>
    public static void DrawBox(Rect rect, Color backgroundColor)
    {
        GUI.Box(rect, "", CustomGUIUtils.GetColorBackgroundStyle(backgroundColor));
    }


    public static void Label(Vector3 point, string text)
    {
        Vector3 position = Camera.main.WorldToScreenPoint(point);

        GUIStyle style = CustomGUIUtils.GetColorTextStyle(Color.black, 14);
        style.alignment = TextAnchor.MiddleCenter;
        Vector2 sizeOfLabel = style.CalcSize(new GUIContent(text));

        Rect rect = new Rect(position.x - (sizeOfLabel.x / 2f), (Screen.height - position.y) - (sizeOfLabel.y / 2f), sizeOfLabel.x, sizeOfLabel.y);

        GUI.Label(rect, text, style);
    }


    public static void Label(Vector3 point, string text, float angle)
    {
        Vector3 position = Camera.main.WorldToScreenPoint(point);

        GUIStyle style = CustomGUIUtils.GetColorTextStyle(Color.black, 14);
        style.alignment = TextAnchor.MiddleCenter;
        Vector2 sizeOfLabel = style.CalcSize(new GUIContent(text));

        Rect rect = new Rect(position.x - (sizeOfLabel.x / 2f), (Screen.height - position.y) - (sizeOfLabel.y / 2f), sizeOfLabel.x, sizeOfLabel.y);

        Vector2 pivotPoint = new Vector2(position.x, Screen.height - position.y);

        GUIUtility.RotateAroundPivot(angle, pivotPoint);
        GUI.Label(rect, text, style);
        GUIUtility.RotateAroundPivot(-angle, pivotPoint);
    }

    public static void Label(string text, Rect rect, float angle)
    {
        //Vector3 position = Camera.main.WorldToScreenPoint(point);

        GUIStyle style = CustomGUIUtils.GetColorTextStyle(Color.black, 14);
        //style.alignment = TextAnchor.MiddleCenter;
        //Vector2 sizeOfLabel = style.CalcSize(new GUIContent(text));

        //Rect rect = new Rect(position.x - (sizeOfLabel.x / 2f), (Screen.height - position.y) - (sizeOfLabel.y / 2f), sizeOfLabel.x, sizeOfLabel.y);

        //Vector2 pivotPoint = new Vector2(position.x, Screen.height - position.y);
        //Rect rect = GUILayoutUtility.GetLastRect(); 
        Vector2 pivotPoint = new Vector2(rect.xMax, rect.yMax);
        GUIUtility.RotateAroundPivot(angle, pivotPoint);
        GUI.Label(rect, text, style);
        GUIUtility.RotateAroundPivot(-angle, pivotPoint);
    }




    #region Sidebar
    private class SidebarProperties
    {
        public string sidebarName = string.Empty;
        public SidebarType sidebarType;
        public Rect currentRect;
        public Rect boundingRect;
        public Rect openRect;
        public Rect closedRect;
        public Vector2 scrollPosition;
        public float inset;
        //public bool open = false;
        public float open;
        public float target;
        public float current;
        public bool ease;
        public float factor = 2.5f;
        public EasingCurves.EaseType easeInType;
        public EasingCurves.EaseType easeOutType;
        public bool isDragging = false;
        public Vector2 draggingDirection;
        public int touchIndex;
    }

    private static Dictionary<string, SidebarProperties> sidebars = new Dictionary<string, SidebarProperties>();
    private static Stack<SidebarProperties> sidebarPropertiesStack = new Stack<SidebarProperties>();
    //private static SidebarProperties sidebarProperties;
    private static Vector2 sidebarButtonSize = new Vector2(40f, 40f);


    public static Rect BeginSideBar(string sidebarName, SidebarType sidebarType, Rect openRect, bool startStateOpen)
    {
        return BeginSideBar(sidebarName, sidebarType, new Rect(), openRect, startStateOpen, 5f, EasingCurves.EaseType.none, 1f);
    }

    public static Rect BeginSideBar(string sidebarName, SidebarType sidebarType, Rect openRect, bool startStateOpen, float inset)
    {
        return BeginSideBar(sidebarName, sidebarType, new Rect(), openRect, startStateOpen, 5f, EasingCurves.EaseType.none, 1f);
    }

    public static Rect BeginSideBar(string sidebarName, SidebarType sidebarType, Rect openRect, bool startStateOpen, float inset, EasingCurves.EaseType easeInType)
    {
        return BeginSideBar(sidebarName, sidebarType, new Rect(), openRect, startStateOpen, 5f, easeInType, 1f);
    }

    public static Rect BeginSideBar(string sidebarName, SidebarType sidebarType, Rect openRect, bool startStateOpen, float inset, EasingCurves.EaseType easeInType, float factor)
    {
        return BeginSideBar(sidebarName, sidebarType, new Rect(), openRect, startStateOpen, inset, easeInType, easeInType, factor);
    }

    public static Rect BeginSideBar(string sidebarName, SidebarType sidebarType, Rect boundingRect, Rect openRect, bool startStateOpen)
    {
        return BeginSideBar(sidebarName, sidebarType, boundingRect, openRect, startStateOpen, 5f, EasingCurves.EaseType.none, 1f);
    }

    public static Rect BeginSideBar(string sidebarName, SidebarType sidebarType, Rect boundingRect, Rect openRect, bool startStateOpen, float inset)
    {
        return BeginSideBar(sidebarName, sidebarType, boundingRect, openRect, startStateOpen, 5f, EasingCurves.EaseType.none, 1f);
    }

    public static Rect BeginSideBar(string sidebarName, SidebarType sidebarType, Rect boundingRect, Rect openRect, bool startStateOpen, float inset, EasingCurves.EaseType easeInType)
    {
        return BeginSideBar(sidebarName, sidebarType, boundingRect, openRect, startStateOpen, 5f, easeInType, 1f);
    }

    public static Rect BeginSideBar(string sidebarName, SidebarType sidebarType, Rect boundingRect, Rect openRect, bool startStateOpen, float inset, EasingCurves.EaseType easeInType, float factor)
    {
        return BeginSideBar(sidebarName, sidebarType, boundingRect, openRect, startStateOpen, inset, easeInType, easeInType, factor);
    }

    public static Rect BeginSideBar(string sidebarName, SidebarType sidebarType, Rect openRect, bool startStateOpen, float inset, EasingCurves.EaseType easeInType, EasingCurves.EaseType easeOutType, float factor)
    {
        return BeginSideBar(sidebarName, sidebarType, new Rect(), openRect, startStateOpen, inset, easeInType, easeInType, factor);
    }

    public static Rect BeginSideBar(string sidebarName, SidebarType sidebarType, Rect boundingRect, Rect openRect, bool startStateOpen, float inset, EasingCurves.EaseType easeInType, EasingCurves.EaseType easeOutType, float factor)
    {
        SidebarProperties sidebarProperties = null;
        // If the Sidebar has not been created yet --> create the Sidebar
        if (!sidebars.ContainsKey(sidebarName))
        {
            // create properties instance
            sidebarProperties = new SidebarProperties();
            sidebarProperties.sidebarName = sidebarName;
            sidebarProperties.sidebarType = sidebarType;
            sidebarProperties.inset = inset;
            sidebarProperties.easeInType = easeInType;
            sidebarProperties.easeOutType = easeOutType;
            sidebarProperties.factor = factor;
            sidebarProperties.boundingRect = boundingRect;


            if (sidebarProperties.boundingRect.Equals(new Rect()))
                sidebarProperties.boundingRect = new Rect(0f, 0f, Screen.width, Screen.height);

            // initialize the Rectangles
            switch (sidebarType)
            {
                case SidebarType.LEFT:
                    sidebarProperties.openRect = new Rect(sidebarProperties.boundingRect.xMin, sidebarProperties.boundingRect.yMin + openRect.yMin, openRect.width, openRect.height);
                    sidebarProperties.closedRect = new Rect(sidebarProperties.boundingRect.xMin, sidebarProperties.boundingRect.yMin + openRect.yMin, 0f, openRect.height);
                    break;
                case SidebarType.RIGHT:
                    sidebarProperties.openRect = new Rect(sidebarProperties.boundingRect.xMax - openRect.width, sidebarProperties.boundingRect.yMin + openRect.yMin, openRect.width, openRect.height);
                    sidebarProperties.closedRect = new Rect(sidebarProperties.boundingRect.xMax, sidebarProperties.boundingRect.yMin + openRect.yMin, 0f, openRect.height);
                    break;
                case SidebarType.TOP:
                    sidebarProperties.openRect = new Rect(sidebarProperties.boundingRect.xMin + openRect.xMin, sidebarProperties.boundingRect.yMin, openRect.width, openRect.height);
                    sidebarProperties.closedRect = new Rect(sidebarProperties.boundingRect.xMin + openRect.xMin, sidebarProperties.boundingRect.yMin, openRect.width, 0f);
                    break;
                case SidebarType.BOTTOM:
                    sidebarProperties.openRect = new Rect(sidebarProperties.boundingRect.xMin + openRect.xMin, sidebarProperties.boundingRect.yMax - openRect.height, openRect.width, openRect.height);
                    sidebarProperties.closedRect = new Rect(sidebarProperties.boundingRect.xMin + openRect.xMin, sidebarProperties.boundingRect.yMax, openRect.width, 0f);
                    break;
                default:
                    break;
            }

            // Initialize withe the desired state after creation
            if (startStateOpen)
            {
                sidebarProperties.currentRect = sidebarProperties.openRect;
                sidebarProperties.open = 1f;
                sidebarProperties.current = 1f;
                sidebarProperties.target = 0f;
            }
            else
            {
                sidebarProperties.currentRect = sidebarProperties.closedRect;
                sidebarProperties.open = 0f;
                sidebarProperties.current = 0f;
                sidebarProperties.target = 1f;
            }

            // Add SidebarProperties to the Dictionary
            sidebars.Add(sidebarName, sidebarProperties);

        }
        else
        {
            // A sidebar of that name already existed --> get its Properties
            sidebars.TryGetValue(sidebarName, out sidebarProperties);


            //// initialize the Rectangles
            switch (sidebarProperties.sidebarType)
            {
                case SidebarType.LEFT:
                    sidebarProperties.openRect = new Rect(sidebarProperties.boundingRect.xMin, sidebarProperties.boundingRect.yMin + openRect.yMin, openRect.width, openRect.height);
                    sidebarProperties.closedRect = new Rect(sidebarProperties.boundingRect.xMin, sidebarProperties.boundingRect.yMin + openRect.yMin, 0f, openRect.height);
                    break;
                case SidebarType.RIGHT:
                    sidebarProperties.openRect = new Rect(sidebarProperties.boundingRect.xMax - openRect.width, sidebarProperties.boundingRect.yMin + openRect.yMin, openRect.width, openRect.height);
                    sidebarProperties.closedRect = new Rect(sidebarProperties.boundingRect.xMax, sidebarProperties.boundingRect.yMin + openRect.yMin, 0f, openRect.height);
                    break;
                case SidebarType.TOP:
                    sidebarProperties.openRect = new Rect(sidebarProperties.boundingRect.xMin + openRect.xMin, sidebarProperties.boundingRect.yMin, openRect.width, openRect.height);
                    sidebarProperties.closedRect = new Rect(sidebarProperties.boundingRect.xMin + openRect.xMin, sidebarProperties.boundingRect.yMin, openRect.width, 0f);
                    break;
                case SidebarType.BOTTOM:
                    sidebarProperties.openRect = new Rect(sidebarProperties.boundingRect.xMin + openRect.xMin, sidebarProperties.boundingRect.yMax - openRect.height, openRect.width, openRect.height);
                    sidebarProperties.closedRect = new Rect(sidebarProperties.boundingRect.xMin + openRect.xMin, sidebarProperties.boundingRect.yMax, openRect.width, 0f);
                    break;
                default:
                    break;
            }
        }


        GUIAreas.Add(sidebarProperties.sidebarName, sidebarProperties.currentRect);
        // Draw the current Rectangle
        CustomGUIUtils.DrawFrameBox(sidebarProperties.currentRect, Color.gray, 3f, Color.black);

        // Start GUIArea with Insets and ScrollView
        GUI.BeginGroup(currentRect);
        GUILayout.BeginArea(sidebarProperties.currentRect);
        GUILayout.BeginVertical();
        GUILayout.Space(sidebarProperties.inset);
        GUILayout.BeginHorizontal();
        GUILayout.Space(sidebarProperties.inset);
        sidebarProperties.scrollPosition = GUILayout.BeginScrollView(sidebarProperties.scrollPosition);

        sidebarPropertiesStack.Push(sidebarProperties);
        return sidebarProperties.currentRect;
    }




    public static void EndSideBar()
    {
        SidebarProperties sidebarProperties = sidebarPropertiesStack.Pop();
        // End the ScrollView, Insets and GUIArea
        GUILayout.EndScrollView();
        GUILayout.Space(sidebarProperties.inset);
        GUILayout.EndHorizontal();
        GUILayout.Space(sidebarProperties.inset);
        GUILayout.EndVertical();
        GUILayout.EndArea();
        GUI.EndGroup();

        // Prepare the Slider-Button 
        Rect buttonRect = new Rect();
        Texture2D icon = null;
        switch (sidebarProperties.sidebarType)
        {
            case SidebarType.LEFT:
                buttonRect = new Rect(
                    sidebarProperties.currentRect.xMax,
                    sidebarProperties.currentRect.yMin + sidebarProperties.currentRect.height / 2 - sidebarButtonSize.y / 2f,
                    sidebarButtonSize.x,
                    sidebarButtonSize.y);
                icon = DefaultIconSet.ListLeftRight;
                break;
            case SidebarType.RIGHT:
                buttonRect = new Rect(
                    sidebarProperties.currentRect.xMin - sidebarButtonSize.x,
                    sidebarProperties.currentRect.yMin + sidebarProperties.currentRect.height / 2 - sidebarButtonSize.y / 2f,
                    sidebarButtonSize.x,
                    sidebarButtonSize.y);
                icon = DefaultIconSet.ListLeftRight;
                break;
            case SidebarType.TOP:
                buttonRect = new Rect(
                    sidebarProperties.currentRect.xMin + sidebarProperties.currentRect.width / 2 - sidebarButtonSize.x / 2f,
                    sidebarProperties.currentRect.yMax,
                    sidebarButtonSize.x,
                    sidebarButtonSize.y);
                icon = DefaultIconSet.ListUpDown;
                break;
            case SidebarType.BOTTOM:
                buttonRect = new Rect(
                    sidebarProperties.currentRect.xMin + sidebarProperties.currentRect.width / 2 - sidebarButtonSize.x / 2f,
                    sidebarProperties.currentRect.yMin - sidebarButtonSize.y,
                    sidebarButtonSize.x,
                    sidebarButtonSize.y);
                icon = DefaultIconSet.ListUpDown;
                break;
            default:
                break;
        }

        GUIAreas.Add(sidebarProperties.sidebarName + "_Button", buttonRect);
        // Draw the SliderButton
        GUI.Label(buttonRect, icon);


        // Handle Mouse and Touch-Input
        Event currentEvent = Event.current;

        #region Multitouch preparation
        // Check if the current touchindex is still correct and available
        if (Input.touchCount < sidebarProperties.touchIndex)
            sidebarProperties.touchIndex = int.MaxValue;

        for (int i = 0; i < Input.touches.Length; i++)
        {
            if (buttonRect.Contains(Input.touches[i].position.SwapScreenToWorldPoint()))
                sidebarProperties.touchIndex = i;
        }
        #endregion // Multitouch preparation


        #region Dragging
        bool isTouchMoving = false;
        if (Input.touchCount > sidebarProperties.touchIndex)
        {
            isTouchMoving =
                (buttonRect.Contains(Input.touches[sidebarProperties.touchIndex].position.SwapScreenToWorldPoint()) || sidebarProperties.isDragging)
                && Input.touches[sidebarProperties.touchIndex].phase == TouchPhase.Moved;
        }

        bool isMouseDown = currentEvent.isMouse
            && currentEvent.type == EventType.MouseDrag
            && currentEvent.button == 0
            && (buttonRect.Contains(currentEvent.mousePosition) || sidebarProperties.isDragging);


        if (isTouchMoving || isMouseDown || sidebarProperties.isDragging)
        {
            sidebarProperties.isDragging = true;

            float newPositionXmax;
            float newPositionXmin;
            float newPositionYmax;
            float newPositionYmin;


            Vector2 delta = Vector2.zero;
            Vector2 newPosition = Vector2.zero;
            if (isTouchMoving)
            {
                delta = Input.touches[sidebarProperties.touchIndex].deltaPosition;
                newPosition = Input.touches[sidebarProperties.touchIndex].position.SwapScreenToWorldPoint();
            }
            else
            {
                delta = currentEvent.delta;
                newPosition = currentEvent.mousePosition;
            }

            if (delta != Vector2.zero)
            {
                switch (sidebarProperties.sidebarType)
                {
                    case SidebarType.LEFT:
                        newPositionXmax = newPosition.x;
                        if (newPositionXmax >= sidebarProperties.openRect.xMin && newPositionXmax <= sidebarProperties.openRect.xMax)
                        {
                            sidebarProperties.currentRect.xMax = newPositionXmax;
                            sidebarProperties.current = MathUtils.ParameterLine(sidebarProperties.openRect.xMin, sidebarProperties.openRect.xMax, sidebarProperties.currentRect.xMax);

                        }
                        sidebarProperties.draggingDirection = delta.ResetY().normalized;
                        break;
                    case SidebarType.RIGHT:
                        newPositionXmin = newPosition.x;
                        if (newPositionXmin >= sidebarProperties.openRect.xMin && newPositionXmin <= sidebarProperties.openRect.xMax)
                        {
                            sidebarProperties.currentRect.xMin = newPositionXmin;
                            sidebarProperties.current = MathUtils.ParameterLine(sidebarProperties.openRect.xMax, sidebarProperties.openRect.xMin, sidebarProperties.currentRect.xMin);
                        }
                        sidebarProperties.draggingDirection = delta.ResetY().normalized;
                        break;
                    case SidebarType.TOP:
                        newPositionYmax = newPosition.y;
                        if (newPositionYmax >= sidebarProperties.openRect.yMin && newPositionYmax <= sidebarProperties.openRect.yMax)
                        {
                            sidebarProperties.currentRect.yMax = newPositionYmax;
                            sidebarProperties.current = MathUtils.ParameterLine(sidebarProperties.openRect.yMin, sidebarProperties.openRect.yMax, sidebarProperties.currentRect.yMax);
                        }
                        sidebarProperties.draggingDirection = delta.ResetX().normalized;
                        break;
                    case SidebarType.BOTTOM:
                        newPositionYmin = newPosition.y;
                        if (newPositionYmin >= sidebarProperties.openRect.yMin && newPositionYmin <= sidebarProperties.openRect.yMax)
                        {
                            sidebarProperties.currentRect.yMin = newPositionYmin;
                            sidebarProperties.current = MathUtils.ParameterLine(sidebarProperties.openRect.yMax, sidebarProperties.openRect.yMin, sidebarProperties.currentRect.yMin);
                        }
                        sidebarProperties.draggingDirection = delta.ResetX().normalized;
                        break;
                    default:
                        break;
                }
            }
            Debug.Log(delta + " --> DraggingDirection: " + sidebarProperties.draggingDirection);
        }
        #endregion // Dragging



        #region Button clicked or DragMode-Ended
        // If Touch ended inside Button-Rectangle
        bool isTouchEnded = false;
        if (Input.touchCount > sidebarProperties.touchIndex)
        {
            isTouchEnded =
                (buttonRect.Contains(Input.touches[sidebarProperties.touchIndex].position.SwapScreenToWorldPoint()) || sidebarProperties.isDragging)
                && Input.touches[sidebarProperties.touchIndex].phase == TouchPhase.Ended;
        }

        // If Mouse clicked inside Button-Rectangle
        bool isMouseUp =
            currentEvent.isMouse
            && currentEvent.type == EventType.MouseUp
            && currentEvent.button == 0
            && (buttonRect.Contains(currentEvent.mousePosition) || sidebarProperties.isDragging);

        // If either mouse ot touch clicked the button
        //    OR DraggingMode has been ended
        if (isTouchEnded || isMouseUp)
        {
            Debug.Log("Ended Touch / Mouse Clicked" + sidebarProperties.current);
            // If we were in DraggingMode
            if (sidebarProperties.isDragging)
            {
                Debug.Log("DraggingDirection: " + sidebarProperties.draggingDirection);
                if (sidebarProperties.draggingDirection == Vector2.zero)
                {
                    // snap to the nearest side
                    //Debug.Log("Setting new Target for " + sidebarProperties.current);
                    if (sidebarProperties.current <= 0.5f)
                        sidebarProperties.target = 0;
                    if (sidebarProperties.current > 0.5f)
                        sidebarProperties.target = 1;
                    //Debug.Log("---> " + sidebarProperties.target);
                }
                else
                {
                    switch (sidebarProperties.sidebarType)
                    {
                        case SidebarType.LEFT:
                            if (sidebarProperties.draggingDirection == Vector2.right)
                                sidebarProperties.target = 1;
                            else
                                sidebarProperties.target = 0;
                            break;
                        case SidebarType.RIGHT:
                            if (sidebarProperties.draggingDirection == -Vector2.right)
                                sidebarProperties.target = 1;
                            else
                                sidebarProperties.target = 0;
                            break;
                        case SidebarType.TOP:
                            if (sidebarProperties.draggingDirection == -Vector2.up)
                                sidebarProperties.target = 1;
                            else
                                sidebarProperties.target = 0;
                            break;
                        case SidebarType.BOTTOM:
                            if (sidebarProperties.draggingDirection == Vector2.up)
                                sidebarProperties.target = 1;
                            else
                                sidebarProperties.target = 0;
                            break;
                        default:
                            break;
                    }
                }
            }
            else
            {
                // Clicked the button --> open or close the sidebar
                if (sidebarProperties.current <= 0.5f)
                    sidebarProperties.target = 1;
                if (sidebarProperties.current > 0.5f)
                    sidebarProperties.target = 0;
            }

            //Debug.Log("Setting ease TRUE");
            // set to ease --> animate the 
            sidebarProperties.ease = true;
            sidebarProperties.isDragging = false;
        }
        #endregion // Button clicked or DragMode-Ended

        #region Easing the Sidebar
        if (sidebarProperties.ease)
        {
            if (sidebarProperties.easeInType == EasingCurves.EaseType.none)
            {
                sidebarProperties.ease = false;

                if (sidebarProperties.target == 0)
                {
                    sidebarProperties.currentRect = sidebarProperties.closedRect;
                    sidebarProperties.target = 1;
                }
                else if (sidebarProperties.target == 1)
                {
                    sidebarProperties.currentRect = sidebarProperties.openRect;
                    sidebarProperties.target = 0;
                }

            }
            else
            {
                if (sidebarProperties.target == 0)
                {
                    if (sidebarProperties.current > 0)
                        sidebarProperties.current -= Time.deltaTime / sidebarProperties.factor;
                }
                else if (sidebarProperties.target == 1)
                {
                    if (sidebarProperties.current < 1)
                        sidebarProperties.current += Time.deltaTime / sidebarProperties.factor;
                }

                sidebarProperties.current = Mathf.Clamp01(sidebarProperties.current);

                if (sidebarProperties.ease)
                {

                    if (sidebarProperties.target == 1)
                    {
                        sidebarProperties.currentRect = EasingCurves.EaseRect(
                            sidebarProperties.closedRect,
                            sidebarProperties.openRect,
                            sidebarProperties.easeInType,
                            sidebarProperties.current);
                    }
                    else if (sidebarProperties.target == 0)
                    {
                        sidebarProperties.currentRect = EasingCurves.EaseRect(
                            sidebarProperties.closedRect,
                            sidebarProperties.openRect,
                            sidebarProperties.easeOutType,
                            sidebarProperties.current);
                    }
                }

                if (sidebarProperties.current >= 1)
                {
                    sidebarProperties.ease = false;
                    sidebarProperties.current = 1;
                    sidebarProperties.target = 0;
                }
                else if (sidebarProperties.current <= 0)
                {
                    sidebarProperties.ease = false;
                    sidebarProperties.current = 0;
                    sidebarProperties.target = 1;
                }
            }
        }
        #endregion // Easing the Sidebar
    }

    #endregion // Sidebar


    #region Dialog



    public static float target = 1f;
    public static float t = 0f;
    public static bool ease = true;
    public static Rect currentRect;
    public static bool displayed = false;
    public static Rect closedRect = new Rect(Screen.width / 2f, Screen.height / 2f, 0f, 0f);
    public static DialogResultType result = DialogResultType.UNDEFINED;

    public static DialogResultType Dialog(DialogType dialogType, string title, string message, Rect rect, float factor, ref bool showDialog)
    {
        result = DialogResultType.UNDEFINED;

        if (target == 0)
        {
            if (t > 0)
                t -= Time.deltaTime / factor;
        }
        else if (target == 1)
        {
            if (t < 1)
                t += Time.deltaTime / factor;
        }

        t = Mathf.Clamp01(t);

        if (MathUtils.AlmostEqual(t, 0f, 0.01f) && target == 0f)
        {
            //ease = false;
            showDialog = false;
            target = 1f;
            Debug.Log("Done?" + showDialog);
        }


        if (ease && showDialog)
        {
            if (target == 1)
            {
                currentRect = EasingCurves.EaseRect(
                    closedRect,
                    rect,
                    EasingCurves.EaseType.easeOutExpo,
                    t);
            }
            else if (target == 0)
            {
                currentRect = EasingCurves.EaseRect(
                    closedRect,
                    rect,
                    EasingCurves.EaseType.easeInExpo,
                    t);
            }
        }

        CustomGUIUtils.DrawFrameBox(currentRect, Color.gray, 3f, Color.black);

        //if(t == 1){
        GUILayout.BeginArea(currentRect);
        GUILayout.Label(title, GUILayout.Width(currentRect.width), GUILayout.Height(30f));
        GUILayout.Space(30f);
        GUILayout.BeginHorizontal();
        GUILayout.Space(30f);
        GUILayout.BeginVertical();
        GUILayout.Label(message);

        GUILayout.EndVertical();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        switch (dialogType)
        {
            case DialogType.YES_NO:

                if (GUILayout.Button("Yes"))
                {
                    CloseDialog();
                    result = DialogResultType.YES;
                }

                if (GUILayout.Button("No"))
                {
                    CloseDialog();
                    result = DialogResultType.NO;
                }
                break;
            case DialogType.YES_NO_CANCEL:
                if (GUILayout.Button("Yes"))
                {
                    CloseDialog();
                    result = DialogResultType.YES;
                }
                if (GUILayout.Button("No"))
                {
                    CloseDialog();
                    result = DialogResultType.NO;
                }
                if (GUILayout.Button("Cancel"))
                {
                    CloseDialog();
                    result = DialogResultType.CANCEL;
                }

                break;
            case DialogType.OK:
                if (GUILayout.Button("OK"))
                {
                    CloseDialog();
                    result = DialogResultType.OK;
                }
                break;
            case DialogType.OK_CANCEL:
                if (GUILayout.Button("Cancel"))
                {
                    CloseDialog();
                    result = DialogResultType.CANCEL;
                }
                break;
            default:
                result = DialogResultType.UNDEFINED;
                break;
        }

        GUILayout.EndHorizontal();

        GUILayout.EndArea();

        return result;
    }


    private static void CloseDialog()
    {
        target = 0;
        ease = true;
    }
    #endregion //Dialog



    public static Color GetRedYellowGreenBar(float t)
    {
        //”rgb(“&INT(MIN(510-[% Complete]*255*2,255))&”,”&INT(MIN([% Complete]*255*2,255))&”,0)”
        t = 1 - t;

        int r = (int)Mathf.Min(510f - (int)(t * 255f) * 2f, 255f);
        int g = (int)Mathf.Min(t * 255f * 2f, 255f);
        int b = 0;

        Color color = new Color(r / 255f, g / 255f, b / 255f);

        //Debug.Log(t + " --> " + color);
        return color;
    }

    // TODO: Gradient Berechnen
    // http://www.blitzforum.de/forum/viewtopic.php?t=29955


    // Irgendwie erstellen:
    //private static List<Color> list = new List<Color>();

    public static Texture2D GetGradientTexture(int width, int height, params Color[] list)
    {
        //list.Add(Color.red);
        //list.Add(Color.green);
        //list.Add(Color.blue);

        int[] rgbColors = new int[width];
        float stepSize = (float)width / (list.Length - 1);

        List<Color> pixels = new List<Color>();
        for (int i = 0; i < width; i++)
        {
            float a = (float)i / width;
            int ci0 = (int)(a * (list.Length - 1));
            int ci1 = ci0 + 1;

            Color c0 = list[ci0];
            int r0 = (int)c0.r * 255;
            int g0 = (int)c0.g * 255;
            int b0 = (int)c0.b * 255;

            Color c1 = list[ci1];
            int r1 = (int)c1.r * 255;
            int g1 = (int)c1.g * 255;
            int b1 = (int)c1.b * 255;

            int dr = r1 - r0;
            int dg = g1 - g0;
            int db = b1 - b0;

            float alpha = (i - (ci0 * stepSize)) / stepSize;

            int r = (int)(r0 + alpha * dr);
            int g = (int)(g0 + alpha * dg);
            int b = (int)(b0 + alpha * db);

            pixels.Add(new Color(r / 255f, g / 255f, b / 255f, alpha));


            int rgb =
                (r << 16) |
                (g << 8) |
                (b << 0);

            //Debug.Log("at "+i+" a "+a+" ci0 "+ci0+" alpha "+alpha+" rgb "+r+" "+g+" "+b);

            rgbColors[i] = rgb;

        }

        pixels.Reverse();
        Texture2D texture;
        texture = new Texture2D(width, height);
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                texture.SetPixel(i, j, pixels[i]);
            }
        }
        texture.Apply();
        texture.filterMode = FilterMode.Point;

        return texture;
    }

    //private Color[] buildColors(int size)
    //{
    //    int rgbColors[] = buildRGB(size); 
    //    Color colors[] = new Color[size];
    //    for (int i=0; i<size; i++)
    //    {
    //        colors[i] = new Color(rgbColors[i]);
    //    }
    //    return colors;
    //}




    public static void DrawSquareTextureAtWorldPoint(Vector3 position, float width, Texture texture)
    {
        Vector3 screenPoint = Camera.main.WorldToScreenPoint(position).SwapScreenToWorldPoint();
        Rect rect = new Rect(screenPoint.x - width / 2f, screenPoint.y - width / 2f, width, width);
        GUI.DrawTexture(rect, texture);
    }

    public static bool DrawSquareButtonAtWorldPoint(Vector3 position, float width, Texture texture)
    {
        DrawSquareTextureAtWorldPoint(position, width, texture);
        Vector3 screenPoint = Camera.main.WorldToScreenPoint(position).SwapScreenToWorldPoint();
        Rect rect = new Rect(screenPoint.x - width / 2f, screenPoint.y - width / 2f, width, width);
        
        return GUI.Button(rect, texture, GUIStyle.none);
    }

    public static void DrawSquareTextureAtWorldPoint(Vector3 position, float width, Texture texture, Texture textureSelected)
    {
        Vector3 screenPoint = Camera.main.WorldToScreenPoint(position).SwapScreenToWorldPoint();
        Rect rect = new Rect(screenPoint.x - width / 2f, screenPoint.y - width / 2f, width, width);

        if (CustomHandles.InsideHandleOnGUI(position))
            GUI.DrawTexture(rect, textureSelected);
        else
            GUI.DrawTexture(rect, texture);
    }


    public static void DrawSquareTextureAtWorldPoint(Vector3 position, float width, Texture texture, Texture textureSelected, float angle)
    {
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(position);


        Vector2 pivotPoint = new Vector2(screenPosition.x, Screen.height - screenPosition.y);

        GUIUtility.RotateAroundPivot(angle, pivotPoint);
        DrawSquareTextureAtWorldPoint(position, width, texture, textureSelected);
        GUIUtility.RotateAroundPivot(-angle, pivotPoint);
    }

    public static void DrawSquareTextureAtWorldPoint(Vector3 position, float width, Texture texture, float angle)
    {
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(position);
        Vector2 pivotPoint = new Vector2(screenPosition.x, Screen.height - screenPosition.y);
        GUIUtility.RotateAroundPivot(angle, pivotPoint);
        DrawSquareTextureAtWorldPoint(position, width, texture);
        GUIUtility.RotateAroundPivot(-angle, pivotPoint);
    }

    public static bool DrawSquareButtonAtWorldPoint(Vector3 position, float width, Texture texture, float angle)
    {
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(position);
        Vector2 pivotPoint = new Vector2(screenPosition.x, Screen.height - screenPosition.y);
        GUIUtility.RotateAroundPivot(angle, pivotPoint);
        bool buttonClick = DrawSquareButtonAtWorldPoint(position, width, texture);
        GUIUtility.RotateAroundPivot(-angle, pivotPoint);

        return buttonClick;
    }

}

public enum SidebarType
{
    LEFT,
    RIGHT,
    TOP,
    BOTTOM
}


public enum DialogType
{
    YES_NO,
    YES_NO_CANCEL,
    OK,
    OK_CANCEL
}

public enum DialogResultType
{
    YES,
    NO,
    CANCEL,
    OK,
    UNDEFINED
}




