using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ProtoBuf;
using UnityEngine;

namespace ProtobufSceneSerialization
{
    [ProtoContract]
    public class Vector3Surrogate
    {
        [ProtoMember(1)]
        public float x;

        [ProtoMember(2)]
        public float y;

        [ProtoMember(3)]
        public float z;

        public Vector3Surrogate()
        {

        }

        public Vector3Surrogate(Vector3 vec)
        {
            this.x = vec.x;
            this.y = vec.y;
            this.z = vec.z;
        }

        public Vector3 Get()
        {
            return new Vector3(this.x, this.y, this.z);
        }
    }
}