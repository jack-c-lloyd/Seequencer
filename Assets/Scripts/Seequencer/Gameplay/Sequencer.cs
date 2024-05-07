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
/// Contains pads, for which it can generate, play, and record sequences.
/// </summary>
[AddComponentMenu("Seequencer/Gameplay/Sequencer")]
[RequireComponent(typeof(Utility.Switchable))]
public class Sequencer : MonoBehaviour, Utility.ISubject<Pad>
{
	/// <summary>
	/// List of pads, maintained as a set.
	/// </summary>
	private List<Pad> _pads = new();

	/// <summary>
	/// Public-safe access to <see cref="_pads"/>.
	/// </summary>
	public IReadOnlyCollection<Pad> Observers => _pads;

	/// <summary>
	/// Generated sequence of pads.
	/// </summary>
	private List<Pad> _sequence = new();

	/// <summary>
	/// Switch for recording mode.
	/// </summary>
	private Utility.Switchable _switch = null;

	/// <summary>
	/// <c>true</c> if it is recording, otherwise <c>false</c>.
	/// </summary>
	private bool _isRecording = false;

	/// <summary>
	/// Index of the current pad in the sequence to record.
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
	/// Attach a pad.
	/// </summary>
	/// <param name="pad">The pad to attach.</param>
	/// <returns>
	/// <c>true</c> if <c>pad</c> is attached, otherwise <c>false</c>.
	/// </returns>
	/// <exception cref="System.ArgumentNullException"></exception>
	public bool Attach(Pad pad)
	{
		if (pad == null)
		{
			throw new System.ArgumentNullException(nameof(pad));
		}

		if (!_pads.Contains(pad))
		{
			_pads.Add(pad);
		}

		return true;
	}

	/// <summary>
	/// Detach a pad.
	/// </summary>
	/// <param name="pad">The pad to detach.</param>
	/// <returns>
	/// <c>true</c> if <c>pad</c> is detached, otherwise <c>false</c>.
	/// </returns>
	/// <exception cref="System.ArgumentNullException"></exception>
	public bool Detach(Pad pad)
	{
		if (pad == null)
		{
			throw new System.ArgumentNullException(nameof(pad));
		}

		_pads.Remove(pad);
		_sequence.RemoveAll(attached => attached == pad);

		return true;
	}

	/// <summary>
	/// Generate a random sequence with the specified count.
	/// </summary>
	/// <param name="count">A positive integer.</param>
	public void Generate(uint count)
	{
		_sequence.Clear();

		if (_pads.Count == 0)
		{
			Debug.LogError($"{_pads} is empty.");
		}

		while (count-- > 0)
		{
			int index = Random.Range(0, _pads.Count);
			_sequence.Add(_pads[index]);
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

			_switch.Close();

			yield return new WaitWhile(() => _isRecording);

			_switch.Open();

			callback?.Invoke(_recordingState == State.CORRECT);
		}
	}

	/// <summary>
	/// Method called by a pad if it has been pressed by a player, and if in
	/// recording mode, then it should be compared to the sequence.
	/// </summary>
	/// <param name="pad">The pad that has been pressed.</param>
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
	/// Get the lock.
	/// </remarks>
	private void Awake()
	{
		_switch = GetComponent<Utility.Switchable>();
	}

	/// <remarks>
	/// Should lock itself.
	/// </remarks>
	private void Start()
	{
		_switch.Open();
	}
}

}