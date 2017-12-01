using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ProtoBuf;
using UnityEngine;

namespace ProtobufSceneSerialization
{
    [ProtoContract]
    public class MeshSurrogate
    {
        [ProtoMember(1)]
        public string name;

        [ProtoMember(2)]
        public List<Vector3Surrogate> vertices = new List<Vector3Surrogate>();

        [ProtoMember(3)]
        public List<ColorSurrogate> colors = new List<ColorSurrogate>();

        [ProtoMember(4)]
        public List<Vector4Surrogate> tangents = new List<Vector4Surrogate>();

        [ProtoMember(5)]
        public List<Vector3Surrogate> normals = new List<Vector3Surrogate>();

        [ProtoMember(6)]
        public List<Vector2Surrogate> uvs = new List<Vector2Surrogate>();

        [ProtoMember(7)]
        public List<Vector2Surrogate> uv2 = new List<Vector2Surrogate>();

        [ProtoMember(8)]
        public List<Vector2Surrogate> uv3 = new List<Vector2Surrogate>();

        [ProtoMember(9)]
        public List<Vector2Surrogate> uv4 = new List<Vector2Surrogate>();

        [ProtoMember(10)]
        public int subMeshCount;

        [ProtoMember(11)]
        public List<int> subMeshes = new List<int>();

        [ProtoMember(12)]
        public List<int> trianglesInSubMesh = new List<int>();

        public MeshSurrogate()
        {

        }

        public MeshSurrogate(Mesh mesh)
        {
            if (mesh == null)
                return;

            this.name = mesh.name;


            vertices = new List<Vector3Surrogate>();
            foreach (Vector3 vertex in mesh.vertices)
            {
                vertices.Add(new Vector3Surrogate(vertex));
            }

            colors = new List<ColorSurrogate>();
            foreach (Color color in mesh.colors)
            {
                colors.Add(new ColorSurrogate(color));
            }

            tangents = new List<Vector4Surrogate>();
            foreach (Vector4 tangent in mesh.tangents)
            {
                tangents.Add(new Vector4Surrogate(tangent));
            }

            normals = new List<Vector3Surrogate>();
            foreach (Vector3 normal in mesh.normals)
            {
                normals.Add(new Vector3Surrogate(normal));
            }

            uvs = new List<Vector2Surrogate>();
            foreach (Vector2 uv in mesh.uv)
            {
                uvs.Add(new Vector2Surrogate(uv));
            }

            uv2 = new List<Vector2Surrogate>();
            foreach (Vector2 uv in mesh.uv2)
            {
                uv2.Add(new Vector2Surrogate(uv));
            }

            uv3 = new List<Vector2Surrogate>();
            foreach (Vector2 uv in mesh.uv3)
            {
                uv3.Add(new Vector2Surrogate(uv));
            }

            uv4 = new List<Vector2Surrogate>();
            foreach (Vector2 uv in mesh.uv4)
            {
                uv4.Add(new Vector2Surrogate(uv));
            }

            this.subMeshCount = mesh.subMeshCount;

            subMeshes = new List<int>();
            trianglesInSubMesh = new List<int>();
            for (int i = 0; i < mesh.subMeshCount; i++)
            {
                int[] triangles = mesh.GetTriangles(i);
                trianglesInSubMesh.Add(triangles.Length);
                subMeshes.AddRange(triangles);
            }

        }

        public Mesh Get()
        {
            Mesh mesh = new Mesh();

            mesh.name = this.name;

            List<Vector3> verts = new List<Vector3>();
            foreach (Vector3Surrogate vertex in this.vertices)
            {
                verts.Add(vertex.Get());
            }
            mesh.vertices = verts.ToArray();

            List<Color> cols = new List<Color>();
            foreach (ColorSurrogate color in this.colors)
            {
                cols.Add(color.Get());
            }
            mesh.colors = cols.ToArray();

            List<Vector4> tan = new List<Vector4>();
            foreach (Vector4Surrogate tangent in this.tangents)
            {
                tan.Add(tangent.Get());
            }
            mesh.tangents = tan.ToArray();

            List<Vector3> normalen = new List<Vector3>();
            foreach (Vector3Surrogate normal in this.normals)
            {
                normalen.Add(normal.Get());
            }
            mesh.normals = normalen.ToArray();

            List<Vector2> realUV = new List<Vector2>();
            foreach (Vector2Surrogate item in this.uvs)
            {
                realUV.Add(item.Get());
            }
            mesh.uv = realUV.ToArray();

            List<Vector2> realUV2 = new List<Vector2>();
            foreach (Vector2Surrogate item in this.uv2)
            {
                realUV2.Add(item.Get());
            }
            mesh.uv2 = realUV2.ToArray();

            List<Vector2> realUV3 = new List<Vector2>();
            foreach (Vector2Surrogate item in this.uv3)
            {
                realUV3.Add(item.Get());
            }
            mesh.uv3 = realUV3.ToArray();

            List<Vector2> realUV4 = new List<Vector2>();
            foreach (Vector2Surrogate item in this.uv4)
            {
                realUV4.Add(item.Get());
            }
            mesh.uv4 = realUV4.ToArray();

            mesh.subMeshCount = this.subMeshCount;

            int offset = 0;
            for (int i = 0; i < subMeshCount; i++)
            {
                int numberOfTriangles = trianglesInSubMesh[i];

                List<int> tris = new List<int>();
                for (int j = offset; j < offset + numberOfTriangles; j++)
                {
                    tris.Add(subMeshes[j]);
                }
                offset += numberOfTriangles;
                mesh.SetTriangles(tris, i);
            }

            return mesh;
        }
    }
}

//[CollectionDataContract]
//public class ListOfInt : List<int>
//{
//}

//[CollectionDataContract]
//public class ListOfListOfInt : List<ListOfInt>
//{
//}