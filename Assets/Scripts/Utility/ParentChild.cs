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
/// Base of <see cref="IChild"/>.
/// </summary>
public interface IChildBase
{
	// ...
}

/// <summary>
/// Base of <see cref="IParent"/>.
/// </summary>
public interface IParentBase
{
	// ...
}

/// <summary>
/// Define a parent-child relationship with <see cref="T"/> as the parent.
/// </summary>
/// <typeparam name="T">Should derive from <see cref="IParent"/>.</typeparam>
public interface IChild<T> : IChildBase where T : IParentBase
{
	/// <summary>
	/// Public-safe access to the parent.
	/// </summary>
	T Parent { get; }

	/// <summary>
	/// Must call <see cref="IParent{T}.Attach"/> on the <see cref="Parent"/>.
	/// </summary>
	void OnEnable();

	/// <summary>
	/// Must call <see cref="IParent{T}.Detach"/> on the <see cref="Parent"/>.
	/// </summary>
	void OnDisable();
}

/// <summary>
/// Define a parent-child relationship with <see cref="T"/> as the children.
/// </summary>
/// <typeparam name="T">Should derive from <see cref="IChild"/>.</typeparam>
public interface IParent<T> : IParentBase where T : IChildBase
{
	/// <summary>
	/// Public-safe access to the children.
	/// </summary>
	IReadOnlyCollection<T> Children { get; }

	/// <summary>
	/// Attach a child.
	/// </summary>
	/// <param name="child">The child to attach.</param>
	/// <returns>
	/// <c>true</c> if <c>child</c> is attached, otherwise <c>false</c>.
	/// </returns>
	/// <exception cref="System.ArgumentNullException"></exception>
	bool Attach(T child);

	/// <summary>
	/// Detach a child.
	/// </summary>
	/// <param name="child">The child to detach.</param>
	/// <returns>
	/// <c>true</c> if <c>child</c> is detached, otherwise <c>false</c>.
	/// </returns>
	/// <exception cref="System.ArgumentNullException"></exception>
	bool Detach(T child);
}

}