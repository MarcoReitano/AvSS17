using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;


public class CurvePoint : MonoBehaviour
{
	[SerializeField]
	private Vector3 oldPosition;
	[SerializeField]
	public Vector3 position;
	public Vector3 Position {
		get { return this.position; }
		set {
			this.oldPosition = this.position;
			this.position = value;
			transform.position = this.position;
			
			this.startTangentPosition = this.position + this.startTangentVector;
			this.endTangentPosition = this.position - this.endTangentVector;
		}
	}

	[SerializeField]
	public Vector3 startTangentVector;
	[SerializeField]
	private Vector3 oldStartTangentPosition;
	[SerializeField]
	public Vector3 startTangentPosition;
	public Vector3 StartTangentPosition {
		get { return this.startTangentPosition; }
		set {
			this.oldStartTangentPosition = this.startTangentPosition;
			this.startTangentPosition = value;
			this.startTangentVector = this.startTangentPosition - this.position;
			
			this.endTangentVector = this.startTangentVector.normalized * this.endTangentVector.magnitude;
			this.endTangentPosition = this.position - this.endTangentVector;
			
			RecalculateRotation();
		}
	}


	[SerializeField]
	public Vector3 endTangentVector;
	[SerializeField]
	private Vector3 oldEndTangentPosition;
	[SerializeField]
	public Vector3 endTangentPosition;
	public Vector3 EndTangentPosition {
		get { return this.endTangentPosition; }
		set {
			this.oldEndTangentPosition = this.endTangentPosition;
			this.endTangentPosition = value;
			this.endTangentVector = this.position - this.endTangentPosition;
			
			this.startTangentVector = this.endTangentVector.normalized * this.startTangentVector.magnitude;
			this.startTangentPosition = this.position + this.startTangentVector;
			
			RecalculateRotation();

		}
	}

	public float zRotationDegree = 0f;

	public void RecalculateRotation()
	{
		Quaternion calculatedRotation = Quaternion.LookRotation(startTangentPosition - endTangentPosition);
		Vector3 calculatedEulerAngles = calculatedRotation.eulerAngles;

		Vector3 correctRotation = new Vector3(calculatedEulerAngles.x, calculatedEulerAngles.y, zRotationDegree);
		this.Rotation = Quaternion.Euler(correctRotation);
	}

	public Quaternion rotation;
	public Quaternion Rotation {
		get { return this.transform.rotation; }
		set {
			this.rotation = value;
//			this.transform.rotation = Quaternion.Euler(Vector3.zero);

			this.transform.localRotation = rotation;
		}
	}

	public Vector3 normalPosition;
	public Vector3 normalVector;

	public void ApplyRotation(Vector3 euler){

		this.Rotation *= Quaternion.Euler(euler);
	}

	Vector3 testPosition;



	private Vector3 GetOrthogonalNormalizedVector()
    {
        Vector3 normal = Vector3.up;
        Vector3 zielVect = Vector3.zero;
        Vector3 tangent = startTangentVector;

        Vector3.OrthoNormalize(ref normal, ref tangent, ref zielVect);
        return zielVect;
    }

    public Vector3 GetOrthogonalNormalizedVectorDown()
    {
        Vector3 normal =  GetOrthogonalNormalizedVector();
        Vector3 zielVect = Vector3.zero;
		Vector3 tangent = startTangentVector;
        Vector3.OrthoNormalize(ref normal, ref tangent, ref zielVect);

        return zielVect;
    }

	public Vector3 GetOrthogonalNormalizedVectorUp()
    {
		return -GetOrthogonalNormalizedVectorDown();
    }



	public void Awake()
	{
		Debug.Log("Awake()");
	}


	public void Start()
	{
		Debug.Log("Start()");
		this.startTangentPosition = position + new Vector3(1f, 0, 1f);
		this.endTangentPosition = position + new Vector3(-1f, 0, -1f);
		RecalculateRotation();
	}



    public void OnDrawGizmos()
	{
		Gizmos.DrawIcon(Position, "CurvesAndSurfaces/red8.png");
	}

	public void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawLine(Position, StartTangentPosition);
		Gizmos.DrawLine(Position, EndTangentPosition);

#if UNITY_EDITOR
        Handles.Label(Position, "P");
		Handles.Label(StartTangentPosition, "Ts");
		Handles.Label(EndTangentPosition, "Te");
#endif
    }



    [SerializeField]
	private List<UnityEngine.Object> positionListener = new List<UnityEngine.Object>();


	public void AttachListener(UnityEngine.Object listener)
	{
		if(!positionListener.Contains(listener))
			positionListener.Add(listener);
	}

	public void DetachListener(UnityEngine.Object listener)
	{
		positionListener.Remove(listener);
	}



	public void FirePositionChanged()
	{
        //foreach(IDefaultListener listener in positionListener) {
        //    listener.OnChanged();
        //}
	}
	
}

