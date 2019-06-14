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

    public GameObject SelfObject;
    public Vector3 localCOM;
    public Vector3 inertiaTensor;
    public float mass;

    private void Start()
    {
        Rigidbody thisObjectRB = gameObject.GetComponent<Rigidbody>();
        SelfObject = gameObject;
        localCOM = thisObjectRB.centerOfMass;
        inertiaTensor = thisObjectRB.inertiaTensor;
        mass = thisObjectRB.mass;
    }
}