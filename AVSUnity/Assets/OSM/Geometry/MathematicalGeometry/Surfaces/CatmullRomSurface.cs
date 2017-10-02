using System;
using UnityEngine;


[Serializable]
public class CatmullRomSurface
{
    [SerializeField]
    private int numberOfSegmentsWidth = 10;
    public int NumberOfSegmentsWidth
    {
        get { return numberOfSegmentsWidth; }
        set
        {
            this.numberOfSegmentsWidth = value;
            RecalculateSurface();
        }
    }


    [SerializeField]
    private int numberOfSegmentsHeight = 10;
    public int NumberOfSegmentsHeight
    {
        get { return numberOfSegmentsHeight; }
        set
        {
            this.numberOfSegmentsHeight = value;
            RecalculateSurface();
        }
    }


    [SerializeField]
    private int numberOfControlPointsWidth = 4;
    public int NumberOfControlPointsWidth
    {
        get { return numberOfControlPointsWidth; }
        set { numberOfControlPointsWidth = value; }
    }


    [SerializeField]
    private int numberOfControlPointsHeight = 4;
    public int NumberOfControlPointsHeight
    {
        get { return numberOfControlPointsHeight; }
        set { numberOfControlPointsHeight = value; }
    }


    [SerializeField]
    public Vector3Grid controlPoints;

    [SerializeField]
    public Vector3Grid surfacePoints;


    [SerializeField]
    private float gridSize = 10f;

    public float GridSize
    {
        get { return gridSize; }
        set
        {
            gridSize = value;
            for (int m = 0; m < this.numberOfControlPointsWidth; m++)
            {
                for (int n = 0; n < this.numberOfControlPointsHeight; n++)
                {
                    float oldHeight = this.controlPoints[m,n].y;
                    this.controlPoints[m, n] = new Vector3(position.x + m * gridSize, oldHeight, position.z + n * gridSize);
                }
            }
            RecalculateSurface();
        }
    }

    [SerializeField]
    private Vector3 position;
    public Vector3 Position
    {
        get { return position; }
        set
        {
            position = value;
            for (int m = 0; m < this.numberOfControlPointsWidth; m++)
            {
                for (int n = 0; n < this.numberOfControlPointsHeight; n++)
                {
                    float oldHeight = this.controlPoints[m, n].y;
                    this.controlPoints[m, n]= new Vector3(position.x + m * gridSize, oldHeight, position.z + n * gridSize);
                }
            }
            RecalculateSurface();
        }
    }


    public void Initialize()
    {
        this.controlPoints = new Vector3Grid(this.numberOfControlPointsWidth, this.numberOfControlPointsHeight);

        for (int m = 0; m < this.numberOfControlPointsWidth; m++)
        {
            for (int n = 0; n < this.numberOfControlPointsHeight; n++)
            {
                this.controlPoints[m, n] = new Vector3(position.x + m * gridSize, 0f, position.z + n * gridSize);
            }
        }
        RecalculateSurface();
    }


    public Vector3 P(int i, int j, float u, float w)
    {
        Vector3 P3 = CardinalSpline.P(controlPoints[i + 3,  j], controlPoints[i + 3,  j + 1], controlPoints[i + 3,  j + 2], controlPoints[i + 3,  j + 3], w, 0.5f);
        Vector3 P2 = CardinalSpline.P(controlPoints[i + 2,  j], controlPoints[i + 2,  j + 1], controlPoints[i + 2,  j + 2], controlPoints[i + 2,  j + 3], w, 0.5f);
        Vector3 P1 = CardinalSpline.P(controlPoints[i + 1,  j], controlPoints[i + 1,  j + 1], controlPoints[i + 1,  j + 2], controlPoints[i + 1,  j + 3], w, 0.5f);
        Vector3 P0 = CardinalSpline.P(controlPoints[i + 0,  j], controlPoints[i + 0,  j + 1], controlPoints[i + 0,  j + 2], controlPoints[i + 0,  j + 3], w, 0.5f);

        Vector3 result = CardinalSpline.P(P0, P1, P2, P3, u, 0.5f);

        return result;
    }

    //public static Vector3 P(Array2D<Vector3> controlPoints, int i, int j, float u, float w)
    //{
    //    Vector3 P3 = CardinalSpline.P(controlPoints.Get(i + 3, j), controlPoints.Get(i + 3, j + 1), controlPoints.Get(i + 3, j + 2), controlPoints.Get(i + 3, j + 3), w, 0.5f);
    //    Vector3 P2 = CardinalSpline.P(controlPoints.Get(i + 2, j), controlPoints.Get(i + 2, j + 1), controlPoints.Get(i + 2, j + 2), controlPoints.Get(i + 2, j + 3), w, 0.5f);
    //    Vector3 P1 = CardinalSpline.P(controlPoints.Get(i + 1, j), controlPoints.Get(i + 1, j + 1), controlPoints.Get(i + 1, j + 2), controlPoints.Get(i + 1, j + 3), w, 0.5f);
    //    Vector3 P0 = CardinalSpline.P(controlPoints.Get(i + 0, j), controlPoints.Get(i + 0, j + 1), controlPoints.Get(i + 0, j + 2), controlPoints.Get(i + 0, j + 3), w, 0.5f);

    //    Vector3 result = CardinalSpline.P(P0, P1, P2, P3, u, 0.5f);

    //    return result;
    //}


    public void RecalculateSurface()
    {
        float deltaU = 1f / numberOfSegmentsWidth;
        float deltaW = 1f / numberOfSegmentsHeight;

        int arrayWidth = (numberOfSegmentsWidth) * (this.numberOfControlPointsWidth - 3) + 1;
        int arrayHeight = (numberOfSegmentsHeight) * (this.numberOfControlPointsHeight - 3) + 1;

        surfacePoints = new Vector3Grid(numberOfSegmentsWidth * (this.numberOfControlPointsWidth - 3) + 1, numberOfSegmentsHeight * (this.numberOfControlPointsHeight - 3) + 1);

        for (int i = 0; i < this.numberOfControlPointsWidth - 3; i++)
        {
            for (int j = 0; j < this.numberOfControlPointsHeight - 3; j++)
            {
                for (int u = 0; u < numberOfSegmentsWidth + 1; u++)
                {
                    for (int w = 0; w < numberOfSegmentsHeight + 1; w++)
                    {
                        surfacePoints[i * numberOfSegmentsWidth + u, j * numberOfSegmentsHeight + w] = P(i, j, u * deltaU, w * deltaW);
                    }
                }
            }
        }
    }

    public static void DrawSurface(CatmullRomSurface surface)
    {
        if (surface.surfacePoints == null)
        {
            //surface.surfacePoints = new Vector3[(surface.NumberOfSegmentsWidth) * (surface.NumberOfControlPointsWidth - 3) + 1, (surface.NumberOfSegmentsHeight) * (surface.NumberOfControlPointsHeight - 3) + 1];
            return;
        }

        Gizmos.color = Color.gray;
        for (int u = 0; u < surface.surfacePoints.Width; u++)
            for (int w = 0; w < surface.surfacePoints.Height - 1; w++)
                Gizmos.DrawLine(surface.surfacePoints[u, w], surface.surfacePoints[u, w + 1]);

        for (int u = 0; u < surface.surfacePoints.Width - 1; u++)
            for (int w = 0; w < surface.surfacePoints.Height; w++)
                Gizmos.DrawLine(surface.surfacePoints[u, w], surface.surfacePoints[u + 1, w]);
    }


}

