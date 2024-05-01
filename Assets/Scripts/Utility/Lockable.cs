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
/// A <see cref="Lockable"/> invokes events if it is locked/unlocked with a key.
/// </summary>
[AddComponentMenu("Utility/Lockable")]
public class Lockable : MonoBehaviour, IChild<Lockable>, IParent<Lockable>
{
	/// <summary>
	/// The parent <see cref="Lockable"/>.
	/// </summary>
	[SerializeField]
	private Lockable _parent = null;

	/// <summary>
	/// Public-safe access to <see cref="_parent"/>.
	/// </summary>
	public Lockable Parent => _parent;

	/// <summary>
	/// Set of <see cref="Lockable"/> children.
	/// </summary>
	private HashSet<Lockable> _children = new();

	/// <summary>
	/// Public-safe access to <see cref="_children"/>.
	/// </summary>
	public IReadOnlyCollection<Lockable> Children => _children;

	
	[SerializeField]
	private bool unlockChildren = false;

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

		LockChildren();
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
	/// Method called to lock the children.
	/// </summary>
	private void LockChildren()
	{
		foreach (Lockable child in _children)
		{
			child.ForceLock();
		}
	}

	/// <summary>
	/// Method called to unlock without a key.
	/// </summary>
	private void ForceUnlock()
	{
		_isLocked = false;

		OnUnlocked?.Invoke();

		if (unlockChildren)
		{
			UnlockChildren();
		}
	}

	/// <summary>
	/// Method called to unlock with a key.
	/// </summary>
	/// <param name="key">A public or private key.</param>
	public void Unlock(int key = 0)
	{
		if (IsLocked && key == _key)
		{
			if (Parent == null || !Parent.IsLocked)
			{
				ForceUnlock();
			}
		}
	}

	/// <summary>
	/// Method called to unlock the children.
	/// </summary>
	private void UnlockChildren()
	{
		foreach (Lockable child in _children)
		{
			child.ForceUnlock();
		}
	}

	/// <summary>
	/// Set up the initial state.
	/// </summary>
	/// <remarks>
	/// Should be called in <see cref="Start"/>.
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
	/// Call <see cref="Setup"/>.
	/// </remarks>
	private void Start()
	{
		Setup();
	}

	/// <summary>
	/// Attach a <see cref="Lockable"/> child.
	/// </summary>
	/// <param name="child">The child to attach.</param>
	/// <returns>
	/// <c>true</c> if <c>child</c> is attached, otherwise <c>false</c>.
	/// </returns>
	/// <exception cref="System.ArgumentNullException"></exception>
	public bool Attach(Lockable child)
	{
		if (child == null)
		{
			throw new System.ArgumentNullException(nameof(child));
		}

		return _children.Add(child);
	}

	/// <summary>
	/// Detach a child.
	/// </summary>
	/// <param name="child">The child to detach.</param>
	/// <returns>
	/// <c>true</c> if <c>child</c> is detached, otherwise <c>false</c>.
	/// </returns>
	/// <exception cref="System.ArgumentNullException"></exception>
	public bool Detach(Lockable child)
	{
		if (child == null)
		{
			throw new System.ArgumentNullException(nameof(child));
		}

		return _children.Remove(child);
	}

	/// <remarks>
	/// Must call <see cref="Attach"/>.
	/// </remarks>
	public void OnEnable()
	{
		if (_parent != null && !_parent.Attach(this))
		{
			Debug.LogError($"{this} could not attach to {_parent}.");
		}
	}

	/// <remarks>
	/// Must call <see cref="Detach"/>.
	/// </remarks>
	public void OnDisable()
	{
		if (_parent != null && !_parent.Detach(this))
		{
			Debug.LogError($"{this} could not detach from {_parent}.");
		}
	}

	/// <remarks>
	/// Validate if there is not a circular dependency on itself.
	/// </remarks>
	private void OnValidate()
	{
		for (Lockable root = _parent; root != null; root = root.Parent)
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