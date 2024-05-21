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

[AddComponentMenu("Utility/StabilityTrigger")]
public class StabilityTrigger : MonoBehaviour
{
	/// <summary>
	/// Stability threshold (in degrees).
	/// </summary>
	[SerializeField]
	private float _threshold = 10.0f;

	/// <summary>
	/// Cool-down time (in seconds).
	/// </summary>
	[SerializeField]
	private float _cooldown = 1.0f;

	/// <summary>
	/// Event invoked if it is stable (within the threshold).
	/// </summary>
	[SerializeField]
	private UnityEvent OnStable = null;

	/// <summary>
	/// Event invoked if it is unstable (bot within the threshold).
	/// </summary>
	[SerializeField]
	private UnityEvent OnUnstable = null;

	/// <summary>
	/// Forward-vector of the previous update.
	/// </summary>
	private Vector3 _lastForward = Vector3.zero;

	/// <summary>
	/// Elapsed time for the cooldown (in seconds).
	/// </summary>
	private float _elapsed = 0.0f;

	/// <summary>
	/// <c>true</c> if it is stable, otherwise <c>false</c>.
	/// </summary>
	private bool IsStable()
	{
		float dot = Vector3.Dot(_lastForward, transform.forward);
		float radians = Mathf.Acos(Mathf.Clamp01(dot));
		float degrees = radians * Mathf.Rad2Deg;

		_lastForward = transform.forward;

		return Mathf.Abs(degrees * Time.deltaTime) < _threshold * Time.deltaTime;
	}

	/// <summary>
	/// Invoke the events, <see cref="OnStable"/> or <see cref="OnUnstable"/>,
	/// if it is stable or unstable, respectively.
	/// </summary>
	private void LateUpdate()
	{
		if (IsStable())
		{
			if ((_elapsed += Time.deltaTime) > _cooldown)
			{
				OnStable?.Invoke();
			}
		}
		else
		{
			_elapsed = 0.0f;

			OnUnstable?.Invoke();
		}
	}
}

