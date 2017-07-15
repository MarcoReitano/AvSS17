using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public class DragNDropUseNew: MonoBehaviour
{

    public DND dragNDrop = new DND();

    A a1 = new A(1);
    A a2 = new A(2);

    B b1 = new B(1);
    B b2 = new B(2);

    C c1 = new C("C1");
    C c2 = new C("C2");

    List<object> objects = new List<object>();

    List<String> strings = new List<String>();

    // Use this for initialization
    public void Start()
    {
        float boxHeight = 100;
        float boxWidth = 200;
        for (int i = 0; i < 4; i++)
        {
            Rect rect = new Rect(2 * boxWidth, i * boxHeight, boxWidth, boxHeight);
            Rect rect2 = new Rect(3 * boxWidth, i * boxHeight, boxWidth, boxHeight);
            Rect rect3 = new Rect(4 * boxWidth, i * boxHeight, boxWidth * 2, boxHeight);

            dragNDrop.CreateDropArea("A" + i, null, rect, typeof(A), typeof(C));
            dragNDrop.CreateDropArea("B" + i, null, rect2, typeof(B));
            dragNDrop.CreateDropArea("C" + i, null, rect3, typeof(C), typeof(A));

        }

        dragNDrop.SetObject("A1", a1);
        dragNDrop.SetObject("A2", a2);

        dragNDrop.SetObject("B1", b1);
        dragNDrop.SetObject("B2", b2);

        dragNDrop.SetObject("C1", c1);
        dragNDrop.SetObject("C2", c2);

        objects.Add(a1);
        objects.Add(a2);
        objects.Add(b1);
        objects.Add(b2);
        objects.Add(c1);
        objects.Add(c2);

        Rect rect4 = new Rect(0, 0, 150, 400);
        Rect rect5 = new Rect(10, 10, 130, 180);
        Rect rect6 = new Rect(10, 210, 130, 180);

        dragNDrop.CreateDropArea("D", null, rect4, typeof(String));
        dragNDrop.CreateDropArea("inner1", null, rect5, typeof(String));
        dragNDrop.CreateDropArea("inner2", null, rect6, typeof(String));


        strings.Add(new String("outer".ToCharArray()));
        strings.Add(new String("inner1".ToCharArray()));
        strings.Add(new String("inner1".ToCharArray()));


        //dragNDrop.SetObject("D", strings[0]);
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

        for (int i = 0; i < 4; i++)
        {
            dragNDrop.DropAreaBegin("A" + i);
            CustomGUIUtils.DrawBox(dragNDrop.ThisDropArea.Rect, XKCDColors.LightRed);
            GUI.Label(dragNDrop.ThisDropArea.Rect, dragNDrop.ThisDropArea.ToString());
            dragNDrop.DropAreaEnd("A" + i);


            dragNDrop.DropAreaBegin("B" + i);
            CustomGUIUtils.DrawBox(dragNDrop.ThisDropArea.Rect, XKCDColors.GreenApple);
            GUI.Label(dragNDrop.ThisDropArea.Rect, dragNDrop.ThisDropArea.ToString());
            dragNDrop.DropAreaEnd("B" + i);


            dragNDrop.DropAreaBegin("C" + i);
            CustomGUIUtils.DrawBox(dragNDrop.ThisDropArea.Rect, XKCDColors.LightGreyBlue);
            GUI.Label(dragNDrop.ThisDropArea.Rect, dragNDrop.ThisDropArea.ToString());
            dragNDrop.DropAreaEnd("C" + i);
        }

        dragNDrop.DropAreaBegin("D");
        CustomGUIUtils.DrawBox(dragNDrop.ThisDropArea.Rect, XKCDColors.LightGreyBlue);
        GUI.Label(dragNDrop.ThisDropArea.Rect, dragNDrop.ThisDropArea.ToString());
        dragNDrop.DropAreaEnd("D");

        dragNDrop.DropAreaBegin("inner1");
        CustomGUIUtils.DrawBox(dragNDrop.ThisDropArea.Rect, XKCDColors.LightRed);
        GUI.Label(dragNDrop.ThisDropArea.Rect, dragNDrop.ThisDropArea.ToString());
        dragNDrop.DropAreaEnd("inner1");

        dragNDrop.DropAreaBegin("inner2");
        CustomGUIUtils.DrawBox(dragNDrop.ThisDropArea.Rect, XKCDColors.GreenApple);
        GUI.Label(dragNDrop.ThisDropArea.Rect, dragNDrop.ThisDropArea.ToString());
        dragNDrop.DropAreaEnd("inner2");


        foreach (String stri in strings)
        {
            dragNDrop.DragObjectBegin(stri);
            CustomGUIUtils.DrawBox(dragNDrop.CurrentDragObject.CurrentRect, myGrey);
            GUI.Label(dragNDrop.CurrentDragObject.CurrentRect, stri.ToString());
            GUI.DrawTexture(dragNDrop.CurrentDragObject.CurrentRect, DefaultIconSet.Device);
            dragNDrop.DragObjectEnd(stri);
        }



        foreach (object obj in objects)
        {
            dragNDrop.DragObjectBegin(obj);

            if (dragNDrop.CurrentDragObject.IsDragging)
            {
                Rect drawRect = dragNDrop.CurrentDragObject.CurrentRect;
                drawRect.width = drawRect.height = 50;
                drawRect.x = InputHelpers.MouseOnGUI().x - drawRect.width / 2;
                drawRect.y = InputHelpers.MouseOnGUI().y - drawRect.height / 2;
                GUI.DrawTexture(drawRect, DefaultIconSet.Device);
            }
            else
            {
                CustomGUIUtils.DrawBox(dragNDrop.CurrentDragObject.CurrentRect, myGrey);
                GUI.Label(dragNDrop.CurrentDragObject.CurrentRect, obj.ToString());
                GUI.DrawTexture(dragNDrop.CurrentDragObject.CurrentRect, DefaultIconSet.Device);
            }



            dragNDrop.DragObjectEnd(obj);
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
