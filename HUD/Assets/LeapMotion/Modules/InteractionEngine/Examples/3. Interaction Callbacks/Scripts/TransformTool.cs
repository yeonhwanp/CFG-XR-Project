/******************************************************************************
 * Copyright (C) Leap Motion, Inc. 2011-2018.                                 *
 * Leap Motion proprietary and confidential.                                  *
 *                                                                            *
 * Use subject to the terms of the Leap Motion SDK Agreement available at     *
 * https://developer.leapmotion.com/sdk_agreement, or another agreement       *
 * between Leap Motion and you, your company or other organization.           *
 ******************************************************************************/

﻿using Leap.Unity.Interaction;
using Leap.Unity.Query;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Leap.Unity.Examples {

  [AddComponentMenu("")]
  public class TransformTool : MonoBehaviour {

    [Tooltip("The scene's InteractionManager, used to get InteractionControllers and "
           + "manage handle state.")]
    public InteractionManager interactionManager;

    [Tooltip("The target object to be moved by this tool.")]
    public Transform target;

    private Vector3    _moveBuffer = Vector3.zero;
    private Quaternion _rotateBuffer = Quaternion.identity;

    private HashSet<TransformHandle> _transformHandles = new HashSet<TransformHandle>();

    private enum ToolState { Idle, Translating, Rotating, Scaling }
    private ToolState _toolState = ToolState.Idle;
    private HashSet<TransformHandle> _activeHandles = new HashSet<TransformHandle>();

    private HashSet<TranslationAxis> _activeTranslationAxes = new HashSet<TranslationAxis>();

    void Start() {
      if (interactionManager == null) {
        interactionManager = InteractionManager.instance;
      }

      foreach (var handle in GetComponentsInChildren<TransformHandle>()) {
        _transformHandles.Add(handle);
      }

      // PhysicsCallbacks is useful for creating explicit pre-physics and post-physics
      // behaviour.
      PhysicsCallbacks.OnPostPhysics += onPostPhysics; // Delegate (omg I still have to learn how these work lol)
    }

    void Update() {
      // Enable or disable handles based on hand proximity and tool state.
      updateHandles();
    }

    // Assuming I'll have to edit code here
    #region Handle Movement / Rotation

    /// <summary>
    /// Transform handles call this method to notify the tool that they were used
    /// to move the target object.
    /// </summary>
    public void NotifyHandleMovement(Vector3 deltaPosition) {
      _moveBuffer += deltaPosition;
    }

    /// <summary>
    /// Transform handles call this method to notify the tool that they were used
    /// to rotate the target object.
    /// </summary>
    public void NotifyHandleRotation(Quaternion deltaRotation) {
      _rotateBuffer = deltaRotation * _rotateBuffer;
    }

    private void onPostPhysics() {
            // Hooked up via PhysicsCallbacks in Start(), this method will run after
            // FixedUpdate and after PhysX has run. We take the opportunity to immediately
            // manipulate the target object's and this object's transforms using the
            // accumulated information about movement and rotation from the Transform Handles.

            // Apply accumulated movement and rotation to target object.

            // Can we do the stuff here? Aka conditionals?

            if (_activeHandles.Count == 2)
            {
                float xCount = 0;
                float yCount = 0;
                float zCount = 0;
                // If the two opposite sides are in, then scale.
                foreach (TransformHandle handle in _activeHandles)
                {
                    if (handle.name == "Translate Pos X" || handle.name == "Translate Neg X")
                    {
                        xCount += 1;
                    }
                    if (handle.name == "Translate Pos Y" || handle.name == "Translate Neg Y")
                    {
                        yCount += 1;
                    }
                    if (handle.name == "Translate Pos Z" || handle.name == "Translate Neg Y")
                    {
                        zCount += 1;
                    }
                }

                // Do scaling + enum here
                if (xCount == 2)
                {
                    _toolState = ToolState.Scaling;
                }
                if (yCount == 2)
                {
                    _toolState = ToolState.Scaling;
                }
                if (zCount == 2)
                {
                    _toolState = ToolState.Scaling;
                }
            }

            switch (_toolState)
            {
                case ToolState.Rotating:
                    target.transform.rotation = _rotateBuffer * target.transform.rotation;
                    this.transform.rotation = target.transform.rotation;
                    break;
                case ToolState.Translating:
                    target.transform.position += _moveBuffer; // Just adding stuff, not a delegate. Lit.
                    this.transform.position = target.transform.position;
                    break;
            }

              // Explicitly sync TransformHandles' rigidbodies with their transforms,
              // which moved along with this object's transform because they are children of it.
              foreach (var handle in _transformHandles) {
                handle.syncRigidbodyWithTransform();
              }

              // Reset movement and rotation buffers.
              _moveBuffer = Vector3.zero;
              _rotateBuffer = Quaternion.identity;

            foreach (TransformHandle handle in _activeHandles)
            {
                Debug.Log(handle.name);
            }
            // Problem: Doesn't seem to but too accurate... But at the same time we kind of need a mount atm LOL
            }

    #endregion

    // No need to edit this (I think?)
    #region Handle Visibility
    
    // Literally just handles visibility. Maybe we should add a scaling enum? Yea... So it doesnt move. I guess?
    private void updateHandles() {
      switch (_toolState) {
        case ToolState.Idle:
          // Find the closest handle to any InteractionHand.
          TransformHandle closestHandleToAnyHand = null;
          float closestHandleDist = float.PositiveInfinity;
          foreach (var intController in interactionManager.interactionControllers
                                                          .Query()
                                                          .Where(controller => controller.isTracked)) {
            if (!intController.isPrimaryHovering) continue;
            TransformHandle testHandle = intController.primaryHoveredObject
                                                      .gameObject
                                                      .GetComponent<TransformHandle>();

            if (testHandle == null || !_transformHandles.Contains(testHandle)) continue;

            float testDist = intController.primaryHoverDistance;
            if (testDist < closestHandleDist) {
              closestHandleToAnyHand = testHandle;
              closestHandleDist = testDist;
            }
          }

          // While idle, only show the closest handle to any hand, hide other handles.
          foreach (var handle in _transformHandles) {
            if (closestHandleToAnyHand != null && handle == closestHandleToAnyHand) {
              handle.EnsureVisible();
            }
            else {
              handle.EnsureHidden();
            }
          }
          break;

        case ToolState.Translating: 
          // ***************** How about if we make others not visible and only axis visible? ************************
          // While translating, show all translation handles except the other handle
          // on the same axis, and hide rotation handles.
          foreach (var handle in _transformHandles) {
            if (handle is TransformTranslationHandle) {
              var translateHandle = handle as TransformTranslationHandle;

              if (!_activeHandles.Contains(translateHandle)
                  && _activeTranslationAxes.Contains(translateHandle.axis)) {
                handle.EnsureVisible();
              }
              else {
                handle.EnsureVisible();
              }
            }
            else {
              handle.EnsureHidden();
            }
          }
          break;

        case ToolState.Rotating:
          // While rotating, only show the active rotating handle.
          foreach (var handle in _transformHandles) {
            if (_activeHandles.Contains(handle)) {
              handle.EnsureVisible();
            }
            else {
              handle.EnsureHidden();
            }
          }
          break;
      }
    }

    /// <summary>
    /// Called by handles when they are grasped.
    /// </summary>
    /// <param name="handle"></param>
    public void NotifyHandleActivated(TransformHandle handle) {
      switch (_toolState) {
        case ToolState.Idle:
          _activeHandles.Add(handle);

          if (handle is TransformTranslationHandle) {
            _toolState = ToolState.Translating;
            _activeTranslationAxes.Add(((TransformTranslationHandle)handle).axis);
          }
          else {
            _toolState = ToolState.Rotating;
          }
          break;

        case ToolState.Translating:
          if (handle is TransformRotationHandle) {
            Debug.LogError("Error: Can't rotate a transform while it is already being "
                         + "translated.");
          }
          else {
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
    /// Called by Handles when they are released.
    /// </summary>
    public void NotifyHandleDeactivated(TransformHandle handle) {
      if (handle is TransformTranslationHandle) {
        _activeTranslationAxes.Remove(((TransformTranslationHandle)handle).axis);
      }

      _activeHandles.Remove(handle);

      switch (_toolState) {
        case ToolState.Idle:
          Debug.LogWarning("Warning: Handle was deactived while Tool was already idle.");
          break;

        default:
          if (_activeHandles.Count == 0) {
            _toolState = ToolState.Idle;
          }
          break;
      }
    }

    #endregion

  }

}
