//using UnityEngine;
//using System.Collections;
//using System.Collections.Generic;


//public class DragNDropUseNew2 : MonoBehaviour
//{
    
//    // Use this for initialization
//    void Start()
//    {
//        float columnCount = 6f;
//        float columnWidth = Screen.width / columnCount;
//        float gapWidth = 20f;
//        float boxWidth = columnWidth - gapWidth;

//        DND.createDropArea(typeof(GameObject), new Rect(0, 0, 100, 100));

//        dropAreasA = new List<DropArea<A>>();

//        // DropAreas A
//        int boxCount = 8;
//        float heightStep = Screen.height / boxCount;
//        for (int y = 0; y < boxCount; y++)
//            dropAreasA.Add(new DropArea<A>(null, new Rect(gapWidth, y * heightStep + 10, boxWidth, heightStep - 20)));

//        for (int y = 0; y < boxCount; y++)
//            dropAreasA.Add(new DropArea<A>(null, new Rect(Screen.width - columnWidth + gapWidth, y * heightStep + 10, boxWidth, heightStep - 20)));

//        // DropAreas B
//        for (int y = 0; y < boxCount; y++)
//            dropAreasB.Add(new DropArea<B>(null, new Rect(columnWidth + gapWidth, y * heightStep + 10, boxWidth, heightStep - 20)));

//        for (int y = 0; y < boxCount; y++)
//            dropAreasB.Add(new DropArea<B>(null, new Rect(Screen.width - columnWidth * 2 + gapWidth, y * heightStep + 10, boxWidth, heightStep - 20)));

//        // DropAreas C
//        for (int y = 0; y < boxCount; y++)
//            dropAreasC.Add(new DropArea<C>(null, new Rect(2f * columnWidth + gapWidth, y * heightStep + 10, boxWidth, heightStep - 20)));

//        for (int y = 0; y < boxCount; y++)
//            dropAreasC.Add(new DropArea<C>(null, new Rect(Screen.width - columnWidth * 3 + gapWidth, y * heightStep + 10, boxWidth, heightStep - 20)));

//        List<DropArea<A>> combined = new List<DropArea<A>>();
//        combined.AddRange(dropAreasA);

//        boxCount = 4;
//        dragObjectsA = new List<DragObject<A>>();
//        for (int y = 0; y < boxCount; y++)
//            dragObjectsA.Add(new DragObject<A>(new A(y), dropAreasA[y]));

//        dragObjectsB = new List<DragObject<B>>();
//        for (int y = 0; y < boxCount; y++)
//            dragObjectsB.Add(new DragObject<B>(new B(y), dropAreasB[y]));

//        dragObjectsC = new List<DragObject<C>>();
//        for (int y = 0; y < boxCount; y++)
//            dragObjectsC.Add(new DragObject<C>(new C("C" + y), dropAreasC[y]));

//    }

//    // Update is called once per frame
//    void Update()
//    {

//    }


//    void OnGUI()
//    {
//        for (int i = 0; i < dropAreasA.Count; i++)
//        {
//            CustomGUIUtils.DrawBox(dropAreasA[i].Rect, XKCDColors.LightRed);
//            GUI.Label(dropAreasA[i].Rect, dropAreasA[i].ToString());
//        }

//        for (int i = 0; i < dropAreasB.Count; i++)
//        {
//            CustomGUIUtils.DrawBox(dropAreasB[i].Rect, XKCDColors.LightGreyGreen);
//            GUI.Label(dropAreasB[i].Rect, dropAreasB[i].ToString());
//        }

//        for (int i = 0; i < dropAreasC.Count; i++)
//        {
//            CustomGUIUtils.DrawBox(dropAreasC[i].Rect, XKCDColors.LightGreyBlue);
//            GUI.Label(dropAreasC[i].Rect, dropAreasC[i].ToString());
//        }


//        foreach (DragObject<A> dragObject in this.dragObjectsA)
//        {
//            Rect drawRect = new Rect(dragObject.CurrentRect.xMin,
//            dragObject.CurrentRect.yMin,
//            dragObject.CurrentRect.width,
//            dragObject.CurrentRect.height);

//            CustomGUIUtils.DrawBox(drawRect, Color.red);

//            DragNDrop<A>.DragObjectBegin(dragObject, dropAreasA);

//            GUILayout.Label(dragObject.DraggedObject.ToString());
//            GUI.DrawTexture(new Rect(0, 0, dragObject.CurrentRect.height, dragObject.CurrentRect.height), DefaultIconSet.Device);

//            DragNDrop<A>.DragObjectEnd(dragObject);
//        }

//        foreach (DragObject<B> dragObject in this.dragObjectsB)
//        {

//            Rect drawRect = new Rect(dragObject.CurrentRect.xMin,
//            dragObject.CurrentRect.yMin,
//            dragObject.CurrentRect.width,
//            dragObject.CurrentRect.height);

//            CustomGUIUtils.DrawBox(drawRect, Color.green);

//            DragNDrop<B>.DragObjectBegin(dragObject, dropAreasB);

//            GUILayout.Label(dragObject.DraggedObject.ToString());
//            GUI.DrawTexture(new Rect(0, 0, dragObject.CurrentRect.height, dragObject.CurrentRect.height), DefaultIconSet.Device);

//            DragNDrop<B>.DragObjectEnd(dragObject);
//        }

//        foreach (DragObject<C> dragObject in this.dragObjectsC)
//        {
//            Rect drawRect = new Rect(dragObject.CurrentRect.xMin,
//            dragObject.CurrentRect.yMin,
//            dragObject.CurrentRect.width,
//            dragObject.CurrentRect.height);

//            CustomGUIUtils.DrawBox(drawRect, Color.blue);

//            DragNDrop<C>.DragObjectBegin(dragObject, dropAreasC);

//            GUILayout.Label(dragObject.DraggedObject.ToString());
//            GUI.DrawTexture(new Rect(0, 0, dragObject.CurrentRect.height, dragObject.CurrentRect.height), DefaultIconSet.Device);

//            DragNDrop<C>.DragObjectEnd(dragObject);
//        }
//    }

//}
