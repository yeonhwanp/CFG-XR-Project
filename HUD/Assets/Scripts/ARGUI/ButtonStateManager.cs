using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
