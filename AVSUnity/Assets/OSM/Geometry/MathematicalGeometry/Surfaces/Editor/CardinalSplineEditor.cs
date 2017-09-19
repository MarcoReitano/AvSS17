using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(CardinalSpline))]
public class CardinalSplineEditor : Editor
{

    private CardinalSpline cardinalSpline;

	public void Awake()
	{
        cardinalSpline = target as CardinalSpline;
	}


	public void OnSceneGUI()
	{
		// Handles for the data points
		Handles.color = Color.red;
        bool changed = false;
        if (cardinalSpline.controlPoints.Count > 3)
        {
            for (int i = 0; i < cardinalSpline.controlPoints.Count; i++)
            {
                Vector3 oldPosition = cardinalSpline.controlPoints[i];
                Vector3 newPosition = Handles.FreeMoveHandle(oldPosition, Quaternion.identity, 1.0f, Vector3.zero, Handles.SphereCap);
                if (newPosition != oldPosition)
                {
                    cardinalSpline.controlPoints[i] = newPosition;
                    changed = true;
                }
            }
        }

        if (changed)
            cardinalSpline.CalculateSplinePoints();


//        Vector3 mousePosition = GetXZPlaneCollisionInEditor(Event.current);
////		Handles.SphereCap(0, mousePosition, Quaternion.identity, 5.3f);
////		mousePosition = Camera.current.ScreenToWorldPoint(Input.mousePosition);

////		Debug.Log(mousePosition);
//        Vector3 pointOnCurve = this.cardinalSpline.GetMinDistancePointOnCurve(mousePosition);
//        Handles.color = Color.yellow;
//        Handles.SphereCap(0, pointOnCurve, Quaternion.identity, 0.5f);


////		Vector2 pointOnScreen = (Vector2)Camera.current.WorldToScreenPoint(pointOnCurve);
//////		Vector3 mouseViewportPoint = Camera.current.ScreenToViewportPoint(Event.current.mousePosition);
////		Vector2 mouseViewportPoint = Event.current.mousePosition;
////
//////		Debug.Log(pointOnScreen +  "  " +  mouseViewportPoint);
////
////		float distance = Mathf.Abs(Vector2.Distance(mouseViewportPoint, pointOnScreen));
////		Debug.Log(pointOnScreen +  "  " +  mouseViewportPoint +  "  =  " + distance);
////
////		if(distance < 100f){
////			Handles.color = Color.yellow;
////			Handles.SphereCap(0, pointOnCurve, Quaternion.identity, 0.3f);
////		}

////
////		Handles.FreeMoveHandle(this.cubicBezier.GetMinDistancePointOnCurve(mousePosition), Quaternion.identity, 0.3f, Vector3.zero, Handles.SphereCap);

////		if (GUI.changed)
        	EditorUtility.SetDirty(cardinalSpline);
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
		int old = this.cardinalSpline.segments;
		this.cardinalSpline.segments = EditorGUILayout.IntSlider(this.cardinalSpline.segments, 1, 1000);
		if (old != this.cardinalSpline.segments)
            changed = true;


   
        if(GUILayout.Button("Add Point")){
            this.cardinalSpline.controlPoints.Add(new Vector3(0f,0f,0f));
        }

        //float oldValue = cardinalSpline.width;
        //float newValue = GUILayout.HorizontalSlider(cardinalSpline.width, 0f, 20f);
        //if (oldValue != newValue)
        //{
        //    cardinalSpline.width = newValue;
        //    cardinalSpline.offsets.Clear();
        //    cardinalSpline.offsets.Add(new Vector2(-cardinalSpline.width * 0.5f, 0f));
        //    cardinalSpline.offsets.Add(new Vector2(cardinalSpline.width * 0.5f, 0f));
        //    cardinalSpline.GenerateMesh();
        //}

        //bool oldBoolValue = cardinalSpline.isLine;
        //bool newBoolValue = EditorGUILayout.Toggle("Is Line", oldBoolValue);
        //if(oldBoolValue != newBoolValue){
        //    cardinalSpline.isLine = newBoolValue;

        //    if (cardinalSpline.isLine)
        //    {
        //        cardinalSpline.offsets.Clear();
        //        cardinalSpline.offsets.Add(new Vector2(-cardinalSpline.width * 0.5f, 0f));
        //        cardinalSpline.offsets.Add(new Vector2(cardinalSpline.width * 0.5f, 0f));
        //    }
        //    cardinalSpline.GenerateMesh();
        //}


		if (changed)
            cardinalSpline.CalculateSplinePoints();

//		if(GUILayout.Button("")) {
//
//		}

		if (GUI.changed)
            EditorUtility.SetDirty(cardinalSpline);
	}

}

