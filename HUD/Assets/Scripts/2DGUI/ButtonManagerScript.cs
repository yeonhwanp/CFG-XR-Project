using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManagerScript : MonoBehaviour {

    public Button JointButton;
    public Button LinkButton;

    private void Start()
    {
        JointButton.onClick.AddListener(SpawnJoint);
        LinkButton.onClick.AddListener(SpawnLink);
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
