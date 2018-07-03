using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//NOTE: Want to set objects as inactive as we spawn them because... it would look weird otherwise.
//NOTE: Really hoping that this works... 
//NOTE: Not sure if this works, am going to have to test it... lol.

/// <summary>
/// Class to construct the data from the protobuf
/// </summary>
public class ConstructionManager : MonoBehaviour {

    // Main overarching method to generate the entire robot itself
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

    // Method to make each joint
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

        return newJoint;
    }

    // Method to make each link
    public static GameObject GenerateLink(LinkStorage linkConfig)
    {
        GameObject newLink = new GameObject(); // NOTE: really want to get around this later... I hope that it works LOL

        switch (linkConfig.shape.Type) {
            case "hello":
                newLink = GameObject.CreatePrimitive(PrimitiveType.Cube);
                break;
            default:
                Debug.Log("huh");
                break;
        }

        newLink.transform.localScale = new Vector3(linkConfig.shape.xScale, linkConfig.shape.yScale, linkConfig.shape.zScale);
        newLink.transform.position = new Vector3(linkConfig.xLoc, linkConfig.yLoc, linkConfig.zLoc);
        newLink.transform.rotation = new Quaternion(linkConfig.xRot, linkConfig.yRot, linkConfig.zRot, linkConfig.wRot);

        // There should be code here to set COM and inertiatensors, but will leave that for later.
        // Also will do more stuff with this link component... For now it's just... uh... kind of useless
        RobotLink newRobotLink = newLink.AddComponent<RobotLink>();

        return newLink;
    }

    // Method to set the transforms and relationship properties. Wait I think we good.
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
            rootJoint.ChildJoints.Add(jointDict[ID]);
            jointDict[ID].transform.parent = rootJoint.transform;
        }

        if (rootJoint.ParentLinkID != 0)
        {
            rootJoint.ParentLink = linkDict[rootJoint.ParentLinkID];
            rootJoint.transform.parent = rootJoint.ParentLink.transform;
        }

        foreach (int ID in rootJoint.ChildJointIDs)
        {
            rootJoint.ChildJoints.Add(jointDict[ID]);
            SetTransforms(jointDict[ID], jointDict, linkDict);
        }
    }
}
