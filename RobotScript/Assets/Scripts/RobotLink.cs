using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Quite useless at the moment, but might have some use in the future.
/// Stores all of the values.
/// </summary>
public class RobotLink : MonoBehaviour
{

    #region Public Variables

    // General Properties of the GameObject
    public GameObject SelfObject;
    public Vector3 localCOM;
    public Vector3 inertiaTensor;
    public float mass;

    #endregion

    /// <summary>
    /// Initializer
    /// </summary>
    private void Start()
    {
        Rigidbody thisObjectRB = gameObject.GetComponent<Rigidbody>();
        SelfObject = gameObject;
        localCOM = thisObjectRB.centerOfMass;
        inertiaTensor = thisObjectRB.inertiaTensor;
        mass = thisObjectRB.mass;
    }
}