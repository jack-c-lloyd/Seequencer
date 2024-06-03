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

using UnityEngine;
using UnityEngine.Events;

namespace See
{
    /// <summary>
    /// Rotates in 2D (i.e., around the z-axis) towards the target in 3D.
    /// </summary>
    [AddComponentMenu("Seequencer/Navigation/Navigator")]
    public class Navigator : MonoBehaviour
    {
        /// <summary>
        /// Camera to navigate towards the target.
        /// </summary>
        [SerializeField]
        private Transform _camera = null;

        /// <summary>
        /// Target to navigate towards.
        /// </summary>
        [SerializeField]
        private Transform _target = null;

        /// <summary>
        /// Threshold of the look-at angle (in degrees).
        /// </summary>
        [Range(0, 90.0f)]
        [SerializeField]
        private float _threshold = 45.0f;

        /// <summary>
        /// Event invoked if the target is (re)set or detargeted.
        /// </summary>
        [SerializeField]
        private UnityEvent<bool> OnRetarget = null;

        /// <summary>
        /// (Re)set the target.
        /// </summary>
        /// <param name="target">Transform of the target.</param>
        public void Retarget(Transform target)
        {
            _target = target;
        }

        /// <summary>
        /// Rotate in 2D (i.e., around the z-axis) towards the target in 3D by
        /// projecting its position onto the plane of the camera.
        /// </summary>
        private void Rotate()
        {
            Vector3 point = _target.position;
            Vector3 planeNormal = _camera.forward;
            Vector3 projection = Vector3.ProjectOnPlane(point, planeNormal);
            Vector3 inverse = Quaternion.Inverse(_camera.rotation) * projection;

            float angle = Mathf.Atan2(inverse.y, inverse.x) * Mathf.Rad2Deg;

            transform.localRotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

        /// <summary>
        /// Try to calculate the angle between the camera and the target.
        /// </summary>
        /// <param name="angle">
        /// Angle between the camera and the target (in degrees).
        /// </param>
        /// <returns>
        /// <c>true</c> if the angle was calculated, otherwise <c>false</c>.
        /// </returns>
        /// <remarks>
        /// <b>Warning:</b>
        /// <c>angle</c> is set to <c>Mathf.Infinity</c> if it is not calculable.
        /// </remarks>
        private bool TryCalculateAngle(out float angle)
        {
            if (_camera == null || _target == null)
            {
                angle = Mathf.Infinity;

                return false;
            }

            Vector3 direction = (_target.position - _camera.position).normalized;

            float dot = Vector3.Dot(_camera.forward, direction);
            float radians = Mathf.Acos(Mathf.Clamp01(dot));
            float degrees = radians * Mathf.Rad2Deg;

            angle = degrees;

            return true;
        }

        /// <remarks>
        /// Must only call <see cref="Rotate"/>.
        /// </remarks>
        private void LateUpdate()
        {
            bool calculatedAngle = TryCalculateAngle(out float angle);

            OnRetarget?.Invoke(calculatedAngle && angle > _threshold);

            if (calculatedAngle)
            {
                Rotate();
            }
        }
    }
}
