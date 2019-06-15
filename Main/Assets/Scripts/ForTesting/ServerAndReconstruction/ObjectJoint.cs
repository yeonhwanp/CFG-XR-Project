using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProtoBuf;

/// <summary>
/// ObjectJoint: A container for a joint.
///     Requires that the parent is a joint.
///
/// Attributes:
///
///     --- Attached Objects ---
///     ParentJoint: The GameObject reference to the parent joint.
///     ChildJoints: The children joints associated with this joint.
///     ReferenceDict: A dictionary with references to all of joints. "Used to set positions."
///     ParentLink: The GameObject reference to the parent link.
///     ChildLink: The GameObjet reference to the child link.
///
///     --- Position Values ---
///     RotateAxis: The fixed axis that the joint is rotating around.
///     AxisRotation: How much the joint is rotated around "RotateAxis."
///     LocalVelocity: Not used yet, but stores the localvelocity of the object.
///
///     --- Creation from Script ---
///     ChildJointIDs: A list of the joint's children jointIDs.
///     ChildLinkID: The linkID of the joint's child link.
///     ParentLinkID: The linkID of the joint's parent link.
/// </summary>
public class ObjectJoint : MonoBehaviour
{
    // Objects attached
    public GameObject ParentJoint = null;
    public List<GameObject> ChildJoints = new List<GameObject>();
    public Dictionary<int, GameObject> ReferenceDict = new Dictionary<int, GameObject>(); // A dictionary with references with all of the joints --> Used to set positions.
    public GameObject ParentLink = null;
    public GameObject ChildLink = null;
    public bool IsRoot;

    // Useful when creating from script
    public IList<int> ChildJointIDs = new List<int>();
    public int ChildLinkID;
    public int ParentLinkID;
    public int SelfID;

    // Position values
    public Vector3 RotateAxis;
    public float AxisRotation;

    // Velocity (Not doing anything with this yet)
    public float LocalVelocity = 0.0f;

    /// <summary>
    /// SetJoints: Sets the new positions for all joints of the robot.
    ///
    /// <param name="newJoints"> A PositionList holding all of the new positions of the joints. </param>
    /// <param name="rootObject"> The root joint GameObject. </param>
    /// </summary>
    public static void SetJoints(PositionList newJoints, GameObject rootObject)
    {
        foreach(KeyValuePair<int, PositionStorage> pair in newJoints.PList)
        {
            GameObject joint = rootObject.GetComponent<ObjectJoint>().ReferenceDict[pair.Key];
            ObjectJoint jointComponent = joint.GetComponent<ObjectJoint>();
            jointComponent.transform.Rotate(jointComponent.RotateAxis, pair.Value.Rotation);
            jointComponent.AxisRotation = pair.Value.Rotation;
            jointComponent.AxisRotation = pair.Value.Velocity;
        }
    }

    // Simple enough
    private void Start()
    {
        IsRoot = ParentJoint == null ? true : false;
    }

    #region TO BE USED FOR GUI

    /// <summary>
    /// SpawnJoint: Spawns a new joint object.
    /// </summary>
    public static GameObject SpawnJoint()
    {
        Vector3 SpawnPosition = new Vector3(0.05f, 0f, -5.8f);

        GameObject newJoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        newJoint.transform.position = SpawnPosition;
        ObjectJoint JointComponent = newJoint.AddComponent<ObjectJoint>();
        JointComponent.IsRoot = true;

        return newJoint;
        // Also need to set the rotation axis somehow?
    }

    #endregion
}





