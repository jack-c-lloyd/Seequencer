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
/// An <see cref="Interactable"/> is called by an <see cref="Interactor"/> to
/// enter/exit an interaction, which calls <see cref="Interactor.Complete"/>.
/// </summary>
/// <remarks>
/// <b>Note:</b>
/// they co-maintain a one-to-one relationship.
/// </remarks>
[AddComponentMenu("Seequencer/Interaction/Interactable")]
public class Interactable : MonoBehaviour
{
	/// <summary>
	/// Minimum duration of an interaction (in seconds).
	/// </summary>
	private const float _DURATION_MIN = 0.5f;

	/// <summary>
	/// Maximum duration of an interaction (in seconds).
	/// </summary>
	private const float _DURATION_MAX = 3.0f;

	/// <summary>
	/// Duration of an interaction (in seconds).
	/// </summary>
	[Range(_DURATION_MIN, _DURATION_MAX)]
	[SerializeField]
	private float _duration = 1.5f;

	/// <summary>
	/// Elapsed time of an interaction (in seconds).
	/// </summary>
	private float _elapsed = 0.0f;

	/// <summary>
	/// Percentage of an interaction.
	/// </summary>
	public float Percentage => Mathf.Clamp01(_elapsed / _duration) * 100;

	/// <summary>
	/// Event invoked if an interaction is entered.
	/// </summary>
	[SerializeField]
	protected UnityEvent OnEnter;

	/// <summary>
	/// Event invoked if an interaction is exited.
	/// </summary>
	[SerializeField]
	protected UnityEvent OnExit;

	/// <summary>
	/// Event invoked if an interaction is completed.
	/// </summary>
	[SerializeField]
	protected UnityEvent OnComplete;

	/// <summary>
	/// Reference to the currently interacting with <see cref="Interactor"/>,
	/// if not <c>null</c>.
	/// </summary>
	private Interactor _current = null;
	
	/// <summary>
	/// Method called by an <see cref="Interactor"/> to enter an interaction.
	/// </summary>
	/// <remarks>
	/// <b>Note:</b>
	/// an interaction can only be entered if the <see cref="Interactable"/> is
	/// not interacting with another <see cref="Interactor"/>.
	/// </remarks>
	/// <param name="interactor">Entering <see cref="Interactor"/>.</param>
	/// <returns>
	/// <c>true</c> if the interaction is entered, otherwise <c>false</c>.
	/// </returns>
	public bool Enter(Interactor interactor)
	{
		if (_current != null)
		{
			return false;
		}

		_current = interactor;

		OnEnter?.Invoke();

		return true;
	}

	/// <summary>
	/// Method called by an <see cref="Interactor"/> to exit an interaction.
	/// </summary>
	/// <remarks>
	/// <b>Note:</b>
	/// an interaction can only be exited if the <see cref="Interactable"/> is
	/// interacting with the same <see cref="Interactor"/>.
	/// </remarks>
	/// <param name="interactor">Exiting <see cref="Interactor"/>.</param>
	public void Exit(Interactor interactor)
	{
		if (_current == interactor)
		{
			OnExit?.Invoke();

			_current = null;
		}
	}

	/// <summary>
	/// Method called by an <see cref="Interactor"/> to complete an interaction.
	/// </summary>
	/// <remarks>
	/// <b>Note:</b>
	/// an interaction can only be completed if the <see cref="Interactable"/>
	/// is interacting with the same <see cref="Interactor"/>.
	/// </remarks>
	/// <param name="interactor">An <see cref="Interactor"/>.</param>
	public void Complete(Interactor interactor)
	{
		if (_current == interactor)
		{
			CompleteTimer();
		}
	}

	/// <summary>
	/// Reset the elapsed time of an interaction.
	/// </summary>
	private void ResetTimer()
	{
		_elapsed = 0.0f;
	}

	/// <summary>
	/// Method called to complete an interaction.
	/// </summary>
	private void CompleteTimer()
	{
		if (_current != null && _current.Complete(this))
		{
			OnComplete?.Invoke();
			ResetTimer();
		}
	}

	/// <summary>
	/// Update the elapsed time of an interaction.
	/// </summary>
	private void UpdateTimer()
	{
		if (_current == null)
		{
			ResetTimer();
			return;
		}

		if ((_elapsed += Time.deltaTime) >= _duration)
		{
			CompleteTimer();
		}
	}

	/// <remarks>
	/// Must call <see cref="UpdateTimer"/>. 
	/// </remarks>
	private void Update()
	{
		UpdateTimer();
	}
}

}