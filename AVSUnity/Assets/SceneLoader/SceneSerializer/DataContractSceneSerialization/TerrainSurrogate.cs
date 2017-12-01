using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

namespace DataContractSceneSerialization
{
    [DataContract]
    public class TerrainSurrogate : ComponentSurrogate
    {
        [DataMember(Name = "width")]
        public int width;

        [DataMember(Name = "height")]
        public int height;

        [DataMember(Name = "heightData")]
        public float[] heightData;

        [DataMember(Name = "terrainSize")]
        public Vector3Surrogate terrainSize;

        public TerrainSurrogate()
        {

        }

        public TerrainSurrogate(Terrain terrain)
        {
            TerrainData terrainData = terrain.terrainData;
            this.width = terrainData.heightmapWidth;
            this.height = terrainData.heightmapHeight;
            this.terrainSize = new Vector3Surrogate(terrainData.size);

            float[,] heights = terrainData.GetHeights(0, 0, terrainData.heightmapWidth, terrainData.heightmapHeight);

            this.heightData = new float[terrainData.heightmapWidth * terrainData.heightmapHeight];

            int i = 0;
            for (int x = 0; x < terrainData.heightmapWidth; x++)
            {
                for (int y = 0; y < terrainData.heightmapHeight; y++)
                {
                    this.heightData[i++] = heights[x, y];
                }
            }
            //heightData = heights.Cast<float>().ToArray();
        }

        public float[,] GetHeights()
        {
            float[,] heights = new float[width, height];
            int pointer = 0;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    heights[x, y] = heightData[pointer++];
                }
            }
            return heights;
        }
    }
}
