using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using CymaticLabs.Unity3D.Amqp;
using ProtoBuf;
using UnityEngine;


[ProtoContract]
public enum Status
{
    PENDING = 0,
    IN_PROGRESS = 1,
    DONE = 2
}

#region TimeStampedObject
[ProtoContract]
[ProtoInclude(9, typeof(StatusUpdateMessage))]
public class TodoItem
{
    [ProtoMember(1)]
    public string name;

    [ProtoMember(2, IsRequired = true)]
    public Status status;

    [ProtoMember(3)]
    public TimeStamp startTime;

    [ProtoMember(4)]
    public bool priorisedStart = false;

    [ProtoMember(5)]
    public TimeStamp endTime;

    [ProtoMember(6)]
    public bool priorisedEnd = false;

    [ProtoMember(7)]
    public List<string> childTodos = new List<string>();

    [ProtoMember(8)]
    public Dictionary<string, TodoItem> childDict = new Dictionary<string, TodoItem>();
    public TodoItem()
    {

    }

    public TodoItem(string stepName)
    {
        this.name = stepName;
        this.status = Status.PENDING;
    }

    #region Start-Stop-Duration
    public void Start(TimeStamp timeStamp)
    {
        Start();
        this.startTime = timeStamp;
    }

    public void Start()
    {
        switch (this.status)
        {
            case Status.PENDING:
                this.startTime = new TimeStamp();
                break;
            case Status.IN_PROGRESS:
                Debug.LogWarning(this.name + " already started!");
                break;
            case Status.DONE:
                Debug.LogWarning(this.name + " already done!");
                break;
            default:
                break;
        }

        this.status = Status.IN_PROGRESS;
    }

    public void Stop(TimeStamp timeStamp)
    {
        Stop();
        this.endTime = timeStamp;
    }

    public void Stop()
    {
        switch (this.status)
        {
            case Status.PENDING:
                Debug.LogWarning(this.name + " had not been started!");
                this.priorisedStart = true;
                this.startTime = new TimeStamp();
                this.endTime = new TimeStamp();
                break;
            case Status.IN_PROGRESS:
                this.endTime = new TimeStamp();
                break;
            case Status.DONE:
                Debug.LogWarning(this.name + " already stopped!");
                break;
            default:
                break;
        }
        this.status = Status.DONE;
        Debug.Log("Set " + this.name + " to Done!");
    }

