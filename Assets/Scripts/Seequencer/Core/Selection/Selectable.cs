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

namespace See.Selection
{

    /// <summary>
    /// A <see cref="Selectable"/> may be selected by its <see cref="Selector"/>.
    /// </summary>
    [AddComponentMenu("Seequencer/Selection/Selectable")]
    public class Selectable : MonoBehaviour, Utility.IObserver<Selector>
    {
        /// <summary>
        /// The <see cref="Selector"/>.
        /// </summary>
        [SerializeField]
        private Selector _selector = null;

        /// <summary>
        /// Public-safe access to <see cref="_selector"/>.
        /// </summary>
        public Selector Subject => _selector;

        /// <summary>
        /// Event invoked if selected by a <see cref="Selector"/>.
        /// </summary>
        [SerializeField]
        private UnityEvent OnSelect = null;

        /// <summary>
        /// Event invoked if deselected by a <see cref="Selector"/>.
        /// </summary>
        [SerializeField]
        private UnityEvent OnDeselect = null;

        /// <summary>
        /// Method called to invoke the event <see cref="OnSelect"/>.
        /// </summary>
        /// <remarks>
        /// Should be called by a <see cref="Selector"/>.
        /// </remarks>
        public void Selected()
        {
            OnSelect?.Invoke();
        }

        /// <summary>
        /// Method called to invoke the event <see cref="OnDeselect"/>.
        /// </summary>
        /// <remarks>
        /// Should be called by a <see cref="Selector"/>.
        /// </remarks>
        public void Deselected()
        {
            OnDeselect?.Invoke();
        }

        /// <summary>
        /// Method called to select the <see cref="Selectable"/>.
        /// </summary>
        public void Select()
        {
            if (_selector != null)
            {
                _selector.Select(this);
            }
        }

        /// <remarks>
        /// <b>Warning</b>:
        /// must call <see cref="Selector.Attach"/>.
        /// </remarks>
        public void OnEnable()
        {
            if (_selector != null && !_selector.Attach(this))
            {
                Debug.LogError($"{this} could not attach to {_selector}.");
            }
        }

        /// <remarks>
        /// <b>Warning</b>:
        /// must call <see cref="Selector.Detach"/>.
        /// </remarks>
        public void OnDisable()
        {
            if (_selector != null && !_selector.Detach(this))
            {
                Debug.LogError($"{this} could not detach from {_selector}.");
            }
        }
    }

}
