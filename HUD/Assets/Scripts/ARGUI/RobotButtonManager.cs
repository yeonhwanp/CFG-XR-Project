﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap.Unity;
using Leap.Unity.Interaction;
using System.Windows.Forms;
using System.Globalization;

namespace Leap.Unity.Examples
{
    [AddComponentMenu("")]
    [RequireComponent(typeof(InteractionBehaviour))]
    public class RobotButtonManager : MonoBehaviour
    {

        #region LeapMotion Stuff
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

        private int _defaultEntry = 110;
        private int _defaultLabel = 10;
        private Material _material;
        private InteractionBehaviour _intObj;
        #endregion

        // -------------------------------------------------- My Code ------------------------------------------------ //
        public enum ButtonType { SpawnJoint, SpawnLink, Attach };
        public ButtonType ButtonChoice;
        public GameObject MovingTool;
        private bool _isPushed = false; // For Cooldown
        private Vector3 toolTransformSize;

        // Just initialization here
        void Start()
        {
            toolTransformSize = new Vector3(5.388435f, 5.388435f, 5.388435f);

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
                                GameObject Linktool = SpawnTransformTool(newLink);

                                newLink.transform.localScale = new Vector3(.1855826f, 0.1855826f, 0.1855826f);
                                newLink.transform.position = new Vector3(0.02505559f, -0.1699998f, 0.597624f);

                                break;
                            case ButtonType.SpawnJoint:
                                GameObject newJoint = ObjectJoint.SpawnJoint();
                                GameObject Jointtool = SpawnTransformTool(newJoint);

                                newJoint.transform.localScale = new Vector3(.1855826f, 0.1855826f, 0.1855826f);
                                newJoint.transform.position = new Vector3(0.02505559f, -0.1699998f, 0.597624f);

                                break;
                            case ButtonType.Attach:
                                AttachObjects();
                                break;
                        }

                        targetColor = pressedColor;
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

        // Attach the transform tool to the object
        private GameObject SpawnTransformTool(GameObject parent)
        {
            GameObject tool = Instantiate(MovingTool);

            tool.transform.parent = parent.transform;
            tool.GetComponent<TransformTool>().target = parent.transform;
            tool.transform.localScale = toolTransformSize;
            tool.transform.localPosition = Vector3.zero;
            tool.GetComponent<TransformTool>().ToolConnected = true;

            return tool;
        }

        private void AttachObjects()
        {
            GameObject selected = GameObject.Find("Plane").GetComponent<ButtonStateManager>().SelectedObject;

            if (selected != null)
            {
                if (selected.GetComponent<RobotLink>() != null)
                {
                    GameObject closestJoint = GetClosestJoint(selected);

                    if (closestJoint != null)
                    {
                        closestJoint.GetComponent<ObjectJoint>().ChildLink = selected;
                        selected.GetComponent<RobotLink>().ParentJoint = closestJoint;

                        selected.transform.parent = closestJoint.transform;
                        foreach(Transform child in selected.transform)
                        {
                            if (child.name == "Transform Tool(Clone)")
                            {
                                child.GetComponent<TransformTool>().ObjectConnected = true;
                            }
                        }
                    }

                    else
                    {
                        Debug.Log("You need a Joint to attach the Link to!");
                    }
                }

                else if (selected.GetComponent<ObjectJoint>() != null)
                {
                    GameObject closestLink = GetClosestLink(selected);

                    if (closestLink != null)
                    {
                        ObjectJoint thisJoint = selected.GetComponent<ObjectJoint>();

                        thisJoint.ParentJoint = closestLink.GetComponent<RobotLink>().ParentJoint;
                        thisJoint.ParentJoint.GetComponent<ObjectJoint>().ChildJoints.Add(selected);
                        thisJoint.ParentLink = closestLink;

                        selected.transform.parent = closestLink.transform;
                    }

                    else
                    {
                        Debug.Log("You need a Link to attach the Joint to!");
                    }
                }
            }
        }

        private GameObject GetClosestJoint(GameObject link)
        {
            GameObject closest = null;
            GameObject[] allObjects = FindObjectsOfType<GameObject>();

            foreach(GameObject GO in allObjects)
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

        private GameObject GetClosestLink(GameObject joint)
        {
            GameObject closest = null;
            GameObject[] allObjects = FindObjectsOfType<GameObject>();
            foreach (GameObject GO in allObjects)
            {
                if (GO.GetComponent<RobotLink>() != null)
                {
                    if (closest == null)
                    {
                        closest = GO;
                    }
                    else
                    {
                        if (Vector3.Distance(joint.transform.position, GO.transform.position) < Vector3.Distance(joint.transform.position, closest.transform.position))
                        {
                            closest = GO;
                        }
                    }
                }
            }

            return closest;
        }
    }
}

