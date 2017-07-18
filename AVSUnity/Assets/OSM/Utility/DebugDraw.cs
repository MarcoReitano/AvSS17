﻿// Remi Gillig - http://speps.fr - 2012 - Public domain

using System;
using System.Collections.Generic;
using UnityEngine;

public class DebugDraw
{
    static Material material = new Material(
    @"Shader ""Custom/DebugDraw"" {
        Properties {
            _Color (""Main Color"", Color) = (1,1,1,1)
        }
        SubShader {
            Pass {
                Color [_Color]
            }
        }
    }");


    static MeshCreator creator = new MeshCreator();
    static Mesh solidSphere;
    static Mesh solidCube;
    static Mesh solidDisc;
    static Mesh solidQuad;

    static DebugDraw()
    {
        solidSphere = creator.CreateSphere(3);
        solidCube = creator.CreateCube(0);
        //solidDisc = creator.CreateDisc();
        solidQuad = creator.CreateQuad();
    }

    public static void DrawSphere(Vector3 position, float radius, Color color)
    {
        Matrix4x4 mat = Matrix4x4.TRS(position, Quaternion.identity, radius * 0.5f * Vector3.one);
        MaterialPropertyBlock block = new MaterialPropertyBlock();
        block.SetColor("_Color", color);
        Graphics.DrawMesh(solidSphere, mat, material, 0, null, 0, block);
    }

    public static void DrawCube(Vector3 position, Quaternion rotation, float size, Color color)
    {
        Matrix4x4 mat = Matrix4x4.TRS(position, rotation, size * Vector3.one);
        MaterialPropertyBlock block = new MaterialPropertyBlock();
        block.SetColor("_Color", color);
        Graphics.DrawMesh(solidCube, mat, material, 0, null, 0, block);
    }

    public static void DrawDisc(Vector3 position, Quaternion rotation, float radius, Color color)
    {
        Matrix4x4 mat = Matrix4x4.TRS(position, rotation, radius * Vector3.one);
        MaterialPropertyBlock block = new MaterialPropertyBlock();
        block.SetColor("_Color", color);
        Graphics.DrawMesh(solidDisc, mat, material, 0, null, 0, block);
    }

    public static void DrawQuad(Vector3 position, Quaternion rotation, float width, Color color)
    {
        Matrix4x4 mat = Matrix4x4.TRS(position, rotation, width * Vector3.one);
        MaterialPropertyBlock block = new MaterialPropertyBlock();
        block.SetColor("_Color", color);
        Graphics.DrawMesh(solidQuad, mat, material, 0, null, 0, block);
    }


    #region MeshCreator
    public class MeshCreator
    {
        private List<Vector3> positions;
        private List<Vector2> uvs;
        private int index;
        private Dictionary<Int64, int> middlePointIndexCache;

        // add vertex to mesh, fix position to be on unit sphere, return index
        private int addVertex(Vector3 p, Vector2 uv)
        {
            positions.Add(p);
            uvs.Add(uv);
            return index++;
        }

        // return index of point in the middle of p1 and p2
        private int getMiddlePoint(int p1, int p2)
        {
            // first check if we have it already
            bool firstIsSmaller = p1 < p2;
            Int64 smallerIndex = firstIsSmaller ? p1 : p2;
            Int64 greaterIndex = firstIsSmaller ? p2 : p1;
            Int64 key = (smallerIndex << 32) + greaterIndex;

            int ret;
            if (this.middlePointIndexCache.TryGetValue(key, out ret))
            {
                return ret;
            }

            // not in cache, calculate it
            Vector3 point1 = this.positions[p1];
            Vector3 point2 = this.positions[p2];
            Vector3 middle = new Vector3(
                (point1.x + point2.x) / 2.0f,
                (point1.y + point2.y) / 2.0f,
                (point1.z + point2.z) / 2.0f);

            Vector2 uv1 = this.uvs[p1];
            Vector2 uv2 = this.uvs[p2];
            Vector2 uvmid = new Vector2(
                (uv1.x + uv2.x) / 2.0f,
                (uv1.y + uv2.y) / 2.0f);

            // add vertex makes sure point is on unit sphere
            int i = addVertex(middle, uvmid);

            // store it, return index
            this.middlePointIndexCache.Add(key, i);
            return i;
        }

