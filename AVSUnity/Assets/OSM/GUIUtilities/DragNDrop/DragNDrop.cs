//using UnityEngine;
//using System.Collections;
//using System.Collections.Generic;

//public static class DragNDrop<T>
//{

//    #region Drag'n'Drop Rect
//    private class DragDropRectProperties
//    {
//        public string name = string.Empty;

//        public Rect startRect;
//        public Rect currentRect;
//        public Rect easeStartRect;
//        public Rect easeEndRect;

//        public bool isDragging;

//        public bool ease = false;
//        public EasingCurves.EaseType easeInType = EasingCurves.EaseType.easeOutElastic;
//        public int target = 1;
//        public float current = 0;
//        public float factor = 1;
//    }

//    private static Dictionary<object, DragDropRectProperties> dragDropRects = new Dictionary<object, DragDropRectProperties>();
//    private static DragDropRectProperties dragDropProperties;

//    private static bool isdragging = false;


//    public static void DragObjectBegin(object dragObject)
//    {
//        // Check if properties for the dragObject have been created
//        if (!dragDropRects.TryGetValue(dragObject, out dragDropProperties))
//        {
//            // create new properties
//            dragDropProperties = new DragDropRectProperties();
            
//            // initialize dragObjects current Rect with its dropareas
//            //dragObject.CurrentRect = dragObject.DropArea.Rect;
//            //dragDropProperties.dropAreas = dropAreas;

//            dragDropRects.Add(dragObject, dragDropProperties);
//        }
                
//        // Start GUIArea 
//        GUILayout.BeginArea(dragDropProperties.currentRect);
//        dragDropRects[dragObject] = dragDropProperties;
//    }


//    public static bool DragObjectEnd(DragObject<T> dragObject)
//    {
//        // End GUIArea
//        GUILayout.EndArea();

//        DropArea<T> selectedDropArea = null;

//        bool newDropPosition = false;

//        Event currentEvent = Event.current;
//        if (Event.current != null)
//        {
//            Vector3 mouse = InputHelpers.MouseOnGUI();

//            if (InputHelpers.MouseDown(0) && dragObject.CurrentRect.Contains(mouse)
//                || InputHelpers.MouseMoved(0) && dragDropProperties.isDragging)
//            {
//                dragDropProperties.isDragging = true;

//                dragObject.CurrentRect = new Rect(
//                    mouse.x - dragObject.CurrentRect.width / 2f,
//                    mouse.y - dragObject.CurrentRect.height / 2f,
//                    dragObject.CurrentRect.width,
//                    dragObject.CurrentRect.height);
//            }
//            else if (InputHelpers.MouseUp(0) && dragDropProperties.isDragging)
//            {
//                dragDropProperties.isDragging = false;
//                dragDropProperties.ease = true;

//                foreach (DropArea<T> dropArea in dragDropProperties.dropAreas)
//                {
//                    if (dropArea.Rect.Contains(InputHelpers.MouseOnGUI()) && dropArea.DragObject == null)
//                    {
//                        selectedDropArea = dropArea;
//                        newDropPosition = true;
//                        break;
//                    }
//                }
                
//                if (selectedDropArea != null && selectedDropArea.DragObject == null)
//                {
//                    dragDropProperties.easeStartRect = dragDropProperties.currentRect;
//                    dragDropProperties.easeEndRect = selectedDropArea.Rect;

//                    selectedDropArea.DragObject = dragObject;
                    
//                    dragObject.DropArea.DragObject = null;
//                    dragObject.DropArea = selectedDropArea;
//                }
//                else
//                {
//                    dragDropProperties.easeStartRect = dragDropProperties.currentRect;
//                    dragDropProperties.easeEndRect = dragObject.StartRect;
//                }
//            }
//        }

        
//        easeDragObject(dragObject);
        

//        return newDropPosition;
//    }
//    #region Easing the DragDropRect
//    private static void easeDragObject(DragObject<T> dragObject)
//    {
//        if (dragDropProperties.ease)
//        {
//            if (dragDropProperties.current < 1)
//                dragDropProperties.current += Time.deltaTime / dragDropProperties.factor;

//            dragDropProperties.current = Mathf.Clamp01(dragDropProperties.current);

//            if (dragDropProperties.ease)
//            {
//                dragObject.CurrentRect = EasingCurves.EaseRect(
//                    dragDropProperties.easeStartRect,
//                    dragDropProperties.easeEndRect,
//                    dragDropProperties.easeInType,
//                    dragDropProperties.current);
//            }

//            if (dragDropProperties.current == 1)
//            {
//                dragDropProperties.ease = false;
//                dragDropProperties.current = 0;
//            }
//        }
//    }
//    #endregion // Easing the Sidebar
//    #endregion //Drag'n'Drop Rect
//}
