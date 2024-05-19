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
/// An <see cref="Interactor"/> calls <see cref="Interactable.Enter"/> and
/// <see cref="Interactable.Exit"/>, which invokes events per interaction.
/// </summary>
/// <remarks>
/// <b>Note:</b>
/// they co-maintain a one-to-one relationship (i.e., constraint).
/// </remarks>
[DisallowMultipleComponent]
[AddComponentMenu("Seequencer/Interaction/Interactor")]
public class Interactor : MonoBehaviour
{
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
	/// Reference to the currently interacting with <see cref="Interactable"/>,
	/// if not <c>null</c>.
	/// </summary>
	protected Interactable _current = null;

	/// <summary>
	/// Reference to the previously interacted with <see cref="Interactable"/>,
	/// if not <c>null</c>.
	/// </summary>
	protected Interactable _previous = null;

	/// <summary>
	/// Detect the current <see cref="Interactable"/>.
	/// </summary>
	/// <remarks>
	/// Should be called before <see cref="Compare"/>.
	/// </remarks>
	protected virtual void Detect()
	{
		Ray ray = new(transform.position, transform.forward);

		if (Physics.Raycast(ray, out RaycastHit hit))
		{
			_current = hit.transform.GetComponent<Interactable>();
		}
		else
		{
			_current = null;
		}
	}
	
	/// <summary>
	/// Compare the current <see cref="Interactable"/> to the previous; if they
	/// are not the same, exit the previous interaction and attempt to enter an
	/// interaction with the current.
	/// </summary>
	/// <remarks>
	/// Should be called after <see cref="Detect"/>.
	/// </remarks>
	private void Compare()
	{
		if (_current != _previous)
		{
			if (_previous != null)
			{
				_previous.Exit(this);

				OnExit?.Invoke();
			}

			_previous = null;

			if (_current != null && _current.Enter(this))
			{
				_previous = _current;

				OnEnter?.Invoke();
			}
		}
	}

	/// <summary>
	/// Method called by an <see cref="Interactable"/> to complete an interaction.
	/// </summary>
	/// <remarks>
	/// <b>Note:</b>
	/// an interaction can only be completed if the <see cref="Interactor"/> is
	/// interacting with the same <see cref="Interactable"/>.
	/// </remarks>
	/// <param name="interactable">Completed <see cref="Interactable"/>.</param>
	/// <returns>
	/// <c>true</c> if the interaction is completed, otherwise <c>false</c>.
	/// </returns>
	public bool Complete(Interactable interactable)
	{
		if (interactable != _current)
		{
			return false;
		}

		OnComplete?.Invoke();

		return true;
	}

	/// <remarks>
	/// Must call <see cref="Detect"/> before <see cref="Compare"/>.
	/// </remarks>
	private void FixedUpdate()
	{
		Detect();
		Compare();
	}

	/// <remarks>
	/// Must call <see cref="Interactable.Exit"/> in order to prevent an
	/// interaction-based softlock for <see cref="_previous"/>.
	/// </remarks>
	private void OnDestroy()
	{
		if (_previous != null)
		{
			_previous.Exit(this);
		}

		OnExit?.Invoke();
	}

	/// <remarks>
	/// Must call <see cref="Interactable.Exit"/> in order to prevent an
	/// interaction-based softlock for <see cref="_previous"/>.
	/// </remarks>
	private void OnDisable()
	{
		if (_previous != null)
		{
			_previous.Exit(this);
			_previous = null;
		}

		OnExit?.Invoke();
	}
}

}