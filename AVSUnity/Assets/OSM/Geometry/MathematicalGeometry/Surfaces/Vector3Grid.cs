using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Vector3Grid
{
    public Vector3[] array;// = new Vector3[1];

    [SerializeField]
    private int height = 0;
    public int Height
    {
        get { return height; }
        set { height = value; }
    }

    [SerializeField]
    private int width = 0;
    public int Width
    {
        get { return width; }
        set { width = value; }
    }


    public Vector3Grid(int width, int height)
    {
        this.width = width;
        this.height = height;

        this.array = new Vector3[width*height];
    }

    public Vector3Grid(int width, int height, Vector3[,] values)
    {
        this.width = width;
        this.height = height;

        this.array = new Vector3[width * height];
        
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                this[x, y] = values[x, y];
            }
        }
    }


    public Vector3 this[int x, int y]{
        get{return this.array[x * height + y];}
        set{this.array[x * height + y] = value;}
    }


    //public void Set(int x, int y, Vector3 value)
    //{
    //    //if(x < width && y < height)
    //    this.array[x * height + y] = value;
    //}



    //public Vector3 Get(int x, int y)
    //{
    //    //if(x * height + y < this.list.Count)
    //    return this.array[x * height + y];
    //    //else
    //    //    return default(T);
    //}

}

