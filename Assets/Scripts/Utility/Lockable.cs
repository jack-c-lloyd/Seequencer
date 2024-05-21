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

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Utility
{

/// <summary>
/// Invokes events if it is locked/unlocked with a key.
/// </summary>
[AddComponentMenu("Utility/Lockable")]
public class Lockable : MonoBehaviour, IObserver<Lockable>, ISubject<Lockable>
{
	/// <summary>
	/// The master lock.
	/// </summary>
	[SerializeField]
	private Lockable _master = null;

	/// <summary>
	/// Public-safe access to <see cref="_master"/>.
	/// </summary>
	public Lockable Subject => _master;

	/// <summary>
	/// Set of dependent locks.
	/// </summary>
	private HashSet<Lockable> _dependents = new();

	/// <summary>
	/// Public-safe access to <see cref="_dependents"/>.
	/// </summary>
	public IReadOnlyCollection<Lockable> Observers => _dependents;

	/// <summary>
	/// Key required to lock and unlock.
	/// </summary>
	[SerializeField]
	private int _key = 0;

	/// <summary>
	/// <c>true</c> if it is locked, otherwise <c>false</c>.
	/// </summary>
	[SerializeField]
	private bool _isLocked = false;

	/// <summary>
	/// Public-safe access to <see cref="_isLocked"/>.
	/// </summary>
	public bool IsLocked => _isLocked;

	/// <summary>
	/// Event invoked if it is locked.
	/// </summary>
	[SerializeField]
	private UnityEvent OnLocked = null;
	
	/// <summary>
	/// Event invoked if it is unlocked.
	/// </summary>
	[SerializeField]
	private UnityEvent OnUnlocked = null;

	/// <summary>
	/// Method called to lock without a key.
	/// </summary>
	private void ForceLock()
	{
		_isLocked = true;

		OnLocked?.Invoke();

		LockDependents();
	}

	/// <summary>
	/// Method called to lock with a key.
	/// </summary>
	/// <param name="key">A public or private key.</param>
	public void Lock(int key = 0)
	{
		if (!_isLocked && key == _key)
		{
			ForceLock();
		}
	}

	/// <summary>
	/// Method called to lock the dependents.
	/// </summary>
	private void LockDependents()
	{
		foreach (Lockable dependents in _dependents)
		{
			dependents.ForceLock();
		}
	}

	/// <summary>
	/// Method called to unlock without a key.
	/// </summary>
	private void ForceUnlock()
	{
		_isLocked = false;

		OnUnlocked?.Invoke();
	}

	/// <summary>
	/// Method called to unlock with a key.
	/// </summary>
	/// <param name="key">A public or private key.</param>
	public void Unlock(int key = 0)
	{
		if (IsLocked && key == _key)
		{
			if (Subject == null || !Subject.IsLocked)
			{
				ForceUnlock();
			}
		}
	}

	/// <summary>
	/// Set up the initial state.
	/// </summary>
	/// <remarks>
	/// <b>Warning</b>:
	/// must be called in <see cref="Start"/>.
	/// </remarks>
	protected void Setup()
	{
		if (_isLocked)
		{
			ForceLock();
		}
		else
		{
			ForceUnlock();
		}
	}

	/// <remarks>
	/// <b>Warning</b>:
	/// must call <see cref="Setup"/>.
	/// </remarks>
	private void Start()
	{
		Setup();
	}

	/// <summary>
	/// Attach a dependent.
	/// </summary>
	/// <param name="dependent">The dependent to attach.</param>
	/// <returns>
	/// <c>true</c> if <c>dependent</c> is attached, otherwise <c>false</c>.
	/// </returns>
	/// <exception cref="System.ArgumentNullException"/>
	public bool Attach(Lockable dependent)
	{
		if (dependent == null)
		{
			throw new System.ArgumentNullException(nameof(dependent));
		}

		return _dependents.Add(dependent);
	}

	/// <summary>
	/// Detach a dependent.
	/// </summary>
	/// <param name="dependent">The dependent to detach.</param>
	/// <returns>
	/// <c>true</c> if <c>dependent</c> is detached, otherwise <c>false</c>.
	/// </returns>
	/// <exception cref="System.ArgumentNullException"></exception>
	public bool Detach(Lockable dependent)
	{
		if (dependent == null)
		{
			throw new System.ArgumentNullException(nameof(dependent));
		}

		return _dependents.Remove(dependent);
	}

	/// <remarks>
	/// <b>Warning</b>:
	/// must call <see cref="Attach"/>.
	/// </remarks>
	public void OnEnable()
	{
		if (Subject != null && !Subject.Attach(this))
		{
			Debug.LogError($"{this} could not attach to {Subject}.");
		}
	}

	/// <remarks>
	/// <b>Warning</b>:
	/// must call <see cref="Detach"/>.
	/// </remarks>
	public void OnDisable()
	{
		if (Subject != null && !Subject.Detach(this))
		{
			Debug.LogError($"{this} could not detach from {Subject}.");
		}
	}

	/// <remarks>
	/// Valid if there is not a circular dependency on itself.
	/// </remarks>
	private void OnValidate()
	{
		for (Lockable root = Subject; root != null; root = root.Subject)
		{
			if (root == this)
			{
				Debug.LogError($"{this} has a circular dependency on itself.");
				return;
			}
		}
	}
}

}
