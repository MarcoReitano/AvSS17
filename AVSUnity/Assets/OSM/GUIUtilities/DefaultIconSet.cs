using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public static class DefaultIconSet
{
    private static string IconFolder = "DefaultIconSet/";

    private static Texture2D LoadIcon(string iconName)
    {
        return Resources.Load(IconFolder + iconName, typeof(Texture2D)) as Texture2D;
    }

    public static Texture2D BuildingIcon3D = LoadIcon("building3D");
    public static Texture2D BuildingIcon = LoadIcon("building");
    public static Texture2D BuildingInactiveIcon = LoadIcon("buildingInactive");

    public static Texture2D EyeIcon = LoadIcon("eye"); 
    public static Texture2D EyeInactiveIcon = LoadIcon("eyeInactive");

    public static Texture2D OpenIcon = LoadIcon("open");
    public static Texture2D OpenSelectedIcon = LoadIcon("openSelected");

    public static Texture2D SaveIcon = LoadIcon("save");
    public static Texture2D SaveSelectedIcon = LoadIcon("saveSelected");
    
    public static Texture2D EditIcon = LoadIcon("edit");
    public static Texture2D EditSelectedIcon = LoadIcon("editSelected");

    public static Texture2D MoveIcon = LoadIcon("move");
    public static Texture2D MoveSelectedIcon = LoadIcon("moveSelected");

    public static Texture2D AddIcon = LoadIcon("add");
    public static Texture2D AddSelectedIcon = LoadIcon("addSelected");

    public static Texture2D DeleteIcon = LoadIcon("delete");
    public static Texture2D DeleteSelectedIcon = LoadIcon("deleteSelected");

    public static Texture2D TextIcon = LoadIcon("text");
    public static Texture2D TextSelectedIcon = LoadIcon("textSelected");

    public static Texture2D RulerIcon = LoadIcon("ruler");
    public static Texture2D RulerSelectedIcon = LoadIcon("rulerSelected");

    public static Texture2D GearsIcon = LoadIcon("gears");
    public static Texture2D GearsSelectedIcon = LoadIcon("gearsSelected");

    public static Texture2D DoorIcon = LoadIcon("door");
    public static Texture2D DoorSelectedIcon = LoadIcon("doorSelected");

    public static Texture2D WindowIcon = LoadIcon("window");
    public static Texture2D WindowSelectedIcon = LoadIcon("windowSelected");
    
    public static Texture2D SettingsIcon = LoadIcon("settings");
    public static Texture2D SettingsSelectedIcon = LoadIcon("settingsSelected");

    public static Texture2D RedirectIcon = LoadIcon("arrowRedirect");
    public static Texture2D AddBuilding = LoadIcon("addBuilding");

    public static Texture2D Paint = LoadIcon("paint");
    public static Texture2D PaintSelected = LoadIcon("paintSelected");

    public static Texture2D Wall = LoadIcon("wall");
    public static Texture2D WallSelected = LoadIcon("wallSelected");
    
    public static Texture2D Device = LoadIcon("device");
    public static Texture2D DeviceSelected = LoadIcon("deviceSelected");

    public static Texture2D Light = LoadIcon("light");
    public static Texture2D LightSelected = LoadIcon("lightSelected");

    
    public static Texture2D Reset = LoadIcon("reset");
    public static Texture2D Shutdown = LoadIcon("shutdown");


    public static Texture2D Mode2D = LoadIcon("2DMode");
    public static Texture2D Mode3D = LoadIcon("3DMode");

    public static Texture2D FloorPlan = LoadIcon("floorplan");
    public static Texture2D FloorPlanSelected = LoadIcon("floorplanSelected");

    public static Texture2D ArrowsLeft = LoadIcon("arrowsLeft");
    public static Texture2D ArrowsRight = LoadIcon("arrowsRight");
    public static Texture2D ArrowsUp = LoadIcon("arrowsUp");
    public static Texture2D ArrowsDown = LoadIcon("arrowsDown");

    public static Texture2D ListUpDown = LoadIcon("listUpDown");
    public static Texture2D ListLeftRight = LoadIcon("listLeftRight");


    public static Texture2D SwitchItem = LoadIcon("switch");
    public static Texture2D SwitchItemSelected = LoadIcon("switchSelected");

    public static Texture2D DimmerItem = LoadIcon("dimmer");
    public static Texture2D DimmerItemSelected = LoadIcon("dimmerSelected");

    //public static Texture2D IEM_Haus_Dach = LoadIcon("IEMHausDach");
    //public static Texture2D IEM_Haus_Geschoss = LoadIcon("IEMHausGeschoss");
    //public static Texture2D IEM_Haus_Boden = LoadIcon("IEMHausBoden");

    public static Texture2D IEM_Haus_Dach = LoadIcon("IEMHausDachWeiss");
    public static Texture2D IEM_Haus_Geschoss = LoadIcon("IEMHausGeschossWeiss");
    public static Texture2D IEM_Haus_Boden = LoadIcon("IEMHausBoden");


    public static Texture2D ColorWheel = LoadIcon("colorWheel");
    public static Texture2D Location = LoadIcon("location");

    public static Texture2D FloorMaterial = LoadIcon("floorMaterial");

    public static Texture2D OrangeCircle = LoadIcon("orangeCircle");
    public static Texture2D OrangeCircleOutline = LoadIcon("circleOutlineOrange");
    public static Texture2D CircleOutline = LoadIcon("circleOutline");


    public static Texture2D ArrowDown4 = LoadIcon("arrow4down");
    public static Texture2D ArrowUp4 = LoadIcon("arrow4up");
    public static Texture2D ArrowLeft4 = LoadIcon("arrow4left");
    public static Texture2D ArrowRight4 = LoadIcon("arrow4right");
    public static Texture2D ArrowMove4 = LoadIcon("arrow4direction");
    public static Texture2D ArrowMove4Selected = LoadIcon("arrow4directionSelected");

    public static Texture2D Arrow4UpDown = LoadIcon("arrow4UpDown");
    public static Texture2D Arrow4UpDownSelected = LoadIcon("arrow4UpDownSelected");

    public static Texture2D Arrow4LeftRight = LoadIcon("arrow4LeftRight");
    public static Texture2D Arrow4LeftRightSelected = LoadIcon("arrow4LeftRightSelected");


    public static Texture2D ArrowDown7 = LoadIcon("arrow7down");
    public static Texture2D ArrowUp7 = LoadIcon("arrow7up");
    public static Texture2D ArrowLeft7 = LoadIcon("arrow7left");
    public static Texture2D ArrowRight7 = LoadIcon("arrow7right");

    
    public static Texture2D Navigation = LoadIcon("navigation");
    public static Texture2D Move = LoadIcon("move");
    public static Texture2D MoveSelected = LoadIcon("moveSelected");

}
