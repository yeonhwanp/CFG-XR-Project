using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap.Unity;
using Leap.Unity.Interaction;
using System.Windows.Forms;
using System.Globalization;

[AddComponentMenu("")]
[RequireComponent(typeof(InteractionBehaviour))]
public class RobotButtonManager : MonoBehaviour {

    [Tooltip("If enabled, the object will lerp to its hoverColor when a hand is nearby.")]
    public bool useHover = true;

    [Header("InteractionBehaviour Colors")]
    public Color defaultColor = Color.Lerp(Color.black, Color.white, 0.1F);
    public Color suspendedColor = Color.red;
    public Color hoverColor = Color.Lerp(Color.black, Color.white, 0.7F);
    public Color primaryHoverColor = Color.Lerp(Color.black, Color.white, 0.8F);

    [Header("InteractionButton Colors")]
    [Tooltip("This color only applies if the object is an InteractionButton or InteractionSlider.")]
    public Color pressedColor = Color.white;

    // -------------------------------------------------- My Code ------------------------------------------------ //
    [Tooltip("Choose between Link and Joint button.")]
    public ButtonType ButtonChoice;

    public enum ButtonType { SpawnJoint, SpawnLink, Attach };

    private Material _material;
    private InteractionBehaviour _intObj;

    private int _defaultEntry = 110;
    private int _defaultLabel = 10;

    private bool _isPushed = false; // For Cooldown

    // Just initialization here
    void Start()
    {
        _intObj = GetComponent<InteractionBehaviour>();

        Renderer renderer = GetComponent<Renderer>();
        if (renderer == null)
        {
            renderer = GetComponentInChildren<Renderer>();
        }
        if (renderer != null)
        {
            _material = renderer.material;
        }
    }

    void Update()
    {
        if (_material != null)
        {

            // The target color for the Interaction object will be determined by various simple state checks.
            Color targetColor = defaultColor;

            if (_intObj.isHovered && useHover)
            {
                float glow = _intObj.closestHoveringControllerDistance.Map(0F, 0.2F, 1F, 0.0F);
                targetColor = Color.Lerp(defaultColor, hoverColor, glow);
            }

            // On press
            if (_intObj is InteractionButton && (_intObj as InteractionButton).isPressed)
            {
                if (!_isPushed)
                {
                    _isPushed = true;

                    switch (ButtonChoice)
                    {
                        case ButtonType.SpawnLink:
                            GameObject newLink = RobotLink.SpawnLink();
                            newLink.AddComponent<ClickerTest>();
                            newLink.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
                            newLink.transform.position = new Vector3(.008f, -0.171f, 0.633f);
                            break;
                        case ButtonType.SpawnJoint:
                            GameObject newJoint = ObjectJoint.SpawnJoint();
                            newJoint.AddComponent<ClickerTest>();
                            newJoint.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
                            break;
                        case ButtonType.Attach:
                            Debug.Log("Attach function not implemented yet!");
                            break;
                    }

                    targetColor = pressedColor;


                    //if (ButtonChoice == ButtonType.SpawnLink) 
                    //{
                    //    GameObject newLink = RobotLink.SpawnLink();
                    //    newLink.AddComponent<ClickerTest>();
                    //    newLink.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
                    //    newLink.transform.position = new Vector3(.008f, -0.171f, 0.633f);
                    //}
                    //if (ButtonChoice == ButtonType.SpawnJoint) 
                    //{
                    //    GameObject newJoint = ObjectJoint.SpawnJoint();
                    //    newJoint.AddComponent<ClickerTest>();
                    //    newJoint.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
                    //    newJoint.transform.position = new Vector3(.008f, -0.171f, 0.633f);
                    //}
                    //targetColor = pressedColor;
                    //if (ButtonChoice == ButtonType.Select)
                    //{
                    //    ButtonStateManager ButtonStateManager = GameObject.Find("Plane").GetComponent<ButtonStateManager>();
                    //    ButtonStateManager.Selected = ButtonStateManager.ButtonState.Select;
                    //}
                    //if (ButtonChoice == ButtonType.Deselect)
                    //{
                    //    ButtonStateManager ButtonStateManager = GameObject.Find("Plane").GetComponent<ButtonStateManager>();
                    //    ButtonStateManager.Selected = ButtonStateManager.ButtonState.None;
                    //}
                }
            }

            // For the cooldown
            if (_intObj is InteractionButton && !(_intObj as InteractionButton).isPressed)
            {
                _isPushed = false;
            }

            // Lerp actual material color to the target color.
            _material.color = Color.Lerp(_material.color, targetColor, 30F * Time.deltaTime);
        }
    }
}