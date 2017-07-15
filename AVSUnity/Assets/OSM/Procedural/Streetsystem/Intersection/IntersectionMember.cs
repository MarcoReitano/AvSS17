using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IntersectionMember
{
    public IntersectionMember(Street street, Intersection intersection)
    {
        this.Street = street;
        if (street.From == intersection && street.To != intersection)
        {
            Type = IntersectionMemberType.Departing;
            if (street.Beginning)
            {
                Vector2 direction = (Vector2)street.Beginning - (Vector2)intersection.Node;
                Angle = direction.ToOrientation();
            }
        }
        else if (street.From != intersection && street.To == intersection)
        {
            Type = IntersectionMemberType.Arriving;
            if (street.End)
            {
                Vector2 direction = (Vector2)street.End - (Vector2)intersection.Node;
                Angle = direction.ToOrientation();
            }
        }
    }

    #region streetPropertyWrapper
    public float FromOffset
    {
        get 
        {
            if(Type == IntersectionMemberType.Arriving)
                return Street.ToOffset;
            return Street.FromOffset;
        }
        set 
        {
            if (Type == IntersectionMemberType.Arriving)
                Street.ToOffset = value;
            else
                Street.FromOffset = value;            
        }
    }
    public float ToOffset
    {
        get
        {
            if (Type == IntersectionMemberType.Arriving)
                return Street.FromOffset;
            return Street.ToOffset;
        }
        set
        {
            if (Type == IntersectionMemberType.Arriving)
                Street.FromOffset = value;
            else
                Street.ToOffset = value;
        }
    }

    public Vertex Beginning
    {
        get
        {
            if (Type == IntersectionMemberType.Arriving)
                return Street.End;
            return Street.Beginning;
        }

    }
    public Vertex End
    {
        get
        {
            if (Type == IntersectionMemberType.Arriving)
                return Street.Beginning;
            return Street.End;
        }

    }
    public Vertex BeginningLeft
    {
        get
        {
            if (Type == IntersectionMemberType.Arriving)
                return Street.EndRight;
            return Street.BeginningLeft;
        }
    }
    public Vertex BeginningRight
    {
        get
        {
            if (Type == IntersectionMemberType.Arriving)
                return Street.EndLeft;
            return Street.BeginningRight;
        }
    }
    public Vertex EndLeft
    {
        get
        {
            if (Type == IntersectionMemberType.Arriving)
                return Street.BeginningRight;
            return Street.EndLeft;
        }
    }
    public Vertex EndRight
    {
        get
        {
            if (Type == IntersectionMemberType.Arriving)
                return Street.BeginningLeft;
            return Street.EndRight;
        }
    }
    public Polyline LeftOutline
    {
        get 
        {
            if (Type == IntersectionMemberType.Arriving)
                return Street.RightOutline;
            return Street.LeftOutline;
        }
    }
    public Polyline RightOutline
    {
        get 
        {
            if (Type == IntersectionMemberType.Arriving)
                return Street.LeftOutline;
            return Street.RightOutline;
        }
    }
    public CarriageWay ForwardCarriageWay
    {
        get 
        {
            if (Type == IntersectionMemberType.Arriving)
                return Street.BackwardCarriageWay;
            return Street.ForwardCarriageWay;
        }
    }
    public CarriageWay BackwardCarriageWay
    {
        get
        {
            if (Type == IntersectionMemberType.Arriving)
                return Street.BackwardCarriageWay;
            return Street.ForwardCarriageWay;
        }
    }

    #endregion

    public bool IsValid()
    {
        if (Type == IntersectionMemberType.Departing && Street.Beginning == null)
        {
            //Debug.Log("Case1" + " Street.Beginning is " + Street.Beginning);
            return false;
        }
        if (Type == IntersectionMemberType.Arriving && Street.End == null)
        {
            //Debug.Log("Case2" + " Street.Beginning is " + Street.End);
            return false;
        }
        if (Type == IntersectionMemberType.None)
        {
            //Debug.Log("Case3");
            return false;
        }
        //Debug.Log("Case4");
        return true;
    }

    public Street Street;
    public IntersectionMemberType Type;
    public float Angle;
}

//Allows sorting by ConnectionAngle
public class IntersectionMemberComparer : IComparer<IntersectionMember>
{
    public int Compare(IntersectionMember x, IntersectionMember y)
    {
        return x.Angle.CompareTo(y.Angle);
    }
}
public enum IntersectionMemberType
{
    None,
    Arriving,
    Departing,
    //Circular
}
public enum IntersectionType
{
    Intersection,
    Connection,
    DeadEnd
}
