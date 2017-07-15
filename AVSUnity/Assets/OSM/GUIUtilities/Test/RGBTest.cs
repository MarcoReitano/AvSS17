using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RGBTest : MonoBehaviour {

    [SerializeField]
    public int width = 255;
    [SerializeField]
    public int height = 255;
    [SerializeField]
    public List<Color> colors = new List<Color>();
    [SerializeField]
    public Texture2D tex;

	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnGUI(){
        
        if(colors.Count > 1)
            tex = CustomGUIUtils.GetGradientTexture(width, height, colors.ToArray());
        
        if (tex != null)
        {
            //for (int i = 0; i < height; i++)
            //{
                GUI.Label(new Rect(0f, 0f, width, height), tex);    
            //}
        }
    }
}
