//using System.Collections;
//using System.Collections.Generic;
//using System.Text;
//using UnityEngine;
//using UnityEngine.SceneManagement;


//[System.Serializable]
//public class SceneSerializer
//{

//    public class SerializedGameObject
//    {
//        public Transform transform;
//        public List<System.Type> components;

//        public SerializedGameObject(GameObject go)
//        {
//            Component[] components = go.GetComponents<Component>();

//            foreach (Component item in components)
//            {
//                System.Type type = item.GetType();
//                Debug.Log(go.name + ": " + type);

//                if (type == typeof(Transform))
//                {
//                    // if transform cannot be serialized
//                }
//                if (type == typeof(MeshFilter))
//                {
//                    // Mesh serialisieren
//                }
//                else if (type == typeof(MeshRenderer))
//                {
//                    // Materials-Array serialisieren

//                }
//            }


//        }
//    }

//    [System.Serializable]
//    public class SerializedTransform
//    {
//        public Vector3 position;

//        public Vector3 scale;
//    }

//    [System.Serializable]
//    public class SerializedMeshFilter
//    {
   
//    }

//    [System.Serializable]
//    public class SerializedMeshRenderer
//    {

//    }

//    // SerializedScene soll eine Liste mit allen serialized Meshes enthalten
//    // Das SerializedMesh soll sowohl Mesh als auch Materialien enthalten.
//    //


//    // GameObject
//    //     Transform Serialisieren
//    //     Mesh serialisieren
//    //     Materialien Serialisieren

//    // Terrain Serialisieren
//    //      Welche Attribute?
//    //      nur heightmap?

//    public static string SerializeScene(Scene scene)
//    {
//        // Mesh serialisieren --> wie?
//        //     Mesh serializable?
//        //     ansonsten --> Vertex-Array / Triangle-Array(s) / UVs
//        //     Mesh und/oder Scene WrapperKlasse die Serializable ist
//        // Alle Objekte auf root-Ebene der Szene traversieren
//        foreach (GameObject item in scene.GetRootGameObjects())
//        {

//            // Prüfen ob Mesh vorhanden
//            foreach (Transform transform in item.GetComponentsInChildren<Transform>())
//            {
//                SerializedGameObject serGo = new SerializedGameObject(transform.gameObject);


//                MeshFilter mf = transform.GetComponent<MeshFilter>();
//                if (mf != null)
//                {
//                    Mesh mesh = mf.sharedMesh;

//                    byte[] serializedMesh = MeshSerializer.WriteMesh(mesh, false);

//                    Debug.Log(transform.name + ": SerializedMesh has " + serializedMesh.Length + " bytes");
//                }
//            }
//        }
//        return "It works!";



//    }



//}



//public static class SerializerHelper
//{
//    #region Helper
//    public static int tabLevel = 0;

//    public static string Tabs()
//    {
//        return new string(' ', tabLevel * 4);
//    }
//    #endregion Helper

//    public class JValue
//    {
//        public void Parse(string json)
//        {
//            //string value = json.Trim();
//            //switch ((char) value[0])
//            //{
//            //    case '{': // object
//            //         break;
//            //    case '[': // array
//            //        break;
                
//            //    case '\"': // string
//            //        break;
//            //    case 't': // true
//            //        break;
//            //    case 'f': // false
//            //        break;
//            //    case 'n': // null
//            //        break;
//            //    case '-': // number
//            //        break;
//            //    case '-': // number 0
//            //        break;
//            //    case '-': // number 1
//            //        break;
//            //    case '-': // number 2
//            //        break;
//            //    case '-': // number 3
//            //        break;
//            //    case '-': // number 4
//            //        break;
//            //    case '-': // number 5
//            //        break;
//            //    case '-': // number 6
//            //        break;
//            //    case '-': // number 7
//            //        break;
//            //    case '-': // number 8
//            //        break;
//            //    case '-': // number 9
//            //        break;
//            //    default:
//            //        break;
//            //}


//        }
//    }



//    //private static List<JValue> Tokenize(this string value)
//    //{
//    //    List<string> result;

