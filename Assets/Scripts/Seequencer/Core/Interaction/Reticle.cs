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
	/// Projector used by the reticle.
	/// </summary>
	private Hardboard.Projector _projector = new();

	/// <summary>
	/// Skinned mesh used by the reticle.
	/// </summary>
	private SkinnedMeshRenderer _renderer = null;

	/// <summary>
	/// Index of the <see cref="BlendShape"/> to close the reticle.
	/// </summary>
	private const int _BLENDSHAPE_INDEX = 0;

	/// <summary>
	/// Update the mesh based on the reticle properties.
	/// </summary>
	private void UpdateWeights()
	{
		float weight = Current?.Percentage ?? 0.0f;

		_renderer.SetBlendShapeWeight(_BLENDSHAPE_INDEX, weight);
	}

	/// <remarks>
	/// Get <see cref="_renderer"/>.
	/// </remarks>
	private void Awake()
	{
		if (!TryGetComponent(out _renderer))
		{
			Debug.LogError($"{nameof(_renderer)} is null.");
		}
	}

	/// <remarks>
	/// Use the Google Cardboard XR Plugin API for manual interactions.
	/// </remarks>
	private void Update()
	{
		if (Google.XR.Cardboard.Api.IsTriggerPressed)
		{
			Current?.Complete(this);
		}
	}

	/// <remarks>
	/// Refer to <see cref="UpdateWeights"/>.
	/// </remarks>
	private void LateUpdate()
	{
		_projector.SetParams(Distance, Current != null);
		_projector.UpdateDiameters(_renderer.material);

		UpdateWeights();
	}
}

}
