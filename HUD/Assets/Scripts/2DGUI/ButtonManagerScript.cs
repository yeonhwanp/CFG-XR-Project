using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// NOTE: Do we want to be able to move the thing freely after attaching? Probably not. Will set a flag in the thing to prevent from happening.

/// <summary>
/// Class to keep track of button states
/// </summary>
public class ButtonManagerScript : MonoBehaviour {

    public Button JointButton;
    public Button LinkButton;
    public Button MoveButton;
    public Button TransformButton;
    public Button ScaleButton;
    public Button RotateButton;
    public Button AttachButton;

    // Used to use specific modes of manipulation.
    public EnabledButton enabledButton;

    public enum EnabledButton
    {
        MoveButton,
        TransformButton,
        ScaleButton,
        RotateButton
    };

    private void Start()
    {
        JointButton.onClick.AddListener(SpawnJoint);
        LinkButton.onClick.AddListener(SpawnLink);
        MoveButton.onClick.AddListener(EnableCameraMovement);
        TransformButton.onClick.AddListener(EnableTransform);
        ScaleButton.onClick.AddListener(EnableScaling);
        RotateButton.onClick.AddListener(EnableRotation);
        AttachButton.onClick.AddListener(Attach);
    }

    private void EnableTransform()
    {
        enabledButton = EnabledButton.TransformButton;
    }

    private void EnableCameraMovement()
    {
        enabledButton = EnabledButton.MoveButton;
    }

    private void EnableScaling()
    {
        enabledButton = EnabledButton.ScaleButton;
    }

    private void EnableRotation()
    {
        enabledButton = EnabledButton.RotateButton;
    }

    private void SpawnJoint()
    {
        GameObject newJoint = ObjectJoint.SpawnJoint();
        newJoint.AddComponent<ClickerTest>();
    }

    private void SpawnLink()
    {
        GameObject newLink = RobotLink.SpawnLink();
        newLink.AddComponent<ClickerTest>();
    }

    private void Attach()
    {
        GameObject selected = GameObject.Find("Plane").GetComponent<SelectorManagerScript>().selected;

        Debug.Log(selected.name + "hey");

        if (selected != null)
        {
            if (selected.GetComponent<RobotLink>() != null)
            {
                GameObject closestJoint = GetClosestJoint(selected);

                // If there is a joint, then we want to parent it to the closest joint.
                if (closestJoint != null)
                {
                    closestJoint.GetComponent<ObjectJoint>().ChildLink = selected;
                    selected.GetComponent<RobotLink>().ParentJoint = closestJoint;

                    GameObject ScaleProtect = new GameObject();
                    ScaleProtect.name = "ScaleProtect";
                    ScaleProtect.transform.parent = closestJoint.transform;
                    selected.transform.parent = ScaleProtect.transform;
                }
                else
                {
                    Debug.Log("You need a Joint to attach the Link to!");
                }
            }

            else if (selected.GetComponent<ObjectJoint>() != null)
            {

            }
        }
    }

    // Getting the closest joint (returns null if no ObjectJoints)
    private GameObject GetClosestJoint(GameObject link)
    {
        GameObject closest = null;
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        foreach (GameObject GO in allObjects)
        {
            if (GO.GetComponent<ObjectJoint>() != null && GO.activeInHierarchy)
            {
                if (closest == null)
                {
                    closest = GO;
                }
                else
                {
                    if (Vector3.Distance(link.transform.position, GO.transform.position) < Vector3.Distance(link.transform.position, closest.transform.position))
                    {
                        closest = GO;
                    }
                }
            }
        }

        return closest;
    }

}