//    //}


//    private static List<string> Tokenize(string value, char seperator)
//    {
//        List<string> result = new List<string>();
//        value = value.Replace("  ", " ").Replace("  ", " ").Trim();
//        value = value.Replace("\n", " ").Replace("  ", " ").Trim();
//        StringBuilder sb = new StringBuilder();
//        bool insideQuote = false;
//        foreach (char c in value.ToCharArray())
//        {
//            if (c == '"')
//            {
//                insideQuote = !insideQuote;
//            }
//            if ((c == seperator) && !insideQuote)
//            {
//                if (sb.ToString().Trim().Length > 0)
//                {
//                    result.Add(sb.ToString().Trim());
//                    sb = new StringBuilder();
//                }
//            }
//            else
//            {
//                sb.Append(c);
//            }
//        }
//        if (sb.ToString().Trim().Length > 0)
//        {
//            result.Add(sb.ToString().Trim());
//        }

//        return result;
//    }

//    public class JObject
//    {

//    }

//    public class JArray
//    {
//    }

//    public class JNumber
//    {
//    }

//    public class JString
//    {
//    }


//}

//public static class SceneExtensions
//{
//    #region Scene
//    public static string ToJSON(this Scene scene)
//    {
//        StringBuilder sb = new StringBuilder();

//        sb.Append(SerializerHelper.Tabs()).Append("{\n");
//        SerializerHelper.tabLevel++;
//        {
//            sb.Append(SerializerHelper.Tabs()).Append("\"Scene\" : {\n");
//            SerializerHelper.tabLevel++;
//            {
//                sb.Append(SerializerHelper.Tabs()).Append("\"name\"").Append(" : ").Append("\"").Append(scene.name).Append("\"").Append(",\n");
//                sb.Append(SerializerHelper.Tabs()).Append("\"rootGameObjects\"").Append(" : [\n");

//                SerializerHelper.tabLevel++;
//                {
//                    foreach (GameObject item in scene.GetRootGameObjects())
//                    {
//                        sb.Append(SerializerHelper.Tabs()).Append("\"GameObject\" : ").Append(item.ToJSON()).Append(",\n");
//                    }
//                    if (scene.GetRootGameObjects().Length > 0)
//                        sb.Remove(sb.Length - 2, 1);
//                }
//                SerializerHelper.tabLevel--;
//                sb.Append(SerializerHelper.Tabs()).Append("]\n");
//            }
//            SerializerHelper.tabLevel--;
//            sb.Append(SerializerHelper.Tabs()).Append("}\n");
//        }
//        SerializerHelper.tabLevel--;
//        sb.Append(SerializerHelper.Tabs()).Append("}");
//        return sb.ToString();
//        //return SceneSerializer.SerializeScene(scene);
//    }

//    //public static Scene FromJSON(string json)
//    //{
        
//    //    SceneManager.CreateScene(name);

//    //    return null;
//    //}
//    #endregion Scene
//}

//public static class GameObjectSerializer
//{
//    #region GameObject
//    public static string ToJSON(this GameObject go)
//    {
//        StringBuilder sb = new StringBuilder();
//        sb.Append("{\n");
//        SerializerHelper.tabLevel++;
//        {
//            sb.Append(SerializerHelper.Tabs()).Append("\"name\"").Append(" : ").Append("\"").Append(go.name).Append("\"").Append(",\n");

//            sb.Append(SerializerHelper.Tabs()).Append("\"Components\"").Append(" : [\n");

//            SerializerHelper.tabLevel++;
//            {
//                Component[] components = go.GetComponents<Component>();

//                foreach (Component item in components)
//                {
//                    System.Type type = item.GetType();

