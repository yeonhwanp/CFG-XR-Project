using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> MakeMethods: Class to hold methods to help with saving robot parts. </summary>
public class MakeMethods : MonoBehaviour
{
    /// <summary>
    /// MakeJoint: Creates a new JointStorage object.
    ///
    /// <param name="Position"> A vector representing the global position of the joint. </param>
    /// <param name="Rotation"> A quaternion representing the global rotation of the joint. </param>
    /// <param name="Axis"> A vector representing the axis that the joint is rotating around. </param>
    /// <param name="RotationAmount"> A float representing the amount that the joint is rotated around the axis. </param>
    /// <param name="isRoot"> An optional boolean indicating whether this joint is the root joint. </param>
    /// <param name="ParentJoint"> An optional JointStorage object referring to a parent joint. </param>
    /// <param name="Children"> An optional list of children jointIDs. </param>
    /// <param name="child"> An optional linkID indicating the child link. </param>
    /// <param name = "parent"> An optional linkID indicating the parent link. </param>
    ///
    /// <returns> The newly generated JointStorage object. </returns>
    /// </summary>
    public static JointStorage MakeJoint(Vector3 Position, Quaternion Rotation, Vector3 Axis, float RotationAmount, bool isRoot = false, JointStorage ParentJoint = null, List<int> Children = null, int child = 0, int parent = 0)
    {
        JointStorage newJointStorage = new JointStorage();
        // Question is... Do we need to store this information? For now... but I'll ask about it
        List<float> positions = new List<float> { Position.x, Position.y, Position.z };
        List<float> rotations = new List<float> { Rotation.x, Rotation.y, Rotation.z, Rotation.w };
        List<float> axis = new List<float> { Axis.x, Axis.y, Axis.z, RotationAmount };
        newJointStorage.PositionParams.AddRange(positions);
        newJointStorage.RotationParams.AddRange(rotations);
        newJointStorage.AxisParams.AddRange(axis);
        newJointStorage.Parent = ParentJoint;

        // Hm.. But what if we have no children? What happens? (aka no list bug)
        if (Children != null)
        {
            newJointStorage.ChildrenJoints.AddRange(Children);
        }

        newJointStorage.ChildrenLink = child;
        newJointStorage.ParentLink = parent;

        if (isRoot)
        {
            // Store the 6 DOF
        }

        return newJointStorage;
    }

    /// <summary>
    /// MakeLink: Creates a new LinkStorage object.
    ///
    /// <param name="Position"> A vector representing the global position of the link. </param>
    /// <param name="Rotation"> A quaternion representing the global rotation of the link. </param>
    /// <param name="Shape"> A vector representing the axis that the joint is rotating link. </param>
    ///
    /// <returns> The newly generated LinkStorage object. </returns>
    /// </summary>
    public static LinkStorage MakeLink(Vector3 Position, Quaternion Rotation, ObjectSpecs Shape)
    {
        LinkStorage newLinkStorage = new LinkStorage();
        List<float> positions = new List<float> { Position.x, Position.y, Position.z };
        List<float> rotations = new List<float> { Rotation.x, Rotation.y, Rotation.z, Rotation.w };
        newLinkStorage.PositionParams.AddRange(positions);
        newLinkStorage.RotationParams.AddRange(rotations);
        newLinkStorage.Shape = Shape;

        return newLinkStorage;
    }

    /// <summary>
    /// MakeShape: Creates a new ObjectSpecs object.
    ///
    /// <param name="type"> A string representing the type of shape the link is. </param>
    /// <param name="x, y, z"> Coordinates representing the scale of the object.. </param>
    ///
    /// <returns> The newly generated ObjectSpecs object. </returns>
    /// </summary>
    public static ObjectSpecs MakeShape(string type, float x, float y, float z)
    {
        ObjectSpecs newShape = new ObjectSpecs();
        newShape.Type = type;
        List<float> scale = new List<float> { x, y, z};
        newShape.ScaleParams.AddRange(scale);

        return newShape;
    }
}


/// <summary> PositionListCreator: A wrapper class to hold a method that fills in PositionStorage.PList. </summary>
public static class PositionListCreator
{
    /// <summary>
    /// CreateDict: Given the root joint, assigns all of the positions of the joints in the joint's dictionary.
    ///
    /// <param name="rootJoint"> The joint to store the dictioary in. </param>
    /// <param name="defaultDict"> The dictionary to store the PositionStorages in. </param>
    ///
    /// <returns> The newly generated ObjectSpecs object. </returns>
    /// </summary>
    public static void CreateDict(GameObject rootJoint, IDictionary<int, PositionStorage> defaultDict)
    {
        ObjectJoint JointObject = rootJoint.GetComponent<ObjectJoint>();
        foreach(KeyValuePair<int, GameObject> pair in JointObject.ReferenceDict)
        {
            PositionStorage newStorage = new PositionStorage();
            newStorage.Rotation = pair.Value.GetComponent<ObjectJoint>().AxisRotation;
            newStorage.Velocity = pair.Value.GetComponent<ObjectJoint>().LocalVelocity;
            defaultDict[pair.Key] = newStorage;
        }
    }
}


// TODO: Make both classes under one (protobuf any?)
// TODO: LocalPosition/LocalRotation/RootDOF?