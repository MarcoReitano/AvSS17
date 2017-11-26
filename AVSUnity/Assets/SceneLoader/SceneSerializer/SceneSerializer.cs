using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class SceneSerializer
{

    public class SerializedGameObject
    {
        public Transform transform;
        public List<System.Type> components;

        public SerializedGameObject(GameObject go)
        {
            Component[] components = go.GetComponents<Component>();

            foreach (Component item in components)
            {
                System.Type type = item.GetType();
                Debug.Log(go.name + ": " + type);

                if (type == typeof(Transform))
                {
                    // if transform cannot be serialized
                }
                if (type == typeof(MeshFilter))
                {
                    // Mesh serialisieren
                }
                else if (type == typeof(MeshRenderer))
                {
                    // Materials-Array serialisieren

                }
            }


        }
    }

    [System.Serializable]
    public class SerializedTransform
    {
        public Vector3 position;

        public Vector3 scale;
    }

    [System.Serializable]
    public class SerializedMeshFilter
    {

    }

    [System.Serializable]
    public class SerializedMeshRenderer
    {

    }

    // SerializedScene soll eine Liste mit allen serialized Meshes enthalten
    // Das SerializedMesh soll sowohl Mesh als auch Materialien enthalten.
    //


    // GameObject
    //     Transform Serialisieren
    //     Mesh serialisieren
    //     Materialien Serialisieren

    // Terrain Serialisieren
    //      Welche Attribute?
    //      nur heightmap?

    public static string SerializeScene(Scene scene)
    {
        // Mesh serialisieren --> wie?
        //     Mesh serializable?
        //     ansonsten --> Vertex-Array / Triangle-Array(s) / UVs
        //     Mesh und/oder Scene WrapperKlasse die Serializable ist
        // Alle Objekte auf root-Ebene der Szene traversieren
        foreach (GameObject item in scene.GetRootGameObjects())
        {

            // Prüfen ob Mesh vorhanden
            foreach (Transform transform in item.GetComponentsInChildren<Transform>())
            {
                SerializedGameObject serGo = new SerializedGameObject(transform.gameObject);


                MeshFilter mf = transform.GetComponent<MeshFilter>();
                if (mf != null)
                {
                    Mesh mesh = mf.sharedMesh;

                    byte[] serializedMesh = MeshSerializer.WriteMesh(mesh, false);

                    Debug.Log(transform.name + ": SerializedMesh has " + serializedMesh.Length + " bytes");
                }
            }
        }
        return "It works!";



    }



}



public static class SerializerHelper
{
    #region Helper
    private static int tabLevel = 0;

    public static string Tabs()
    {
        return new string(' ', tabLevel * 4);
    }
    #endregion Helper
}

    public static class SceneExtensions
{
    #region Helper
    private static int tabLevel = 0;

    private static string Tabs()
    {
        return new string(' ', tabLevel * 4);
    }
    #endregion Helper

    #region Scene
    public static string ToJSON(this Scene scene)
    {
        StringBuilder sb = new StringBuilder();

        sb.Append(Tabs()).Append("{\n");
        tabLevel++;
        {
            sb.Append(Tabs()).Append("\"Scene\" : {\n");
            tabLevel++;
            {
                sb.Append(Tabs()).Append("\"name\"").Append(" : ").Append("\"").Append(scene.name).Append("\"").Append(",\n");
                sb.Append(Tabs()).Append("\"rootGameObjects\"").Append(" : [\n");

                tabLevel++;
                {
                    foreach (GameObject item in scene.GetRootGameObjects())
                    {
                        sb.Append(Tabs()).Append("\"GameObject\" : ").Append(item.ToJSON()).Append(",\n");
                    }
                    if (scene.GetRootGameObjects().Length > 0)
                        sb.Remove(sb.Length - 2, 1);
                }
                tabLevel--;
                sb.Append(Tabs()).Append("]\n");
            }
            tabLevel--;
            sb.Append(Tabs()).Append("}\n");
        }
        tabLevel--;
        sb.Append(Tabs()).Append("}");
        return sb.ToString();
        //return SceneSerializer.SerializeScene(scene);
    }

    //public static Scene FromJSON(string json)
    //{
    //    return new Vector3();
    //}
    #endregion Scene


