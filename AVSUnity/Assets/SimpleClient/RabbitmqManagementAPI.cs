using System.Collections;
using System.Collections.Generic;
using CymaticLabs.Unity3D.Amqp;
using UnityEngine;
using UnityEngine.Networking;


public enum UnityWebRequestResponseCode
{
    NOT_FINISHED = 0,
    FINISHED_SUCCESSFULLY = 200,
    UNAUTHORIZED = 401,
    SOMETHING_ELSE
}


public class RabbitmqManagementAPI : MonoBehaviour
{
    private AmqpConnection connection;
    public AmqpConnection Connection
    {
        get
        {
            return this.connection;
        }
        set
        {
            this.connection = value;
            this.url = this.Url;
            Debug.Log("Connection set...");
        }
    }


    private string url;
    public string Url
    {
        get
        {
            if (this.connection != null)
            {
                //this.url = "http://" + connection.Username + ":" + connection.Password + "@" + connection.Host + ":" + connection.AmqpPort;
                this.url = "http://" +  connection.Host + ":" + connection.AmqpPort;
            }
            else
                this.url = "URL not set";
            
            return this.url;
        }
    }

    public void Awake()
    {
        Debug.Log("Awake");
        Init();
    }

    public void Start()
    {
        Debug.Log("Start");

        Init();
    }

    public void Init()
    {
        Debug.Log("Init RabbitmqManagementAPI");

        SimpleClient simpleClient = this.gameObject.GetComponent<SimpleClient>();
        if (simpleClient != null)
        {
            this.Connection = SimpleClient.GetConnection(simpleClient.Connection);
            if (this.Connection != null)
            {
                Debug.Log("Connected");
            }
            else
            {
                Debug.Log("No Connection");
               
            }
        }
         
        //this.connection = connection;
        //this.url = "http://" + connection.Username + ":" + connection.Password + "@" + connection.Host + ":" + connection.AmqpPort;
        //this.url = "http://" + connection.Host + ":" + connection.AmqpPort;
        //RabbitmqWebRequest request = new RabbitmqWebRequest(url, UnityWebRequest.kHttpVerbGET);
    }

    List<IEnumerator> requests = new List<IEnumerator>();
    
    //public RabbitmqManagementAPI(AmqpConnection connection)
    //{
    //    this.connection = connection;
    //    this.url = "http://" + connection.Username + ":" + connection.Password + "@" + connection.Host + ":" + connection.AmqpPort;
    //    //this.url = "http://" + connection.Host + ":" + connection.AmqpPort;
    //    //RabbitmqWebRequest request = new RabbitmqWebRequest(url, UnityWebRequest.kHttpVerbGET);
    //}



    #region Requests
    // Overview
    private static RabbitmqWebRequest requestOverview;
    public RabbitmqWebRequest Overview()
    {
        if (this.connection == null)
        {
            Init();
        }
        // GET
        string path = url + "/api/overview";
        if (requestOverview == null)
        {
            requestOverview = new RabbitmqWebRequest(path, UnityWebRequest.kHttpVerbGET);
        }
        else
        {
            switch (requestOverview.ResponseCode)
            {
                case UnityWebRequestResponseCode.NOT_FINISHED:
                    break;
                case UnityWebRequestResponseCode.FINISHED_SUCCESSFULLY:
                    StartCoroutine(requestOverview.GetRequest());
                    break;
                case UnityWebRequestResponseCode.UNAUTHORIZED:
                    StartCoroutine(requestOverview.GetRequest());
                    break;
                case UnityWebRequestResponseCode.SOMETHING_ELSE:
                    StartCoroutine(requestOverview.GetRequest());
                    break;
                default:
                    break;
            }

            //if (requestOverview.ResponseCode == UnityWebRequestResponseCode. requestOverview.RequestFinished && requestOverview.continuous)
            //{
            //    StartCoroutine(requestOverview.GetRequest());
            //}
        }
        
        //StartCoroutine(requestOverview.GetRequest());
        return requestOverview;
    }

