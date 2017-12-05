using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;

using UnityEngine;
using UnityEngine.SceneManagement;
using ProtoBuf;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif
public enum SerializationMethod
{
    DataContractSerializer,
    ProtoBuf
}


public class SceneMessage
{
    public int Job_ID;

    public string messageText;

    private Scene scene;

    public byte[] sceneBytes;

    public TimeStamp timeStamp;

    public SerializationMethod method = SerializationMethod.DataContractSerializer;

    public SceneMessage(int Job_ID, string messageText, Scene scene, TimeStamp timeStamp, SerializationMethod method)
    {
        this.Job_ID = Job_ID;
        this.method = method;
        this.messageText = messageText;
        this.scene = scene;
        this.sceneBytes = SceneFileToByteArray(this.scene, method);
        this.timeStamp = timeStamp;
    }

    public string ToJSON()
    {
        return JsonUtility.ToJson(this);
    }

    public static SceneMessage FromJson(string json)
    {
        SceneMessage message = JsonUtility.FromJson<SceneMessage>(json);
        // TODO: Change this. its not really loading a Scene, rather loading only the root-GameObjects
        message.scene = ByteArrayToScene(message.sceneBytes, message.method);
        return message;
    }

    public static byte[] SceneFileToByteArray(Scene scene, SerializationMethod method)
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();

        MemoryStream memStream = new MemoryStream();
        byte[] sceneBytes = null;

        switch (method)
        {
            case SerializationMethod.DataContractSerializer:
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

                serializer.WriteObject(memStream, sceneSurrogate);
                sceneBytes = memStream.GetBuffer();
                break;
            case SerializationMethod.ProtoBuf:
                ProtobufSceneSerialization.SceneSurrogate sceneSurrogateProto = new ProtobufSceneSerialization.SceneSurrogate(scene);
                Serializer.Serialize<ProtobufSceneSerialization.SceneSurrogate>(memStream, sceneSurrogateProto);
                sceneBytes = memStream.ToArray();
                break;
            default:
                break;
        }

        memStream.Close();
        sw.Stop();
        UnityEngine.Debug.Log("Serialization using " + method.ToString() + " took " + sw.ElapsedMilliseconds + "ms");
        return sceneBytes;
    }


    public static Scene ByteArrayToScene(byte[] bytes, SerializationMethod method)
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();
        MemoryStream memStream = new MemoryStream(bytes);
        long millis = 0;
        long millisRecreation = 0;
        Scene scene;
        switch (method)
        {
            case SerializationMethod.DataContractSerializer:
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

                DataContractSceneSerialization.SceneSurrogate obj = (DataContractSceneSerialization.SceneSurrogate)serializer.ReadObject(memStream);

                sw.Stop();
                millis = sw.ElapsedMilliseconds;
                UnityEngine.Debug.Log("Deserialization using DataContractSerializer took " + millis + "ms");
                sw.Reset();
                sw.Start();
                scene = obj.Get();
                sw.Stop();
                millisRecreation = sw.ElapsedMilliseconds;
                UnityEngine.Debug.Log("Recreation of Scene took" + millisRecreation + "ms");
                UnityEngine.Debug.Log("Deserialisation/Recreation of Scene using " + method.ToString() + " took" + (millisRecreation + millis) + "ms");
                return scene;
            case SerializationMethod.ProtoBuf:
                MemoryStream outMemStream = new MemoryStream(bytes, 0, bytes.Length);
                ProtobufSceneSerialization.SceneSurrogate objProto =
                    (ProtobufSceneSerialization.SceneSurrogate)Serializer.Deserialize<ProtobufSceneSerialization.SceneSurrogate>(outMemStream);
                outMemStream.Close();

                millis = sw.ElapsedMilliseconds;
                UnityEngine.Debug.Log("Deserialization using Protobuf-Serialization took " + millis + "ms");
                sw.Reset();
                sw.Start();
                scene = objProto.Get();
                sw.Stop();
                millisRecreation = sw.ElapsedMilliseconds;
                UnityEngine.Debug.Log("Recreation of Scene took" + millisRecreation + "ms");
                UnityEngine.Debug.Log("Deserialisation/Recreation of Scene using " + method.ToString() + " took" + (millisRecreation + millis) + "ms");
                return scene;
            default:
                break;
        }
        return new Scene();
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