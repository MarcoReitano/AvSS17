using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CubicBezierSpline))]
public class CubicBezierSplineEditor : Editor
{

	private CubicBezierSpline cubicBezierSpline;

	public void Awake()
	{
		cubicBezierSpline = target as CubicBezierSpline;
	}



	public void OnSceneGUI()
	{
		
		bool changed = false;
//		foreach(CurvePoint point in cubicBezierSpline.points) {
		if(cubicBezierSpline.points != null) {
			for(int i = 0; i < cubicBezierSpline.points.Count; i++) {
				CurvePoint point = cubicBezierSpline.points[i];
				Handles.color = Color.red;
				Vector3 oldPosition = point.Position;
				Vector3 newPosition = Handles.FreeMoveHandle(point.Position, Quaternion.identity, 1f, Vector3.zero, Handles.SphereCap);
				if(newPosition != oldPosition) {
					point.Position = newPosition;
					changed = true;
				}
				
				Handles.color = Color.green;
				oldPosition = point.StartTangentPosition;
				newPosition = Handles.FreeMoveHandle(point.StartTangentPosition, Quaternion.identity, 0.5f, Vector3.zero, Handles.SphereCap);
				if(newPosition != oldPosition) {
					point.StartTangentPosition = newPosition;
					changed = true;
				}

				Handles.color = Color.blue;
				oldPosition = point.EndTangentPosition;
				newPosition = Handles.FreeMoveHandle(point.EndTangentPosition, Quaternion.identity, 0.5f, Vector3.zero, Handles.SphereCap);
				if(newPosition != oldPosition) {
					point.EndTangentPosition = newPosition;
					changed = true;
				}

				Handles.color = Color.yellow;
				Handles.DrawLine(point.Position, point.StartTangentPosition);
				Handles.DrawLine(point.Position, point.EndTangentPosition);
				Handles.Label(point.Position + Vector3.one, "P" + i);
			}
			
			if(changed)
				cubicBezierSpline.RecalculateCurve();
		}


//		Vector3 worldPoint = GetXZPlaneCollisionInEditor(currentEvent);
//
//			Handles.color = Color.green;
//			Handles.DrawLine(node.transform.position, worldPoint);
//
//			if(currentEvent.type == EventType.MouseDown) {
//
//				GameObject newNode = Instantiate(node.gameObject) as GameObject;
//				newNode.transform.position = worldPoint;
//				newNode.name = "newNode";
//
//				Selection.activeGameObject = newNode;
//			}

		
		if(GUI.changed)
			EditorUtility.SetDirty(cubicBezierSpline);
	}


	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		bool changed = false;
		int old = this.cubicBezierSpline.segments;
		this.cubicBezierSpline.segments = EditorGUILayout.IntSlider(this.cubicBezierSpline.segments, 1, 10000);
		if (old != this.cubicBezierSpline.segments)
            changed = true;

		bool oldCircle = this.cubicBezierSpline.circle;
		this.cubicBezierSpline.circle = EditorGUILayout.Toggle("closed circle?",this.cubicBezierSpline.circle);
		if (oldCircle != this.cubicBezierSpline.circle)
            changed = true;


		if (changed)
            cubicBezierSpline.RecalculateCurve();

		if(GUILayout.Button("AppendPoint")) {
			cubicBezierSpline.AppendPoint();
		}
		
		if(GUI.changed)
			EditorUtility.SetDirty(cubicBezierSpline);
	}
	
}

