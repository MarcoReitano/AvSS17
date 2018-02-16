using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using CymaticLabs.Unity3D.Amqp;
using UnityEditor;
using UnityEngine;
using System.Net;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;
using UnityEngine.Networking;
using System.Collections;
using System.IO;

/// <summary>
/// 
/// - Start Server
/// - Reset Server
/// - Lock Clients
/// - Kill Clients
/// </summary>
public class SimpleClientEditorWindow : EditorWindow
{
    private static SimpleClientEditorWindow window;

    #region GUI fields
    private static GUIStyle lightGreen = new GUIStyle();
    private static GUIStyle lightRed = new GUIStyle();
    private static GUIStyle lightYellow = new GUIStyle();
    private static GUIStyle darkgrey = new GUIStyle();
    private static GUIStyle statusStyle = new GUIStyle();

    private static Font timerFont;
    public static GUIStyle timerFontStyle = new GUIStyle();

    private static OSMMapRect osmMapRect;

    private int buttonWidth = 120;
    private int buttonHeight = 30;

    private Vector2 scrollPos;
    private static float zoomFactor = 0.1f;
    private static int xOffset = 12;
    private static float yOffset = 7;

    private static float separator = 4;
    private static float padding = 2;
    private static float border = 8;
    private static bool autoScroll = false;
    private static float height = 16;

    private static int selectedMenu = 0;
    #endregion //GUI fields

    #region Client fields
    private string createQueueName;

    private int selectedQueue;

    private static SimpleClient[] clients;
    private static SimpleClient client;
    private static int selectedConfigurationIndex;

    private string jobQueueName;
    private int jobQueueIndex;

    private string replyQueueName;
    private int replyQueueIndex;

    private string statusUpdateQueueName;
    private int statusUpdateQueueIndex;

    private int numberOfWorkers = 10;
    #endregion // Client fields

    #region Job fields
    private static TimeStamp startTime;
    private static TimeStamp stopTime;

    private static bool done;

    private bool generateLocal = false;
    private bool sequential = true;

    private List<Tile> tiles;
    private int jobs;
    #endregion Job fields


    [MenuItem("Window/FlatEarthEditor")]
    static void Init()
    {
        Debug.Log("Init");
        clients = FindObjectsOfType<SimpleClient>();

        // Load configuration data
        AmqpConfigurationEditor.LoadConfiguration();

        //Restore the connection index
        RestoreConfigurationIndex();

        osmMapRect = new OSMMapRect();

        // Prepare GUI-Styles
        lightGreen = CustomGUIUtils.GetColorBackgroundStyle(XKCDColors.LightGreen);
        lightRed = CustomGUIUtils.GetColorBackgroundStyle(XKCDColors.LightRed);
        lightYellow = CustomGUIUtils.GetColorBackgroundStyle(XKCDColors.LightYellow);
        darkgrey = CustomGUIUtils.GetColorBackgroundStyle(XKCDColors.DarkGrey);

        timerFontStyle.font = (Font)Resources.Load("digitalmono");
        timerFontStyle.fontSize = 34;

        // Get existing open window or if none, make a new one:
        window = (SimpleClientEditorWindow)EditorWindow.GetWindow(typeof(SimpleClientEditorWindow));
        window.Show();
    }

    private static void RestoreConfigurationIndex()
    {
        string[] connectionNames = AmqpConfigurationEditor.GetConnectionNames();
        if (client != null)
        {
            for (int i = 0; i < connectionNames.Length; i++)
            {
                string cName = connectionNames[i];
                if (client.Connection == cName)
                {
                    selectedConfigurationIndex = i;
                    break;
                }
            }
        }
    }

