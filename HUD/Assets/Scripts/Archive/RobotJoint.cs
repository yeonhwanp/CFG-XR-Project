using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Need to update the component with a "limit axis" --> Use clamp for that

public class RobotJoint : MonoBehaviour {

    public GameObject ChildGO;
    public string rotationDirection;
    public Quaternion JointRotation;
    public float JointVelocity;

    // On start, it's going to match the configs
    private void Start()
    {
        ChildGO.transform.parent = gameObject.transform;

        JointRotation = ChildGO.transform.localRotation;
        JointVelocity = ChildGO.GetComponent<Rigidbody>().velocity.magnitude;

        //rotate around axis functions
    }

}
