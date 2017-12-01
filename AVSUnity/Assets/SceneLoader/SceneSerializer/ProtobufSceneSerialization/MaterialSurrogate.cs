using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ProtoBuf;
using UnityEngine;

namespace ProtobufSceneSerialization
{
    [ProtoContract]
    public class MaterialSurrogate
    {
        [ProtoMember(1)]
        public string shader;

        // TODO: Support Texture Serialization
        //[DataMember(Name = "Texture")]
        //public Texture2D texture;

        public MaterialSurrogate()
        {

        }

        public MaterialSurrogate(Material material)
        {
            this.shader = material.shader.name;


            //Texture2D texture = material.mainTexture as Texture2D;
            //byte[] rawTextureData = texture.GetRawTextureData();

            //Texture2D tex = new Texture()
        }


        public Material Get()
        {
            return new Material(Shader.Find(shader));
        }
    }
}
