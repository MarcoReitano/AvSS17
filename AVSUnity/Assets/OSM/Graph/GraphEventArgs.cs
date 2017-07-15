using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public enum GraphEventType
{
    NodeCreated,
    NodeDeleted,
    NodeChanged,
    
    EdgeCreated,
    EdgeDeleted,
    EdgeChanged,
    EdgeFromChanged,
    EdgeToChanged,
    EdgeEndsSwitched,

    BuildingCreated,
    BuildingDeleted,
    BuildingChanged,

    FloorCreated,
    FloorDeleted,
    FloorChanged,

    RoomCreated,
    RoomDeleted,
    RoomChanged,
    
    WallCreated,
    WallDeleted,
    WallChanged,
    WallOnStructureChanged,

    WallOpeningCreated,
    WallOpeningDeleted,
    WallOpeningChanged,
    
    SlabCreated,
    SlabDeleted,
    SlabChanged,
    
    SlabOpeningCreated,
    SlabOpeningDeleted,
    SlabOpeningChanged
}



public class GraphEventArgs : EventArgs
{
    private System.Object origin;
    public System.Object Origin
    {
        get { return origin; }
        set { origin = value; }
    }

    private System.Object lastSender;
    public System.Object LastSender
    {
        get { return lastSender; }
        set { lastSender = value; }
    }

    private int timeToLive;
    public int TimeToLive
    {
        get { return timeToLive; }
        set { timeToLive = value; }
    }

    private string message;
    public string Message
    {
        get { return message; }
        set { message = value; }
    }

    private GraphEventType graphEvent;
    public GraphEventType GraphEvent
    {
        get { return graphEvent; }
        set { graphEvent = value; }
    }


    public GraphEventArgs(System.Object origin, System.Object lastSender, GraphEventType graphEvent, int timeToLive, string message)
    {
        this.origin = origin;
        this.lastSender = lastSender;
        this.GraphEvent = graphEvent;
        this.timeToLive = timeToLive;
        this.message = message;
    }


    public GraphEventArgs nextLevelArgs(System.Object lastSender, GraphEventType graphEvent)
    {
        return new GraphEventArgs(origin, lastSender, graphEvent, this.timeToLive-1, this.message);
    }

    public GraphEventArgs nextLevelArgs(System.Object lastSender, GraphEventType graphEvent, string appendString)
    {
        GraphEventArgs eventArgs = new GraphEventArgs(origin, lastSender, graphEvent, this.timeToLive - 1, this.message + "\n" +timeToLive + ": " + appendString);
        //Debug.Log(eventArgs.ToString());
        return eventArgs;
    }


    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("Origin").Append(": ").Append(origin).Append("\n");
        sb.Append("LastSender").Append(": ").Append(lastSender).Append("\n");
        sb.Append("GraphEvent").Append(": ").Append(graphEvent.ToString()).Append("\n");
        sb.Append("TimeToLive").Append(": ").Append(timeToLive).Append("\n");
        sb.Append("Message").Append(": ").Append(message).Append("\n");

        return sb.ToString();
    }

}

