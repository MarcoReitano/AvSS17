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
public class MeshFilterSurrogate : ComponentSurrogate
{

    [DataMember(Name = "Mesh")]
    public MeshSurrogate mesh;

    public MeshFilterSurrogate(MeshFilter meshFilter)
    {
        this.mesh = new MeshSurrogate(meshFilter.sharedMesh);
    }

    //public MeshFilter Get()
    //{

    //}

}
