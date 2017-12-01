
using System.Runtime.Serialization;
using ProtoBuf;

namespace ProtobufSceneSerialization
{
    [ProtoContract]
    [ProtoInclude(1, typeof(TransformSurrogate))]
    [ProtoInclude(2, typeof(MeshFilterSurrogate))]
    [ProtoInclude(3, typeof(MeshRendererSurrogate))]
    [ProtoInclude(4, typeof(TerrainSurrogate))]
    public class ComponentSurrogate
    {
        public ComponentSurrogate()
        {

        }

    }
}