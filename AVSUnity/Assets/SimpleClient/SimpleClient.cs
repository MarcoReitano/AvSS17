using System;
using System.Collections;
using System.Collections.Generic;

using CymaticLabs.Unity3D.Amqp;
using CymaticLabs.Unity3D.Amqp.SimpleJSON;
using CymaticLabs.Unity3D.Amqp.UI;


using UnityEditor.SceneManagement;
using UnityEditor;

using UnityEngine;
using UnityEngine.SceneManagement;

//[ExecuteInEditMode]
public class SimpleClient : MonoBehaviour
{
#if UNITY_EDITOR
    [MenuItem("DistributedCityGeneration/Create Server")]
    public static void CreateServer()
    {
        GameObject serverGO = new GameObject("Server");
        SimpleClient server = serverGO.AddComponent<SimpleClient>();
        server.ServerMode = true;
    }

    [MenuItem("DistributedCityGeneration/Create Client")]
    public static void CreateClient()
    {
        GameObject clientGO = new GameObject("Client");
        clientGO.AddComponent<SimpleClient>();
    }
#endif 
    #region Inspector
    /// <summary>
    /// The name of the connection to use.
    /// </summary>
    [HideInInspector]
    public string Connection;

    /// <summary>
    /// Whether or not to connect to the messaging broker on start.
    /// </summary>
    [Tooltip("Whether or not to connect to the messaging broker on start.")]
    public bool ConnectOnStart = true;

    /// <summary>
    /// Whether or not to use relaxed SSL certificate validation for the broker.
    /// </summary>
    /// <remarks>
    /// This can be useful since Unity's version of Mono (as of 5.x) has a very strict/buggy implementation
    /// around SSL/TLS certificate validation. This can make the connection inherently less secure as it will
    /// trust unverified certificates, but encrypted communications will work. This setting is applied on start 
    /// and updating it during runtime will have no effect.
    /// </remarks>
    [Tooltip("Whether or not to use relaxed SSL certificate validation for the broker. Server SSL certificates will not be verified but encryption will be used.")]
    public bool RelaxedSslValidation = false;

    [Tooltip("Whether or not to act as Server (Changes our handling of Messages).")]
    public bool ServerMode = false;

    /// <summary>
    /// A list of queue subscriptions to subscribe to when connected.
    /// </summary>
    [Tooltip("A list of queue subscriptions to subscribe to when connected.")]
    public UnityAmqpQueueSubscription[] QueueSubscriptions;
    #endregion // Inspector

    #region Fields
    // The text asset that contains the AMQP configuration data
    TextAsset configJson;

    // The internal message broker client
    IAmqpBrokerConnection client;

    // Flag used to tell when the client should restore subscriptions
    public bool canSubscribe = false;

    // Flag used to tell when the connection to the host was blocked
    public bool wasBlocked = false;

    // Flag used to tell when the client has connected to the host
    public bool hasConnected = false;

    public bool isConnecting = false;

    // Whether or not the client has begun graceful disconnection
    public bool isDisconnecting = false;

    // Flag to tell whether when the client has disconnected from the host
    public bool hasDisconnected = false;

    // Flag to tell whether when the client has aborted its connection the host
    public bool hasAborted = false;

    // Flag to tell when the client is attempting to reconnect to the host
    public bool isReconnecting = false;

    // List of available connections
    static List<AmqpConnection> connections = new List<AmqpConnection>();


    // List of queue-based subscriptions
    List<AmqpQueueSubscription> queueSubscriptions;

    // A queue of incoming queue-based messages
    Queue<AmqpQueueReceivedMessage> queueMessages;

    // A queue of incoming unhandled exceptions
    Queue<Exception> connectionExceptions;

    // A queue of incoming unhandled queue subscribe exceptions
    Queue<Exception> queueSubscribeExceptions;

    // A queue of incoming unhandled queue unsubscribe exceptions
    Queue<Exception> queueUnsubscribeExceptions;

    // A queue of exchange subscribes
    Queue<AmqpQueueSubscription> subscribedQueues;

    // A queue of exchange unsubscribes
    Queue<AmqpQueueSubscription> unsubscribedQueues;

    // A queue of async results when listing AMQP queues
    Queue<AsyncQueueListResult> queueListResults;

    // Whether or not the application is currently quitting
    public bool isQuitting = false;

    /// The message broker host.
    string host;

    /// The message broker AMQP port.
    int amqpPort = 5672;

    /// The message broker web/REST port.
    int webPort = 80;

    /// The message broker's virtual host to use.
    string virtualHost;

    /// The username to use.
    string username;

    /// The password to use.
    string password;

    /// The interval in seconds for reconnection attempts.
    short reconnectInterval = 5;

    /// The requested keep-alive heartbeat in seconds.
    ushort requestedHeartBeat = 30;
    #endregion // Fields

    #region Properties
    /// <summary>
    /// Gets the singleton instance of the class.
    /// </summary>
    /// <remarks>
    /// If you used more than one instance of <see cref="AmqpClient"/> then you should ignore
    /// this value is it will always resolve to the last instance to have initialized. This is
    /// convenient for global access to the class when only one instance is being used at a time.
    /// </remarks>
    // public static AmqpClient Instance
    // {
    // get; private set;
    // }
    /// <summary>
    /// Gets whether or not the amqp messaging system is currently connected or not.
    /// </summary>
    public bool IsConnected
    {
        get
        {
            return this.client != null ? this.client.IsConnected : false;
        }
    }