//                    if (type == typeof(Transform))
//                    {
//                        sb.Append(SerializerHelper.Tabs()).Append("\"Transform\"").Append(" : ").Append(((Transform)item).ToJSON()).Append(",\n");
//                    }
//                    if (type == typeof(MeshFilter))
//                    {
//                        sb.Append(SerializerHelper.Tabs()).Append("\"MeshFilter\"").Append(" : ").Append(((MeshFilter)item).ToJSON()).Append(",\n");
//                    }
//                    else if (type == typeof(MeshRenderer))
//                    {
//                        sb.Append(SerializerHelper.Tabs()).Append("\"MeshRenderer\"").Append(" : ").Append(((MeshRenderer)item).ToJSON()).Append(",\n");
//                    }
//                }
//                if (components.Length > 0)
//                    sb.Remove(sb.Length - 2, 1);
//            }
//            SerializerHelper.tabLevel--;
//            sb.Append(SerializerHelper.Tabs()).Append("],\n");

//            // Children
//            List<GameObject> realChildren = new List<GameObject>();
//            foreach (Transform child in go.GetComponentsInChildren<Transform>())
//            {
//                if (child.gameObject == go)
//                    continue;
//                if (child.parent != go.transform)
//                    continue;
//                else
//                    realChildren.Add(child.gameObject);
//            }
//            Debug.Log(go.name + ": " + realChildren.Count);
//            if (realChildren.Count > 0)
//            {
//                sb.Append(SerializerHelper.Tabs()).Append("\"Children\"").Append(" : ").Append("[");
//                {
//                    SerializerHelper.tabLevel++;
//                    foreach (GameObject child in realChildren)
//                    {
//                        sb.Append("\"GameObject\" : ").Append(child.ToJSON()).Append(",\n");
//                    }
//                    SerializerHelper.tabLevel--;
//                    if (realChildren.Count > 0)
//                        sb.Remove(sb.Length - 2, 1);
//                }
//                sb.Append(SerializerHelper.Tabs()).Append("]\n");
//            }
//            else
//            {
//                sb.Remove(sb.Length - 2, 1);
//            }
//        }
//        SerializerHelper.tabLevel--;
//        sb.Append(SerializerHelper.Tabs()).Append("}");
//        return sb.ToString();
//    }
//    #endregion GameObject
//}

//public static class QuaternionSerializer
//{
//    #region Quaternion
//    public static string ToJSON(this Quaternion quat)
//    {
//        StringBuilder sb = new StringBuilder();
//        sb.Append("{\n");
//        SerializerHelper.tabLevel++;
//        {
//            sb.Append(SerializerHelper.Tabs()).Append("\"x\"").Append(" : ").Append(quat.x).Append(",\n");
//            sb.Append(SerializerHelper.Tabs()).Append("\"y\"").Append(" : ").Append(quat.y).Append(",\n");
//            sb.Append(SerializerHelper.Tabs()).Append("\"z\"").Append(" : ").Append(quat.z).Append(",\n");
//            sb.Append(SerializerHelper.Tabs()).Append("\"w\"").Append(" : ").Append(quat.w).Append("\n");
//        }
//        SerializerHelper.tabLevel--;
//        sb.Append(SerializerHelper.Tabs()).Append("}");
//        return sb.ToString();
//    }

//    //public static Quaternion FromJSON(string json)
//    //{

//    //    Quaternion quat = new Quaternion(;
//    //    return Quaternion.;
//    //}
//    #endregion Quaternion
//}

//public static class Vector3Serializer
//{
//    #region Vector3
//    public static string ToJSON(this Vector3 vec3)
//    {
//        StringBuilder sb = new StringBuilder();

//        sb.Append("{\n");
//        SerializerHelper.tabLevel++;
//        {
//            sb.Append(SerializerHelper.Tabs()).Append("\"x\"").Append(" : ").Append(vec3.x).Append(",\n");
//            sb.Append(SerializerHelper.Tabs()).Append("\"y\"").Append(" : ").Append(vec3.y).Append(",\n");
//            sb.Append(SerializerHelper.Tabs()).Append("\"z\"").Append(" : ").Append(vec3.z).Append("\n");
//        }
//        SerializerHelper.tabLevel--;
//        sb.Append(SerializerHelper.Tabs()).Append("}");
//        return sb.ToString();
//    }
//    #endregion Vector3
//}

