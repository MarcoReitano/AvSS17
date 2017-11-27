using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor.SceneManagement;

using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

public class SceneMessage
{
    public string messageText;

    private Scene scene;

    public byte[] sceneBytes;

    public SceneMessage(string messageText, Scene scene)
    {
        this.messageText = messageText;
        this.scene = scene;
        this.sceneBytes = SceneFileToByteArray(this.scene);
    }

    public string ToJSON()
    {
        //BinaryFormatter serializer = new BinaryFormatter();
        //SurrogateSelector selector = new SurrogateSelector();
        //selector.AddSurrogate(typeof(Vector3), new StreamingContext(), new Vector3Surrogate());
        //selector.AddSurrogate(typeof(Mesh), new StreamingContext(), new MeshSurrogate());
        //selector.AddSurrogate(typeof(MeshRenderer), new StreamingContext(), new MeshRendererSurrogate());
        //selector.AddSurrogate(typeof(UnityEngine.Object), new StreamingContext(), new UnityObjectSurrogate());

        //return string.Empty;

        // Mesh serialisieren --> wie?
        //     Mesh serializable?
        //     ansonsten --> Vertex-Array / Triangle-Array(s) / UVs
        //     Mesh und/oder Scene WrapperKlasse die Serializable ist
        // Alle Objekte auf root-Ebene der Szene traversieren
        //foreach (GameObject item in scene.GetRootGameObjects())
        //{
        //    // Prüfen ob Mesh vorhanden
        //    foreach (Transform go in item.GetComponentsInChildren<Transform>())
        //    {
        //        MeshFilter mf = go.GetComponent<MeshFilter>();
        //        if (mf != null)
        //        {
        //            Mesh mesh = mf.sharedMesh;

        //            byte[] serializedMesh = MeshSerializer.WriteMesh(mesh, false);

        //            Debug.Log(go.name + ": SerializedMesh has " + serializedMesh.Length + " bytes");





        //        }
        //    }
        //}
        //return "It works!";
        //Scene scene = SceneManager.GetActiveScene();
       
        return JsonUtility.ToJson(this);
    }

    public static SceneMessage FromJson(string json)
    {
        SceneMessage message = JsonUtility.FromJson<SceneMessage>(json);
        message.scene = ByteArrayToScene(message.sceneBytes);
        return message;
    }

    public static byte[] SceneFileToByteArray(Scene scene)
    {
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
        byte[] sceneBytes = memStream.GetBuffer();

        memStream.Close();

        sw.Stop();
        UnityEngine.Debug.Log("Serialization using DataContractSerializer took " + sw.ElapsedMilliseconds + "ms");
        return sceneBytes;
    }


    public static Scene ByteArrayToScene(byte[] bytes)
    {
        //ByteArrayToSceneFile(filename, bytes);
        //return EditorSceneManager.OpenScene(Application.dataPath + "/" + filename, OpenSceneMode.Additive);

        Stopwatch sw = new Stopwatch();
 
        sw.Start();


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
        MemoryStream memStream = new MemoryStream(bytes);

        DataContractSceneSerialization.SceneSurrogate obj = (DataContractSceneSerialization.SceneSurrogate)serializer.ReadObject(memStream);
        memStream.Close();
        sw.Stop();
        long millis = sw.ElapsedMilliseconds;
        UnityEngine.Debug.Log("Deserialization using DataContractSerializer took " + millis + "ms");

        sw.Reset();
        sw.Start();
        Scene scene = obj.Get();
        sw.Stop();
        long millisRecreation = sw.ElapsedMilliseconds;
        UnityEngine.Debug.Log("Recreation of Scene took" + millisRecreation + "ms");
        UnityEngine.Debug.Log("Deserialisation/Recreation of Scene took" + (millisRecreation + millis) + "ms");

        return scene;
    }

    //public static void ByteArrayToSceneFile(string filename, byte[] bytes)
    //{
    //    //File.WriteAllBytes(Application.dataPath + "/" + filename.Substring("/Assets".Length), bytes);
    //    File.WriteAllBytes(Application.dataPath + "/" + filename, bytes);
    //}

}