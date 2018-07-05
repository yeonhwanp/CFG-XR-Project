using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.Protobuf;

public class RobotTestManager : MonoBehaviour {

    public GameObject rootObject;
    public PositionList testList = new PositionList();

	// Use this for initialization
	void Start ()
    {
        PositionListCreator.CreateList(rootObject, testList.PList);
        ObjectJoint.SetParents(rootObject);
	}

    private void Update()
    {

        //For testing purposes-- > sends joint information then gets updated joint information, updates the information.
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PositionList newList = ClientUDP.UDPSend("127.0.0.1", testList);
            ObjectJoint.GetJoints(rootObject.GetComponent<ObjectJoint>().ChildObjectJoints, rootObject);
            Debug.Log(newList.PList[0].Rotation);
            ObjectJoint.SetJoints(newList, rootObject);
            Debug.Log("Complete!");
        }
    }
}
