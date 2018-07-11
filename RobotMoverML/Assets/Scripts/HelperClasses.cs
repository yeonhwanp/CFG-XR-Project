using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeMethods : MonoBehaviour
{
    // Returns a new JointStorage
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

    // Returns a new LinkStorage
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

    // Returns a new shape spec
    public static ObjectSpecs MakeShape(string type, float x, float y, float z)
    {
        ObjectSpecs newShape = new ObjectSpecs();
        newShape.Type = type;
        List<float> scale = new List<float> { x, y, z};
        newShape.ScaleParams.AddRange(scale);

        return newShape;
    }
}


/// <summary>
/// I've been confused by this multiple times so I'm going to add a comment here
/// This class holds a method that fills in PositionStorage.PList.
/// </summary>
/// 
// Lets make this a dictionary... It's kind of annoying to guess the stuff. But for now, I'll leave it?
public static class PositionListCreator
{
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