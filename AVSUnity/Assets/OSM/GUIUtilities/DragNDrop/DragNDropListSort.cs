using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public class DragNDropListSort : MonoBehaviour
{

    public DND dragNDrop = new DND();
    List<String> strings = new List<String>();

    float boxWidth = 300f;
    float boxHeight = 70f;
    float gap = 5f;
    float accumulatedHeight;

    // Use this for initialization
    public void Start()
    {
        accumulatedHeight = gap;

        for (int i = 0; i < 8; i++)
        {
            strings.Add(i.ToString());

            Rect rect = new Rect(gap, accumulatedHeight, boxWidth, boxHeight);
            accumulatedHeight += gap + boxHeight;

            dragNDrop.CreateDropArea(i.ToString(), null, rect, typeof(String));
            dragNDrop.SetObject(i.ToString(), strings[i]);
        }
    }


    // Update is called once per frame
    void Update()
    {

    }

    Color myGrey = new Color(0.5f, 0.5f, 0.5f, 0.5f);

    String draggingString = null;
    int draggingStringOldIndex = -1;

    public void OnGUI()
    {
        GUI.Label(new Rect(boxWidth + 50, 0, 500, 500), "Dragging String = " + draggingString + "\n strings.Count " + strings.Count + "\n" + strings.ListToString<String>());

        accumulatedHeight = gap;

        // Draw DropAreas
        for (int i = 0; i < strings.Count; i++)
        {
            if (dragNDrop.DropAreaBegin(i.ToString()))
            {
                CustomGUIUtils.DrawBox(dragNDrop.ThisDropArea.Rect, XKCDColors.Orangeish);
                //GUI.Label(dragNDrop.ThisDropArea.Rect, "  " + i + ". " + dragNDrop.ThisDropArea.ToString());
                GUI.Label(dragNDrop.ThisDropArea.Rect, "   " + i);

                dragNDrop.ThisDropArea.Rect = new Rect(gap, accumulatedHeight, boxWidth, boxHeight);
                accumulatedHeight += gap + boxHeight;

                dragNDrop.SetObject(i.ToString(), strings[i]);
                dragNDrop.DropAreaEnd(i.ToString());
            }
        }

        
        accumulatedHeight = gap;
        List<String> concurrentStrings = new List<string>(strings);
        int currentItem = 0;
        foreach (String str in concurrentStrings)
        {
            DragObject currentstr = dragNDrop.TryGetDragObject(str);

            int i = concurrentStrings.IndexOf(str);

            if (currentstr.IsDragging)
            {
                draggingStringOldIndex = i;
                draggingString = strings[i];
                strings.RemoveAt(i);
                Debug.Log("Dragging String = " + draggingString + "\n" + strings.ListToString<String>());
            }
            if (dragNDrop.DragObjectBegin(concurrentStrings[i]))
            {
                CustomGUIUtils.DrawBox(dragNDrop.CurrentDragObject.CurrentRect, myGrey);
                GUI.Label(dragNDrop.CurrentDragObject.CurrentRect, concurrentStrings[i].ToString());

                Rect icon = new Rect(dragNDrop.CurrentDragObject.CurrentRect);
                icon.width = icon.height;
                GUI.DrawTexture(icon, DefaultIconSet.Device);
                dragNDrop.DragObjectEnd(concurrentStrings[i]);
            }

            currentItem++;

        }


        if (draggingString != null)
        {
            for (int i = 0; i < strings.Count; i++)
            {
                DropArea dropArea;
                if(dragNDrop.dropAreas.TryGetValue(i.ToString(), out dropArea)){
                    dragNDrop.TryCreateDropArea("before" + i, null, new Rect(dropArea.Rect.x, dropArea.Rect.y, dropArea.Rect.width, dropArea.Rect.height / 2f), dropArea.Types.ToArray());
                    dragNDrop.TryCreateDropArea("after" + i, null, new Rect(dropArea.Rect.x, dropArea.Rect.y + dropArea.Rect.height/2f, dropArea.Rect.width, dropArea.Rect.height / 2f), dropArea.Types.ToArray());

                    if (dragNDrop.DropAreaBegin("before" + i))
                    {
                        if (dragNDrop.ThisDropArea.ContainsMouse())
                        {
                            CustomGUIUtils.DrawBox(dragNDrop.ThisDropArea.Rect, XKCDColors.Greenish);
                            //GUI.Label(dragNDrop.ThisDropArea.Rect, "  " + i + ". " + dragNDrop.ThisDropArea.ToString());
                            GUI.Label(dragNDrop.ThisDropArea.Rect, "before   " + i);
                        }
                        dragNDrop.DropAreaEnd("before" + i);
                    }

                    if (dragNDrop.DropAreaBegin("after" + i))
                    {
                        //CustomGUIUtils.DrawBox(dragNDrop.ThisDropArea.Rect, XKCDColors.Greenish);
                        ////GUI.Label(dragNDrop.ThisDropArea.Rect, "  " + i + ". " + dragNDrop.ThisDropArea.ToString());
                        //GUI.Label(dragNDrop.ThisDropArea.Rect, "after   " + i);
                        if (dragNDrop.ThisDropArea.ContainsMouse())
                        {
                            CustomGUIUtils.DrawBox(dragNDrop.ThisDropArea.Rect, XKCDColors.Greenish);
                            //GUI.Label(dragNDrop.ThisDropArea.Rect, "  " + i + ". " + dragNDrop.ThisDropArea.ToString());
                            GUI.Label(dragNDrop.ThisDropArea.Rect, "after   " + i);
                        }
                        dragNDrop.DropAreaEnd("after" + i);
                    }

                }

            }


            bool newDropArea = false;
            if (dragNDrop.DragObjectBegin(draggingString))
            {
                CustomGUIUtils.DrawBox(dragNDrop.CurrentDragObject.CurrentRect, myGrey);
                GUI.Label(dragNDrop.CurrentDragObject.CurrentRect, draggingString.ToString());

                Rect icon = new Rect(dragNDrop.CurrentDragObject.CurrentRect);
                icon.width = icon.height;
                GUI.DrawTexture(icon, DefaultIconSet.Device);
                newDropArea = dragNDrop.DragObjectEnd(draggingString);
            }

            //if (newDropArea)
            //{

            //}

            DragObject currentstr = dragNDrop.TryGetDragObject(draggingString);
            if (!currentstr.IsDragging)
            {
                Debug.Log(currentstr.DropArea.Id);
                if (currentstr.DropArea.Id.StartsWith("before") || currentstr.DropArea.Id.StartsWith("after"))
                {
                    if (currentstr.DropArea.Id.StartsWith("before"))
                    {
                        int newIndex = int.Parse(currentstr.DropArea.Id.Replace("before", ""));
                        strings.Insert(newIndex, draggingString);
                    }
                    else if (currentstr.DropArea.Id.StartsWith("after"))
                    {
                        int newIndex = int.Parse(currentstr.DropArea.Id.Replace("after", "")) + 1;
                        strings.Insert(newIndex, draggingString);
                        currentstr.t = 0f;
                        currentstr.Ease = true;
                        currentstr.EaseEndRect = dragNDrop.dropAreas[newIndex.ToString()].Rect;
                    }

                }
                else
                {

                    //strings.Capacity += 1;
                    Debug.Log("old Index" + draggingStringOldIndex);
                    strings.Insert(draggingStringOldIndex, draggingString);
                }
                draggingString = null;
                draggingStringOldIndex = -1;
            }
        }

    }

}