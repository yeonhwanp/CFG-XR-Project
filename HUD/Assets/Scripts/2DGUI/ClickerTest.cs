using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is used for all of the clicking interface.
/// </summary>
public class ClickerTest : MonoBehaviour {

    float distance = 3;
    GameObject ButtonManager;

    // TEST
    float sizingFactor = 0.02f;
    private float startSize;
    private float startX;

    private void OnMouseDown()
    {
        GameObject selectorManager = GameObject.Find("Plane");
        selectorManager.GetComponent<SelectorManagerScript>().selected = gameObject;
    }

    private void Start()
    {
        gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.gray);
        ButtonManager = GameObject.Find("ButtonManager");
    }

    private void Update()
    {
        // For the selection color
        if (GameObject.Find("Plane").GetComponent<SelectorManagerScript>().selected != gameObject)
        {
            gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.gray);
        }

        //for resizing
        if (ButtonManager.GetComponent<ButtonManagerScript>().enabledButton == ButtonManagerScript.EnabledButton.ScaleButton)
        {
            // Setting the values on buttondown
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z);
                startX = position.x;
                position = Camera.main.ScreenToWorldPoint(position);
                startSize = gameObject.transform.localScale.z;
            }

            // Doing the actual stuff
            if (Input.GetMouseButton(0))
            {
                Vector3 size = gameObject.transform.localScale;
                size.x = startSize + (Input.mousePosition.x - startX) * sizingFactor;
                gameObject.transform.localScale = size;
            }
        }


        //if (ButtonManager.GetComponent<ButtonManagerScript>().enabledButton == ButtonManagerScript.EnabledButton.ScaleButton)
        //{
        //    float startSize;

        //    if (Input.GetMouseButtonDown(0))
        //    {
        //        Vector3 mouseOriginal = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z);
        //        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(mouseOriginal);
        //        startSize = gameObject.transform.localScale.x;
        //    }

        //    if (Input.GetMouseButton(0))
        //    {
        //        Vector3 originalSize = gameObject.transform.localScale;
        //        float newX = originalSize.x + (Input.mousePosition.x - startX) * sizingFactor;
        //        Vector3 newSize = new Vector3(newX, originalSize.y, originalSize.z);
        //        transform.localScale = newSize;
        //    }

        //}
    }

    private void OnMouseDrag()
    {
        // moving object
        if (ButtonManager.GetComponent<ButtonManagerScript>().enabledButton == ButtonManagerScript.EnabledButton.TransformButton)
        {
            Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance);
            Vector3 objPosition = Camera.main.ScreenToWorldPoint(mousePosition);
            gameObject.transform.position = objPosition;
        }
    }
}
