using UnityEngine;
#if UNITY_EDITOR
//using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// MaterialManager.
/// </summary>
public static class MaterialManager
{
    private static Dictionary<string, Material> materialDict = new Dictionary<string, Material>();
    private static Material errorPink;

    static MaterialManager()
    {
        materialDict.Add("diffuseWhite", (Material)Resources.Load("Materials/Colors/White"));
        materialDict.Add("diffuseBlack", (Material)Resources.Load("Materials/Colors/Black"));
        materialDict.Add("diffuseGray", (Material)Resources.Load("Materials/Colors/Gray"));
        materialDict.Add("diffuseDarkGray", (Material)Resources.Load("Materials/Colors/DarkGray"));
        materialDict.Add("diffuseGrey", (Material)Resources.Load("Materials/Colors/Gray"));
        materialDict.Add("diffuseDarkGrey", (Material)Resources.Load("Materials/Colors/DarkGray"));
        materialDict.Add("diffuseBrown", (Material)Resources.Load("Materials/Colors/Brown"));
        materialDict.Add("diffuseRed", (Material)Resources.Load("Materials/Colors/Red"));
        materialDict.Add("diffuseBlue", (Material)Resources.Load("Materials/Colors/Blue"));
        materialDict.Add("diffuseDirt", (Material)Resources.Load("Materials/Dirt"));
        materialDict.Add("diffuseCheckerboard", (Material)Resources.Load("Materials/BlackWhite"));
        errorPink = (Material)Resources.Load("Materials/Colors/ErrorPink");
    }

    public static bool AddMaterial(string materialName, string resourcePath)
    {
        Material m = (Material) Resources.Load(resourcePath, (typeof(Material)));
        if(m == null)
            return false;

        materialDict.Add(materialName, m);

        return true;
    }
    public static Material GetMaterial(string materialName)
    {
        Material m;
        if (materialDict.TryGetValue(materialName, out m))
            return m;
        else
            return errorPink; 
    }
}
