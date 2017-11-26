using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class MeshAnalyse : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnDrawGizmos()
    {
        MeshFilter mf = GetComponent<MeshFilter>();
        if (mf != null)
        {
            Mesh mesh = mf.sharedMesh;

            foreach (Vector3 vertex in mesh.vertices)
            {
                Gizmos.DrawSphere(vertex, 0.1f);
            }

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < mesh.triangles.Length; i++)
            {
                sb.Append(mesh.triangles[i] + ", ");
            }
            Debug.Log(sb.ToString());
        }

    }
}
