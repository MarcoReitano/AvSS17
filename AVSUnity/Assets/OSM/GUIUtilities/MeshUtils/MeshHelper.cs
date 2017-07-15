using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif



public static class MeshHelper
{

    public static Material DummyMaterialTransparent = new Material(Shader.Find("Transparent/Diffuse"));
    public static Material DummyMaterial = new Material(Shader.Find("Diffuse"));
    //public static Material DoorMaterial = new Material(Shader.Find("Transparent/Diffuse")).mainTexture = Resources.Load("glass");



    public static Mesh CombineMeshesToOneSubMesh(this Mesh combinedMesh, params Mesh[] meshes)
    {
        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> triangles = new List<int>();

        vertices.AddRange(combinedMesh.vertices);
        uvs.AddRange(combinedMesh.uv);
        triangles.AddRange(combinedMesh.triangles);

        foreach (Mesh currentMesh in meshes)
        {

            int vertexOffset = vertices.Count;

            // append vertices
            vertices.AddRange(currentMesh.vertices);

            // append uvs
            uvs.AddRange(currentMesh.uv);

            // append trianglesa
            foreach (int i in currentMesh.triangles)
                triangles.Add(i + vertexOffset);
        }

        combinedMesh.vertices = vertices.ToArray();
        combinedMesh.uv = uvs.ToArray();
        combinedMesh.triangles = triangles.ToArray();

        combinedMesh.RecalculateNormals();
        combinedMesh.RecalculateBounds();
        ;

        return combinedMesh;
    }


    public static Mesh CombineMeshesToOneSubmeshAndreas(Mesh[] mesh)
    {
        return CombineMeshesAndreas(mesh, true);
    }


    public static Mesh CombineMeshesToSeparateSubmeshAndreas(Mesh[] mesh)
    {
        return CombineMeshesAndreas(mesh, false);
    }


    private static Mesh CombineMeshesAndreas(Mesh[] mesh, bool oneSubmesh)
    {
        CombineInstance[] combineInstance = new CombineInstance[mesh.Length];

        int i = 0;
        while (i < mesh.Length)
        {
            CombineInstance comb = new CombineInstance();
            comb.mesh = mesh[i];
            combineInstance[i] = comb;
            i++;
        }
        Mesh resultMesh = new Mesh();
        resultMesh.CombineMeshes(combineInstance, oneSubmesh, false);
        resultMesh.RecalculateBounds();

        return resultMesh;
    }


    public static Mesh CombineMeshesToSeparateSubMeshes(this Mesh combinedMesh, params Mesh[] meshes)
    {
        if (combinedMesh == null)
            combinedMesh = new Mesh();

        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int>[] triangles = new List<int>[meshes.Length + 1];

        vertices.AddRange(combinedMesh.vertices);
        uvs.AddRange(combinedMesh.uv);
        triangles[0] = new List<int>();
        triangles[0].AddRange(combinedMesh.triangles);

        for (int i = 0; i < meshes.Length; i++)
        {
            int vertexOffset = vertices.Count;
            vertices.AddRange(meshes[i].vertices);
            uvs.AddRange(meshes[i].uv);

            triangles[i + 1] = new List<int>();

            // append triangles
            foreach (int index in meshes[i].triangles)
                triangles[i + 1].Add(index + vertexOffset);
        }

        combinedMesh.Clear();
        combinedMesh.subMeshCount = meshes.Length + 1;
        combinedMesh.vertices = vertices.ToArray();
        combinedMesh.uv = uvs.ToArray();

        for (int i = 0; i < triangles.Length; i++)
        {
            combinedMesh.SetTriangles(triangles[i].ToArray(), i);
        }

        combinedMesh.RecalculateNormals();
        combinedMesh.RecalculateBounds();
        ;

        return combinedMesh;
    }


    private static Mesh tmpMesh;

