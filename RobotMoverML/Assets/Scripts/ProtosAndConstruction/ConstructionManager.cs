using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//NOTE: Set objects as inactive when initialized because it would look weird otherwise.

/// <summary>
/// Class containing methods to parse robot data.
/// </summary>
public class ConstructionManager : MonoBehaviour {

    /// <summary>
    /// GenerateRobot: Recreates a robot given the proper specifications by:
    ///     1. Recreating all of the joints and links as Unity objects, as well as setting their parents and children.
    ///     2. Setting the transforms of all of the individual objects.
    ///
    /// <param name="structure"> A protobuf object holding all of the necessary information to recreate a robot. </param>
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
            SetTransforms(jointDict[structure.RootJointID], jointDict, linkDict, true);
        else
            Debug.Log("RootJoint not found.");

    }

    /// <summary>
    /// GenerateJoint: Helper method to recreate joints by:
    ///     1. Creating the appropriate GameObject
    ///     2. Setting its position properties.
    ///     3. Setting misc. properties as well as removing the collider.
    ///
    /// <param name="jointConfig"> A protobuf object holding all of the necessary information to recreate a joint. </param>
    ///
    /// <returns> A GameObject representing a joint on a robot. </returns>
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

        Rigidbody newRigid = newJoint.AddComponent<Rigidbody>();
        newRigid.isKinematic = true;
        newRigid.useGravity = false;
        Destroy(newJoint.GetComponent<Collider>());

        return newJoint;
    }

    /// <summary>
    /// GenerateJoint: Helper method to recreate links by:
    ///     1. Creating the appropriate GameObject
    ///     2. Setting its position properties.
    ///     3. Setting misc. properties as well as removing the collider.
    ///
    /// TODO: Need to set COM and inertiatensors upon initialization.
    /// TODO: See if link component is actually useful.
    ///
    /// <param name="linkConfig"> A protobuf object holding all of the necessary information to recreate a link. </param>
    ///
    /// <returns> A GameObject representing a link of a robot. </returns>
    /// </summary>
    public static GameObject GenerateLink(LinkStorage linkConfig)
    {
        GameObject newLink = GenerateShape(linkConfig.Shape);

        newLink.transform.localScale = new Vector3(linkConfig.Shape.ScaleParams[0], linkConfig.Shape.ScaleParams[1], linkConfig.Shape.ScaleParams[2]);
        newLink.transform.position = new Vector3(linkConfig.PositionParams[0], linkConfig.PositionParams[1], linkConfig.PositionParams[2]);
        newLink.transform.rotation = new Quaternion(linkConfig.RotationParams[0], linkConfig.RotationParams[1], linkConfig.RotationParams[2], linkConfig.RotationParams[3]);

        // There should be code here to set COM and inertiatensors.
        RobotLink newRobotLink = newLink.AddComponent<RobotLink>();

        Rigidbody newRigid = newLink.AddComponent<Rigidbody>();
        newRigid.isKinematic = true;
        newRigid.useGravity = false;
        Destroy(newLink.GetComponent<Collider>());

        return newLink;
    }

    /// <summary>
    /// GenerateShape: Helper method to generate the proper shape for a link.
    /// TODO: Add more shapes.
    ///
    /// <param name="specs"> A protobuf object holding all of the information regarding a link's shape. </param>
    ///
    /// <returns> A GameObject representing the shape of a link. </returns>
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
    /// GenerateRobot: Recursively sets transform parent/child properties.
    ///
    /// <param name="root"> A protobuf object holding all of the necessary information to recreate a robot. </param>
    /// <param name="jointDict"> A dictionary mapping jointIDs to their joint GameObjects</param>
    /// <param name="linkDict"> A dictionary mapping linkIDs to their link GameObjects</param>
    /// <param name="_isRoot"> An optional boolean indicating whether an object is the root joint or not</param>
    /// </summary>
    public static void SetTransforms(GameObject root, Dictionary<int, GameObject> jointDict, Dictionary<int, GameObject> linkDict, bool _isRoot=false)
    {
        // Setting up the reference dict here, should only be run in the beginning.
        if (_isRoot)
        {
            foreach (KeyValuePair<int, GameObject> pair in jointDict)
            {
                root.GetComponent<ObjectJoint>().ReferenceDict.Add(pair.Key, pair.Value);
            }
        }

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
            GameObject ScaleProtection = new GameObject();
            ScaleProtection.name = "ScaleProtection";
            rootJoint.ParentLink = linkDict[rootJoint.ParentLinkID];
            ScaleProtection.transform.parent = rootJoint.ParentLink.transform;
            rootJoint.transform.parent = ScaleProtection.transform;
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
