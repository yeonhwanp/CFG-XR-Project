using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Note: Need to add the joints to the parent in order for this to work
//       This is because you need to get the gameObjects from the joints

// Note: still need to include: CoM (in local fram), mass, intertia tensor, link extents (?)
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

        #endregion

        /// <summary>
        /// Method to actually initialize the part
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

        // Configures a new RobotTree given the root of the robot... Yea.
        // These seem to work. Need to test the parents. Should work though.
        public static RobotTree ConfigureRobot(GameObject rootObject, string RobotType, bool _isTheRoot = true, RobotTree parentTree = null)
        {

            RobotJoint[] robotJoints = rootObject.GetComponents<RobotJoint>();

            if (_isTheRoot)
            {
                RobotTree newRobotTree = new RobotTree(rootObject, RobotType, true);
                newRobotTree.BaseType = RobotType;
                addToList(robotJoints, newRobotTree, RobotType);
                return newRobotTree;
            }
            else
            {
                RobotTree newRobotTree = new RobotTree(rootObject, RobotType, false);
                newRobotTree.BaseType = RobotType;
                newRobotTree.Parent = parentTree;
                addToList(robotJoints, newRobotTree, RobotType);
                return null;
            }

        }

        // Method to add the hinge to the list then recursively run ConfigureRobot 
        private static void addToList(RobotJoint[] listJoints, RobotTree theTree, string RobotType)
        {
            foreach (RobotJoint joint in listJoints)
            {
                theTree.JointDict.Add(joint, theTree);
                if (joint.ChildGO != null)
                {
                    ConfigureRobot(joint.ChildGO.gameObject, RobotType, false, theTree);
                }
            }
        }

        /// <summary>
        /// Protobuf to send and receive joint positions and velocities.
        /// </summary>
        public class JointConfigsList
        {
            List<JointConfigs> TheList = new List<JointConfigs>();

            // Takes in the jointlist from the RobotTree, adds as JointConfigs into TheList.
            public void AddToJointList(List<Joint> jointlist)
            {
                foreach (Joint joint in jointlist)
                {

                }
            }

        }

        /// <summary>
        /// Class to store each JointPosition and JointVelocity
        /// But... seems like you can't change the position and velocity? It's read only...
        /// Also, skipping over prismatic joints for now.
        /// </summary>
        public class JointConfigs : JointConfigsList
        {
            public Vector3 JointPosition;
            public Vector3 JointVelocity;

            public JointConfigs(Vector3 position, Vector3 velocity)
            {

            }
        }

    }
}