    /// <summary>
    /// Gets or sets the maximum number of times the client will attempt to reconnect to the host before aborting.
    /// </summary>
    public uint ReconnectRetryLimit
    {
        get
        {
            return this.client != null ? this.client.ReconnectRetryLimit : uint.MaxValue;
        }

        set
        {
            this.client.ReconnectRetryLimit = value;
        }
    }

    /// <summary>
    /// Gets or sets the maximum number of failed subscriptions the client will tolerate before preventing connection to the host.
    /// </summary>
    /// <remarks>
    /// Presently in RabbitMQ if there is an error during subscription the host will close the connection
    /// so the client must reconnect. In cases where the client attempts to resubscribe the same failing subscription
    /// this can lead to an endless loop of connect->subscribe->error->reconnect->infinity. Putting a limit
    /// prevents the loop from going on infinitely.
    /// </remarks>
    public byte SubscribeRetryLimit
    {
        get
        {
            return this.client != null ? this.client.SubscribeRetryLimit : (byte)10;
        }

        set
        {
            this.client.SubscribeRetryLimit = value;
        }
    }

    /// <summary>
    /// The underlying broker connection used by the client.
    /// </summary>
    public IAmqpBrokerConnection BrokerConnection
    {
        get
        {
            return this.client;
        }
    }

    /// <summary>
    /// Gets the file name for the AMQP connections data.
    /// </summary>
    public static string ConfigurationFilename
    {
        get
        {
            return "AmqpConfiguration.json";
        }
    }
    #endregion //Properties

    #region Events
    /// <summary>
    /// Occurs when the client has connected to the AMQP message broker.
    /// </summary>
    public SimpleClientUnityEvent OnConnected;

    /// <summary>
    /// Occurs when the client has begun disconnecting from the AMQP message broker, but before it has finished.
    /// </summary>
    public SimpleClientUnityEvent OnDisconnecting;
    IEnumerator disconnectingEnumerator;



    /// <summary>
    /// Occurs when the client has disconnected from the AMQP message broker.
    /// </summary>
    public SimpleClientUnityEvent OnDisconnected;

    /// <summary>
    /// Occurs when the client has been blocked by the AMQP message broker.
    /// </summary>
    public SimpleClientUnityEvent OnBlocked;

    /// <summary>
    /// Occurs when the client has started reconnecting to the AMQP message broker.
    /// </summary>
    public SimpleClientUnityEvent OnReconnecting;

    /// <summary>
    /// Occurs when the client connection has aborted and must be manually reset.
    /// </summary>
    public SimpleClientUnityEvent OnConnectionAborted;

    /// <summary>
    /// Occurs when there is a connection error.
    /// </summary>
    public ExceptionUnityEvent OnConnectionError;

    /// <summary>
    /// Occurs when the client subscribes to a queue.
    /// </summary>
    public AmqpQueueSubscriptionUnityEvent OnSubscribedToQueue;

    /// <summary>
    /// Occurs when the client unsubscribes from a queue.
    /// </summary>
    public AmqpQueueSubscriptionUnityEvent OnUnsubscribedFromQueue;

    /// <summary>
    /// Occurs when there is an error unsubscribing from an exchange.
    /// </summary>
    public ExceptionUnityEvent OnQueueSubscribeError;

    /// <summary>
    /// Occurs when there is an error unsubscribing from a queue.
    /// </summary>
    public ExceptionUnityEvent OnQueueUnsubscribeError;
    #endregion // Events

    #region Methods
    #region Init
    private void Reset()
    {
        Debug.Log("<color=blue><b>" + this.name + ": SimpleClient.Reset()</b></color>");
    }

    public void Awake()
    {
        Debug.Log("<color=blue><b>" + this.name + ": SimpleClient.Awake()</b></color>");
        // Initialize fields
        this.queueSubscriptions = new List<AmqpQueueSubscription>();
        this.queueMessages = new Queue<AmqpQueueReceivedMessage>();
        this.connectionExceptions = new Queue<Exception>();
        this.queueSubscribeExceptions = new Queue<Exception>();
        this.queueUnsubscribeExceptions = new Queue<Exception>();
        this.subscribedQueues = new Queue<AmqpQueueSubscription>();
        this.unsubscribedQueues = new Queue<AmqpQueueSubscription>();
        connections = new List<AmqpConnection>();

        this.queueListResults = new Queue<AsyncQueueListResult>();


        // If queue subscriptions were provided through the inspector, add them in
        if (this.QueueSubscriptions != null && this.QueueSubscriptions.Length > 0)
        {
            this.queueSubscriptions.AddRange(this.QueueSubscriptions);
        }

        // Apply SSL settings
        SslHelper.RelaxedValidation = this.RelaxedSslValidation;

        // Look for connections file
        this.configJson = Resources.Load<TextAsset>(ConfigurationFilename.Replace(".json", string.Empty));

        if (this.configJson == null)
        {
            Debug.LogErrorFormat("<color=red>AMQP JSON configuration asset not found: {0}</color>", ConfigurationFilename);
            return;
        }

        // Parse connection JSON data
        try
        {
            var config = JSON.Parse(this.configJson.text).AsObject;
            var jsonConnections = config["Connections"].AsArray;

            // Populate the connection list from the data
            for (int i = 0; i < jsonConnections.Count; i++)
            {
                var c = AmqpConnection.FromJsonObject(jsonConnections[i].AsObject);
                connections.Add(c);
            }
        }
        catch (Exception ex)
        {
            Debug.LogErrorFormat("{0}", ex);
        }

        //EditorApplication.update += this.Update;
    }
#if UNITY_EDITOR
    public void EnableUpdate()
    {
        EditorApplication.update -= this.Update;
        EditorApplication.update += this.Update;
        Debug.Log("<color=green><b>" + this.name + ": Started listening to EditorApplication.update</b></color>");
    }