        public Mesh CreateCube(int subdivisions)
        {
            positions = new List<Vector3>();
            uvs = new List<Vector2>();
            middlePointIndexCache = new Dictionary<long, int>();
            index = 0;

            var indices = new List<int>();

            // front
            addVertex(new Vector3(-1, -1, 1), new Vector2(1, 0));
            addVertex(new Vector3(-1, 1, 1), new Vector2(1, 1));
            addVertex(new Vector3(1, 1, 1), new Vector2(0, 1));
            addVertex(new Vector3(1, -1, 1), new Vector2(0, 0));
            indices.Add(0); indices.Add(3); indices.Add(2);
            indices.Add(2); indices.Add(1); indices.Add(0);

            // right
            addVertex(new Vector3(1, -1, 1), new Vector2(1, 0));
            addVertex(new Vector3(1, 1, 1), new Vector2(1, 1));
            addVertex(new Vector3(1, 1, -1), new Vector2(0, 1));
            addVertex(new Vector3(1, -1, -1), new Vector2(0, 0));
            indices.Add(4); indices.Add(7); indices.Add(6);
            indices.Add(6); indices.Add(5); indices.Add(4);

            // back
            addVertex(new Vector3(1, -1, -1), new Vector2(1, 0));
            addVertex(new Vector3(1, 1, -1), new Vector2(1, 1));
            addVertex(new Vector3(-1, 1, -1), new Vector2(0, 1));
            addVertex(new Vector3(-1, -1, -1), new Vector2(0, 0));
            indices.Add(8); indices.Add(11); indices.Add(10);
            indices.Add(10); indices.Add(9); indices.Add(8);

            // left
            addVertex(new Vector3(-1, -1, -1), new Vector2(1, 0));
            addVertex(new Vector3(-1, 1, -1), new Vector2(1, 1));
            addVertex(new Vector3(-1, 1, 1), new Vector2(0, 1));
            addVertex(new Vector3(-1, -1, 1), new Vector2(0, 0));
            indices.Add(12); indices.Add(15); indices.Add(14);
            indices.Add(14); indices.Add(13); indices.Add(12);

            // top
            addVertex(new Vector3(1, 1, 1), new Vector2(0, 0));
            addVertex(new Vector3(1, 1, -1), new Vector2(0, 1));
            addVertex(new Vector3(-1, 1, -1), new Vector2(1, 1));
            addVertex(new Vector3(-1, 1, 1), new Vector2(1, 0));
            indices.Add(16); indices.Add(17); indices.Add(18);
            indices.Add(18); indices.Add(19); indices.Add(16);

            // bottom
            addVertex(new Vector3(1, -1, 1), new Vector2(1, 0));
            addVertex(new Vector3(1, -1, -1), new Vector2(1, 1));
            addVertex(new Vector3(-1, -1, -1), new Vector2(0, 1));
            addVertex(new Vector3(-1, -1, 1), new Vector2(0, 0));
            indices.Add(21); indices.Add(20); indices.Add(23);
            indices.Add(23); indices.Add(22); indices.Add(21);

            for (int i = 0; i < subdivisions; i++)
            {
                var indices2 = new List<int>();
                for (int idx = 0; idx < indices.Count; idx += 3)
                {
                    // replace triangle by 4 triangles
                    int a = getMiddlePoint(indices[idx + 0], indices[idx + 1]);
                    int b = getMiddlePoint(indices[idx + 1], indices[idx + 2]);
                    int c = getMiddlePoint(indices[idx + 2], indices[idx + 0]);

                    indices2.Add(indices[idx + 0]); indices2.Add(a); indices2.Add(c);
                    indices2.Add(indices[idx + 1]); indices2.Add(b); indices2.Add(a);
                    indices2.Add(indices[idx + 2]); indices2.Add(c); indices2.Add(b);
                    indices2.Add(a); indices2.Add(b); indices2.Add(c);
                }
                indices = indices2;
            }

            // done, create the mesh
            var mesh = new Mesh();

            mesh.vertices = positions.ToArray();
            mesh.triangles = indices.ToArray();
            mesh.uv = uvs.ToArray();
            mesh.RecalculateNormals();

            var colors = new Color[mesh.vertexCount];
            for (int i = 0; i < colors.Length; i++)
                colors[i] = new Color(1.0f, 1.0f, 1.0f);
            mesh.colors = colors;

            RecalculateTangents(mesh);

            return mesh;
        }

        public Mesh CreateSphere(int subdivisions)
        {
            var sphere = CreateCube(subdivisions);
            var vertices = new List<Vector3>(sphere.vertices);

            for (int i = 0; i < vertices.Count; i++)
                vertices[i] = vertices[i].normalized;

            sphere.vertices = vertices.ToArray();
            sphere.RecalculateNormals();
            RecalculateTangents(sphere);

            return sphere;
        }

        //public Mesh CreateDisc()
        //{
        //    CustomMesh circleCustomMesh = new CustomMesh();

        //    Vector3 P1, P2;
        //    P1 = MathUtils.PointOnCircleXZ(Vector3.zero, 1f, 0);

