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

namespace See
{

/// <summary>
/// Based on the reticle-pointer from the Google Cardboard XR Plugin for Unity.
/// </summary>
/// <remarks>
/// <b>Reference</b>: 
/// <see href="https://github.com/googlevr/cardboard-xr-plugin"/>
/// </remarks>
[AddComponentMenu("Interactive/Interaction/Reticle")]
[RequireComponent(typeof(SkinnedMeshRenderer))]
public class Reticle : Interactor
{
	/// <summary>
	/// Maximum distance of an interaction (in meters).
	/// </summary>
	[Range(_RETICLE_MIN_DISTANCE, _RETICLE_MAX_DISTANCE)]
	[SerializeField]
	private float _distance = 10.0f;

	/// <summary>
	/// Layer mask of <see cref="Interactable"/> components.
	/// </summary>
	public LayerMask layerMask = 1 << 3;

	/// <summary>
	/// Index of the <see cref="BlendShape"/> to close the reticle.
	/// </summary>
	private const int _BLENDSHAPE_INDEX = 0;

	/// <summary>
	/// Mesh used to render the reticle.
	/// </summary>
	private SkinnedMeshRenderer _renderer = null;

	/// <summary>
	/// Update the mesh based on the reticle properties.
	/// </summary>
	private void UpdateWeights()
	{
		float weight = (_current != null) ? _current.Percentage : 0.0f;

		_renderer.SetBlendShapeWeight(_BLENDSHAPE_INDEX, weight);
	}

	/// <summary>
	/// Detect the current <see cref="Interactable"/>.
	/// </summary>
	/// <remarks>
	/// <b>Note:</b>
	/// overridden to accommodate for the Google Cardboard XR Plugin for Unity.
	/// </remarks>
	protected override void Detect()
	{
		Ray ray = new Ray(transform.position, transform.forward);

		if (Physics.Raycast(ray, out RaycastHit hit, _distance, layerMask.value))
		{
			_current = hit.transform.GetComponent<Interactable>();

			SetParams(hit.distance, _current != null);
		}
		else
		{
			_current = null;

			ResetParams();
		}
	}

	/// <remarks>
	/// Get <see cref="_renderer"/>.
	/// </remarks>
	private void Awake()
	{
		_renderer = GetComponent<SkinnedMeshRenderer>();
	}

	/// <remarks>
	/// Use the Google Cardboard XR Plugin API for manual interactions.
	/// </remarks>
	private void Update()
	{
		if (Google.XR.Cardboard.Api.IsTriggerPressed && _current != null)
		{
			_current.Complete(this);
		}
	}

	/// <remarks>
	/// Refer to <see cref="UpdateDiameters"/> and <see cref="UpdateWeights"/>.
	/// </remarks>
	private void LateUpdate()
	{
		UpdateDiameters();
		UpdateWeights();
	}

//-----------------------------------------------------------------------
// <copyright file="CardboardReticlePointer.cs" company="Google LLC">
// Copyright 2023 Google LLC
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

	/// <summary>
	/// The angle in degrees defined between the 2 vectors that depart from the camera and point to
	/// the extremes of the minimum inner diameter of the reticle.
	///
	/// Being `z` the distance from the camera to the object and `d_i` the inner diameter of the
	/// reticle, this is 2*arctg(d_i/(2*z)).
	/// </summary>
	private const float _RETICLE_MIN_INNER_ANGLE = 0.0f;

	/// <summary>
	/// The angle in degrees defined between the 2 vectors that depart from the camera and point to
	/// the extremes of the minimum outer diameter of the reticle.
	///
	/// Being `z` the distance from the camera to the object and `d_o` the outer diameter of the
	/// reticle, this is 2*arctg(d_o/(2*z)).
	/// </summary>
	private const float _RETICLE_MIN_OUTER_ANGLE = 0.5f;

	/// <summary>
	/// Angle at which to expand the reticle when intersecting with an object (in degrees).
	/// </summary>
	private const float _RETICLE_GROWTH_ANGLE = 1.5f;

	/// <summary>
	/// Minimum distance between the camera and the reticle (in meters).
	/// </summary>
	private const float _RETICLE_MIN_DISTANCE = 0.45f;

	/// <summary>
	/// Maximum distance between the camera and the reticle (in meters).
	/// </summary>
	private const float _RETICLE_MAX_DISTANCE = 20.0f;

