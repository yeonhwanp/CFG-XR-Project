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
    public GameObject ChildLink;
    public bool IsRoot;

    // Position values
    public Vector3 AxisPoint;
    public Vector3 AxisRotation; 
    public float RotateAngle { get; set; } // Seems like it can be used as "localRotation"

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
            float newPosition = storage.Rotation;
            float newVelocity = storage.Velocity;

            Debug.Log("new" + newPosition);
            allJoints[counter].transform.RotateAround(allJoints[counter].AxisPoint, allJoints[counter].AxisRotation, newPosition);
            allJoints[counter].RotateAngle = newPosition;
            allJoints[counter].LocalVelocity = newVelocity;

            counter += 1;
        }
    }

    // Set the transforms on load.
    private void Start()
    {
        IsRoot = ParentJoint == null ? true : false;

        foreach(GameObject child in ChildJoints)
        {
            child.transform.parent = gameObject.transform;
        }

        ChildLink.transform.parent = gameObject.transform;
    }
}


