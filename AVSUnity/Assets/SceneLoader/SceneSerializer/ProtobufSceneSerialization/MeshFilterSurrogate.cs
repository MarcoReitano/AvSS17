using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ProtoBuf;
using UnityEngine;

namespace ProtobufSceneSerialization
{
    [ProtoContract]
    public class MeshFilterSurrogate : ComponentSurrogate
    {

        [ProtoMember(1)]
        public MeshSurrogate mesh;

        [ProtoMember(2)]
        public bool hasMesh;

        public MeshFilterSurrogate()
        {

        }

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