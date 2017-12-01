using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ProtoBuf;
using UnityEngine;

namespace ProtobufSceneSerialization
{
    [ProtoContract]
    public class GameObjectSurrogate
    {
        [ProtoMember(1)]
        public string name;

        [ProtoMember(2)]
        public List<ComponentSurrogate> components;

        [ProtoMember(3)]
        public List<GameObjectSurrogate> children = new List<GameObjectSurrogate>();

        public GameObjectSurrogate()
        {

        }

        public GameObjectSurrogate(GameObject go)
        {
            this.name = go.name;

            components = new List<ComponentSurrogate>();

            Component[] realComponents = go.GetComponents<Component>();
            foreach (Component item in realComponents)
            {
                System.Type type = item.GetType();

                if (type == typeof(Transform))
                {
                    TransformSurrogate transformSurrogate = new TransformSurrogate(item as Transform);
                    components.Add(transformSurrogate);
                }
                else if (type == typeof(MeshFilter))
                {
                    MeshFilterSurrogate meshFilterSurrogate = new MeshFilterSurrogate(item as MeshFilter);
                    components.Add(meshFilterSurrogate);
                }
                else if (type == typeof(MeshRenderer))
                {
                    MeshRendererSurrogate meshRendererSurrogate = new MeshRendererSurrogate(item as MeshRenderer);
                    components.Add(meshRendererSurrogate);
                }
                else if (type == typeof(Terrain))
                {
                    Debug.Log("Found Terrain");
                    TerrainSurrogate terrainSurrogate = new TerrainSurrogate(item as Terrain);
                    components.Add(terrainSurrogate);
                }
            }

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

            children = new List<GameObjectSurrogate>();
            foreach (GameObject realChild in realChildren)
            {
                GameObjectSurrogate childSurrogate = new GameObjectSurrogate(realChild);
                children.Add(childSurrogate);
            }
        }

        public GameObject Get()
        {
            GameObject go = new GameObject(this.name);

            foreach (ComponentSurrogate component in components)
            {
                System.Type type = component.GetType();

                if (type == typeof(TransformSurrogate))
                {
                    TransformSurrogate transformSurrogate = component as TransformSurrogate;
                    go.transform.position = transformSurrogate.position.Get();
                    go.transform.rotation = transformSurrogate.rotation.Get();
                    go.transform.localScale = transformSurrogate.scale.Get();
                }
                else if (type == typeof(MeshFilterSurrogate))
                {
                    MeshFilterSurrogate mfSurrogate = component as MeshFilterSurrogate;
                    MeshFilter mf = go.AddComponent<MeshFilter>();
                    if (mfSurrogate.hasMesh)
                        mf.sharedMesh = mfSurrogate.mesh.Get();
                }
                else if (type == typeof(MeshRendererSurrogate))
                {
                    MeshRendererSurrogate mrSurrogate = component as MeshRendererSurrogate;

                    MeshRenderer mr = go.AddComponent<MeshRenderer>();
                    mr.sharedMaterials = mrSurrogate.GetMaterials();
                }
                else if (type == typeof(TerrainSurrogate))
                {
                    Debug.Log("Deserialize Terrain");
                    TerrainSurrogate terrainSurrogate = component as TerrainSurrogate;

                    Terrain terrain = go.AddComponent<Terrain>();
                    TerrainData terrainData = terrain.terrainData;
                    
                    if (terrainData == null)
                    {
                        terrainData = new TerrainData();
                    }
                    
                    float[,] heights = terrainSurrogate.GetHeights();

                    terrainData.heightmapResolution = heights.GetLength(0);
                    terrainData.SetHeights(0, 0, heights);
                    terrainData.size = terrainSurrogate.terrainSize.Get();
                    //terrainData.size = new Vector3((float)TileManager.TileWidth * (float)TileManager.Scaling, 10f, (float)TileManager.TileWidth * (float)TileManager.Scaling);
                    terrain.terrainData = terrainData;
                }
             
            }

            foreach (GameObjectSurrogate childSurrogate in children)
            {
                GameObject child = childSurrogate.Get();
                child.transform.parent = go.transform;
            }

            return go;
        }
    }
}