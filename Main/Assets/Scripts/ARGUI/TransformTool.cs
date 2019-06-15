/******************************************************************************
 * Copyright (C) Leap Motion, Inc. 2011-2018.                                 *
 * Leap Motion proprietary and confidential.                                  *
 *                                                                            *
 * Use subject to the terms of the Leap Motion SDK Agreement available at     *
 * https://developer.leapmotion.com/sdk_agreement, or another agreement       *
 * between Leap Motion and you, your company or other organization.           *
 ******************************************************************************/

using Leap.Unity.Interaction;
using Leap.Unity.Query;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Leap.Unity.Examples
{

    [AddComponentMenu("")]
    public class TransformTool : MonoBehaviour
    {
        #region LeapMotion Stuff
        [Tooltip("The scene's InteractionManager, used to get InteractionControllers and "
               + "manage handle state.")]
        public InteractionManager interactionManager;

        [Tooltip("The target object to be moved by this tool.")]
        public Transform target;

        private Vector3 _moveBuffer = Vector3.zero;
        private Quaternion _rotateBuffer = Quaternion.identity;

        private HashSet<TransformHandle> _transformHandles = new HashSet<TransformHandle>();

        public enum ToolState { Idle, Translating, Rotating, Scaling }
        public ToolState _toolState = ToolState.Idle;
        private HashSet<TransformHandle> _activeHandles = new HashSet<TransformHandle>();

        private HashSet<TranslationAxis> _activeTranslationAxes = new HashSet<TranslationAxis>();
        #endregion

        // My Stuff: Hand stuff
        public enum ScaleAxis { x, y, z };
        public ScaleAxis ChosenAxis;
        public List<Hand> hands;
        public List<Transform> IndividualHandles;
        public List<Transform> IndividualRotationHandles;
        public TransformHandle HandleOne;
        public TransformHandle HandleTwo;
        public Vector3 InitialHandleOnePosition;
        public Vector3 InitialHandleTwoPosition;
        public Vector3 InitialHandOnePosition;
        public Vector3 InitialHandTwoPosition;
        public Vector3 EditScale;
        public Vector3 InitialHandleScale;
        public Vector3 InitialRotationHandleScale;
        public bool ToolConnected = false;
        public bool ObjectConnected = false;

        private Controller controller;
        private Vector3 initialScaling;
        private GameObject arrowOne;
        private List<string> rotateHandleNames;
        private float initialHandDistance;
        private float scaleDistance;
        private bool initialScaled = false;
        private bool _rotationMarkerSpawned;

        void Start()
        {
            // Initializing and setting values.
            controller = new Controller();
            IndividualHandles = new List<Transform>();
            rotateHandleNames = new List<string>() { "Y Rotator 0", "Y Rotator 1", "Y Rotator 2", "Y Rotator 3" };
            InitialHandleScale = new Vector3(0.8f, 0.8f, 0.8f);
            InitialRotationHandleScale = new Vector3(.5f, .5f, .5f);
            GetArrows();

            // LeapMotion properties
            if (interactionManager == null)
            {
                interactionManager = InteractionManager.instance;
            }
            foreach (var handle in GetComponentsInChildren<TransformHandle>())
            {
                _transformHandles.Add(handle);
            }
            PhysicsCallbacks.OnPostPhysics += onPostPhysics;
        }

        void Update()
        {
            // Hand properties.
            Frame frame = controller.Frame();
            hands = frame.Hands;

            // Enable or disable handles based on hand proximity and tool state.
            updateHandles();

            // Scaling method.
            ScaleObject();
        }

        // Assuming I'll have to edit code here
        #region Handle Movement / Rotation

        #region LeapMotion Stuff

        /// <summary>
        /// NotifyHandleMovement: Transform handles call this method to notify the tool that they were used
        ///                       to move the target object.
        /// </summary>
        public void NotifyHandleMovement(Vector3 deltaPosition)
        {
            _moveBuffer += deltaPosition;
        }

        /// <summary>
        /// NotifyHandleRotation: Transform handles call this method to notify the tool that they were used
        ///                       to rotate the target object.
        /// </summary>
        public void NotifyHandleRotation(Quaternion deltaRotation)
        {
            _rotateBuffer = deltaRotation * _rotateBuffer;
        }
        #endregion

        // Runs after FixedUpdate and PhysX.
        private void onPostPhysics()
        {

            ScalingSetup();

            switch (_toolState)
            {
                case ToolState.Rotating:
                    target.transform.rotation = _rotateBuffer * target.transform.rotation;
                    this.transform.rotation = target.transform.rotation;
                    break;
                case ToolState.Translating:
                    if (!ObjectConnected)
                    {
                        target.transform.position += _moveBuffer;
                        this.transform.position = target.transform.position;
                    }
                    else
                    {
                        GameObject rootJoint = GetRootJoint(target.gameObject);
                        rootJoint.transform.position += _moveBuffer;
                        transform.position = target.transform.position;
                    }
                    break;
                case ToolState.Scaling:
                    break;
            }

            // Spawn the rotation axis handles.
            if (GameObject.Find("Plane").GetComponent<ButtonStateManager>().SelectedObject == target.gameObject && !_rotationMarkerSpawned && target.GetComponent<ObjectJoint>() != null)
            {
                _rotationMarkerSpawned = true;

                arrowOne = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                arrowOne.transform.localScale = new Vector3(0.03f, 0.05f, 0.03f);
                arrowOne.transform.parent = target.transform;
                Vector3 arrowOneTransform = new Vector3(0, .3f, 0);
                Quaternion arrowOneRotation = Quaternion.Euler(0, 0, 0);
                arrowOne.transform.localPosition = arrowOneTransform;
                arrowOne.transform.localRotation = arrowOneRotation;

                arrowOne.GetComponent<Renderer>().material.SetColor("_Color", Color.magenta);
            }

            if (GameObject.Find("Plane").GetComponent<ButtonStateManager>().SelectedObject != target.gameObject)
            {
                _rotationMarkerSpawned = false;
                Destroy(arrowOne);
            }

            // Select this object
            if (_toolState != ToolState.Idle)
            {
                GameObject.Find("Plane").GetComponent<ButtonStateManager>().SelectedObject = target.gameObject;
            }

            // Deselect the object
            if (GameObject.Find("Plane").GetComponent<ButtonStateManager>().SelectedObject != target.gameObject)
            {
                Material selectedMaterial = target.GetComponent<Renderer>().material;
                selectedMaterial.SetColor("_Color", Color.white);
            }

            // Explicitly sync TransformHandles' rigidbodies with their transforms,
            // which moved along with this object's transform because they are children of it.
            foreach (var handle in _transformHandles)
            {
                handle.syncRigidbodyWithTransform();
            }

            // Reset movement and rotation buffers.
            _moveBuffer = Vector3.zero;
            _rotateBuffer = Quaternion.identity;
        }

        #endregion

        #region Handle Visibility

        private void updateHandles()
        {
            switch (_toolState)
            {
                case ToolState.Idle:
                    // Find the closest handle to any InteractionHand.
                    TransformHandle closestHandleToAnyHand = null;
                    float closestHandleDist = float.PositiveInfinity;
                    foreach (var intController in interactionManager.interactionControllers
                                                                    .Query()
                                                                    .Where(controller => controller.isTracked))
                    {
                        if (!intController.isPrimaryHovering) continue;
                        TransformHandle testHandle = intController.primaryHoveredObject
                                                                  .gameObject
                                                                  .GetComponent<TransformHandle>();

                        if (testHandle == null || !_transformHandles.Contains(testHandle)) continue;

                        float testDist = intController.primaryHoverDistance;
                        if (testDist < closestHandleDist)
                        {
                            closestHandleToAnyHand = testHandle;
                            closestHandleDist = testDist;
                        }
                    }

                    // While idle, only show the closest handle to any hand, hide other handles.
                    foreach (var handle in _transformHandles)
                    {
                        if (closestHandleToAnyHand != null && handle == closestHandleToAnyHand)
                        {
                            // Check if rotation should be restricted (aka is it connected?)
                            if (ObjectConnected)
                            {
                                // If it's a Link, then shouldn't be able to rotate it (but should be able to translate the parent using it)
                                if (target.GetComponent<ObjectJoint>() == null)
                                {
                                    if (closestHandleToAnyHand is TransformTranslationHandle)
                                    {
                                        handle.EnsureVisible();
                                    }
                                    else
                                    {
                                        handle.EnsureHidden();
                                    }
                                }
                                // If it's a Joint, then we want to be able to rotate around the designated axis.
                                else
                                {
                                    if (rotateHandleNames.Contains(closestHandleToAnyHand.name) || closestHandleToAnyHand is TransformTranslationHandle)
                                    {
                                        handle.EnsureVisible();
                                    }
                                    else
                                    {
                                        handle.EnsureHidden();
                                    }
                                }
                            }
                            // If rotation shouldn't be restricted, then just show any handle.
                            else
                            {
                                handle.EnsureVisible();
                            }
                        }
                        else
                        {
                            handle.EnsureHidden();
                        }
                    }
                    break;

                case ToolState.Translating:
                    // ***************** TODO: How about if we make others not visible and only axis visible? ************************
                    // While translating, show all translation handles except the other handle
                    // on the same axis, and hide rotation handles.
                    foreach (var handle in _transformHandles)
                    {
                        if (handle is TransformTranslationHandle)
                        {
                            var translateHandle = handle as TransformTranslationHandle;

                            if (!_activeHandles.Contains(translateHandle)
                                && _activeTranslationAxes.Contains(translateHandle.axis))
                            {
                                handle.EnsureVisible();
                            }
                            else
                            {
                                handle.EnsureVisible();
                            }
                        }
                        else
                        {
                            handle.EnsureHidden();
                        }
                    }
                    break;

                case ToolState.Rotating:
                    // While rotating, only show the active rotating handle.
                    foreach (var handle in _transformHandles)
                    {
                        if (_activeHandles.Contains(handle))
                        {
                            handle.EnsureVisible();
                        }
                        else
                        {
                            handle.EnsureHidden();
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// NotifyHandleActivated: Called by handles when they are grasped.
        /// </summary>
        public void NotifyHandleActivated(TransformHandle handle)
        {
            switch (_toolState)
            {
                case ToolState.Idle:
                    _activeHandles.Add(handle);

                    if (handle is TransformTranslationHandle)
                    {
                        _toolState = ToolState.Translating;
                        _activeTranslationAxes.Add(((TransformTranslationHandle)handle).axis);
                    }
                    else
                    {
                        _toolState = ToolState.Rotating;
                    }
                    break;

                case ToolState.Translating:
                    if (handle is TransformRotationHandle)
                    {
                        Debug.LogError("Error: Can't rotate a transform while it is already being "
                                     + "translated.");
                    }
                    else
                    {
                        _activeHandles.Add(handle);
                        _activeTranslationAxes.Add(((TransformTranslationHandle)handle).axis);
                    }
                    break;

                case ToolState.Rotating:
                    Debug.LogError("Error: Only one handle can be active while a transform is being "
                                 + "rotated.");
                    break;
            }
        }

        /// <summary>
        /// NotifyHandleDeactivated: Called by Handles when they are released.
        /// </summary>
        public void NotifyHandleDeactivated(TransformHandle handle)
        {
            if (handle is TransformTranslationHandle)
            {
                _activeTranslationAxes.Remove(((TransformTranslationHandle)handle).axis);
            }

            _activeHandles.Remove(handle);

            switch (_toolState)
            {
                case ToolState.Idle:
                    Debug.LogWarning("Warning: Handle was deactived while Tool was already idle.");
                    break;

                default:
                    if (_activeHandles.Count == 0)
                    {
                        _toolState = ToolState.Idle;
                    }
                    break;
            }
        }

        #endregion

        #region MyMethods

        // Get all of the translation and rotation handles individually.
        private void GetArrows()
        {
            foreach (Transform handleTypes in transform)
            {
                if (handleTypes.name == "Translate Handles")
                {
                    foreach (Transform handleAxis in handleTypes)
                    {
                        foreach (Transform individual in handleAxis)
                        {
                            IndividualHandles.Add(individual);
                        }
                    }
                }

                if (handleTypes.name == "Rotate Handles")
                {
                    foreach (Transform handleAxis in handleTypes)
                    {
                        foreach (Transform individual in handleAxis)
                        {
                            IndividualRotationHandles.Add(individual);
                        }
                    }
                }
            }
        }

        // Checks to see if object is being scaled + sets the intialScaling variable of object
        private void ScalingSetup()
        {
            if (_activeHandles.Count == 2)
            {
                float xCount = 0;
                float yCount = 0;
                float zCount = 0;

                foreach (TransformHandle handle in _activeHandles)
                {
                    if (handle.name == "Translate Pos X" || handle.name == "Translate Neg X")
                    {
                        xCount += 1;
                        if (xCount == 1)
                        {
                            HandleOne = handle;
                        }
                        if (xCount == 2)
                        {
                            HandleTwo = handle;
                            ChosenAxis = ScaleAxis.x;
                            _toolState = ToolState.Scaling;
                        }
                    }
                    if (handle.name == "Translate Pos Y" || handle.name == "Translate Neg Y")
                    {
                        yCount += 1;

                        if (yCount == 1)
                        {
                            HandleOne = handle;
                        }
                        if (yCount == 2)
                        {
                            HandleTwo = handle;
                            ChosenAxis = ScaleAxis.y;
                            _toolState = ToolState.Scaling;
                        }
                    }
                    if (handle.name == "Translate Pos Z" || handle.name == "Translate Neg Y")
                    {
                        zCount += 1;
                        if (zCount == 1)
                        {
                            HandleOne = handle;
                        }
                        if (zCount == 2)
                        {
                            HandleTwo = handle;
                            ChosenAxis = ScaleAxis.z;
                            _toolState = ToolState.Scaling;
                        }
                    }
                }

                if (xCount == 2 || yCount == 2 || zCount == 2)
                {
                    if (!initialScaled)
                    {
                        Vector3 leftPosition = new Vector3(hands[0].PalmPosition.x * 1 / 1000, hands[0].PalmPosition.y * 1 / 1000, hands[0].PalmPosition.z * 1 / 1000);
                        Vector3 rightPosition = new Vector3(hands[1].PalmPosition.x * 1 / 1000, hands[1].PalmPosition.y * 1 / 1000, hands[1].PalmPosition.z * 1 / 1000);

                        initialScaling = target.transform.localScale;
                        initialHandDistance = Vector3.Distance(leftPosition, rightPosition);
                        initialScaled = true;

                        InitialHandleOnePosition = HandleOne.transform.position;
                        InitialHandleTwoPosition = HandleTwo.transform.position;

                        InitialHandOnePosition = leftPosition;
                        InitialHandTwoPosition = rightPosition;
                    }
                }
            }

            // To reset values if we're done/not scaling.
            if (_activeHandles.Count != 2)
            {
                initialScaled = false;
            }
        }

        // Deals with actually scaling the object
        private void ScaleObject()
        {
            if (_toolState == ToolState.Scaling)
            {
                // Initial Stuff
                Vector3 leftPosition = new Vector3(hands[0].PalmPosition.x * 1 / 1000, hands[0].PalmPosition.y * 1 / 1000, hands[0].PalmPosition.z * 1 / 1000);
                Vector3 rightPosition = new Vector3(hands[1].PalmPosition.x * 1 / 1000, hands[1].PalmPosition.y * 1 / 1000, hands[1].PalmPosition.z * 1 / 1000);

                // Scaling numbers and vectors
                scaleDistance = Vector3.Distance(leftPosition, rightPosition);
                EditScale = target.transform.localScale;
                float scaleAmount = scaleDistance - initialHandDistance;

                // Do actual scaling
                switch (ChosenAxis)
                {
                    case ScaleAxis.x:
                        if (initialScaling.x + (scaleDistance - initialHandDistance) > 0)
                            EditScale.x = initialScaling.x + (scaleDistance - initialHandDistance);
                        break;
                    case ScaleAxis.y:
                        if (initialScaling.y + scaleDistance - initialHandDistance > 0)
                            EditScale.y = initialScaling.y + (scaleDistance - initialHandDistance);
                        break;
                    case ScaleAxis.z:
                        if (initialScaling.z + scaleDistance - initialHandDistance > 0)
                            EditScale.z = initialScaling.z + (scaleDistance - initialHandDistance);
                        break;
                }

                // Scale other things then set scale
                ScaleTranslationHandles();
                ScaleRotationHandles();

                // Get all children using method... ok I need to fix this thing now ok

                foreach (Transform childTransform in target.transform)
                {
                    if (childTransform.GetComponent<ObjectJoint>() != null || childTransform.GetComponent<RobotLink>() != null)
                    {
                        Transform parent = childTransform.parent;
                        childTransform.parent = null;
                        Vector3 scaleTmp = childTransform.localScale;
                        // Need to somehow get their initial scaling... o right that's why I made the dictionaries and stuff
                        scaleTmp.x = scaleTmp.x / EditScale.x;
                        scaleTmp.y = scaleTmp.y / EditScale.y;
                        scaleTmp.z = scaleTmp.z / EditScale.z;
                        childTransform.parent = parent;
                        childTransform.localScale = scaleTmp;
                    }
                }

                target.transform.localScale = EditScale;
            }
        }

        // Scales the translation handles appropriately while scaling
        private void ScaleTranslationHandles()
        {
            foreach (Transform individualHandle in IndividualHandles)
            {
                Transform parent = individualHandle.parent;
                individualHandle.parent = null;
                Vector3 scaleTmp = new Vector3(InitialHandleScale.x / EditScale.x, InitialHandleScale.y / EditScale.y, InitialHandleScale.z / EditScale.z);

                if (individualHandle.name == "Translate Pos X" || individualHandle.name == "Translate Neg X")
                {
                    scaleTmp.z = InitialHandleScale.z / EditScale.x;
                }

                if (individualHandle.name == "Translate Pos Y" || individualHandle.name == "Translate Neg Y")
                {
                    scaleTmp.z = InitialHandleScale.z / EditScale.y;
                }

                if (individualHandle.name == "Translate Pos Z" || individualHandle.name == "Translate Neg Z")
                {
                    scaleTmp.z = InitialHandleScale.z / EditScale.z;
                }

                individualHandle.parent = parent;
                individualHandle.localScale = scaleTmp;
            }
        }

        // Sets initial handle size so it doesnt randomly change when you start scaling
        private void SetTranslationHandleSize()
        {
            foreach (Transform individualHandle in IndividualHandles)
            {
                Transform parent = individualHandle.parent;
                individualHandle.parent = null;
                Vector3 scaleTmp = new Vector3(InitialHandleScale.x / target.transform.localScale.x, InitialHandleScale.y / target.transform.localScale.y, InitialHandleScale.z / target.transform.localScale.z);
                individualHandle.parent = parent;
                individualHandle.localScale = scaleTmp;
            }
        }

        // Scales the Rotation handles appropriately while scaling
        private void ScaleRotationHandles()
        {
            foreach (Transform rotationHandle in IndividualRotationHandles)
            {
                Vector3 scaleTmp = new Vector3(InitialRotationHandleScale.x / EditScale.x, InitialRotationHandleScale.y / EditScale.y, InitialRotationHandleScale.z / EditScale.z);
                rotationHandle.localScale = InitialRotationHandleScale;
            }
        }

        private static GameObject GetRootJoint(GameObject thisObject)
        {

            // If it's a joint
            if (thisObject.GetComponent<ObjectJoint>() != null)
            {
                if (thisObject.GetComponent<ObjectJoint>().ParentJoint != null)
                {
                    return GetRootJoint(thisObject.GetComponent<ObjectJoint>().ParentJoint);
                }
                else
                {
                    return thisObject;
                }
            }

            // If it's a link
            else if (thisObject.GetComponent<RobotLink>() != null)
            {
                if (thisObject.GetComponent<RobotLink>().ParentJoint != null)
                {
                    return GetRootJoint(thisObject.GetComponent<RobotLink>().ParentJoint);
                }
                else
                {
                    return thisObject;
                }
            }

            else
            {
                return null;
            }
        }

        #endregion
    }

}
