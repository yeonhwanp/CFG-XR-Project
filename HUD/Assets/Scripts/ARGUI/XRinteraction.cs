using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap.Unity.Interaction;
using Leap.Unity.Query;
using System;

namespace Leap.Unity.Examples
{
    public class XRInteraction : MonoBehaviour
    {

        Controller controller;
        List<Hand> HandList = new List<Hand>();

        private void Start()
        {
            controller = new Controller();
        }

        private void FixedUpdate()
        {
            Frame frame = controller.Frame();
            HandList = frame.Hands;
            Vector3 original = transform.position;
        }
    }
}


