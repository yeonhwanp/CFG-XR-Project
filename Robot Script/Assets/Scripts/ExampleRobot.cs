﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Very jank way of connecting two "parts" together using RobotTree.AddRobotPart();
/// </summary>
public class ExampleRobot : MonoBehaviour {

    public GameObject robotRoot;

    private void Start()
    {
        int counter = 0;
        RobotManagerScript.RobotTree testTree = RobotManagerScript.RobotTree.ConfigureRobot(robotRoot, "Fixed");
        foreach(RobotJoint entry in testTree.RobotJoints)
        {
            counter += 1;
        }
        Debug.Log(counter);
    }
}
