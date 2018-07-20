using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used to manage "selecting" an object.
/// </summary>
public class SelectorManagerScript : MonoBehaviour {

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
            //selected = null;
        }
    }
}
