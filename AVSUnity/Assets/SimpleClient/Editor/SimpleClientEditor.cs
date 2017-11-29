using System;
using System.Collections.Generic;
using System.Diagnostics;
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


            //if (GUILayout.Button("Subscribe", GUILayout.Width(30)))
            //{
            //    //this.client.SubscribeToQueue(new AmqpQueueSubscription(queue.Name, true, this.client.UnityEventDebugQueueMessageHandler()));
            //}

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
        //AmqpQueue[] queues = this.client.GetQueues();
        //string[] names = new string[queues.Length];
        //for (int i = 0; i < queues.Length; i++)
        //    names[i] = queues[i].Name;

        //selectedQueue = EditorGUILayout.Popup("Queue", selectedQueue, names);
        //if (queues.Length != 0)
        //{
        //    if (GUILayout.Button("Subscribe to " + names[this.selectedQueue] + " Queue"))
        //        this.client.SubscribeToQueue(names[this.selectedQueue]);
        //}
        #endregion // Subscribe Queue


        if (this.client.ServerMode)
        {
            #region JobQueue 
            string[] jobQueueNames = new string[this.client.GetQueues().Length];
            for (int i = 0; i < this.client.GetQueues().Length; i++)
                jobQueueNames[i] = this.client.GetQueues()[i].Name;

            this.jobQueueIndex = EditorGUILayout.Popup("JobQueue", this.jobQueueIndex, jobQueueNames);
            string jobQueueName = this.client.GetQueues().Length == 0 ? "not set" : jobQueueNames[this.jobQueueIndex];
            EditorGUILayout.LabelField("JobQueue", jobQueueName);
            #endregion // JobQueue

            #region JobQueue 
            string[] replyToQueueNames = new string[this.client.GetQueues().Length];
            for (int i = 0; i < this.client.GetQueues().Length; i++)
                replyToQueueNames[i] = this.client.GetQueues()[i].Name;

            this.replyQueueIndex = EditorGUILayout.Popup("ReplyToQueue", this.replyQueueIndex, replyToQueueNames);
            string replyQueueName = this.client.GetQueues().Length == 0 ? "not set" : replyToQueueNames[this.replyQueueIndex];
            EditorGUILayout.LabelField("ReplyToQueue", replyQueueName);
            #endregion // JobQueue


            this.xMax = EditorGUILayout.IntSlider("xMax", this.xMax, 1, 10);
            this.zMax = EditorGUILayout.IntSlider("zMax", this.zMax, 1, 10);

            EditorGUILayout.BeginVertical("box");
            TileManager.TileWidth = (double)EditorGUILayout.FloatField("TileWidth", (float)TileManager.TileWidth);
            TileManager.tileRadius = EditorGUILayout.IntField("TileRadius", TileManager.tileRadius);
            TileManager.LOD = EditorGUILayout.IntField("LOD", TileManager.LOD);
            TileManager.OriginLatitude = (double)EditorGUILayout.FloatField("OriginLatitude", (float)TileManager.OriginLatitude);
            TileManager.OriginLongitude = (double)EditorGUILayout.FloatField("OriginLongitude", (float)TileManager.OriginLongitude);
            EditorGUILayout.EndVertical();


            if (GUILayout.Button("Send Job-Messages"))
            {
                // make sure we have subscribed the replyQueue
                this.client.SubscribeToQueue(replyQueueName);

                for (int x = 0; x < this.xMax; x++)
                {
                    for (int z = 0; z < this.zMax; z++)
                    {
                        JobMessage jobMessage = new JobMessage(x, z, replyQueueName);
                        string jsonMessage = jobMessage.ToJson();
                        this.client.PublishToQueue(jobQueueName, jsonMessage);
                        Debug.Log("Created Job-Message for (" + x + "," + z + "): " + jsonMessage);
                    }
                }
            }

            if (GUILayout.Button("Send OSM-Job-Messages"))
            {
                this.client.SendOSMJobMessages(
                    jobQueueName,
                    replyQueueName,
                    TileManager.tileRadius,
                    TileManager.TileWidth,
                    TileManager.OriginLongitude,
                    TileManager.OriginLatitude);
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

        if (this.client.osmJobs.Count > 0)
        {
            GUI.backgroundColor = Color.white;
            EditorGUILayout.BeginVertical("box");
            {
                GUI.backgroundColor = Color.grey;
                foreach (OSMJobMessage osmJob in this.client.osmJobs)
                {
                    EditorGUILayout.BeginVertical("box");
                    {
                        bool replyReceived = false;
                        SceneMessage receivedReply = null;
                        foreach (SceneMessage reply in this.client.sceneMessages)
                        {
                            if (reply.messageText == (osmJob.x + "/" + osmJob.y))
                            {
                                receivedReply = reply;
                                replyReceived = true;
                                break;
                            }
                        }
                        if (replyReceived)
                            GUI.backgroundColor = Color.green;
                        else
                            GUI.backgroundColor = Color.grey;

                        EditorGUILayout.LabelField("Jobname", osmJob.x + "/" + osmJob.y);
                        EditorGUILayout.LabelField("replyQueue", osmJob.replyToQueue);
                        EditorGUILayout.LabelField("started at", new DateTime(osmJob.timeStamp).ToString());

                        EditorGUILayout.Toggle(replyReceived, "Received reply");
                        if (replyReceived)
                        {
                            EditorGUILayout.LabelField("Size of Message:", receivedReply.sceneBytes.Length.ToString());
                        }
                    }
                    EditorGUILayout.EndVertical();
                }
            }
            EditorGUILayout.EndVertical();
        }
       
        // Save/serialized modified connection
        serializedObject.ApplyModifiedProperties();

        // Update the last connection index
        lastIndex = index;
    }

    #endregion Methods
}