        //    Vertex V_P0 = new Vertex(Vector3.zero);
        //    Vertex V_P1 = new Vertex(P1);

        //    for (int i = 0; i <= 360; i += 10)
        //    {
        //        P2 = MathUtils.PointOnCircleXZ(Vector3.zero, 1f, i);
        //        Vertex V_P2 = new Vertex(P2);

        //        circleCustomMesh.AddTriangle(V_P0, V_P2, V_P1);
        //        V_P1 = V_P2;
        //    }

        //    return circleCustomMesh.CompileToUnityMesh();
        //}

        public Mesh CreateQuad()
        {
            int[] triangles = new int[6];

            // triangle 1
            triangles[0] = 0;
            triangles[1] = 1;
            triangles[2] = 2;

            // triangle 2
            triangles[3] = 2;
            triangles[4] = 1;
            triangles[5] = 3;

            // calculate vertice positions
            Vector3[] vertices = new Vector3[4];
            vertices[0] = new Vector3(-0.5f, 0f, -0.5f);
            vertices[1] = new Vector3(-0.5f, 0f, 0.5f);
            vertices[2] = new Vector3(0.5f, 0f, -0.5f);
            vertices[3] = new Vector3(0.5f, 0f, 0.5f);


            // calculate uv-mapping		
            float maxY1 = vertices[1].y;
            float maxY2 = vertices[3].y;
            float minY1 = vertices[0].y;
            float minY2 = vertices[2].y;

            float maxX1 = vertices[2].x;
            float maxX2 = vertices[3].x;
            float minX1 = vertices[0].x;
            float minX2 = vertices[1].x;


            Vector2[] uvs = new Vector2[4];

            uvs[0] = new Vector2(minX1, maxY1);
            uvs[1] = new Vector2(minX2, minY1);
            uvs[2] = new Vector2(maxX1, maxY2);
            uvs[3] = new Vector2(maxX2, minY2);


            // clear old Mesh
            Mesh mesh = new Mesh();
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.uv = uvs;

            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            ;

            return mesh;
        }



        // Lengyel, Eric. “Computing Tangent Space Basis Vectors for an Arbitrary Mesh”.
        // Terathon Software 3D Graphics Library, 2001. http://www.terathon.com/code/tangent.html
        public static void RecalculateTangents(Mesh mesh)
        {
            var tan1 = new Vector3[mesh.vertexCount];
            var tan2 = new Vector3[mesh.vertexCount];

            for (int a = 0; a < mesh.triangles.Length; a += 3)
            {
                int i1 = mesh.triangles[a + 0];
                int i2 = mesh.triangles[a + 1];
                int i3 = mesh.triangles[a + 2];

                Vector3 v1 = mesh.vertices[i1];
                Vector3 v2 = mesh.vertices[i2];
                Vector3 v3 = mesh.vertices[i3];

                Vector2 w1 = mesh.uv[i1];
                Vector2 w2 = mesh.uv[i2];
                Vector2 w3 = mesh.uv[i3];

                float x1 = v2.x - v1.x;
                float x2 = v3.x - v1.x;
                float y1 = v2.y - v1.y;
                float y2 = v3.y - v1.y;
                float z1 = v2.z - v1.z;
                float z2 = v3.z - v1.z;

                float s1 = w2.x - w1.x;
                float s2 = w3.x - w1.x;
                float t1 = w2.y - w1.y;
                float t2 = w3.y - w1.y;

                float r = 1.0F / (s1 * t2 - s2 * t1);
                var sdir = new Vector3((t2 * x1 - t1 * x2) * r, (t2 * y1 - t1 * y2) * r,
                        (t2 * z1 - t1 * z2) * r);
                var tdir = new Vector3((s1 * x2 - s2 * x1) * r, (s1 * y2 - s2 * y1) * r,
                        (s1 * z2 - s2 * z1) * r);

                tan1[i1] += sdir;
                tan1[i2] += sdir;
                tan1[i3] += sdir;

                tan2[i1] += tdir;
                tan2[i2] += tdir;
                tan2[i3] += tdir;
            }

            var tangents = new Vector4[mesh.vertexCount];
            for (long a = 0; a < mesh.vertexCount; a++)
            {
                Vector3 n = mesh.normals[a];
                Vector3 t = tan1[a];

                // Gram-Schmidt orthogonalize
                tangents[a] = t - n * Vector3.Dot(n, t);
                tangents[a].Normalize();

                // Calculate handedness
                tangents[a].w = (Vector3.Dot(Vector3.Cross(n, t), tan2[a]) < 0.0f) ? -1.0f : 1.0f;
            }

            mesh.tangents = tangents;
        }
    #endregion


    }
}