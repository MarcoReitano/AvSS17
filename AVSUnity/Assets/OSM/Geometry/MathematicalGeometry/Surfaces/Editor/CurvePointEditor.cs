using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CurvePoint))]
public class CurvePointEditor : Editor
{

	private CurvePoint curvePoint;

	public void Awake()
	{
		curvePoint = target as CurvePoint;
	}

	Vector3 oldTransformPosition = Vector3.zero;

	public void OnSceneGUI()
	{
		Event currentEvent = Event.current;
		
		Handles.color = Color.red;
		Vector3 oldPosition = this.curvePoint.Position;
		Vector3 newPosition = Handles.FreeMoveHandle(this.curvePoint.Position, Quaternion.identity, 1f, Vector3.zero, Handles.SphereCap);
		if(newPosition != oldPosition)
			this.curvePoint.Position = newPosition;
		
		if(this.curvePoint.transform.position != oldTransformPosition)
			this.curvePoint.Position = this.curvePoint.transform.position;
		
		Handles.color = Color.green;
		oldPosition = this.curvePoint.StartTangentPosition;
		newPosition = Handles.FreeMoveHandle(this.curvePoint.StartTangentPosition, Quaternion.identity, 0.5f, Vector3.zero, Handles.SphereCap);
		if(newPosition != oldPosition)
			this.curvePoint.StartTangentPosition = newPosition;
		
		Handles.color = Color.green;
		oldPosition = this.curvePoint.EndTangentPosition;
		newPosition = Handles.FreeMoveHandle(this.curvePoint.EndTangentPosition, Quaternion.identity, 0.5f, Vector3.zero, Handles.SphereCap);
		if(newPosition != oldPosition)
			this.curvePoint.EndTangentPosition = newPosition;
		

//		Handles.color = Color.blue;
//		oldPosition = this.curvePoint.normalPosition;
//		float oldAngle = Vector3.Angle(oldPosition - this.curvePoint.Position, Vector3.up);
////		Debug.Log(angle);
//		newPosition = Handles.FreeMoveHandle(this.curvePoint.normalPosition, Quaternion.identity, 0.5f, Vector3.zero, Handles.SphereCap);
//		if(newPosition != oldPosition){
//			this.curvePoint.normalVector = (newPosition - this.curvePoint.Position).normalized * 5f;
//			Vector3 test = new Vector3(this.curvePoint.normalVector.x, this.curvePoint.normalVector.y, 0f);
//			this.curvePoint.normalPosition = this.curvePoint.Position + test;
//
//			this.curvePoint.normalVector= Vector3.Project(test, this.curvePoint.GetOrthogonalNormalizedVectorUp());
//			
//			float angle = Vector3.Angle(curvePoint.normalVector, Vector3.up);
//
//			this.curvePoint.zRotationDegree = angle;
////			this.curvePoint.ApplyRotation(new Vector3(0f, 0f, this.curvePoint.zRotationDegree));
//		}


////		Handles.color = Color.blue;
		if(currentEvent.control) {
			Quaternion oldRotation = this.curvePoint.transform.rotation;
			Quaternion newRotation = Handles.RotationHandle(this.curvePoint.rotation, this.curvePoint.Position);

//			if(currentEvent.type == EventType.MouseUp) {
				if(newRotation != oldRotation) {
					Vector3 newEuler = newRotation.eulerAngles;
					Vector3 oldEuler = oldRotation.eulerAngles;
					Vector3 correctedEuler = new Vector3(oldEuler.x, oldEuler.y, newEuler.z);
					this.curvePoint.Rotation = Quaternion.Euler(correctedEuler);
				}
//			}

		}

		if(GUI.changed)
			EditorUtility.SetDirty(curvePoint);
	}


	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
		
		float oldValue = curvePoint.zRotationDegree;
		curvePoint.zRotationDegree = EditorGUILayout.FloatField("zRotation", this.curvePoint.zRotationDegree) % 360;
		if(oldValue != curvePoint.zRotationDegree)
			curvePoint.RecalculateRotation();
		
		if(GUI.changed) {
			curvePoint.FirePositionChanged();
			EditorUtility.SetDirty(curvePoint);
		}
	}
	
}

