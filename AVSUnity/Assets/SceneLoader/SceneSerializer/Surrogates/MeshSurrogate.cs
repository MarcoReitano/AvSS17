using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

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
[KnownType(typeof(ListOfInt))]
[KnownType(typeof(ListOfListOfInt))]
public class MeshSurrogate
{
    [DataMember(Name = "Name")]
    public string name;

    [DataMember(Name = "Vertices")]
    public List<Vector3Surrogate> vertices;

    [DataMember(Name = "Colors")]
    public List<ColorSurrogate> colors;

    [DataMember(Name = "Tangents")]
    public List<Vector4Surrogate> tangents;

    [DataMember(Name = "Normals")]
    public List<Vector3Surrogate> normals;

    [DataMember(Name = "UVs")]
    public List<Vector2Surrogate> uvs;

    [DataMember(Name = "UV2")]
    public List<Vector2Surrogate> uv2;

    [DataMember(Name = "UV3")]
    public List<Vector2Surrogate> uv3;

    [DataMember(Name = "UV4")]
    public List<Vector2Surrogate> uv4;

    [DataMember(Name = "SubmeshCount")]
    public int subMeshCount;

    [DataMember(Name = "SubMeshes")]
    public List<int> subMeshes;

    [DataMember(Name = "TrianglesInSubmesh")]
    public List<int> trianglesInSubMesh;


    public MeshSurrogate(Mesh mesh)
    {
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

[CollectionDataContract]
public class ListOfInt : List<int>
{
}

[CollectionDataContract]
public class ListOfListOfInt : List<ListOfInt>
{
}