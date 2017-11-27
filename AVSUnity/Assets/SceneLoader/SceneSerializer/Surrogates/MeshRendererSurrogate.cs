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
    public class MeshRendererSurrogate : ComponentSurrogate
    {
        [DataMember(Name = "Materials")]
        public List<MaterialSurrogate> materials;

        public MeshRendererSurrogate(MeshRenderer meshRenderer)
        {
            this.materials = new List<MaterialSurrogate>();

            foreach (Material material in meshRenderer.sharedMaterials)
            {
                MaterialSurrogate matSurrogate = new MaterialSurrogate(material);
                materials.Add(matSurrogate);
            }
        }

        public Material[] GetMaterials()
        {
            List<Material> mats = new List<Material>();
            foreach (MaterialSurrogate matSurrogate in materials)
            {
                Material mat = matSurrogate.Get();
                mats.Add(mat);
            }

            return mats.ToArray();
        }
    }
}