using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotManagerScript : MonoBehaviour
{
    /// <summary>
    /// At the moment, just a structure. Also, most variables are public. Need to change this soon...
    /// </summary>
    public class RobotTree
    {

        #region Public Variables
        // General Properties of the GameObject
        public GameObject SelfObject;
        public Vector3 localCOM;
        public Vector3 inertiaTensor;
        public float mass;

        // If BaseType == "Floating"
        public Vector3 RootPosition;
        public Quaternion RootRotation;

        // Variables to know how we should be rendering object and what we should be storing.
        public string BaseType;
        public bool isRoot;

        // Optional Parameters
        public RobotTree Parent;

        // Directory of connected Joints. This one is for getting the children. The list is for all of the joints (to send back and forth).
        public Dictionary<RobotJoint, RobotTree> JointDict = new Dictionary<RobotJoint, RobotTree>();
        public List<RobotJoint> RobotJoints = new List<RobotJoint>();

        #endregion

        /// <summary>
        /// Initializer
        /// </summary>
        public RobotTree(GameObject sObject, string typeBase, bool iRoot, RobotTree parent = null)
        {
            Rigidbody thisObjectRB = sObject.GetComponent<Rigidbody>();

            SelfObject = sObject;
            BaseType = typeBase;
            isRoot = iRoot;
            Parent = parent;
            localCOM = thisObjectRB.centerOfMass;
            inertiaTensor = thisObjectRB.inertiaTensor;
            mass = thisObjectRB.mass;

            // If it's a root and floating, then we need to store the position as well as the rotation
            if (isRoot && BaseType.ToLower() == "floating")
            {
                RootPosition = thisObjectRB.position;
                RootRotation = thisObjectRB.rotation;
            }
        }

        /// <summary>
        /// Makes a new "RobotTree" instance given the root of a robot
        /// </summary>
        public static RobotTree ConfigureRobot(GameObject rootObject, string RobotType, bool _isTheRoot = true, RobotTree parentTree = null, List<RobotJoint> originalList = null)
        {

            RobotJoint[] robotJoints = rootObject.GetComponents<RobotJoint>();

            if (_isTheRoot)
            {
                RobotTree newRobotTree = new RobotTree(rootObject, RobotType, true);
                newRobotTree.BaseType = RobotType;
                addToList(robotJoints, newRobotTree, RobotType, newRobotTree.RobotJoints);
                return newRobotTree;
            }
            else
            {
                RobotTree newRobotTree = new RobotTree(rootObject, RobotType, false);
                newRobotTree.BaseType = RobotType;
                newRobotTree.Parent = parentTree;
                addToList(robotJoints, newRobotTree, RobotType, originalList);
                return null;
            }

        }

        /// <summary>
        /// Helps ConfigureRobot by adding into the dict and list, recurses onto the next child
        /// </summary>
        private static void addToList(RobotJoint[] listJoints, RobotTree theTree, string RobotType, List<RobotJoint> originalList = null)
        {
            foreach (RobotJoint joint in listJoints)
            {
                theTree.JointDict.Add(joint, theTree);
                originalList.Add(joint);
                if (joint.ChildGO != null)
                {
                    ConfigureRobot(joint.ChildGO.gameObject, RobotType, false, theTree, originalList);
                }
            }
        }
    }

    /// <summary>
    /// Protobuf class to hold the information to be sent over
    /// </summary>
    public class SendJointClass
    {
        public List<RobotJoint> JointList;
        public Transform RootTransform;

        public SendJointClass(List<RobotJoint> listJoint, Transform transformRoot)
        {
            JointList = listJoint;
            RootTransform = transformRoot;
        }
    }
}
