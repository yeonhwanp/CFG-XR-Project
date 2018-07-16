using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap.Unity;
using Leap.Unity.Interaction;
using System.Windows.Forms;

[AddComponentMenu("")]
[RequireComponent(typeof(InteractionBehaviour))]
public class RobotButtonManager : MonoBehaviour {

    [Tooltip("If enabled, the object will lerp to its hoverColor when a hand is nearby.")]
    public bool useHover = true;

    // What...
    [Tooltip("If enabled, the object will use its primaryHoverColor when the primary hover of an InteractionHand.")]
    public bool usePrimaryHover = false;

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

    // Region for the form stuff
    private Form window;
    private Button button1;
    private Button button2;
    private Button button3;
    private TextBox box1;
    private TextBox box2;
    private TextBox box3;

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

            // "Primary hover" is a special kind of hover state that an InteractionBehaviour can
            // only have if an InteractionHand's thumb, index, or middle finger is closer to it
            // than any other interaction object.
            if (_intObj.isPrimaryHovered && usePrimaryHover)
            {
                targetColor = primaryHoverColor;
            }
            else
            {
                // Of course, any number of objects can be hovered by any number of InteractionHands.
                // InteractionBehaviour provides an API for accessing various interaction-related
                // state information such as the closest hand that is hovering nearby, if the object
                // is hovered at all.
                if (_intObj.isHovered && useHover)
                {
                    float glow = _intObj.closestHoveringControllerDistance.Map(0F, 0.2F, 1F, 0.0F);
                    targetColor = Color.Lerp(defaultColor, hoverColor, glow);
                }
            }

            if (_intObj.isSuspended)
            {
                // If the object is held by only one hand and that holding hand stops tracking, the
                // object is "suspended." InteractionBehaviour provides suspension callbacks if you'd
                // like the object to, for example, disappear, when the object is suspended.
                // Alternatively you can check "isSuspended" at any time.
                targetColor = suspendedColor;
            }

            if (_intObj is InteractionButton && (_intObj as InteractionButton).isPressed)
            {
                if (!_isPushed)
                {
                    _isPushed = true;

                    // Initializing the window stuff
                    window = new Form();
                    button1 = new Button();
                    //button2 = new Button();
                    //button3 = new Button();
                    box1 = new TextBox();
                    box2 = new TextBox();
                    box3 = new TextBox();

                    button1.Name = "Test Button 1";
                    button1.Text = "Submit";
                    button1.Location = new System.Drawing.Point(100, 200);
                    button1.Width = 100;
                    button1.Click += Button1_Click;

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

                    window.Controls.Add(button1);
                    window.Controls.Add(box1);
                    window.Controls.Add(button2);
                    window.Controls.Add(box2);
                    window.Controls.Add(button3);
                    window.Controls.Add(box3);

                    window.ShowDialog();
                    Debug.Log("Opened window?");

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

    // What to do after data submission
    private void Button1_Click(object sender, System.EventArgs e)
    {
        Debug.Log(box1.Text + box2.Text + box3.Text);
        window.Close();
    }
}