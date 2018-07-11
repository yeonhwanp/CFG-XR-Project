using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.Protobuf;

/// <summary>
/// Testing sending joint positions
/// </summary>
/// 


#if end

public class RobotTestManager : MonoBehaviour {

    public GameObject rootObject;
    public PositionList testList = new PositionList();

	// Use this for initialization
	void Start ()
    {
        PositionListCreator.CreateList(rootObject, testList.PList);
        //ObjectJoint.SetParents(rootObject);
	}

    private void Update()
    {

        //For testing purposes-- > sends joint information then gets updated joint information, updates the information.
        if (Input.GetKeyDown(KeyCode.Space))
        {
            testList.PList[0].Rotation = 30;
            testList.PList[1].Rotation = 60;
            ObjectJoint.GetJoints(rootObject.GetComponent<ObjectJoint>().ChildObjectJoints, rootObject);
            ObjectJoint.SetJoints(testList, rootObject);
            Debug.Log("Complete!");
        }
    }
}

#endif