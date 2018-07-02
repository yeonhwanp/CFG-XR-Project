using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstructionManager : MonoBehaviour {

    public static void GenerateRobot(RobotStructure structure)
    {
        JointStorage root = structure.rootJoint;
        // Now need methods here to Look at the chidlren and make the robot from there...
        Vector3 spawnPos = new Vector3(root.xLoc, root.yLoc, root.zLoc);
        Quaternion spawnRot = new Quaternion(root.xRot, root.yRot, root.zRot, root.wRot);
        GameObject rootObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        rootObject.transform.position = spawnPos;
        rootObject.transform.rotation = spawnRot;
        ObjectJoint rootJComponent = rootObject.AddComponent<ObjectJoint>();
    }
}
