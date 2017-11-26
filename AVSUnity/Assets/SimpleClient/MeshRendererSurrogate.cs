using System;
using System.Runtime.Serialization;

internal class MeshRendererSurrogate : ISerializationSurrogate
{
    public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
    {
        throw new NotImplementedException();
    }

    public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
    {
        throw new NotImplementedException();
    }
}