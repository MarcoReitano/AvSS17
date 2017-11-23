using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

#if UNITY_EDITOR
using UnityEditor;
#endif

public static class GameObjectExtensions
{
    /// <summary>
    /// Creates a GameObject as child of the given parent-Object
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="components"></param>
    /// <returns></returns>
    public static GameObject CreateChild(this GameObject parent, params System.Type[] components)
    {
        return parent.CreateChild("Child of " + parent.name, components);
    }

    /// <summary>
    ///  Creates a GameObject with the given name as child of the given parent-Object
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="name"></param>
    /// <param name="components"></param>
    /// <returns></returns>
    public static GameObject CreateChild(this GameObject parent, string name, params System.Type[] components)
    {
        GameObject child = new GameObject(name, components);
        child.transform.parent = parent.transform;

        return child;
    }


    /// <summary>
    /// Creates a GameObject as child of the given parent-Object
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="components"></param>
    /// <returns></returns>
    public static GameObject CreateRenderableChild(this GameObject parent, params System.Type[] components)
    {
        GameObject child = parent.CreateChild("Child of " + parent.name, components);
        child.MakeRenderable();
        return child;
    }

    /// <summary>
    ///  Creates a GameObject with the given name as child of the given parent-Object
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="name"></param>
    /// <param name="components"></param>
    /// <returns></returns>
    public static GameObject CreateRenderableChild(this GameObject parent, string name, params System.Type[] components)
    {
        GameObject child = parent.CreateChild(name, components);
        child.MakeRenderable();
        return child;
    }



    /// <summary>
    /// Ensure that the GameObject has a MeshFilter- and MeshRenderer-Component
    /// </summary>
    /// <param name="gameObject"></param>
    public static void MakeRenderable(this GameObject gameObject)
    {
        gameObject.TryGetMeshFilter();
        gameObject.TryGetMeshRenderer();
    }

    public static void SetVisible(this GameObject gameObject, bool visible)
    {
        MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
        if (meshRenderer != null)
            meshRenderer.enabled = visible;
    }

