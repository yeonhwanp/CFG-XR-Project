using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateTestRobot : MonoBehaviour {

    public PositionList testList = new PositionList();

    // Use this for initialization
    void Start ()
    {
        // Creating the structure
        RobotStructure testStructure = new RobotStructure();
        testStructure.rootJointID = 1;

        // Creating Joint 1
        Vector3 onePos = new Vector3(0, 0, 0);
        Quaternion oneRot = new Quaternion(0, 0, 0, 0);
        Vector3 Axis = new Vector3(1, 0, 0);
        float Rotation = 0;
        JointStorage jointStorage1 = MakeMethods.MakeJoint(onePos, oneRot, Axis, Rotation);
        testStructure.JointDict.Add(1, jointStorage1);

        // Creating Link 1
        Vector3 twoPos = new Vector3(0, 1, 0);
        Quaternion twoRot = new Quaternion();
        ObjectSpecs defaultSpecs = MakeMethods.MakeShape("cube", 1, 1, 1);
        LinkStorage linkStorage1 = MakeMethods.MakeLink(twoPos, twoRot, defaultSpecs);
        testStructure.LinkDict.Add(1, linkStorage1);

        // Childrening
        jointStorage1.ChildrenLink = 1;

        ConstructionManager.GenerateRobot(testStructure);

        ObjectJoint tester = GameObject.Find("Sphere").GetComponent<ObjectJoint>();
        if (tester.ChildJoints == null)
            Debug.Log("WHERE IS IT GOING WRONG");

        testList.CreateList(GameObject.Find("Sphere"), testList.PList);
	}

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log(testList.PList.Count);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log(testList.PList);
            testList.PList[0].Rotation = 30;
            ObjectJoint.GetJoints(GameObject.Find("Sphere").GetComponent<ObjectJoint>().ChildObjectJoints, GameObject.Find("Sphere"));
            ObjectJoint.SetJoints(testList, GameObject.Find("Sphere"));
            Debug.Log("hello?");
        }
    }
}