	/// <summary>
	/// Growth speed multiplier for the reticle.
	/// </summary>
	private const float _RETICLE_GROWTH_SPEED = 8.0f;

	/// <summary>
	/// The material used to render the reticle.
	/// </summary>
	private Material _reticleMaterial => _renderer.material; // MODIFIED

	/// <summary>
	/// The current inner angle of the reticle (in degrees).
	/// </summary>
	private float _reticleInnerAngle;

	/// <summary>
	/// The current outer angle of the reticle (in degrees).
	/// </summary>
	private float _reticleOuterAngle;

	/// <summary>
	/// The current distance of the reticle (in meters).
	/// </summary>
	private float _reticleDistanceInMeters;

	/// <summary>
	/// The current inner diameter of the reticle, before distance multiplication (in meters).
	/// </summary>
	private float _reticleInnerDiameter;

	/// <summary>
	/// The current outer diameter of the reticle, before distance multiplication (in meters).
	/// </summary>
	private float _reticleOuterDiameter;

	/// <summary>
	/// Updates the material based on the reticle properties.
	/// </summary>
	private void UpdateDiameters()
	{
		_reticleDistanceInMeters =
		Mathf.Clamp(_reticleDistanceInMeters, _RETICLE_MIN_DISTANCE, _RETICLE_MAX_DISTANCE);

		if (_reticleInnerAngle < _RETICLE_MIN_INNER_ANGLE)
		{
			_reticleInnerAngle = _RETICLE_MIN_INNER_ANGLE;
		}

		if (_reticleOuterAngle < _RETICLE_MIN_OUTER_ANGLE)
		{
			_reticleOuterAngle = _RETICLE_MIN_OUTER_ANGLE;
		}

		float inner_half_angle_radians = Mathf.Deg2Rad * _reticleInnerAngle * 0.5f;
		float outer_half_angle_radians = Mathf.Deg2Rad * _reticleOuterAngle * 0.5f;

		float inner_diameter = 2.0f * Mathf.Tan(inner_half_angle_radians);
		float outer_diameter = 2.0f * Mathf.Tan(outer_half_angle_radians);

		_reticleInnerDiameter = Mathf.Lerp(
			_reticleInnerDiameter, inner_diameter, Time.unscaledDeltaTime * _RETICLE_GROWTH_SPEED);
		_reticleOuterDiameter = Mathf.Lerp(
			_reticleOuterDiameter, outer_diameter, Time.unscaledDeltaTime * _RETICLE_GROWTH_SPEED);

		_reticleMaterial.SetFloat(
			"_InnerDiameter", _reticleInnerDiameter * _reticleDistanceInMeters);
		_reticleMaterial.SetFloat(
			"_OuterDiameter", _reticleOuterDiameter * _reticleDistanceInMeters);
		_reticleMaterial.SetFloat("_DistanceInMeters", _reticleDistanceInMeters);
	}

	/// <summary>
	/// Sets the reticle pointer's inner angle, outer angle and distance.
	/// </summary>
	/// <param name="distance">The distance to the target location.</param>
	/// <param name="interactive">Whether the pointer is pointing at an interactive object.</param>
	private void SetParams(float distance, bool interactive)
	{
		_reticleDistanceInMeters = Mathf.Clamp(distance,
												_RETICLE_MIN_DISTANCE,
												_RETICLE_MAX_DISTANCE);
		if (interactive)
		{
			_reticleInnerAngle = _RETICLE_MIN_INNER_ANGLE + _RETICLE_GROWTH_ANGLE;
			_reticleOuterAngle = _RETICLE_MIN_OUTER_ANGLE + _RETICLE_GROWTH_ANGLE;
		}
		else
		{
			_reticleInnerAngle = _RETICLE_MIN_INNER_ANGLE;
			_reticleOuterAngle = _RETICLE_MIN_OUTER_ANGLE;
		}
	}

	/// <summary>
	/// Exits the reticle pointer's target.
	/// </summary>
	private void ResetParams()
	{
		_reticleDistanceInMeters = _RETICLE_MAX_DISTANCE;
		_reticleInnerAngle = _RETICLE_MIN_INNER_ANGLE;
		_reticleOuterAngle = _RETICLE_MIN_OUTER_ANGLE;
	}

//-----------------------------------------------------------------------

}

}
