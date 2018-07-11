// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
//
// Copyright (c) 2017 Magic Leap, Inc. (COMPANY) All Rights Reserved.
// Magic Leap, Inc. Confidential and Proprietary
//
//  NOTICE:  All information contained herein is, and remains the property
//  of COMPANY. The intellectual and technical concepts contained herein
//  are proprietary to COMPANY and may be covered by U.S. and Foreign
//  Patents, patents in process, and are protected by trade secret or
//  copyright law.  Dissemination of this information or reproduction of
//  this material is strictly forbidden unless prior written permission is
//  obtained from COMPANY.  Access to the source code contained herein is
//  hereby forbidden to anyone except current COMPANY employees, managers
//  or contractors who have executed Confidentiality and Non-disclosure
//  agreements explicitly covering such access.
//
//  The copyright notice above does not evidence any actual or intended
//  publication or disclosure  of  this source code, which includes
//  information that is confidential and/or proprietary, and is a trade
//  secret, of  COMPANY.   ANY REPRODUCTION, MODIFICATION, DISTRIBUTION,
//  PUBLIC  PERFORMANCE, OR PUBLIC DISPLAY OF OR THROUGH USE  OF THIS
//  SOURCE CODE  WITHOUT THE EXPRESS WRITTEN CONSENT OF COMPANY IS
//  STRICTLY PROHIBITED, AND IN VIOLATION OF APPLICABLE LAWS AND
//  INTERNATIONAL TREATIES.  THE RECEIPT OR POSSESSION OF  THIS SOURCE
//  CODE AND/OR RELATED INFORMATION DOES NOT CONVEY OR IMPLY ANY RIGHTS
//  TO REPRODUCE, DISCLOSE OR DISTRIBUTE ITS CONTENTS, OR TO MANUFACTURE,
//  USE, OR SELL ANYTHING THAT IT  MAY DESCRIBE, IN WHOLE OR IN PART.
//
// %COPYRIGHT_END%
// --------------------------------------------------------------------*/
// %BANNER_END%

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Experimental.XR.MagicLeap;

namespace MagicLeap
{
    /// <summary>
    /// This represents the controller text connectivity status.
    /// Red: MLInput error.
    /// Green: Controller connected.
    /// Yellow: Controller disconnected.
    /// </summary>
    [RequireComponent(typeof(Text))]
    public class ControllerStatusText : MonoBehaviour
    {
        #region Private Variables
        [Space, SerializeField, Tooltip("Controller hand to display stats for.")]
        private MLInput.Hand _hand;

        private MLInputController _controller;

        private Text _controllerStatusText;
        #endregion

        #region Unity Methods
        /// <summary>
        /// Initializes component data and starts MLInput.
        /// </summary>
        void Awake()
        {
            _controllerStatusText = gameObject.GetComponent<Text>();

            MLResult result = MLInput.Start();
            if (!result.IsOk)
            {
                Debug.LogError("Error ControllerStatusText starting MLInput, disabling script.");
                return;
            }

            _controller = MLInput.GetController(_hand);
        }

        /// <summary>
        /// Updates text to specify the latest status of the controller.
        /// </summary>
        void Update()
        {
            if (_controller != null)
            {
                if (_controller.Connected)
                {
                    if (_controller.Type == MLInputControllerType.Control)
                    {
                        _controllerStatusText.text = "Controller Connected";
                        _controllerStatusText.color = Color.green;
                    }
                    else if(_controller.Type == MLInputControllerType.MobileApp)
                    {
                        _controllerStatusText.text = "MLMA Connected";
                        _controllerStatusText.color = Color.green;
                    }
                    else
                    {
                        _controllerStatusText.text = "Unknown";
                        _controllerStatusText.color = Color.red;
                    }
                }
                else
                {
                    _controllerStatusText.text = "Disconnected";
                    _controllerStatusText.color = Color.yellow;
                }
            }
            else
            {
                _controllerStatusText.text = "Input Failed to Start";
                _controllerStatusText.color = Color.red;
            }
        }

        /// <summary>
        /// Cleans up the component.
        /// </summary>
        void OnDestroy()
        {
            MLInput.Stop();
        }
        #endregion
    }
}
