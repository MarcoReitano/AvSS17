using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEditor.SceneManagement;

using UnityEngine;
using UnityEngine.SceneManagement;

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
        return JsonUtility.ToJson(this);
    }

    public static SceneMessage FromJson(string json)
    {
        SceneMessage message = JsonUtility.FromJson<SceneMessage>(json);
        message.scene = ByteArrayToScene(message.messageText, message.sceneBytes);
        return message;
    }

    public static byte[] SceneFileToByteArray(Scene scene)
    {
        Debug.Log("datapath: " + Application.dataPath);
        Debug.Log("scene.path: " + scene.path);
        return File.ReadAllBytes(Application.dataPath + "/" + scene.path.Substring("/Assets".Length));
    }


    public static Scene ByteArrayToScene(string filename, byte[] bytes)
    {
        ByteArrayToSceneFile(filename, bytes);
        return EditorSceneManager.OpenScene(Application.dataPath + "/" + filename, OpenSceneMode.Additive);
    }

    public static void ByteArrayToSceneFile(string filename, byte[] bytes)
    {
        //File.WriteAllBytes(Application.dataPath + "/" + filename.Substring("/Assets".Length), bytes);
        File.WriteAllBytes(Application.dataPath + "/" + filename, bytes);
    }

}