using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OSMJobMessage
{
    public int x;

    public int y;

    public double tileWidth;
    public double originLongitude;
    public double originLatitude;

    public string replyToQueue;
    public long timeStamp;
    public SerializationMethod method;

    public OSMJobMessage(int x, int y, double tileWidth, double originLongitude, double originLatitude, string replyQueueName, long timeStamp, SerializationMethod method)
    {
        this.x = x;
        this.y = y;
        this.tileWidth = tileWidth;
        this.originLongitude = originLongitude;
        this.originLatitude = originLatitude;
        this.replyToQueue = replyQueueName;
        this.timeStamp = timeStamp;
        this.method = method;
    }

    public string ToJson()
    {
        return JsonUtility.ToJson(this);
    }

    public static OSMJobMessage FromByteArray(byte[] bytes)
    {
        var payload = System.Text.Encoding.UTF8.GetString(bytes);
        return FromJson(payload);
    }

    public static OSMJobMessage FromJson(string json)
    {
        return JsonUtility.FromJson<OSMJobMessage>(json);
    }
}
