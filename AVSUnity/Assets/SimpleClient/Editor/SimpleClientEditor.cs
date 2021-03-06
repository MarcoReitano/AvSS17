﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;

using CymaticLabs.Unity3D.Amqp;
using UnityEditor;
using UnityEditor.SceneManagement;

using UnityEngine;
using UnityEngine.SceneManagement;

using Debug = UnityEngine.Debug;

[CustomEditor(typeof(SimpleClient))]
public class SimpleClientEditor : Editor
{

    #region Fields

    // The index of the selected connection
    int index = 0, lastIndex = 0;

    // The target instance being edited
    SimpleClient client;

    // The name of the selected connection
    SerializedProperty connection;

    #endregion Fields

    #region Methods

    private void OnEnable()
    {
        // Reference the selected client
        client = (SimpleClient)target;

        // Get a reference to the serialized connection property
        connection = serializedObject.FindProperty("Connection");

        // Load configuration data
        AmqpConfigurationEditor.LoadConfiguration();

        // Restore the connection index
        var connectionNames = AmqpConfigurationEditor.GetConnectionNames();

        for (var i = 0; i < connectionNames.Length; i++)
        {
            var cName = connectionNames[i];
            if (connection.stringValue == cName)
            {
                index = i;
                break;
            }
        }

        lightGreen = CustomGUIUtils.GetColorBackgroundStyle(XKCDColors.LightGreen);
        lightRed = CustomGUIUtils.GetColorBackgroundStyle(XKCDColors.LightRed);
        lightYellow = CustomGUIUtils.GetColorBackgroundStyle(XKCDColors.LightYellow);
    }


    private static bool showAvailableQueues;

    private string queueName;
    private string replyToQueue;

    private string message;

    private Scene mainScene;
    private Scene newScene;

    private int selectedQueue;

    private int jobQueueIndex;
    private int replyQueueIndex;
    private int statusUpdateQueueIndex;

    private int xMax = 3;
    private int zMax = 3;


    public override void OnInspectorGUI()
    {
        // Update client
        serializedObject.Update();

        this.client.ServerMode = EditorGUILayout.Toggle("Act as Server", this.client.ServerMode);

        // Generate the connection dropdown options/content
        #region Dropdown connections
        var connectionNames = AmqpConfigurationEditor.GetConnectionNames();
        var options = new List<GUIContent>();

        for (var i = 0; i < connectionNames.Length; i++)
        {
            var cName = connectionNames[i];
            if (string.IsNullOrEmpty(client.Connection) || client.Connection == cName)
                index = i;
            options.Add(new GUIContent(cName));
        }

        // Connections drop down
        string tooltip = "Select the AMQP connection to use. Connections can be configured in the AMQP/Configuration menu.";
        index = EditorGUILayout.Popup(new GUIContent("Connection", tooltip), index, options.ToArray());

        // If the index has changed, record the change
        if (index != lastIndex)
            Undo.RecordObject(target, "Undo Connection change");

        // Set the connection name based on dropdown value
        client.Connection = connection.stringValue = options[index].text;
        #endregion // Dropdown connections

        // Draw the rest of the inspector's default layout
        //DrawDefaultInspector();
        if (GUILayout.Button("Awake"))
        {
            this.client.Awake();
            this.Repaint();
        }
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("EnableUpdate"))
        {
            this.client.EnableUpdate();
            this.Repaint();
        }