    /// <summary>
    /// GUI of the FlatEarthCreator
    /// </summary>
    public void OnGUI()
    {
      
        clients = FindObjectsOfType<SimpleClient>();

        if (clients == null)
        {
            GUILayout.Label("No Clients available...", lightRed);
            Debug.Log("No Clients available...");
            return;
        }

        if (clients.Length == 0)
        {
            GUILayout.Label("No Clients available...", lightRed);
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

        
        CheckDone();

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
            client.ResetMaster();
            startTime = null;
            stopTime = null;
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

    private void WorkerStatusGUI()
    {
        EditorGUILayout.BeginVertical();

        zoomFactor = EditorGUILayout.Slider("Zoom", zoomFactor, 0.000001f, 0.25f);
        autoScroll = EditorGUILayout.ToggleLeft("AutoScroll", autoScroll);

        #region StatusBar
        // Count number of Job-Todos
        int todoCount = 0;
        foreach (StatusUpdateMessage item in SimpleClient.jobStatus.Values)
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

        // Calculate %-Step
        float step;
        if (SimpleClient.jobStatus.Values.Count > 0)
            step = 1f / todoCount;
        else
            step = 0;

        // Determine how many Job-Todos have been done
        float complete = 0;
        foreach (StatusUpdateMessage item in SimpleClient.jobStatus.Values)
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
        EditorGUI.ProgressBar(new Rect(0, 90, this.position.width - 200, 20), complete, (int)(complete * 100) + " % ");
        #endregion // Statusbar

        #region Timer
        string timer;
        if (stopTime == null)
            timer = startTime != null ? startTime.Duration() : "00:00:00.000";
        else
            timer = TimeStamp.Duration(TimeStamp.Duration(startTime, stopTime));

        if (timerFontStyle == null)
        {
            timerFontStyle.font = (Font)Resources.Load("digitalmono");
            timerFontStyle.fontSize = 34;
        }
        EditorGUI.LabelField(new Rect(this.position.width - 190, 85, 180, 20), timer, timerFontStyle);
        #endregion // Timer

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

        int jobCount = SimpleClient.jobStatus.Values.Count;

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

            if (generateLocal)
            {
                float y = 0f;
                y += yOffset;
                foreach (StatusUpdateMessage item in SimpleClient.jobStatus.Values)
                {
                    y += separator;
                    TodoItem worker = item.Get(Job.Worker);

                    DrawJobItemBar(item, y);
                    y += padding;

                    y += height + separator;
                    DrawTodoItemBar(worker, y);
                    foreach (string todo in worker.childTodos)
                        DrawTodoItemBar(worker.childDict[todo], y);

                    y += height;
                    y += padding;
                    y += separator;
                }
            }
            else
            {
                float y = 0f;
                y += yOffset;
                foreach (StatusUpdateMessage item in SimpleClient.jobStatus.Values)
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
            }
            if (stopTime == null)
                CustomGUIUtils.DrawBox(new Rect(now, 0, 1, panelHeight), Color.black);

            #region Draw Time indicators
            if (1000 * zoomFactor > 20)
            {
                for (float x = 0; x < panelWidth; x += (1000 * zoomFactor))
                    CustomGUIUtils.DrawBox(new Rect(x, 0, 1, panelHeight), Color.grey);
            }

            if (15000 * zoomFactor > 20)
            {
                for (float x = 0; x < panelWidth; x += (15000 * zoomFactor))
                    CustomGUIUtils.DrawBox(new Rect(x, 0, 1, panelHeight), XKCDColors.LightRed);
            }

            for (float x = 0; x < panelWidth; x += (60000 * zoomFactor))
                CustomGUIUtils.DrawBox(new Rect(x, 0, 1, panelHeight), Color.red);
            #endregion // Draw time indicators
        }
        Repaint();
    }

    private static void CheckDone()
    {
        if (stopTime == null)
        {
            bool allJobsDone = true;
            foreach (StatusUpdateMessage item in SimpleClient.jobStatus.Values)
            {
                if (item.status != Status.DONE)
                {
                    allJobsDone = false;
                    break;
                }
            }

            #region Collect Statistics
            if (allJobsDone && SimpleClient.jobStatus.Count > 0)
            {
                stopTime = TimeStamp.Now();
                Debug.Log("All Jobs Done");
                Debug.Log("Gathering result...");

                Dictionary<string, double> result = new Dictionary<string, double>();

                foreach (StatusUpdateMessage item in SimpleClient.jobStatus.Values)
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
                sb.Append("StartTime,").Append(startTime).AppendLine();
                sb.Append("StopTime,").Append(stopTime).AppendLine();

                sb.Append("OriginLon,").Append(TileManager.OriginLongitude).AppendLine();
                sb.Append("OriginLat,").Append(TileManager.OriginLatitude).AppendLine();

                sb.Append("TileRadius, ").Append(TileManager.tileRadius).AppendLine();
                sb.Append("TileWidth,").Append(TileManager.TileWidth).AppendLine();

                StatusUpdateMessage msg = SimpleClient.jobStatus[0];
                foreach (TodoItem cildItem in msg.childDict.Values)
                {
                    sb.Append(cildItem.name).Append(", ").Append(result[cildItem.name]).Append("\n");
                    foreach (TodoItem childTodo in cildItem.childDict.Values)
                    {
                        sb.Append(childTodo.name).Append(", ").Append(result[childTodo.name]).Append("\n");
                    }
                }

                Debug.Log(sb.ToString());
                string logPath = Application.dataPath + "/Logs/";
                if (!Directory.Exists(logPath))
                {
                    Directory.CreateDirectory(logPath);
                }
                System.IO.File.WriteAllText(logPath + "Session.txt", sb.ToString());

            }
            #endregion // Collect Statistics
        }
    }

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
            generateLocal = false;

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

        sequential = EditorGUILayout.ToggleLeft("Sequential", sequential);
        if (GUILayout.Button("Generate Local"))
        {
            startTime = TimeStamp.Now();
            stopTime = null;
            done = false;
            generateLocal = true;

            if (sequential)
            {
                tiles = new List<Tile>();
                jobs = 0;
                for (int i = -TileManager.tileRadius; i <= TileManager.tileRadius; i++)
                {
                    for (int j = -TileManager.tileRadius; j <= TileManager.tileRadius; j++)
                    {
                        StatusUpdateMessage msg = new StatusUpdateMessage(jobs, i + "," + j);

                        TodoItem worker = msg.AddTodo(Job.Worker);
                        //worker.AddTodo(Job.CreateTile);
                        worker.AddTodo(Job.StartOSMQuery);
                        worker.AddTodo(Job.StartProcedural);
                        worker.AddTodo(Job.ProceduralPreparation);
                        worker.AddTodo(Job.CreateTerrain);
                        worker.AddTodo(Job.MeshPreparation);
                        worker.AddTodo(Job.TileQuad);
                        worker.AddTodo(Job.River);
                        worker.AddTodo(Job.Ways);
                        worker.AddTodo(Job.CreateBuildingMesh);
                        worker.AddTodo(Job.FillMeshDivideMaterials);
                        worker.AddTodo(Job.GarbageCollection);
                        worker.AddTodo(Job.ProceduralDone);
                        SimpleClient.jobStatus.Add(jobs, msg);

                        Tile newTile = Tile.CreateTileGO(i, j, 5);
                        tiles.Add(newTile);
                        newTile.SetJobInfo(jobs, msg);

                        jobs++;
                    }
                }

                // Start first Tile-Generation
                // Next Tile will be started after "GenerationDone" of first Tile
                tiles[0].ProceduralDoneLocal += GenerationDone;
                tiles[0].StartQuery();
            }
            else
            {
                // Start parallel Generation of Tiles locally
                TileManager.GenerateLocal();
            }
        }

        #region ScaleWorkers
        int newNumberOfWorkers = EditorGUILayout.IntSlider("Number of Workers", numberOfWorkers, 1, 100);
        if (newNumberOfWorkers != numberOfWorkers)
        {
            if (newNumberOfWorkers > numberOfWorkers)
                Debug.Log("Scaling UP number of workers (from " + numberOfWorkers + " to " + newNumberOfWorkers + ")");
            else
                Debug.Log("Scaling DOWN number of workers (from " + numberOfWorkers + " to " + newNumberOfWorkers + ")");
            numberOfWorkers = newNumberOfWorkers;
            ScaleWorkers(numberOfWorkers);
        }
        #endregion // ScaleWorkers

        #region DrawOSM-Map
        if (osmMapRect == null)
            osmMapRect = new OSMMapRect();
        osmMapRect.DrawOSMMapRect(new Rect(0, 300, this.position.width, this.position.height - 300));
        if (osmMapRect.ShouldRepaint())
            Repaint();
        #endregion // DrawOSM-Map
    }

