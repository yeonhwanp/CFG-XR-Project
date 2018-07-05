using System.Collections;
using System.Collections.Generic;
using ProtoBuf;

/// <summary>
/// <T> So we can store anything and unwarp generically.
/// </summary>

[ProtoInclude(501, typeof(PositionList))]
[ProtoInclude(502, typeof(RobotStructure))]
[ProtoContract]
public class StorageProto<T>
{
    [ProtoMember(1)]
    public T StoredObject;
}

#region RobotStructure

/// <summary>
/// Used to hold all of the joint/link information.
/// </summary>

[ProtoInclude(503, typeof(LinkStorage))]
[ProtoInclude(504, typeof(JointStorage))]
[ProtoContract]
public class RobotStructure : StorageProto<RobotStructure>
{
    [ProtoMember(1)]
    public int rootJointID;
    [ProtoMember(2)]
    public Dictionary<int, JointStorage> JointDict = new Dictionary<int, JointStorage>();
    [ProtoMember(3)]
    public Dictionary<int, LinkStorage> LinkDict = new Dictionary<int, LinkStorage>();
}

/// <summary>
/// Stores information required to create an ObjectJoint
/// </summary>

[ProtoContract]
public class JointStorage : RobotStructure
{
    // Required Parameters for Position
    [ProtoMember(1)]
    public float xLoc;
    [ProtoMember(2)]
    public float yLoc;
    [ProtoMember(3)]
    public float zLoc;
    [ProtoMember(4)]
    public float xRot;
    [ProtoMember(5)]
    public float yRot;
    [ProtoMember(6)]
    public float zRot;
    [ProtoMember(7)]
    public float wRot;

    // Required Parameters for Axis Position
    [ProtoMember(8)]
    public float xAxisPos;
    [ProtoMember(9)]
    public float yAxisPos;
    [ProtoMember(10)]
    public float zAxisPos;
    [ProtoMember(11)]
    public float RotatePosition;

    // Optional Parameters
    [ProtoMember(12)]
    public JointStorage Parent;
    [ProtoMember(13)]
    public List<int> ChildrenJoints = new List<int>();
    [ProtoMember(14)]
    public int ChildrenLink = 0; // Using 0 as a null value --> Should never have 0 as ID.
    [ProtoMember(15)]
    public int ParentLink = 0;
}

/// <summary>
/// Stores information required to create a Link
/// </summary>

[ProtoInclude(505, typeof(ObjectSpecs))]
[ProtoContract]
public class LinkStorage : RobotStructure
{
    // Required Parameters for Position
    [ProtoMember(1)]
    public float xLoc;
    [ProtoMember(2)]
    public float yLoc;
    [ProtoMember(3)]
    public float zLoc;
    [ProtoMember(4)]
    public float xRot;
    [ProtoMember(5)]
    public float yRot;
    [ProtoMember(6)]
    public float zRot;
    [ProtoMember(7)]
    public float wRot;

    // Required Parameters for Center of Mass (But... InertiaTensor and COM are going to be left blank for now because.. I'm not experienced enough lol)
    [ProtoMember(8)]
    public float xCOM;
    [ProtoMember(9)]
    public float yCOM;
    [ProtoMember(10)]
    public float zCOM;

    // Required Parameters for IntertiaTensor
    [ProtoMember(11)]
    public float xI;
    [ProtoMember(12)]
    public float yI;
    [ProtoMember(13)]
    public float zI;

    [ProtoMember(14)]
    public float mass;
    [ProtoMember(15)]
    public ObjectSpecs shape;
}

/// <summary>
/// Stores information about the shape of the link
/// </summary>
[ProtoInclude (506, typeof(LinkStorage))]
[ProtoContract]
public class ObjectSpecs : LinkStorage
{
    [ProtoMember(1)]
    public string Type;
    [ProtoMember(2)]
    public float xScale;
    [ProtoMember(3)]
    public float yScale;
    [ProtoMember(4)]
    public float zScale;
}

#endregion

#region JointInfo

/// <summary>
/// Below: Data structures to hold information about joint movement and position.
/// </summary>

[ProtoInclude(500, typeof(PositionStorage))]
[ProtoContract]
public class PositionList : StorageProto<PositionList>
{
    [ProtoMember(1)]
    public List<PositionStorage> PList = new List<PositionStorage>();
}

[ProtoContract]
public class PositionStorage : PositionList
{
    [ProtoMember(1)]
    public float Rotation;
    [ProtoMember(2)]
    public float Velocity;
}

#endregion