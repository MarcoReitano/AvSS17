using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
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
        //this.sceneBytes = SceneFileToByteArray(this.scene);
    }

    public string ToJSON()
    {
        // Alle Objekte der Szene traversieren
        // Prüfen ob Mesh vorhanden
        // Mesh serialisieren --> wie?
        //     Mesh serializable?
        //     ansonsten --> Vertex-Array / Triangle-Array(s) / UVs
        //     Mesh und/oder Scene WrapperKlasse die Serializable ist

        foreach (GameObject item in scene.GetRootGameObjects())
        {
            foreach (Transform go in item.GetComponentsInChildren<Transform>())
            {
                MeshFilter mf = go.GetComponent<MeshFilter>();
                if (mf != null)
                {
                    Mesh mesh = mf.sharedMesh;

                    byte[] serializedMesh = MeshSerializer.WriteMesh(mesh, false);

                    Debug.Log(go.name + ": SerializedMesh has " + serializedMesh.Length + " bytes");
                }
            }
        }
        return "It works!";

        //return JsonUtility.ToJson(this);
    }

    public static SceneMessage FromJson(string json)
    {
        SceneMessage message = JsonUtility.FromJson<SceneMessage>(json);
        message.scene = ByteArrayToScene(message.messageText, message.sceneBytes);
        return message;
    }

    public static byte[] SceneFileToByteArray(Scene scene)
    {
        //Didnt work...
        ////Debug.Log("datapath: " + Application.dataPath);
        ////Debug.Log("scene.path: " + scene.path);
        //BinaryFormatter bf = new BinaryFormatter();
        ////FileStream file = new FileStream("c:\\test.scene", FileMode.Create);
        //MemoryStream mem = new MemoryStream();
        //bf.Serialize(mem, scene);
        //byte[] bytes = mem.GetBuffer();
        //return bytes;
        ////return File.ReadAllBytes(Application.dataPath + "/" + scene.path.Substring("/Assets".Length));
        return null;
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