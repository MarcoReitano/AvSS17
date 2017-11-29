using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;

using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

public class SceneMessage
{
    public string messageText;

    private Scene scene;

    public byte[] sceneBytes;

    public long timeStamp;

    public SceneMessage(string messageText, Scene scene, long timeStamp)
    {
        this.messageText = messageText;
        this.scene = scene;
        this.sceneBytes = SceneFileToByteArray(this.scene);
        this.timeStamp = timeStamp;
    }

    public string ToJSON()
    {
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

    public static string CheckForExistingScene(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene checkScene = SceneManager.GetSceneAt(i);
            if (checkScene.name == sceneName)
            {
                sceneName += "_Dublicate";
                return CheckForExistingScene(sceneName);
            }
        }

        return sceneName;
    }

#if UNITY_EDITOR
    public static string CheckForExistingSceneEditor(string sceneName)
    {
        for (int i = 0; i < EditorSceneManager.sceneCount; i++)
        {
            Scene checkScene = EditorSceneManager.GetSceneAt(i);
            if (checkScene.name == sceneName)
            {
                sceneName += "_Dublicate";
                return CheckForExistingScene(sceneName);
            }
        }

        return sceneName;
    }
#endif
}