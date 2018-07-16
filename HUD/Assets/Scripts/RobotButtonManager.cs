using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap.Unity;
using Leap.Unity.Interaction;
using System.Windows.Forms;
using System.Globalization;

[AddComponentMenu("")]
[RequireComponent(typeof(InteractionBehaviour))]
public class RobotButtonManager : MonoBehaviour {

    [Tooltip("If enabled, the object will lerp to its hoverColor when a hand is nearby.")]
    public bool useHover = true;

    // What...
    [Tooltip("If enabled, the object will use its primaryHoverColor when the primary hover of an InteractionHand.")]
    public bool usePrimaryHover = false;

    [Tooltip("Choose between Link and Joint button.")]
    public int JointLinkChoice = 0;

    [Header("InteractionBehaviour Colors")]
    public Color defaultColor = Color.Lerp(Color.black, Color.white, 0.1F);
    public Color suspendedColor = Color.red;
    public Color hoverColor = Color.Lerp(Color.black, Color.white, 0.7F);
    public Color primaryHoverColor = Color.Lerp(Color.black, Color.white, 0.8F);

    [Header("InteractionButton Colors")]
    [Tooltip("This color only applies if the object is an InteractionButton or InteractionSlider.")]
    public Color pressedColor = Color.white;

    private Material _material;

    private InteractionBehaviour _intObj;

    // Region for the form stuff, arbritary until assigned.
    private Form window;
    private Button button1;
    private Button button2;
    private Button button3;
    private TextBox box1;
    private TextBox box2;
    private TextBox box3;
    private TextBox box4;

    private bool _isPushed = false;

    // Just initialization here
    void Start()
    {
        _intObj = GetComponent<InteractionBehaviour>();

        Renderer renderer = GetComponent<Renderer>();
        if (renderer == null)
        {
            renderer = GetComponentInChildren<Renderer>();
        }
        if (renderer != null)
        {
            _material = renderer.material;
        }
    }

    void Update()
    {
        if (_material != null)
        {

            // The target color for the Interaction object will be determined by various simple state checks.
            Color targetColor = defaultColor;

            if (_intObj.isHovered && useHover)
            {
                float glow = _intObj.closestHoveringControllerDistance.Map(0F, 0.2F, 1F, 0.0F);
                targetColor = Color.Lerp(defaultColor, hoverColor, glow);
            }

            // On press
            if (_intObj is InteractionButton && (_intObj as InteractionButton).isPressed)
            {
                if (!_isPushed)
                {
                    _isPushed = true;
                    if (JointLinkChoice == 0) SpawnJointWindow();
                    if (JointLinkChoice == 1) SpawnLinkWindow();
                    else Debug.Log("Please input a valid choice for JointLinkChoice.");
                    targetColor = pressedColor;
                }
            }

            // For the cooldown
            if (_intObj is InteractionButton && !(_intObj as InteractionButton).isPressed)
            {
                _isPushed = false;
            }

            // Lerp actual material color to the target color.
            _material.color = Color.Lerp(_material.color, targetColor, 30F * Time.deltaTime);
        }
    }

    // Takes care of the spawning.
    private void SpawnJointWindow()
    {
        window = new Form();
        button1 = new Button();
        box1 = new TextBox();
        box2 = new TextBox();
        box3 = new TextBox();
        box4 = new TextBox();

        button1.Name = "Test Button 1";
        button1.Text = "Submit";
        button1.Location = new System.Drawing.Point(100, 250);
        button1.Width = 100;
        button1.Click += JointSubmit;

        box1.Name = "Test Box 1";
        box1.Text = "";
        box1.Location = new System.Drawing.Point(50, 50);
        box1.Width = 200;

        box2.Name = "Text Box 2";
        box2.Text = "";
        box2.Location = new System.Drawing.Point(50, 100);
        box2.Width = 200;

        box3.Name = "Text Box 3";
        box3.Text = "";
        box3.Location = new System.Drawing.Point(50, 150);
        box3.Width = 200;

        box4.Name = "Text Box 4";
        box4.Text = "";
        box4.Location = new System.Drawing.Point(50, 200);
        box4.Width = 200;

        window.Controls.Add(button1);
        window.Controls.Add(box1);
        window.Controls.Add(button2);
        window.Controls.Add(box2);
        window.Controls.Add(button3);
        window.Controls.Add(box3);
        window.Controls.Add(box4);

        window.ShowDialog();
    }

    private void SpawnLinkWindow()
    {
        window = new Form();
        button1 = new Button();
        box1 = new TextBox();
        box2 = new TextBox();
        box3 = new TextBox();
        box4 = new TextBox();

        button1.Name = "Test Button 1";
        button1.Text = "Submit";
        button1.Location = new System.Drawing.Point(100, 250);
        button1.Width = 100;
        button1.Click += LinkSubmit;

        box1.Name = "Test Box 1";
        box1.Text = "";
        box1.Location = new System.Drawing.Point(50, 50);
        box1.Width = 200;

        box2.Name = "Text Box 2";
        box2.Text = "";
        box2.Location = new System.Drawing.Point(50, 100);
        box2.Width = 200;

        box3.Name = "Text Box 3";
        box3.Text = "";
        box3.Location = new System.Drawing.Point(50, 150);
        box3.Width = 200;

        box4.Name = "Text Box 4";
        box4.Text = "";
        box4.Location = new System.Drawing.Point(50, 200);
        box4.Width = 200;

        window.Controls.Add(button1);
        window.Controls.Add(box1);
        window.Controls.Add(button2);
        window.Controls.Add(box2);
        window.Controls.Add(button3);
        window.Controls.Add(box3);
        window.Controls.Add(box4);

        window.ShowDialog();
    }

    // Take information provided then spawn part in the world.
    // For now, only child to the root joint. Later, choose which one you want. 
    // Also, need to make a method to generate ObjectJoints instead of just random GameObjects.
    // Also, should eventually make it such that when you open either link or joint button, then it should open up two different windows.
    // Would it just be easier to make two different scripts?
    private void JointSubmit(object sender, System.EventArgs e)
    {
        float xPos = float.Parse(box1.Text.Trim(), CultureInfo.InvariantCulture.NumberFormat);
        float yPos = float.Parse(box2.Text.Trim(), CultureInfo.InvariantCulture.NumberFormat);
        float zPos = float.Parse(box3.Text.Trim(), CultureInfo.InvariantCulture.NumberFormat);

        if (GameObject.Find("RobotRoot") == null)
        {
            GameObject rootObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            rootObject.name = "RobotRoot";
        }
        else
        {
            GameObject newJoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            newJoint.transform.parent = GameObject.Find("RobotRoot").transform;
            newJoint.name = box4.Text.Trim();
        }
        window.Close();
    }

    private void LinkSubmit(object sender, System.EventArgs e)
    {
        float xPos = float.Parse(box1.Text.Trim(), CultureInfo.InvariantCulture.NumberFormat);
        float yPos = float.Parse(box2.Text.Trim(), CultureInfo.InvariantCulture.NumberFormat);
        float zPos = float.Parse(box3.Text.Trim(), CultureInfo.InvariantCulture.NumberFormat);

        if (GameObject.Find("RobotRoot") == null)
        {
            Debug.Log("You need a root joint first before you spawn in a link.");
        }
        else
        {
            GameObject newLink = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            newLink.transform.parent = GameObject.Find("RobotRoot").transform;
            newLink.name = box4.Text.Trim();
        }
        window.Close();
    }
}