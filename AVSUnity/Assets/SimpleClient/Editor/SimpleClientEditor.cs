using System.Collections.Generic;
using CymaticLabs.Unity3D.Amqp;
using UnityEditor;
using UnityEngine;

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

        EditorGUILayout.Toggle("IsConnecting?", this.client.isConnecting);
        EditorGUILayout.Toggle("IsConnected?", this.client.IsConnected);
        EditorGUILayout.Toggle("hasConnected?", this.client.hasConnected);
        EditorGUILayout.Toggle("isDisconnecting?", this.client.isDisconnecting);
        EditorGUILayout.Toggle("hasDisconnected?", this.client.hasDisconnected);
        EditorGUILayout.Toggle("isReconnecting?", this.client.isReconnecting);
        EditorGUILayout.Toggle("wasBlocked?", this.client.wasBlocked);
        EditorGUILayout.Toggle("hasAborted?", this.client.hasAborted);
        EditorGUILayout.Toggle("canSubscribe?", this.client.canSubscribe);



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


        // Save/serialized modified connection
        serializedObject.ApplyModifiedProperties();

        // Update the last connection index
        lastIndex = index;
    }

    #endregion Methods
}
