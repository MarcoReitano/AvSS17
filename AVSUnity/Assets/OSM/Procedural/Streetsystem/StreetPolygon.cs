using UnityEngine;
#if UNITY_EDITOR
//using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// SteetPolygon.
/// </summary>
/// 
/////// WIP!!!!///////
public class StreetPolygon
{
    public StreetPolygon()
    {}
    //private StreetPolygon(IEnumerable<StreetPolygonMember> streets)
    //{
    //    this.streetmember = new List<StreetPolygonMember>(streets);
    //}

    private List<StreetPolygonMember> streetmember = new List<StreetPolygonMember>();
    private StreetPolygonType Type;

    public static bool GetRighthandStreetPolygon(Street street, out StreetPolygon streetPolygon)
    {
        streetPolygon = new StreetPolygon();
        streetPolygon.Type = StreetPolygonType.RightHand;
        StreetPolygonMember first = new StreetPolygonMember(street, street.From);
        streetPolygon.streetmember.Add(first);

        StreetPolygonMember currentStreetMember = first;

        Street righthandStreet;
        while (currentStreetMember.To.TryGetRightStreet(currentStreetMember.Street, out righthandStreet))
        {
            StreetPolygonMember newStreetPolygonMember;
            if (righthandStreet != currentStreetMember.Street && righthandStreet != null)
            {
                newStreetPolygonMember = new StreetPolygonMember(righthandStreet, currentStreetMember.To);
                streetPolygon.streetmember.Add(newStreetPolygonMember);
                if (newStreetPolygonMember.To == first.From)
                    break;
            }
            else
                break;
            currentStreetMember = newStreetPolygonMember;
        }

        if (first.From == streetPolygon.Last.To)
            return true;
        return false;
    }
    public static bool GetLefthandStreetPolygon(Street street, out StreetPolygon streetPolygon)
    {
        streetPolygon = new StreetPolygon();
        streetPolygon.Type = StreetPolygonType.LeftHand;
        StreetPolygonMember first = new StreetPolygonMember(street, street.From);
        streetPolygon.streetmember.Add(first);

        StreetPolygonMember currentStreetMember = first;
        Street lefthandStreet;
        while (currentStreetMember.To.TryGetLeftStreet(currentStreetMember.Street, out lefthandStreet))
        {
            StreetPolygonMember newStreetPolygonMember;
            if (lefthandStreet != currentStreetMember.Street && lefthandStreet != null)
            {
                newStreetPolygonMember = new StreetPolygonMember(lefthandStreet, currentStreetMember.To);
                streetPolygon.streetmember.Add(newStreetPolygonMember);
                if (newStreetPolygonMember.To == first.From)
                    break;
            }
            else
                break;

            currentStreetMember = newStreetPolygonMember;
        }

        if (first.From == streetPolygon.Last.To)
            return true;
        return false;
    }

    public bool IsLefthandPolygonOf(Street street)
    {
        for (int i = 0; i < streetmember.Count; i++)
        {
            if (streetmember[i].Street == street)
            {
                if (streetmember[i].Type == StreetPolygonMemberType.Forwards)
                {
                    if (Type == StreetPolygonType.LeftHand)
                        return true;
                    else
                        return false;
                }
                else 
                {
                    if (Type == StreetPolygonType.RightHand)
                        return false;
                    else
                        return true;
                }
            }
        }
        return false;
    }
    public bool IsRighthandPolygonOf(Street street)
    {
        for (int i = 0; i < streetmember.Count; i++)
        {
            if (streetmember[i].Street == street)
            {
                if (streetmember[i].Type == StreetPolygonMemberType.Forwards)
                {
                    if (Type == StreetPolygonType.LeftHand)
                        return false;
                    else
                        return true;
                }
                else
                {
                    if (Type == StreetPolygonType.RightHand)
                        return true;
                    else
                        return false;
                }
            }
        }
        return false;
    }

