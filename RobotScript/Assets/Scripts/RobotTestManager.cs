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
	}

    private void Update()
    {

        // To test the sending.. But would also have to handle receiving. Whoop de doo going to have to work on this too.
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Working?");

            PositionList newList = ClientUDP.UDPSend("127.0.0.1", testList);
            Debug.Log("helo" + newList.PList.Count);

            ObjectJoint.GetJoints(rootObject.GetComponent<ObjectJoint>().ChildObjectJoints, rootObject);
            ObjectJoint.SetJoints(newList, rootObject);

            Debug.Log("Complete!");
        }

        // Should replace this later.
        //rootObject.transform.RotateAround(rootObject.GetComponent<ObjectJoint>().AxisPoint, rootObject.GetComponent<ObjectJoint>().AxisRotation, 50);
    }
}
