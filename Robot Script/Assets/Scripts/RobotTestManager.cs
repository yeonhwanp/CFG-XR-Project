using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotTestManager : MonoBehaviour {

    public GameObject rootObject;

	// Use this for initialization
	void Start ()
    {

        PositionList testList = new PositionList();
        testList.CreateList(rootObject, testList.PList);

        int counter = 0;

        foreach(PositionStorage storage in testList.PList)
        {
            counter += 1;
        }

        Debug.Log("hello: " + counter);
	}
}
