using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using CymaticLabs.Unity3D.Amqp;
using UnityEngine.SceneManagement;
using System;
using System.Diagnostics;
using System.Threading;
using Debug = UnityEngine.Debug;
using System.Globalization;
using System.Text;
/// <summary>
/// 
/// - Start Server
/// - Reset Server
/// - Lock Clients
/// - Kill Clients
/// </summary>
public class SimpleClientEditorWindow : EditorWindow
{
    private static GUIStyle lightGreen;
    private static GUIStyle lightRed;
    private static GUIStyle lightYellow;
    private static GUIStyle darkgrey;
    private static GUIStyle statusStyle;

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

    private static SimpleClient[] clients;
    private static SimpleClient client;
    private static int index;
    private static SimpleClientEditorWindow window;

    private static TimeStamp startTime;
    private static TimeStamp stopTime;

    private static bool done;

    // Add menu named "My Window" to the Window menu
    [MenuItem("Window/FlatEarthEditor")]
    static void Init()
    {
        Debug.Log("Init");
        clients = FindObjectsOfType<SimpleClient>();

        // Load configuration data
        AmqpConfigurationEditor.LoadConfiguration();

        //Restore the connection index
        string[] connectionNames = AmqpConfigurationEditor.GetConnectionNames();
        if (client != null)
        {
            for (int i = 0; i < connectionNames.Length; i++)
            {
                string cName = connectionNames[i];
                if (client.Connection == cName)
                {
                    index = i;
                    break;
                }
            }
        }

        lightGreen = CustomGUIUtils.GetColorBackgroundStyle(XKCDColors.LightGreen);
        lightRed = CustomGUIUtils.GetColorBackgroundStyle(XKCDColors.LightRed);
        lightYellow = CustomGUIUtils.GetColorBackgroundStyle(XKCDColors.LightYellow);
        darkgrey = CustomGUIUtils.GetColorBackgroundStyle(XKCDColors.DarkGrey);

        // Get existing open window or if none, make a new one:
        window = (SimpleClientEditorWindow)EditorWindow.GetWindow(typeof(SimpleClientEditorWindow));
        window.Show();
    }


    private static int selectedMenu = 0;
    int buttonWidth = 120;
    int buttonHeight = 30;
    public void OnGUI()
    {
        clients = FindObjectsOfType<SimpleClient>();

        if (clients == null)
        {
            Debug.Log("No Clients available..");
            return;
        }


        if (clients.Length == 0)
        {
            Debug.Log("No Clients available..");
            return;
        }

        foreach (SimpleClient item in clients)
        {
            if (item.ServerMode)
            {
                client = item;
                break;
            }
        }
        if (client == null)
            return;


        GUILayout.BeginArea(new Rect(0, 0, position.width, 39), CustomGUIUtils.GetColorBackgroundStyle(XKCDColors.Bluegrey));
        CustomGUIUtils.BeginGroup(5);
        GUILayout.BeginHorizontal();

        Color normalColor = GUI.backgroundColor;
        GUI.backgroundColor = selectedMenu == 0 ? XKCDColors.LightGrey : Color.grey;

        if (GUILayout.Button("Client Settings", GUILayout.Width(buttonWidth), GUILayout.Height(buttonHeight)) || selectedMenu == 0)
        {
            selectedMenu = 0;
        }

        GUI.backgroundColor = selectedMenu == 1 ? XKCDColors.LightGrey : Color.grey;
        if (GUILayout.Button("FlatEarth Creation", GUILayout.Width(buttonWidth), GUILayout.Height(buttonHeight)) || selectedMenu == 1)
        {
            selectedMenu = 1;
        }

        GUI.backgroundColor = selectedMenu == 2 ? XKCDColors.LightGrey : Color.grey;
        if (GUILayout.Button("Worker Status", GUILayout.Width(buttonWidth), GUILayout.Height(buttonHeight)) || selectedMenu == 2)
        {
            selectedMenu = 2;
        }

        GUI.backgroundColor = normalColor;
        if (GUILayout.Button("Reset Everything", GUILayout.Width(buttonWidth), GUILayout.Height(buttonHeight)))
        {
            client.Reset();
        }

        GUI.backgroundColor = normalColor;
        GUILayout.EndHorizontal();
        CustomGUIUtils.EndGroup();
        GUILayout.EndArea();
        GUILayout.Space(buttonHeight + 20);

        switch (selectedMenu)
        {
            case 0: // Client Settings
                ClientSettingsGUI();
                break;
            case 1: // FlatEarth Creation
                JobCreationGUI();
                break;
            case 2: // Worker Status
                WorkerStatusGUI();
                break;

            default:
                break;
        }


    }