    public static void SetVisibleRecursively(this GameObject gameObject, bool visible)
    {
        MeshRenderer[] meshRenders = gameObject.GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer meshRenderer in meshRenders)
            meshRenderer.enabled = visible;
    }

    public static void SetVisible(this List<GameObject> gameObjects, bool visible)
    {
        foreach (GameObject go in gameObjects)
        {
            go.SetVisible(visible);
        }
    }

    public static void SetVisibleRecursively(this List<GameObject> gameObjects, bool visible)
    {
        foreach (GameObject go in gameObjects)
        {
            go.SetVisibleRecursively(visible);
        }
    }

    /// <summary>
    /// Another usefull Gameobject Extension
    /// </summary>
    public static MeshFilter TryGetMeshFilter(this GameObject gameObject)
    {
        return gameObject.TryGetComponent<MeshFilter>();
    }

    /// <summary>
    /// Another usefull Gameobject Extension
    /// </summary>
    public static MeshRenderer TryGetMeshRenderer(this GameObject gameObject)
    {
        return gameObject.TryGetComponent<MeshRenderer>();
    }


    /// <summary>
    /// This Method tries to get the Component of Type <typeparam name="T"></typeparam>.
    /// If there is none it addes the Component to the GameObject
    /// Another usefull Gameobject Extension
    /// </summary>
    public static T TryGetComponent<T>(this GameObject gameObject) where T : Component
    {
        T component = gameObject.GetComponent<T>();
        if (component == null)
            component = gameObject.AddComponent<T>();
        return component;
    }

    /// <summary>
    /// This Method deletes all child-objects the GameObject
    /// </summary>
    /// <param name="gameObject"></param>
    public static void DeleteChildren(this GameObject gameObject)
    {
        List<GameObject> children = gameObject.GetChildGameObjects();

#if UNITY_EDITOR
        foreach (GameObject child in children)
            GameObject.DestroyImmediate(child);
#else
		foreach (GameObject child in children)
            GameObject.DestroyImmediate(child);
			//GameObject.Destroy(child);
#endif

    }

    public static List<GameObject> GetChildGameObjects(this GameObject gameObject)
    {
        Transform[] childTransforms = gameObject.GetComponentsInChildren<Transform>();
        List<GameObject> children = new List<GameObject>();
        foreach (Transform child in childTransforms)
        {
            if (child.gameObject != gameObject)
                children.Add(child.gameObject);
        }
        return children;
    }

    public static List<GameObject> GetChildGameObjectsWithParent(this GameObject gameObject)
    {
        Transform[] childTransforms = gameObject.GetComponentsInChildren<Transform>();
        List<GameObject> children = new List<GameObject>();
        foreach (Transform child in childTransforms)
        {
            children.Add(child.gameObject);
        }
        return children;
    }

    public static bool HasChildNamed(this GameObject gameObject, string name)
    {
        bool result = false;

        foreach (GameObject go in gameObject.GetChildGameObjects())
        {
            if (go.name == name)
                return true;
        }

        return result;
    }

    public static bool HasChildNamed(this GameObject gameObject, string name, out GameObject child)
    {
        bool result = false;
        child = null;

        foreach (GameObject go in gameObject.GetChildGameObjects())
        {
            if (go.name == name)
            {
                child = go;
                return true;
            }
        }

        return result;
    }

    public static int ChildCount(this GameObject gameObject)
    {
        return gameObject.GetChildGameObjects().Count;
    }

    public static int ChildCountFirstLevel(this GameObject gameObject)
    {
        int result = 0;

        foreach (GameObject child in gameObject.GetChildGameObjects())
        {
            if (child.transform.parent == gameObject.transform)
                result++;
        }

        return result;
    }

    public static void DestroyIfEmpty(this GameObject gameObject)
    {

        int childCount = gameObject.transform.childCount;

        int componentCount = gameObject.GetComponents<Component>().Length;

        Debug.Log("Delete GameObject if empty");
        if (componentCount == 1 && childCount == 0)
        {
            Debug.Log("Delete GameObject if empty:  is EMPTY! --> Delete");
            GameObject.DestroyImmediate(gameObject);
        }

    }


    /// <summary>
    /// 
    /// </summary>
    public static Material dummyMaterial = new Material(Shader.Find("Transparent/Diffuse"));


    /// <summary>
    /// 
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="mesh"></param>
    /// <returns></returns>
    public static GameObject Create3DObject(this GameObject gameObject, Mesh mesh)
    {
        if (gameObject == null)
            gameObject = new GameObject();

        MeshFilter meshFilter = gameObject.TryGetComponent<MeshFilter>();
        meshFilter.sharedMesh = mesh;

        MeshRenderer meshRenderer = gameObject.TryGetComponent<MeshRenderer>();
        Material[] materials = new Material[meshFilter.sharedMesh.subMeshCount];
        for (int i = 0; i < materials.Length; i++)
            materials[i] = dummyMaterial;

        meshRenderer.materials = materials;

        MeshCollider meshCollider = gameObject.TryGetComponent<MeshCollider>();
        //meshCollider.mesh = mesh;

        return gameObject;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>

#if UNITY_EDITOR
    [MenuItem("GameObject/Create Other/SimplePlane")]
#endif
    public static GameObject CreateSimplePlaneMesh()
    {
        GameObject simplePlane = new GameObject("SimplePlane");
        simplePlane.MakeRenderable();
        MeshFilter meshfilter = simplePlane.GetComponent<MeshFilter>();
        meshfilter.mesh = MeshHelper.CreateSimplePlaneMesh(10f, 10f);
        simplePlane.GetComponent<MeshRenderer>().material = MeshHelper.DummyMaterial;
        return simplePlane;
    }


    /// <summary>
    /// Find integer log base 10 of an integer the obvious way
    /// This method works well when the input is uniformly distributed over 32-bit values because 76% of the inputs are caught by the first compare, 21% are caught by the second compare, 2% are caught by the third, and so on (chopping the remaining down by 90% with each comparision). As a result, less than 2.6 operations are needed on average.
    ///
    /// On April 18, 2007, Emanuel Hoogeveen suggested a variation on this where the conditions used 
    /// divisions, which were not as fast as simple comparisons. 
    /// 
    /// customized after: 
    /// http://graphics.stanford.edu/~seander/bithacks.html#IntegerLog10Obvious
    /// </summary>
    /// <param name="non-zero 32-bit integer value to compute the log base 10 of "></param>
    /// <returns></returns>
    public static int NumberOfDigits(this int value)
    {
        if (value < 0)
            value *= -1;

        int result =
            (value >= 1000000000) ? 9 :
            (value >= 100000000) ? 8 :
            (value >= 10000000) ? 7 :
            (value >= 1000000) ? 6 :
            (value >= 100000) ? 5 :
            (value >= 10000) ? 4 :
            (value >= 1000) ? 3 :
            (value >= 100) ? 2 :
            (value >= 10) ? 1 : 0;

        return result;
    }

#if UNITY_EDITOR
    public static void SelectInEditor(this GameObject go)
    {
        UnityEngine.Object[] objects = new UnityEngine.Object[1];
        objects[0] = go;
        Selection.objects = objects;
        Selection.activeGameObject = go;
    }
#endif

    /// <summary>
    /// 
    /// </summary>
    /// <param name="go"></param>
    /// <returns></returns>
    public static bool IsDestroyed(this GameObject go)
    {
        return (go == null);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="go"></param>
    /// <returns></returns>
    public static bool IsAlive(this GameObject go)
    {
        return (go != null);
    }

#if UNITY_EDITOR
    public static bool hideDefaultInspectorFoldout = false;
    public static bool DrawDefaultInspectorFoldout(this Editor customEditor, MonoBehaviour behaviour, bool showDefaultInspector)
    {
        // Globaly turned the DefaultInspectorFoldout on or off
        if (hideDefaultInspectorFoldout)
            return false;

        if (showDefaultInspector = EditorGUILayout.Foldout(showDefaultInspector, behaviour.GetType().ToString() + "-Default-Inspector") && behaviour != null)
        {
            CustomGUIUtils.BeginGroup();
            customEditor.DrawDefaultInspector();
            CustomGUIUtils.EndGroup();
        }
        return showDefaultInspector;
    }
#endif
}
