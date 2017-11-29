using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ProtoBuf;
using UnityEngine;

namespace ProtobufSceneSerialization
{
    [ProtoContract]
    public class TransformSurrogate : ComponentSurrogate
    {
        [ProtoMember(1)]
        public Vector3Surrogate position;

        [ProtoMember(2)]
        public QuaternionSurrogate rotation;

        [ProtoMember(3)]
        public Vector3Surrogate scale;

        public TransformSurrogate()
        {

        }

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

