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
using UnityEngine;
using UnityEngine.Assertions.Must;

namespace See.Gameplay
{

/// <summary>
/// Played and recorded by a sequencer.
/// </summary>
[AddComponentMenu("Seequencer/Gameplay/Pad")]
[RequireComponent(typeof(Animator), typeof(Utility.Breaker))]
public class Pad : MonoBehaviour, Utility.IObserver<Sequencer>
{
	/// <summary>
	/// The sequencer to be contained by.
	/// </summary>
	[SerializeField]
	private Sequencer _sequencer = null;

	/// <summary>
	/// Public-safe access to <see cref="_sequencer"/>.
	/// </summary>
	public Sequencer Subject => _sequencer;

	/// <summary>
	/// The animator
	/// </summary>
	private Animator _animator = null;

	/// <summary>
	/// Breaker for enabling and disabling interactions.
	/// </summary>
	private Utility.Breaker _breaker = null;

	/// <summary>
	/// A note from the Seequencer theme.
	/// </summary>
	[SerializeField]
	private Sequencer.Note _note = Sequencer.Note.C2;

	/// <summary>
	/// Public-safe access to <see cref="_note"/>.
	/// </summary>
	public Sequencer.Note Note => _note;

	/// <summary>
	/// Minimum duration of a note (in seconds).
	/// </summary>
	private const float _DURATION_MIN = 1.0f;

	/// <summary>
	/// Maximum duration of a note (in seconds).
	/// </summary>
	private const float _DURATION_MAX = 3.0f;

	/// <summary>
	/// Duration of a note (in seconds).
	/// </summary>
	[Range(_DURATION_MIN, _DURATION_MAX)]
	[SerializeField]
	private float duration = 3.0f;

	/// <summary>
	/// Name of the off state.
	/// </summary>
	private static string _STATE_NAME_OFF = "Off";

	/// <summary>
	/// Name of the on state.
	/// </summary>
	private static string _STATE_NAME_ON = "On";

	/// <summary>
	/// ID of the off state.
	/// </summary>
	private static int _STATE_ID_OFF = Animator.StringToHash(_STATE_NAME_OFF);

	/// <summary>
	/// ID of the on state.
	/// </summary>
	private static int _STATE_ID_ON = Animator.StringToHash(_STATE_NAME_ON);

	/// <remarks>
	/// Get the required components.
	/// </remarks>
	private void Awake()
	{
		_animator = GetComponent<Animator>();

		Debug.Assert(_animator != null);
		Debug.Assert(_animator.HasState(0, _STATE_ID_OFF));
		Debug.Assert(_animator.HasState(0, _STATE_ID_ON));

		_breaker = GetComponent<Utility.Breaker>();
	}

	/// <summary>
	/// Play the pad via the attach animator.
	/// </summary>
	public IEnumerator Play()
	{
		if (_animator.GetCurrentAnimatorStateInfo(0).IsName(_STATE_NAME_OFF))
		{
			_breaker.Open();

			_animator.speed = 1.0f / duration;
			_animator.SetTrigger("Play");

			if (_sequencer != null)
			{
				AudioClip clip = _sequencer.Clip(_note);
				AudioSource.PlayClipAtPoint(clip, transform.position);
			}

			yield return new WaitWhile(() =>
				_animator.GetNextAnimatorStateInfo(0).IsName(_STATE_NAME_ON));
			yield return new WaitWhile(() =>
				_animator.GetCurrentAnimatorStateInfo(0).IsName(_STATE_NAME_ON));

			_breaker.Close();
		}
	}

	/// <summary>
	/// Method called to notify the sequencer that a pad has been pressed.
	/// </summary>
	public void Press()
	{
		if (_sequencer != null)
		{
			_sequencer.Pressed(this);
		}
	}

	/// <summary>
	/// <b>Warning</b>:
	/// must call <see cref="Sequencer.Attach"/>.
	/// </summary>
	public void OnEnable()
	{
		if (_sequencer != null && !_sequencer.Attach(this))
		{
			Debug.LogError($"{this} could not attach to {_sequencer}.");
		}
	}

	/// <summary>
	/// <b>Warning</b>:
	/// must call <see cref="Sequencer.Detach"/>.
	/// </summary>
	public void OnDisable()
	{
		if (_sequencer != null && !_sequencer.Detach(this))
		{
			Debug.LogError($"{this} could not detach from {_sequencer}.");
		}
	}
}

}