    public void DisableUpdate()
    {
        EditorApplication.update -= this.Update;
        Debug.Log("<color=red><b>" + this.name + ": Stopped listening to EditorApplication.update</b></color>");
    }
#endif

    private void Start()
    {
        Debug.Log("<color=blue><b>" + this.name + ": SimpleClient.Start()</b></color>");
        // Connect to host broker on start if configured
        if (this.ConnectOnStart)
            this.Connect();


        newScene = EditorSceneManager.NewScene(
               NewSceneSetup.EmptyScene,
               NewSceneMode.Additive);
        
        string filename = RelativeAssetPathTo("newScene.unity");
        Debug.Log("before newScene-Path: " + newScene.path);

        EditorSceneManager.SaveScene(newScene, filename);
    }
    #endregion // Init

    #region Update
    // Handle Unity update loop
    private void Update()
    {
        //Debug.Log("<color=blue><b>" + this.name + ": SimpleClient.Update()</b></color>");
        /** These flags are set by the thread that the AMQP client runs on and then handled in Unity's game thread **/

        // The client has connected
        if (this.hasConnected)
        {
            this.hasConnected = false; // reset the flag for the next event
            Log("<color=green>Connected to AMQP host {0}</color>", AmqpHelper.GetConnectionInfo(this.client));
            if (this.OnConnected != null)
                this.OnConnected.Invoke(this);
        }

        // The client has disconnected
        if (this.hasDisconnected)
        {
            this.hasDisconnected = false; // reset the flag for the next event
            Log("<color=green>Disconnected from AMQP host {0}</color>", AmqpHelper.GetConnectionInfo(this.client));
            if (this.OnDisconnected != null)
                this.OnDisconnected.Invoke(this);
        }

        // Handle client graceful disconnect
        if (this.isDisconnecting)
        {
            this.isDisconnecting = false;
            this.disconnectingEnumerator = this.DelayDisconnection(2);
            this.StartCoroutine(this.disconnectingEnumerator);
        }
        if (this.disconnectingEnumerator != null)
        {
            this.disconnectingEnumerator.MoveNext();
        }

        // The client has disconnected
        if (this.isReconnecting)
        {
            this.isReconnecting = false; // reset the flag for the next event
            Log("<color=green>Reconnecting to AMQP host: {0}</color>", AmqpHelper.GetConnectionInfo(this.client));
            if (this.OnReconnecting != null)
                this.OnReconnecting.Invoke(this);
        }

        // The client has been blocked
        if (this.wasBlocked)
        {
            this.wasBlocked = false; // reset the flag for the next event
            Log("<color=red>Connection to AMQP host blocked: {0}</color>", AmqpHelper.GetConnectionInfo(this.client));
            if (this.OnBlocked != null)
                this.OnBlocked.Invoke(this);
        }

        // The client connection has aborted
        if (this.hasAborted)
        {
            this.hasAborted = false; // reset the flag for the next event
            Log("<color=red>Connection to AMQP host aborted: {0}</color>", AmqpHelper.GetConnectionInfo(this.client));
            if (this.OnConnectionAborted != null)
                this.OnConnectionAborted.Invoke(this);
        }

        // It's safe to subscribe so restore subscriptions
        if (this.canSubscribe && this.IsConnected)
        {
            this.canSubscribe = false; // reset the flag for the next event
            this.RestoreSubscriptions();
        }

        if (this.isQuitting)
            return;

        #region Process Exceptions
        // Connection Exceptions
        if (this.connectionExceptions.Count > 0)
        {
            var errors = new Exception[this.connectionExceptions.Count];

            lock (this)
            {
                // Copy the list and clear queue
                this.connectionExceptions.CopyTo(errors, 0);
                this.connectionExceptions.Clear();
            }

            foreach (var ex in errors)
            {
                // Notify
                if (this.OnConnectionError != null)
                    this.OnConnectionError.Invoke(ex);

                // Log
                Log("<color=red>{0}</color>", ex);
            }
        }


        // Queue Unsubscribe Exceptions
        if (this.queueUnsubscribeExceptions.Count > 0)
        {
            var errors = new Exception[this.queueUnsubscribeExceptions.Count];

            lock (this)
            {
                // Copy the list and clear queue
                this.queueUnsubscribeExceptions.CopyTo(errors, 0);
                this.queueUnsubscribeExceptions.Clear();
            }

            foreach (var ex in errors)
            {
                // Notify
                if (this.OnQueueUnsubscribeError != null)
                    this.OnQueueUnsubscribeError.Invoke(ex);

                // Log
                if (ex != null)
                    Log("<color=red>{0}</color>", ex);
            }
        }
        #endregion // Process Exceptions

        #region Process Subscribes
        // Queues
        if (this.subscribedQueues.Count > 0)
        {
            var subscriptions = new AmqpQueueSubscription[this.subscribedQueues.Count];

            lock (this)
            {
                // Copy the list and clear queue
                this.subscribedQueues.CopyTo(subscriptions, 0);
                this.subscribedQueues.Clear();
            }

            foreach (var sub in subscriptions)
            {
                // Notify
                if (this.OnSubscribedToQueue != null)
                    this.OnSubscribedToQueue.Invoke(sub);

                // Log
                Log("<color=green>Subscribed to queue: {0}:{1}</color>", sub.QueueName, sub.UseAck);
            }
        }
        #endregion // Process Subscribes

        #region Process Unsubscribes
        // Queues
        if (this.unsubscribedQueues.Count > 0)
        {
            var subscriptions = new AmqpQueueSubscription[this.unsubscribedQueues.Count];

            lock (this)
            {
                // Copy the list and clear queue
                this.unsubscribedQueues.CopyTo(subscriptions, 0);
                this.unsubscribedQueues.Clear();
            }

            foreach (var sub in subscriptions)
            {
                // Notify
                if (this.OnUnsubscribedFromQueue != null)
                    this.OnUnsubscribedFromQueue.Invoke(sub);

                // Log
                Log("<color=green>Unsubscribed from queue: {0}:{1}</color>", sub.QueueName, sub.UseAck);
            }
        }
        #endregion // Process Unsubscribes

        #region Process Incoming Messages
        // Process queue messages
        if (this.queueMessages.Count > 0)
        {
            var received = new AmqpQueueReceivedMessage[this.queueMessages.Count];

            lock (this)
            {
                // Copy messages to temp list and clear queue
                this.queueMessages.CopyTo(received, 0);
                this.queueMessages.Clear();
            }

            // Process messages
            foreach (var rx in received)
            {
                // Call the non-threadsafe handler, this should be the actual Unity message handler
                Debug.Log("Received: " + received.GetLength(0));
                rx.Subscription.Handler(rx);
            }
        }
        #endregion // Process Incoming Messages

        #region Process Async Queue Listings
        if (this.queueListResults.Count > 0)
        {
            var results = new AsyncQueueListResult[this.queueListResults.Count];

            lock (this)
            {
                // Copy the list and clear queue
                this.queueListResults.CopyTo(results, 0);
                this.queueListResults.Clear();
            }

            // Invoke the callbacks and pass the results
            foreach (var result in results)
                result.Callback(result.QueueList);
        }
        #endregion // Process Async Queue Listings
    }
#endregion // Update

