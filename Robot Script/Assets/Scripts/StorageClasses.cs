using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.Protobuf;


/// <summary>
/// Classes to store the values to be sent over.
/// </summary>

public class PositionList
{
    public List<PositionStorage> PList = new List<PositionStorage>();

    // Given the root joint, creates a position list to be sent over via protobuf.
    public void CreateList(GameObject rootJoint, List<PositionStorage> defaultList = null, bool isRoot = true)
    {
        List<GameObject> children = rootJoint.GetComponent<ObjectJoint>().ChildJoints;
        ObjectJoint thisJoint = rootJoint.GetComponent<ObjectJoint>();
        PositionStorage newStorage = new PositionStorage(thisJoint.RotateAngle, thisJoint.LocalVelocity);

        if (isRoot)
        {
            newStorage.rootPosition = rootJoint.transform.position;
            newStorage.rootRotation = rootJoint.transform.rotation;
        }

        defaultList.Add(newStorage);

        foreach(GameObject joint in children)
        {
            CreateList(joint, defaultList, false);
        }
    }
}

public class PositionStorage : PositionList
{
    public float Rotation;
    public float Velocity;
    public Vector3 rootPosition;
    public Quaternion rootRotation;

    public PositionStorage (float rot, float vel)
    {
        Rotation = rot;
        Velocity = vel;
    }
}