    public static class GameObjectSerializer
    {
        #region GameObject
        public static string ToJSON(this GameObject go)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{\n");
            tabLevel++;
            {
                sb.Append(Tabs()).Append("\"name\"").Append(" : ").Append("\"").Append(go.name).Append("\"").Append(",\n");

                sb.Append(Tabs()).Append("\"Components\"").Append(" : [\n");

                tabLevel++;
                {
                    Component[] components = go.GetComponents<Component>();

                    foreach (Component item in components)
                    {
                        System.Type type = item.GetType();

                        if (type == typeof(Transform))
                        {
                            sb.Append(Tabs()).Append("\"Transform\"").Append(" : ").Append(((Transform)item).ToJSON()).Append(",\n");
                        }
                        if (type == typeof(MeshFilter))
                        {
                            sb.Append(Tabs()).Append("\"MeshFilter\"").Append(" : ").Append(((MeshFilter)item).ToJSON()).Append(",\n");
                        }
                        else if (type == typeof(MeshRenderer))
                        {
                            sb.Append(Tabs()).Append("\"MeshRenderer\"").Append(" : ").Append(((MeshRenderer)item).ToJSON()).Append(",\n");
                        }
                    }
                    if (components.Length > 0)
                        sb.Remove(sb.Length - 2, 1);
                }
                tabLevel--;
                sb.Append(Tabs()).Append("],\n");

                // Children
                List<GameObject> realChildren = new List<GameObject>();
                foreach (Transform child in go.GetComponentsInChildren<Transform>())
                {
                    if (child.gameObject == go)
                        continue;
                    if (child.parent != go.transform)
                        continue;
                    else
                        realChildren.Add(child.gameObject);
                }
                Debug.Log(go.name + ": " + realChildren.Count);
                if (realChildren.Count > 0)
                {
                    sb.Append(Tabs()).Append("\"Children\"").Append(" : ").Append("[");
                    {
                        tabLevel++;
                        foreach (GameObject child in realChildren)
                        {
                            sb.Append("\"GameObject\" : ").Append(child.ToJSON()).Append(",\n");
                        }
                        tabLevel--;
                        if (realChildren.Count > 0)
                            sb.Remove(sb.Length - 2, 1);
                    }
                    sb.Append(Tabs()).Append("]\n");
                }
                else
                {
                    sb.Remove(sb.Length - 2, 1);
                }
            }
            tabLevel--;
            sb.Append(Tabs()).Append("}");
            return sb.ToString();
        }
        #endregion GameObject
    }

    public static class QuaternionSerializer
    {
        #region Quaternion
        public static string ToJSON(this Quaternion quat)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{\n");
            tabLevel++;
            {
                sb.Append(Tabs()).Append("\"x\"").Append(" : ").Append(quat.x).Append(",\n");
                sb.Append(Tabs()).Append("\"y\"").Append(" : ").Append(quat.y).Append(",\n");
                sb.Append(Tabs()).Append("\"z\"").Append(" : ").Append(quat.z).Append(",\n");
                sb.Append(Tabs()).Append("\"w\"").Append(" : ").Append(quat.w).Append("\n");
            }
            tabLevel--;
            sb.Append(Tabs()).Append("}");
            return sb.ToString();
        }

        //public static Quaternion FromJSON(string json)
        //{

        //    Quaternion quat = new Quaternion(;
        //    return Quaternion.;
        //}
        #endregion Quaternion
    }

