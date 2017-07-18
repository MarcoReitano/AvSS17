using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Line))]
public class LineEditor : Editor
{

    private Line line;

	public void Awake()
	{
        line = target as Line;
	}


	public void OnSceneGUI()
	{
		Event currentEvent = Event.current;

		Handles.color = Color.yellow;
		Vector3 oldPosition = this.line.P0;
		Vector3 newPosition = Handles.FreeMoveHandle(this.line.P0, Quaternion.identity, 0.2f, Vector3.zero, Handles.SphereCap);
		if(newPosition != oldPosition)
			this.line.P0 = newPosition;

		oldPosition = this.line.P1;
		newPosition = Handles.FreeMoveHandle(this.line.P1, Quaternion.identity, 0.2f, Vector3.zero, Handles.SphereCap);
		if(newPosition != oldPosition)
			this.line.P1 = newPosition;

        Vector3 mousePosition = GetXZPlaneCollisionInEditor(currentEvent);
        Vector3 pointOnCurve = this.line.GetMinDistancePointOnLine(mousePosition);
        Handles.color = Color.red;
        Handles.SphereCap(0, pointOnCurve, Quaternion.identity, 0.2f);


        EditorUtility.SetDirty(line);
	}


    

    public Vector3 GetXZPlaneCollisionInEditor(Event currentEvent)
    {
        Vector3 returnCollision = Vector3.zero;
        
        
        if (Camera.current != null)
        {
            Camera sceneCamera = Camera.current;

            Vector2 correctedMousePosition = new Vector2(currentEvent.mousePosition.x, Screen.height - currentEvent.mousePosition.y - 35);
            Ray terrainRay = sceneCamera.ScreenPointToRay(correctedMousePosition);

            Plane plane = new Plane(line.P0, line.P1, sceneCamera.transform.up);
            float enter;
            if (plane.Raycast(terrainRay, out enter))
            {
                returnCollision = sceneCamera.transform.position + terrainRay.direction.normalized * enter;
            }
        }
        //		Debug.Log(returnCollision);
        return returnCollision;
    }



	public override void OnInspectorGUI()
	{
		//DrawDefaultInspector();

//		if(GUILayout.Button("")) {
//
//		}
        EditorGUILayout.Separator();
        EditorGUILayout.Separator();

		Vector3 oldValue = line.P0;
		Vector3 newValue = EditorGUILayout.Vector3Field("P0", line.P0);
		if(oldValue != newValue)
			line.P0 = newValue;
		
		oldValue = line.P1;
		newValue = EditorGUILayout.Vector3Field("P1", line.P1);
		if(oldValue != newValue)
			line.P1 = newValue;

        EditorGUILayout.Separator();
        EditorGUILayout.Separator();

        float oldFloatValue = line.width;
        line.width = EditorGUILayout.Slider("Width", line.width, 0f, 50f); ;
        if (line.width != oldFloatValue)
            line.GenerateMesh();

		if (GUI.changed)
            EditorUtility.SetDirty(line);
	}

}

