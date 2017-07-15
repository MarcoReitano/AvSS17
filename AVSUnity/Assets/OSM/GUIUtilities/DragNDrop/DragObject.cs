using System.Collections.Generic;
using UnityEngine;

public class DragObject
{
    private Rect startRect;
    public Rect StartRect
    {
        get {
            //if (!isDragging)
                return dropArea.Rect;
            //return startRect; 
        }
        set { startRect = value; }
    }

    private Rect currentRect;
    public Rect CurrentRect
    {
        get {
            if (!Ease)
                return StartRect;
            return currentRect; }
        set { currentRect = value; }
    }

    private Rect easeStartRect;
    public Rect EaseStartRect
    {
        get { return easeStartRect; }
        set { easeStartRect = value; }
    }

    private Rect easeEndRect;
    public Rect EaseEndRect
    {
        get { return easeEndRect; }
        set { easeEndRect = value; }
    }

    private DropArea dropArea;
    public DropArea DropArea
    {
        get { return dropArea; }
        set { dropArea = value; }
    }

    private bool isDragging;
    public bool IsDragging
    {
        get { return isDragging; }
        set { isDragging = value; }
    }

    private bool ease = false;
    public bool Ease
    {
        get { return ease; }
        set { ease = value; }
    }

    private EasingCurves.EaseType easeInType = EasingCurves.EaseType.easeOutElastic;
    public EasingCurves.EaseType EaseInType
    {
        get { return easeInType; }
        set { easeInType = value; }
    }

    private int targetEasingParameter = 1;
    public int TargetEaseingParameter
    {
        get { return targetEasingParameter; }
        set { targetEasingParameter = value; }
    }

    public float t = 0;

    public Dictionary<string, object> properties = new Dictionary<string, object>();
    
    private float timeFactor = 2;
    public float TimeFactor
    {
        get { return timeFactor; }
        set { timeFactor = value; }
    }
}