using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// This is going to be on a joint itself.
/// But... Do we attach the joints as children of the rigidbodies? I think we do?
/// </summary>

public class ObjectJoint : MonoBehaviour {

    // Objects attached
    public GameObject ParentObject;
    public GameObject ChildObject;

    // Position values
    public Vector3 AxisPoint;
    public Vector3 AxisRotation;
    public float RotateAngle; // It seems like this can be used as the "localRotation?"

    // Just for recognition and debugging purposes
    public string Name;

    private void Start()
    {
        // Setting the child and parent transforms

        if (ParentObject != null)
        {
            gameObject.transform.parent = ParentObject.transform;
        }

        if (ChildObject != null)
        {
            ChildObject.transform.parent = gameObject.transform;
        }

        // Setting the position
        gameObject.transform.RotateAround(AxisPoint, AxisRotation, RotateAngle);

        Debug.Log(Name + ": " + RotateAngle);

    }
}