//public static class Vector4Serializer
//{
//    #region Vector4
//    public static string ToJSON(this Vector4 vec4)
//    {
//        StringBuilder sb = new StringBuilder();

//        sb.Append("{\n");
//        SerializerHelper.tabLevel++;
//        {
//            sb.Append(SerializerHelper.Tabs()).Append("\"x\"").Append(" : ").Append(vec4.x).Append(",\n");
//            sb.Append(SerializerHelper.Tabs()).Append("\"y\"").Append(" : ").Append(vec4.y).Append(",\n");
//            sb.Append(SerializerHelper.Tabs()).Append("\"z\"").Append(" : ").Append(vec4.z).Append(",\n");
//            sb.Append(SerializerHelper.Tabs()).Append("\"w\"").Append(" : ").Append(vec4.w).Append("\n");
//        }
//        SerializerHelper.tabLevel--;
//        sb.Append(SerializerHelper.Tabs()).Append("}");
//        return sb.ToString();
//    }
//    #endregion Vector4
//}

//public static class ColorSerializer
//{
//    #region Color
//    public static string ToJSON(this Color color)
//    {
//        StringBuilder sb = new StringBuilder();

//        sb.Append("{\n");
//        SerializerHelper.tabLevel++;
//        {
//            sb.Append(SerializerHelper.Tabs()).Append("\"r\"").Append(" : ").Append(color.r).Append(",\n");
//            sb.Append(SerializerHelper.Tabs()).Append("\"g\"").Append(" : ").Append(color.g).Append(",\n");
//            sb.Append(SerializerHelper.Tabs()).Append("\"b\"").Append(" : ").Append(color.b).Append(",\n");
//            sb.Append(SerializerHelper.Tabs()).Append("\"a\"").Append(" : ").Append(color.a).Append("\n");
//        }
//        SerializerHelper.tabLevel--;
//        sb.Append(SerializerHelper.Tabs()).Append("}");
//        return sb.ToString();
//    }
//    #endregion Color
//}

//public static class Vector2Serializer
//{
//    #region Vector2
//    public static string ToJSON(this Vector2 vec2)
//    {
//        StringBuilder sb = new StringBuilder();
//        sb.Append("{\n");
//        SerializerHelper.tabLevel++;
//        {
//            sb.Append(SerializerHelper.Tabs()).Append("\"x\"").Append(" : ").Append(vec2.x).Append(",\n");
//            sb.Append(SerializerHelper.Tabs()).Append("\"y\"").Append(" : ").Append(vec2.y).Append("\n");
//        }
//        SerializerHelper.tabLevel--;
//        sb.Append(SerializerHelper.Tabs()).Append("}");
//        return sb.ToString();
//    }
//    #endregion Vector2
//}

//public static class TransformSerializer
//{
//    #region Transform
//    public static string ToJSON(this Transform transform)
//    {
//        StringBuilder sb = new StringBuilder();
//        sb.Append("{\n");
//        SerializerHelper.tabLevel++;
//        {
//            sb.Append(SerializerHelper.Tabs()).Append("\"Position\"").Append(" : ").Append(transform.position.ToJSON()).Append(",\n");
//            sb.Append(SerializerHelper.Tabs()).Append("\"Rotation\"").Append(" : ").Append(transform.localRotation.ToJSON()).Append(",\n");
//            sb.Append(SerializerHelper.Tabs()).Append("\"Scale\"").Append(" : ").Append(transform.localScale.ToJSON()).Append("\n");
//        }
//        SerializerHelper.tabLevel--;
//        sb.Append(SerializerHelper.Tabs()).Append("}");
//        return sb.ToString();
//    }
//    #endregion Transform
//}

//public static class MeshFilterSerializer
//{
//    #region MeshFilter
//    public static string ToJSON(this MeshFilter mf)
//    {
//        StringBuilder sb = new StringBuilder();

//        sb.Append("{\n");
//        SerializerHelper.tabLevel++;
//        {
//            sb.Append(SerializerHelper.Tabs()).Append("\"Mesh\"").Append(" : ").Append(mf.sharedMesh.ToJSON());
//        }
//        SerializerHelper.tabLevel--;
//        sb.Append(SerializerHelper.Tabs()).AppendLine("}");
//        return sb.ToString();
//    }
//    #endregion MeshFilter
//}

