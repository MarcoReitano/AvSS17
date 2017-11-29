using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ProtoBuf;
using UnityEngine;

namespace ProtobufSceneSerialization
{
    [ProtoContract]
    public class MeshRendererSurrogate : ComponentSurrogate
    {
        [ProtoMember(1)]
        public List<MaterialSurrogate> materials;

        public MeshRendererSurrogate()
        {

        }

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