    Vector2 scrollPos;
    private static float zoomFactor = 0.1f;
    private static int xOffset = 12;
    private static float yOffset = 7;

    private static float separator = 4;
    private static float padding = 2;
    private static float border = 8;
    private static bool autoScroll = false;
    private static float height = 16;



    private void WorkerStatusGUI()
    {
        EditorGUILayout.BeginVertical();

        zoomFactor = EditorGUILayout.Slider("Zoom", zoomFactor, 0.00001f, 0.5f);
        autoScroll = EditorGUILayout.ToggleLeft("AutoScroll", autoScroll);

        #region StatusBar
        int todoCount = 0;
        foreach (StatusUpdateMessage item in client.jobStatus.Values)
        {
            foreach (TodoItem cildItem in item.childDict.Values)
            {
                foreach (TodoItem childTodo in cildItem.childDict.Values)
                {
                    todoCount++;
                }
                todoCount++;
            }
            todoCount++;
        }

        float step;
        if (client.jobStatus.Values.Count > 0)
            step = 1f / todoCount;
        else
            step = 0;
        float complete = 0;
        foreach (StatusUpdateMessage item in client.jobStatus.Values)
        {
            foreach (TodoItem cildItem in item.childDict.Values)
            {
                foreach (TodoItem childTodo in cildItem.childDict.Values)
                {
                    if (childTodo.status == Status.DONE)
                        complete += step;
                }
                if (cildItem.status == Status.DONE)
                    complete += step;
            }
            if (item.status == Status.DONE)
                complete += step;
        }
        EditorGUILayout.LabelField("", GUILayout.Height(20));
        EditorGUI.ProgressBar(new Rect(0, 90, this.position.width, 20), complete, (complete * 100).ToString("F2", CultureInfo.CreateSpecificCulture("de-DE")) + " % ");
        #endregion // Statusbar



        if (stopTime == null)
        {
            bool allJobsDone = true;
            foreach (StatusUpdateMessage item in client.jobStatus.Values)
            {
                if (item.status != Status.DONE)
                {
                    allJobsDone = false;
                    break;
                }
            }
            if (allJobsDone && client.jobStatus.Count > 0)
            {
                stopTime = TimeStamp.Now();
                Debug.Log("All Jobs Done");
                Debug.Log("Gathering result...");

                Dictionary<string, double> result = new Dictionary<string, double>();

                foreach (StatusUpdateMessage item in client.jobStatus.Values)
                {
                    foreach (TodoItem cildItem in item.childDict.Values)
                    {
                        foreach (TodoItem childTodo in cildItem.childDict.Values)
                        {
                            if (!result.ContainsKey(childTodo.name))
                                result.Add(childTodo.name, 0);
                            result[childTodo.name] += childTodo.Duration();
                        }
                        if (!result.ContainsKey(cildItem.name))
                            result.Add(cildItem.name, 0);
                        result[cildItem.name] += cildItem.Duration();
                    }
                    if (!result.ContainsKey(item.name))
                        result.Add(item.name, 0);
                    result[item.name] += item.Duration();
                }

                StringBuilder sb = new StringBuilder();
                StatusUpdateMessage msg = client.jobStatus[1];
                foreach (TodoItem cildItem in msg.childDict.Values)
                {
                    sb.Append(cildItem.name).Append(", ").Append(result[cildItem.name]).Append("\n");
                    foreach (TodoItem childTodo in cildItem.childDict.Values)
                    {
                        sb.Append(childTodo.name).Append(", ").Append(result[childTodo.name]).Append("\n");
                    }
                    
                }

                Debug.Log(sb.ToString());
            }
        }

        EditorGUILayout.EndVertical();
        GUILayout.Space(10f);
        //Rect lastRect = GUILayoutUtility.GetLastRect();
        //Debug.Log(lastRect);
        float panelOffset = 120;


        float autoScrollPosition = this.position.width - GoldenRatio.LongSideOf(this.position.width);

        float panelWidth = this.position.width - border;
        if (startTime != null)
            panelWidth = Mathf.Max((float)TimeStamp.DurationInMillis(startTime, TimeStamp.Now()) * zoomFactor + autoScrollPosition, this.position.width - border);
        if (stopTime != null)
            panelWidth = Mathf.Min((float)TimeStamp.DurationInMillis(startTime, stopTime) * zoomFactor + autoScrollPosition, panelWidth);

        int jobCount = client.jobStatus.Values.Count;

        float panelHeight = jobCount * (4 * height + 4 * separator + 2 * padding) + yOffset;



        using (EditorGUILayout.ScrollViewScope scrollView = new EditorGUILayout.ScrollViewScope(scrollPos, false, false, GUILayout.Width(this.position.width), GUILayout.Height(this.position.height - panelOffset)))
        {
            scrollPos = scrollView.scrollPosition;

            EditorGUILayout.LabelField("", darkgrey, GUILayout.Width(panelWidth), GUILayout.Height(panelHeight));


            if (autoScroll)
                scrollPos.x = Mathf.Infinity;

            float now = xOffset;
            if (startTime != null)
                now = (float)TimeStamp.DurationInMillis(startTime, TimeStamp.Now()) * zoomFactor + xOffset;


            float y = 0f;
            y += yOffset;

            foreach (StatusUpdateMessage item in client.jobStatus.Values)
            {
                y += separator;
                TodoItem master = item.Get(Job.Master);
                TodoItem transfer = item.Get(Job.Transfer);
                TodoItem worker = item.Get(Job.Worker);

                DrawJobItemBar(item, y);
                y += padding;
                DrawTodoItemBar(master, y);
                foreach (string todo in master.childTodos)
                    DrawTodoItemBar(master.childDict[todo], y);

                y += height + separator;
                foreach (string todo in transfer.childTodos)
                    DrawTodoItemBar(transfer.childDict[todo], y);

                y += height + separator;
                DrawTodoItemBar(worker, y);
                foreach (string todo in worker.childTodos)
                    DrawTodoItemBar(worker.childDict[todo], y);

                y += height;
                y += padding;
                y += separator;
            }

            if (stopTime == null)
                CustomGUIUtils.DrawBox(new Rect(now, 0, 1, panelHeight), Color.black);

            for (float x = 0; x < panelWidth; x += (1000 * zoomFactor))
            {
                CustomGUIUtils.DrawBox(new Rect(x, 0, 1, panelHeight), Color.grey);
            }

            for (float x = 0; x < panelWidth; x += (15000 * zoomFactor))
            {
                CustomGUIUtils.DrawBox(new Rect(x, 0, 1, panelHeight), XKCDColors.LightRed);
            }

            for (float x = 0; x < panelWidth; x += (60000 * zoomFactor))
            {
                CustomGUIUtils.DrawBox(new Rect(x, 0, 1, panelHeight), Color.red);
            }
        }
        Repaint();
    }


