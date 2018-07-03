using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProtoBuf;

// NOTE: CANNOT STORE VECTOR3 IN PROTOBUF -- HOW TO STORE (for root)?

// NOTE: Store all of the links and joints in a dictionary, assigning an ID to each.
// NOTE: That way, we can refernece parents and children by the ID values in a dictionary
// NOTE: Then, instantiate each then add into the children of the actual value.
// NOTE: Just annoying to set up...

/// <summary>
/// Classes to store the values to be sent over.
/// </summary>

[ProtoInclude (500, typeof(PositionList))]
[ProtoContract]
public class StorageProto<T>
{
    [ProtoMember (1)]
    public T StoredObject;
}

/// <summary>
/// Holds the root and all of the joints and the links in a dictinoary. 
/// </summary>
public class RobotStructure
{
    public int rootJointID;
    public Dictionary<int, JointStorage> JointDict = new Dictionary<int, JointStorage>();
    public Dictionary<int, LinkStorage> LinkDict = new Dictionary<int, LinkStorage>();
}

// Also... Why are we storing the relative positions? Like, I know, but I don't think we need it in this case. We CAN store it though.
public class JointStorage
{
    // Required Parameters for Position
    public float xLoc;
    public float yLoc;
    public float zLoc;
    public float xRot;
    public float yRot;
    public float zRot;
    public float wRot;

    // Required Parameters for Axis Position
    public float xAxisPos;
    public float yAxisPos;
    public float zAxisPos;
    public float RotatePosition;

    // Optional Parameters
    public JointStorage Parent;
    public List<int> ChildrenJoints = new List<int>();
    public int ChildrenLink = 0; // Using 0 as a null value --> Should never have 0 as ID.
    public int ParentLink = 0;
}

public class LinkStorage
{
    // Required Parameters for Position
    public float xLoc;
    public float yLoc;
    public float zLoc;
    public float xRot;
    public float yRot;
    public float zRot;
    public float wRot;

    // Required Parameters for Center of Mass (But... InertiaTensor and COM are going to be left blank for now because.. I'm not experienced enough lol)
    public float xCOM;
    public float yCOM;
    public float zCOM;

    // Required Parameters for IntertiaTensor
    public float xI;
    public float yI;
    public float zI;

    public float mass;
    public ObjectSpecs shape;
}

public class ObjectSpecs
{
    public string Type;
    public float xScale;
    public float yScale;
    public float zScale;
}

/// <summary>
/// Class to help make the required objects for a storage
/// </summary>
public class MakeMethods : MonoBehaviour
{
    // Returns a new JointStorage
    public static JointStorage MakeJoint(Vector3 Position, Quaternion Rotation, Vector3 Axis, float RotationAmount, JointStorage ParentJoint=null, List<int> Children=null, int child = 0, int parent = 0)
    {
        JointStorage newJointStorage = new JointStorage();
        newJointStorage.xLoc = Position.x;
        newJointStorage.yLoc = Position.y;
        newJointStorage.zLoc = Position.z;
        newJointStorage.xRot = Rotation.x;
        newJointStorage.yRot = Rotation.y;
        newJointStorage.zRot = Rotation.z;
        newJointStorage.wRot = Rotation.w;
        newJointStorage.xAxisPos = Axis.x;
        newJointStorage.yAxisPos = Axis.y;
        newJointStorage.zAxisPos = Axis.z;
        newJointStorage.RotatePosition = RotationAmount;
        newJointStorage.Parent = ParentJoint;
        newJointStorage.ChildrenJoints = Children;
        newJointStorage.ChildrenLink = child;
        newJointStorage.ParentLink = parent;

        return newJointStorage;
    }

    public static LinkStorage MakeLink(Vector3 Position, Quaternion Rotation, ObjectSpecs Shape)
    {
        LinkStorage newLinkStorage = new LinkStorage();
        newLinkStorage.xLoc = Position.x;
        newLinkStorage.yLoc = Position.y;
        newLinkStorage.zLoc = Position.z;
        newLinkStorage.xRot = Rotation.x;
        newLinkStorage.yRot = Rotation.y;
        newLinkStorage.zRot = Rotation.z;
        newLinkStorage.wRot = Rotation.w;
        newLinkStorage.shape = Shape;

        return newLinkStorage;
    }

    public static ObjectSpecs MakeShape(string type, float x, float y, float z)
    {
        ObjectSpecs newShape = new ObjectSpecs();
        newShape.Type = type;
        newShape.xScale = x;
        newShape.yScale = y;
        newShape.zScale = z;

        return newShape;
    }
}


[ProtoInclude(500, typeof(PositionStorage))]
[ProtoContract]
public class PositionList
{
    [ProtoMember (1)]
    public List<PositionStorage> PList = new List<PositionStorage>();

    // Given the root joint, creates a position list to be sent over via protobuf.
    public void CreateList(GameObject rootJoint, List<PositionStorage> defaultList = null, bool isRoot = true)
    {
        List<GameObject> children = rootJoint.GetComponent<ObjectJoint>().ChildJoints;
        ObjectJoint thisJoint = rootJoint.GetComponent<ObjectJoint>();
        PositionStorage newStorage = new PositionStorage();
        newStorage.Rotation = thisJoint.AxisRotation;

        defaultList.Add(newStorage);

        foreach(GameObject joint in children)
        {
            CreateList(joint, defaultList, false);
        }
    }
}

[ProtoContract]
public class PositionStorage : PositionList
{
    [ProtoMember (1)]
    public float Rotation;
    [ProtoMember (2)]
    public float Velocity;
}

//public class