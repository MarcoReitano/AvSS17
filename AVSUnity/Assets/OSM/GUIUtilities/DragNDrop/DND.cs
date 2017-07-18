using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class DND
{

    // DropArea by Name
    public Dictionary<string, DropArea> dropAreas = new Dictionary<String, DropArea>();
    // DropAreas by Type
    private Dictionary<Type, List<DropArea>> dropAreasByType = new Dictionary<Type, List<DropArea>>();
    // DropArea by Object
    private Dictionary<object, DropArea> objectsDropArea = new Dictionary<object, DropArea>();


    public bool SetObject(string dropAreaID, object obj)
    {
        DropArea dropArea;

        if (!dropAreas.TryGetValue(dropAreaID, out dropArea))
        {
            Debug.Log(dropAreaID + " DropArea not found");
            return false;
        }

        dropArea.Object = obj;
        DragObject dragObject;
        if (dragObjectByObject.TryGetValue(obj, out dragObject))
        {
            dragObject.DropArea = dropArea;
            dragObject.CurrentRect = dropArea.Rect;
        }
        objectsDropArea[obj] = dropArea;
        return true;
    }

    public T GetObject<T>(string dropAreaID) where T : class
    {
        DropArea dropArea;

        if (!dropAreas.TryGetValue(dropAreaID, out dropArea))
        {
            Debug.Log(dropAreaID + " DropArea not found");
            return default(T);
        }

        return dropArea.GetObject<T>();
    }


    /// <summary>
    /// Create a new DropArea with a given name if name does no
    /// </summary>
    /// <param name="id"></param>
    /// <param name="obj"></param>
    /// <param name="rect"></param>
    /// <param name="types"></param>
    public void CreateDropArea(string id, object obj, Rect rect, params Type[] types)
    {
        DropArea dropArea;
        if (!dropAreas.TryGetValue(id, out dropArea))
        {
            // create new DropArea
            dropArea = new DropArea(id, obj, rect, types);

            // store the DropArea with its ID
            dropAreas.Add(id, dropArea);

            // object to DropArea mapping
            if (dropArea.Object != null)
                objectsDropArea.Add(obj, dropArea);

            // Type to DropAreas mapping
            foreach (Type type in dropArea.Types)
            {
                List<DropArea> dropAreasOfType;
                if (!dropAreasByType.TryGetValue(type, out dropAreasOfType))
                    dropAreasByType.Add(type, new List<DropArea>());
                dropAreasByType[type].Add(dropArea);
            }
        }
    }

    /// <summary>
    /// Create a new DropArea with a given name if name does no
    /// </summary>
    /// <param name="id"></param>
    /// <param name="obj"></param>
    /// <param name="rect"></param>
    /// <param name="types"></param>
    public void TryCreateDropArea(string id, object obj, Rect rect, params Type[] types)
    {
        DropArea dropArea;
        if (!dropAreas.TryGetValue(id, out dropArea))
        {
            // create new DropArea
            dropArea = new DropArea(id, obj, rect, types);

            // store the DropArea with its ID
            dropAreas.Add(id, dropArea);

            // object to DropArea mapping
            if (dropArea.Object != null)
                objectsDropArea.Add(obj, dropArea);

            // Type to DropAreas mapping
            foreach (Type type in dropArea.Types)
            {
                List<DropArea> dropAreasOfType;
                if (!dropAreasByType.TryGetValue(type, out dropAreasOfType))
                    dropAreasByType.Add(type, new List<DropArea>());
                dropAreasByType[type].Add(dropArea);
            }
        }
        else
        {
            dropArea.Rect = rect;
            dropArea.Types = types.ToList<Type>();
        }
    }

    
    public void DeleteDropArea(string name)
    {
        DropArea dropArea;
        if (dropAreas.TryGetValue(name, out dropArea))
        {
            dropAreas.Remove(name);
            foreach (Type type in dropArea.Types)
            {
                dropAreasByType[type].Remove(dropArea);
            }

            if (dropArea.Object != null)
                objectsDropArea.Remove(dropArea.Object);
        }
    }

    private Stack<DropArea> dropAreasInProgress = new Stack<DropArea>();

    private DropArea thisDropArea;
    public DropArea ThisDropArea
    {
        get { return dropAreasInProgress.Peek(); }
    }

    public Rect lastRect;


    public bool DropAreaBegin(string id)
    {
        // Check if properties for the dragObject have been created
        if (!dropAreas.TryGetValue(id, out thisDropArea))
        {
            Debug.LogError("DropArea " + id + " NotFound");
            return false;
        }

        dropAreasInProgress.Push(thisDropArea);

        return true;
    }


    public void DropAreaEnd(string id)
    {

        if (!dropAreas.TryGetValue(id, out thisDropArea))
        {
            Debug.LogError("DropArea " + id + " NotFound");
            return;
        }
        lastRect = ThisDropArea.Rect;
        dropAreasInProgress.Pop();
    }


    #region Gets/Checks
    public void CheckValidDropAreas(object obj)
    {
        Type objType = obj.GetType();
        Debug.Log("ObjectType: " + objType);

        foreach (Type type in dropAreasByType.Keys)
        {
            if (objType == type || objType.IsSubclassOf(type))
            {
                Debug.Log(objType + " is (subclass of) " + type);
            }
        }
    }

    private List<DropArea> GetValidDropAreas(Type t)
    {
        List<DropArea> dropAreasForT = new List<DropArea>();

        foreach (Type type in dropAreasByType.Keys)
        {
            if (t == type || t.IsSubclassOf(type))
            {
                Debug.Log(t + " is (subclass of) " + type);
                foreach (var item in dropAreasByType[type])
                {
                    dropAreasForT.Add(item);
                }
            }
        }
        return dropAreasForT;
    }
    #endregion // Gets/Checks


    #region Drag'n'Drop Rect

    private Stack<DragObject> dragObjectsInProgress = new Stack<DragObject>();

    public Dictionary<object, DragObject> dragObjectByObject = new Dictionary<object, DragObject>();

    public DragObject CurrentDragObject
    {
        get { return dragObjectsInProgress.Peek(); }
    }


    public DragObject TryGetDragObject(object obj)
    {
        DragObject dragObject;
        // Check if properties for the dragObject have been created
        if (!dragObjectByObject.TryGetValue(obj, out dragObject))
        {
            // create new properties
            dragObject = new DragObject();

            dragObject.DropArea = objectsDropArea[obj];

            // initialize dragObjects current Rect with its dropareas
            dragObject.StartRect = dragObject.DropArea.Rect;
            dragObject.CurrentRect = dragObject.DropArea.Rect;
            dragObject.EaseStartRect = dragObject.DropArea.Rect;
            dragObject.EaseEndRect = dragObject.DropArea.Rect;

            dragObjectByObject.Add(obj, dragObject);
        }
        return dragObject;
    }

    public bool DragObjectBegin(object obj)
    {
        DropArea drop;
        if (objectsDropArea.TryGetValue(obj, out drop))
        {
            DragObject currentDragObject = TryGetDragObject(obj);

            //dragObjectByObject[obj] = currentDragObject;
            dragObjectsInProgress.Push(currentDragObject);
            return true;
        }
        Debug.Log("No droparea found for object...");
        return false;
    }

    bool somethingIsDraggingAlready = false;

    public bool DragObjectEnd(object obj)
    {
        bool newDropPosition = false;
        DropArea drop;
        if (objectsDropArea.TryGetValue(obj, out drop))
        {


            Event currentEvent = Event.current;
            if (Event.current != null)
            {
                Vector3 mouse = InputHelpers.MouseOnGUI();

                if (InputHelpers.MouseDown(0) && CurrentDragObject.CurrentRect.Contains(mouse) && !somethingIsDraggingAlready)
                {
                    somethingIsDraggingAlready = true;
                    CurrentDragObject.IsDragging = true;
                    CurrentDragObject.Ease = true;

                    CurrentDragObject.t = 0;
                    CurrentDragObject.EaseStartRect = CurrentDragObject.StartRect;
                    CurrentDragObject.EaseEndRect = new Rect(
                        mouse.x - CurrentDragObject.CurrentRect.width / 2f,
                        mouse.y - CurrentDragObject.CurrentRect.height / 2f,
                        CurrentDragObject.CurrentRect.width,
                        CurrentDragObject.CurrentRect.height);

                }
                else if (InputHelpers.MouseMoved(0) && CurrentDragObject.IsDragging)
                {
                    CurrentDragObject.IsDragging = true;

                    CurrentDragObject.t = 0;
                    CurrentDragObject.EaseStartRect = CurrentDragObject.CurrentRect;
                    CurrentDragObject.EaseEndRect = new Rect(
                        mouse.x - CurrentDragObject.CurrentRect.width / 2f,
                        mouse.y - CurrentDragObject.CurrentRect.height / 2f,
                        CurrentDragObject.CurrentRect.width,
                        CurrentDragObject.CurrentRect.height);
                }
                else if (InputHelpers.MouseUp(0) && CurrentDragObject.IsDragging)
                {
                    CurrentDragObject.t = 0;
                    CurrentDragObject.IsDragging = false;
                    somethingIsDraggingAlready = false;
                    CurrentDragObject.Ease = true;

                    DropArea selectedDropArea = null;
                    foreach (DropArea dropArea in GetValidDropAreas(obj.GetType()))
                    {
                        if (dropArea.ContainsMouse() && dropArea.Object == null)
                        {
                            selectedDropArea = dropArea;
                            newDropPosition = true;
                            break;
                        }
                    }

                    if (selectedDropArea != null && selectedDropArea.Object == null)
                    {
                        CurrentDragObject.EaseStartRect = CurrentDragObject.CurrentRect;
                        CurrentDragObject.EaseEndRect = selectedDropArea.Rect;

                        selectedDropArea.Object = obj;
                        CurrentDragObject.DropArea.Object = null;
                        objectsDropArea[obj] = selectedDropArea;
                        CurrentDragObject.DropArea = selectedDropArea;

                        CurrentDragObject.StartRect = CurrentDragObject.DropArea.Rect;
                    }
                    else
                    {
                        CurrentDragObject.EaseStartRect = CurrentDragObject.CurrentRect;
                        CurrentDragObject.EaseEndRect = CurrentDragObject.StartRect;
                    }

                }
            }

            easeDragObject();
            dragObjectsInProgress.Pop();
        }
        return newDropPosition;
    }


    #region Easing the DragDropRect
    private void easeDragObject()
    {
        if (CurrentDragObject.Ease)
        {
            // calculate new parameter-Value
            //#if UNITY_EDITOR
            //            if (CurrentDragObject.t < 1)
            //            {
            //                CurrentDragObject.t += Time.deltaTime / CurrentDragObject.TimeFactor;
            //            }
            //#else
            if (CurrentDragObject.t < 1)
            {
                //CurrentDragObject.t += GUIDeltaTime.deltaTimeGUI / CurrentDragObject.TimeFactor;
                CurrentDragObject.t += Time.deltaTime / CurrentDragObject.TimeFactor;
            }
            //#endif
            CurrentDragObject.t = Mathf.Clamp01(CurrentDragObject.t);

            // calculate the Rectangle for parameter 
            CurrentDragObject.CurrentRect = EasingCurves.EaseRect(
                CurrentDragObject.EaseStartRect,
                CurrentDragObject.EaseEndRect,
                CurrentDragObject.EaseInType,
                CurrentDragObject.t);

            // dragObject reached its targetPosition?
            if (CurrentDragObject.t == 1)
            {
                CurrentDragObject.Ease = false;
                CurrentDragObject.t = 0;
            }
        }
    }
    #endregion // Easing the DragObject
    #endregion //Drag'n'Drop Rect




    #region Helper
    public void LogDNDState()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("CurrentState of DND:\n");

        int count = 0;
        foreach (Type type in dropAreasByType.Keys)
        {
            sb.Append(count++).Append(". Type: ").Append(type).Append("\n");
            int itemCount = 0;
            foreach (var item in dropAreasByType[type])
                sb.Append("\t" + itemCount++ + ". ").Append(item.GetType() + ": " + item.ToString()).Append("\n");
        }
        Debug.Log(sb.ToString());
    }

    private static void DisplayTypeInfo(Type t)
    {
        Debug.Log("\r\n" + t);
        Debug.Log("\tIs this a generic type definition? " + t.IsGenericTypeDefinition);
        Debug.Log("\tIs it a generic type? " + t.IsGenericType);
        Type[] typeArguments = t.GetGenericArguments();

        Debug.Log("\tList type arguments (" + typeArguments.Length + "):");
        foreach (Type tParam in typeArguments)
        {
            Debug.Log("\t\t" + tParam);
        }
    }
    #endregion // Helper




}
