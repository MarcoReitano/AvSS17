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
public class QuaternionSurrogate
{
    [DataMember(Name = "x")]
    public float x;

    [DataMember(Name = "y")]
    public float y;

    [DataMember(Name = "z")]
    public float z;

    [DataMember(Name = "w")]
    public float w;

    public QuaternionSurrogate(Quaternion quat)
    {
        this.x = quat.x;
        this.y = quat.y;
        this.z = quat.z;
        this.w = quat.w;
    }

    public Quaternion Get()
    {
        return new Quaternion(this.x, this.y, this.z, this.w);
    }
}
