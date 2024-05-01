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

namespace See
{

/// <summary>
/// Controller of audio-based resource management.
/// </summary>
[AddComponentMenu("Seequencer/Controllers/AudioController")]
public class AudioController : Utility.Singleton<AudioController>
{
	/// <summary>
	/// Notes of the Seequencer theme.
	/// </summary>
	public enum Note
	{
		/// <summary>
		/// Corresponds to the F2 note on a keyboard.
		/// </summary>
		F2,

		/// <summary>
		/// Corresponds to the E2 note on a keyboard.
		/// </summary>
		E2,

		/// <summary>
		/// Corresponds to the C#2 note on a keyboard.
		/// </summary>
		CS2,

		/// <summary>
		/// Corresponds to the C2 note on a keyboard.
		/// </summary>
		C2,

		/// <summary>
		/// Corresponds to the A1 note on a keyboard.
		/// </summary>
		A1,

		/// <summary>
		/// Corresponds to the G#1 note on a keyboard.
		/// </summary>
		GS1,

		/// <summary>
		/// Corresponds to the F1 note on a keyboard.
		/// </summary>
		F1,
		/// <summary>
		/// Corresponds to the E1 note on a keyboard.
		/// </summary>
		E1
	}

	/// <summary>
	/// An audio clip per <see cref="Note"/>.
	/// </summary>
	[SerializeField]
	private List<AudioClip> _notes = new List<AudioClip>(8);

	/// <summary>
	/// Get the audio clip for a <see cref="Note"/>.
	/// </summary>
	/// <param name="note">Which <see cref="Note"/> to get.</param>
	/// <returns>An <see cref="AudioClip"/> for <c>note</c>.</returns>
	public AudioClip GetAudioClip(Note note)
	{
		int index = (int)note;
		
		return _notes[index];
	}
}

}
