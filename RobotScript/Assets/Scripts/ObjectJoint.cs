using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProtoBuf;

/// <summary>
/// Simple Joint Component. Parent should be a joint, child should be a gameobject.
/// Uh... correlation with RobotLink kind of jank.
/// </summary>

    // NOTE: FOUND SOMETHING OUT TODAY... CAN'T INITIALIZE EMPTY LISTS OUTSIDE OF A FUNCTION???

public class ObjectJoint : MonoBehaviour
{

    // Objects attached
    public GameObject ParentJoint = null;
    public List<GameObject> ChildJoints = new List<GameObject>();
    public List<ObjectJoint> ChildObjectJoints = new List<ObjectJoint>();
    public GameObject ParentLink = null;
    public GameObject ChildLink = null;
    public bool IsRoot;

    // Useful when creating from script
    public List<int> ChildJointIDs = new List<int>(); 
    public int ChildLinkID; 
    public int ParentLinkID; 

    // Position values 
    public Vector3 RotateAxis;
    public float AxisRotation;

    // Velocity (Not doing anything with this yet)
    public float LocalVelocity = 0.0f;

    // Creates the list of all of the joints --> stored in the root joint
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

    // Sets all of the positions of the joints given the new joint position list
    public static void SetJoints(PositionList newJoints, GameObject rootObject)
    {
        List<PositionStorage> newPositions = newJoints.PList;
        List<ObjectJoint> allJoints = rootObject.GetComponent<ObjectJoint>().ChildObjectJoints;

        int counter = 0;
        foreach (PositionStorage storage in newPositions)
        {
            float newVelocity = storage.Velocity;
            float newPosition = storage.Rotation;
            GameObject thisObject = allJoints[counter].gameObject;
            ObjectJoint thisJoint = thisObject.GetComponent<ObjectJoint>();

            thisObject.transform.Rotate(thisJoint.RotateAxis, newPosition);
            allJoints[counter].AxisRotation = newPosition;
            allJoints[counter].LocalVelocity = newVelocity;

            counter += 1;
        }
    }

    // Run once at start to set all of the properties
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

    // Simple enough
    private void Start()
    {
        IsRoot = ParentJoint == null ? true : false;
    }
}