    private void GenerationDone(object sender, EventArgs e)
    {
        jobs--;

        Tile tile = (Tile)sender;
        string tileName = tile.TileIndex[0] + "/" + tile.TileIndex[1];
        Debug.Log("Done Generating " + tileName);
        tile.ProceduralDoneLocal -= GenerationDone;

        if (tiles.Count > 0)
        {
            Tile nextTile = tiles[0];
            tiles.RemoveAt(0);
            nextTile.ProceduralDoneLocal += GenerationDone;
            nextTile.StartQuery();
        }
        else
        {
            done = true;
        }
    }


    //IEnumerator WaitForWWW(WWW www)
    //{
    //    yield return www;


    //    string txt = "";
    //    if (string.IsNullOrEmpty(www.error))
    //        txt = www.text;  //text of success
    //    else
    //        txt = www.error;  //error
    //    GameObject.Find("Txtdemo").GetComponent<Text>().text = "++++++\n\n" + txt;
    //}
    //void TaskOnClick()
    //{
    //    try
    //    {

    //        string ourPostData = "";
    //        //"{\"plan\":\"TESTA02\"";
    //        Dictionary<string, string> headers = new Dictionary<string, string>();
    //        headers.Add("Content-Type", "application/json");
    //        //byte[] b = System.Text.Encoding.UTF8.GetBytes();
    //        byte[] pData = System.Text.Encoding.ASCII.GetBytes(ourPostData.ToCharArray());
    //        ///POST by IIS hosting...
    //        WWW api = new WWW("http://192.168.1.120/si_aoi/api/total", pData, headers);
    //        ///GET by IIS hosting...
    //        ///WWW api = new WWW("http://192.168.1.120/si_aoi/api/total?dynamix={\"plan\":\"TESTA02\"");
    //        StartCoroutine(WaitForWWW(api));
    //    }
    //    catch (UnityException ex) { Debug.Log(ex.Message); }
    //}


