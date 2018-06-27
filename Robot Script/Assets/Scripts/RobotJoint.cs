using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// If you add this script to an object and the attach ANOTHERGO, then this becomes the parent of ANOTHERGO.
    // This allows for "joint movement" but the child can move freely... NICE

// Let's write that out in the script.

// Note: Individual parts still affected by gravity if you turn it on?? --> oh wait that makes sense...
// Note: Would that cause problems though? If we're ever using gravity as a factor in our program
public class RobotJoint : MonoBehaviour {

    // GameObject to attach
    public GameObject ChildGO;

    // Properties of the joint that we want
    public Quaternion JointRotation;
    public Vector3 JointVelocity;

    // On start, it's going to match the configs
    private void Start()
    {
        ChildGO.transform.parent = gameObject.transform;

        // Set Joint Properties -- I guess for the rotation/velocity you would want to know what axis its rotating on?
        // For now just simple rotation and velocity values
        JointRotation = ChildGO.transform.rotation;
        JointVelocity = ChildGO.GetComponent<Rigidbody>().velocity;
    }

}
