﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using ProtoBuf;
using UnityEngine;

public static class Job
{
    #region String Konstanten

    public const string Master = "Master";
    public const string CreateJobMessage = "Create Job-Message";
    public const string SerializeJobMessage = "Serialize Job-Message";
    public const string PublishJob = "Publish Job";
    public const string DeserializeResult = "Deserialize Result";
    public const string RecreateScene = "Recreate Scene";
    public const string MasterGarbageCollection = "Trigger Garbage Collection (Master)";

    public const string Transfer = "Transfer";
    public const string TransferToWorker = "Transfer to Worker";
    public const string TransferToMaster = "Transfer to Master";


    public const string Worker = "Worker";
    public const string DeserializeJobMessage = "Deserialize JobMessage";
    public const string CreateNewScene = "Create new Scene";
    public const string CreateTile = "Create Tile";
    public const string StartOSMQuery = "Start OSM Query";
    public const string StartProcedural = "Start Procedural";
    public const string ProceduralPreparation = "Procedural Preparation";
    public const string CreateTerrain = "Create Terrain";
    public const string MeshPreparation = "Mesh Preparation";
    public const string TileQuad = "Create Background Quad";
    public const string River = "Create Rivers";
    public const string Ways = "Create Ways";
    public const string CreateBuildingMesh = "Create Buildings";
    public const string FillMeshDivideMaterials = "Fill Mesh separating Materials";
    public const string GarbageCollection = "Trigger Garbage Collection (Worker)";
    public const string ProceduralDone = "Procedural Done";
    public const string CreateReplyMessage = "Create Reply Message";
    public const string TidyUpScene = "Tidy up Scene";
    public const string PublishResult = "Publish Result";
    #endregion // String Konstanten
}

[ProtoContract]
public class OSMJobMessage
{
    [ProtoMember(1)]
    public int Job_ID;

    [ProtoMember(2)]
    public int x;
    [ProtoMember(3)]
    public int y;

    [ProtoMember(4)]
    public double tileWidth;
    [ProtoMember(5)]
    public double originLongitude;
    [ProtoMember(6)]
    public double originLatitude;

    [ProtoMember(7)]
    public string replyToQueue;
    [ProtoMember(8)]
    public string statusUpdateQueue;
    [ProtoMember(9)]
    public StatusUpdateMessage statusUpdateMessage;
    [ProtoMember(10)]
    public SerializationMethod method;

    public OSMJobMessage()
    {

    }

    public OSMJobMessage(int x, int y, double tileWidth, double originLongitude, double originLatitude, string replyQueueName, string statusUpdateQueue, StatusUpdateMessage statusUpdateMessage, SerializationMethod method)
    {
        this.x = x;
        this.y = y;
        this.tileWidth = tileWidth;
        this.originLongitude = originLongitude;
        this.originLatitude = originLatitude;
        this.replyToQueue = replyQueueName;
        this.statusUpdateQueue = statusUpdateQueue;
        this.statusUpdateMessage = statusUpdateMessage;
        this.method = method;
    }

    public static byte[] ToByteArray(OSMJobMessage jobMessage)
    {
        MemoryStream memStream = new MemoryStream();
        Serializer.Serialize(memStream, jobMessage);
        byte[] bytes = memStream.ToArray();
        return bytes;
    }

    public static OSMJobMessage FromByteArray(byte[] bytes)
    {
        MemoryStream outMemStream = new MemoryStream(bytes, 0, bytes.Length);
        OSMJobMessage objProto = (OSMJobMessage)Serializer.Deserialize<OSMJobMessage>(outMemStream);
        outMemStream.Close();
        
        return objProto;
    }
}
