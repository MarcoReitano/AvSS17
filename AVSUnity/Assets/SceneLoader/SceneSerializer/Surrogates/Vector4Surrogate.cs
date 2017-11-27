using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

namespace DataContractSceneSerialization
{
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
    public class Vector4Surrogate
    {
        [DataMember(Name = "x")]
        public float x;

        [DataMember(Name = "y")]
        public float y;

        [DataMember(Name = "z")]
        public float z;

        [DataMember(Name = "w")]
        public float w;

        public Vector4Surrogate(Vector4 vec)
        {
            this.x = vec.x;
            this.y = vec.y;
            this.z = vec.z;
            this.w = vec.w;
        }

        public Vector4 Get()
        {
            return new Vector4(this.x, this.y, this.z, this.w);
        }
    }
}