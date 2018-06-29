﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simple Joint Component. Parent should be a joint, child should be a gameobject.
/// Uh... correlation with RobotLink kind of jank.
/// </summary>


public class ObjectJoint : MonoBehaviour
{

    // Objects attached
    public GameObject ParentJoint;
    public List<GameObject> ChildJoints = new List<GameObject>();
    public GameObject ChildLink;
    public bool IsRoot;

    // Position values
    public Vector3 AxisPoint;
    public Vector3 AxisRotation; 
    public float RotateAngle { get; set; } // Seems like it can be used as "localRotation"

    // Velocity (Not doing anything with this yet)
    public float LocalVelocity = 0.0f;

    // Just for recognition and debugging purposes
    public string Name;

    // Setting the variables + the transforms
    public ObjectJoint (GameObject parent, List<GameObject> children)
    {
        ParentJoint = parent;
        ChildJoints = children;

        IsRoot = ParentJoint == null ? true : false;
    }

    // Set the transforms on load.
    private void Start()
    {

        foreach(GameObject child in ChildJoints)
        {
            child.transform.parent = gameObject.transform;
        }

        ChildLink.transform.parent = gameObject.transform;
    }
}


