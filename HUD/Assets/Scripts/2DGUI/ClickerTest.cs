using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Attached to all of the spawned GameObjects.
/// Controls what happens to the object with click actions.
/// </summary>
public class ClickerTest : MonoBehaviour {

    GameObject selected;
    GameObject ButtonManager;
    bool _markersSpawned = false;

    public bool IsLocked = false; // Used to tell if the object is attached to something already or not.

    // For scaling and rotation
    float sizingFactor = 0.02f;
    float turnSpeed = 100.0f;
    private float startSize;
    private float startNum;
    private Vector3 mouseOrigin;

    GameObject arrowOne;
    GameObject arrowTwo;

    // Making this object "Selected"
    private void OnMouseDown()
    {
        GameObject selectorManager = GameObject.Find("Plane");
        selectorManager.GetComponent<SelectorManagerScript>().selected = gameObject;
    }

    // Every gameObject shouldn't be selected at first, assign ButtonManager.
    private void Start()
    {
        gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.gray);
        ButtonManager = GameObject.Find("ButtonManager");
    }

    // Check if it's selected. Also handles scaling.
    private void Update()
    {
        // Scale protection (why didnt this work before???)
        Vector3 original = transform.localScale;
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

        transform.localScale = original;
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

    // Moving + Rotating Objects.
    private void OnMouseDrag()
    {
        // moving object
        if (ButtonManager.GetComponent<ButtonManagerScript>().enabledButton == ButtonManagerScript.EnabledButton.TransformButton)
        {
            if (!IsLocked)
            {
                MoveObject(gameObject);
            }
            else
            {
                GameObject root = GetRootJoint(gameObject);
                MoveObject(root);
            }

        }

        // Rotating object
        // Can't rotate with one of the axis due to the limitations in 2D mouse -- add extra buttons?
        // Darn it 
        if (ButtonManager.GetComponent<ButtonManagerScript>().enabledButton == ButtonManagerScript.EnabledButton.RotateButton)
        {

            if (GameObject.Find("Plane").GetComponent<SelectorManagerScript>().selected == gameObject)
            {

                if (!_markersSpawned)
                {
                    // For the "arrows" we generate
                    _markersSpawned = true;

                    arrowOne = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                    arrowOne.transform.localScale = new Vector3(0.1f, 0.3f, 0.1f);
                    arrowOne.transform.parent = gameObject.transform;
                    Vector3 arrowOneTransform = new Vector3(0, 2f, 0);
                    arrowOne.transform.localPosition = arrowOneTransform;

                    //arrowTwo = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                    //arrowTwo.transform.localScale = new Vector3(0.1f, 0.3f, 0.1f);
                    //Quaternion arrowTwoRotation = Quaternion.Euler(0, 0, 90);
                    //arrowTwo.transform.parent = gameObject.transform;
                    //Vector3 arrowTwoTransform = new Vector3(1f, 0, 0);
                    //arrowTwo.transform.localPosition = arrowTwoTransform;
                    //arrowTwo.transform.localRotation = arrowTwoRotation;
                }




                if (Input.GetMouseButton(0))
                {
                    float rotSpeed = 5;
                    float rotX = Input.GetAxis("Mouse X") * rotSpeed * Mathf.Deg2Rad;
                    float rotY = Input.GetAxis("Mouse Y") * rotSpeed * Mathf.Deg2Rad;

                    transform.RotateAround(Vector3.up, -rotX);
                    transform.RotateAround(Vector3.right, rotY);
                }
            }
            else
            {
                Destroy(arrowOne);
                Destroy(arrowTwo);
                _markersSpawned = false;
            }
        }

        else
        {
            Destroy(arrowOne);
            Destroy(arrowTwo);
            _markersSpawned = false;
        }
    }

    // Method for moving the object 
    private static void MoveObject(GameObject movingObject)
    {
        Vector3 screenSpace = Camera.main.WorldToScreenPoint(movingObject.transform.position);
        Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenSpace.z);
        Vector3 objPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        movingObject.transform.position = objPosition;
    } 

    // Method for getting the rootJoint recursively (Used for moving Locked object)
    private static GameObject GetRootJoint(GameObject thisObject)
    {

        // If it's a joint
        if (thisObject.GetComponent<ObjectJoint>() != null)
        {
            if (thisObject.GetComponent<ObjectJoint>().ParentJoint != null)
            {
                return GetRootJoint(thisObject.GetComponent<ObjectJoint>().ParentJoint);
            }
            else
            {
                return thisObject;
            }
        }

        // If it's a link
        else if (thisObject.GetComponent<RobotLink>() != null)
        {
            if (thisObject.GetComponent<RobotLink>().ParentJoint != null)
            {
                return GetRootJoint(thisObject.GetComponent<RobotLink>().ParentJoint);
            }
            else
            {
                return thisObject;
            }
        }

        else
        {
            return null;
        }
    }
}
