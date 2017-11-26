using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using System.Linq;
using System.Text;

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

        if (GUILayout.Button("Serialize All Meshes in Scene"))
        {
            Debug.Log(SceneSerializer.SerializeScene(SceneManager.GetActiveScene()));

        }

        if (GUILayout.Button("Print Unity-Types"))
        {

            StringBuilder sb = new StringBuilder();
            var unityTypes = typeof(UnityEngine.Object).Assembly.GetTypes().Where(t => typeof(UnityEngine.Object).IsAssignableFrom(t));
            foreach (var item in unityTypes)
            {
                sb.AppendLine(item.ToString());
            }
            Debug.Log(sb.ToString());
        }

        if (GUILayout.Button("JSON test"))
        {
            Scene scene = SceneManager.GetActiveScene();


            Debug.Log(scene.ToJSON());
            string json = scene.ToJSON();

            System.IO.File.WriteAllText(Application.dataPath + @"\scene.txt", json);


        }
    }
}
