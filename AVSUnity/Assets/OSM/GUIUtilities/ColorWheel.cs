using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class ColorWheel {

    public static Texture2D BlackTransparentGradient;

    static ColorWheel()
    {
        BlackTransparentGradient = CustomGUIUtils.GetGradientTexture(255, 1, new Color(0, 0, 0, 1f), Color.black);
    }

    #region ColorPicker
    private class ColorPickerProperties
    {
        public float angle;
        public float radius;
        public float dimm;

        public Vector2 pickerXY;
        public bool isDragging = false;
    }

    private static Dictionary<string, ColorPickerProperties> colorPickers = new Dictionary<string, ColorPickerProperties>();
    private static ColorPickerProperties currentColorPicker;

    public static void ColorPicker(string name, Rect boundingRect, ref float angle, ref float radius, ref float dimm, ref Color color)
    {
        // Draw bounding Rectangle
        Rect newBounds = new Rect(boundingRect);
        newBounds.height += 70f;
        newBounds = newBounds.AddFrame(4);
        GUIAreas.Add(name, newBounds);
        CustomGUIUtils.DrawFrameBox(newBounds, Color.white, 1f, Color.black);
        CustomGUIUtils.DrawFrameBox(boundingRect, Color.white, 1f, Color.black);

        // Calculate box-values
        float boxWidth = Mathf.Min(boundingRect.width, boundingRect.height);
        float boxWidthHalf = boxWidth / 2f;

        // Draw Color-Wheel
        GUI.DrawTexture(boundingRect, DefaultIconSet.ColorWheel);

        // Get old State
        if (!colorPickers.TryGetValue(name, out currentColorPicker))
        {
            currentColorPicker = new ColorPickerProperties();
            //currentColorPicker.pickerXY = new Vector2(boxWidthHalf, boxWidthHalf);
            colorPickers.Add(name, currentColorPicker);
        }

        if (!currentColorPicker.isDragging)
        {
            // Calculate Vector for the PickerPosition
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            Vector3 vect = rotation * Vector3.left;
            Debug.Log(vect);

            currentColorPicker.pickerXY = vect.normalized * ((boxWidthHalf / 100) * radius) + boundingRect.Position().Vector3FromVector2XY();
            Debug.Log("Calculated Color Picker: " + currentColorPicker.pickerXY);

            currentColorPicker.dimm = dimm;
            currentColorPicker.angle = angle;
            currentColorPicker.radius = radius;
        }

        Vector3 mousePosition = Input.mousePosition.SwapScreenToWorldPoint();
        Vector2 mouseXY = new Vector2(mousePosition.x, mousePosition.y);

        // Draw Picker
        Rect pickerRect = new Rect(
                currentColorPicker.pickerXY.x + boundingRect.xMin - 10f,
                currentColorPicker.pickerXY.y + boundingRect.yMin - 10f,
                20f,
                20f);
        GUI.DrawTexture(pickerRect, DefaultIconSet.Location);

        // Handle Mouse-Input
        if (pickerRect.Contains(Input.mousePosition.SwapScreenToWorldPoint()) && Input.GetMouseButtonDown(0))
        {
            currentColorPicker.isDragging = true;
        }
        else if (currentColorPicker.isDragging && Input.GetMouseButton(0))
        {
            // Set Color-Picker Position
            currentColorPicker.pickerXY = mouseXY - boundingRect.Position();

            Vector2 direction = mouseXY - boundingRect.center;
            if (mouseXY.y > boundingRect.center.y)
                angle = 360 - Vector2.Angle(-Vector2.right, direction);
            else
                angle = Vector2.Angle(-Vector2.right, direction);


            float magnitude = direction.magnitude / (boxWidthHalf / 100);
            radius = Mathf.Clamp(magnitude, 0f, 100f);

            currentColorPicker.pickerXY = boundingRect.Position() + direction.normalized * Mathf.Clamp(magnitude, 0, boxWidthHalf);

            currentColorPicker.angle = angle;
            currentColorPicker.radius = radius;

        }
        else if (currentColorPicker.isDragging && Input.GetMouseButtonUp(0))
        {
            currentColorPicker.isDragging = false;
        }

        // Calculate Pixel Coordinates of the picker in the ColorWheel
        Vector2 pixelCoords = new Vector2();
        pixelCoords.x = ((float)DefaultIconSet.ColorWheel.width / boxWidth) * currentColorPicker.pickerXY.x;
        pixelCoords.y = DefaultIconSet.ColorWheel.height - ((float)DefaultIconSet.ColorWheel.height / boxWidth) * currentColorPicker.pickerXY.y;
        //Debug.Log("pixelcoords: " + pixelCoords);

        color = DefaultIconSet.ColorWheel.GetPixel((int)pixelCoords.x, (int)pixelCoords.y);

        // Draw Dimmer-Gradient-Color-View        
        CustomGUIUtils.DrawFrameBox(new Rect(boundingRect.xMin, boundingRect.yMax + 5, boxWidth, 25), color, 1f, Color.black);
        GUI.DrawTexture(new Rect(boundingRect.xMin, boundingRect.yMax + 5, boxWidth, 25), BlackTransparentGradient);

        angle = currentColorPicker.angle;
        radius = currentColorPicker.radius;
        dimm = GUI.HorizontalSlider(new Rect(boundingRect.xMin, boundingRect.yMax + 40f, boxWidth, 20), Mathf.Clamp(currentColorPicker.dimm, 0, 100), 0f, 100f); ;
        currentColorPicker.dimm = dimm;

        // Draw Dimmer-Label
        GUIStyle style = CustomGUIUtils.GetColorTextStyle(Color.black, 14);
        style.alignment = TextAnchor.MiddleCenter;
        GUI.Label(new Rect(boundingRect.xMin, boundingRect.yMax + 50f, boxWidth, 20), ((int)dimm).ToString(), style);
        //Debug.Log(currentColorPicker.pickerXY.ToString() + "  angle: " + angle + "  r=" + radius + "  dimm=" + dimm);
        colorPickers[name] = currentColorPicker;
    }
    #endregion // ColorPicker
}