    // cluster name
    private static RabbitmqWebRequest requestClusterName;
    public RabbitmqWebRequest ClusterName()
    {
        // GET
        string path = url + "/api/cluster-name";
        requestClusterName = new RabbitmqWebRequest(path, UnityWebRequest.kHttpVerbGET);
        requestClusterName.Start();
        return requestClusterName;
    }

    // nodes
    private static RabbitmqWebRequest requestNodes;
    public RabbitmqWebRequest Nodes()
    {
        // GET
        string path = url + "/api/nodes";
        requestNodes = new RabbitmqWebRequest(path, UnityWebRequest.kHttpVerbGET);
        requestNodes.Start();
        return requestNodes;
    }

    // nodes/name individual Node
    private static RabbitmqWebRequest requestNode;
    public RabbitmqWebRequest Node(string node)
    {
        // GET
        string path = url + "/api/nodes/" + node;
        requestNode = new RabbitmqWebRequest(path, UnityWebRequest.kHttpVerbGET);
        requestNode.Start();
        return requestNode;
    }

    // connections
    private static RabbitmqWebRequest requestConnections;
    public RabbitmqWebRequest Connections()
    {
        // GET
        string path = url + "/api/connections";
        requestConnections = new RabbitmqWebRequest(path, UnityWebRequest.kHttpVerbGET);
        requestConnections.Start();
        return requestConnections;
    }

    // consumers
    private static RabbitmqWebRequest requestConsumers;
    public RabbitmqWebRequest Consumers()
    {
        // GET
        string path = url + "/api/consumers";
        requestConsumers = new RabbitmqWebRequest(path, UnityWebRequest.kHttpVerbGET);
        requestConsumers.Start();
        return requestConsumers;
    }

    // consumers in a given host
    private static RabbitmqWebRequest RequestConsumersInHost;
    public RabbitmqWebRequest ConsumersInHost(string vhost)
    {
        // GET
        string path = url + "/api/consumers/" + vhost;
        RequestConsumersInHost = new RabbitmqWebRequest(path, UnityWebRequest.kHttpVerbGET);
        RequestConsumersInHost.Start();
        return RequestConsumersInHost;
    }

    // queues
    private static RabbitmqWebRequest requestQueues;
    public RabbitmqWebRequest Queues()
    {
        // GET
        string path = url + "/api/queues";
        requestQueues = new RabbitmqWebRequest(path, UnityWebRequest.kHttpVerbGET);
        requestQueues.Start();
        return requestQueues;
    }

    // purge
    private static RabbitmqWebRequest requestPurge;
    public RabbitmqWebRequest Purge(string queue)
    {
        string vhost = this.connection.Host;
        // Delete
        string path = url + "/api/queues/" + vhost + "/" + queue + "/contents";
        requestPurge = new RabbitmqWebRequest(path, UnityWebRequest.kHttpVerbDELETE);
        requestPurge.Start();
        return requestPurge;
    }

    // vhosts
    private static RabbitmqWebRequest requestHosts;
    public RabbitmqWebRequest Hosts()
    {
        // GET
        string path = url + "/api/vhosts";
        requestHosts = new RabbitmqWebRequest(path, UnityWebRequest.kHttpVerbGET);
        requestHosts.Start();
        return requestHosts;
    }

    // aliveness-test
    private static RabbitmqWebRequest requestIsAlive;
    public RabbitmqWebRequest IsAlive(string vhost)
    {
        // GET
        string path = url + "/api/aliveness-test/" + vhost;
        requestIsAlive = new RabbitmqWebRequest(path, UnityWebRequest.kHttpVerbGET);
        requestIsAlive.Start();
        return requestIsAlive;
    }
    #endregion //Requests
}




public class RabbitmqWebRequest
{

    private string url;
    private string method;

    public bool continuous = true;

    public RabbitmqWebRequest(string url, string method)
    {
        this.url = url;
        this.method = method;
        //EditorApplication.update += this.Update;
        //request = new UnityWebRequest(url, method);
        //enumerator = GetRequest(url, method);
    }

