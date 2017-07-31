using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JobMessage
{

    public int x;

    public int y;

    public string replyToQueue;

    public JobMessage(int x, int y, string replyToQueue)
    {
        this.x = x;
        this.y = y;
        this.replyToQueue = replyToQueue;
    }

    public string ToJson()
    {
        return JsonUtility.ToJson(this);
    }

    public static JobMessage FromByteArray(byte[] bytes)
    {
        var payload = System.Text.Encoding.UTF8.GetString(bytes);
        return FromJson(payload);
    }

    public static JobMessage FromJson(string json)
    {
        return JsonUtility.FromJson<JobMessage>(json);
    }
}
