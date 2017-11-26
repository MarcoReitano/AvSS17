using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.IO;

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

        if (GUILayout.Button("Surrogatte test"))
        {

            Scene scene = SceneManager.GetActiveScene();

            SceneSurrogate sceneSurrogate = new SceneSurrogate(scene);


            List<System.Type> knownTypes = new List<System.Type>();

            knownTypes.Add(typeof(Vector2Surrogate));
            knownTypes.Add(typeof(Vector3Surrogate));
            knownTypes.Add(typeof(Vector4Surrogate));
            knownTypes.Add(typeof(ColorSurrogate));
            knownTypes.Add(typeof(QuaternionSurrogate));
            knownTypes.Add(typeof(TransformSurrogate));
            knownTypes.Add(typeof(GameObjectSurrogate));
            knownTypes.Add(typeof(MeshFilterSurrogate));
            knownTypes.Add(typeof(MeshRendererSurrogate));
            knownTypes.Add(typeof(MeshSurrogate));
            knownTypes.Add(typeof(MaterialSurrogate));
            knownTypes.Add(typeof(ComponentSurrogate));
            knownTypes.Add(typeof(float));
            knownTypes.Add(typeof(int));
            knownTypes.Add(typeof(ListOfInt));
            knownTypes.Add(typeof(ListOfListOfInt));

            DataContractSerializer serializer = new DataContractSerializer(typeof(SceneSurrogate), knownTypes);
          

            FileStream writer = new FileStream(Application.dataPath + @"\sceneSurrogate.txt", FileMode.Create);
  
            serializer.WriteObject(writer, sceneSurrogate);
            writer.Close();

            FileStream file = File.OpenRead(Application.dataPath + @"\sceneSurrogate.txt");
            SceneSurrogate obj = (SceneSurrogate) serializer.ReadObject(file);
            file.Close();

            obj.Get();
            

        }
    }
}
