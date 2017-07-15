using UnityEngine;
using System.Collections;
using System.Text;
using System;

public class GoldenRatioTest : MonoBehaviour {

	// Use this for initialization
	void Start () {

        printTest(GoldenRatio.phi, 1);
        printTest(1, GoldenRatio.phi);

        printTest(1, GoldenRatio.Phi);
        printTest(GoldenRatio.Phi, 1);

        printTest(162f, 100f);
        printTest(100f, 162f);

        printTest(16.2f, 10.0f);
        printTest(10f, 16.2f);

        printTest(10f, 6.2f);

        printTest(10f, 20);

        //float number = 0.99999f.Round(4);
        //Debug.Log(number + " = Rounded(3) = " + number.Round(3));

        //number = 0.1f;
        //Debug.Log(number + " = Rounded(0) = " + number.Round(0));

        //number = 0.01f;
        //Debug.Log(number + " = Rounded(1) = " + number.Round(1));

        //number = 0.9f;
        //Debug.Log(number + " = Rounded(2) = " + number.Round(2));

        //number = 1111f;
        //Debug.Log(number + " = Rounded(-1) = " + number.Round(-1));

        //GL_RendererOnCamera.DrawLines += new EventHandler(DrawEdgeGL);
        //GL_RendererOnCamera.DrawTriangles += new EventHandler(DrawTriangleGL);
	}

    public void printTest(float longSide, float shortSide)
    {
        StringBuilder sb = new StringBuilder();

        sb.Append("\n").Append(longSide + " and " + shortSide);

        sb.Append("\n").Append("phi = " + GoldenRatio.phi);
        sb.Append("\n").Append("Phi = " + GoldenRatio.Phi);

        sb.Append("\n").Append("Long side of " + longSide + " : " + GoldenRatio.LongSideOf(longSide) + "   -->  " + longSide.LongSideOfLengthGoldenRatio());
        sb.Append("\n").Append("Shortside of " + longSide + " : " + GoldenRatio.ShortSideOf(longSide) + "   -->  " + longSide.ShortSideOfLengthGoldenRatio());


        sb.Append("\n").Append("Long side of " + shortSide + " : " + GoldenRatio.LongSideOf(shortSide) + "   -->  " + shortSide.LongSideOfLengthGoldenRatio());
        sb.Append("\n").Append("Shortside of " + shortSide + " : " + GoldenRatio.ShortSideOf(shortSide) + "   -->  " + shortSide.ShortSideOfLengthGoldenRatio());

        sb.Append("\n").Append("Long side for " + longSide + " : " + GoldenRatio.LongSideFor(longSide) + "   -->  " + longSide.LongSideForShortSideGoldenRatio());
        sb.Append("\n").Append("Shortside for " + longSide + " : " + GoldenRatio.ShortSideFor(longSide) + "   -->  " + longSide.ShortSideForLongSideGoldenRatio());

        sb.Append("\n").Append("Long side for " + shortSide + " : " + GoldenRatio.LongSideFor(shortSide) + "   -->  " + shortSide.LongSideForShortSideGoldenRatio());
        sb.Append("\n").Append("Shortside for " + shortSide + " : " + GoldenRatio.ShortSideFor(shortSide) + "   -->  " + shortSide.ShortSideForLongSideGoldenRatio());
        
        sb.Append("\n").Append(longSide + " and " + shortSide + " GoldenRatio ? " + GoldenRatio.IsGoldenRatio(longSide, shortSide));
        sb.Append("\n").Append(longSide + " and " + shortSide + " GoldenRatio ? " + GoldenRatio.IsGoldenRatio(shortSide, longSide));

        sb.Append("\n").Append("Approx: " + longSide + " and " + shortSide + " GoldenRatio ? " + GoldenRatio.IsGoldenRatio(longSide, shortSide));
        sb.Append("\n").Append("Approx: " + longSide + " and " + shortSide + " GoldenRatio ? " + GoldenRatio.IsGoldenRatio(shortSide, longSide));

        sb.Append("\n").Append("Rounded 0: " + longSide + " and " + shortSide + " GoldenRatio ? " + GoldenRatio.IsGoldenRatio(longSide, shortSide, 0));
        sb.Append("\n").Append("Rounded 0: " + longSide + " and " + shortSide + " GoldenRatio ? " + GoldenRatio.IsGoldenRatio(shortSide, longSide, 0));

        sb.Append("\n").Append("Rounded 2: " + longSide + " and " + shortSide + " GoldenRatio ? " + GoldenRatio.IsGoldenRatio(longSide, shortSide, 2));
        sb.Append("\n").Append("Rounded 2: " + longSide + " and " + shortSide + " GoldenRatio ? " + GoldenRatio.IsGoldenRatio(shortSide, longSide, 2));

        sb.Append("\n").Append("Rounded 8: " + longSide + " and " + shortSide + " GoldenRatio ? " + GoldenRatio.IsGoldenRatio(longSide, shortSide, 8));
        sb.Append("\n").Append("Rounded 8: " + longSide + " and " + shortSide + " GoldenRatio ? " + GoldenRatio.IsGoldenRatio(shortSide, longSide, 8));


        sb.Append("\n").Append("ShortSide for " + longSide + " = " + GoldenRatio.ShortSideFor(longSide));
        sb.Append("\n").Append("LongSide for " + shortSide + " = " + GoldenRatio.LongSideFor(shortSide));

        Debug.Log(sb.ToString());
    }


	// Update is called once per frame
	void Update () {
	    
	}


    public void DrawEdgeGL(System.Object sender, EventArgs e)
    {
        //GL.Color(XKCDColors.White);

        ////camera.WorldToScreenPoint(Vector3.zero);
        ////Debug.Log(Camera.main.ScreenToWorldPoint(new Vector3(0f, 0f, Camera.main.nearClipPlane)));
        ////Debug.Log(Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.nearClipPlane)));

        //GL.Vertex(Camera.main.ScreenToWorldPoint(new Vector3(0f, 0f, Camera.main.nearClipPlane)));
        //GL.Vertex(Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.nearClipPlane)));
    }


    public float startWinkel = 0;
    public float endWinkel = 90;
    public int punktanzahl = 360;


    public void DrawTriangleGL(System.Object sender, EventArgs e)
    {
        GL.Color(XKCDColors.Wintergreen);
        
        float step = (2*Mathf.PI) / punktanzahl;

       

        float radius = 300;

        Vector3 midPoint = new Vector3(Screen.width / 2, Screen.height / 2, Camera.main.nearClipPlane);

        Vector3 Mittelpunkt = Camera.main.ScreenToWorldPoint(midPoint);


        for (float i = startWinkel; i < endWinkel; i += step)
        {
            Vector3 first = PointOnCircle(i, radius, midPoint);
            Vector3 second = PointOnCircle(i + step, radius, midPoint);

            GL.TexCoord2(0, 0); GL.Vertex(Mittelpunkt);
            GL.TexCoord2(1, 0); GL.Vertex(first);
            GL.TexCoord2(1, 1); GL.Vertex(second);
        }

    }

    private static Vector3 PointOnCircle(float winkel, float radius, Vector3 midPoint)
    {
        return Camera.main.ScreenToWorldPoint(
            new Vector3(
                Mathf.Cos(winkel * Mathf.Deg2Rad) * radius + midPoint.x, 
                Mathf.Sin(winkel * Mathf.Deg2Rad) * radius + midPoint.y, 
                Camera.main.nearClipPlane));
    }
}