    #region Clean Up
    // Handles clean-up of AMQP resources when quitting the application
    private void OnApplicationQuit()
    {
        if (this.isQuitting)
            return;
        this.isQuitting = true;
        if (this.client != null && this.client.IsConnected)
            this.client.Disconnect(); // if not properly disconnected, Unity will hang on quit
    }
    #endregion // Clean Up

    #region Connection
    /// <summary>
    /// Gets the current list of connections.
    /// </summary>
    /// <returns>An array of the current AMQP connections.</returns>
    public static AmqpConnection[] GetConnections()
    {
        return connections.ToArray();
    }

    /// <summary>
    /// Gets a connection by name if it exists.
    /// </summary>
    /// <param name="name">The name of the connection to get.</param>
    /// <returns>The connection if it is found, otherwise NULL.</returns>
    public static AmqpConnection GetConnection(string name)
    {
        if (string.IsNullOrEmpty(name))
            throw new ArgumentNullException("name");

        foreach (var connection in connections)
        {
            if (connection.Name == name)
                return connection;
        }

        return null;
    }


    /// <summary>
    /// Connects to the messaging broker.
    /// </summary>
    public void Connect()
    {
        if (!this.isConnecting)
        {
            this.isConnecting = true;
        }

        if (this.client != null && this.client.IsConnected)
        {
            Log("<color=red>Client is already connected and cannot reconnect</color>");
            return;
        }

        // Find the connection by name
        var connection = GetConnection(this.Connection);

        if (connection != null)
        {
            this.host = connection.Host;
            this.amqpPort = connection.AmqpPort;
            this.webPort = connection.WebPort;
            this.virtualHost = connection.VirtualHost;
            this.username = connection.Username;
            this.password = connection.Password;
            this.reconnectInterval = connection.ReconnectInterval;
            this.requestedHeartBeat = connection.RequestedHeartBeat;
        }

        // Create the client for the connection
        if (this.client == null)
        {
            this.client = AmqpConnectionFactory.Create(
                this.host,
                this.amqpPort,
                this.webPort,
                this.virtualHost,
                this.username,
                this.password,
                this.reconnectInterval,
                this.requestedHeartBeat);

            Debug.Log("<color=green><b>Initializing EventHandler</b></color>");
            this.client.Blocked += this.Client_Blocked;
            this.client.Connected += this.Client_Connected;
            this.client.Disconnected += this.Client_Disconnected;
            this.client.Disconnecting += this.Client_Disconnecting;
            this.client.Reconnecting += this.Client_Reconnecting;
            this.client.ConnectionError += this.Client_ConnectionError;
            this.client.ConnectionAborted += this.Client_ConnectionAborted;
            this.client.SubscribedToQueue += this.Client_SubscribedToQueue;
            this.client.UnsubscribedFromQueue += this.Client_UnsubscribedFromQueue;
            this.client.QueueSubscribeError += this.Client_QueueSubscribeError;
            this.client.QueueUnsubscribeError += this.Client_QueueUnsubscribeError;
        }

        // Or reuse it if possibly
        else
        {
            if (connection != null)
            {
                this.client.Server = connection.Host;
                this.client.AmqpPort = connection.AmqpPort;
                this.client.WebPort = connection.WebPort;
                this.client.VirtualHost = connection.VirtualHost;
                this.client.Username = this.username;
                this.client.Password = this.password;
                this.client.ReconnectInterval = connection.ReconnectInterval;
                this.client.RequestedHeartbeat = connection.RequestedHeartBeat;
            }
            else
            {
                Debug.Log("<color=red><b>Connection is NULL</b></color>");
            }
        }

        // Connect the client
        Log("<color=green>Connecting to AMQP host: {0}</color>", AmqpHelper.GetConnectionInfo(this.client));
        Debug.Log("Really trying to connect?");
        this.client.Connect();
        Debug.Log("After connect()");
    }

