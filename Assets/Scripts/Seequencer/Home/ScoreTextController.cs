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
using UnityEngine.Events;

namespace See.Home
{

    /// <summary>
    /// Invokes an event with a formatted string containing the score of a scene.
    /// </summary>
    /// <remarks>
    /// Used to set the score text in the Home scene.
    /// </remarks>
    [AddComponentMenu("Seequencer/Home/ScoreTextController")]
    public class ScoreTextController : MonoBehaviour
    {
        /// <summary>
        /// Build index of a scene for which to get the score.
        /// </summary>
        public Controllers.SceneController.SceneIndex sceneIndex;

        /// <summary>
        /// Prefix of the score text.
        /// </summary>
        [SerializeField]
        private string _prefix = "SCORE: ";

        /// <summary>
        /// Event invoked on <see cref="Awake"/>.
        /// </summary>
        public UnityEvent<string> OnAwake = null;

        /// <remarks>
        /// Invoke <see cref="OnAwake"/> with the formatted string.
        /// </remarks>
        private void Awake()
        {
            string name = System.Enum.GetName(typeof(Controllers.SceneController.SceneIndex),
                (int)sceneIndex);

            int score = PlayerPrefs.GetInt(name, 0);
            string formatted = $"{_prefix}{score}";

            OnAwake?.Invoke(formatted);
        }
    }

}
