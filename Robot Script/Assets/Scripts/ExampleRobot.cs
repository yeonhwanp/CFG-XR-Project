using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Very jank way of connecting two "parts" together using RobotTree.AddRobotPart();
/// </summary>
public class ExampleRobot : MonoBehaviour {

    #region Testing
    //private void Start()
    //{
    //    GameObject sphereObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
    //    Transform sphereTransform = sphereObject.GetComponent<Transform>();
    //    sphereTransform.position = new Vector3(0.0f, 1.0f, 0.0f);
    //    Rigidbody sphereRigid = sphereObject.AddComponent<Rigidbody>();
    //    sphereRigid.useGravity = false;


    //    RobotManagerScript.RobotTree testTree = new RobotManagerScript.RobotTree(sphereObject, "fixedbase", true);
    //    testTree.AddRobotPart();
    //}
    #endregion

    public GameObject robotRoot;

    private void Start()
    {
        int counter = 0;
        RobotManagerScript.RobotTree testTree = RobotManagerScript.RobotTree.ConfigureRobot(robotRoot, "Fixed");
        foreach(KeyValuePair<Joint, RobotManagerScript.RobotTree> entry in testTree.JointDict)
        {
            counter += 1;
        }
        Debug.Log(counter);
    }
}
