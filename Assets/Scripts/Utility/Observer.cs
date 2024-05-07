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

namespace Utility
{

/// <summary>
/// Base of <see cref="ISubject"/>.
/// </summary>
public interface ISubjectBase
{
	// ...
}

/// <summary>
/// <see cref="MonoBehaviour"/>-based adaption of the Observer design pattern.
/// </summary>
/// <remarks>
/// <b>Reference</b>: 
/// <see href="https://refactoring.guru/design-patterns/observer"/>
/// </remarks>
/// <example>
/// <code>
/// public class MySubject : MonoBehaviour, Utility.ISubject&lt;MyObserver&gt;
/// {
///     private HashSet&lt;MyObserver&gt; _observers = new();
///     
///     public IReadOnlyCollection&lt;MyObserver&gt; Observers => _observers;
///     
///     public bool Attach(MyObserver observer)
///     {
///         if (observer == null)
///         {
///             throw new System.ArgumentNullException(nameof(observer));
///         }
///         
///         return _observers.Add(observer);
///     }
///     
///     public bool Detach(MyObserver observer)
///     {
///         if (observer == null)
///         {
///             throw new System.ArgumentNullException(nameof(observer));
///         }
///         
///         return _observers.Remove(observer);
///     }
/// }
/// </code>
/// </example>
/// <typeparam name="T">Should derive from <see cref="IObserver"/>.</typeparam>
public interface ISubject<T> : ISubjectBase where T : IObserverBase
{
	/// <summary>
	/// Public-safe access to the observers.
	/// </summary>
	IReadOnlyCollection<T> Observers { get; }

	/// <summary>
	/// Attach an observer.
	/// </summary>
	/// <param name="observer">The observer to attach.</param>
	/// <returns>
	/// <c>true</c> if <c>observer</c> is attached, otherwise <c>false</c>.
	/// </returns>
	/// <exception cref="System.ArgumentNullException"/>
	bool Attach(T observer);

	/// <summary>
	/// Detach an observer.
	/// </summary>
	/// <param name="observer">The observer to detach.</param>
	/// <returns>
	/// <c>true</c> if <c>observer</c> is detached, otherwise <c>false</c>.
	/// </returns>
	/// <exception cref="System.ArgumentNullException"/>
	bool Detach(T observer);
}

/// <summary>
/// Base of <see cref="IObserver"/>.
/// </summary>
public interface IObserverBase
{
	// ...
}

/// <summary>
/// <see cref="MonoBehaviour"/>-based adaption of the Observer design pattern.
/// </summary>
/// <remarks>
/// <b>Reference</b>: 
/// <see href="https://refactoring.guru/design-patterns/observer"/>
/// </remarks>
/// <example>
/// <code>
/// public class MyObserver : MonoBehaviour, Utility.IObserver&lt;MySubject&gt;
/// {
///     public MySubject Subject { get; private set; }
///
///     public void OnEnable()
///     {
///         if (Subject != null)
///         {
///             Subject.Attach(this);
///         }
///     }
///
///     public void OnDisable()
///     {
///         if (Subject != null)
///         {
///             Subject.Detach(this);
///         }
///     }
/// }
/// </code>
/// </example>
/// <typeparam name="T">Should derive from <see cref="ISubject"/>.</typeparam>
public interface IObserver<T> : IObserverBase where T : ISubjectBase
{
	/// <summary>
	/// Public-safe access to the subject.
	/// </summary>
	T Subject { get; }

	/// <summary>
	/// <b>Warning</b>:
	/// must call <see cref="ISubject{T}.Attach"/> on the subject.
	/// </summary>
	void OnEnable();

	/// <summary>
	/// <b>Warning</b>:
	/// must call <see cref="ISubject{T}.Detach"/> on the subject.
	/// </summary>
	void OnDisable();
}

}