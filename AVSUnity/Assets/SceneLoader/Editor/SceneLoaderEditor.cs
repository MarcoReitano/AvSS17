using System.Diagnostics;
using System.IO;

using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

using Debug = UnityEngine.Debug;

[CustomEditor(typeof(SceneLoader))]
public class SceneLoaderEditor : Editor
{

    SceneLoader sceneLoader;
    // Use this for initialization
    void Awake()
    {
        this.sceneLoader = target as SceneLoader;
    }


    Scene newScene;
    Scene mainScene;
    private static int sceneCount = 0;
    public override void OnInspectorGUI()
    {
        GUILayout.Label("relativeAssetPath:" + RelativeAssetPath());
        if (GUILayout.Button("Create Scene"))
        {
            //SceneManager
            newScene = EditorSceneManager.NewScene(
                NewSceneSetup.EmptyScene,
                NewSceneMode.Additive);

            mainScene = EditorSceneManager.GetActiveScene();
            EditorSceneManager.SetActiveScene(this.newScene);

            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);

        }

        if (GUILayout.Button("Save Scene"))
        {
            string absolutepath = Application.dataPath;
            string relativepath = "Assets" + absolutepath.Substring(Application.dataPath.Length);



            EditorSceneManager.SaveScene(
                newScene,
                   relativepath + "/newScene" + sceneCount++ + ".unity");

            EditorSceneManager.SetActiveScene(this.mainScene);
        }


        if (GUILayout.Button("Generate Grid"))
        {
            for (int x = 0; x < 3; x++)
            {
                for (int z = 0; z < 3; z++)
                {
                    mainScene = EditorSceneManager.GetActiveScene();

                    newScene = EditorSceneManager.NewScene(
                        NewSceneSetup.EmptyScene,
                        NewSceneMode.Additive);

                    EditorSceneManager.SetActiveScene(this.newScene);

                    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.transform.position = new Vector3(x, 0f, z);

                    string absolutePath = Application.dataPath;
                    string relativePath = "Assets" + absolutePath.Substring(Application.dataPath.Length);


                    EditorSceneManager.SaveScene(
                        newScene,
                        relativePath + "/Scene_" + x + "_" + z + ".unity");

                    EditorSceneManager.SetActiveScene(this.mainScene);
                    EditorSceneManager.CloseScene(this.newScene, true);

                }
            }
        }


        if (GUILayout.Button("Load Grid"))
        {
            for (int x = 0; x < 3; x++)
            {
                for (int z = 0; z < 3; z++)
                {
                    string relativePath =
                        "Assets" + Application.dataPath.Substring(Application.dataPath.Length) +
                        "/Scene_" + x + "_" + z + ".unity";

                    Scene sceneToLoad = EditorSceneManager.OpenScene(relativePath, OpenSceneMode.Additive);
                }
            }
        }

        if (GUILayout.Button(("Merge Scenes to New")))
        {
            this.MergeScenesToNew();
        }


        if (GUILayout.Button("Generate, transfer, load Scenes"))
        {
            Stopwatch sw = new Stopwatch();

            for (int x = 0; x < 3; x++)
            {
                for (int z = 0; z < 3; z++)
                {
                    sw.Start();

                    // Aktuelle Szene als MainScene merken
                    mainScene = EditorSceneManager.GetActiveScene();

                    // Neue (leere) Szene erstellen
                    newScene = EditorSceneManager.NewScene(
                        NewSceneSetup.EmptyScene,
                        NewSceneMode.Additive);

                    // Neue Szene als aktive Szene setzen
                    EditorSceneManager.SetActiveScene(this.newScene);

                    //###########################
                    // Erzeuge Content:
                    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.transform.position = new Vector3(x, 0f, z);
                    //###########################

                    // Szene speichern
                    string filename = RelativeAssetPathTo("Scene_" + x + "_" + z + ".unity");
                    EditorSceneManager.SaveScene(newScene, filename);

                    // ByteArray für Message aus Szene erstellen
                    byte[] bytes = SceneFileToByteArray(this.newScene);

                    // Filename must be send in some form... 
                    //
                    //      |
                    //      |
                    // Transfer via RabbitMQ-Message
                    //      |
                    //     \|/
                    //      v

                    Scene transferedScene = ByteArrayToScene(filename, bytes);
                    EditorSceneManager.SetActiveScene(transferedScene);
                    EditorSceneManager.CloseScene(this.newScene, true);
                    sw.Stop();
                    Debug.Log("Szene " + x + "," + z + " took: " + sw.ElapsedMilliseconds + "ms");

                }
            }
        }
    }

    private void MergeScenesToNew()
    {
        this.newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);

        EditorSceneManager.SetActiveScene(this.newScene);

        for (int x = 0; x < 3; x++)
        {
            for (int z = 0; z < 3; z++)
            {
                string relativePath = "Assets" + Application.dataPath.Substring(Application.dataPath.Length) + "/Scene_" + x + "_" + z + ".unity";

                Scene sceneToLoad = EditorSceneManager.OpenScene(RelativeAssetPathTo("Scene_" + x + "_" + z + ".unity"), OpenSceneMode.Additive);

                EditorSceneManager.MergeScenes(sceneToLoad, this.newScene);
            }
        }

        EditorSceneManager.SaveScene(this.newScene, RelativeAssetPath() + "/MergedScene.unity");
    }

    public byte[] SceneFileToByteArray(Scene scene)
    {
        Debug.Log("datapath: " + Application.dataPath);
        Debug.Log("scene.path: " + scene.path);
        return File.ReadAllBytes(Application.dataPath + "/" + scene.path.Substring("/Assets".Length));
    }


    public Scene ByteArrayToScene(string filename, byte[] bytes)
    {
        ByteArrayToSceneFile(filename, bytes);
        return EditorSceneManager.OpenScene(filename, OpenSceneMode.Additive);
    }

    public void ByteArrayToSceneFile(string filename, byte[] bytes)
    {
        File.WriteAllBytes(Application.dataPath + "/test_" + filename.Substring("/Assets".Length), bytes);
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="filename"></param>
    /// <returns></returns>
    private static string RelativeAssetPathTo(string filename)
    {
        return RelativeAssetPath() + "/" + filename;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private static string RelativeAssetPath()
    {
        return "Assets" + Application.dataPath.Substring(Application.dataPath.Length);
    }
}
