/******************************************************************************
 * Copyright (C) Leap Motion, Inc. 2011-2018.                                 *
 * Leap Motion proprietary and confidential.                                  *
 *                                                                            *
 * Use subject to the terms of the Leap Motion SDK Agreement available at     *
 * https://developer.leapmotion.com/sdk_agreement, or another agreement       *
 * between Leap Motion and you, your company or other organization.           *
 ******************************************************************************/

using UnityEngine;

namespace Leap.Unity {

  public static class TransformUtil {

    public static Quaternion TransformRotation(this Transform transform, Quaternion rotation) {
      return transform.rotation * rotation;
    }

    public static Quaternion InverseTransformRotation(this Transform transform, Quaternion rotation) {
      return Quaternion.Inverse(transform.rotation) * rotation;
    }

    public static void SetLocalX(this Transform transform, float localX) {
      transform.setSimpleAxis(localX, 0);
    }

    public static void SetLocalY(this Transform transform, float localY) {
      transform.setSimpleAxis(localY, 1);
    }

    public static void SetLocalZ(this Transform transform, float localZ) {
      transform.setSimpleAxis(localZ, 2);
    }

    private static void setSimpleAxis(this Transform transform, float value, int axis) {
      Vector3 local = transform.localPosition;
      local[axis] = value;
      transform.localPosition = local;
    }
  }

}
