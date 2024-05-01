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

namespace See
{

/// <summary>
/// A <see cref="Pad"/> is played and recorded by a <see cref="Sequencer"/>.
/// </summary>
[AddComponentMenu("Seequencer/Gameplay/Pad")]
[RequireComponent(typeof(Animator), typeof(Utility.Lockable))]
public class Pad : MonoBehaviour, Utility.IChild<Sequencer>
{
	/// <summary>
	/// The <see cref="Sequencer"/> parent.
	/// </summary>
	[SerializeField]
	private Sequencer _parent = null;

	/// <summary>
	/// Public-safe access to <see cref="_parent"/>.
	/// </summary>
	public Sequencer Parent => _parent;

	/// <summary>
	/// 
	/// </summary>
	private Animator _animator = null;

	/// <summary>
	/// Lock for enabling and disabling interactions.
	/// </summary>
	/// <remarks>
	/// <see cref="Utility.Lockable._parent"/> should be <see cref="_parent"/>.
	/// <remarks>
	private Utility.Lockable _lock = null;

	/// <summary>
	/// A note from the Seequencer theme.
	/// </summary>
	[SerializeField]
	private AudioController.Note _note = AudioController.Note.C2;

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
	/// Get the attached <see cref="Animator"/> and <see cref="Utility.Lockable"/>.
	/// </remarks>
	private void Awake()
	{
		_animator = GetComponent<Animator>();

		Debug.Assert(_animator != null);
		Debug.Assert(_animator.HasState(0, _STATE_ID_OFF));
		Debug.Assert(_animator.HasState(0, _STATE_ID_ON));

		_lock = GetComponent<Utility.Lockable>();
	}

	/// <remarks>
	/// Set the audio clip as per <see cref="_note"/> on the attached source.
	/// </remarks>
	private void Start()
	{
		// Ewww... horrible hack, refactor this!
		if (TryGetComponent(out AudioSource audioSource))
		{
			audioSource.clip = AudioController.Instance.GetAudioClip(_note);
		}
	}

	/// <summary>
	/// Play the pad via the attach <see cref="Animator"/>.
	/// </summary>
	public IEnumerator Play()
	{
		if (_animator.GetCurrentAnimatorStateInfo(0).IsName(_STATE_NAME_OFF))
		{
			_lock.Lock();

			_animator.speed = 1.0f / duration;
			_animator.SetTrigger("Play");

			yield return new WaitWhile(() =>
				_animator.GetNextAnimatorStateInfo(0).IsName(_STATE_NAME_ON));
			yield return new WaitWhile(() =>
				_animator.GetCurrentAnimatorStateInfo(0).IsName(_STATE_NAME_ON));

			_lock.Unlock();
		}
	}

	/// <summary>
	/// Method called to notify the parent <see cref="Sequencer"/>.
	/// </summary>
	public void Press()
	{
		if (_parent != null)
		{
			_parent.Pressed(this);
		}
	}

	/// <summary>
	/// Must call <see cref="Sequencer.Attach"/>.
	/// </summary>
	public void OnEnable()
	{
		if (_parent != null && !_parent.Attach(this))
		{
			Debug.LogError($"{this} could not attach to {_parent}.");
		}
	}

	/// <summary>
	/// Must call <see cref="Sequencer.Detach"/>.
	/// </summary>
	public void OnDisable()
	{
		if (_parent != null && !_parent.Detach(this))
		{
			Debug.LogError($"{this} could not detach from {_parent}.");
		}
	}
}

}