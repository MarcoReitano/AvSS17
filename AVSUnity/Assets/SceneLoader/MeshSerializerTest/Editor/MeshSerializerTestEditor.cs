using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

[CustomEditor(typeof(MeshSerializerTest))]
public class MeshSerializerTestEditor : Editor
{

    MeshSerializerTest test;
    public void Awake()
    {
        test = target as MeshSerializerTest;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Serialize Mesh"))
        {
            Mesh mesh = test.GetComponent<MeshFilter>().sharedMesh;

            byte[] serializedMesh = MeshSerializer.WriteMesh(mesh, false);
            Debug.Log("SerializedMesh has " + serializedMesh.Length + " bytes");


            Mesh deserializedMesh = MeshSerializer.ReadMesh(serializedMesh);
            GameObject go = new GameObject("Deserialized Mesh");
            MeshFilter meshFilter = go.AddComponent<MeshFilter>();
            go.AddComponent<MeshRenderer>();

            meshFilter.sharedMesh = deserializedMesh;
        }

        if (GUILayout.Button("Serialize All Meshes in Scene"))
        {
            SceneMessage sceneMessage = new SceneMessage("bla", SceneManager.GetActiveScene());

            Debug.Log(sceneMessage.ToJSON());
        }
    }
}
