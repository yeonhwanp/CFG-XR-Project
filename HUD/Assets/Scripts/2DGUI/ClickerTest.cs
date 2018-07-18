using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is used for all of the clicking interface.
/// </summary>
public class ClickerTest : MonoBehaviour {

    float distance = 3;

    private void OnMouseDown()
    {
        GameObject selectorManager = GameObject.Find("Plane");
        selectorManager.GetComponent<SelectorManagerScript>().selected = gameObject;
    }

    private void Start()
    {
        gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.gray);
    }

    private void Update()
    {
        if (GameObject.Find("Plane").GetComponent<SelectorManagerScript>().selected != gameObject)
        {
            gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.gray);
        }
    }

    private void OnMouseDrag()
    {
        Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance);
        Vector3 objPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        gameObject.transform.position = objPosition;
    }
}
