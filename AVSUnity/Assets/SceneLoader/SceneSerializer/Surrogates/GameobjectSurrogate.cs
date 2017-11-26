using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public class GameobjectSurrogate : ISerializationSurrogate
{
    private KeyValuePairListsDictionary<string, UnityEngine.Object> serializedObjects;


    public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
    {
        throw new NotImplementedException();
    }

    public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
    {
        throw new NotImplementedException();
    }
}
