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
	private bool _isFading = false;

	/// <summary>
	/// Fade in from the minimum to the maximum value.
	/// </summary>
	public void FadeIn()
	{
		StopAllCoroutines();
		StartCoroutine(Fade(_current, _maximum));
	}

	/// <summary>
	/// Fade out to the minimum from the maximum value.
	/// </summary>
	public void FadeOut()
	{	
		StopAllCoroutines();
		StartCoroutine(Fade(_current, _minimum));
	}

	/// <summary>
	/// Fade from the <c>start</c> to <c>end</c> values.
	/// </summary>
	/// <param name="start">Start value of the fade.</param>
	/// <param name="end">End value of the fade.</param>
	private IEnumerator Fade(float start, float end)
	{
		if (!_isFading)
		{
			_isFading = true;

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

			_isFading = false;
		}
	}
}

}