    public static Mesh CombineMeshesToSeparateSubMeshesParams(params Mesh[] meshes)
    {
        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int>[] triangles = new List<int>[meshes.Length];

        for (int i = 0; i < meshes.Length; i++)
        {
            int vertexOffset = vertices.Count;
            vertices.AddRange(meshes[i].vertices);
            uvs.AddRange(meshes[i].uv);

            triangles[i] = new List<int>();

            // append triangles
            foreach (int index in meshes[i].triangles)
                triangles[i].Add(index + vertexOffset);
        }

        if (tmpMesh == null)
            tmpMesh = new Mesh();

        tmpMesh.Clear();
        tmpMesh.subMeshCount = meshes.Length;
        tmpMesh.vertices = vertices.ToArray();
        tmpMesh.uv = uvs.ToArray();

        for (int i = 0; i < triangles.Length; i++)
        {
            tmpMesh.SetTriangles(triangles[i].ToArray(), i);
        }

        tmpMesh.RecalculateNormals();
        tmpMesh.RecalculateBounds();
        ;

        return tmpMesh;
    }





    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public static Mesh CreateSimplePlaneMesh()
    {
        return MeshHelper.CreateSimplePlaneMesh(1f, 1f);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="height"></param>
    /// <param name="width"></param>
    /// <returns></returns>
    public static Mesh CreateSimplePlaneMesh(float height, float width)
    {
        // calculate vertex-positions
        float heightHalf = height * 0.5f;
        float widthHalf = width * 0.5f;

        return new Mesh().CreateFace(
            new Vector3(-heightHalf, 0f, -widthHalf),
            new Vector3(-heightHalf, 0f, widthHalf),
            new Vector3(heightHalf, 0f, -widthHalf),
            new Vector3(heightHalf, 0f, widthHalf));
    }


    public static Mesh CreateFace(this Mesh mesh, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        /*                  triangles:
        //     *      1-----3      [0] = 0 
        //     *      |\    |      [1] = 1
        //     *      | \   |      [2] = 2
        //     * from o-----o to   [3] = 2
        //     *      |   \ |      [4] = 1
        //     *      |    \|      [5] = 3
        //     *      0-----2      
        //     */

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
        vertices[0] = p0;
        vertices[1] = p1;
        vertices[2] = p2;
        vertices[3] = p3;


        // calculate uv-mapping
        float y1 = (vertices[1] - vertices[0]).magnitude;
        float y2 = (vertices[3] - vertices[2]).magnitude;
        float x1 = (vertices[2] - vertices[0]).magnitude;
        float x2 = (vertices[3] - vertices[1]).magnitude;

        float maxY1 = vertices[1].y;
        float maxY2 = vertices[3].y;
        float minY1 = vertices[0].y;
        float minY2 = vertices[2].y;

        float maxX1 = vertices[2].x;
        float maxX2 = vertices[3].x;
        float minX1 = vertices[0].x;
        float minX2 = vertices[1].x;


        Vector2[] uvs = new Vector2[4];

        uvs[0] = new Vector2(minX1, minY1);
        uvs[1] = new Vector2(minX2, maxY1);
        uvs[2] = new Vector2(maxX1, minY2);
        uvs[3] = new Vector2(maxX2, maxY2);

        // Z - Achse, Y - Achse
        if (minX1 == maxX1 || minY1 == maxY1)
        {
            uvs[0] = new Vector2(0f, 0f);
            uvs[1] = new Vector2(0f, y1);
            uvs[2] = new Vector2(x1, 0f);
            uvs[3] = new Vector2(x2, y2);
        }

        // clear old Mesh
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        ;

        return mesh;
    }


    public static Mesh CreateQuad(this Mesh mesh, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        /*                  triangles:
        //     *      1-----3      [0] = 0 
        //     *      |\    |      [1] = 1
        //     *      | \   |      [2] = 2
        //     * from o-----o to   [3] = 2
        //     *      |   \ |      [4] = 1
        //     *      |    \|      [5] = 3
        //     *      0-----2      
        //     */

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
        vertices[0] = p0;
        vertices[1] = p1;
        vertices[2] = p2;
        vertices[3] = p3;


        // calculate uv-mapping
        float y1 = (vertices[1] - vertices[0]).magnitude;
        float y2 = (vertices[3] - vertices[2]).magnitude;
        float x1 = (vertices[2] - vertices[0]).magnitude;
        float x2 = (vertices[3] - vertices[1]).magnitude;

        float maxY1 = vertices[1].y;
        float maxY2 = vertices[3].y;
        float minY1 = vertices[0].y;
        float minY2 = vertices[2].y;

        float maxX1 = vertices[2].x;
        float maxX2 = vertices[3].x;
        float minX1 = vertices[0].x;
        float minX2 = vertices[1].x;


        Vector2[] uvs = new Vector2[4];

        uvs[0] = new Vector2(0f, 0f);
        uvs[1] = new Vector2(0f, 1f);
        uvs[2] = new Vector2(1f, 0f);
        uvs[3] = new Vector2(1f, 1f);

       

        // clear old Mesh
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        ;

        return mesh;
    }

    public static Mesh CreateQuad(this Mesh mesh, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, Vector2 uv0, Vector2 uv1, Vector2 uv2, Vector2 uv3)
    {
        /*                  triangles:
        //     *      1-----3      [0] = 0 
        //     *      |\    |      [1] = 1
        //     *      | \   |      [2] = 2
        //     * from o-----o to   [3] = 2
        //     *      |   \ |      [4] = 1
        //     *      |    \|      [5] = 3
        //     *      0-----2      
        //     */

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
        vertices[0] = p0;
        vertices[1] = p1;
        vertices[2] = p2;
        vertices[3] = p3;


        // calculate uv-mapping
        float y1 = (vertices[1] - vertices[0]).magnitude;
        float y2 = (vertices[3] - vertices[2]).magnitude;
        float x1 = (vertices[2] - vertices[0]).magnitude;
        float x2 = (vertices[3] - vertices[1]).magnitude;

        float maxY1 = vertices[1].y;
        float maxY2 = vertices[3].y;
        float minY1 = vertices[0].y;
        float minY2 = vertices[2].y;

        float maxX1 = vertices[2].x;
        float maxX2 = vertices[3].x;
        float minX1 = vertices[0].x;
        float minX2 = vertices[1].x;


        Vector2[] uvs = new Vector2[4];

        uvs[0] = uv0;
        uvs[1] = uv1;
        uvs[2] = uv2;
        uvs[3] = uv3;



        // clear old Mesh
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        ;

        return mesh;
    }



    public static Mesh CreateFace(this Mesh mesh, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        /*                  triangle:
        //     *      1            [0] = 0 
        //     *      |\           [1] = 1
        //     *      | \          [2] = 2
        //     * from o-----o to
        //     *      |   \  
        //     *      |    \ 
        //     *      0-----2      
        //     */

        int[] triangles = new int[3];

        // triangle
        triangles[0] = 0;
        triangles[1] = 1;
        triangles[2] = 2;

        // calculate vertice positions
        Vector3[] vertices = new Vector3[3];
        vertices[0] = p0;
        vertices[1] = p1;
        vertices[2] = p2;


        // calculate uv-mapping
        /*float y1 = (vertices[1] - vertices[0]).magnitude;
        float y2 = (vertices[3] - vertices[2]).magnitude;
        float x1 = (vertices[2] - vertices[0]).magnitude;
        float x2 = (vertices[3] - vertices[1]).magnitude;
		
        float maxY1 = vertices[1].y;
        float maxY2 = vertices[3].y;
        float minY1 = vertices[0].y;
        float minY2 = vertices[2].y;
		
        float maxX1 = vertices[2].x;
        float maxX2 = vertices[3].x;
        float minX1 = vertices[0].x;
        float minX2 = vertices[1].x;
        */

        Vector2[] uvs = new Vector2[3];
        /*
        uvs[0] = new Vector2(minX1, minY1);
        uvs[1] = new Vector2(minX2, maxY1);
        uvs[2] = new Vector2(maxX1, minY2);
        uvs[3] = new Vector2(maxX2, maxY2);

        // Z - Achse, Y - Achse
        if (minX1 == maxX1 || minY1 == maxY1) {
            uvs[0] = new Vector2(0f, 0f);
            uvs[1] = new Vector2(0f, y1);
            uvs[2] = new Vector2(x1, 0f);
            uvs[3] = new Vector2(x2, y2);
        }*/

        uvs[0] = new Vector2(0f, 0f);
        uvs[1] = new Vector2(0f, 0f);
        uvs[2] = new Vector2(0f, 0f);


        // clear old Mesh
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        ;

        return mesh;
    }


    public static Mesh CreateFaceTopDownUV(this Mesh mesh, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        /*                  triangles:
        //     *      1-----3      [0] = 0 
        //     *      |\    |      [1] = 1
        //     *      | \   |      [2] = 2
        //     * from o-----o to   [3] = 2
        //     *      |   \ |      [4] = 1
        //     *      |    \|      [5] = 3
        //     *      0-----2      
        //     */

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
        vertices[0] = p0;
        vertices[1] = p1;
        vertices[2] = p2;
        vertices[3] = p3;


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
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        ;

        return mesh;
    }

    [System.Obsolete("Use Triangulation-Method with holes..")]
    public static Mesh Triangulate(List<Vector3> vertices, bool reverse)
    {
        return Triangulate(reverse, vertices.ToArray());
    }


    [System.Obsolete("Use Triangulation-Method with holes..")]
    public static Mesh Triangulate(bool reverse, params Vector3[] vertices)
    {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;

        // set default uvs
        List<Vector2> uvs = new List<Vector2>();
        for (int i = 0; i < vertices.Length; i++)
            uvs.Add(new Vector2(vertices[i].x, vertices[i].z));
        mesh.uv = uvs.ToArray();

        List<int> triangles = new List<int>();
        List<Vector3> rest = new List<Vector3>();

        if (vertices.Length == 3)
        {
            if (reverse)
            {
                triangles.Add(0);
                triangles.Add(1);
                triangles.Add(2);

            }
            else
            {
                triangles.Add(0);
                triangles.Add(2);
                triangles.Add(1);
            }
        }
        else if (vertices.Length > 3)
        {

            for (int i = 0; i < vertices.Length - 1; i++)
            {
                Vector3 A = vertices[i % vertices.Length];
                Vector3 B = vertices[(i + 1) % vertices.Length];
                Vector3 C = vertices[(i + 2) % vertices.Length];
                Vector3 D = vertices[(i + 3) % vertices.Length];

                // calculate vectors and determine their angles to z-axis
                float upperAngle = AngleToZAxis(D - C);
                float lowerAngle = AngleToZAxis(B - C);
                float partitionAngle = AngleToZAxis(A - C);

                // angle-correction
                if (upperAngle < lowerAngle)
                {
                    lowerAngle -= upperAngle;
                    partitionAngle -= upperAngle;
                    upperAngle = 360f;
                }

                rest.Add(vertices[i % vertices.Length]);

                if (partitionAngle < upperAngle && partitionAngle > lowerAngle)
                {
                    if (reverse)
                    {
                        triangles.Add(i % vertices.Length);
                        triangles.Add((i + 1) % vertices.Length);
                        triangles.Add((i + 2) % vertices.Length);
                    }
                    else
                    {
                        triangles.Add(i % vertices.Length);
                        triangles.Add((i + 2) % vertices.Length);
                        triangles.Add((i + 1) % vertices.Length);
                    }
                    i++;
                }
                if (i == vertices.Length - 2)
                    rest.Add(vertices[vertices.Length - 1]);
            }
        }

        if (vertices.Length > 2)
            mesh.triangles = triangles.ToArray();

        if (rest.Count >= 3)
            return mesh.CombineMeshesToOneSubMesh(Triangulate(rest, reverse));
        else
            return mesh;
    }


    public static float AngleToZAxis(Vector3 targetDir)
    {
        ///TODO: Mathf. Artan2 evtl.
        if (targetDir.x >= 0)
            return Vector3.Angle(targetDir, Vector3.forward);
        else
            return 360f - Vector3.Angle(targetDir, Vector3.forward);
    }





    public static Mesh GenerateGridMesh(Mesh mesh, Vector3 transformPosition, Vector3Grid points)
    {
        if (points == null)
            throw new UnityException("No Points! Points not initialized?");


        mesh.Clear();

        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> triangles = new List<int>();

        int width = points.Width;
        int height = points.Height;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 point = points[x, y];
                vertices.Add(point - transformPosition);
                uvs.Add(new Vector2(point.x, point.z));
            }
        }

