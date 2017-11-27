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
    public class TransformSurrogate : ComponentSurrogate
    {
        [DataMember(Name = "Position")]
        public Vector3Surrogate position;

        [DataMember(Name = "Rotation")]
        public QuaternionSurrogate rotation;

        [DataMember(Name = "Scale")]
        public Vector3Surrogate scale;

        public TransformSurrogate(Transform transform)
        {
            this.position = new Vector3Surrogate(transform.position);
            this.rotation = new QuaternionSurrogate(transform.rotation);
            this.scale = new Vector3Surrogate(transform.localScale);
        }

        public void Get(ref Transform transform)
        {
            transform.position = this.position.Get();
            transform.rotation = this.rotation.Get();
            transform.localScale = this.scale.Get();
        }
    }
}

