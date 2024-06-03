//-----------------------------------------------------------------------
// <copyright file="CardboardarrowPointer.cs" company="Google LLC">
// Copyright 2023 Google LLC, 2024 Jack C. Lloyd
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
// </copyright>
//-----------------------------------------------------------------------

using UnityEngine;

namespace Hardboard
{
    /// <summary>
    /// Based on the reticle-pointer from the Google Cardboard XR Plugin for Unity.
    /// </summary>
    /// <remarks>
    /// <b>Reference</b>: 
    /// <see href="https://github.com/googlevr/cardboard-xr-plugin"/>
    /// </remarks>
    public class Projector
    {
        /// <summary>
        /// The angle in degrees defined between the two vectors that depart from
        /// the camera and point to the extremes of the minimum inner diameter of
        /// the projection.
        /// </summary>
        /// <remarks>
        /// Being `z` the distance from the camera to the object and `d_i` the
        /// inner diameter of the projection, this is 2*arctg(d_i/(2*z)).
        /// </remarks>
        public static float MIN_INNER_ANGLE = 0.0f;

        /// <summary>
        /// The angle in degrees defined between the two vectors that depart from
        /// the camera and point to the extremes of the minimum outer diameter of
        /// the projection.
        /// </summary>
        /// <remarks>
        /// Being `z` the distance from the camera to the object and `d_o` the
        /// outer diameter of the projection, this is 2*arctg(d_o/(2*z)).
        /// </remarks>
        public static float MIN_OUTER_ANGLE = 0.5f;

        /// <summary>
        /// Angle at which to expand the projection when intersecting with an
        /// object (in degrees).
        /// </summary>
        public static float GROWTH_ANGLE = 1.5f;

        /// <summary>
        /// Minimum distance between the camera and the projector (in meters).
        /// </summary>
        public static float MIN_DISTANCE = 0.45f;

        /// <summary>
        /// Maximum distance between the camera and the projector (in meters).
        /// </summary>
        public static float MAX_DISTANCE = 20.0f;

        /// <summary>
        /// Growth speed multiplier for the projection.
        /// </summary>
        public static float GROWTH_SPEED = 8.0f;

        /// <summary>
        /// The current inner angle of the projection (in degrees).
        /// </summary>
        private float _innerAngle = 0.0f;

        /// <summary>
        /// The current outer angle of the projection (in degrees).
        /// </summary>
        private float _outerAngle = 0.0f;

        /// <summary>
        /// The current distance of the projection (in meters).
        /// </summary>
        private float _distance = 0.0f;

        /// <summary>
        /// The current inner diameter of the projection, before distance
        /// multiplication (in meters).
        /// </summary>
        private float _innerDiameter = 0.0f;

        /// <summary>
        /// The current outer diameter of the projection, before distance
        /// multiplication (in meters).
        /// </summary>
        private float _outerDiameter = 0.0f;

        /// <summary>
        /// Renderer used by the projector.
        /// </summary>
        private Renderer _renderer = null;

        /// <summary>
        /// Create a projector for a renderer.
        /// </summary>
        /// <param name="renderer">The renderer to project.</param>
        public Projector(Renderer renderer)
        {
            if (renderer == null)
            {
                Debug.LogError($"{nameof(renderer)} is null.");
            }

            _renderer = renderer;
        }

        /// <summary>
        /// Updates the material based on the projection properties.
        /// </summary>
        public void UpdateDiameters()
        {
            _distance = Mathf.Clamp(_distance, MIN_DISTANCE, MAX_DISTANCE);

            _innerAngle = Mathf.Max(_innerAngle, MIN_INNER_ANGLE);
            _outerAngle = Mathf.Max(_outerAngle, MIN_OUTER_ANGLE);

            float inner_half_angle_radians = Mathf.Deg2Rad * _innerAngle * 0.5f;
            float outer_half_angle_radians = Mathf.Deg2Rad * _outerAngle * 0.5f;

            float inner_diameter = 2.0f * Mathf.Tan(inner_half_angle_radians);
            float outer_diameter = 2.0f * Mathf.Tan(outer_half_angle_radians);

            _innerDiameter = Mathf.Lerp(_innerDiameter,
                inner_diameter, Time.unscaledDeltaTime * GROWTH_SPEED);
            _outerDiameter = Mathf.Lerp(_outerDiameter,
                outer_diameter, Time.unscaledDeltaTime * GROWTH_SPEED);

            Material material = _renderer.material;

            material.SetFloat("_InnerDiameter", _innerDiameter * _distance);
            material.SetFloat("_OuterDiameter", _outerDiameter * _distance);
            material.SetFloat("_DistanceInMeters", _distance);
        }

        /// <summary>
        /// Sets the projection's inner angle, outer angle and distance.
        /// </summary>
        /// <param name="distance">The distance to the target location.</param>
        /// <param name="expand">Whether the projection should expand.</param>
        public void SetParams(float distance, bool expand)
        {
            _distance = Mathf.Clamp(distance, MIN_DISTANCE, MAX_DISTANCE);

            if (expand)
            {
                _innerAngle = MIN_INNER_ANGLE + GROWTH_ANGLE;
                _outerAngle = MIN_OUTER_ANGLE + GROWTH_ANGLE;
            }
            else
            {
                _innerAngle = MIN_INNER_ANGLE;
                _outerAngle = MIN_OUTER_ANGLE;
            }
        }

        /// <summary>
        /// Reset the projection's inner angle, outer angle and distance.
        /// </summary>
        public void ResetParams()
        {
            _distance = MAX_DISTANCE;
            _innerAngle = MIN_INNER_ANGLE;
            _outerAngle = MIN_OUTER_ANGLE;
        }
    }
}