    public void Start()
    {
        //EditorApplication.update -= this.Update;
        //EditorApplication.update += this.Update;
        //this.enumerator = GetRequest(url, method);
    }

    private UnityWebRequestResponseCode responseCode;
    public UnityWebRequestResponseCode ResponseCode
    {
        get
        {
            return responseCode;
        }
    }

    private bool requestErrorOccurred;
    public bool RequestErrorOccurred
    {
        get
        {
            return requestErrorOccurred;
        }
    }

    private bool requestFinished;
    public bool RequestFinished
    {
        get
        {
            return requestFinished;
        }
    }

    //private IEnumerator enumerator;
    private UnityWebRequest request;

    //private void Update()
    //{
    //    if (enumerator == null)
    //        return;

    //    if (!enumerator.MoveNext())
    //    {
    //        EditorApplication.update -= this.Update;
    //        //enumerator = GetRequest(url, method);
    //    }
    //}

    public string GetResult()
    {
        if (request != null)
        {
            if (RequestErrorOccurred)
            {
                return "An Error occured";
            }
            if (RequestFinished)
            {
                return request.downloadHandler.text;
            }
        }
        return "Request not send";
    }

    public IEnumerator GetRequest()
    {
        return GetRequest(this.url, this.method);
    }


    private IEnumerator GetRequest(string url, string method)
    {
        //responseCode = UnityWebRequestResponseCode.NOT_FINISHED;
        requestFinished = false;
        requestErrorOccurred = false;

        //UnityWebRequest req = new UnityWebRequest()
        request = new UnityWebRequest(url, method);
        //request.method = method;
        Debug.Log("URL: " + url);

        yield return request.Send();

        requestFinished = true;
        if (request.isError)
        {
            Debug.Log("Something went wrong, and returned error: " + request.error);
            requestErrorOccurred = true;
        }
        else
        {
            // Show results as text
            //Debug.Log(request.downloadHandler.text);
            //string answer = request.downloadHandler.text;

            Debug.Log("request.responseCode = " + request.responseCode);
            if (request.responseCode == 200)
            {
                //responseCode = UnityWebRequestResponseCode.FINISHED_SUCCESSFULLY;
                Debug.Log("Request finished successfully!");
            }
            else if (request.responseCode == 401) // an occasional unauthorized error
            {
                //responseCode = UnityWebRequestResponseCode.UNAUTHORIZED;
                requestErrorOccurred = true;
                //this.enumerator = GetRequest(url, method);
                Debug.Log("Error 401: Unauthorized. Resubmitted request!");
            }
            else
            {
                //responseCode = UnityWebRequestResponseCode.SOMETHING_ELSE;
                requestErrorOccurred = true;
                Debug.Log("Request failed (status:" + request.responseCode + ")");
                //Debug.Log("Answer: "+request.downloadHandler.text);
               //GetRequest(url, method);
            }

            if (!RequestErrorOccurred)
            {
                yield return null;
                // process results
            }
        }
    }
}


public class Arguments
{
}

public class ChannelDetails
{
    public string peer_host
    {
        get; set;
    }
    public int peer_port
    {
        get; set;
    }
    public string connection_name
    {
        get; set;
    }
    public string user
    {
        get; set;
    }
    public int number
    {
        get; set;
    }
    public string node
    {
        get; set;
    }
    public string name
    {
        get; set;
    }
}

public class Queue
{
    public string vhost
    {
        get; set;
    }
    public string name
    {
        get; set;
    }
}

public class RootObject
{
    public Arguments arguments
    {
        get; set;
    }
    public int prefetch_count
    {
        get; set;
    }
    public bool ack_required
    {
        get; set;
    }
    public bool exclusive
    {
        get; set;
    }
    public string consumer_tag
    {
        get; set;
    }
    public ChannelDetails channel_details
    {
        get; set;
    }
    public Queue queue
    {
        get; set;
    }
}