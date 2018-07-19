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
    float turnSpeed = 100.0f;
    private float startSize;
    private float startNum;
    private Vector3 mouseOrigin;

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
        mouseOrigin = Input.mousePosition;

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
            Vector3 newPosition = new Vector3(objPosition.x, objPosition.y, gameObject.transform.position.z);
            gameObject.transform.position = newPosition;

        }

        // Rotating object
        // Can't rotate with one of the axis due to the limitations in 2D mouse -- add extra buttons?
        if (ButtonManager.GetComponent<ButtonManagerScript>().enabledButton == ButtonManagerScript.EnabledButton.RotateButton)
        {
            if (Input.GetMouseButton(0))
            {
                float rotSpeed = 5;
                float rotX = Input.GetAxis("Mouse X") * rotSpeed * Mathf.Deg2Rad;
                float rotY = Input.GetAxis("Mouse Y") * rotSpeed * Mathf.Deg2Rad;

                transform.RotateAround(Vector3.up, -rotX);
                transform.RotateAround(Vector3.right, rotY);
            }

            // Need to make this work for full rotation capabilities.
            else if (Input.GetMouseButton(1))
            {
                Debug.Log("hi?");
                float rotSpeed = 5;
                float rotX = Input.GetAxis("Mouse X") * rotSpeed * Mathf.Deg2Rad;
                transform.RotateAround(Vector3.forward, rotX);
            }

        }
    }
}