    /// <summary>
    /// Disconnects from the messaging broker.
    /// </summary>
    public void Disconnect()
    {
        if (this.client == null || !this.client.IsConnected)
        {
            Log("<color=green>Client is not connected and cannot disconnect</color>");
            return;
        }

        // Connect the client
        Log("<color=green>Disconnecting from AMQP host: {0}</color>", AmqpHelper.GetConnectionInfo(this.client));
        if (this.OnDisconnecting != null)
            this.OnDisconnecting.Invoke(this);
        this.isDisconnecting = true;
    }

    // Delays true client disconnectiong by a number of seconds
    IEnumerator DelayDisconnection(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (this.client != null)
            this.client.Disconnect();
    }


    /// <summary>
    /// Resets the connection when it has ended up in an aborted state.
    /// </summary>
    public void ResetConnection()
    {
        if (this.client == null)
            return;

        // Connect the client
        Log("<color=green>Reseting connection for AMQP host: {0}</color>", AmqpHelper.GetConnectionInfo(this.client));
        this.client.ResetConnection();
    }
    #endregion // Connection

    #region Event Handlers
    // Handles a connection being blocked
    private void Client_Blocked(object sender, EventArgs e)
    {
        lock (this)
        {
            this.wasBlocked = true;
        }
    }

    // Handles when the client connects to the message broker
    private void Client_Connected(object sender, EventArgs e)
    {
        lock (this)
        {
            this.isConnecting = false;
            this.hasConnected = true;
            this.canSubscribe = true;
        }

        this.client.BasicQos(0, 1, false);
        SubscribeToQueue("avsqueue");
    }

    // Handles when the client starts disconnecting
    private void Client_Disconnecting(object sender, EventArgs e)
    {
        Debug.Log("<color=red><b>Disconnecting Handled...</b></color>");
        lock (this)
        {
            this.isDisconnecting = true;
        }
    }

    // Handles when the client disconnects
    private void Client_Disconnected(object sender, EventArgs e)
    {
        lock (this)
        {
            this.hasDisconnected = true;
        }
    }

    // Handles when the client is trying to reconnect
    private void Client_Reconnecting(object sender, EventArgs e)
    {
        lock (this)
        {
            this.isReconnecting = true;
        }
    }

    // Handles when a connection error occurs
    private void Client_ConnectionError(object sender, ExceptionEventArgs e)
    {
        lock (this)
        {
            this.connectionExceptions.Enqueue(e.Exception);
        }
    }

    // Handles when the client connection is aborted
    private void Client_ConnectionAborted(object sender, EventArgs e)
    {
        lock (this)
        {
            this.hasAborted = true;
        }
    }

    // Handles when a queue is subscribed to
    private void Client_SubscribedToQueue(AmqpQueueSubscription subscription)
    {
        lock (this)
        {
            this.subscribedQueues.Enqueue(subscription);
        }
    }

    // Handles when a queue is unsubscribed from
    private void Client_UnsubscribedFromQueue(AmqpQueueSubscription subscription)
    {
        lock (this)
        {
            this.unsubscribedQueues.Enqueue(subscription);
        }
    }

    // Handles when a queue subscribe error occurs
    private void Client_QueueSubscribeError(object sender, ExceptionEventArgs e)
    {
        lock (this)
        {
            this.queueSubscribeExceptions.Enqueue(e.Exception);
        }
    }

    // Handles when a queue unsubscribe error occurs
    private void Client_QueueUnsubscribeError(object sender, ExceptionEventArgs e)
    {
        lock (this)
        {
            this.queueUnsubscribeExceptions.Enqueue(e.Exception);
        }
    }

    // Handles when a message is received from the client
    void Client_QueueMessageReceived(AmqpQueueReceivedMessage received)
    {
        if (this.isQuitting)
            return;

        // Enqueue the message for processing
        lock (this)
        {
            this.queueMessages.Enqueue(received);
        }
    }
    #endregion // Event Handlers

    #region Subscriptions
    /// <summary>
    /// Subscribes to a given queue.
    /// </summary>
    /// <param name="subscription">The exchange subscription to apply.</param>
    public void Subscribe(AmqpQueueSubscription subscription)
    {
        this.SubscribeToQueue(subscription);
    }

    public void SubscribeToQueue(string name)
    {

        UnityAmqpQueueSubscription subscription =
            new UnityAmqpQueueSubscription(
                name, true, null,
            UnityEventDebugQueueMessageHandler);

        SubscribeToQueue(subscription);
    }

    /// <summary>
    /// Subscribes to a given exchange.
    /// </summary>
    /// <param name="subscription">The exchange subscription to apply.</param>
    public void SubscribeToQueue(AmqpQueueSubscription subscription)
    {
        if (this.isQuitting)
            return;
        if (subscription == null)
            throw new ArgumentNullException("subscription");
        if (this.queueSubscriptions.Contains(subscription))
            return;
        this.queueSubscriptions.Add(subscription);

        // Process new subscriptions if we're currently connected
        this.canSubscribe = true;
    }

