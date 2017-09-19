using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public static class MaterialRepository
{

    public static Material DummyMaterialTransparent = new Material(Shader.Find("Transparent/Diffuse"));
    public static Material DummyMaterial = new Material(Shader.Find("Diffuse"));

    //private static readonly MaterialRepository instance = new MaterialRepository();
    

    //private MaterialRepository()
    //{
        
    //}

    //public static MaterialRepository Instance
    //{
    //    get
    //    {
    //        //// Just in case Mono freaks out over the static initialization...
    //        //// -->  uncomment this and remove the "readonly" marker of the instance-field.
    //        //if (instance == null)
    //        //{
    //        //    instance = new MaterialRepository();
    //        //}
    //        return instance;
    //    }
    //}


    private static Dictionary<Color, Material> colorMaterials = new Dictionary<Color, Material>();

    public static Material GetColorMaterial(Color color){

        Material colorMaterial;
        if(!colorMaterials.TryGetValue(color, out colorMaterial)){
            colorMaterial = new Material(Shader.Find("Transparent/Diffuse"));
            colorMaterial.color = color;
            colorMaterials.Add(color, colorMaterial);
        }

        return colorMaterial;
    }


    public static Material Wall_AussenPutz = 
        new Material(Shader.Find("Transparent/Diffuse"))
        .SetTextureOnCreate(TextureRepository.Wall_AussenPutz);

    public static Material Raufaser =
       new Material(Shader.Find("Transparent/Diffuse"))
       .SetTextureOnCreate(TextureRepository.Raufaser);

   

}
