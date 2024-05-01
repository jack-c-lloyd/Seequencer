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
using System.Collections.Generic;
using UnityEngine;

namespace See
{

/// <summary>
/// A <see cref="Sequencer"/> has a set of attached <see cref="Pad"/> children,
/// of which it can generate, play, and record sequences.
/// </summary>
[AddComponentMenu("Seequencer/Gameplay/Sequencer")]
[RequireComponent(typeof(Utility.Lockable))]
public class Sequencer : MonoBehaviour, Utility.IParent<Pad>
{
	/// <summary>
	/// List of <see cref="Pad"/> children.
	/// </summary>
	private List<Pad> _children = new();

	/// <summary>
	/// Public-safe access to the children.
	/// </summary>
	public IReadOnlyCollection<Pad> Children => _children;

	/// <summary>
	/// Generated sequence of <see cref="Pad"/> children.
	/// </summary>
	private List<Pad> _sequence = new();

	/// <summary>
	/// Lock for recording.
	/// </summary>
	/// <remarks>
	/// <see cref="Utility.Lockable.unlockChildren"/> must be <c>true</c>.
	/// </remarks>
	private Utility.Lockable _lock = null;

	/// <summary>
	/// <c>true</c> if it is recording, otherwise <c>false</c>.
	/// </summary>
	private bool _isRecording = false;

	/// <summary>
	/// Index of the current <see cref="Pad"/>
	/// </summary>
	private int _recordingIndex = 0;

	/// <summary>
	/// Internal states.
	/// </summary>
	private enum State
	{
		/// <summary>
		/// Correct state.
		/// </summary>
		CORRECT,

		/// <summary>
		/// Wrong state.
		/// </summary>
		WRONG,

		/// <summary>
		/// Incomplete state.
		/// </summary>
		INCOMPLETE
	}

	/// <summary>
	/// Internal state.
	/// </summary>
	private State _recordingState = State.INCOMPLETE;

	/// <summary>
	/// Attach a <see cref="Pad"/> child.
	/// </summary>
	/// <param name="child">The child to attach.</param>
	/// <returns>
	/// <c>true</c> if <c>child</c> is attached, otherwise <c>false</c>.
	/// </returns>
	/// <exception cref="System.ArgumentNullException"></exception>
	public bool Attach(Pad child)
	{
		if (child == null)
		{
			throw new System.ArgumentNullException(nameof(child));
		}

		if (!_children.Contains(child))
		{
			_children.Add(child);
		}

		return true;
	}

	/// <summary>
	/// Detach a <see cref="Pad"/> child.
	/// </summary>
	/// <param name="child">The child to detach.</param>
	/// <returns>
	/// <c>true</c> if <c>child</c> is detached, otherwise <c>false</c>.
	/// </returns>
	/// <exception cref="System.ArgumentNullException"></exception>
	public bool Detach(Pad child)
	{
		if (child == null)
		{
			throw new System.ArgumentNullException(nameof(child));
		}

		_children.Remove(child);
		_sequence.RemoveAll(pad => pad == child);

		return true;
	}

	/// <summary>
	/// Generate a random sequence with the specified count.
	/// </summary>
	/// <param name="count">A positive integer.</param>
	public void Generate(uint count)
	{
		_sequence.Clear();

		if (_children.Count == 0)
		{
			Debug.LogError($"{_children} is empty.");
		}

		while (count-- > 0)
		{
			int index = Random.Range(0, _children.Count);
			_sequence.Add(_children[index]);
		}
	}
	
	/// <summary>
	/// Play the generated sequence.
	/// </summary>
	public IEnumerator Play()
	{
		foreach (Pad pad in _sequence)
		{
			yield return pad.Play();
		}
	}

	/// <summary>
	/// Record a sequence and compare it to the generated sequence.
	/// </summary>
	/// <param name="callback">
	/// Callback set to <c>true</c> if the recorded sequence is the same as the
	/// generated sequence, otherwise <c>false</c>.
	/// </param>
	public IEnumerator Record(System.Action<bool> callback)
	{
		if (!_isRecording)
		{
			_isRecording = true;
			_recordingState = State.INCOMPLETE;
			_recordingIndex = 0;

			_lock.Unlock();

			yield return new WaitWhile(() => _isRecording);

			_lock.Lock();

			callback?.Invoke(_recordingState == State.CORRECT);
		}
	}

	/// <summary>
	/// Method called by a <see cref="Pad"/> if it has been pressed by a player;
	/// if in recording mode it should be compared to the sequence.
	/// </summary>
	/// <param name="pad">The <see cref="Pad"/> that has been pressed.</param>
	public void Pressed(Pad pad)
	{
		if (_isRecording)
		{
			if (_sequence[_recordingIndex] == pad)
			{
				_recordingIndex++;

				StartCoroutine(pad.Play());

				if (_recordingIndex < _sequence.Count)
				{
					_recordingState = State.INCOMPLETE;
				}
				else
				{
					_recordingState = State.CORRECT;
					_isRecording = false;
				}
			}
			else
			{
				_recordingState = State.WRONG;
				_isRecording = false;
			}
		}
	}

	/// <remarks>
	/// Get <see cref="_lock"/>.
	/// </remarks>
	private void Awake()
	{
		_lock = GetComponent<Utility.Lockable>();
	}

	/// <remarks>
	/// Lock <see cref="_lock"/>.
	/// </remarks>
	private void Start()
	{
		_lock.Lock();
	}
}

}