    // Restores the current list of subscriptions
    void RestoreSubscriptions()
    {
        if (this.isQuitting)
            return;

        foreach (var queueSubscription in this.queueSubscriptions)
        {
            if (!string.IsNullOrEmpty(queueSubscription.ConsumerTag))
                continue; // already subscribed

            // Assign the thread-safe handler; to interact with Unity's game thread/loop we need this
            queueSubscription.ThreadsafeHandler = this.Client_QueueMessageReceived;

            // If this is the Unity subscription type, hook up its Unity event handler
            if (queueSubscription is UnityAmqpQueueSubscription)
            {
                var unityQueueSubscription = (UnityAmqpQueueSubscription)queueSubscription;

                // Assign a default handler that invokes the UnityEvent
                if (unityQueueSubscription.Handler == null)
                {
                    unityQueueSubscription.Handler = (r) =>
                        {
                            if (unityQueueSubscription.Enabled && unityQueueSubscription.OnMessageReceived != null)
                                unityQueueSubscription.OnMessageReceived.Invoke(r.Subscription, r.Message);
                        };
                }
            }

            // Subscribe with the message broker
            this.client.Subscribe(queueSubscription);
        }
    }

    // Sets up subscriptions on the message broker
    IEnumerator DoRestoreSubscriptions(float delay)
    {
        if (this.isQuitting)
            yield break;
        yield return new WaitForSeconds(delay);
        this.RestoreSubscriptions();
    }

    /// <summary>
    /// unsubscribes from a given queue.
    /// </summary>
    /// <param name="subscription">The queue subscription to unsubscribe from.</param>
    public void Unsubscribe(AmqpQueueSubscription subscription)
    {
        this.UnsubscribeFromQueue(subscription);
    }

    /// <summary>
    /// unsubscribes from a given queue.
    /// </summary>
    /// <param name="subscription">The queue subscription to unsubscribe from.</param>
    public void UnsubscribeFromQueue(AmqpQueueSubscription subscription)
    {
        if (this.isQuitting)
            return;
        if (subscription == null)
            throw new ArgumentNullException("subscription");
        if (this.queueSubscriptions.Contains(subscription))
            this.queueSubscriptions.Remove(subscription);

        if (this.client.IsConnected)
            this.client.Unsubscribe(subscription);
    }
    #endregion // Subscriptions

    #region Publish
    //// TODO Test and support queue publishing scenarios
    ///// <summary>
    ///// Publishes a message to a given queue.
    ///// </summary>
    ///// <param name="queueName">The name of the queue.</param>
    ///// <param name="message">The message to publish.</param>
    ///// <param name="mandatory">Whether or not to publish with the AMQP "mandatory" flag.</param>
    ///// <param name="immediate">Whether or not to publish with the AMQP "immediate" flag.</param>
    public void PublishToQueue(string queueName, string message, bool mandatory = false, bool immediate = false)
    {
        if (this.isQuitting)
            return;
        if (string.IsNullOrEmpty(queueName))
            throw new ArgumentNullException("queueName");
        if (string.IsNullOrEmpty(message))
            throw new ArgumentNullException("message");
        if (this.client == null)
            throw new InvalidOperationException("Must be connected to message broker first.");
        this.client.Publish(string.Empty, queueName, message, mandatory, immediate);
    }
    #endregion // Publish

    #region Queues
    /// <summary>
    /// Declares a queue on the broker for the current virtual host.
    /// </summary>
    /// <param name="name">The name of the queue to declare.</param>
    /// <param name="durable">Whether or not the queue should be durable.</param>
    /// <param name="autoDelete">Whether or not the queue will have auto-delete enabled.</param>
    /// <param name="exclusive">Whether or not the queue is exclusive.</param>
    /// <param name="args">Optional exchange arguments.</param>
    /// <returns>An Exception if one occurred during the operation, otherwise NULL.</returns>
    public Exception DeclareQueue(string name, bool durable = true, bool autoDelete = false, bool exclusive = false, IDictionary<string, object> args = null)
    {
        return this.DeclareQueueOnHost(name, durable, autoDelete, exclusive, args);
    }

    /// <summary>
    /// Declares a queue on the broker for the current virtual host.
    /// </summary>
    /// <param name="name">The name of the queue to declare.</param>
    /// <param name="durable">Whether or not the queue should be durable.</param>
    /// <param name="autoDelete">Whether or not the queue will have auto-delete enabled.</param>
    /// <param name="exclusive">Whether or not the queue is exclusive.</param>
    /// <param name="args">Optional exchange arguments.</param>
    /// <returns>An Exception if one occurred during the operation, otherwise NULL.</returns>
    public Exception DeclareQueueOnHost(string name, bool durable = true, bool autoDelete = false, bool exclusive = false, IDictionary<string, object> args = null)
    {
        return this.client.DeclareQueue(name, durable, autoDelete, exclusive, args);
    }

    /// <summary>
    /// Declares a queue on the broker for the current virtual host.
    /// </summary>
    /// <param name="name">The name of the queue to delete</param>
    /// <param name="ifUnused">Only delete the queue if it is currently unused.</param>
    /// <param name="ifEmpty">Only delete the queue if it is empty.</param>
    /// <returns>An Exception if one occurred during the operation, otherwise NULL.</returns>
    public Exception DeleteQueue(string name, bool ifUnused = false, bool ifEmpty = false)
    {
        return this.DeleteQueueOnHost(name, ifUnused, ifEmpty);
    }

