using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif


[ExecuteInEditMode]
public class VertexBehaviour : MonoBehaviour {

    public Vertex vertex = new Vertex(Vector3.zero);

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        vertex.Position = transform.position;
	}


    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(vertex, 0.1f);
        
        Gizmos.color = Color.grey;
        Gizmos.DrawLine(vertex, vertex.Position.ToXZPlaneVector3());
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawCube(vertex.Position.ToXZPlaneVector3(), Vector3.one * 0.1f);
#if UNITY_EDITOR
        Handles.Disc(Quaternion.identity, vertex.Position.ToXZPlaneVector3(), Vector3.down, 0.3f, false, 0f);
        Handles.Label(new Vector3(vertex.X, vertex.Y * 0.5f, vertex.Z), (vertex.Y.ToString("0.00") + " m"));
#endif

    }
}