    public Rect TodoRect(TodoItem item, float y)
    {
        Rect rect = new Rect();

        rect.x = (float)TimeStamp.DurationInMillis(item.startTime, startTime) * zoomFactor + xOffset;
        rect.width = (float)item.Duration() * zoomFactor;
        rect.height = height;
        rect.y = y;

        return rect;
    }

    public Rect JobRect(TodoItem item, float y)
    {
        Rect rect = new Rect();

        rect.x = (float)TimeStamp.DurationInMillis(item.startTime, startTime) * zoomFactor + xOffset;
        rect.width = (float)item.Duration() * zoomFactor;
        rect.height = 3 * height + 2 * separator + 2 * padding;
        rect.y = y;

        return rect;
    }

    private void DrawTodoItemBar(TodoItem item, float y)
    {
        if (item.status == Status.PENDING)
            return;

        Color color = Color.white;
        switch (item.status)
        {
            case Status.PENDING:
                color = XKCDColors.LightRed;
                break;
            case Status.IN_PROGRESS:
                color = XKCDColors.LightYellow;
                Rect rect = TodoRect(item, y);
                CustomGUIUtils.DrawFrameBox(rect, color, 1f, Color.black);
                rect.width = 200;
                GUI.Label(rect, item.name);
                break;
            case Status.DONE:
                color = XKCDColors.LightGreen;
                Rect rectBox = TodoRect(item, y);
                CustomGUIUtils.DrawFrameBox(rectBox, color, 1f, Color.black);
                //if (rectBox.Contains(Event.current.mousePosition))
                //{
                //    Rect tooltip = new Rect(rectBox);
                //    tooltip.width = 100;
                //    CustomGUIUtils.DrawFrameBox(tooltip, XKCDColors.YellowTan, 1, XKCDColors.YellowOrange);
                //    GUI.Label(tooltip, item.name);

                //}
                break;
            default:
                statusStyle = GUIStyle.none;
                break;
        }
    }

