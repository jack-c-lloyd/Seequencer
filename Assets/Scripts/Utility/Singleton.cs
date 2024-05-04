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

namespace Utility
{

/// <summary>
/// <see cref="MonoBehaviour"/>-based adaption of the Singleton design pattern.
/// </summary>
/// <remarks>
/// <b>Reference</b>: 
/// <see href="https://refactoring.guru/design-patterns/singleton"/>
/// </remarks>
/// <example>
/// <code>
/// // MySingleton.cs
/// public class MySingleton : Utility.Singleton&lt;MySingleton&gt;
/// {
///     public void MyMethod()
///     {
///         // ...
///     }
/// }
/// </code>
/// <code>
/// // MyExample.cs
/// public class MyExample : MonoBehaviour
/// {
///     private void Awake()
///     {
///         MySingleton.Instance.MyMethod(); // Unsafe
///     }
///
///     private void Start()
///     {
///         MySingleton.Instance.MyMethod(); // Safe
///     }
/// }
/// </code>
/// </example>
/// <typeparam name="T">Inherits from <see cref="MonoBehaviour"/>.</typeparam>
[DisallowMultipleComponent]
public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
	/// <summary>
	/// If it is marked as persistent, <see cref="Object.DontDestroyOnLoad"/>
	/// is called in <see cref="Setup"/>.
	/// </summary>
	[SerializeField]
	private bool _persistent = false;

	/// <summary>
	/// Singular instance of <see cref="T"/>, if not <c>null</c>.
	/// </summary>
	/// <remarks>
	/// <b>Warning</b>:
	/// do not call before <see cref="Setup"/>.
	/// </remarks>
	public static T Instance { get; private set; } = null;

	/// <summary>
	/// Set up <see cref="Instance"/>, if and only if it is <c>null</c>.
	/// </summary>
	/// <remarks>
	/// <b>Warning</b>:
	/// must be called in <see cref="Awake"/>.
	/// </remarks>
	protected void Setup()
	{
		if (Instance == null)
		{
			Instance = this as T;

			if (_persistent)
			{
				DontDestroyOnLoad(this);
			}
		}

		if (Instance != this)
		{
			Destroy(this);
		}
	}

	/// <remarks>
	/// <b>Warning</b>:
	/// must call <see cref="Setup"/>.
	/// </remarks>
	private void Awake()
	{
		Setup();
	}
}

}