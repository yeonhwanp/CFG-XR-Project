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

    #region TO BE USED FOR GUI

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