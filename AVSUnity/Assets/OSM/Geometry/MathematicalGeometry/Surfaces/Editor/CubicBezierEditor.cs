using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(CubicBezier))]
public class CubicBezierEditor : Editor
{

	private CubicBezier cubicBezier;

	public void Awake()
	{
		cubicBezier = target as CubicBezier;
	}


	public void OnSceneGUI()
	{
		// Handles for the data points
		Handles.color = Color.red;
		Vector3 oldPosition = this.cubicBezier.startPosition;
		Vector3 newPosition = Handles.FreeMoveHandle(this.cubicBezier.startPosition, Quaternion.identity, 0.3f, Vector3.zero, Handles.SphereCap);
		if(newPosition != oldPosition)
			this.cubicBezier.StartPosition = newPosition;

		oldPosition = this.cubicBezier.endPosition;
		newPosition = Handles.FreeMoveHandle(this.cubicBezier.endPosition, Quaternion.identity, 0.3f, Vector3.zero, Handles.SphereCap);
		if(newPosition != oldPosition)
			this.cubicBezier.EndPosition = newPosition;

		// Handles for the tangents
		Handles.color = Color.green;
		oldPosition = this.cubicBezier.StartTangentPosition;
		newPosition = Handles.FreeMoveHandle(this.cubicBezier.StartTangentPosition, Quaternion.identity, 0.2f, Vector3.zero, Handles.SphereCap);
		if(newPosition != oldPosition)
			this.cubicBezier.StartTangentPosition = newPosition;

		oldPosition = this.cubicBezier.EndTangentPosition;
		newPosition = Handles.FreeMoveHandle(this.cubicBezier.EndTangentPosition, Quaternion.identity, 0.2f, Vector3.zero, Handles.SphereCap);
		if(newPosition != oldPosition)
			this.cubicBezier.EndTangentPosition = newPosition;


		Vector3 mousePosition = GetXZPlaneCollisionInEditor(Event.current);
//		Handles.SphereCap(0, mousePosition, Quaternion.identity, 5.3f);
//		mousePosition = Camera.current.ScreenToWorldPoint(Input.mousePosition);

//		Debug.Log(mousePosition);
		Vector3 pointOnCurve = this.cubicBezier.GetMinDistancePointOnCurve(mousePosition);
		Handles.color = Color.yellow;
		Handles.SphereCap(0, pointOnCurve, Quaternion.identity, 0.5f);


//		Vector2 pointOnScreen = (Vector2)Camera.current.WorldToScreenPoint(pointOnCurve);
////		Vector3 mouseViewportPoint = Camera.current.ScreenToViewportPoint(Event.current.mousePosition);
//		Vector2 mouseViewportPoint = Event.current.mousePosition;
//
////		Debug.Log(pointOnScreen +  "  " +  mouseViewportPoint);
//
//		float distance = Mathf.Abs(Vector2.Distance(mouseViewportPoint, pointOnScreen));
//		Debug.Log(pointOnScreen +  "  " +  mouseViewportPoint +  "  =  " + distance);
//
//		if(distance < 100f){
//			Handles.color = Color.yellow;
//			Handles.SphereCap(0, pointOnCurve, Quaternion.identity, 0.3f);
//		}

//
//		Handles.FreeMoveHandle(this.cubicBezier.GetMinDistancePointOnCurve(mousePosition), Quaternion.identity, 0.3f, Vector3.zero, Handles.SphereCap);

//		if (GUI.changed)
        	EditorUtility.SetDirty(cubicBezier);
	}


	Plane plane = new Plane(Vector3.up, Vector3.zero);


	public Vector3 GetXZPlaneCollisionInEditor(Event currentEvent)
	{
		Vector3 returnCollision = Vector3.zero;
		
		if(Camera.current != null) {
			Camera sceneCamera = Camera.current;
			
			Vector2 correctedMousePosition = new Vector2(currentEvent.mousePosition.x, Screen.height - currentEvent.mousePosition.y - 35);
			Ray terrainRay = sceneCamera.ScreenPointToRay(correctedMousePosition);
//			Ray terrainRay2 = sceneCamera.ScreenPointToRay(new Vector2(Screen.width / 2 , Screen.height / 2));
//			if(terrainRay.direction == Vector3.zero) {
//				if(sceneCamera.transform.position.y < 0f)
//					terrainRay.direction = Vector3.up;
//				else
//					terrainRay.direction = Vector3.down; 
//			}

//			plane = new Plane(terrainRay2.direction, sceneCamera.transform.position + terrainRay2.direction.normalized * sceneCamera.far);

			float enter;
			if(plane.Raycast(terrainRay, out enter)) {
				returnCollision = sceneCamera.transform.position + terrainRay.direction.normalized * enter;
			}
		}
//		Debug.Log(returnCollision);
		return returnCollision;
	}

    float newOffset = 0;
    float newHeightOffset = 0;
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		bool changed = false;
		int old = this.cubicBezier.segments;
		this.cubicBezier.segments = EditorGUILayout.IntSlider(this.cubicBezier.segments, 1, 1000);
		if (old != this.cubicBezier.segments)
            changed = true;


        newOffset = EditorGUILayout.FloatField("New Offset Value", newOffset);
        newHeightOffset = EditorGUILayout.FloatField("New Offset Value", newHeightOffset);
        if(GUILayout.Button("Add new Offset")){
            this.cubicBezier.offsets.Add(new Vector2(newOffset, newHeightOffset));
        }

