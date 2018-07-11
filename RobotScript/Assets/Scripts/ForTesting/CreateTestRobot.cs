using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Google.Protobuf;


// TODO: Remove Colliders --> not important
// TODO: Add Rigidbody to everything --> not important
// TODO: Create a "root" joint --> Pretty important but not important to function (i think?)
// TODO: Create a robot with four legs --> Goal (so pretty important)
// TODO: Send data over protobuf... PLEASE WORK

// NOTE: Do we want to merge the dictionary into one? But then... Lots of refactoring to do jeez.
// NOTE: I see... what local position does omg ok it makes more sense now.]
// NOTE: Defaults to local position if there is a parent.

// NOTE: Unity bug... Can't scale non-uniformally -> will have shape shifting children  

public class CreateTestRobot : MonoBehaviour {

    // For sending configuration over
    public RobotStructure testStructure = new RobotStructure();
    public PositionList testList = new PositionList();

    // Use this for initialization
    void Start ()
    {
        // Creating the structure

        testStructure.RootJointID = 1;

        // Creating RootJoint
        Vector3 onePos = new Vector3(0, 5, 0);
        Quaternion oneRot = new Quaternion(0, 0, 0, 0);
        Vector3 Axis = new Vector3(0, 0, 1);
        float Rotation = 0;
        JointStorage jointStorage1 = MakeMethods.MakeJoint(onePos, oneRot, Axis, Rotation, isRoot: true);
        testStructure.JointDict.Add(1, jointStorage1);

        // Creating Link 1 (Body of the robot)
        Vector3 twoPos = new Vector3(0, 4, 0);
        Quaternion defaultLinkRotation = new Quaternion();
        ObjectSpecs defaultSpecs = MakeMethods.MakeShape("cube", 5, 1, 5);
        LinkStorage linkStorage1 = MakeMethods.MakeLink(twoPos, defaultLinkRotation, defaultSpecs);
        testStructure.LinkDict.Add(1, linkStorage1);

        // Creating legs (first set)
        Vector3 link2Pos = new Vector3(2, 2, 2);
        ObjectSpecs defaultLeg = MakeMethods.MakeShape("cube", 1, 2, 1);
        LinkStorage linkStorage2 = MakeMethods.MakeLink(link2Pos, defaultLinkRotation, defaultLeg);
        testStructure.LinkDict.Add(2, linkStorage2);

        Vector3 link3Pos = new Vector3(2, 2, -2);
        LinkStorage linkStorage3 = MakeMethods.MakeLink(link3Pos, defaultLinkRotation, defaultLeg);
        testStructure.LinkDict.Add(3, linkStorage3);

        Vector3 link4Pos = new Vector3(-2, 2, 2);
        LinkStorage linkStorage4 = MakeMethods.MakeLink(link4Pos, defaultLinkRotation, defaultLeg);
        testStructure.LinkDict.Add(4, linkStorage4);

        Vector3 link5Pos = new Vector3(-2, 2, -2);
        LinkStorage linkStorage5 = MakeMethods.MakeLink(link5Pos, defaultLinkRotation, defaultLeg);
        testStructure.LinkDict.Add(5, linkStorage5);

        // Creating Joint 2
        Vector3 threePos = new Vector3(2, 3, 2);
        Quaternion threeRot = new Quaternion(0, 0, 0, 0);
        Vector3 threeAxis = new Vector3(0, 0, 1);
        float threeRotation = 0;
        JointStorage jointStorage2 = MakeMethods.MakeJoint(threePos, threeRot, threeAxis, threeRotation);
        testStructure.JointDict.Add(2, jointStorage2);

        // Creating Joint 3
        Vector3 fourPos = new Vector3(2, 3, -2);
        Quaternion fourRot = new Quaternion(0, 0, 0, 0);
        Vector3 fourAxis = new Vector3(0, 0, 1);
        float fourRotation = 0;
        JointStorage jointStorage3 = MakeMethods.MakeJoint(fourPos, fourRot, fourAxis, fourRotation);
        testStructure.JointDict.Add(3, jointStorage3);

        // Creating Joint 4
        Vector3 fivePos = new Vector3(-2, 3, 2);
        Quaternion fiveRot = new Quaternion(0, 0, 0, 0);
        Vector3 fiveAxis = new Vector3(0, 0, 1);
        float fiveRotation = 0;
        JointStorage jointStorage4 = MakeMethods.MakeJoint(fivePos, fiveRot, fiveAxis, fiveRotation);
        testStructure.JointDict.Add(4, jointStorage4);

        // Creating Joint 5
        Vector3 sixPos = new Vector3(-2, 3, -2);
        Quaternion sixRot = new Quaternion(0, 0, 0, 0);
        Vector3 sixAxis = new Vector3(0, 0, 1);
        float sixRotation = 0;
        JointStorage jointStorage5 = MakeMethods.MakeJoint(sixPos, sixRot, sixAxis, sixRotation);
        testStructure.JointDict.Add(5, jointStorage5);

        // Creating Knee Joints (need to switch axis rotation)
        Vector3 kneeOnePos = new Vector3(2, 1, 2);
        Quaternion defaultKneeRotation = new Quaternion(0, 0, 0, 0);
        Vector3 kneeRotationAxis = new Vector3(0, 0, 0);
        float defaultKneeRotationAmount = 0;
        JointStorage kneeOneStorage = MakeMethods.MakeJoint(kneeOnePos, defaultKneeRotation, kneeRotationAxis, defaultKneeRotationAmount);
        testStructure.JointDict.Add(6, kneeOneStorage);

        Vector3 kneeTwoPos = new Vector3(2, 1, -2);
        JointStorage kneeTwoStorage = MakeMethods.MakeJoint(kneeTwoPos, defaultKneeRotation, kneeRotationAxis, defaultKneeRotationAmount);
        testStructure.JointDict.Add(7, kneeTwoStorage);

        Vector3 kneeThreePos = new Vector3(-2, 1, 2);
        JointStorage kneeThreeStorage = MakeMethods.MakeJoint(kneeThreePos, defaultKneeRotation, kneeRotationAxis, defaultKneeRotationAmount);
        testStructure.JointDict.Add(8, kneeThreeStorage);

        Vector3 kneeFourPos = new Vector3(-2, 1, -2);
        JointStorage kneeFourStorage = MakeMethods.MakeJoint(kneeFourPos, defaultKneeRotation, kneeRotationAxis, defaultKneeRotationAmount);
        testStructure.JointDict.Add(9, kneeFourStorage);

        // Creating Final Leg Links
        Vector3 shinLeg1Pos = new Vector3(2, -0.5f, 2);
        LinkStorage shinLeg1Storage = MakeMethods.MakeLink(shinLeg1Pos, defaultLinkRotation, defaultLeg);
        testStructure.LinkDict.Add(6, shinLeg1Storage);

        Vector3 shinLeg2Pos = new Vector3(2, -0.5f, -2);
        LinkStorage shinLeg2Storage = MakeMethods.MakeLink(shinLeg2Pos, defaultLinkRotation, defaultLeg);
        testStructure.LinkDict.Add(7, shinLeg2Storage);

        Vector3 shinLeg3Pos = new Vector3(-2, -0.5f, 2);
        LinkStorage shinLeg3Storage = MakeMethods.MakeLink(shinLeg3Pos, defaultLinkRotation, defaultLeg);
        testStructure.LinkDict.Add(8, shinLeg3Storage);

        Vector3 shinLeg4Pos = new Vector3(-2, -0.5f, -2);
        LinkStorage shinLeg4Storage = MakeMethods.MakeLink(shinLeg4Pos, defaultLinkRotation, defaultLeg);
        testStructure.LinkDict.Add(9, shinLeg4Storage);

        // Childrening from the root
        jointStorage1.ChildrenLink = 1;
        jointStorage1.ChildrenJoints.Add(2);
        jointStorage1.ChildrenJoints.Add(3);
        jointStorage1.ChildrenJoints.Add(4);
        jointStorage1.ChildrenJoints.Add(5);

        jointStorage2.ChildrenLink = 2;
        jointStorage2.ChildrenJoints.Add(6);

        jointStorage3.ChildrenLink = 3;
        jointStorage3.ChildrenJoints.Add(7);

        jointStorage4.ChildrenLink = 4;
        jointStorage4.ChildrenJoints.Add(8);

        jointStorage5.ChildrenLink = 5;
        jointStorage5.ChildrenJoints.Add(9);

        kneeOneStorage.ChildrenLink = 6;
        kneeTwoStorage.ChildrenLink = 7;
        kneeThreeStorage.ChildrenLink = 8;
        kneeFourStorage.ChildrenLink = 9;


        // Childrening to the main body
        jointStorage2.ParentLink = 1;
        jointStorage3.ParentLink = 1;
        jointStorage4.ParentLink = 1;
        jointStorage5.ParentLink = 1;
        kneeOneStorage.ParentLink = 2;
        kneeTwoStorage.ParentLink = 3;
        kneeThreeStorage.ParentLink = 4;
        kneeFourStorage.ParentLink = 5;

        ConstructionManager.GenerateRobot(testStructure); // creates the robot from local data

        // Moving robot a little up just for presentation
        GameObject root = GameObject.Find("Sphere");
        PositionListCreator.CreateDict(root, testList.PList);
        root.transform.Translate(Vector3.up);
    }

    private void Update()
    {
        // Lets just somehow make it look like it's moving...
        // Will have to move the first joints up, then move the second joints in
        // 
        if (Input.GetKeyDown(KeyCode.Space))
        {
            testList.PList[2].Rotation = 30;
            ObjectJoint.SetJoints(testList, GameObject.Find("Sphere"));
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            testList.PList[0].Rotation = -30;
            //ObjectJoint.SetJoints(testList, GameObject.Find("Sphere"));
        }


#if ServerTesting
        // Configuring the robot with server test
        if (Input.GetKeyDown(KeyCode.A))
        {
            RobotStructure test = ClientUDP<RobotStructure>.UDPSend(8888, testStructure);
            ConstructionManager.GenerateRobot(test);
            ObjectJoint.GetJoints(GameObject.Find("Sphere").GetComponent<ObjectJoint>().ChildObjectJoints, GameObject.Find("Sphere"));
        }

        // Moving the thing
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PositionListCreator.CreateList(GameObject.Find("Sphere"), testList.PList);
            PositionList newList = ClientUDP<PositionList>.UDPSend(7777, testList);
            ObjectJoint.SetJoints(newList, GameObject.Find("Sphere"));
        }
#endif
    }
}