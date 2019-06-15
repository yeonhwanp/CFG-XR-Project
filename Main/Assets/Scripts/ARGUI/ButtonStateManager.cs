using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ButtonStateManager: A class to keep track of and manage button states.
///
/// Attributes:
///     Selected: The currently selected button state.
///		SelectedObject: The currently selected GameObject.
/// </summary>
public class ButtonStateManager : MonoBehaviour {
    public enum ButtonState { Select, None };
    public ButtonState Selected;
    public GameObject SelectedObject;

    private void FixedUpdate()
    {
        Material selectedMaterial = SelectedObject.GetComponent<Renderer>().material;
        selectedMaterial.SetColor("_Color", Color.black);
    }
}