    public double Duration()
    {
        if (this.startTime == null)
            return 0;
        if (this.endTime == null)
            return this.startTime.DurationSince().TotalMilliseconds;

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
    #endregion // Start-Stop-Duration

    #region ChildTodos
    public TodoItem AddTodo(string todoName)
    {
        TodoItem step;
        if (childDict.TryGetValue(todoName, out step))
        {
            Debug.LogError("A Child-Element with the same Name " + todoName + " already exists!");
            return null;
        }
        step = new TodoItem(todoName);
        childTodos.Add(todoName);
        childDict.Add(todoName, step);
        return step;
    }

    public void RemoveTodo(string todoName)
    {
        TodoItem step;
        if (!childDict.TryGetValue(todoName, out step))
        {
            Debug.LogError("A Child-Element with the Name '" + todoName + "' does not exist!");
            return;
        }
        childTodos.Remove(todoName);
        childDict.Remove(todoName);
    }

    public TodoItem Get(string todoName)
    {
        TodoItem step;
        if (this.name == todoName)
            return this;
        else
        {
            if (!childDict.TryGetValue(todoName, out step))
            {
                foreach (string item in childTodos)
                {
                    step = childDict[item].Get(todoName);
                    if (step != null)
                        return step;
                }
                //Debug.LogError("A Child-Element with the Name '" + todoName + "' does not exist!");
                return null;
            }
        }
        return step;
    }
  
    public void Start(string stepName)
    {
        TodoItem step;
        if (!childDict.TryGetValue(stepName, out step))
        {
            foreach (TodoItem item in childDict.Values)
                item.Start(stepName);
        }
        else
        {
            step.Start();
        }
    }

    public void Stop(string stepName)
    {
        TodoItem step;
        if (!childDict.TryGetValue(stepName, out step))
        {
            foreach (TodoItem item in childDict.Values)
                item.Stop(stepName);
        }
        else
        {
            step.Stop();
        }
    }

    public void Merge(TodoItem msg)
    {
        TodoItem local = this;
        TodoItem remote = msg;

        switch (local.status)
        {
            case Status.PENDING:
                switch (remote.status)
                {
                    case Status.PENDING:
                        Debug.Log(local.name + ": local Pending - remote Pending");

                        break;
                    case Status.IN_PROGRESS:
                        Debug.Log(local.name + ": local Pending - remote InProgress");
                        this.childDict[local.name] = remote;
                        break;
                    case Status.DONE:
                        Debug.Log(local.name + ": local Pending - remote Done");
                        this.childDict[local.name] = remote;
                        break;
                    default:
                        break;
                }
                break;
            case Status.IN_PROGRESS:
                switch (remote.status)
                {
                    case Status.PENDING:
                        Debug.Log(local.name + ": local InProgress - remote Pending");
                        this.childDict[local.name] = local;
                        break;
                    case Status.IN_PROGRESS:
                        Debug.Log(local.name + ": local InProgress - remote InProgress");
                        if (local.startTime != remote.startTime)
                        {
                            if (local.priorisedStart)
                                msg.childDict[local.name].startTime = remote.startTime;
                            else if (remote.priorisedStart)
                                msg.childDict[local.name].startTime = local.startTime;
                        }
                        if (local.endTime != remote.endTime)
                        {
                            if (local.priorisedEnd)
                                msg.childDict[local.name].endTime = remote.endTime;
                            else if (remote.priorisedEnd)
                                msg.childDict[local.name].endTime = local.endTime;
                        }
                        break;
                    case Status.DONE:
                        Debug.Log(local.name + ": local InProgress - remote Done");
                        if (local.startTime != remote.startTime)
                        {
                            if (local.priorisedStart)
                                msg.childDict[local.name].startTime = remote.startTime;
                            else if (remote.priorisedStart)
                                msg.childDict[local.name].startTime = local.startTime;
                        }
                        if (local.endTime != remote.endTime)
                        {
                            if (local.priorisedEnd)
                                msg.childDict[local.name].endTime = remote.endTime;
                            else if (remote.priorisedEnd)
                                msg.childDict[local.name].endTime = local.endTime;
                        }
                        break;
                    default:
                        break;
                }
                break;
            case Status.DONE:
                switch (remote.status)
                {
                    case Status.PENDING:
                        Debug.Log(local.name + ": local Done - remote Pending");
                        this.childDict[local.name] = local;
                        break;
                    case Status.IN_PROGRESS:
                        Debug.Log(local.name + ": local Done - remote InProgress");
                        this.childDict[local.name] = local;
                        break;
                    case Status.DONE:
                        Debug.Log(local.name + ": local Done - remote Done");
                        if (local.startTime != remote.startTime)
                        {
                            if (local.priorisedStart)
                            {
                                msg.childDict[local.name].startTime = remote.startTime;
                            }
                            else if (remote.priorisedStart)
                            {
                                msg.childDict[local.name].startTime = local.startTime;
                            }
                        }
                        if (local.endTime != remote.endTime)
                        {
                            if (local.priorisedEnd)
                            {
                                msg.childDict[local.name].endTime = remote.endTime;
                            }
                            else if (remote.priorisedEnd)
                            {
                                msg.childDict[local.name].endTime = local.endTime;
                            }
                        }
                        break;
                    default:
                        break;
                }
                break;
            default:
                break;
        }

        foreach (string itemName in childTodos)
        {
            TodoItem localNext = this.childDict[itemName];
            TodoItem remoteNext = msg.childDict[itemName];

            localNext.Merge(remoteNext);

        }
    }
    #endregion // ChildTodos

    #region ToString
    private string Header()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(this.name).Append("(");

        switch (this.status)
        {
            case Status.PENDING:
                sb.Append("<color=red>").Append(this.status.ToString()).Append("</color>");
                break;
            case Status.IN_PROGRESS:
                sb.Append("<color=yellow>").Append(this.status.ToString()).Append("</color>");
                break;
            case Status.DONE:
                sb.Append("<color=green>").Append(this.status.ToString()).Append("</color>");
                break;
            default:
                break;
        }
        sb.Append(") ");
        if (this.startTime != null)
            sb.Append(this.startTime.ToString());
        else
            sb.Append("not started");

        sb.Append(" -> ");

        if (this.endTime != null)
            sb.Append(this.endTime.ToString());
        else
            sb.Append("not ended");

        sb.Append("  {").Append(this.Duration()).Append("}");
        return sb.ToString();
    }

    public string ToString(int tabLevel)
    {
        tabLevel++;
        StringBuilder sb = new StringBuilder();
        sb.Append(new String('\t', tabLevel)).Append(this.Header()).AppendLine();
        tabLevel++;
        foreach (string item in childTodos)
        {
            sb.Append(childDict[item].ToString(tabLevel));
        }

        return sb.ToString();
    }

    public override string ToString()
    {
        return this.ToString(0);
    }

