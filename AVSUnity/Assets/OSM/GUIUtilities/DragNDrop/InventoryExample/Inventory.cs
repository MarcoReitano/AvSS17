//using UnityEngine;
//using System.Collections;

//public class Inventory : MonoBehaviour {


//    DND dnd = new DND();

//    // Use this for initialization
//    void Start () {
	
//        dnd.CreateDropArea("CurrentWeapon", null, new Rect()


//    }
	
//    // Update is called once per frame
//    void Update () {
	
//    }

//    Color myGrey = new Color(0.5f, 0.5f, 0.5f, 0.5f);

//    void OnGUI()
//    {

//        for (int i = 0; i < 4; i++)
//        {
//            dragNDrop.DropAreaBegin("A" + i);
//            CustomGUIUtils.DrawBox(dragNDrop.ThisDropArea.Rect, XKCDColors.LightRed);
//            GUI.Label(dragNDrop.ThisDropArea.Rect, dragNDrop.ThisDropArea.ToString());
//            dragNDrop.DropAreaEnd("A" + i);


//            dragNDrop.DropAreaBegin("B" + i);
//            CustomGUIUtils.DrawBox(dragNDrop.ThisDropArea.Rect, XKCDColors.GreenApple);
//            GUI.Label(dragNDrop.ThisDropArea.Rect, dragNDrop.ThisDropArea.ToString());
//            dragNDrop.DropAreaEnd("B" + i);


//            dragNDrop.DropAreaBegin("C" + i);
//            CustomGUIUtils.DrawBox(dragNDrop.ThisDropArea.Rect, XKCDColors.LightGreyBlue);
//            GUI.Label(dragNDrop.ThisDropArea.Rect, dragNDrop.ThisDropArea.ToString());
//            dragNDrop.DropAreaEnd("C" + i);
//        }


//        foreach (object obj in objects)
//        {
//            dragNDrop.DragObjectBegin(obj);
//            CustomGUIUtils.DrawBox(dragNDrop.dragObjectProperties.currentRect, myGrey);
//            GUI.Label(dragNDrop.dragObjectProperties.currentRect, obj.ToString());
//            GUI.DrawTexture(dragNDrop.dragObjectProperties.currentRect, DefaultIconSet.Device);
//            dragNDrop.DragObjectEnd(obj);
//        }


//        foreach (DropArea dropArea in dragNDrop.dropAreas.Values)
//        {
//            if (dropArea.Object != null)
//            {
//                if (dropArea.Object.GetType() == typeof(A))
//                {
//                    A readA = dropArea.getObject<A>();
//                    Debug.Log("A gelesen: " + readA.ToString());
//                }
//            }
//        }

//    }
//}
