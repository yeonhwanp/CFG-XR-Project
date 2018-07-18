using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectorManagerScript : MonoBehaviour {

    // Should be the one that's selected. Oh wow this works perfectly, don't have to search through.
    // Can just say "ok this is selected change the material"
    // Cool
    public GameObject selected;

    private void Update()
    {
        if (selected != gameObject && selected != null) 
        {
            Material theMaterial = selected.GetComponent<Renderer>().material;
            theMaterial.SetColor("_Color", Color.black);
        }

    }

    private void OnMouseDown()
    {
        if (selected != gameObject)
        {
            Material theMaterial = selected.GetComponent<Renderer>().material;
            theMaterial.SetColor("_Color", Color.gray);
            selected = null;
        }
    }
}
