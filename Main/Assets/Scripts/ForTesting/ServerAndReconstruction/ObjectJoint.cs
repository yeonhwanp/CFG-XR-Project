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

    // Go through the refernece and set all of the things
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