//public static class MeshSerializerHelper
//{
//    #region Mesh
//    public static string ToJSON(this Mesh mesh)
//    {
//        StringBuilder sb = new StringBuilder();
//        sb.Append("{\n");
//        SerializerHelper.tabLevel++;
//        {
//            // name
//            sb.Append(SerializerHelper.Tabs()).Append("\"name\"").Append(" : ").Append("\"").Append(mesh.name).Append("\"").Append(",\n");

//            // Vertices
//            sb.Append(SerializerHelper.Tabs()).Append("\"Vertices\"").Append(" : [\n");
//            SerializerHelper.tabLevel++;
//            {
//                Vector3[] vertices = mesh.vertices;

//                foreach (Vector3 item in vertices)
//                {
//                    sb.Append(SerializerHelper.Tabs()).Append(item.ToJSON()).Append(",\n");
//                }
//                if (vertices.Length > 0)
//                    sb.Remove(sb.Length - 2, 1);
//            }
//            SerializerHelper.tabLevel--;
//            sb.Append(SerializerHelper.Tabs()).Append("],\n");

//            // Colors
//            sb.Append(SerializerHelper.Tabs()).Append("\"Colors\"").Append(" : [\n");
//            SerializerHelper.tabLevel++;
//            {
//                Color[] colors = mesh.colors;

//                foreach (Color item in colors)
//                {
//                    sb.Append(SerializerHelper.Tabs()).Append(item.ToJSON()).Append(",\n");
//                }
//                if (colors.Length > 0)
//                    sb.Remove(sb.Length - 2, 1);
//            }
//            SerializerHelper.tabLevel--;
//            sb.Append(SerializerHelper.Tabs()).Append("],\n");


//            // Tangents
//            sb.Append(SerializerHelper.Tabs()).Append("\"Tangents\"").Append(" : [\n");
//            SerializerHelper.tabLevel++;
//            {
//                Vector4[] tangents = mesh.tangents;

//                foreach (Vector4 item in tangents)
//                {
//                    sb.Append(SerializerHelper.Tabs()).Append(item.ToJSON()).Append(",\n");
//                }
//                if (tangents.Length > 0)
//                    sb.Remove(sb.Length - 2, 1);
//            }
//            SerializerHelper.tabLevel--;
//            sb.Append(SerializerHelper.Tabs()).Append("],\n");

//            // Normals
//            sb.Append(SerializerHelper.Tabs()).Append("\"Normals\"").Append(" : [\n");
//            SerializerHelper.tabLevel++;
//            {
//                Vector3[] normals = mesh.normals;

//                foreach (Vector3 item in normals)
//                {
//                    sb.Append(SerializerHelper.Tabs()).Append(item.ToJSON()).Append(",\n");
//                }
//                if (normals.Length > 0)
//                    sb.Remove(sb.Length - 2, 1);
//            }
//            SerializerHelper.tabLevel--;
//            sb.Append(SerializerHelper.Tabs()).Append("],\n");

//            // UVs
//            sb.Append(SerializerHelper.Tabs()).Append("\"UV\"").Append(" : [\n");
//            SerializerHelper.tabLevel++;
//            {
//                Vector2[] uv = mesh.uv;

//                foreach (Vector2 item in uv)
//                {
//                    sb.Append(SerializerHelper.Tabs()).Append(item.ToJSON()).Append(",\n");
//                }
//                if (uv.Length > 0)
//                    sb.Remove(sb.Length - 2, 1);
//            }
//            SerializerHelper.tabLevel--;
//            sb.Append(SerializerHelper.Tabs()).Append("],\n");

//            // UV2s
//            sb.Append(SerializerHelper.Tabs()).Append("\"UV2\"").Append(" : [\n");
//            SerializerHelper.tabLevel++;
//            {
//                Vector2[] uv2 = mesh.uv2;

