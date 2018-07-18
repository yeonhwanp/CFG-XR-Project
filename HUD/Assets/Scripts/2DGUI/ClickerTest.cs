using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is used for all of the clicking interface.
/// </summary>
public class ClickerTest : MonoBehaviour {

    // Is there a way to dynamically set this?
    float distance = 5;

    GameObject selected;
    GameObject ButtonManager;

    // For scaling
    float sizingFactor = 0.02f;
    private float startSize;
    private float startNum;

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
        selected = GameObject.Find("Plane").GetComponent<SelectorManagerScript>().selected;

        // For the selection color
        if (GameObject.Find("Plane").GetComponent<SelectorManagerScript>().selected != gameObject)
        {
            gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.gray);
        }

        // For scaling
        if (ButtonManager.GetComponent<ButtonManagerScript>().enabledButton == ButtonManagerScript.EnabledButton.ScaleButton)
        {
            ChangeXScale();
            ChangeYScale();
            ChangeZScale();
        }
    }

    #region Scaling
    private void ChangeXScale()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z);
            startNum = mousePosition.x;
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
            startSize = selected.transform.localScale.x;
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 startScale = selected.transform.localScale;

            // Necessary so it doesnt rever to original.
            if (Input.mousePosition.x - startNum != 0)
            {
                startScale.x = System.Math.Abs(startSize + (Input.mousePosition.x - startNum) * sizingFactor);
                selected.transform.localScale = startScale;
            }

        }
    }

    private void ChangeYScale()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z);
            startNum = mousePosition.y;
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
            startSize = selected.transform.localScale.y;
        }

        if (Input.GetMouseButton(1))
        {
            Vector3 startScale = selected.transform.localScale;

            // Necessary so it doesnt rever to original.
            if (Input.mousePosition.y - startNum != 0)
            {
                startScale.y = System.Math.Abs(startSize + (Input.mousePosition.y - startNum) * sizingFactor);
                selected.transform.localScale = startScale;
            }

        }
    }

    // For now use in and out
    private void ChangeZScale()
    {
        if (Input.GetMouseButtonDown(2))
        {
            Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z);
            startNum = mousePosition.y;
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
            startSize = selected.transform.localScale.z;
        }

        if (Input.GetMouseButton(2))
        {
            Vector3 startScale = gameObject.transform.localScale;

            // Necessary so it doesnt rever to original.
            if (Input.mousePosition.y - startNum != 0)
            {
                startScale.z = System.Math.Abs(startSize + (Input.mousePosition.y - startNum) * sizingFactor);
                selected.transform.localScale = startScale;
            }

        }
    }
    #endregion

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