    /// <summary>
    /// Declares a queue on the broker for the current virtual host.
    /// </summary>
    /// <param name="name">The name of the queue to delete</param>
    /// <param name="ifUnused">Only delete the queue if it is currently unused.</param>
    /// <param name="ifEmpty">Only delete the queue if it is empty.</param>
    /// <returns>An Exception if one occurred during the operation, otherwise NULL.</returns>
    public Exception DeleteQueueOnHost(string name, bool ifUnused = false, bool ifEmpty = false)
    {
        return this.client.DeleteQueue(name, ifUnused, ifEmpty);
    }

    /// <summary>
    /// Gets whether or not a queue by a given name exists.
    /// </summary>
    /// <param name="name">The name of the queue to check for.</param>
    /// <param name="virtualHost">The optional virtual host to get queues for. If NULL the connection's default virtual host is used.</param>
    /// <returns>True if the queue exists, False if not.</returns>
    public bool QueueExists(string name, string virtualHost = null)
    {
        return this.QueueExistsOnHost(name, virtualHost);
    }

    /// <summary>
    /// Gets whether or not a queue by a given name exists.
    /// </summary>
    /// <param name="name">The name of the queue to check for.</param>
    /// <param name="virtualHost">The optional virtual host to get queues for. If NULL the connection's default virtual host is used.</param>
    /// <returns>True if the queue exists, False if not.</returns>
    public bool QueueExistsOnHost(string name, string virtualHost = null)
    {
        return this.client.QueueExists(name, virtualHost);
    }

    /// <summary>
    /// Gets a list of queues for the current connection.
    /// </summary>
    /// <param name="virtualHost">The optional virtual host to get exchanges for. If NULL the connection's default virtual host is used.</param>
    /// <returns>A list of AMQP exchanges for the current connection.</returns>
    public AmqpQueue[] GetQueues(string virtualHost = null)
    {
        return this.GetQueueList(virtualHost);
    }

    /// <summary>
    /// Gets a list of queues for the current connection.
    /// </summary>
    /// <param name="virtualHost">The optional virtual host to get exchanges for. If NULL the connection's default virtual host is used.</param>
    /// <returns>A list of AMQP exchanges for the current connection.</returns>
    public AmqpQueue[] GetQueueList(string virtualHost = null)
    {
        if (!this.IsConnected)
            return new AmqpQueue[0];
        return this.client.GetQueues(virtualHost);
    }

    /// <summary>
    /// Gets a list of queues for the current connection using an asynchronous request.
    /// </summary>
    /// <remarks>
    /// This method is better suited for Unity 3D since it will not block the game thread while the request is made.
    /// </remarks>
    /// <param name="callback">The callback that will receive the results.</param>
    /// <param name="virtualHost">The optional virtual host to get queues for. If NULL the connection's default virtual host is used.</param>
    public void GetQueuesAsync(QueueListEventHandler callback, string virtualHost = null)
    {
        this.GetQueueListAsync(callback, virtualHost);
    }

    /// <summary>
    /// Gets a list of queues for the current connection using an asynchronous request.
    /// </summary>
    /// <remarks>
    /// This method is better suited for Unity 3D since it will not block the game thread while the request is made.
    /// </remarks>
    /// <param name="callback">The callback that will receive the results.</param>
    /// <param name="virtualHost">The optional virtual host to get queues for. If NULL the connection's default virtual host is used.</param>
    public void GetQueueListAsync(QueueListEventHandler callback, string virtualHost = null)
    {
        if (!this.IsConnected)
        {
            if (callback != null)
                callback(new AmqpQueue[0]);
            return;
        }

        this.client.GetQueuesAsync(
            (queueList) =>
                {
                    // Queue the results to be handled on the game thread
                    lock (this)
                    {
                        this.queueListResults.Enqueue(new AsyncQueueListResult(callback, queueList));
                    }
                },
            virtualHost);
    }

    /// <summary>
    /// BasicAck
    /// </summary>
    /// <param name="delivertag"></param>
    /// <param name="multiple"></param>
    public void BasicAck(ulong delivertag, bool multiple)
    {
        this.client.BasicAck(delivertag, multiple);
    }

    /// <summary>
    /// BasicQos
    /// </summary>
    /// <param name="prefetchSize"></param>
    /// <param name="prefetchCount"></param>
    /// <param name="global"></param>
    public void BasicQos(uint prefetchSize, ushort prefetchCount, bool global)
    {
        this.client.BasicQos(prefetchSize, prefetchCount, global);
    }
    #endregion // Queues

    #region Logging
    // <summary>
    /// Logs a value to the console.
    /// </summary>
    /// <remarks>If the inspector property <see cref="WriteToConsole"/> is enabled, 
    /// the value will also be written to the <see cref="AmqpConsole"/>.</remarks>
    /// <param name="value">The value to log.</param>
    public void Log(object value)
    {
        this.LogToConsole(value);
    }

    /// <summary>
    /// Logs a value to the console.
    /// </summary>
    /// <remarks>If the inspector property <see cref="WriteToConsole"/> is enabled, 
    /// the value will also be written to the <see cref="AmqpConsole"/>.</remarks>
    /// <param name="value">The value to log.</param>
    public void LogToConsole(object value)
    {
        Debug.Log(value);
    }

