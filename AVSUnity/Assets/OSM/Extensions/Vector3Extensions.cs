using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;




public enum SnapValueMode
{
    Default,

    LockXY,
    LockXZ,
    LockYZ,

    LockX,
    LockY,
    LockZ,
}




public static class Vector3Extensions
{
        
    public static Vector3 SnapValue(this Vector3 position, Vector3 oldPosition, SnapValueMode mode)
    {
        switch (mode)
        {
            case SnapValueMode.Default:
                return position;
            case SnapValueMode.LockXY:
                return new Vector3(oldPosition.x, oldPosition.y, position.z);
            case SnapValueMode.LockXZ:
                return new Vector3(oldPosition.x, position.y, oldPosition.z);
            case SnapValueMode.LockYZ:
                return new Vector3(position.x, oldPosition.y, oldPosition.z);
            case SnapValueMode.LockX:
                return new Vector3(oldPosition.x, position.y, position.z);
            case SnapValueMode.LockY:
                return new Vector3(position.x, oldPosition.y, position.z);
            case SnapValueMode.LockZ:
                return new Vector3(position.x, position.y, oldPosition.z);
            default:
                return position;
        }
    }



    public static Vector3 SnapOnRay(this Vector3 position, Vector3 direction)
    {
        return MathUtils.GetMinDistancePointOnRay(position, position+direction, position);
    }


    public static Vector3 SnapOnRay(this Vector3 position, Vector3 start, Vector3 end)
    {
        return MathUtils.GetMinDistancePointOnRay(start, end, position);
    }


    public static Vector3 SnapOnLine(this Vector3 position, Vector3 start, Vector3 end)
    {
        return MathUtils.GetMinDistancePointOnLine(start, end, position);
    }


    public static bool InDistance(this Vector3 position, Vector3 otherPosition, float distance)
    {
         float tmp = (position - otherPosition).sqrMagnitude;
         if (tmp < distance * distance)
             return true;

         return false;
    }

    public static bool InDistance(this Vector2 position, Vector2 otherPosition, float distance)
    {
        float tmp = (position - otherPosition).sqrMagnitude;
        if (tmp < distance * distance)
            return true;

        return false;
    }



    public static string ListToString(this List<Vector3> list)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("List<Vector3> = {\n");
        foreach (Vector3 vector in list)
        {
            sb.Append('\t').Append(vector.ToString()).Append('\n');
        }
        sb.Append("}");
        return sb.ToString();
    }


    public static string ListToString<T>(this List<T> list)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("List<" + typeof(T) + "> = {\n");
        foreach (T vector in list)
        {
            sb.Append('\t').Append(vector.ToString()).Append('\n');
        }
        sb.Append("}");
        return sb.ToString();
    }


    public static Vector3 SwapScreenToWorldPoint(this Vector3 position)
    {
        position.y = Screen.height - position.y;
        return position;
    }

    public static Vector2 SwapScreenToWorldPoint(this Vector2 position)
    {
        position.y = Screen.height - position.y;
        return position;
    }


    public static Vector3 Vector3FromVector2XY(this Vector2 vector)
    {
        return new Vector3(vector.x, vector.y, 0f);
    }

    public static Vector3 Vector3FromVector2XZ(this Vector2 vector)
    {
        return new Vector3(vector.x, 0f, vector.y);
    }
}
