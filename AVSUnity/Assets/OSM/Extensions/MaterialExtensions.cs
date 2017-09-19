using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public static class MaterialExtensions
{

    public static Material SetTextureOnCreate(this Material material, Texture texture)
    {
        material.mainTexture = texture;

        if (material.mainTexture != texture)
            Debug.Log("Setting mainTexture failed!!!");

        return material;
    }
}

