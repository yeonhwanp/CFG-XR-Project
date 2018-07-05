using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if end
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

        SetTransforms(jointDict[structure.rootJointID], jointDict, linkDict);

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
        newJoint.transform.position = new Vector3(jointConfig.xLoc, jointConfig.yLoc, jointConfig.zLoc);
        newJoint.transform.rotation = new Quaternion(jointConfig.xRot, jointConfig.yRot, jointConfig.zRot, jointConfig.wRot);
        // Setting ObjectJoint Configs
        newObjectJoint.RotateAxis = new Vector3(jointConfig.xAxisPos, jointConfig.yAxisPos, jointConfig.zAxisPos);
        newObjectJoint.AxisRotation = jointConfig.RotatePosition;
        newObjectJoint.ChildJointIDs = jointConfig.ChildrenJoints;
        newObjectJoint.ChildLinkID = jointConfig.ChildrenLink;
        newObjectJoint.ParentLinkID = jointConfig.ParentLink;
        newObjectJoint.ChildJoints = new List<GameObject>();
        newObjectJoint.ChildObjectJoints = new List<ObjectJoint>();

        return newJoint;
    }

    /// <summary>
    /// Method to recreate a link
    /// </summary>
    public static GameObject GenerateLink(LinkStorage linkConfig)
    {
        GameObject newLink = GenerateShape(linkConfig.shape);

        newLink.transform.localScale = new Vector3(linkConfig.shape.xScale, linkConfig.shape.yScale, linkConfig.shape.zScale);
        newLink.transform.position = new Vector3(linkConfig.xLoc, linkConfig.yLoc, linkConfig.zLoc);
        newLink.transform.rotation = new Quaternion(linkConfig.xRot, linkConfig.yRot, linkConfig.zRot, linkConfig.wRot);

        // There should be code here to set COM and inertiatensors, but will leave that for later.
        // Also will do more stuff with this link component... For now it's just... uh... kind of useless
        RobotLink newRobotLink = newLink.AddComponent<RobotLink>();

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

#endif