using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace DataContractSceneSerialization
{
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
    public class SceneSurrogate
    {
        [DataMember(Name = "Name")]
        public string name;

        [DataMember(Name = "RootGameObjects")]
        public List<GameObjectSurrogate> rootGameObjects;


        public SceneSurrogate(Scene scene)
        {
            this.name = scene.name;

            rootGameObjects = new List<GameObjectSurrogate>();
            foreach (GameObject go in scene.GetRootGameObjects())
            {
                GameObjectSurrogate goSurrogate = new GameObjectSurrogate(go);
                rootGameObjects.Add(goSurrogate);
            }
        }

        public Scene Get()
        {
            Scene scene = SceneManager.GetActiveScene();
            if (Application.isEditor)
            {
#if UNITY_EDITOR
                if (EditorApplication.isPlaying)
                {
                    string sceneName = SceneMessage.CheckForExistingScene(this.name + "_Deserialized");
                    scene = SceneManager.CreateScene(sceneName);
                    SceneManager.SetActiveScene(scene);
                }
                else
                {
                    //SceneMessage.CheckForExistingSceneEditor(this.name + "Deserialized");
                    scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
                    //scene.nthis.name + "Deserialized");
                    EditorSceneManager.SetActiveScene(scene);
                }
#endif   
            }
            else
            {
                string sceneName = SceneMessage.CheckForExistingScene(this.name + "_Deserialized");
                scene = SceneManager.CreateScene(sceneName);
                SceneManager.SetActiveScene(scene);
            }

            foreach (GameObjectSurrogate goSurrogate in rootGameObjects)
            {
                goSurrogate.Get();
            }

            return scene;
        }
    }
}