    RabbitmqManagementAPI managementAPI;
    RabbitmqWebRequest overview;
    private void ClientSettingsGUI()
    {
        
        GUILayout.BeginArea(new Rect(0, buttonHeight + 20, 300, 400));
        client.ServerMode = EditorGUILayout.Toggle("Act as Server", client.ServerMode);

        #region Dropdown connections
        // Generate the connection dropdown options/content
        List<GUIContent> options = new List<GUIContent>();
        string[] connectionNames = AmqpConfigurationEditor.GetConnectionNames();

        //if (Event.current.type == EventType.Layout)
        //{
        for (var i = 0; i < connectionNames.Length; i++)
        {
            var cName = connectionNames[i];
            if (string.IsNullOrEmpty(client.Connection) || client.Connection == cName)
                selectedConfigurationIndex = i;
            options.Add(new GUIContent(cName));
        }

        if (options.Count == 0)
        {
            Init();
        }
        //}

        // Connections drop down
        string tooltip = "Select the AMQP connection to use. Connections can be configured in the AMQP/Configuration menu.";
        selectedConfigurationIndex = EditorGUILayout.Popup(new GUIContent("Connection", tooltip), selectedConfigurationIndex, options.ToArray());

        // Set the connection name based on dropdown value
        try
        {
            GUIContent con = options[selectedConfigurationIndex];
            client.Connection = options[selectedConfigurationIndex].text;
        }
        catch (ArgumentException)
        {
            Repaint();
            return;
        }
        #endregion // Dropdown connections
        string connectionString = options[selectedConfigurationIndex].text;

        #region REST-Versuche
        AmqpConnection connection = SimpleClient.GetConnection(connectionString);
        //if (connection != null)
        //{
        //    //string consumers = "http://guest:guest@" + connection.Host + ":" + connection.AmqpPort + "/api/consumers";
        //    //Debug.Log("http://guest:guest@" + connection.Host + ":" + connection.AmqpPort + "/api/consumers");

        //    if (managementAPI == null && client.IsConnected)
        //    {
        //        managementAPI = new RabbitmqManagementAPI(connection);
        //    }

        //    if (overview == null)
        //    {
        //        if (client.IsConnected)
        //        {
        //            overview = managementAPI.Overview();
        //            //overview.Start();
        //        }

        //    }
        //    else
        //    {
        //        Debug.Log("OverviewResult: " + overview.GetResult());
        //        //Debug.Log("overViewRequest != null");
        //        if (client.IsConnected)
        //        {
        //            if (overview.RequestFinished)
        //            {
        //                if (!overview.RequestErrorOccurred)
        //                {
        //                    Debug.Log("not finished: " + overview.ResponseCode.ToString());
        //                }
        //                else
        //                {
        //                    Debug.Log("OverviewResult: " + overview.GetResult());
        //                }
        //            }
        //            else
        //            {
        //                //Debug.Log("not finished: " + overview.ResponseCode.ToString());
        //            }
        //        }
        //    }
        //}


        //if (connection != null)
        //{
        //    //http://10.211.55.2:15672/api/queues
        //    string ourPostData = "";
        //    Dictionary<string, string> headers = new Dictionary<string, string>();
        //    headers.Add("Content-Type", "application/json");

        //    byte[] pData = new byte[0];
        //    string consumers = "http://" + connection.Host + ":" + connection.AmqpPort + "/api/queues";
        //    WWW api = new WWW(consumers, pData, headers);

        //    while (!api.isDone)
        //    {
        //        if (api.error != null)
        //        {
        //            Debug.Log("Error");
        //            break;
        //        }
        //    }

        //    string jsonStr = Encoding.UTF8.GetString(api.bytes);
        //    Debug.Log(consumers + " : " + api.text);
        //}



        //WWW api = new WWW(consumers);

        //while (!api.isDone)
        //{
        //    // wait
        //}
        //Debug.Log("result: " + api.text);

        //UnityWebRequest
        //UnityWebRequest request = UnityWebRequest.Get(consumers);
        //while (!request.isDone || request.isError)
        //{
        //    // wait
        //    if (request.isError)
        //    {
        //        Debug.Log("Error");
        //    }
        //}
        //Debug.Log("result UnityWebRequest: " + request.downloadHandler.text);
        #endregion // REST-Versuche


        if (GUILayout.Button("Awake"))
        {
            client.Awake();
            Repaint();
            return;
        }

        EditorGUILayout.BeginHorizontal();
        {
            if (GUILayout.Button("EnableUpdate"))
            {
                client.EnableUpdate();
                return;
            }

            if (GUILayout.Button("DisableUpdate"))
            {
                client.DisableUpdate();
                return;
            }
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
        {
            if (GUILayout.Button("Connect"))
            {
                client.Connect();
                return;
            }

            if (GUILayout.Button("Disconnect"))
            {
                client.Disconnect();
                return;
            }
        }
        EditorGUILayout.EndHorizontal();

        #region Available Queues
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
        #endregion Available Queues

        #region Create Queue
        EditorGUILayout.BeginHorizontal("box");
        this.createQueueName = EditorGUILayout.TextField("Queue Name", this.createQueueName);
        if (GUILayout.Button("Create"))
        {
            client.DeclareQueue(this.createQueueName);
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

        #region StatusUpdateQueue 
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
        #endregion // StatusUpdateQueue

        GUILayout.EndArea();
    }


    #region Scale Workers
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
    #endregion // Scale Workers

    #region Draw Methods for JobStatusMessages
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
    #endregion // Draw Methods for JobStatusMessages
}


