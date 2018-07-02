using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotTestManager : MonoBehaviour {

    public GameObject rootObject;
    public GameObject secondObject;
    public PositionList testList = new PositionList();
    public StorageProto<PositionList> tester = new StorageProto<PositionList>();


	// Use this for initialization
	void Start ()
    {
        testList.CreateList(rootObject, testList.PList);
        ObjectJoint.SetParents(rootObject);
	}

    private void Update()
    {

        //For testing purposes-- > sends joint information then gets updated joint information, updates the information.
        if (Input.GetKeyDown(KeyCode.Space))
        {
            tester.StoredObject = testList;
            StorageProto<PositionList> test = ClientUDP<PositionList>.UDPSend("127.0.0.1", tester);
            PositionList newList = test.StoredObject;
            Debug.Log(newList.PList[0].Rotation);

            ObjectJoint.GetJoints(rootObject.GetComponent<ObjectJoint>().ChildObjectJoints, rootObject);
            ObjectJoint.SetJoints(newList, rootObject);

            Debug.Log("Complete!");
        }
    }
}
