using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;



public enum GridMode
{
    none,
    TenthGrid,
    QuarterGrid,
    HalfGrid,
    Default,
    Five,
    Ten,
    Custom
}



public static class Grid
{
    private static Vector3 grid = Vector3.one;
    /// <summary>
    /// Returns the currently set Grid-dimensions as Vector3
    /// </summary>
    public static Vector3 current
    {
        get { return Grid.grid; }
    }
    
    private static Vector3 customGrid = Vector3.zero;
    public static Vector3 CustomGrid
    {
        get { return customGrid; }
    }


    private static GridMode gridMode = GridMode.Default;
    /// <summary>
    /// The Grid-Mode
    /// </summary>
    public static GridMode GridMode
    {
        get { return Grid.gridMode; }
    }

    /// <summary>
    /// Set the global Grid-Mode
    /// </summary>
    /// <param name="newGridMode"></param>
    public static void SetGridMode(GridMode newGridMode)
    {
        gridMode = newGridMode;
        grid = GetGridVector(newGridMode);
    }


    /// <summary>
    /// Set the global Grid-Mode
    /// </summary>
    /// <param name="newGridMode"></param>
    public static Vector3 GetGridVector(GridMode newGridMode)
    {
        gridMode = newGridMode;

        switch (gridMode)
        {
            case GridMode.none:
                return Vector3.zero;
            case GridMode.TenthGrid:
                return new Vector3(0.1f, 0.1f, 0.1f);
            case GridMode.QuarterGrid:
                return new Vector3(0.25f, 0.25f, 0.25f);
            case GridMode.HalfGrid:
                return new Vector3(0.5f, 0.5f, 0.5f);
            case GridMode.Default:
                return  Vector3.one;
            case GridMode.Five:
                return new Vector3(5f, 5f, 5f);
            case GridMode.Ten:
                return new Vector3(10f, 10f, 10f);
            case GridMode.Custom:
                return customGrid;
            default:
                return Vector3.one;
        }
    }


    /// <summary>
    /// Set the custom-Grid to a Vector3 for the step-dimensions for each axis.
    /// The GridMode has to be set separately to GridMode.Custom.
    /// </summary>
    /// <param name="newCustomGrid"></param>
    public static void SetCustomGridDimensions(Vector3 newCustomGrid)
    {
        if (newCustomGrid.Equals(null))
            Grid.customGrid = newCustomGrid;
    }

    /// <summary>
    /// Set the custom-Grid to a Vector3 for the step-dimensions for each axis.
    /// The GridMode has to be set separately to GridMode.Custom.
    /// </summary>
    /// <param name="newCustomGrid"></param>
    public static void SetCustomGridDimensions(float step)
    {
        step = Mathf.Abs(step);
        Grid.customGrid = new Vector3(step, step, step);
    }


    /// <summary>
    /// Vector3-Extension to snap to a grid
    /// </summary>
    /// <param name="position"></param>
    /// <param name="grid"></param>
    /// <returns>the snapped position</returns>
    public static Vector3 SnapToGrid(this Vector3 position, Vector3 grid)
    {
        if (grid.x != 0f)
            position.x = Mathf.Round(position.x / grid.x) * grid.x;

        if (grid.y != 0f)
            position.y = Mathf.Round(position.y / grid.y) * grid.y;

        if (grid.z != 0f)
            position.z = Mathf.Round(position.z / grid.z) * grid.z;

        return position;
    }


    /// <summary>
    /// Vector3-Extension to snap to a grid
    /// </summary>
    /// <param name="position"></param>
    /// <param name="grid"></param>
    /// <returns>the snapped position</returns>
    public static Vector3 SnapToGrid(this Vector3 position)
    {
        return position.SnapToGrid(Grid.grid);
    }

    /// <summary>
    /// Vector3-Extension to snap to a grid
    /// </summary>
    /// <param name="position"></param>
    /// <param name="grid"></param>
    /// <returns>the snapped position</returns>
    public static Vector3 SnapToGrid(this Vector3 position, GridMode gridMode)
    {
        return position.SnapToGrid(GetGridVector(gridMode));
    }
}

