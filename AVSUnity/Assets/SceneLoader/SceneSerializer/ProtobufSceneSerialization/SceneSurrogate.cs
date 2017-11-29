using System.Collections.Generic;
using System.Runtime.Serialization;
using ProtoBuf;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace ProtobufSceneSerialization
{
    [ProtoContract]
    public class SceneSurrogate
    {
        [ProtoMember(1)]
        public string name;

        [ProtoMember(2)]
        public List<GameObjectSurrogate> rootGameObjects;

        public SceneSurrogate()
        {

        }

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
                    scene = SceneManager.CreateScene(this.name + "Deserialized");
                    SceneManager.SetActiveScene(scene);
                }
                else
                {
                    //scene = EditorSceneManager.CreateScene(this.name + "Deserialized");
                    //EditorSceneManager.SetActiveScene(scene);

                    scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
                    //scene.nthis.name + "Deserialized");
                    EditorSceneManager.SetActiveScene(scene);
                }
#endif   
            }
            else
            {
                scene = SceneManager.CreateScene(this.name + "Deserialized");
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