        for (int x = 0; x < width - 1; x++)
        {
            for (int y = 0; y < height - 1; y++)
            {
                triangles.Add(x * height + y);
                triangles.Add(x * height + y + 1);
                triangles.Add((x + 1) * height + y);

                triangles.Add((x + 1) * height + y);
                triangles.Add(x * height + y + 1);
                triangles.Add((x + 1) * height + y + 1);
            }
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        ;
        return mesh;
    }


    public static Mesh GenerateGridMesh(Mesh mesh, Vector3 transformPosition, Vector3[,] points)
    {
        if (points == null)
            throw new UnityException("No Points! Points not initialized?");


        mesh.Clear();

        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> triangles = new List<int>();

        int width = points.GetLength(0);
        int height = points.GetLength(1);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 point = points[x, y];
                vertices.Add(point - transformPosition);
                uvs.Add(new Vector2(point.x, point.z));
            }
        }

        for (int x = 0; x < width - 1; x++)
        {
            for (int y = 0; y < height - 1; y++)
            {
                triangles.Add(x * height + y);
                triangles.Add(x * height + y + 1);
                triangles.Add((x + 1) * height + y);

                triangles.Add((x + 1) * height + y);
                triangles.Add(x * height + y + 1);
                triangles.Add((x + 1) * height + y + 1);
            }
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        ;
        return mesh;
    }

    public static Mesh GenerateWaveMesh(Mesh mesh, Vector3 transformPosition, List<Vector3> points)
    {
        mesh.Clear();

        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> triangles = new List<int>();

        foreach (Vector3 vertex in points)
        {
            vertices.Add(vertex - transformPosition);
        }
        // points.count --> number of wave-vertices
        // points.count * 2 --> number of vertices complete

        foreach (Vector3 vertex in points)
        {
            vertices.Add(new Vector3(vertex.x, 0f, vertex.z) - transformPosition);
        }

        int numberOfPoints = points.Count;
        for (int i = 0; i < points.Count - 1; i++)
        {
            triangles.Add(i + numberOfPoints);
            triangles.Add(i);
            triangles.Add(i + numberOfPoints + 1);

            triangles.Add(i + numberOfPoints + 1);
            triangles.Add(i);
            triangles.Add(i + 1);
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        ;
        return mesh;
    }
}