//                foreach (Vector2 item in uv2)
//                {
//                    sb.Append(SerializerHelper.Tabs()).Append(item.ToJSON()).Append(",\n");
//                }
//                if (uv2.Length > 0)
//                    sb.Remove(sb.Length - 2, 1);
//            }
//            SerializerHelper.tabLevel--;
//            sb.Append(SerializerHelper.Tabs()).Append("],\n");

//            // UV3s
//            sb.Append(SerializerHelper.Tabs()).Append("\"UV3\"").Append(" : [\n");
//            SerializerHelper.tabLevel++;
//            {
//                Vector2[] uv3 = mesh.uv3;

//                foreach (Vector2 item in uv3)
//                {
//                    sb.Append(SerializerHelper.Tabs()).Append(item.ToJSON()).Append(",\n");
//                }
//                if (uv3.Length > 0)
//                    sb.Remove(sb.Length - 2, 1);
//            }
//            SerializerHelper.tabLevel--;
//            sb.Append(SerializerHelper.Tabs()).Append("],\n");

//            // UV4s
//            sb.Append(SerializerHelper.Tabs()).Append("\"UV4\"").Append(" : [\n");
//            SerializerHelper.tabLevel++;
//            {
//                Vector2[] uv4 = mesh.uv3;

//                foreach (Vector2 item in uv4)
//                {
//                    sb.Append(SerializerHelper.Tabs()).Append(item.ToJSON()).Append(",\n");
//                }
//                if (uv4.Length > 0)
//                    sb.Remove(sb.Length - 2, 1);
//            }
//            SerializerHelper.tabLevel--;
//            sb.Append(SerializerHelper.Tabs()).Append("],\n");

//            // Submeshes
//            sb.Append(SerializerHelper.Tabs()).Append("\"subMeshCount\"").Append(" : ").Append(mesh.subMeshCount).Append(",\n");

//            // Triangles
//            sb.Append(SerializerHelper.Tabs()).Append("\"Submeshes\"").Append(" : [\n");
//            SerializerHelper.tabLevel++;
//            {
//                for (int i = 0; i < mesh.subMeshCount; i++)
//                {
//                    sb.Append(SerializerHelper.Tabs()).Append("\"Submesh" + i + "\"").Append(" : [");
//                    SerializerHelper.tabLevel++;
//                    {
//                        int[] indices = mesh.GetTriangles(i);

//                        foreach (int item in indices)
//                        {
//                            sb.Append(item).Append(",");
//                        }
//                        if (indices.Length > 0)
//                            sb.Remove(sb.Length - 1, 1);
//                    }
//                    SerializerHelper.tabLevel--;
//                    sb.Append(SerializerHelper.Tabs()).Append("],\n");
//                }
//                if (mesh.subMeshCount > 0)
//                    sb.Remove(sb.Length - 2, 1);
//            }
//            SerializerHelper.tabLevel--;
//            sb.Append(SerializerHelper.Tabs()).Append("],\n");

//        }
//        SerializerHelper.tabLevel--;
//        sb.Append(SerializerHelper.Tabs()).Append("}");
//        return sb.ToString();
//    }
//    #endregion Mesh
//}

//public static class MeshRendererSerializer
//{
//    #region MeshRenderer
//    public static string ToJSON(this MeshRenderer mr)
//    {
//        StringBuilder sb = new StringBuilder();
//        sb.Append("{\n");
//        SerializerHelper.tabLevel++;
//        {
//            //Material[]mr.materials


//            sb.Append(SerializerHelper.Tabs()).Append("\"MeshRenderer\"").Append(" : ").Append("[\n");
//            SerializerHelper.tabLevel++;
//            sb.Append(SerializerHelper.Tabs()).Append("IMPLEMENT THIS!");
//            SerializerHelper.tabLevel--;
//            sb.Append(SerializerHelper.Tabs()).Append("]");
//        }
//        SerializerHelper.tabLevel--;
//        sb.Append(SerializerHelper.Tabs()).AppendLine("}\n");
//        return sb.ToString();
//    }
//    #endregion MeshRenderer
//}

