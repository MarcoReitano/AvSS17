using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CymaticLabs.Unity3D.Amqp;
using System.Threading;

[RequireComponent(typeof(AmqpClient))]
[ExecuteInEditMode]
public class Worker : MonoBehaviour {


    void Awake() {

        Debug.Log("Awake");
        AmqpClient.Instance.OnConnected.AddListener(OnConnected);
        AmqpClient.Instance.Connection = "localhost";
        AmqpClient.Instance.ConnectToHost();

        //amqpClient.DeclareQueueOnHost("myQ");

        //foreach (AmqpQueue q in amqpClient.GetQueueList())
        //{
        //    Debug.Log("Queue: " + q.Name);
        //}

        //amqpClient.DeleteQueueOnHost("myQ");
    }

    public void OnConnected(AmqpClient client)
    {
        Debug.Log("OnConnected");
        Debug.Log("isConnected?" + AmqpClient.Instance.IsConnected);

        AmqpClient.DeclareQueue("Test");

        AmqpQueue[] qs = AmqpClient.Instance.GetQueueList();

        foreach (AmqpQueue q in qs)
        {
            Debug.Log(q.Name);
        }
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnApplicationQuit()
    {
        Debug.Log("ApplicationQuit");
    }
}
