using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProtoBuf;

/// <summary>
/// <T> So we can store anything and unwarp generically.
/// </summary>

[ProtoInclude (500, typeof(PositionList))]
[ProtoContract]
public class StorageProto<T>
{
    [ProtoMember (1)]
    public T StoredObject;
}

#region RobotStructure

/// <summary>
/// Used to hold all of the joint/link information.
/// </summary>
public class RobotStructure
{
    public int rootJointID;
    public Dictionary<int, JointStorage> JointDict = new Dictionary<int, JointStorage>();
    public Dictionary<int, LinkStorage> LinkDict = new Dictionary<int, LinkStorage>();
}

/// <summary>
/// Stores information required to create an ObjectJoint
/// </summary>
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

/// <summary>
/// Stores information required to create a Link
/// </summary>
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

/// <summary>
/// Stores information about the shape of the link
/// </summary>
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
    public static JointStorage MakeJoint(Vector3 Position, Quaternion Rotation, Vector3 Axis, float RotationAmount, JointStorage ParentJoint=null, List<int> Children = null, int child = 0, int parent = 0)
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
        if (Children != null)
        {
            newJointStorage.ChildrenJoints = Children; 
        }
        else
        {
            newJointStorage.ChildrenJoints = new List<int>();
        }
        newJointStorage.ChildrenLink = child;
        newJointStorage.ParentLink = parent;

        return newJointStorage;
    }

    // Returns a new LinkStorage
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

    // Returns a new shape spec
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

#endregion

#region JointInfo

/// <summary>
/// Below: Data structures to hold information about joint movement and position.
/// </summary>

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

#endregion