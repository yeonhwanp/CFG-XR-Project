using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: Remove Colliders --> not important
// TODO: Add Rigidbody to everything --> not important
// TODO: Create a "root" joint --> Pretty important but not important to function (i think?)
// TODO: Create a robot with four legs --> Goal (so pretty important)

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

        // Creating Joint 2
        Vector3 threePos = new Vector3(0, 2, 0);
        Quaternion threeRot = new Quaternion(0, 0, 0, 0);
        Vector3 threeAxis = new Vector3(0, 0, 1);
        float threeRotation = 0;
        JointStorage jointStorage2 = MakeMethods.MakeJoint(threePos, threeRot, threeAxis, threeRotation);
        testStructure.JointDict.Add(2, jointStorage2);

        // Creating Link 2 (NOT COMPLETE)
        Vector3 fourPos = new Vector3(0, 3, 0);
        Quaternion fourRot = new Quaternion();
        ObjectSpecs Spec2 = MakeMethods.MakeShape("cube", 1, 1, 3);
        LinkStorage linkStorage2 = MakeMethods.MakeLink(fourPos, fourRot, Spec2);
        testStructure.LinkDict.Add(2, linkStorage2);

        // Childrening
        jointStorage1.ChildrenLink = 1;
        jointStorage1.ChildrenJoints.Add(2);

        jointStorage2.ParentLink = 1;
        jointStorage2.ChildrenLink = 2;

        ConstructionManager.GenerateRobot(testStructure);

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
            testList.PList[1].Rotation = 40;
            ObjectJoint.GetJoints(GameObject.Find("Sphere").GetComponent<ObjectJoint>().ChildObjectJoints, GameObject.Find("Sphere"));
            ObjectJoint.SetJoints(testList, GameObject.Find("Sphere"));
            Debug.Log("hello?");
        }
    }
}
