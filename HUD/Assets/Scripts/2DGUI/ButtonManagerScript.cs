using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManagerScript : MonoBehaviour {

    public Button JointButton;
    public Button LinkButton;
    public Button MoveButton;
    public Button TransformButton;
    public Button ScaleButton;
    public Button RotateButton;

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

}
