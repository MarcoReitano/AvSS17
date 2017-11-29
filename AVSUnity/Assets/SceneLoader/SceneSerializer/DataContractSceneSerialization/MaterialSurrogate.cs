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
    public class MaterialSurrogate
    {
        [DataMember(Name = "ShaderName")]
        public string shader;

        // TODO: Support Texture Serialization
        //[DataMember(Name = "Texture")]
        //public Texture2D texture;



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
