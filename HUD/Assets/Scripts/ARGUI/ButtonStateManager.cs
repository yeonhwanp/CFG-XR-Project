using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonStateManager : MonoBehaviour {
    public enum ButtonState { Select, None };
    public ButtonState Selected;
}
