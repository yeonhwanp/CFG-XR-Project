using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProtoBuf;

// NOTE: CANNOT STORE VECTOR3 IN PROTOBUF -- HOW TO STORE?

/// <summary>
/// Classes to store the values to be sent over.
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

        newStorage.xRot = thisJoint.xRotation;
        newStorage.yRot = thisJoint.yRotation;
        newStorage.zRot = thisJoint.zRotation;

        #region for root (need to fix)

        //if (isRoot)
        //{
        //    newStorage.rootPosition = rootJoint.transform.position;
        //    newStorage.rootRotation = rootJoint.transform.rotation;
        //}

        #endregion

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
    public float xRot;
    [ProtoMember (2)]
    public float yRot;
    [ProtoMember (3)]
    public float zRot;
    [ProtoMember (4)]
    public float Velocity;

    #region for root (need to fix)
    //[ProtoMember (3)]
    //public Vector3 rootPosition;
    //[ProtoMember (4)]
    //public Quaternion rootRotation;
    #endregion 

    //public PositionStorage (float rot, float vel)
    //{
    //    Rotation = rot;
    //    Velocity = vel;
    //}
}