    protected static TodoItem Merge(TodoItem local, TodoItem remote)
    {
        switch (local.status)
        {
            case Status.PENDING:
                #region Pending
                switch (remote.status)
                {
                    case Status.PENDING:
                        // Nothing to be done.
                        break;
                    case Status.IN_PROGRESS:
                        local.status = Status.IN_PROGRESS;
                        local.startTime = remote.startTime;
                        break;
                    case Status.DONE:
                        local.status = Status.DONE;
                        local.startTime = remote.startTime;
                        local.endTime = remote.endTime;
                        break;
                    default:
                        break;
                }
                #endregion Pending
                break;
            case Status.IN_PROGRESS:
                #region In Progress
                switch (remote.status)
                {
                    case Status.PENDING:
                        // Nothing to be done.
                        break;
                    case Status.IN_PROGRESS:
                        if (local.startTime != remote.startTime)
                        {
                            if (local.startTime.CompareTo(remote.startTime) > 0)
                            {
                                local.startTime = remote.startTime;
                            }
                        }
                        break;
                    case Status.DONE:
                        local.status = Status.DONE;
                        if (local.startTime != remote.startTime)
                        {
                            if (local.startTime.CompareTo(remote.startTime) > 0)
                            {
                                local.startTime = remote.startTime;
                            }
                        }
                        local.endTime = remote.endTime;

                        break;
                    default:
                        break;
                }
                #endregion In Progress
                break;
            case Status.DONE:
                #region done
                switch (remote.status)
                {
                    case Status.PENDING:
                        // nothing to be done
                        break;
                    case Status.IN_PROGRESS:
                        // nothing to be done
                        break;
                    case Status.DONE:
                        if (local.startTime != remote.startTime)
                        {
                            if (local.startTime.CompareTo(remote.startTime) > 0)
                            {
                                local.startTime = remote.startTime;
                            }
                        }
                        if (local.endTime != remote.startTime)
                        {
                            if (local.startTime.CompareTo(remote.startTime) > 0)
                            {
                                local.startTime = remote.startTime;
                            }
                        }
                        break;
                    default:
                        break;
                }
                #endregion done
                break;
            default:
                break;
        }

        foreach (string item in local.childTodos)
        {
            TodoItem newLocal = local.childDict[item];
            TodoItem newRemote = remote.childDict[item];

            local.childDict[item] = TodoItem.Merge(newLocal, newRemote);
        }
        return local;
    }
    #endregion // ToString
}
#endregion // TimeStampedObject


[ProtoContract]
public class StatusUpdateMessage : TodoItem
{
    [ProtoMember(10)]
    public int jobID;
    
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
    #endregion Serialization/Deserialization

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(this.jobID).Append(":").Append(this.name).Append("(").Append(this.status.ToString()).Append(") [\n");
        for (int i = 0; i < childTodos.Count; i++)
        {
            TodoItem item = childDict[childTodos[i]];
            sb.Append("\t").Append(item.ToString()).AppendLine();
        }
        sb.Append("\n");
        return sb.ToString();
    }

    public static StatusUpdateMessage Merge(StatusUpdateMessage local, StatusUpdateMessage remote)
    {
        switch (local.status)
        {
            case Status.PENDING:
                #region Pending
                switch (remote.status)
                {
                    case Status.PENDING:
                        // Nothing to be done.
                        break;
                    case Status.IN_PROGRESS:
                        local.status = Status.IN_PROGRESS;
                        local.startTime = remote.startTime;
                        break;
                    case Status.DONE:
                        local.status = Status.DONE;
                        local.startTime = remote.startTime;
                        local.endTime = remote.endTime;
                        break;
                    default:
                        break;
                }
                #endregion Pending
                break;
            case Status.IN_PROGRESS:
                #region In Progress
                switch (remote.status)
                {
                    case Status.PENDING:
                        // Nothing to be done.
                        break;
                    case Status.IN_PROGRESS:
                        if (local.startTime != remote.startTime)
                        {
                            if (local.startTime.CompareTo(remote.startTime) > 0)
                                local.startTime = remote.startTime;
                        }
                        break;
                    case Status.DONE:
                        local.status = Status.DONE;
                        if (local.startTime != remote.startTime)
                        {
                            if (local.startTime.CompareTo(remote.startTime) > 0)
                                local.startTime = remote.startTime;
                        }
                        local.endTime = remote.endTime;
                        
                        break;
                    default:
                        break;
                }
                #endregion In Progress
                break;
            case Status.DONE:
                #region done
                switch (remote.status)
                {
                    case Status.PENDING:
                        // nothing to be done
                        break;
                    case Status.IN_PROGRESS:
                        // nothing to be done
                        break;
                    case Status.DONE:
                        if (local.startTime != remote.startTime)
                        {
                            if (local.startTime.CompareTo(remote.startTime) > 0)
                                local.startTime = remote.startTime;
                        }
                        if (local.endTime != remote.startTime)
                        {
                            if (local.startTime.CompareTo(remote.startTime) > 0)
                                local.startTime = remote.startTime;
                        }
                        break;
                    default:
                        break;
                }
                #endregion done
                break;
            default:
                break;
        }

        foreach (string item in local.childTodos)
        {
            TodoItem newLocal = local.childDict[item];
            TodoItem newRemote = remote.childDict[item];

            local.childDict[item] = TodoItem.Merge(newLocal, newRemote);
        }
        return local;
    }
}
