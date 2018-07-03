using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    // For sending positions over
    public StorageProto<PositionList> testerPosition = new StorageProto<PositionList>();
    public PositionList PositionList = new PositionList();

    // For sending configuration over
    public StorageProto<RobotStructure> testerRobot = new StorageProto<RobotStructure>();

    // Use this for initialization
    void Start ()
    {
        // Creating the structure
        RobotStructure testStructure = new RobotStructure();
        testStructure.rootJointID = 1;

        testerRobot.StoredObject = testStructure;

        // Creating RootJoint
        Vector3 onePos = new Vector3(0, 5, 0);
        Quaternion oneRot = new Quaternion(0, 0, 0, 0);
        Vector3 Axis = new Vector3(1, 0, 0);
        float Rotation = 0;
        JointStorage jointStorage1 = MakeMethods.MakeJoint(onePos, oneRot, Axis, Rotation, isRoot: true);
        testStructure.JointDict.Add(1, jointStorage1);

        // Creating Link 1 (Body of the robot)
        Vector3 twoPos = new Vector3(0, 4, 0);
        Quaternion twoRot = new Quaternion();
        ObjectSpecs defaultSpecs = MakeMethods.MakeShape("cube", 5, 1, 5);
        LinkStorage linkStorage1 = MakeMethods.MakeLink(twoPos, twoRot, defaultSpecs);
        testStructure.LinkDict.Add(1, linkStorage1);

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

        // Creating Joint 4
        Vector3 sixPos = new Vector3(-2, 3, -2);
        Quaternion sixRot = new Quaternion(0, 0, 0, 0);
        Vector3 sixAxis = new Vector3(0, 0, 1);
        float sixRotation = 0;
        JointStorage jointStorage5 = MakeMethods.MakeJoint(sixPos, sixRot, sixAxis, sixRotation);
        testStructure.JointDict.Add(5, jointStorage5);

        // Childrening from the root
        jointStorage1.ChildrenLink = 1;
        jointStorage1.ChildrenJoints.Add(2);
        jointStorage1.ChildrenJoints.Add(3);
        jointStorage1.ChildrenJoints.Add(4);
        jointStorage1.ChildrenJoints.Add(5);

        // Childrening to the main body
        jointStorage2.ParentLink = 1;
        jointStorage3.ParentLink = 1;
        jointStorage4.ParentLink = 1;
        jointStorage5.ParentLink = 1;

        //ConstructionManager.GenerateRobot(testStructure); // creates the robot
        PositionList.CreateList(GameObject.Find("Sphere"), PositionList.PList);
	}

    private void Update()
    {
        // Configuring the robot
        if (Input.GetKeyDown(KeyCode.A))
        {
            StorageProto<RobotStructure> test = ClientUDP<RobotStructure>.UDPSend("127.0.0.1", testerRobot);
        }

        // Moving the thing
        if (Input.GetKeyDown(KeyCode.Space))
        {
            testerPosition.StoredObject = PositionList;
            StorageProto<PositionList> test = ClientUDP<PositionList>.UDPSend("127.0.0.1", testerPosition);
            PositionList newList = test.StoredObject;

            ObjectJoint.GetJoints(GameObject.Find("Sphere").GetComponent<ObjectJoint>().ChildObjectJoints, GameObject.Find("Sphere"));
            ObjectJoint.SetJoints(newList, GameObject.Find("Sphere"));
        }
    }
}
