using System;

using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public class SimpleMessage
{
    internal string name;
    internal Scene scene;

    public static string Serialize(SimpleMessage message)
    {
        return JsonUtility.ToJson(message);
    }

    public static SimpleMessage Deserialize(string jsonString)
    {
        return JsonUtility.FromJson<SimpleMessage>(jsonString);
    }
}

