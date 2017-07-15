using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;




public static class TextureRepository
{
    private static string TextureFolder = "DefaultTextures/";

    private static Texture2D LoadTexture(string iconName)
    {
        Texture2D texture = Resources.Load(TextureFolder + iconName, typeof(Texture2D)) as Texture2D;
        //Debug.Log("Loaded Texture " + iconName + "  suceeded? " + (texture != null));
        return texture;
    }

    public static Texture2D Wall_AussenPutz = LoadTexture("putz");
    public static Texture2D Raufaser = LoadTexture("RaufaserWeiss");


}

