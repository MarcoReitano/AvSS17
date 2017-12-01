using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ProtoBuf;
using UnityEngine;

namespace ProtobufSceneSerialization
{
    [ProtoContract]
    public class Vector2Surrogate
    {
        [ProtoMember(1)]
        public float x;

        [ProtoMember(2)]
        public float y;

        public Vector2Surrogate()
        {

        }

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