        if (GUILayout.Button("DisableUpdate"))
        {
            this.client.DisableUpdate();
            this.Repaint();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Toggle("IsConnecting?", this.client.isConnecting);
        EditorGUILayout.Toggle("IsConnected?", this.client.IsConnected);
        EditorGUILayout.Toggle("hasConnected?", this.client.hasConnected);
        EditorGUILayout.Toggle("isDisconnecting?", this.client.isDisconnecting);
        EditorGUILayout.Toggle("hasDisconnected?", this.client.hasDisconnected);
        EditorGUILayout.Toggle("isReconnecting?", this.client.isReconnecting);
        EditorGUILayout.Toggle("wasBlocked?", this.client.wasBlocked);
        EditorGUILayout.Toggle("hasAborted?", this.client.hasAborted);
        EditorGUILayout.Toggle("canSubscribe?", this.client.canSubscribe);


        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Connect"))
        {
            this.client.Connect();
            this.Repaint();
        }

        if (GUILayout.Button("Disconnect"))
        {
            this.client.Disconnect();
            this.Repaint();
        }
        EditorGUILayout.EndHorizontal();

        #region Available Queues
        //if (showAvailableQueues = EditorGUILayout.Foldout(showAvailableQueues, "Available Queues:"))
        //{
        EditorGUILayout.BeginVertical("box");
        foreach (var queue in this.client.GetQueues())
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Name:", queue.Name);


            if (GUILayout.Button("Subscribe", GUILayout.Width(30)))
            {
                this.client.SubscribeToQueue(queue.Name);
            }

            if (GUILayout.Button("X", GUILayout.Width(30)))
            {
                this.client.DeleteQueue(queue.Name);
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();
        //}
        #endregion Available Queues

        #region Create Queue
        EditorGUILayout.BeginHorizontal("box");
        this.queueName = EditorGUILayout.TextField("Queue Name", this.queueName);
        if (GUILayout.Button("Create"))
        {
            this.client.DeclareQueue(this.queueName);
        }
        EditorGUILayout.EndHorizontal();
        #endregion // Create Queue

        #region Subscribe Queue
        AmqpQueue[] queues = this.client.GetQueues();
        string[] names = new string[queues.Length];
        for (int i = 0; i < queues.Length; i++)
            names[i] = queues[i].Name;

        selectedQueue = EditorGUILayout.Popup("Queue", selectedQueue, names);
        if (queues.Length != 0)
        {
            if (GUILayout.Button("Subscribe to " + names[this.selectedQueue] + " Queue"))
                this.client.SubscribeToQueue(names[this.selectedQueue]);
        }
        #endregion // Subscribe Queue


        if (this.client.ServerMode)
        {
            #region JobQueue 
            string[] jobQueueNames = new string[this.client.GetQueues().Length];
            for (int i = 0; i < this.client.GetQueues().Length; i++)
                jobQueueNames[i] = this.client.GetQueues()[i].Name;

            this.jobQueueIndex = EditorGUILayout.Popup("JobQueue", this.jobQueueIndex, jobQueueNames);
            for (int i = 0; i < jobQueueNames.Length; i++)
            {
                if (jobQueueNames[i] == "jobs")
                {
                    this.jobQueueIndex = i;
                    break;
                }
            }
            string jobQueueName = this.client.GetQueues().Length == 0 ? "not set" : jobQueueNames[this.jobQueueIndex];
            //EditorGUILayout.LabelField("JobQueue", jobQueueName);
            #endregion // JobQueue

            #region ReplyQueue 
            string[] replyToQueueNames = new string[this.client.GetQueues().Length];
            for (int i = 0; i < this.client.GetQueues().Length; i++)
                replyToQueueNames[i] = this.client.GetQueues()[i].Name;

            this.replyQueueIndex = EditorGUILayout.Popup("ReplyToQueue", this.replyQueueIndex, replyToQueueNames);
            for (int i = 0; i < replyToQueueNames.Length; i++)
            {
                if (replyToQueueNames[i] == "reply")
                {
                    this.replyQueueIndex = i;
                    break;
                }
            }
            string replyQueueName = this.client.GetQueues().Length == 0 ? "not set" : replyToQueueNames[this.replyQueueIndex];
            //EditorGUILayout.LabelField("ReplyToQueue", replyQueueName);
            #endregion // JobQueue

            #region JobQueue 
            string[] statusUpdateQueueNames = new string[this.client.GetQueues().Length];
            for (int i = 0; i < this.client.GetQueues().Length; i++)
                statusUpdateQueueNames[i] = this.client.GetQueues()[i].Name;

            this.statusUpdateQueueIndex = EditorGUILayout.Popup("ReplyToQueue", this.statusUpdateQueueIndex, statusUpdateQueueNames);
            for (int i = 0; i < statusUpdateQueueNames.Length; i++)
            {
                if (statusUpdateQueueNames[i] == "statusUpdates")
                {
                    this.statusUpdateQueueIndex = i;
                    break;
                }
            }
            string statusUpdateQueueName = this.client.GetQueues().Length == 0 ? "not set" : statusUpdateQueueNames[this.statusUpdateQueueIndex];
            //EditorGUILayout.LabelField("ReplyToQueue", replyQueueName);
            #endregion // JobQueue


            EditorGUILayout.BeginVertical("box");
            TileManager.TileWidth = (double)EditorGUILayout.FloatField("TileWidth", (float)TileManager.TileWidth);
            TileManager.tileRadius = EditorGUILayout.IntField("TileRadius", TileManager.tileRadius);
            TileManager.LOD = EditorGUILayout.IntField("LOD", TileManager.LOD);
            TileManager.OriginLatitude = (double)EditorGUILayout.FloatField("OriginLatitude", (float)TileManager.OriginLatitude);
            TileManager.OriginLongitude = (double)EditorGUILayout.FloatField("OriginLongitude", (float)TileManager.OriginLongitude);
            EditorGUILayout.EndVertical();

            this.client.method = (SerializationMethod)EditorGUILayout.EnumPopup("SerializationMethod", this.client.method);

            if (GUILayout.Button("Send OSM-Job-Messages"))
            {
                this.client.SendOSMJobMessages(
                    jobQueueName,
                    replyQueueName,
                    statusUpdateQueueName,
                    TileManager.tileRadius,
                    TileManager.TileWidth,
                    TileManager.OriginLongitude,
                    TileManager.OriginLatitude,
                    this.client.method);
            }

            if (GUILayout.Button("Generate locally"))
            {
                mainScene = EditorSceneManager.GetActiveScene();

                swComplete.Start();

                tiles = new List<Tile>();

                for (int i = -TileManager.tileRadius; i <= TileManager.tileRadius; i++)
                {
                    for (int j = -TileManager.tileRadius; j <= TileManager.tileRadius; j++)
                    {
                        jobs++;
                        Stopwatch sw = new Stopwatch();
                        sw.Reset();
                        sw.Start();

                        Tile newTile = Tile.CreateTileGO(i, j, 5);
                        tiles.Add(newTile);
                        tileJobsLocal.Add(i + "/" + j, newTile);
                        tileJobsStopwatchLocal.Add(i + "/" + j, sw);
                        //newTile.ProceduralDone += GenerationDone;
                        //newTile.StartQuery();

                        //EditorSceneManager.SaveScenes
                        //SceneManager.SetActiveScene(mainScene);
                    }
                }

                tiles[0].ProceduralDone += GenerationDone;
                tiles[0].StartQuery();
            }
        }
        else // Client-Mode
        {
            #region JobQueue 
            string[] jobQueueNames = new string[this.client.GetQueues().Length];
            for (int i = 0; i < this.client.GetQueues().Length; i++)
                jobQueueNames[i] = this.client.GetQueues()[i].Name;

            this.jobQueueIndex = EditorGUILayout.Popup("JobQueue", this.jobQueueIndex, jobQueueNames);
            string jobQueueName = this.client.GetQueues().Length == 0 ? "not set" : jobQueueNames[this.jobQueueIndex];
            EditorGUILayout.LabelField("JobQueue", jobQueueName);
            if (jobQueueNames.Length != 0)
            {
                if (GUILayout.Button("Subscribe to " + jobQueueNames[this.jobQueueIndex] + " Queue"))
                {
                    //this.client.BasicQos(1, 1, true);
                    this.client.SubscribeToQueue(jobQueueNames[this.jobQueueIndex]);
                }
            }

            #endregion // JobQueue
        }

        GUI.backgroundColor = Color.white;
        EditorGUILayout.BeginVertical("box");
        {
            foreach (StatusUpdateMessage item in SimpleClient.jobStatus.Values)
            {
                CustomGUIUtils.BeginGroup();
                {
                    switch (item.status)
                    {
                        case Status.PENDING:
                            statusStyle = lightRed;
                            break;
                        case Status.IN_PROGRESS:
                            statusStyle = lightYellow;
                            break;
                        case Status.DONE:
                            statusStyle = lightGreen;
                            break;
                        default:
                            statusStyle = GUIStyle.none;
                            break;
                    }
                    EditorGUILayout.BeginHorizontal(statusStyle);
                    EditorGUILayout.LabelField("" + item.jobID, GUILayout.Width(20f));
                    EditorGUILayout.LabelField("" + item.name, GUILayout.Width(80f));
                    EditorGUILayout.EndHorizontal();

                    foreach (string todo in item.childTodos)
                    {
                        CustomGUIUtils.BeginGroup();
                        TodoItem todoItem = item.childDict[todo];
                        switch (todoItem.status)
                        {
                            case Status.PENDING:
                                statusStyle = lightRed;
                                break;
                            case Status.IN_PROGRESS:
                                statusStyle = lightYellow;
                                break;
                            case Status.DONE:
                                statusStyle = lightGreen;
                                break;
                            default:
                                statusStyle = GUIStyle.none;
                                break;
                        }
                        EditorGUILayout.BeginHorizontal(statusStyle);
                        EditorGUILayout.LabelField(todoItem.name);
                        EditorGUILayout.EndHorizontal();
                        DrawTodoItem(todoItem);

                        CustomGUIUtils.EndGroup();
                    }
                }
                CustomGUIUtils.EndGroup();
            }
        }
        EditorGUILayout.EndVertical();

        // Save/serialized modified connection
        serializedObject.ApplyModifiedProperties();

        // Update the last connection index
        lastIndex = index;
    }

    GUIStyle lightGreen;
    GUIStyle lightRed;
    GUIStyle lightYellow;
    GUIStyle statusStyle;

    private void DrawTodoItem(TodoItem item)
    {
        CustomGUIUtils.BeginGroup();
        {
            switch (item.status)
            {
                case Status.PENDING:
                    statusStyle = lightRed;
                    break;
                case Status.IN_PROGRESS:
                    statusStyle = lightYellow;
                    break;
                case Status.DONE:
                    statusStyle = lightGreen;
                    break;
                default:
                    statusStyle = GUIStyle.none;
                    break;
            }
            EditorGUILayout.BeginHorizontal(statusStyle);
            EditorGUILayout.LabelField("" + item.name, GUILayout.Width(80f));
            EditorGUILayout.EndHorizontal();

            foreach (string todo in item.childTodos)
            {
                CustomGUIUtils.BeginGroup();
                DrawTodoItem(item.childDict[todo]);
                CustomGUIUtils.EndGroup();
            }
        }
        CustomGUIUtils.EndGroup();
    }


    private static void WaitForSeconds(int seconds)
    {
        Stopwatch sw = new Stopwatch();
        Debug.Log("Wait for " + seconds + " Seconds...");
        sw.Reset();
        sw.Start();
        while (sw.ElapsedMilliseconds < (seconds * 1000))
        {
            // wait
        }
        sw.Stop();
    }

    List<Tile> tiles;

    Dictionary<string, Stopwatch> tileJobsStopwatchLocal = new Dictionary<string, Stopwatch>();
    Dictionary<string, Tile> tileJobsLocal = new Dictionary<string, Tile>();
    int jobs = 0;
    Stopwatch swComplete = new Stopwatch();
    private void GenerationDone(object sender, EventArgs e)
    {
        jobs--;

        Tile tile = (Tile)sender;
        string tileName = tile.TileIndex[0] + "/" + tile.TileIndex[1];

        tile.ProceduralDone -= GenerationDone;
        if (tiles.Count > 1)
        {
            tiles.RemoveAt(0);
            tiles[0].ProceduralDone += GenerationDone;
            tiles[0].StartQuery();
        }

        Stopwatch sw = tileJobsStopwatchLocal[tileName];
        sw.Stop();
        Debug.Log(tileName + " Done! in " + sw.ElapsedMilliseconds + " ms");
        if (jobs == 0)
        {
            swComplete.Stop();
            Debug.Log("Completed Generation in " + swComplete.ElapsedMilliseconds);
        }
    }

    #endregion Methods
}
