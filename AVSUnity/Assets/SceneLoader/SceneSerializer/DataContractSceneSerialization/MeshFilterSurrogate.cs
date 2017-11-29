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
    public class MeshFilterSurrogate : ComponentSurrogate
    {

        [DataMember(Name = "Mesh")]
        public MeshSurrogate mesh;

        [DataMember(Name = "HasMesh")]
        public bool hasMesh;

        public MeshFilterSurrogate(MeshFilter meshFilter)
        {
            if (meshFilter.sharedMesh != null)
            {
                this.hasMesh = true;
                this.mesh = new MeshSurrogate(meshFilter.sharedMesh);
            }
            else
            {
                this.hasMesh = false;
            }
        }

        //public MeshFilter Get()
        //{

        //}

    }
}