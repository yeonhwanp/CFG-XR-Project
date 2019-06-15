using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// RobotLink: Stores all of the properties of a link.
/// Not used at the moment.
///
/// Attributes:
///     SelfObject: The GameObject that this RobotLink object is attached to.
///     localCOM: The center of mass of this link.
///     intertiaTensor: The inertiaTensor of this link.
///     mass: The mass of this link.
/// </summary>
public class RobotLink : MonoBehaviour
{


    #region Public Variables

    // General Properties of the GameObject
    public GameObject SelfObject;
    public Vector3 localCOM;
    public Vector3 inertiaTensor;
    public float mass;

    public GameObject ParentJoint;
    public int SelfID;
    #endregion

    private void Start()
    {
        Rigidbody thisObjectRB = gameObject.GetComponent<Rigidbody>();
        SelfObject = gameObject;
        localCOM = thisObjectRB.centerOfMass;
        inertiaTensor = thisObjectRB.inertiaTensor;
        mass = thisObjectRB.mass;
    }

    #region TO BE USED FOR GUI

    /// <summary>
    /// SpawnJoint: Spawns a new joint object.
    /// </summary>
    public static GameObject SpawnLink()
    {
        Vector3 SpawnPosition = new Vector3(0.05f, 0f, -5.8f);

        GameObject newLink = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        newLink.transform.position = SpawnPosition;
        RobotLink LinkComponent = newLink.AddComponent<RobotLink>();

        return newLink;
    }

    #endregion
}