    private void DrawJobItemBar(TodoItem item, float y)
    {
        if (item.status == Status.PENDING)
            return;

        Color color = Color.white;
        switch (item.status)
        {
            case Status.PENDING:
                color = XKCDColors.LightRed;
                break;
            case Status.IN_PROGRESS:
                color = XKCDColors.LightRed;
                Rect rect = JobRect(item, y);
                CustomGUIUtils.DrawFrameBox(rect, color, 1f, Color.black);
                rect.width = 200;
                GUI.Label(rect, item.name);
                break;
            case Status.DONE:
                color = XKCDColors.LightAquamarine;
                CustomGUIUtils.DrawFrameBox(JobRect(item, y), color, 1f, Color.black);
                break;
            default:
                statusStyle = GUIStyle.none;
                break;
        }
    }

    private void DrawTodoItem(StatusUpdateMessage item)
    {
        switch (item.status)
        {
            case Status.PENDING:
                break;
            case Status.IN_PROGRESS:
                GUILayout.Button(item.name, GUILayout.Width((float)item.Duration() * zoomFactor));
                break;
            case Status.DONE:
                GUILayout.Button(item.name, GUILayout.Width((float)item.Duration() * zoomFactor));
                break;
            default:
                break;
        }
    }


    int numberOfWorkers = 10;
    private void JobCreationGUI()
    {
        EditorGUILayout.BeginVertical("box");
        TileManager.TileWidth = (double)EditorGUILayout.FloatField("TileWidth", (float)TileManager.TileWidth);
        TileManager.tileRadius = EditorGUILayout.IntField("TileRadius", TileManager.tileRadius);
        TileManager.LOD = EditorGUILayout.IntField("LOD", TileManager.LOD);
        TileManager.OriginLatitude = (double)EditorGUILayout.FloatField("OriginLatitude", (float)TileManager.OriginLatitude);
        TileManager.OriginLongitude = (double)EditorGUILayout.FloatField("OriginLongitude", (float)TileManager.OriginLongitude);
        EditorGUILayout.EndVertical();

        client.method = (SerializationMethod)EditorGUILayout.EnumPopup("SerializationMethod", client.method);


        if (GUILayout.Button("Send OSM-Job-Messages"))
        {
            startTime = TimeStamp.Now();
            stopTime = null;
            done = false;

            client.SendOSMJobMessages(
                jobQueueName,
                replyQueueName,
                statusUpdateQueueName,
                TileManager.tileRadius,
                TileManager.TileWidth,
                TileManager.OriginLongitude,
                TileManager.OriginLatitude,
                client.method);
        }

        if (GUILayout.Button("Generate Local"))
        {
            Debug.LogError("Doesn't work anymore... the enumerator has to be reactivated...");
            TileManager.CreateTileMap();
        }

        int newNumberOfWorkers = EditorGUILayout.IntSlider("Number of Workers", numberOfWorkers, 1, 100);
        if (newNumberOfWorkers != numberOfWorkers)
        {
            numberOfWorkers = newNumberOfWorkers;
            ScaleWorkers(numberOfWorkers);
        }
    }

