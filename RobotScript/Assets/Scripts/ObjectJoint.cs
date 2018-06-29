using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProtoBuf;

/// <summary>
/// Simple Joint Component. Parent should be a joint, child should be a gameobject.
/// Uh... correlation with RobotLink kind of jank.
/// </summary>

public class ObjectJoint : MonoBehaviour
{

    // Objects attached
    public GameObject ParentJoint = null;
    public List<GameObject> ChildJoints = new List<GameObject>();
    public List<ObjectJoint> ChildObjectJoints = new List<ObjectJoint>();
    public GameObject ParentLink = null;
    public GameObject ChildLink = null;
    public bool IsRoot;

    // Position values
    public float xRotation;
    public float yRotation;
    public float zRotation;

    // Velocity (Not doing anything with this yet)
    public float LocalVelocity = 0.0f;

    // Just for recognition and debugging purposes
    public string Name;

    // For initailization
    public ObjectJoint (GameObject parent, List<GameObject> children)
    {
        ParentJoint = parent;
        ChildJoints = children;
    }

    // Gets all into a list
    public static void GetJoints(List<ObjectJoint> defaultList, GameObject initialObject)
    {
        ObjectJoint theJoint = initialObject.GetComponent<ObjectJoint>();
        defaultList.Add(theJoint);

        foreach(GameObject childJoint in theJoint.ChildJoints)
        {
            if (childJoint != null)
                GetJoints(defaultList, childJoint);
        }
    }

    // We can set the position of the joints now.
    public static void SetJoints(PositionList newJoints, GameObject rootObject)
    {
        List<PositionStorage> newPositions = newJoints.PList;
        List<ObjectJoint> allJoints = rootObject.GetComponent<ObjectJoint>().ChildObjectJoints;

        int counter = 0;
        foreach (PositionStorage storage in newPositions)
        {
            float newVelocity = storage.Velocity;

            allJoints[counter].transform.Rotate(storage.xRot, storage.yRot, storage.zRot);
            allJoints[counter].LocalVelocity = newVelocity;

            counter += 1;
        }
    }

    // Only to be run one time at start
    public static void SetParents(GameObject Root)
    {
        ObjectJoint rootJoint = Root.GetComponent<ObjectJoint>();

        if (rootJoint.ChildLink != null)
        {
            rootJoint.ChildLink.transform.parent = rootJoint.transform;
        }

        foreach (GameObject joint in rootJoint.ChildJoints)
        {
            joint.transform.parent = rootJoint.transform;
        }

        if (rootJoint.ParentLink != null)
        {
            rootJoint.transform.parent = rootJoint.ParentLink.transform;
        }

        foreach (GameObject joint in rootJoint.ChildJoints)
        {
            SetParents(joint);
        }
    }

    // Set the transforms on load.
    private void Start()
    {
        IsRoot = ParentJoint == null ? true : false;
    }
}


