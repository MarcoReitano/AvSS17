using UnityEngine;
#if UNITY_EDITOR
//using UnityEditor;
#endif
using System.Collections;

/// <summary>
/// Terraintest.
/// </summary>
public class Terraintest : MonoBehaviour
{

#if UNITY_EDITOR
    [UnityEditor.MenuItem("City/TerrainTest")]
    public static void Test()
    {
        Terraintest tT = FindObjectOfType(typeof(Terraintest)) as Terraintest;
        tT.TerrainTest();
    }
#endif

    public void TerrainTest()
    {
        Terrain t = gameObject.GetOrAddComponent<Terrain>();
        TerrainCollider tC = gameObject.GetOrAddComponent<TerrainCollider>();

        TerrainData terrainData = new TerrainData();
        terrainData.heightmapResolution = 1025;
        terrainData.size = new Vector3(500f, 100f, 500f);
        //terrainData.baseMapResolution = 1025;
        float[,] heightmap = new float[1025, 1025];
        for (int i = 0; i < 1025; i++)
        {
            for (int j = 0; j < 1025; j++)
            {
                heightmap[i, j] = 0.5f + 0.5f * Mathf.Sin(((float)i/1025f + (float)j/1025f)*Mathf.PI*2);
            }
        }

        terrainData.SetHeights(0, 0, heightmap);

        t.terrainData = terrainData;
        tC.terrainData = terrainData;
    }
}