    public static void ScaleWorkers(int numberOfWorkers)
    {
        var thread = new Thread(delegate ()
        {
            Command(numberOfWorkers);
        });
        thread.Start();
    }

    static void Command(int numberOfWorkers)
    {
        var processInfo = new ProcessStartInfo("docker-machine ssh default \"docker service scale unityTest_avsbuild =" + numberOfWorkers + "\"");
        processInfo.CreateNoWindow = true;
        processInfo.UseShellExecute = false;

        var process = Process.Start(processInfo);

        process.WaitForExit();
        process.Close();
    }

    string jobQueueName;
    string replyQueueName;
    string statusUpdateQueueName;
    private void ClientSettingsGUI()
    {
        GUILayout.BeginArea(new Rect(0, buttonHeight + 20, 300, 400));
        client.ServerMode = EditorGUILayout.Toggle("Act as Server", client.ServerMode);

        // Generate the connection dropdown options/content
        #region Dropdown connections
        string[] connectionNames = AmqpConfigurationEditor.GetConnectionNames();
        List<GUIContent> options = new List<GUIContent>();

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

        if (options.Count == 0)
            Init();

        // Set the connection name based on dropdown value
        client.Connection = options[index].text;
        #endregion // Dropdown connections

        // Draw the rest of the inspector's default layout
        //DrawDefaultInspector();
        if (GUILayout.Button("Awake"))
        {
            client.Awake();
            this.Repaint();
        }
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("EnableUpdate"))
        {
            client.EnableUpdate();
            this.Repaint();
        }

