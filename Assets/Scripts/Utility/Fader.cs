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
using UnityEngine;
using UnityEngine.Events;

namespace Utility
{

/// <summary>
/// Fade in or out between a minimum and maximum limit-range.
/// </summary>
[AddComponentMenu("Utility/Fader")]
public class Fader : MonoBehaviour
{
	/// <summary>
	/// Whether or not to disallow a fade from being interrupted by another.
	/// </summary>
	[SerializeField]
	private bool _disallowInterrupts = false;

	/// <summary>
	/// Current value before, during, or after a fade.
	/// </summary>
	[SerializeField]
	private float _current = 0.0f;

	/// <summary>
	/// Minimum value to fade in from and fade out to.
	/// </summary>
	[SerializeField]
	private float _minimum = 0.0f;

	/// <summary>
	/// Maximum value to fade in to and fade out from.
	/// </summary>
	[SerializeField]
	private float _maximum = 1.0f;

	/// <summary>
	/// Duration of a fade (in seconds).
	/// </summary>
	[Min(float.Epsilon)]
	[SerializeField]
	private float _duration = 1.0f;

	/// <summary>
	/// Event invoked if it is fading in or out.
	/// </summary>
	[SerializeField]
	private UnityEvent<float> OnFade = null;

	/// <summary>
	/// <c>true</c> if it is fading in or out, otherwise <c>false</c>.
	/// </summary>
	public bool IsFading { get; private set; } = false;

	/// <summary>
	/// Fade in from the minimum to the maximum value.
	/// </summary>
	public void FadeIn()
	{
		if (!IsFading || !_disallowInterrupts)
		{
			StopAllCoroutines();
			StartCoroutine(Fade(_current, _maximum));

		}
	}

	/// <summary>
	/// Fade out to the minimum from the maximum value.
	/// </summary>
	public void FadeOut()
	{	
		if (!IsFading || !_disallowInterrupts)
		{
			StopAllCoroutines();
			StartCoroutine(Fade(_current, _minimum));
		}
	}

	/// <summary>
	/// Fade from the <c>start</c> to <c>end</c> values.
	/// </summary>
	/// <param name="start">Start value of the fade.</param>
	/// <param name="end">End value of the fade.</param>
	private IEnumerator Fade(float start, float end)
	{
		IsFading = true;

		_current = start;
		OnFade?.Invoke(start);

		float elapsed = 0.0f;

		while ((elapsed += Time.deltaTime) < _duration)
		{
			float t = Mathf.Clamp01(elapsed / _duration);
			_current = Mathf.Lerp(start, end, t);

			OnFade?.Invoke(_current);

			yield return null;
		}

		_current = end;
		OnFade?.Invoke(end);

		IsFading = false;
	}

	/// <summary>
	/// Invokes <see cref="OnFade"/> with the initial state.
	/// </summary>
	private void Setup()
	{
		OnFade?.Invoke(_current);
	}

	/// <remarks>
	/// Must call <see cref="Setup"/>.
	/// </remarks>
	private void Awake()
	{
		Setup();
	}

	/// <remarks>
	/// Ensure the minimum limit is less than or equal to the maximum limit,
	/// and that <see cref="_current"/> is in-between.
	/// </remarks>
	private void OnValidate()
	{
		if (_minimum > _maximum)
		{
			Debug.LogError($"{nameof(_minimum)} must not be greater than {nameof(_maximum)}.");

			_minimum = _maximum;
		}

		if (_current < _minimum)
		{
			Debug.LogError($"{nameof(_current)} must not be less than {nameof(_minimum)}.");

			_current = _minimum;
		}

		if (_current > _maximum)
		{
			Debug.LogError($"{nameof(_current)} must not be greater than {nameof(_maximum)}.");

			_current = _maximum;
		}
	}
}

}
