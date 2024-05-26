// Copyright 2024 Jack C. Lloyd
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

namespace See
{
    /// <summary>
    /// Control the pitch and yaw rotation of a camera.
    /// </summary>
    public class CameraController : MonoBehaviour
    {
        /// <summary>
        /// Speed of rotation (in degrees per second).
        /// </summary>
        [Range(float.Epsilon, 360.0f)]
        [SerializeField]
        private float _speed = 360.0f;

        /// <summary>
        /// Pitch of the camera (in degrees).
        /// </summary>
        [SerializeField]
        private float _pitch = 0.0f;

        /// <summary>
        /// Public-safe access to <see cref="_pitch"/>.
        /// </summary>
        public float Pitch => _pitch;

        /// <summary>
        /// Yaw of the camera (in degrees).
        /// </summary>
        [SerializeField]
        private float _yaw = 0.0f;

        /// <summary>
        /// Public-safe access to <see cref="_yaw"/>.
        /// </summary>
        public float Yaw => _yaw;

        /// <summary>
        /// Value of <see cref="Look"/>.
        /// </summary>
        private Vector2 _look = Vector2.zero;

        /// <summary>
        /// Control the pitch and yaw.
        /// </summary>
        /// <param name="context"></param>
        public void Look(InputAction.CallbackContext context)
        {
            _look = context.ReadValue<Vector2>();
        }

        /// <summary>
        /// Control the mouse lock-state and visibility.
        /// </summary>
        /// <param name="hasFocus">
        /// <c>true</c> if it is focused, otherwise <c>false</c>.
        /// </param>
        private void OnApplicationFocus(bool hasFocus)
        {
            Cursor.lockState = hasFocus ? CursorLockMode.Locked
                                        : CursorLockMode.None;
            Cursor.visible = !hasFocus;
        }

        /// <summary>
        /// Rotate the camera based on the pitch and yaw.
        /// </summary>
        private void Rotate(float deltaTime)
        {
            _pitch -= _look.y * _speed * deltaTime;
            _pitch = Mathf.Clamp(_pitch, -90.0f, 90.0f);
            _yaw += _look.x * _speed * deltaTime;

            transform.localEulerAngles = new Vector3(_pitch, _yaw, 0.0f);
        }

        /// <remarks>
        /// Refer to <see cref="Rotate"/>.
        /// </remarks>
        private void Update()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            // ...
#elif UNITY_IOS && !UNITY_EDITOR
            // ...
#else
            Rotate(Time.deltaTime);
#endif
        }
    }
}