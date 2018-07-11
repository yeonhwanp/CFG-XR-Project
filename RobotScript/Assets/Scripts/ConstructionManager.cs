using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//NOTE: Want to set objects as inactive as we spawn them because... it would look weird otherwise.

/// <summary>
/// Class to help construct robot from data received
/// </summary>
public class ConstructionManager : MonoBehaviour {

    /// <summary>
    /// Create the robot by:
    /// 1. Recreating all of the ObjectJoints/Links
    /// 2. Setting the transforms 
    /// </summary>
    public static void GenerateRobot(RobotStructure structure)
    {
        Dictionary<int, GameObject> jointDict = new Dictionary<int, GameObject>();
        Dictionary<int, GameObject> linkDict = new Dictionary<int, GameObject>();

        // We should have all of the objects with their IDs in the dict now. 
        foreach(KeyValuePair<int, JointStorage> joints in structure.JointDict)
        {
            GameObject GeneratedJoint = GenerateJoint(joints.Value);
            jointDict.Add(joints.Key, GeneratedJoint);
        }

        foreach(KeyValuePair<int, LinkStorage> links in structure.LinkDict)
        {
            GameObject GeneratedLink = GenerateLink(links.Value);
            linkDict.Add(links.Key, GeneratedLink);
        }

        if (jointDict.ContainsKey(structure.RootJointID))
            SetTransforms(jointDict[structure.RootJointID], jointDict, linkDict);
        else
            Debug.Log("RootJoint not found.");

    }

    /// <summary>
    /// Method to recreate a joint
    /// </summary>
    public static GameObject GenerateJoint(JointStorage jointConfig)
    {
        // Creating actual joint
        GameObject newJoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        ObjectJoint newObjectJoint = newJoint.AddComponent<ObjectJoint>();
        // Putting into right position
        newJoint.transform.position = new Vector3(jointConfig.PositionParams[0], jointConfig.PositionParams[1], jointConfig.PositionParams[2]);
        newJoint.transform.rotation = new Quaternion(jointConfig.RotationParams[0], jointConfig.RotationParams[1], jointConfig.RotationParams[2], jointConfig.RotationParams[3]);
        // Setting ObjectJoint Configs
        newObjectJoint.RotateAxis = new Vector3(jointConfig.AxisParams[0], jointConfig.AxisParams[1], jointConfig.AxisParams[2]);
        newObjectJoint.AxisRotation = jointConfig.AxisParams[3];
        newObjectJoint.ChildJointIDs = jointConfig.ChildrenJoints;
        newObjectJoint.ChildLinkID = jointConfig.ChildrenLink;
        newObjectJoint.ParentLinkID = jointConfig.ParentLink;
        newObjectJoint.ChildJoints = new List<GameObject>();
        newObjectJoint.ChildObjectJoints = new List<ObjectJoint>();

        Rigidbody newRigid = newJoint.AddComponent<Rigidbody>();
        newRigid.isKinematic = true;
        newRigid.useGravity = false;
        Destroy(newJoint.GetComponent<Collider>());

        return newJoint;
    }

    /// <summary>
    /// Method to recreate a link 
    /// </summary>
    public static GameObject GenerateLink(LinkStorage linkConfig)
    {
        GameObject newLink = GenerateShape(linkConfig.Shape);

        newLink.transform.localScale = new Vector3(linkConfig.Shape.ScaleParams[0], linkConfig.Shape.ScaleParams[1], linkConfig.Shape.ScaleParams[2]);
        newLink.transform.position = new Vector3(linkConfig.PositionParams[0], linkConfig.PositionParams[1], linkConfig.PositionParams[2]);
        newLink.transform.rotation = new Quaternion(linkConfig.RotationParams[0], linkConfig.RotationParams[1], linkConfig.RotationParams[2], linkConfig.RotationParams[3]);

        // There should be code here to set COM and inertiatensors, but will leave that for later.
        // Also will do more stuff with this link component... For now it's just... uh... kind of useless
        RobotLink newRobotLink = newLink.AddComponent<RobotLink>();

        Rigidbody newRigid = newLink.AddComponent<Rigidbody>();
        newRigid.isKinematic = true;
        newRigid.useGravity = false;
        Destroy(newLink.GetComponent<Collider>());

        return newLink;
    }

    /// <summary>
    /// Used to help GenerateLink (which object should it be?)
    /// </summary>
    public static GameObject GenerateShape(ObjectSpecs specs)
    {
        switch (specs.Type)
        {
            case "cube":
                GameObject newLink = GameObject.CreatePrimitive(PrimitiveType.Cube);
                return newLink;
            default:
                return null;
        }
    }

    /// <summary>
    /// Sets all of the transforms.
    /// </summary>
    public static void SetTransforms(GameObject root, Dictionary<int, GameObject> jointDict, Dictionary<int, GameObject> linkDict)
    {
        ObjectJoint rootJoint = root.GetComponent<ObjectJoint>();

        if (rootJoint.ChildLinkID != 0)
        {
             
            rootJoint.ChildLink = linkDict[rootJoint.ChildLinkID];
            rootJoint.ChildLink.transform.parent = rootJoint.transform;
        }

        foreach (int ID in rootJoint.ChildJointIDs)
        {
            if (ID != 0)
            {
                rootJoint.ChildJoints.Add(jointDict[ID]);
                jointDict[ID].transform.parent = rootJoint.transform;
            }
        }

        if (rootJoint.ParentLinkID != 0)
        {
            rootJoint.ParentLink = linkDict[rootJoint.ParentLinkID];
            rootJoint.transform.parent = rootJoint.ParentLink.transform;
        }

        foreach (int ID in rootJoint.ChildJointIDs)
        {
            if (ID != 0)
            {
                SetTransforms(jointDict[ID], jointDict, linkDict);
            }
        }
    }
}
