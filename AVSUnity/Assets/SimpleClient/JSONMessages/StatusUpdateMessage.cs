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

    [ProtoMember(5)]
    public List<TodoItem> childTodos = new List<TodoItem>();

    [ProtoMember(6)]
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
                break;
            case Status.DONE:
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
                break;
            case Status.IN_PROGRESS:
                this.endTime = new TimeStamp();
                break;
            case Status.DONE:
                break;
            default:
                break;
        }
        this.status = Status.DONE;
    }

    public double Duration()
    {
        if (this.startTime == null )
        {
            return 0;
        }
        if (this.endTime == null)
        {
            return this.startTime.DurationSince().TotalMilliseconds;
        }
        
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
        childTodos.Add(step);
        childDict.Add(todoName, step);
        return step;
    }

    public void ReplaceTodo(string todoName, TodoItem item)
    {
        TodoItem step;
        if (!childDict.TryGetValue(todoName, out step))
        {
            Debug.LogError("A Child-Element with the Name '" + todoName + "' does not exist!");
            return;
        }
        int index = childTodos.IndexOf(step);
        childTodos.RemoveAt(index);
        childTodos.Insert(index, item);

        childDict[todoName] = item;
    }

    public void RemoveTodo(string todoName)
    {
        TodoItem step;
        if (!childDict.TryGetValue(todoName, out step))
        {
            Debug.LogError("A Child-Element with the Name '" + todoName + "' does not exist!");
            return;
        }
        childTodos.Remove(step);
        childDict.Remove(todoName);
    }

    public TodoItem Get(string todoName)
    {
        TodoItem step;
        if (!childDict.TryGetValue(todoName, out step))
        {
            foreach (TodoItem item in childTodos)
            {
                step = Get(todoName);
                if (step != null)
                {
                    return step;
                }
            }
            Debug.LogError("A Child-Element with the Name '" + todoName + "' does not exist!");
            return null;
        }
        return step;
    }

    //public StatusUpdateMessage Merge(StatusUpdateMessage other)
    //{
    //    if (this.status != other.Get(this.name).status)
    //    {
    //        if (other.status == Status.DONE)
    //        {
    //            if ((DateTime)other.endTime <(DateTime)this.endTime)
    //            {
    //                this.
    //            }
    //            this.endTime = other.endTime;
    //        }
    //    }

    //    foreach (TodoItem item in )
    //    {

    //    }
    //    return this;
    //}

    public void Start(string stepName)
    {
        TodoItem step;

        if (childDict.TryGetValue(stepName, out step))
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

        if (childDict.TryGetValue(stepName, out step))
        {
            step.Stop();
            //bool pendingOrRunningItems = false;
            //foreach (TodoItem item in childDict.Values)
            //{
            //    if (item.status == Status.IN_PROGRESS || item.status == Status.PENDING)
            //    {
            //        pendingOrRunningItems = true;
            //        break;
            //    }
            //}
            //if (!pendingOrRunningItems)
            //{
            //    this.status = Status.DONE;
            //}
        }
        else
        {
            Debug.LogWarning("No Step of that name available!");
        }
    }
    #endregion // ChildTodos

    #region ToString

    private string Header()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(this.name).Append("(").Append(this.status.ToString()).Append(") ");
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
        foreach (TodoItem item in childTodos)
        {
            sb.Append(item.ToString(tabLevel));
        }

        return sb.ToString();
    }

    public override string ToString()
    {
        return this.ToString(0);
    }
    #endregion // ToString
}
#endregion // TimeStampedObject



[ProtoContract]
public class StatusUpdateMessage : TodoItem
{
    [ProtoMember(7)]
    public int jobID;

    public SimpleClient client;

    public void SendStatusUpdateMessages()
    {
        this.client.PublishToQueue(SimpleClient.statusUpdateQueueName, this.Serialize());
    }


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
            TodoItem item = childTodos[i];
            sb.Append("\t").Append(item.ToString()).AppendLine();
        }
        sb.Append("\n");
        return sb.ToString();
    }
}
