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
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace See
{

/// <summary>
/// Direct the gameplay and instruct the player.
/// </summary>
[AddComponentMenu("Seequencer/Gameplay/Director")]
public class Director : Utility.Singleton<Director>
{
	/// <summary>
	/// Sequencer used by the director.
	/// </summary>
	[SerializeField]
	private Sequencer _sequencer = null;

	[Header("Messages")]

	/// <summary>
	/// TextMeshPro component for messages.
	/// </summary>
	public TMPro.TextMeshPro messageText = null;

	/// <summary>
	/// Duration of a message (in seconds).
	/// </summary>
	[Range(1, 3)]
	public const int messageDuration = 2;

	[Header("Events")]

	/// <summary>
	/// Event invoked if the player is correct.
	/// </summary>
	[SerializeField]
	private UnityEvent OnCorrect;

	/// <summary>
	/// Event invoked if the player is wrong.
	/// </summary>
	[SerializeField]
	private UnityEvent OnWrong;

	/// <summary>
	/// Event invoked if the game is over.
	/// </summary>
	[SerializeField]
	private UnityEvent OnGameOver;

	/// <summary>
	/// The current stage (counting upwards from one).
	/// </summary>
	private uint _stage = 0;

	/// <summary>
	/// Guesses remaining until the game is over.
	/// </summary>
	private uint _lives = 0;

	/// <summary>
	/// Display a message for a duration (in seconds).
	/// </summary>
	/// <param name="message">Message to display.</param>
	/// <param name="duration">Duration of the message (in seconds).</param>
	private IEnumerator Message(string message, float duration)
	{
		messageText.SetText(message);

		yield return new WaitForSeconds(duration);

		messageText.SetText("");
	}

	/// <summary>
	/// Display a flashed message for a count (in seconds).
	/// </summary>
	/// <param name="message">Message to display.</param>
	/// <param name="count">Count of flashes (in seconds).</param>
	private IEnumerator Flash(string message, int count)
	{
		for (; count > 0; count--)
		{
			yield return Message(message, 0.5f);
			yield return new WaitForSeconds(0.5f);
		}
	}

	/// <summary>
	/// Display a countdown as messages.
	/// </summary>
	/// <param name="count">Count of the countdown (in seconds).</param>
	private IEnumerator Countdown(int count)
	{
		for (; count > 0; count--)
		{
			yield return Message(count.ToString(), 1.0f);
		}
	}

	/// <summary>
	/// Check if the current stage is a high score.
	/// </summary>
	private IEnumerator Score()
	{
		string sceneName = SceneManager.GetActiveScene().name;
		int highscore = PlayerPrefs.GetInt(sceneName);

		if (_stage > highscore)
		{
			yield return Flash("HIGH SCORE", messageDuration);

			PlayerPrefs.SetInt(sceneName, (int)_stage);
		}

		yield return Message($"STAGE {_stage}", messageDuration);
	}

	/// <summary>
	/// Set up the initial state of a new game.
	/// </summary>
	private void NewGame()
	{
		_stage = 1;
		_lives = 3;
	}

	/// <summary>
	/// Instruct the player(s) that a new stage has 
	/// </summary>
	private IEnumerator Stage()
	{
		yield return Message($"STAGE {_stage}", messageDuration);
	}

	/// <summary>
	/// Method invoked if the player is correct.
	/// </summary>
	/// <returns></returns>
	private IEnumerator Correct()
	{
		_stage++;

		yield return Flash("CORRECT", messageDuration);

		OnCorrect?.Invoke();
	}

	/// <summary>
	/// Method invoked if the player is wrong.
	/// </summary>
	private IEnumerator Wrong()
	{
		_lives--;

		yield return Flash("WRONG", messageDuration);

		if (_lives == 1)
		{
			yield return Flash("LAST CHANCE", messageDuration);
		}

		OnWrong?.Invoke();
	}

	/// <summary>
	/// Method invoked if the game is over.
	/// </summary>
	private IEnumerator GameOver()
	{
		yield return Flash("GAME OVER", messageDuration);
		yield return Score();

		OnGameOver?.Invoke();
	}

	/// <summary>
	/// Direct the gameplay.
	/// </summary>
	private IEnumerator Direct()
	{
		bool result = false;
		
	 	NewGame();

		do
		{
			yield return Stage();

			_sequencer.Generate(_stage);

			do
			{
				yield return Countdown(3);

				yield return _sequencer.Play();
				yield return _sequencer.Record(callback => result = callback);

				if (result)
				{
					yield return Correct();
					break;
				}
				else
				{
					yield return Wrong();
				}
			}
			while (_lives > 0);
		}
		while (result);

		yield return GameOver();
	}

	/// <remarks>
	/// Invoke <see cref="Direct"/>.
	/// </remarks>
	private void Start()
	{
		StartCoroutine(Direct());
	}
}

}