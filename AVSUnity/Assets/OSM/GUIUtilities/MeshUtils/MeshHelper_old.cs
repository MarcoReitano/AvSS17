//using UnityEngine;
//using System.Collections;
//using UnityEditor;
//using System.Collections.Generic;
//using System.Text;
//
//
//public static class MeshHelper
//{
//
//    /// <summary>
//    /// Example-Usage
//    /// </summary>
//    [MenuItem("GameObject/Create Other/3D-Object")]
//    public static void CombineSimpleSubMeshes()
//    {
//        Mesh newMesh = new Mesh().CreateFace(
//                       new Vector3(0f, 0f, 0f),
//                       new Vector3(0f, 0f, 1f),
//                       new Vector3(1f, 0f, 0f),
//                       new Vector3(1f, 0f, 1f));
//
//        Mesh newMesh1 = new Mesh().CreateFace(
//            new Vector3(1f, 0f, 0f),
//            new Vector3(1f, 0f, 1f),
//            new Vector3(2f, 0f, 0f),
//            new Vector3(2f, 0f, 1f));
//
//
//        Mesh newMesh2 = new Mesh().CreateFace(
//            new Vector3(2f, 0f, 0f),
//            new Vector3(2f, 0f, 1f),
//            new Vector3(3f, 0f, 0f),
//            new Vector3(3f, 0f, 1f));
//
//
//        Mesh subMesh1 = newMesh.CombineMeshesToOneSubMesh(newMesh, newMesh1);
//        Mesh completeMesh = subMesh1.CombineMeshesToSeparateSubMeshes(subMesh1, newMesh2);
//
//        GameObject go = new GameObject("3DObject");
//        go.Create3DObject(completeMesh);
//    }
//
//
//    ///// <summary>
//    ///// Example-Usage
//    ///// </summary>
//    //[MenuItem("GameObject/Create Other/3D-Object")]
//    //public static void CombineSimpleSubMeshes()
//    //{
//    //    Mesh newMesh = new Mesh().CreateFace(
//    //                   new Vector3(0f, 0f, 0f),
//    //                   new Vector3(0f, 0f, 1f),
//    //                   new Vector3(1f, 0f, 0f),
//    //                   new Vector3(1f, 0f, 1f));
//
//    //    Mesh newMesh1 = new Mesh().CreateFace(
//    //        new Vector3(1f, 0f, 0f),
//    //        new Vector3(1f, 0f, 1f),
//    //        new Vector3(2f, 0f, 0f),
//    //        new Vector3(2f, 0f, 1f));
//
//
//    //    Mesh newMesh2 = new Mesh().CreateFace(
//    //        new Vector3(2f, 0f, 0f),
//    //        new Vector3(2f, 0f, 1f),
//    //        new Vector3(3f, 0f, 0f),
//    //        new Vector3(3f, 0f, 1f));
//
//
//    //    Mesh subMesh1 = newMesh.CombineMeshesToOneSubMesh(newMesh, newMesh1);
//    //    Mesh completeMesh = subMesh1.CombineMeshesToSeparateSubMeshes(subMesh1, newMesh2);
//
//    //    GameObject go = new GameObject("3DObject");
//    //    go.Create3DObject(completeMesh);
//    //}
//
//
//    ///// <summary>
//    ///// Example-Usage
//    ///// </summary>
//    //[MenuItem("GameObject/Create Other/Brett")]
//    //public static void CreateBrett()
//    //{
//    //    GameObject go = new GameObject("3DObject");
//    //    go.Create3DObject(CreateBrettMesh());
//    //}
//
//    //private static Mesh CreateBrettMesh(float length, float width, float strength)
//    //{
//    //    Mesh newMesh = new Mesh().CreateFace(
//    //                   new Vector3(0f, 0f, 0f),
//    //                   new Vector3(0f, 0f, 1f),
//    //                   new Vector3(1f, 0f, 0f),
//    //                   new Vector3(1f, 0f, 1f));
//
//    //    Mesh newMesh1 = new Mesh().CreateFace(
//    //        new Vector3(1f, 0f, 0f),
//    //        new Vector3(1f, 0f, 1f),
//    //        new Vector3(2f, 0f, 0f),
//    //        new Vector3(2f, 0f, 1f));
//
//
//    //    Mesh newMesh2 = new Mesh().CreateFace(
//    //        new Vector3(2f, 0f, 0f),
//    //        new Vector3(2f, 0f, 1f),
//    //        new Vector3(3f, 0f, 0f),
//    //        new Vector3(3f, 0f, 1f));
//
//
//    //    Mesh subMesh1 = newMesh.CombineMeshesToOneSubMesh(newMesh, newMesh1);
//    //    Mesh completeMesh = subMesh1.CombineMeshesToSeparateSubMeshes(subMesh1, newMesh2);
//    //    return completeMesh;
//    //}
//
//
//
//
//    /// <summary>
//    /// Example-Usage
//    /// </summary>
//    [MenuItem("GameObject/Create Other/PolygonTriangulation XZ-Plane")]
//    public static void PolygonTriangulationExampleXZ()
//    {
//        List<Vector3> points = new List<Vector3>();
//
//        points.Add(new Vector3(0f, 0f, 20f));
//        points.Add(new Vector3(0f, 0f, 0f));
//        points.Add(new Vector3(10f, 0f, 0f));
//        points.Add(new Vector3(10f, 0f, 10f));
//        points.Add(new Vector3(20f, 0f, 10f));
//        points.Add(new Vector3(20f, 0f, 0f));
//        points.Add(new Vector3(30f, 0f, 0f));
//        points.Add(new Vector3(35f, 0f, 10f));
//        points.Add(new Vector3(30f, 0f, 20f));
//
//        Mesh completeMesh = MeshHelper.Triangulate(points);
//        //completeMesh.RemoveDoubles();
//        GameObject go = new GameObject("PolygonTriangulationExample");
//        go.Create3DObject(completeMesh);
//    }
//
//    /// <summary>
//    /// Example-Usage
//    /// </summary>
//    [MenuItem("GameObject/Create Other/PolygonTriangulation XY-Plane")]
//    public static void PolygonTriangulationExampleXY()
//    {
//        List<Vector3> points = new List<Vector3>();
//
//        points.Add(new Vector3(0f, 20f, 0f));
//        points.Add(new Vector3(0f, 0f, 0f));
//        points.Add(new Vector3(10f, 0f, 0f));
//        points.Add(new Vector3(10f, 10f, 0f));
//        points.Add(new Vector3(20f, 10f, 0f));
//        points.Add(new Vector3(20f, 0f, 0f));
//        points.Add(new Vector3(30f, 0f, 0f));
//        points.Add(new Vector3(35f, 10f, 0f));
//        points.Add(new Vector3(30f, 20f, 0f));
//
//        Mesh completeMesh = MeshHelper.Triangulate(points);
//        //completeMesh.RemoveDoubles();
//        GameObject go = new GameObject("PolygonTriangulationExample");
//        go.Create3DObject(completeMesh);
//    }
//
//
//    /// <summary>
//    /// 
//    /// </summary>
//    /// <param name="gameObject"></param>
//    /// <param name="mesh"></param>
//    /// <returns></returns>
//    public static GameObject Create3DObject(this GameObject gameObject, Mesh mesh)
//    {
//        if(gameObject == null)
//            gameObject = new GameObject();
//
//        MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
//        if(meshFilter == null)
//            meshFilter = gameObject.AddComponent<MeshFilter>();
//        
//        meshFilter.sharedMesh = mesh;
//
//        MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
//        if(meshRenderer == null)
//            meshRenderer = gameObject.AddComponent<MeshRenderer>();
//        meshRenderer.materials = new Material[meshFilter.sharedMesh.subMeshCount];
//
//        return gameObject;
//    }
//
//
//    /// <summary>
//    /// 
//    /// </summary>
//    /// <param name="combinedMesh"></param>
//    /// <param name="meshes"></param>
//    /// <returns></returns>
//    public static Mesh CombineMeshesToOneSubMesh(this Mesh combinedMesh, params Mesh[] meshes)
//    {
//        List<Vector3> vertices = new List<Vector3>();
//        List<Vector2> uvs = new List<Vector2>();
//        List<int> triangles = new List<int>();
//
//        vertices.AddRange(combinedMesh.vertices);
//        uvs.AddRange(combinedMesh.uv);
//        triangles.AddRange(combinedMesh.triangles);
//
//        combinedMesh.Clear();
//
//        foreach (Mesh currentMesh in meshes)
//        {
//            int vertexOffset = vertices.Count;
//            // append vertices
//            vertices.AddRange(currentMesh.vertices);
//
//            // append uvs
//            uvs.AddRange(currentMesh.uv);
//
//            // append triangles
//            foreach (int i in currentMesh.triangles)
//                triangles.Add(i + vertexOffset);
//        }
//
//        combinedMesh.vertices = vertices.ToArray();
//        combinedMesh.uv = uvs.ToArray();
//        combinedMesh.triangles = triangles.ToArray();
//
//        //combinedMesh.RemoveDoubles();
//        combinedMesh.RecalculateNormals();
//        combinedMesh.RecalculateBounds();
//        combinedMesh.Optimize();
//
//
//        return combinedMesh;
//    }
//
//    private static Mesh tmpMesh;
//
//    /*
//     * TODO: remove first face
//     * 
//    public static Mesh CombineMeshesToOneSubMeshParams(params Mesh[] meshes) {
//			
//        Mesh[] meshes2 = new Mesh[meshes.Length-1];
//			
//        for(int i = 1; i < meshes.Length; i++){
//            meshes2[i-1] = meshes[i];
//        }
//			
//        return meshes[0].CombineMeshesToOneSubMesh(meshes2);
//    }*/
//
//    /// <summary>
//    /// 
//    /// </summary>
//    /// <param name="meshes"></param>
//    /// <returns></returns>
//    public static Mesh CombineMeshesToOneSubMeshParams(params Mesh[] meshes)
//    {
//        if (tmpMesh == null)
//            tmpMesh = new Mesh();
//        else
//            tmpMesh.Clear();
//
//        return tmpMesh.CombineMeshesToOneSubMesh(meshes);
//        /*
//        List<Vector3> vertices = new List<Vector3>();
//        List<Vector2> uvs = new List<Vector2>();
//        List<int> triangles = new List<int>();
//
//        foreach (Mesh currentMesh in meshes)
//        {
//            int vertexOffset = vertices.Count;
//            // append vertices
//            vertices.AddRange(currentMesh.vertices);
//				
//            // append uvs
//            uvs.AddRange(currentMesh.uv);
//
//            // append triangles
//            foreach (int i in currentMesh.triangles)
//                triangles.Add(i + vertexOffset);
//        }
//
//        tmpMesh.vertices = vertices.ToArray();
//        tmpMesh.uv = uvs.ToArray();
//        tmpMesh.triangles = triangles.ToArray();
//
//        //tmpMesh.RemoveDoubles();
//        tmpMesh.RecalculateNormals();
//        tmpMesh.RecalculateBounds();
//        tmpMesh.Optimize();
//
//        return tmpMesh;*/
//
//    }
//
//    /// <summary>
//    /// 
//    /// </summary>
//    /// <param name="combinedMesh"></param>
//    /// <param name="meshes"></param>
//    /// <returns></returns>
//    public static Mesh CombineMeshesToSeparateSubMeshes(this Mesh combinedMesh, params Mesh[] meshes)
//    {
//
//        List<Vector3> vertices = new List<Vector3>();
//        List<Vector2> uvs = new List<Vector2>();
//        List<int>[] triangles = new List<int>[meshes.Length + 1];
//
//
//        vertices.AddRange(combinedMesh.vertices);
//        uvs.AddRange(combinedMesh.uv);
//        triangles[0] = new List<int>();
//        triangles[0].AddRange(combinedMesh.triangles);
//
//        combinedMesh.Clear();
//        for (int i = 0; i < meshes.Length; i++)
//        {
//            int vertexOffset = vertices.Count;
//            vertices.AddRange(meshes[i].vertices);
//            uvs.AddRange(meshes[i].uv);
//
//            triangles[i + 1] = new List<int>();
//
//            // append triangles
//            foreach (int index in meshes[i].triangles)
//                triangles[i + 1].Add(index + vertexOffset);
//
//        }
//
//       
//        combinedMesh.subMeshCount = meshes.Length + 1;
//        combinedMesh.vertices = vertices.ToArray();
//        combinedMesh.uv = uvs.ToArray();
//
//        for (int i = 0; i < triangles.Length; i++)
//        {
//            //Debug.Log(i + ". Submesh gesetzt");
//            combinedMesh.SetTriangles(triangles[i].ToArray(), i);
//        }
//
//
//        combinedMesh.RecalculateNormals();
//        combinedMesh.RecalculateBounds();
//        combinedMesh.Optimize();
//
//        return combinedMesh;
//    }
//
//
//    /// <summary>
//    /// 
//    /// </summary>
//    /// <returns></returns>
//    public static Mesh CreateSimplePlaneMesh()
//    {
//        return MeshHelper.CreateSimplePlaneMesh(1f, 1f);
//    }
//
//    /// <summary>
//    /// 
//    /// </summary>
//    /// <param name="height"></param>
//    /// <param name="width"></param>
//    /// <returns></returns>
//    public static Mesh CreateSimplePlaneMesh(float height, float width)
//    {
//        // calculate vertex-positions
//        float heightHalf = height * 0.5f;
//        float widthHalf = width * 0.5f;
//
//        return new Mesh().CreateFace(
//            new Vector3(-heightHalf, 0f, -widthHalf),
//            new Vector3(-heightHalf, 0f, widthHalf),
//            new Vector3(heightHalf, 0f, -widthHalf),
//            new Vector3(heightHalf, 0f, widthHalf));
//    }
//
//
//    /// <summary>
//    /// 
//    /// </summary>
//    /// <param name="mesh"></param>
//    /// <param name="P0"></param>
//    /// <param name="P1"></param>
//    /// <param name="P2"></param>
//    /// <param name="P3"></param>
//    /// <returns></returns>
//    public static Mesh CreateFace(this Mesh mesh, Vector3 P0, Vector3 P1, Vector3 P2, Vector3 P3)
//    {
//        //Mesh mesh = new Mesh();
//        mesh.Clear();
//
//        /*                  triangles:
//        //     *      1-----3      [0] = 0 
//        //     *      |\    |      [1] = 1
//        //     *      | \   |      [2] = 2
//        //     * from o-----o to   [3] = 2
//        //     *      |   \ |      [4] = 1
//        //     *      |    \|      [5] = 3
//        //     *      0-----2      
//        //     */
//
//        int[] triangles = new int[6];
//
//        // triangle 1
//        triangles[0] = 0;
//        triangles[1] = 1;
//        triangles[2] = 2;
//
//        // triangle 2
//        triangles[3] = 2;
//        triangles[4] = 1;
//        triangles[5] = 3;
//
//
//        // calculate verticepositions
//        Vector3[] vertices = new Vector3[4];
//        vertices[0] = P0;
//        vertices[1] = P1;
//        vertices[2] = P2;
//        vertices[3] = P3;
//
//        ///TODO: UV-Mapping not correct
//        // calculate uvs
//        float y1 = (vertices[1] - vertices[0]).magnitude;
//        float y2 = (vertices[3] - vertices[2]).magnitude;
//        float x = (vertices[2] - vertices[0]).magnitude;
//
//        float maxY1 = vertices[1].y;
//        float maxY2 = vertices[3].y;
//        float minY1 = maxY1 - y1;
//        float minY2 = maxY2 - y2;
//
//
//        Vector2[] uvs = new Vector2[4];
//        uvs[0] = new Vector2(0f, minY1);
//        uvs[1] = new Vector2(0f, maxY1);
//        uvs[2] = new Vector2(x, minY2);
//        uvs[3] = new Vector2(x, maxY2);
//
//
//
//        //float height = (vertices[1] - vertices[0]).magnitude;
//        //float width = (vertices[2] - vertices[0]).magnitude;
//        //Vector2[] uvs = new Vector2[4];
//        //uvs[0] = new Vector2(0f, 0f);
//        //uvs[1] = new Vector2(0f, height);
//        //uvs[2] = new Vector2(width, 0f);
//        //uvs[3] = new Vector2(width, height);
//        //Vector2[] uvs = new Vector2[4];
//        //            uvs[0] = new Vector2(P0.x, P0.z);
//        //            uvs[1] = new Vector2(P1.x, P1.z);
//        //            uvs[2] = new Vector2(P2.x, P2.z);
//        //            uvs[3] = new Vector2(P3.x, P3.z);
//
//        //            uvs[0] = new Vector2(P0.x, P0.y);
//        //            uvs[1] = new Vector2(P1.x, P1.y);
//        //            uvs[2] = new Vector2(P2.x, P2.y);
//        //            uvs[3] = new Vector2(P3.x, P3.y);
//
//
//        // clear the old one an set the new mesh
//        mesh.Clear();
//        mesh.vertices = vertices;
//        mesh.triangles = triangles;
//        mesh.uv = uvs;
//
//
//        mesh.RecalculateNormals();
//        mesh.RecalculateBounds();
//        mesh.Optimize();
//
//        return mesh;
//    }
//
//
//    public static Mesh CreateFace(this Mesh mesh, Vector3 P0, Vector3 P1, Vector3 P2)
//    {
//        //Mesh mesh = new Mesh();
//        mesh.Clear();
//
//        /*                  triangles:
//        //     *      1-----3      [0] = 0 
//        //     *      |\    |      [1] = 1
//        //     *      | \   |      [2] = 2
//        //     * from o-----o to   [3] = 2
//        //     *      |   \ |      [4] = 1
//        //     *      |    \|      [5] = 3
//        //     *      0-----2      
//        //     */
//
//        int[] triangles = new int[3];
//
//        // triangle 1
//        triangles[0] = 0;
//        triangles[1] = 1;
//        triangles[2] = 2;
//
//
//        // calculate verticepositions
//        Vector3[] vertices = new Vector3[3];
//        vertices[0] = P0;
//        vertices[1] = P1;
//        vertices[2] = P2;
//     
//        /////TODO: UV-Mapping not correct
//        //// calculate uvs
//        //float y1 = (vertices[1] - vertices[0]).magnitude;
//        //float y2 = (vertices[3] - vertices[2]).magnitude;
//        //float x = (vertices[2] - vertices[0]).magnitude;
//
//        //float maxY1 = vertices[1].y;
//        //float maxY2 = vertices[3].y;
//        //float minY1 = maxY1 - y1;
//        //float minY2 = maxY2 - y2;
//
//
//        //Vector2[] uvs = new Vector2[4];
//        //uvs[0] = new Vector2(0f, minY1);
//        //uvs[1] = new Vector2(0f, maxY1);
//        //uvs[2] = new Vector2(x, minY2);
//        //uvs[3] = new Vector2(x, maxY2);
//
//        Vector2[] uvs = new Vector2[3];
//        uvs[0] = new Vector2(P0.x, P0.y);
//        uvs[1] = new Vector2(P1.x, P1.y);
//        uvs[2] = new Vector2(P2.x, P2.y);
//
//
//        //float height = (vertices[1] - vertices[0]).magnitude;
//        //float width = (vertices[2] - vertices[0]).magnitude;
//        //Vector2[] uvs = new Vector2[4];
//        //uvs[0] = new Vector2(0f, 0f);
//        //uvs[1] = new Vector2(0f, height);
//        //uvs[2] = new Vector2(width, 0f);
//        //uvs[3] = new Vector2(width, height);
//        //Vector2[] uvs = new Vector2[4];
//        //            uvs[0] = new Vector2(P0.x, P0.z);
//        //            uvs[1] = new Vector2(P1.x, P1.z);
//        //            uvs[2] = new Vector2(P2.x, P2.z);
//        //            uvs[3] = new Vector2(P3.x, P3.z);
//
//        //            uvs[0] = new Vector2(P0.x, P0.y);
//        //            uvs[1] = new Vector2(P1.x, P1.y);
//        //            uvs[2] = new Vector2(P2.x, P2.y);
//        //            uvs[3] = new Vector2(P3.x, P3.y);
//
//
//        // clear the old one an set the new mesh
//        mesh.Clear();
//        mesh.vertices = vertices;
//        mesh.triangles = triangles;
//        mesh.uv = uvs;
//
//
//        mesh.RecalculateNormals();
//        mesh.RecalculateBounds();
//        mesh.Optimize();
//
//        return mesh;
//    }
//
//    /// <summary>
//    /// 
//    /// </summary>
//    /// <param name="vertices"></param>
//    /// <returns></returns>
//    public static Mesh Triangulate(List<Vector3> vertices)
//    {
//        return Triangulate(vertices.ToArray());
//    }
//
//    /// <summary>
//    /// 
//    /// </summary>
//    /// <param name="vertices"></param>
//    /// <returns></returns>
//    public static Mesh Triangulate(params Vector3[] vertices)
//    {
//
//        Mesh mesh = new Mesh();
//        mesh.vertices = vertices;
//
//        // set default uvs
//        List<Vector2> uvs = new List<Vector2>();
//        for (int i = 0; i < vertices.Length; i++)
//            uvs.Add(new Vector2(vertices[i].x, vertices[i].z));
//        mesh.uv = uvs.ToArray();
//
//        List<int> triangles = new List<int>();
//        List<Vector3> rest = new List<Vector3>();
//
//        if (vertices.Length == 3)
//        {
//            triangles.Add(0);
//            triangles.Add(2);
//            triangles.Add(1);
//        }
//        else if (vertices.Length > 3)
//        {
//
//            for (int i = 0; i < vertices.Length - 1; i++)
//            {
//                Vector3 A = vertices[i % vertices.Length];
//                Vector3 B = vertices[(i + 1) % vertices.Length];
//                Vector3 C = vertices[(i + 2) % vertices.Length];
//                Vector3 D = vertices[(i + 3) % vertices.Length];
//
//                // calculate vectors and determine their angles to z-axis
//                float upperAngle = AngleToZAxis(D - C);
//                float lowerAngle = AngleToZAxis(B - C);
//                float partitionAngle = AngleToZAxis(A - C);
//
//                // angle-correction
//                if (upperAngle < lowerAngle)
//                {
//                    lowerAngle -= upperAngle;
//                    partitionAngle -= upperAngle;
//                    upperAngle = 360f;
//                }
//
//                rest.Add(vertices[i % vertices.Length]);
//
//                if (partitionAngle < upperAngle && partitionAngle > lowerAngle)
//                {
//                    triangles.Add(i % vertices.Length);
//                    triangles.Add((i + 2) % vertices.Length);
//                    triangles.Add((i + 1) % vertices.Length);
//                    i++;
//                }
//                if (i == vertices.Length - 2)
//                    rest.Add(vertices[vertices.Length - 1]);
//            }
//        }
//
//        if (vertices.Length > 2)
//            mesh.triangles = triangles.ToArray();
//
//        if (rest.Count >= 3)
//        {
//            return mesh.CombineMeshesToOneSubMesh(mesh, Triangulate(rest));
//        }
//        else
//            return mesh;
//    }
//
//    /// <summary>
//    /// 
//    /// </summary>
//    /// <param name="targetDir"></param>
//    /// <returns></returns>
//    public static float AngleToZAxis(Vector3 targetDir)
//    {
//        ///TODO: Mathf. Artan2 evtl.
//        if (targetDir.x >= 0)
//            return Vector3.Angle(targetDir, Vector3.forward);
//        else
//            return 360f - Vector3.Angle(targetDir, Vector3.forward);
//    }
//}