    /// <summary>
    /// Logs a value to the console.
    /// </summary>
    /// <remarks>If the inspector property <see cref="WriteToConsole"/> is enabled, 
    /// the value will also be written to the <see cref="AmqpConsole"/>.</remarks>
    /// <param name="text">The text string to format with values.</param>
    /// <param name="values">The values to use in the formatted string.</param>
    public void Log(string text, params object[] values)
    {
        this.LogToConsole(text, values);
    }

    /// <summary>
    /// Logs a value to the console.
    /// </summary>
    /// <remarks>If the inspector property <see cref="WriteToConsole"/> is enabled, 
    /// the value will also be written to the <see cref="AmqpConsole"/>.</remarks>
    /// <param name="text">The text string to format with values.</param>
    /// <param name="values">The values to use in the formatted string.</param>
    public void LogToConsole(string text, params object[] values)
    {
        Debug.LogFormat(text, values);
    }
    #endregion // Logging

    #region Utility
    /// <summary>
    /// A default message received handler useful for debugging.
    /// </summary>
    /// <param name="subscription">The subscription the message was received on.</param>
    /// <param name="message">The message that was received.</param>
    public void UnityEventDebugQueueMessageHandler(AmqpQueueSubscription subscription, IAmqpReceivedMessage message)
    {
        currentMessage = message;
        if (ServerMode)
        {
            // Decode as text
            var payload = System.Text.Encoding.UTF8.GetString(message.Body);
            // new Color(1f, 0.5f, 0);
            Debug.Log("<b>Server:</b> <color=ff7f00ff>Message received on " + subscription.QueueName + ": " +
                //payload + 
                "</color>");
            SceneMessage sceneMessage = SceneMessage.FromJson(payload);



            this.client.BasicAck(message.DeliveryTag, false);
        }
        else
        {
            // Decode as text
            string payload = System.Text.Encoding.UTF8.GetString(message.Body);
            //new Color(1f, 0.5f, 0);
            Debug.Log("<b>Client:</b> <color=ff7f00ff>Message received on " + subscription.QueueName + ": " +
                //payload + 
                "</color>");
            jobMessage = JobMessage.FromJson(payload);
            

            // Aktuelle Szene als MainScene merken
#if UNITY_EDITOR
            mainScene = EditorSceneManager.GetActiveScene();
#else
            //mainScene = SceneManager.GetActiveScene();
             mainScene = EditorSceneManager.GetActiveScene();
#endif

#if UNITY_EDITOR
            // Neue (leere) Szene erstellen
            newScene = EditorSceneManager.NewScene(
                NewSceneSetup.EmptyScene,
                NewSceneMode.Additive);

            // Neue Szene als aktive Szene setzen
            EditorSceneManager.SetActiveScene(newScene);
#else
            //newScene = SceneManager.CreateScene("");
            //SceneManager.SetActiveScene(newScene);
            newScene = EditorSceneManager.NewScene(
                NewSceneSetup.EmptyScene,
                NewSceneMode.Additive);

            // Neue Szene als aktive Szene setzen
            EditorSceneManager.SetActiveScene(newScene);
#endif

            //###########################
            // Erzeuge Content:
            //GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //cube.transform.position = new Vector3(jobMessage.x, 0f, jobMessage.y);

            Tile newTile = Tile.CreateTileGO(jobMessage.x, jobMessage.y, 5);
            newTile.ProceduralDone += GenerationDone;
            newTile.StartQuery();
            

            Debug.Log(jobMessage.x + "/" + jobMessage.y);
            
        }
    }
    Scene mainScene;
    Scene newScene;
    JobMessage jobMessage;
    IAmqpReceivedMessage currentMessage;

    private void GenerationDone(object sender, EventArgs e)
    {
        //tiles.Add(i + ":" + j, newTile);
        //###########################

        // Szene speichern
        string filename = RelativeAssetPathTo("Scene_" + jobMessage.x + "_" + jobMessage.y + ".unity");
        Debug.Log("before newScene-Path: " + newScene.path);
#if UNITY_EDITOR
        EditorSceneManager.SaveScene(newScene, filename);
#else
        //TODO: SceneManager cant save scenes? What to do? Do we need to save?
        EditorSceneManager.SaveScene(newScene, filename);
#endif
        Debug.Log("after newScene-Path: " + newScene.path);


        SceneMessage sceneMessage = new SceneMessage("replyScene_" + jobMessage.x + "_" + jobMessage.y + ".unity", newScene);
        string jsonMessage = sceneMessage.ToJSON();
        Debug.Log(jsonMessage);
#if UNITY_EDITOR
        EditorSceneManager.CloseScene(newScene, true);
#else
        //TODO: SceneManager cant close scenes, do we have to?
        EditorSceneManager.CloseScene(newScene, true);
#endif 

        Debug.Log("afterClosing newScene-Path: " + newScene.path);
#if UNITY_EDITOR
        EditorSceneManager.SetActiveScene(mainScene);
#else
        SceneManager.SetActiveScene(mainScene);
        EditorSceneManager.SetActiveScene(mainScene);
#endif

        Debug.Log("Reply newScene to queue: " + jobMessage.replyToQueue);
        PublishToQueue(jobMessage.replyToQueue, jsonMessage);
        BasicAck(currentMessage.DeliveryTag, false);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="filename"></param>
    /// <returns></returns>
    public static string RelativeAssetPathTo(string filename)
    {
        return RelativeAssetPath() + "/" + filename;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public static string RelativeAssetPath()
    {
        return "Assets" + Application.dataPath.Substring(Application.dataPath.Length);
    }
#endregion // Utility
#endregion // Methods
}