        if (GUILayout.Button("DisableUpdate"))
        {
            client.DisableUpdate();
            this.Repaint();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Toggle("IsConnecting?", client.isConnecting);
        EditorGUILayout.Toggle("IsConnected?", client.IsConnected);
        EditorGUILayout.Toggle("hasConnected?", client.hasConnected);
        EditorGUILayout.Toggle("isDisconnecting?", client.isDisconnecting);
        EditorGUILayout.Toggle("hasDisconnected?", client.hasDisconnected);
        EditorGUILayout.Toggle("isReconnecting?", client.isReconnecting);
        EditorGUILayout.Toggle("wasBlocked?", client.wasBlocked);
        EditorGUILayout.Toggle("hasAborted?", client.hasAborted);
        EditorGUILayout.Toggle("canSubscribe?", client.canSubscribe);


        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Connect"))
        {
            client.Connect();
            this.Repaint();
        }

        if (GUILayout.Button("Disconnect"))
        {
            client.Disconnect();
            this.Repaint();
        }
        EditorGUILayout.EndHorizontal();

        #region Available Queues
        //if (showAvailableQueues = EditorGUILayout.Foldout(showAvailableQueues, "Available Queues:"))
        //{
        EditorGUILayout.BeginVertical("box");
        foreach (var queue in client.GetQueues())
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Name:", queue.Name);


            if (GUILayout.Button("Subscribe", GUILayout.Width(30)))
            {
                client.SubscribeToQueue(queue.Name);
            }

            if (GUILayout.Button("X", GUILayout.Width(30)))
            {
                client.DeleteQueue(queue.Name);
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
            client.DeclareQueue(this.queueName);
        }
        EditorGUILayout.EndHorizontal();
        #endregion // Create Queue

        #region Subscribe Queue
        AmqpQueue[] queues = client.GetQueues();
        string[] names = new string[queues.Length];
        for (int i = 0; i < queues.Length; i++)
            names[i] = queues[i].Name;

        selectedQueue = EditorGUILayout.Popup("Queue", selectedQueue, names);
        if (queues.Length != 0)
        {
            if (GUILayout.Button("Subscribe to " + names[this.selectedQueue] + " Queue"))
                client.SubscribeToQueue(names[this.selectedQueue]);
        }
        #endregion // Subscribe Queue



        #region JobQueue 
        string[] jobQueueNames = new string[client.GetQueues().Length];
        for (int i = 0; i < client.GetQueues().Length; i++)
            jobQueueNames[i] = client.GetQueues()[i].Name;

        this.jobQueueIndex = EditorGUILayout.Popup("JobQueue", this.jobQueueIndex, jobQueueNames);
        for (int i = 0; i < jobQueueNames.Length; i++)
        {
            if (jobQueueNames[i] == "jobs")
            {
                this.jobQueueIndex = i;
                break;
            }
        }
        jobQueueName = client.GetQueues().Length == 0 ? "not set" : jobQueueNames[this.jobQueueIndex];
        //EditorGUILayout.LabelField("JobQueue", jobQueueName);
        #endregion // JobQueue

        #region ReplyQueue 
        string[] replyToQueueNames = new string[client.GetQueues().Length];
        for (int i = 0; i < client.GetQueues().Length; i++)
            replyToQueueNames[i] = client.GetQueues()[i].Name;

        this.replyQueueIndex = EditorGUILayout.Popup("ReplyToQueue", this.replyQueueIndex, replyToQueueNames);
        for (int i = 0; i < replyToQueueNames.Length; i++)
        {
            if (replyToQueueNames[i] == "reply")
            {
                this.replyQueueIndex = i;
                break;
            }
        }
        replyQueueName = client.GetQueues().Length == 0 ? "not set" : replyToQueueNames[this.replyQueueIndex];
        //EditorGUILayout.LabelField("ReplyToQueue", replyQueueName);
        #endregion // JobQueue

        #region JobQueue 
        string[] statusUpdateQueueNames = new string[client.GetQueues().Length];
        for (int i = 0; i < client.GetQueues().Length; i++)
            statusUpdateQueueNames[i] = client.GetQueues()[i].Name;

        this.statusUpdateQueueIndex = EditorGUILayout.Popup("ReplyToQueue", this.statusUpdateQueueIndex, statusUpdateQueueNames);
        for (int i = 0; i < statusUpdateQueueNames.Length; i++)
        {
            if (statusUpdateQueueNames[i] == "statusUpdates")
            {
                this.statusUpdateQueueIndex = i;
                break;
            }
        }
        statusUpdateQueueName = client.GetQueues().Length == 0 ? "not set" : statusUpdateQueueNames[this.statusUpdateQueueIndex];
        //EditorGUILayout.LabelField("ReplyToQueue", replyQueueName);
        #endregion // JobQueue




        //if (GUILayout.Button("Generate locally"))
        //{
        //    mainScene = EditorSceneManager.GetActiveScene();

        //    swComplete.Start();

        //    tiles = new List<Tile>();

        //    for (int i = -TileManager.tileRadius; i <= TileManager.tileRadius; i++)
        //    {
        //        for (int j = -TileManager.tileRadius; j <= TileManager.tileRadius; j++)
        //        {
        //            jobs++;
        //            Stopwatch sw = new Stopwatch();
        //            sw.Reset();
        //            sw.Start();

        //            Tile newTile = Tile.CreateTileGO(i, j, 5);
        //            tiles.Add(newTile);
        //            tileJobsLocal.Add(i + "/" + j, newTile);
        //            tileJobsStopwatchLocal.Add(i + "/" + j, sw);
        //            //newTile.ProceduralDone += GenerationDone;
        //            //newTile.StartQuery();

        //            //EditorSceneManager.SaveScenes
        //            //SceneManager.SetActiveScene(mainScene);
        //        }
        //    }

        //    tiles[0].ProceduralDone += GenerationDone;
        //    tiles[0].StartQuery();
        //}
        GUILayout.EndArea();
    }



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
            EditorGUILayout.LabelField("" + item.name);
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

}


