using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

[DataContract]
[KnownType(typeof(TransformSurrogate))]
[KnownType(typeof(ComponentSurrogate))]
[KnownType(typeof(MeshRendererSurrogate))]
[KnownType(typeof(MeshFilterSurrogate))]
[KnownType(typeof(Vector2Surrogate))]
[KnownType(typeof(Vector3Surrogate))]
[KnownType(typeof(Vector4Surrogate))]
[KnownType(typeof(QuaternionSurrogate))]
[KnownType(typeof(MaterialSurrogate))]
[KnownType(typeof(MeshSurrogate))]
[KnownType(typeof(SceneSurrogate))]
[KnownType(typeof(GameObjectSurrogate))]
[KnownType(typeof(SerializableList))]
[Serializable]
public class SerializableList
{
    [DataMember(Name = "List")]
    public List<int> list;

    public SerializableList()
    {
        list = new List<int>();
    }
}
