using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using ProtoBuf;
using UnityEditor;
using UnityEngine;
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
        //base.OnInspectorGUI();
        //if (GUILayout.Button("Serialize Mesh"))
        //{
        //    Mesh mesh = test.GetComponent<MeshFilter>().sharedMesh;

        //    byte[] serializedMesh = MeshSerializer.WriteMesh(mesh, false);
        //    Debug.Log("SerializedMesh has " + serializedMesh.Length + " bytes");


        //    Mesh deserializedMesh = MeshSerializer.ReadMesh(serializedMesh);
        //    GameObject go = new GameObject("Deserialized Mesh");
        //    MeshFilter meshFilter = go.AddComponent<MeshFilter>();
        //    go.AddComponent<MeshRenderer>();

        //    meshFilter.sharedMesh = deserializedMesh;
        //}

        //if (GUILayout.Button("Serialize All Meshes in Scene"))
        //{
        //    SceneMessage sceneMessage = new SceneMessage("bla", SceneManager.GetActiveScene());

        //    Debug.Log(sceneMessage.ToJSON());
        //}

        //if (GUILayout.Button("Serialize All Meshes in Scene"))
        //{
        //    Debug.Log(SceneSerializer.SerializeScene(SceneManager.GetActiveScene()));

        //}

        //if (GUILayout.Button("Print Unity-Types"))
        //{

        //    StringBuilder sb = new StringBuilder();
        //    var unityTypes = typeof(UnityEngine.Object).Assembly.GetTypes().Where(t => typeof(UnityEngine.Object).IsAssignableFrom(t));
        //    foreach (var item in unityTypes)
        //    {
        //        sb.AppendLine(item.ToString());
        //    }
        //    Debug.Log(sb.ToString());
        //}

        //if (GUILayout.Button("JSON test"))
        //{
        //    Scene scene = SceneManager.GetActiveScene();


        //    Debug.Log(scene.ToJSON());
        //    string json = scene.ToJSON();

        //    System.IO.File.WriteAllText(Application.dataPath + @"\scene.txt", json);


        //}
        #region DataContractSerializer
        if (GUILayout.Button("Surrogate Serialization"))
        {

            Scene scene = SceneManager.GetActiveScene();
            DataContractSceneSerialization.SceneSurrogate sceneSurrogate = new DataContractSceneSerialization.SceneSurrogate(scene);

            List<System.Type> knownTypes = new List<System.Type>();
            knownTypes.Add(typeof(DataContractSceneSerialization.Vector2Surrogate));
            knownTypes.Add(typeof(DataContractSceneSerialization.Vector3Surrogate));
            knownTypes.Add(typeof(DataContractSceneSerialization.Vector4Surrogate));
            knownTypes.Add(typeof(DataContractSceneSerialization.ColorSurrogate));
            knownTypes.Add(typeof(DataContractSceneSerialization.QuaternionSurrogate));
            knownTypes.Add(typeof(DataContractSceneSerialization.TransformSurrogate));
            knownTypes.Add(typeof(DataContractSceneSerialization.GameObjectSurrogate));
            knownTypes.Add(typeof(DataContractSceneSerialization.MeshFilterSurrogate));
            knownTypes.Add(typeof(DataContractSceneSerialization.MeshRendererSurrogate));
            knownTypes.Add(typeof(DataContractSceneSerialization.MeshSurrogate));
            knownTypes.Add(typeof(DataContractSceneSerialization.MaterialSurrogate));
            knownTypes.Add(typeof(DataContractSceneSerialization.ComponentSurrogate));
            //knownTypes.Add(typeof(float));
            //knownTypes.Add(typeof(int));
            //knownTypes.Add(typeof(ListOfInt));
            //knownTypes.Add(typeof(ListOfListOfInt));

            DataContractSerializer serializer = new DataContractSerializer(typeof(DataContractSceneSerialization.SceneSurrogate), knownTypes);
            MemoryStream memStream = new MemoryStream();
            //FileStream writer = new FileStream(Application.dataPath + @"\sceneSurrogate.txt", FileMode.Create);

            Stopwatch sw = new Stopwatch();
            sw.Start();
            serializer.WriteObject(memStream, sceneSurrogate);
            byte[] bytes = memStream.GetBuffer();

            //SceneMessage message = new SceneMessage()
            memStream.Close();
            
            sw.Stop();
            UnityEngine.Debug.Log("Serialization using DataContractSerializer took " + sw.ElapsedMilliseconds + "ms");

            sw.Reset();
            sw.Start();
            MemoryStream outMemStream = new MemoryStream(bytes);
            //FileStream file = File.OpenRead(Application.dataPath + @"\sceneSurrogate.txt");
            DataContractSceneSerialization.SceneSurrogate obj = (DataContractSceneSerialization.SceneSurrogate) serializer.ReadObject(outMemStream);
            outMemStream.Close();
            sw.Stop();
            long millis = sw.ElapsedMilliseconds;
            UnityEngine.Debug.Log("Deserialization using DataContractSerializer took " + millis + "ms");

            sw.Reset();
            sw.Start();
            obj.Get();
            sw.Stop();
            long millisRecreation = sw.ElapsedMilliseconds;
            UnityEngine.Debug.Log("Recreation of Scene took" + millisRecreation + "ms");
            UnityEngine.Debug.Log("Deserialisation/Recreation of Scene took" + (millisRecreation + millis) + "ms");
            UnityEngine.Debug.Log("###############################");

        }
        #endregion DataContractSerializer

        #region Protobuf Serializer
        if (GUILayout.Button("Protobuf Serialization"))
        {

            Scene scene = SceneManager.GetActiveScene();
            ProtobufSceneSerialization.SceneSurrogate sceneSurrogate = new ProtobufSceneSerialization.SceneSurrogate(scene);

            List<System.Type> knownTypes = new List<System.Type>();
            knownTypes.Add(typeof(ProtobufSceneSerialization.Vector2Surrogate));
            knownTypes.Add(typeof(ProtobufSceneSerialization.Vector3Surrogate));
            knownTypes.Add(typeof(ProtobufSceneSerialization.Vector4Surrogate));
            knownTypes.Add(typeof(ProtobufSceneSerialization.ColorSurrogate));
            knownTypes.Add(typeof(ProtobufSceneSerialization.QuaternionSurrogate));
            knownTypes.Add(typeof(ProtobufSceneSerialization.TransformSurrogate));
            knownTypes.Add(typeof(ProtobufSceneSerialization.GameObjectSurrogate));
            knownTypes.Add(typeof(ProtobufSceneSerialization.MeshFilterSurrogate));
            knownTypes.Add(typeof(ProtobufSceneSerialization.MeshRendererSurrogate));
            knownTypes.Add(typeof(ProtobufSceneSerialization.MeshSurrogate));
            knownTypes.Add(typeof(ProtobufSceneSerialization.MaterialSurrogate));
            knownTypes.Add(typeof(ProtobufSceneSerialization.ComponentSurrogate));

            

            Stopwatch sw = new Stopwatch();
            sw.Start();

            MemoryStream memStream = new MemoryStream();
            Serializer.Serialize<ProtobufSceneSerialization.SceneSurrogate>(memStream, sceneSurrogate);
            //byte[] bytes = memStream.GetBuffer();

            byte[] bytes = memStream.ToArray();
            //Debug.Log("memStream.Length=" + memStream.Length + "  -->  bytes.Length=" + bytes.Length + "  -->  bytes2.Length=" + bytes2.Length);
            memStream.Close();
            sw.Stop();
            UnityEngine.Debug.Log("Serialization using Protobuf-Serialization took " + sw.ElapsedMilliseconds + "ms");

            sw.Reset();
            sw.Start();
          
            MemoryStream outMemStream = new MemoryStream(bytes,0, bytes.Length);
            ProtobufSceneSerialization.SceneSurrogate obj = 
                (ProtobufSceneSerialization.SceneSurrogate) Serializer.Deserialize<ProtobufSceneSerialization.SceneSurrogate>(outMemStream);
            outMemStream.Close();
            sw.Stop();
            long millis = sw.ElapsedMilliseconds;
            UnityEngine.Debug.Log("Deserialization using Protobuf-Serialization took " + millis + "ms");

            sw.Reset();
            sw.Start();
            obj.Get();
            sw.Stop();
            long millisRecreation = sw.ElapsedMilliseconds;
            UnityEngine.Debug.Log("Recreation of Scene took" + millisRecreation + "ms");
            UnityEngine.Debug.Log("Deserialisation/Recreation of Scene took" + (millisRecreation + millis) + "ms");
        }
        #endregion Protobuf Serializer
    }
}
