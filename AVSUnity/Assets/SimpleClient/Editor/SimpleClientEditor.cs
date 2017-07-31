﻿using System.Collections.Generic;
using System.Diagnostics;

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

    private string message;

    private Scene mainScene;
    private Scene newScene;

    public override void OnInspectorGUI()
    {
        // Update client
        serializedObject.Update();

        // Generate the connection dropdown options/content
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

        if (showAvailableQueues = EditorGUILayout.Foldout(showAvailableQueues, "Available Queues:"))
        {
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
        }

        this.queueName = EditorGUILayout.TextField("Queue Name", this.queueName);
        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Create Queue"))
            {
                this.client.DeclareQueue(this.queueName);
            }

            if (GUILayout.Button("Subscribe Queue"))
            {
                this.client.SubscribeToQueue(this.queueName);
            }

        }

        this.message = EditorGUILayout.TextField("Message", this.message);
        if (GUILayout.Button("Send Message"))
        {
            this.client.PublishToQueue(this.queueName, this.message);
        }

        if (GUILayout.Button("Send JSON-Message"))
        {


            Stopwatch sw = new Stopwatch();

            for (int x = 0; x < 3; x++)
            {
                for (int z = 0; z < 3; z++)
                {
                    sw.Start();

                    // Aktuelle Szene als MainScene merken
                    mainScene = EditorSceneManager.GetActiveScene();

                    // Neue (leere) Szene erstellen
                    newScene = EditorSceneManager.NewScene(
                        NewSceneSetup.EmptyScene,
                        NewSceneMode.Additive);

                    // Neue Szene als aktive Szene setzen
                    EditorSceneManager.SetActiveScene(this.newScene);

                    //###########################
                    // Erzeuge Content:
                    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.transform.position = new Vector3(x, 0f, z);
                    //###########################

                    // Szene speichern
                    string filename = SceneLoaderEditor.RelativeAssetPathTo("Scene_" + x + "_" + z + ".unity");
                    EditorSceneManager.SaveScene(newScene, filename);

                    // ByteArray für Message aus Szene erstellen
                    byte[] bytes = SceneLoaderEditor.SceneFileToByteArray(this.newScene);

                    // Filename must be send in some form... 
                    //
                    //      |
                    //      |
                    // Transfer via RabbitMQ-Message
                    //      |
                    //     \|/
                    //      v

                    Scene transferedScene = SceneLoaderEditor.ByteArrayToScene(filename, bytes);
                    EditorSceneManager.SetActiveScene(transferedScene);
                    EditorSceneManager.CloseScene(this.newScene, true);
                    sw.Stop();
                    Debug.Log("Szene " + x + "," + z + " took: " + sw.ElapsedMilliseconds + "ms");

                }

            }




            this.client.PublishToQueue(this.queueName, this.message);
        }



        // Save/serialized modified connection
        serializedObject.ApplyModifiedProperties();

        // Update the last connection index
        lastIndex = index;
    }

    #endregion Methods
}