        float oldValue = cubicBezier.width;
        float newValue = GUILayout.HorizontalSlider(cubicBezier.width, 0f, 20f);
        if (oldValue != newValue)
        {
            cubicBezier.width = newValue;
            cubicBezier.offsets.Clear();
            cubicBezier.offsets.Add(new Vector2(-cubicBezier.width * 0.5f, 0f));
            cubicBezier.offsets.Add(new Vector2(cubicBezier.width * 0.5f, 0f));
            cubicBezier.GenerateMesh();
        }

        bool oldBoolValue = cubicBezier.isLine;
        bool newBoolValue = EditorGUILayout.Toggle("Is Line", oldBoolValue);
        if(oldBoolValue != newBoolValue){
            cubicBezier.isLine = newBoolValue;

            if (cubicBezier.isLine)
            {
                cubicBezier.offsets.Clear();
                cubicBezier.offsets.Add(new Vector2(-cubicBezier.width * 0.5f, 0f));
                cubicBezier.offsets.Add(new Vector2(cubicBezier.width * 0.5f, 0f));
            }
            cubicBezier.GenerateMesh();
        }


        if (GUILayout.Button("Make Street"))
        {
            this.cubicBezier.offsets.Clear();

            float akku=0;

            // Mittelstreifen
            this.cubicBezier.offsets.Add(new Vector2(akku, 0.4f));
            this.cubicBezier.offsets.Add(new Vector2(akku += 0.4f, 0.35f));
            this.cubicBezier.offsets.Add(new Vector2(akku += 0.7f, 0.12f));
            this.cubicBezier.offsets.Add(new Vector2(akku += 0.12f, 0.12f));
            this.cubicBezier.offsets.Add(new Vector2(akku += 0.03f, 0.00f));

            //Straße
            this.cubicBezier.offsets.Add(new Vector2(akku += 0.25f, 0.04f));
            this.cubicBezier.offsets.Add(new Vector2(akku += 2.5f, 0.1f));
            this.cubicBezier.offsets.Add(new Vector2(akku += 2.5f, 0.04f));
            this.cubicBezier.offsets.Add(new Vector2(akku += 0.25f, 0.0f));
            
            // Parkstreifen mit Bordstein
            this.cubicBezier.offsets.Add(new Vector2(akku += 0.075f, 0.05f));
            this.cubicBezier.offsets.Add(new Vector2(akku += 0.075f, 0.06f));
            this.cubicBezier.offsets.Add(new Vector2(akku += 1.85f, 0.06f));

            // Zwischenstreifen mit Bordstein
            this.cubicBezier.offsets.Add(new Vector2(akku += 0.075f, 0.16f));
            this.cubicBezier.offsets.Add(new Vector2(akku += 0.075f, 0.18f));
            this.cubicBezier.offsets.Add(new Vector2(akku += 0.60f, 0.18f));

            // Radweg
            this.cubicBezier.offsets.Add(new Vector2(akku += 1.6f, 0.2f));
            
            // Fußgänger
            this.cubicBezier.offsets.Add(new Vector2(akku += 4f, 0.3f));

            //Downward
            this.cubicBezier.offsets.Add(new Vector2(akku, -3f));
            

            akku = -akku;
            this.cubicBezier.offsets.Add(new Vector2(akku, -3f));
            this.cubicBezier.offsets.Add(new Vector2(akku, 0.3f));

            // Fußgänger
            this.cubicBezier.offsets.Add(new Vector2(akku += 4f, 0.2f));
            // Radweg
            this.cubicBezier.offsets.Add(new Vector2(akku += 1.6f, 0.18f));

            // Zwischenstreifen mit Bordstein
            this.cubicBezier.offsets.Add(new Vector2(akku += 0.60f, 0.18f));
            this.cubicBezier.offsets.Add(new Vector2(akku += 0.075f, 0.16f));
            this.cubicBezier.offsets.Add(new Vector2(akku += 0.075f, 0.06f));
        

            // Parkstreifen mit Bordstein
            this.cubicBezier.offsets.Add(new Vector2(akku += 1.85f, 0.06f));
            this.cubicBezier.offsets.Add(new Vector2(akku += 0.075f, 0.05f));
            this.cubicBezier.offsets.Add(new Vector2(akku += 0.075f, 0.0f));

            //Straße
            this.cubicBezier.offsets.Add(new Vector2(akku += 0.25f, 0.04f));
            this.cubicBezier.offsets.Add(new Vector2(akku += 2.5f, 0.1f));
            this.cubicBezier.offsets.Add(new Vector2(akku += 2.5f, 0.04f));
            this.cubicBezier.offsets.Add(new Vector2(akku += 0.25f, 0.00f));

            // Mittelstreifen
            this.cubicBezier.offsets.Add(new Vector2(akku += 0.03f, 0.12f));
            this.cubicBezier.offsets.Add(new Vector2(akku += 0.12f, 0.12f));
            this.cubicBezier.offsets.Add(new Vector2(akku += 0.7f, 0.35f));
            this.cubicBezier.offsets.Add(new Vector2(akku += 0.4f, 0.4f));
            //this.cubicBezier.offsets.Add(new Vector2(akku, 0.0f));
            
           
           
           

            
          
            
           
            

          
           
           
            

           
            
            
            

          

            

            

            changed = true;
        }


		if (changed)
            cubicBezier.RecalculateCurve();

//		if(GUILayout.Button("")) {
//
//		}

		if (GUI.changed)
            EditorUtility.SetDirty(cubicBezier);
	}

}

