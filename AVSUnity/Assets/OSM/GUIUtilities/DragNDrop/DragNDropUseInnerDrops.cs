using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

//[ExecuteInEditMode]
public class DragNDropUseInnerDrops : MonoBehaviour
{

    public DND dragNDrop = new DND();

    List<String> strings = new List<String>();

    // Use this for initialization
    public void Start()
    {
        Rect rect1 = new Rect(0, 0, 150, 400);
        Rect rect4 = new Rect(Screen.width - 150, Screen.height - 400, 150, 400);
        
        Rect rect2 = new Rect(10, 10, 130, 150);
        Rect rect3 = new Rect(10, 210, 130, 150);

        dragNDrop.CreateDropArea("outer", null, rect1, typeof(String));
        dragNDrop.CreateDropArea("outer2", null, rect4, typeof(String));
        
        dragNDrop.CreateDropArea("inner1", null, rect2, typeof(String));
        dragNDrop.CreateDropArea("inner2", null, rect3, typeof(String));


        strings.Add(new String("outer".ToCharArray()));
        strings.Add(new String("inner1".ToCharArray()));
        strings.Add(new String("inner2".ToCharArray()));


        dragNDrop.SetObject("outer", strings[0]);
        dragNDrop.SetObject("inner1", strings[1]);
        dragNDrop.SetObject("inner2", strings[2]);
    }


    // Update is called once per frame
    void Update()
    {

    }

    Color myGrey = new Color(0.5f, 0.5f, 0.5f, 0.5f);

    public void OnGUI()
    {
        if (dragNDrop.DropAreaBegin("outer"))
        {
            CustomGUIUtils.DrawBox(dragNDrop.ThisDropArea.Rect, XKCDColors.LightGreyBlue);
            GUI.Label(dragNDrop.ThisDropArea.Rect, dragNDrop.ThisDropArea.ToString());
            dragNDrop.DropAreaEnd("outer");
        }

        if (dragNDrop.DropAreaBegin("outer2"))
        {
            CustomGUIUtils.DrawBox(dragNDrop.ThisDropArea.Rect, XKCDColors.LightGreyBlue);
            GUI.Label(dragNDrop.ThisDropArea.Rect, dragNDrop.ThisDropArea.ToString());
            dragNDrop.DropAreaEnd("outer2");
        }

        if (dragNDrop.DragObjectBegin(strings[0]))
        {
            CustomGUIUtils.DrawBox(dragNDrop.CurrentDragObject.CurrentRect, myGrey);
            GUI.Label(dragNDrop.CurrentDragObject.CurrentRect, strings[0].ToString());
            GUI.DrawTexture(dragNDrop.CurrentDragObject.CurrentRect, DefaultIconSet.Device);

            // DropAreas
            if (dragNDrop.DropAreaBegin("inner1"))
            {
                Rect drawRect = new Rect(dragNDrop.CurrentDragObject.CurrentRect);
                drawRect.x += 10;
                drawRect.y += 10;
                drawRect.width = 130;
                drawRect.height = 150;

                dragNDrop.ThisDropArea.Rect = drawRect;
                CustomGUIUtils.DrawBox(drawRect, XKCDColors.LightRed);
                GUI.Label(drawRect, dragNDrop.ThisDropArea.ToString());
                dragNDrop.DropAreaEnd("inner1");
            }

            if (dragNDrop.DropAreaBegin("inner2"))
            {
                Rect drawRect = new Rect(dragNDrop.CurrentDragObject.CurrentRect);
                drawRect.x += 10;
                drawRect.y += 210;
                drawRect.width = 130;
                drawRect.height = 150;
                
                dragNDrop.ThisDropArea.Rect = drawRect;
                CustomGUIUtils.DrawBox(drawRect, XKCDColors.GreenApple);
                GUI.Label(drawRect, dragNDrop.ThisDropArea.ToString());
                dragNDrop.DropAreaEnd("inner2");
            }


            // DragObjects
            if (dragNDrop.DragObjectBegin(strings[1]))
            {
                CustomGUIUtils.DrawBox(dragNDrop.CurrentDragObject.CurrentRect, myGrey);
                GUI.Label(dragNDrop.CurrentDragObject.CurrentRect, strings[1].ToString());
                GUI.DrawTexture(dragNDrop.CurrentDragObject.CurrentRect, DefaultIconSet.Device);
                dragNDrop.DragObjectEnd(strings[1]);
            }

            if (dragNDrop.DragObjectBegin(strings[2]))
            {
                CustomGUIUtils.DrawBox(dragNDrop.CurrentDragObject.CurrentRect, myGrey);
                GUI.Label(dragNDrop.CurrentDragObject.CurrentRect, strings[2].ToString());
                GUI.DrawTexture(dragNDrop.CurrentDragObject.CurrentRect, DefaultIconSet.Device);
                dragNDrop.DragObjectEnd(strings[2]);
            }
            
            
            dragNDrop.DragObjectEnd(strings[0]);
        }


        //foreach (DropArea dropArea in dragNDrop.dropAreas.Values)
        //{
        //    if (dropArea.Object != null)
        //    {
        //        if (dropArea.Object.GetType() == typeof(A))
        //        {
        //            A readA = dropArea.GetObject<A>();
        //            Debug.Log("A gelesen: " + readA.ToString());
        //        }
        //    }
        //}

    }

}