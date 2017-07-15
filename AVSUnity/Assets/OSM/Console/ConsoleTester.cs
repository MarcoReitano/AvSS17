using UnityEngine;
using System.Collections;

public class ConsoleTester : MonoBehaviour {

    private float startTime;
    private float lapTime;

	// Use this for initialization
	void Start () {
        startTime = Time.time;
        lapTime = startTime;
        Console.AddMessage(startTime + ": Startet ConsoleTester");
	}
	
	// Update is called once per frame
	void Update () {

        if ((Time.time - lapTime) > 2f)
        {
            lapTime = Time.time;
            Console.AddMessage(Time.time + ": Startet ConsoleTester");
        }

	}
}
