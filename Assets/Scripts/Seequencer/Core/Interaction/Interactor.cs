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
	/// <c>true</c> if it can interact with <see cref="_current"/>.
	/// </summary>
	/// <remarks>
	/// Must use <see cref="CanInteract"/> for getting/setting.
	/// </remarks>
	[SerializeField]
	private bool _canInteract = false;

	/// <summary>
	/// Public-safe access to <see cref="_canInteract"/>.
	/// </summary>
	public bool CanInteract
	{
		get
		{
			return _canInteract;
		}

		set
		{
			if (!value)
			{
				Cancel();
			}

			_canInteract = value;
		}
	}

	/// <summary>
	/// Layer mask of <see cref="Interactable"/> components to detect.
	/// </summary>
	[SerializeField]
	protected LayerMask layerMask = 1 << 3;

	/// <summary>
	/// Event invoked if an interaction is entered.
	/// </summary>
	[SerializeField]
	private UnityEvent OnEnter;

	/// <summary>
	/// Event invoked if an interaction is exited.
	/// </summary>
	[SerializeField]
	private UnityEvent OnExit;

	/// <summary>
	/// Event invoked if an interaction is completed.
	/// </summary>
	[SerializeField]
	private UnityEvent OnComplete;

	/// <summary>
	/// Reference to the currently interacting with <see cref="Interactable"/>,
	/// if not <c>null</c>.
	/// </summary>
	private Interactable _current = null;

	/// <summary>
	/// Public-safe access to <see cref="_current"/>.
	/// </summary>
	/// <remarks>
	/// <b>Note:</b>
	/// this allows for both null-conditional and null-coalescing operations.
	/// </remarks>
	public Interactable Current => (_current != null) ? _current : null;

	/// <summary>
	/// Reference to the previously interacted with <see cref="Interactable"/>,
	/// if not <c>null</c>.
	/// </summary>
	private Interactable _previous = null;

	/// <summary>
	/// Distance of the raycast (in meters).
	/// </summary>
	public float Distance { get; private set; } = 0.0f;

	/// <summary>
	/// Maximum distance of a raycast (in meters).
	/// </summary>
	public static readonly float MAX_DISTANCE = Hardboard.Projector.MAX_DISTANCE;

	/// <summary>
	/// Detect the current <see cref="Interactable"/>.
	/// </summary>
	/// <remarks>
	/// Must be called before <see cref="Compare"/>.
	/// </remarks>
	protected virtual void Detect()
	{
		Ray ray = new(transform.position, transform.forward);

		if (Physics.Raycast(ray, out RaycastHit hit, MAX_DISTANCE, layerMask.value))
		{
			_current = hit.transform.GetComponent<Interactable>();
			Distance = hit.distance;
		}
		else
		{
			_current = null;
			Distance = MAX_DISTANCE;
		}
	}
	
	/// <summary>
	/// Compare the current <see cref="Interactable"/> to the previous; if they
	/// are not the same, exit the previous interaction and attempt to enter an
	/// interaction with the current.
	/// </summary>
	/// <remarks>
	/// Must be called after <see cref="Detect"/>.
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

	private void Cancel()
	{
		if (_previous != null)
		{
			_previous.Exit(this);
			_previous = null;

			OnExit?.Invoke();
		}
		
		_current = null;
	}

	/// <remarks>
	/// Must call <see cref="Detect"/> before <see cref="Compare"/>.
	/// </remarks>
	private void FixedUpdate()
	{
		Detect();

		if (CanInteract)
		{
			Compare();
		}
	}

	/// <remarks>
	/// Must call <see cref="Cancel"/> to prevent an interaction softlock.
	/// </remarks>
	private void OnDestroy()
	{
		Cancel();
	}
}

}