using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotTestManager : MonoBehaviour {

    public GameObject rootObject;
    public PositionList testList = new PositionList();

	// Use this for initialization
	void Start ()
    {
        testList.CreateList(rootObject, testList.PList);

        int counter = 0;

        foreach(PositionStorage storage in testList.PList)
        {
            counter += 1;
        }

        Debug.Log("hello: " + counter);
	}

    private void Update()
    {

        // To test the sending.. But would also have to handle receiving. Whoop de doo going to have to work on this too.
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ClientUDP.UDPSend("127.0.0.1", testList);
        }
    }
}
