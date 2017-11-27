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
    public class Vector2Surrogate
    {
        [DataMember(Name = "x")]
        public float x;

        [DataMember(Name = "y")]
        public float y;

        public Vector2Surrogate(Vector2 vec)
        {
            this.x = vec.x;
            this.y = vec.y;
        }

        public Vector2 Get()
        {
            return new Vector2(this.x, this.y);
        }
    }
}