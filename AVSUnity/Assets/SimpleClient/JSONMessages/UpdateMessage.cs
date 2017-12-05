using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using ProtoBuf;
using UnityEngine;


[ProtoContract]
public enum Status
{
    //[ProtoEnum]
    PENDING = 0,
    //[ProtoEnum]
    IN_PROGRESS = 1,
    //[ProtoEnum]
    DONE = 2
}

#region TimeStampedObject
[ProtoContract]
[ProtoInclude(8, typeof(StatusUpdateMessage))]
public class TodoItem
{
    [ProtoMember(1)]
    public string name;

    [ProtoMember(2, IsRequired = true)]
    public Status status;

    [ProtoMember(3)]
    public TimeStamp startTime;

    [ProtoMember(4)]
    public TimeStamp endTime;

    public TodoItem()
    {
       
    }
    
    public TodoItem(string stepName)
    {
        this.name = stepName;
        this.status = Status.PENDING;
    }

    public void Start()
    {
        if (this.status == Status.PENDING)
            this.startTime = new TimeStamp();
        this.status = Status.IN_PROGRESS;
    }

    public void Stop()
    {
        if (this.status == Status.IN_PROGRESS)
        {
            this.endTime = new TimeStamp();
        }
        this.status = Status.DONE;
    }

    public double Duration()
    {
        switch (this.status)
        {
            case Status.PENDING:
                return 0;
            case Status.IN_PROGRESS:
                return this.startTime.DurationSince().TotalMilliseconds;
            case Status.DONE:
                return TimeStamp.Duration(this.startTime, this.endTime).TotalMilliseconds;
            default:
                return 0;
        }
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(this.name).Append("(").Append(this.status.ToString()).Append(")");
        if (this.startTime != null)
            sb.Append(this.startTime.ToString());
        else
            sb.Append("now");

        sb.Append(" -> ");

        if (this.endTime != null)
            sb.Append(this.endTime.ToString());
        else
            sb.Append("now");

        sb.Append("  {").Append(this.Duration()).Append("}");
        return sb.ToString();
    }
}
#endregion // TimeStampedObject

//[DataContract]
[ProtoContract]
public class StatusUpdateMessage : TodoItem
{
    //[DataMember]
    [ProtoMember(5)]
    public int jobID;

    //[DataMember]
    [ProtoMember(6)]
    public List<TodoItem> orderedSteps = new List<TodoItem>();

    //[DataMember]
    [ProtoMember(7)]
    public Dictionary<string, TodoItem> stepDict = new Dictionary<string, TodoItem>();


    public StatusUpdateMessage()
    {
       
    }

    public StatusUpdateMessage(int jobID, string name) : this()
    {
        this.jobID = jobID;
        this.name = name;
    }

    #region Serialization/Deserialization
    public static StatusUpdateMessage Deserialize(byte[] bytes)
    {
        MemoryStream outMemStream = new MemoryStream(bytes, 0, bytes.Length);
        StatusUpdateMessage updateMessage = (StatusUpdateMessage)Serializer.Deserialize<StatusUpdateMessage>(outMemStream);
        outMemStream.Close();
        return updateMessage;
    }


    public byte[] Serialize()
    {
        MemoryStream memStream = new MemoryStream();
        Serializer.Serialize<StatusUpdateMessage>(memStream, this);
        byte[] bytes = memStream.ToArray();
        memStream.Close();
        return bytes;
    }

    public void AddStep(string stepName)
    {
        TodoItem step = new TodoItem(stepName);
        orderedSteps.Add(step);
        stepDict.Add(stepName, step);
    }
    #endregion Serialization/Deserialization

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(this.jobID).Append(":").Append(this.name).Append("(").Append(this.status.ToString()).Append(") [\n");
        for (int i = 0; i < orderedSteps.Count; i++)
        {
            TodoItem item = orderedSteps[i];
            sb.Append("\t").Append(item.ToString()).AppendLine();
        }
        sb.Append("\n");
        return sb.ToString();
    }

    public void Start(string stepName)
    {
        TodoItem step;

        if (stepDict.TryGetValue(stepName, out step))
        {
            if (this.status == Status.PENDING || this.status == Status.DONE)
            {
                this.status = Status.IN_PROGRESS;
            }
            step.Start();
        }
        else
        {
            Debug.LogWarning("No Step of that name available!");
        }
    }

    public void Stop(string stepName)
    {
        TodoItem step;

        if (stepDict.TryGetValue(stepName, out step))
        {
            step.Stop();
            bool pendingOrRunningItems = false;
            foreach (TodoItem item in stepDict.Values)
            {
                if (item.status == Status.IN_PROGRESS || item.status == Status.PENDING)
                {
                    pendingOrRunningItems = true;
                    break;
                }
            }
            if (!pendingOrRunningItems)
            {
                this.status = Status.DONE;
            }
        }
        else
        {
            Debug.LogWarning("No Step of that name available!");
        }
    }
}
