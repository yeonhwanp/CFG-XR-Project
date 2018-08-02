using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Attached to all of the spawned GameObjects.
/// Controls what happens to the object with click actions.
/// </summary>
public class ClickerTest : MonoBehaviour {

    #region Variables
    // Shortcuts
    SelectorManagerScript SelectorManager;
    ButtonManagerScript ButtonManager;

    // Booleans
    bool _markersSpawned = false;
    public bool _isScaling = false;

    // For when things are connected
    public bool IsLocked = false; 
    public bool IsRotationLocked = false;

    // For scaling
    private float sizingFactor = 0.02f;
    private float startSize;
    private float startNum;
    private Vector3 mouseOrigin;

    // Rotation arrows
    GameObject arrowOne;
    GameObject arrowTwo;

    // Testing Scaling
    Vector3 startMouse;
    Vector3 startSizes;
    Vector3 startPositions;
    float closestFloat = 0;
    public bool _Snapped = false;
    public closestAxis nearest;
    public enum closestAxis { xRight, xLeft, yUp, yDown};
    #endregion

    #region UnityMethods
    // Set color, initalize shortcuts
    private void Start()
    {
        gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.gray);
        ButtonManager = GameObject.Find("ButtonManager").GetComponent<ButtonManagerScript>();
        SelectorManager = GameObject.Find("Plane").GetComponent<SelectorManagerScript>();
    }

    private void Update()
    {
        Vector3 original = transform.localScale; // Scale Protection
        mouseOrigin = Input.mousePosition;

        // Switch to defualt color if not selected
        if (SelectorManager.selected != gameObject)
        {
            gameObject.GetComponent<Renderer>().material.SetColor("_Color", Color.gray);
        }

        // For scaling
        if (ButtonManager.enabledButton == ButtonManagerScript.EnabledButton.ScaleButton && SelectorManager.selected == gameObject)
        {
            _isScaling = true;
            ChangeScale();
        }

        else 
        {
            _isScaling = false;
        }

        // For rotation
        if (SelectorManager.selected == gameObject && ButtonManager.enabledButton == ButtonManagerScript.EnabledButton.RotateButton)
        {
            // Managing Markers.
            if (!_markersSpawned)
            {
                _markersSpawned = true;

                arrowOne = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                arrowOne.transform.localScale = new Vector3(0.1f, 0.3f, 0.1f);
                arrowOne.transform.parent = gameObject.transform;
                Vector3 arrowOneTransform = new Vector3(0, 1f, 0);
                Quaternion arrowOneRotation = Quaternion.Euler(0, 0, 0);
                arrowOne.transform.localPosition = arrowOneTransform;
                arrowOne.transform.localRotation = arrowOneRotation;

                if (!IsRotationLocked && gameObject.GetComponent<ObjectJoint>() != null)
                {
                    arrowOne.GetComponent<Renderer>().material.SetColor("_Color", Color.green);
                }

                if (!IsRotationLocked)
                {
                    arrowTwo = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                    arrowTwo.transform.localScale = new Vector3(0.1f, 0.3f, 0.1f);
                    Quaternion arrowTwoRotation = Quaternion.Euler(0, 0, 90);
                    arrowTwo.transform.parent = gameObject.transform;
                    Vector3 arrowTwoTransform = new Vector3(1f, 0, 0);
                    arrowTwo.transform.localPosition = arrowTwoTransform;
                    arrowTwo.transform.localRotation = arrowTwoRotation;

                    Destroy(arrowTwo.GetComponent<Rigidbody>());
                    Destroy(arrowTwo.GetComponent<CapsuleCollider>());
                }
                Destroy(arrowOne.GetComponent<Rigidbody>());
                Destroy(arrowOne.GetComponent<CapsuleCollider>());
            }

            // Actual rotation managed here
            if (Input.GetMouseButton(0))
            {
                float rotSpeed = 5;
                float rotX = Input.GetAxis("Mouse X") * rotSpeed * Mathf.Deg2Rad;
                float rotY = Input.GetAxis("Mouse Y") * rotSpeed * Mathf.Deg2Rad;

                // How rotation should be if it's a joint
                if (gameObject.GetComponent<ObjectJoint>() != null)
                {
                    transform.RotateAround(Vector3.up, -rotX);
                    if (!IsRotationLocked)
                    {
                        transform.RotateAround(Vector3.right, rotY);
                    }
                }

                // How rotation should be if it's a link
                else
                {
                    if (!IsLocked)
                    {
                        transform.RotateAround(Vector3.up, -rotX);
                        transform.RotateAround(Vector3.right, rotY);
                    }
                    else
                    {
                        GameObject parent = gameObject.GetComponent<RobotLink>().ParentJoint;
                        parent.transform.RotateAround(Vector3.up, -rotX);
                        if (!parent.GetComponent<ClickerTest>().IsRotationLocked)
                        {
                            parent.transform.RotateAround(Vector3.right, rotY);
                        }
                    }
                }

            }
        }

        else
        {
            Destroy(arrowOne);
            Destroy(arrowTwo);
            _markersSpawned = false;
        }

        // Don't change scale if you don't need to.
        if (!_isScaling)
            transform.localScale = original;
    }

    // Selecting the object
    private void OnMouseDown()
    {
        SelectorManager.selected = gameObject;
    }

    // Moving objects. 
    private void OnMouseDrag()
    {
        if (ButtonManager.enabledButton == ButtonManagerScript.EnabledButton.TransformButton)
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
    }

    #endregion

    // ---------------------------------------------- Start of other methods ---------------------------------------------- //

    #region OtherMethods
    #region Scaling

    // This method gets the starting position/sizes then applys other methods to scale the gameObject.
    private void ChangeScale()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z);
            startMouse = new Vector3(mousePosition.x, mousePosition.y, mousePosition.z); // Where the mouse starts out
            startSizes = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z); // Scaling before changes
            startPositions = new Vector3(transform.position.x - transform.localScale.x / 2.0f, transform.position.y - transform.localScale.y / 2.0f); // Use for moving
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 startScale = gameObject.transform.localScale;

            if (Input.mousePosition.x - startMouse.x != 0 || Input.mousePosition.y - startMouse.y != 0 || Input.mousePosition.z - startMouse.z != 0)
            {
                if (!_Snapped) // To make sure that we're not trying to randomly change the scaling. Once it's snapped, it shouldn't be unsnapped.
                {
                    closestAxis closestVector = GetClosestVector(startMouse);
                    ScaleObject(closestVector);
                    _Snapped = true;
                }
                else
                {
                    Debug.Log(nearest);
                    ScaleObject(nearest);
                }
            }
        }

        // Unsnap
        if (Input.GetMouseButtonUp(0))
        {
            _Snapped = false; 
            closestFloat = 0;
        }
    }

    // Get the closest vector then return it (aka what do we think does the user want to do?)
    // Currently trying to get it to work in the original frame of the object
    private closestAxis GetClosestVector(Vector3 startMouse)
    {
        // Mouse stuff
        Vector3 screenSpace = Camera.main.WorldToScreenPoint(transform.position);
        Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenSpace.z);
        Vector3 mouseInWorld = Camera.main.ScreenToWorldPoint(mousePosition);

        Vector3 InverseVector = transform.InverseTransformPoint(mouseInWorld); // Get the position of the mouse relative to the object (shortcut I guess)

        if (InverseVector.x != 0 && System.Math.Abs(InverseVector.x) > closestFloat)
        {
            closestFloat = System.Math.Abs(InverseVector.x);
            
            if (InverseVector.x > 0)
            {
                nearest = closestAxis.xRight;
            }
            else
            {
                nearest = closestAxis.xLeft; 
            }
        }

        if (InverseVector.y != 0 && System.Math.Abs(InverseVector.y) > closestFloat)
        {
            closestFloat = System.Math.Abs(InverseVector.y);

            if (InverseVector.y > 0)
            {
                nearest = closestAxis.yUp;
            }
            else
            {
                nearest = closestAxis.yDown;
            }
        }

        return nearest;
    }

    // Does actual scaling here
    private void ScaleObject(closestAxis axis)
    {
        // To be edited
        Vector3 startScale = transform.localScale;
        Vector3 startPosition = transform.position;

        // Screen -> World position converison
        Vector3 screenSpace = Camera.main.WorldToScreenPoint(transform.position);
        Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenSpace.z);
        Vector3 mouseInWorld = Camera.main.ScreenToWorldPoint(mousePosition);

        #region old (refernce code)
        //switch (axis) // Old
        //{
        //    case closestAxis.x:
        //        startScale.x = System.Math.Abs(startSizes.x + (Input.mousePosition.x - startMouse.x) * sizingFactor);
        //        break;
        //    case closestAxis.y:
        //        startScale.y = System.Math.Abs(startSizes.y + (Input.mousePosition.y - startMouse.y) * sizingFactor);
        //        break;
        //    default:
        //        break;
        //}
        #endregion

        // in progress
        // BUG: Shifts a little bit at the beginning when scaling left or down. Not sure why this is happening?
            // I tried fixing it... But it just doens't work. WHY???
        switch (axis)
        {
            case closestAxis.xRight:
                // Tbh idk how this stuff works... 
                startScale.x = System.Math.Abs(startSizes.x + (Input.mousePosition.x - startMouse.x) * sizingFactor);
                startPosition.x = startPositions.x + startScale.x / 2.0f;
                transform.position = startPosition;
                break;
            case closestAxis.xLeft:
                // How to move it to the left? I know that startScale is just scaling it...
                startScale.x = System.Math.Abs(startSizes.x + (-Input.mousePosition.x + startMouse.x) * sizingFactor);
                startPosition.x = -(startPositions.x + startScale.x / 2.0f);
                transform.position = startPosition;
                break;
            case closestAxis.yUp:
                startScale.y = System.Math.Abs(startSizes.y + (Input.mousePosition.y - startMouse.y) * sizingFactor);
                startPosition.y = startPositions.y + startScale.y / 2.0f;
                transform.position = startPosition;
                break;
            case closestAxis.yDown:
                startScale.y = System.Math.Abs(startSizes.y + (-Input.mousePosition.y + startMouse.y) * sizingFactor);
                startPosition.y = -(startPositions.x + startScale.y / 2.0f);
                transform.position = startPosition;
                break;
        }

        oppositeScaleChildren(startScale);       
    }

    #region OldScaling
    // Deleted mouseposition = Camera.main.ScreenToWorldPoint(mousePosition)
    private void ChangeXScale()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z);
            startNum = mousePosition.x;
            startSize = gameObject.transform.localScale.x;
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 startScale = gameObject.transform.localScale;

            // Necessary so it doesnt rever to original.
            if (Input.mousePosition.x - startNum != 0)
            {
                startScale.x = System.Math.Abs(startSize + (Input.mousePosition.x - startNum) * sizingFactor);
                oppositeScaleChildren(startScale);
            }
        }
    }

    private void ChangeYScale()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z);
            startNum = mousePosition.y;
            startSize = gameObject.transform.localScale.y;
        }

        if (Input.GetMouseButton(1))
        {
            Vector3 startScale = gameObject.transform.localScale;

            // Necessary so it doesnt rever to original.
            if (Input.mousePosition.y - startNum != 0)
            {
                startScale.y = System.Math.Abs(startSize + (Input.mousePosition.y - startNum) * sizingFactor);
                oppositeScaleChildren(startScale);
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
            startSize = gameObject.transform.localScale.z;
        }

        if (Input.GetMouseButton(2))
        {
            Vector3 startScale = gameObject.transform.localScale;

            // Necessary so it doesnt rever to original.
            if (Input.mousePosition.y - startNum != 0)
            {
                startScale.z = System.Math.Abs(startSize + (Input.mousePosition.y - startNum) * sizingFactor);
                oppositeScaleChildren(startScale);
            }

        }
    }
    #endregion

    // Scales the children in the opposite way such that they retain their "scale." 
    // Intorduces "space" atm but doesnt seem like a scaling issue... It might scale the space around the objects just a little bit?
    private void oppositeScaleChildren(Vector3 newScale)
    {
        // Get all of the children then set their scale to the opposite reciprocal
        Transform[] childrenTransforms = GetComponentsInChildren<Transform>();
        foreach (Transform childTransform in childrenTransforms)
        {
            if (childTransform != transform)
            { 
                childTransform.parent = null;
                Vector3 scaleTmp = childTransform.localScale;
                scaleTmp.x = scaleTmp.x / newScale.x;
                scaleTmp.y = scaleTmp.y / newScale.y;
                scaleTmp.z = scaleTmp.z / newScale.z;
                Debug.Log(scaleTmp);
                childTransform.parent = transform;
                childTransform.localScale = scaleTmp;
            }
        }

        // Set gameObject scale
        transform.localScale = newScale;
    }
    #endregion

    // Method for moving the object 
    private static void MoveObject(GameObject movingObject)
    {
        Vector3 screenSpace = Camera.main.WorldToScreenPoint(movingObject.transform.position);
        Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenSpace.z);
        Vector3 objPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        movingObject.transform.position = objPosition;
    } 

    // Method for getting the rootJoint recursively (Used for moving locked object)
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

    #endregion
}

// Note: Need root joint if we want to test transform.