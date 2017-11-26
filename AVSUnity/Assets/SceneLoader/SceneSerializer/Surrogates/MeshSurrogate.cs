using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public class MeshSurrogate : ISerializationSurrogate
{
    public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
    {
        Vector3 vec = (Vector3)obj;
        info.AddValue("x", vec.x);
        info.AddValue("y", vec.y);
        info.AddValue("z", vec.z);
    }

    public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
    {
        Func<string, float> get = name => (float) info.GetValue(name, typeof(float));
        return new Vector3(get("x"), get("y"), get("z"));
    }
}