    public static class Vector3Serializer
    {
        #region Vector3
        public static string ToJSON(this Vector3 vec3)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("{\n");
            tabLevel++;
            {
                sb.Append(Tabs()).Append("\"x\"").Append(" : ").Append(vec3.x).Append(",\n");
                sb.Append(Tabs()).Append("\"y\"").Append(" : ").Append(vec3.y).Append(",\n");
                sb.Append(Tabs()).Append("\"z\"").Append(" : ").Append(vec3.z).Append("\n");
            }
            tabLevel--;
            sb.Append(Tabs()).Append("}");
            return sb.ToString();
        }
        #endregion Vector3
    }

    public static class Vector4Serializer
    {
        #region Vector4
        public static string ToJSON(this Vector4 vec4)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("{\n");
            tabLevel++;
            {
                sb.Append(Tabs()).Append("\"x\"").Append(" : ").Append(vec4.x).Append(",\n");
                sb.Append(Tabs()).Append("\"y\"").Append(" : ").Append(vec4.y).Append(",\n");
                sb.Append(Tabs()).Append("\"z\"").Append(" : ").Append(vec4.z).Append(",\n");
                sb.Append(Tabs()).Append("\"w\"").Append(" : ").Append(vec4.w).Append("\n");
            }
            tabLevel--;
            sb.Append(Tabs()).Append("}");
            return sb.ToString();
        }
        #endregion Vector4
    }

    public static class ColorSerializer
    {
        #region Color
        public static string ToJSON(this Color color)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("{\n");
            tabLevel++;
            {
                sb.Append(Tabs()).Append("\"r\"").Append(" : ").Append(color.r).Append(",\n");
                sb.Append(Tabs()).Append("\"g\"").Append(" : ").Append(color.g).Append(",\n");
                sb.Append(Tabs()).Append("\"b\"").Append(" : ").Append(color.b).Append(",\n");
                sb.Append(Tabs()).Append("\"a\"").Append(" : ").Append(color.a).Append("\n");
            }
            tabLevel--;
            sb.Append(Tabs()).Append("}");
            return sb.ToString();
        }
        #endregion Color
    }

    public static class Vector2Serializer
    {
        #region Vector2
        public static string ToJSON(this Vector2 vec2)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{\n");
            tabLevel++;
            {
                sb.Append(Tabs()).Append("\"x\"").Append(" : ").Append(vec2.x).Append(",\n");
                sb.Append(Tabs()).Append("\"y\"").Append(" : ").Append(vec2.y).Append("\n");
            }
            tabLevel--;
            sb.Append(Tabs()).Append("}");
            return sb.ToString();
        }
        #endregion Vector2
    }

    public static class TransformSerializer
    {
        #region Transform
        public static string ToJSON(this Transform transform)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{\n");
            tabLevel++;
            {
                sb.Append(Tabs()).Append("\"Position\"").Append(" : ").Append(transform.position.ToJSON()).Append(",\n");
                sb.Append(Tabs()).Append("\"Rotation\"").Append(" : ").Append(transform.localRotation.ToJSON()).Append(",\n");
                sb.Append(Tabs()).Append("\"Scale\"").Append(" : ").Append(transform.localScale.ToJSON()).Append("\n");
            }
            tabLevel--;
            sb.Append(Tabs()).Append("}");
            return sb.ToString();
        }
        #endregion Transform
    }

    public static class MeshFilterSerializer
    {
        #region MeshFilter
        public static string ToJSON(this MeshFilter mf)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("{\n");
            tabLevel++;
            {
                tabLevel++;
                sb.Append(Tabs()).Append("\"Mesh\"").Append(" : ").Append(mf.sharedMesh.ToJSON());
                tabLevel--;
            }
            tabLevel--;
            sb.Append(Tabs()).AppendLine("}");
            return sb.ToString();
        }
        #endregion MeshFilter
    }

    public static class MeshSerializer
    {
        #region Mesh
        public static string ToJSON(this Mesh mesh)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{\n");
            tabLevel++;
            {
                // name
                sb.Append(Tabs()).Append("\"name\"").Append(" : ").Append("\"").Append(mesh.name).Append("\"").Append(",\n");

                // Vertices
                sb.Append(Tabs()).Append("\"Vertices\"").Append(" : [\n");
                tabLevel++;
                {
                    Vector3[] vertices = mesh.vertices;

                    foreach (Vector3 item in vertices)
                    {
                        sb.Append(Tabs()).Append(item.ToJSON()).Append(",\n");
                    }
                    if (vertices.Length > 0)
                        sb.Remove(sb.Length - 2, 1);
                }
                tabLevel--;
                sb.Append(Tabs()).Append("],\n");

                // Colors
                sb.Append(Tabs()).Append("\"Colors\"").Append(" : [\n");
                tabLevel++;
                {
                    Color[] colors = mesh.colors;

                    foreach (Color item in colors)
                    {
                        sb.Append(Tabs()).Append(item.ToJSON()).Append(",\n");
                    }
                    if (colors.Length > 0)
                        sb.Remove(sb.Length - 2, 1);
                }
                tabLevel--;
                sb.Append(Tabs()).Append("],\n");


                // Tangents
                sb.Append(Tabs()).Append("\"Tangents\"").Append(" : [\n");
                tabLevel++;
                {
                    Vector4[] tangents = mesh.tangents;

                    foreach (Vector4 item in tangents)
                    {
                        sb.Append(Tabs()).Append(item.ToJSON()).Append(",\n");
                    }
                    if (tangents.Length > 0)
                        sb.Remove(sb.Length - 2, 1);
                }
                tabLevel--;
                sb.Append(Tabs()).Append("],\n");

                // Normals
                sb.Append(Tabs()).Append("\"Normals\"").Append(" : [\n");
                tabLevel++;
                {
                    Vector3[] normals = mesh.normals;

                    foreach (Vector3 item in normals)
                    {
                        sb.Append(Tabs()).Append(item.ToJSON()).Append(",\n");
                    }
                    if (normals.Length > 0)
                        sb.Remove(sb.Length - 2, 1);
                }
                tabLevel--;
                sb.Append(Tabs()).Append("],\n");

                // UVs
                sb.Append(Tabs()).Append("\"UV\"").Append(" : [\n");
                tabLevel++;
                {
                    Vector2[] uv = mesh.uv;

                    foreach (Vector2 item in uv)
                    {
                        sb.Append(Tabs()).Append(item.ToJSON()).Append(",\n");
                    }
                    if (uv.Length > 0)
                        sb.Remove(sb.Length - 2, 1);
                }
                tabLevel--;
                sb.Append(Tabs()).Append("],\n");

                // UV2s
                sb.Append(Tabs()).Append("\"UV2\"").Append(" : [\n");
                tabLevel++;
                {
                    Vector2[] uv2 = mesh.uv2;

                    foreach (Vector2 item in uv2)
                    {
                        sb.Append(Tabs()).Append(item.ToJSON()).Append(",\n");
                    }
                    if (uv2.Length > 0)
                        sb.Remove(sb.Length - 2, 1);
                }
                tabLevel--;
                sb.Append(Tabs()).Append("],\n");

                // UV3s
                sb.Append(Tabs()).Append("\"UV3\"").Append(" : [\n");
                tabLevel++;
                {
                    Vector2[] uv3 = mesh.uv3;

                    foreach (Vector2 item in uv3)
                    {
                        sb.Append(Tabs()).Append(item.ToJSON()).Append(",\n");
                    }
                    if (uv3.Length > 0)
                        sb.Remove(sb.Length - 2, 1);
                }
                tabLevel--;
                sb.Append(Tabs()).Append("],\n");

                // UV4s
                sb.Append(Tabs()).Append("\"UV4\"").Append(" : [\n");
                tabLevel++;
                {
                    Vector2[] uv4 = mesh.uv3;

                    foreach (Vector2 item in uv4)
                    {
                        sb.Append(Tabs()).Append(item.ToJSON()).Append(",\n");
                    }
                    if (uv4.Length > 0)
                        sb.Remove(sb.Length - 2, 1);
                }
                tabLevel--;
                sb.Append(Tabs()).Append("],\n");

                // Submeshes
                sb.Append(Tabs()).Append("\"subMeshCount\"").Append(" : ").Append(mesh.subMeshCount).Append(",\n");

                // Triangles
                sb.Append(Tabs()).Append("\"Submeshes\"").Append(" : [\n");
                tabLevel++;
                {
                    for (int i = 0; i < mesh.subMeshCount; i++)
                    {
                        sb.Append(Tabs()).Append("\"Submesh" + i + "\"").Append(" : [");
                        tabLevel++;
                        {
                            int[] indices = mesh.GetTriangles(i);

                            foreach (int item in indices)
                            {
                                sb.Append(item).Append(",");
                            }
                            if (indices.Length > 0)
                                sb.Remove(sb.Length - 1, 1);
                        }
                        tabLevel--;
                        sb.Append(Tabs()).Append("],\n");
                    }
                    if (mesh.subMeshCount > 0)
                        sb.Remove(sb.Length - 2, 1);
                }
                tabLevel--;
                sb.Append(Tabs()).Append("],\n");

            }
            tabLevel--;
            sb.Append(Tabs()).Append("}");
            return sb.ToString();
        }
        #endregion Mesh
    }

    public static class MeshRendererSerializer
    {
        #region MeshRenderer
        public static string ToJSON(this MeshRenderer mr)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{\n");
            tabLevel++;
            {
                //Material[]mr.materials


                sb.Append(Tabs()).Append("\"MeshRenderer\"").Append(" : ").Append("[\n");
                tabLevel++;
                sb.Append(Tabs()).Append("IMPLEMENT THIS!");
                tabLevel--;
                sb.Append(Tabs()).Append("]");
            }
            tabLevel--;
            sb.Append(Tabs()).AppendLine("}\n");
            return sb.ToString();
        }
        #endregion MeshRenderer
    }
}
