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

/// <summary>
/// Based on the reticle-pointer from the Google Cardboard XR Plugin for Unity.
/// </summary>
/// <remarks>
/// <b>Reference</b>: 
/// <see href="https://github.com/googlevr/cardboard-xr-plugin"/>
/// </remarks>
[AddComponentMenu("Interactive/Navigation/Arrow")]
[RequireComponent(typeof(Renderer))]
public class Arrow : MonoBehaviour
{
	/// <summary>
	/// Projector used by the arrow.
	/// </summary>
	private Hardboard.Projector _projector;

	/// <summary>
	/// Used for the arrow projection.
	/// </summary>
	[SerializeField]
	private See.Interactor _interactor = null;

	/// <remarks>
	/// Get <see cref="_renderer"/>.
	/// </remarks>
	private void Awake()
	{
		if (!TryGetComponent(out Renderer renderer))
		{
			Debug.LogError($"{nameof(renderer)} is null.");
		}

		_projector = new(renderer);
	}

	/// <remarks>
	/// Update the properties of the projection.
	/// </remarks>
	private void Update()
	{
		_projector.SetParams(_interactor.Distance, true);
		_projector.UpdateDiameters();
	}
}