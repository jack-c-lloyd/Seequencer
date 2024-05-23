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

namespace See.Controllers
{

    /// <summary>
    /// Controller of scene loading.
    /// </summary>
    [AddComponentMenu("Seequencer/Controllers/SceneController")]
    public class SceneController : Utility.Singleton<SceneController>
    {
        /// <summary>
        /// Delay between preloading and loading a scene (in seconds).
        /// </summary>
        [SerializeField]
        private float _delay = 1.0f;

        /// <summary>
        /// Scene indexes based on the build settings.
        /// </summary>
        [SerializeField]
        public enum SceneIndex
        {
            /// <summary>
            /// Home.
            /// </summary>
            Home,

            /// <summary>
            /// Easy Mode.
            /// </summary>
            Easy,

            /// <summary>
            /// Normal Mode.
            /// </summary>
            Normal,

            /// <summary>
            /// Hard Mode.
            /// </summary>
            Hard
        }

        /// <summary>
        /// Current index of the scene to load.
        /// </summary>
        [SerializeField]
        private SceneIndex _sceneIndex = SceneIndex.Home;

        /// <summary>
        /// Event invoked before a scene is loaded.
        /// </summary>
        [SerializeField]
        private UnityEvent OnPreloaded = null;

        /// <summary>
        /// Event invoked after a scene is loaded.
        /// </summary>
        [SerializeField]
        private UnityEvent OnLoaded = null;

        /// <summary>
        /// Load a scene by its build index with a predefined delay.
        /// </summary>
        /// <param name="sceneBuildIndex">Build index of the scene to load.</param>
        private IEnumerator LoadSceneDelay(int sceneBuildIndex)
        {
            AsyncOperation async = SceneManager.LoadSceneAsync(sceneBuildIndex);
            async.allowSceneActivation = false;

            OnPreloaded?.Invoke();

            yield return new WaitForSeconds(_delay);

            async.allowSceneActivation = true;
        }

        /// <summary>
        /// Method called to invoke <see cref="LoadSceneDelay(int)"/>.
        /// </summary>
        public void LoadSceneDelay()
        {
            StartCoroutine(LoadSceneDelay((int)_sceneIndex));
        }

        /// <summary>
        /// Set the scene index for Home.
        /// </summary>
        public void SetHome()
        {
            _sceneIndex = SceneIndex.Home;
        }

        /// <summary>
        /// Load the Home scene.
        /// </summary>
        public void LoadHome()
        {
            SetHome();
            LoadSceneDelay();
        }

        /// <summary>
        /// Set the scene index for easy mode.
        /// </summary>
        public void SetEasy()
        {
            _sceneIndex = SceneIndex.Easy;
        }

        /// <summary>
        /// Load the scene for easy mode.
        /// </summary>
        public void LoadEasy()
        {
            SetEasy();
            LoadSceneDelay();
        }

        /// <summary>
        /// Set the scene index for normal mode.
        /// </summary>
        public void SetNormal()
        {
            _sceneIndex = SceneIndex.Normal;
        }

        /// <summary>
        /// Load the scene for normal mode.
        /// </summary>
        public void LoadNormal()
        {
            SetNormal();
            LoadSceneDelay();
        }

        /// <summary>
        /// Set the scene index for hard mode.
        /// </summary>
        public void SetHard()
        {
            _sceneIndex = SceneIndex.Hard;
        }

        /// <summary>
        /// Load the scene for hard mode.
        /// </summary>
        public void LoadHard()
        {
            SetHard();
            LoadSceneDelay();
        }

        /// <summary>
        /// Method called via <see cref="SceneManager.sceneLoaded"/>.
        /// </summary>
        /// <param name="scene">The loaded scene.</param>
        /// <param name="mode">The load mode of the scene.</param>
        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            OnLoaded?.Invoke();
        }

        /// <remarks>
        /// Add the delegate to <see cref="SceneManager.sceneLoaded"/>.
        /// </remarks>
        void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        /// <remarks>
        /// Remove the delegate to <see cref="SceneManager.sceneLoaded"/>.
        /// </remarks>
        void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

}
