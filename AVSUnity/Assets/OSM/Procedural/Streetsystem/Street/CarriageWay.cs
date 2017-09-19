using UnityEngine;
#if UNITY_EDITOR
//using UnityEditor;
#endif
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// CarriageWay.
/// </summary>
public class CarriageWay : IStreetMember, IProceduralObjects 
{
    public CarriageWay()
    { }
    public CarriageWay(OSMWay way)
    {
        if (way.Tags["highway"] == "path" ||
            way.Tags["highway"] == "footway" ||
            way.Tags["highway"] == "pedestrian"
            )
        {
            streetMember.Add(new PathStreetMember());
        }
        else
        {
            if (way.Tags.ContainsKey("lanes"))
            {
                int lanes;
                if (int.TryParse(way.Tags["lanes"], out lanes))
                {
                    createCarLanes(lanes);
                }
            }
            else
            {
                createCarLanes(1);
            }

            //Gutter
            streetMember.Add(new Gutter());

            bool createFoot = false;
            bool createBike = false;

            if (way.Tags.ContainsKey("foot"))
            {
                if (way.Tags["foot"] == "yes" || way.Tags["foot"] == "designated" || way.Tags["foot"] == "1" || way.Tags["foot"] == "true")
                    createFoot = true;
            }
            if (way.Tags.ContainsKey("bicycle"))
            {
                if (way.Tags["bicycle"] == "yes" || way.Tags["bicycle"] == "designated" || way.Tags["bicycle"] == "1" || way.Tags["bicycle"] == "true")
                    createBike = true;
            }

            if (createFoot || createBike)
                streetMember.Add(new Curb());
            if (createBike)
                streetMember.Add(new BikeLane());
            if (createFoot)
                streetMember.Add(new WalkWay());
        }
    }

    public System.Type Type
    {
        get { return typeof(CarriageWay); }
    }

    public Polyline LeftOutline
    {
        get { return leftOutline; }
        set { leftOutline = value; }
    }
    public Polyline RightOutline
    {
        get 
        {
            if(streetMember.Count > 0)
            {
                return streetMember[streetMember.Count - 1].RightOutline;
            }
            if (leftOutline)
                return leftOutline;
            return null;
        }
    }

    public float StartParameter
    {
        get
        {
            return startParameter;
        }
        set
        {
            startParameter = value;
        }
    }
    public float EndParameter
    {
        get
        {
            return endParameter;
        }
        set
        {
            endParameter = value;
        }
    }

    private Polyline leftOutline;

    private float startParameter = 0f;
    private float endParameter = 1f;

    public void CreateMesh(ModularMesh mesh)
    {
        if (streetMember.Count > 0)
            streetMember[0].LeftOutline = leftOutline;
        for (int i = 0; i < streetMember.Count-1; i++)
        {
            streetMember[i].CreateMesh(mesh);
            streetMember[i + 1].LeftOutline = streetMember[i].RightOutline;
        }
        if (streetMember.Count > 0)
            streetMember[streetMember.Count - 1].CreateMesh(mesh);
    }
    public void UpdateMesh(ModularMesh mesh)
    {
        if (streetMember.Count > 0)
            streetMember[0].LeftOutline = leftOutline;
        for (int i = 0; i < streetMember.Count - 1; i++)
        {
            streetMember[i].UpdateMesh(mesh);
            streetMember[i + 1].LeftOutline = streetMember[i].RightOutline;
        }
        if (streetMember.Count > 0)
            streetMember[streetMember.Count - 1].UpdateMesh(mesh);
    }
    public void Destroy()
    {
        throw new System.NotImplementedException();
    }

    private void createCarLanes(int count)
    {
        for (int i = 0; i < count; i++)
        {
            streetMember.Add(new CarLane());
        }
    }

    public void AddStreetMember(IStreetMember sM)
    {
        streetMember.Add(sM);
    }
    private List<IStreetMember> streetMember = new List<IStreetMember>();

    public IStreetMember GetFirstStreetMemberOfType(Type type)
    {
        //Check if type implements IStreetMember (better use generics here?)
        if (!type.GetInterfaces().Contains(typeof(IStreetMember)))
            return null;
        for (int i = 0; i < streetMember.Count; i++)
        {
            if( streetMember[i].Type == type)
                return streetMember[i];
        }
        return null;
    }
    public IStreetMember GetLastStreetMemberOfType(Type type)
    {
        //Check if type implements IStreetMember (better use generics here?)
        if (!type.GetInterfaces().Contains(typeof(IStreetMember)))
            return null;
        for (int i = streetMember.Count-1; i >= 0; i--)
        {
            if (streetMember[i].Type == type)
                return streetMember[i];
        }
        return null;
    }
    public List<IStreetMember> GetStreetMembersOfType(Type type)
    {
        List<IStreetMember> result = new List<IStreetMember>();
        //Check if type implements IStreetMember (better use generics here?)
        if (!type.GetInterfaces().Contains(typeof(IStreetMember)))
            return result;
        
        for (int i = 0; i < streetMember.Count; i++)
        {
            if (streetMember[i].Type == type)
                result.Add(streetMember[i]);
        }
        return result;
    }
    

    #region Polyline Listlike
    public int Count
    {
        get { return streetMember.Count; }
    }
    
    public IStreetMember this[int i]
    {
        get { return streetMember[i]; }
    }
    public IStreetMember First
    {
        get
        {
            if (streetMember.Count > 0)
                return streetMember[0];
            return null;
        }
    }
    public IStreetMember Last
    {
        get
        {
            if (streetMember.Count > 0)
                return streetMember[streetMember.Count - 1];
            return null;
        }
    }
    public void Add(IStreetMember sM)
    {
        streetMember.Add(sM);
    }
    public void AddRange(IEnumerable<IStreetMember> streetMembers)
    {
        streetMember.AddRange(streetMembers);
    }
    public bool Remove(IStreetMember sM)
    {
        return streetMember.Remove(sM);
    }
    //public CarriageWay Reversed()
    //{
    //    List<Vertex> reversedPolylineVertices = new List<Vertex>(polylineVertices);
    //    reversedPolylineVertices.Reverse();
    //    return new Polyline(reversedPolylineVertices);
    //}
    public void Clear()
    {
        streetMember.Clear();
    }
    #endregion
    public static implicit operator bool(CarriageWay exists)
    {
        if (exists != null)
            return true;
        return false;
    }
}
