using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//NOTE: Want to set objects as inactive as we spawn them because... it would look weird otherwise.

/// <summary>
/// Class to construct the data from the protobuf
/// </summary>
public class ConstructionManager : MonoBehaviour {

    // Main overarching method to generate the entire robot itself
    public static void GenerateRobot(RobotStructure structure)
    {
        Dictionary<int, GameObject> jointDict = new Dictionary<int, GameObject>();
        Dictionary<int, GameObject> linkDict = new Dictionary<int, GameObject>();

        // Actually, can we set up all of the things inside the dictionaries then just pair them up later? Sounds good?
        foreach(KeyValuePair<int, JointStorage> joints in structure.JointDict)
        {
            GenerateJoint();
        }

        foreach(KeyValuePair<int, LinkStorage> links in structure.LinkDict)
        {
            GenerateLink();
        }
    }

    // Method to make each joint
    public static void GenerateJoint()
    {

    }

    // Method to make each link
    public static void GenerateLink()
    {

    }
}
