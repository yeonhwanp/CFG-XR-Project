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
        private int test = 0;

        #region Public Variables
        // General Properties of the GameObject
        public GameObject SelfObject;
        public Vector3 localCOM;
        public Vector3 inertiaTensor;
        public float mass;

        // BaseType so we know how to render the object
        public string BaseType;
        public bool isRoot;

        // Optional Parameters
        public RobotTree Parent;

        // List of the connected joints -- can get the objects through these as well.
        public Dictionary<Joint, RobotTree> JointDict = new Dictionary<Joint, RobotTree>();
        #endregion

        /// <summary>
        /// Method to actually initialize the part
        /// </summary>
        public RobotTree(GameObject sObject, string typeBase, bool iRoot, RobotTree parent = null)
        {
            Rigidbody thisObejctRB = sObject.GetComponent<Rigidbody>();

            SelfObject = sObject;
            BaseType = typeBase;
            isRoot = iRoot;
            Parent = parent;
            localCOM = thisObejctRB.centerOfMass;
            inertiaTensor = thisObejctRB.inertiaTensor;
            mass = thisObejctRB.mass;
            
        }

        // Configures a new RobotTree given the root of the robot... Yea.
        // These seem to work. Need to test the parents. Should work though.
        public static RobotTree ConfigureRobot(GameObject rootObject, string RobotType, bool _isTheRoot = true, RobotTree parentTree = null)
        {

            HingeJoint[] hingeJoints = rootObject.GetComponents<HingeJoint>();

            if (_isTheRoot)
            {
                RobotTree newRobotTree = new RobotTree(rootObject, RobotType, true);
                newRobotTree.BaseType = RobotType;
                addToList(hingeJoints, newRobotTree, RobotType);
                return newRobotTree;
            }
            else
            {
                RobotTree newRobotTree = new RobotTree(rootObject, RobotType, false);
                newRobotTree.BaseType = RobotType;
                newRobotTree.Parent = parentTree;
                addToList(hingeJoints, newRobotTree, RobotType);
                return null;
            }

        }

        // Method to add the hinge to the list then recursively run ConfigureRobot 
        private static void addToList(HingeJoint[] listJoints, RobotTree theTree, string RobotType)
        {
            foreach(HingeJoint joint in listJoints)
            {
                theTree.JointDict.Add(joint, theTree);
                if (joint.connectedBody != null)
                {
                    ConfigureRobot(joint.connectedBody.gameObject, RobotType, false, theTree);
                }
            }
        }


        #region DebugArea (Currently broken due to dict -> list)
        /// <summary>
        /// Going to create a new RobotTree to attach to a current robotTree instance
        /// Have to give a GameObject you want to attach it to though.
        /// </summary>
        public RobotTree createRobotTreePart(GameObject newObject)
        {
            return new RobotTree(newObject, BaseType, false, this);
        }
        
        /// <summary>
        /// Method for creating a new Joint, to be used in conjunction with createRobotTreePart
        /// Because Unity doesn't allow for independent components, will add to GameObject itself.
        /// 
        /// Also, currently only focusing on the hinge Joint for debug purposes. Can easily add in other joints later.
        /// </summary>
        public Joint createJointPart(string JointType, int limitmin, int limitmax)
        {
            if (JointType.ToLower() == "hinge")
            {
                HingeJoint newJoint = SelfObject.AddComponent<HingeJoint>();
                JointLimits newJointLimits = newJoint.limits;
                newJointLimits.min = limitmin;
                newJointLimits.max = limitmax;
                return newJoint;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Adds a newRobotPart as a child of this RobotTree
        /// 
        /// ATM the gameobject is a cube
        /// </summary>
        public void AddRobotPart()
        {
            GameObject testCubePart = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Rigidbody cubeRigid = testCubePart.AddComponent<Rigidbody>();
            cubeRigid.useGravity = false;
            RobotTree newPart = createRobotTreePart(testCubePart);
            Joint newJoint = createJointPart("hinge", 0, 180);
            newJoint.connectedBody = cubeRigid;
        }
        #endregion
    }
}