    public PolygonSurface TriangulateByCenterLine(ModularMesh mesh, Material material)
    {
        Polygon polygon = new Polygon();

        for(int i = 0; i < streetmember.Count; i++)
        {
            for (int j = 0; j < streetmember[i].CenterLine.Count - 1; j++)
            {
                polygon.Add(streetmember[i].CenterLine[j]);
            }
        }

        polygon.MakeClockwise();
        return polygon.Triangulate(mesh, material);
    }
    public PolygonSurface Triangulate(ModularMesh mesh, Material material)
    {
        Polygon polygon = new Polygon();

        if (Type == StreetPolygonType.LeftHand)
        {
            for (int i = 0; i < streetmember.Count; i++)
            {
                polygon.Add(streetmember[i].LeftOutline);
            }
        }
        else
        {
            for (int i = 0; i < streetmember.Count; i++)
            {
                polygon.Add(streetmember[i].RightOutline); 
            }
        }
        polygon.MakeClockwise();
        return polygon.Triangulate(mesh, material);
    }
    #region StreetPolygon Listlike
    public int Count
    {
        get { return streetmember.Count; }
    }
    public Street this[int i]
    {
        get { return streetmember[i].Street; }
    }
    public Street First
    {
        get
        {
            if (streetmember.Count > 0)
                return streetmember[0].Street;
            return null;
        }
    }
    public Street Last
    {
        get
        {
            if (streetmember.Count > 0)
                return streetmember[streetmember.Count - 1].Street;
            return null;
        }
    }
    //public StreetPolygon Reversed()
    //{
    //    List<StreetPolygonMember> reversedStreets = new List<StreetPolygonMember>(streetmember);
    //    reversedStreets.Reverse();
    //    return new StreetPolygon(reversedStreets);
    //}
    public void Clear()
    {
        streetmember.Clear();
    }
    #endregion

    class StreetPolygonMember
    {
        public StreetPolygonMember(Street street, Intersection intersection)
        {
            this.street = street;
            if (street.From == intersection)
                type = StreetPolygonMemberType.Forwards;
            else
                type = StreetPolygonMemberType.Backwards;
        }

        public Street Street
        {
            get { return street; }
        }
        public StreetPolygonMemberType Type
        {
            get { return type; }
        }

        Street street;
        StreetPolygonMemberType type;

        #region streetPropertyWrapper
        public Intersection From
        {
            get
            {
                if (type == StreetPolygonMemberType.Backwards)
                    return street.To;
                return street.From;
            }
        }
        public Intersection To
        {
            get 
            {
                if (type == StreetPolygonMemberType.Backwards)
                    return street.From;
                return street.To;
            }
 
        }
        public Vertex Beginning
        {
            get
            {
                if (Type == StreetPolygonMemberType.Backwards)
                    return Street.End;
                return Street.Beginning;
            }

        }
        public Vertex End
        {
            get
            {
                if (Type == StreetPolygonMemberType.Backwards)
                    return Street.Beginning;
                return Street.End;
            }

        }
        public Vertex BeginningLeft
        {
            get
            {
                if (Type == StreetPolygonMemberType.Backwards)
                    return Street.EndRight;
                return Street.BeginningLeft;
            }
        }
        public Vertex BeginningRight
        {
            get
            {
                if (Type == StreetPolygonMemberType.Backwards)
                    return Street.EndLeft;
                return Street.BeginningRight;
            }
        }
        public Vertex EndLeft
        {
            get
            {
                if (Type == StreetPolygonMemberType.Backwards)
                    return Street.BeginningRight;
                return Street.EndLeft;
            }
        }
        public Vertex EndRight
        {
            get
            {
                if (Type == StreetPolygonMemberType.Backwards)
                    return Street.BeginningLeft;
                return Street.EndRight;
            }
        }

        public Polyline CenterLine
        {
            get 
            {
                if (type == StreetPolygonMemberType.Backwards)
                    return street.CenterLine.Reversed();
                return street.CenterLine;
                
            }
        }
        public Polyline LeftOutline
        {
            get 
            {
                if (type == StreetPolygonMemberType.Backwards)
                    return street.RightOutline;
                return street.LeftOutline;
            }
        }
        public Polyline RightOutline
        {
            get 
            {
                if (type == StreetPolygonMemberType.Backwards)
                    return street.LeftOutline;
                return street.RightOutline;
            }
        }
        #endregion
    }

    enum StreetPolygonMemberType
    {
        Forwards,
        Backwards
    }
    enum StreetPolygonType
    {
        RightHand,
        LeftHand
    }

    public void OnDrawGizmos()
    {
        foreach (StreetPolygonMember sM in streetmember)
        {
            sM.Street.OnDrawGizmos();
        }
    }
}

