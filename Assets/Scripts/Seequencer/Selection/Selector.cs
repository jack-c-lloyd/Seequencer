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
using UnityEngine.Events;

namespace See
{
    /// <summary>
    /// A <see cref="Selector"/> may select one <see cref="Selectable"/> from a set
    /// of attached children.
    /// </summary>
    [AddComponentMenu("Seequencer/Selection/Selector")]
    public class Selector : MonoBehaviour, Utility.ISubject<Selectable>
    {
        /// <summary>
        /// The selected <see cref="Selectable"/>, if not <c>null</c>.
        /// </summary>
        [SerializeField]
        private Selectable _selected = null;

        /// <summary>
        /// Disallow reselecton of a selected <see cref="Selectable"/>.
        /// </summary>
        [SerializeField]
        private bool _disallowReselection = true;

        /// <summary>
        /// Event invoked if a <see cref="Selectable"/> is selected.
        /// </summary>
        [SerializeField]
        private UnityEvent OnSelect = null;

        /// <summary>
        /// Event invoked if a <see cref="Selectable"/> is not selected.
        /// </summary>
        [SerializeField]
        private UnityEvent OnDeselect = null;

        /// <summary>
        /// Set of <see cref="Selectable"/> children.
        /// </summary>
        private HashSet<Selectable> _children = new();

        /// <summary>
        /// Public-safe access to <see cref="_children"/>.
        /// </summary>
        public IReadOnlyCollection<Selectable> Observers => _children;

        /// <summary>
        /// Attach a selectable.
        /// </summary>
        /// <param name="selectable">The selectable to attach.</param>
        /// <returns>
        /// <c>true</c> if <c>selectable</c> is attached, otherwise <c>false</c>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException"/>
        public bool Attach(Selectable selectable)
        {
            if (selectable == null)
            {
                throw new System.ArgumentNullException(nameof(selectable));
            }

            return _children.Add(selectable);
        }

        /// <summary>
        /// Detach a selectable.
        /// </summary>
        /// <param name="selectable">The selectable to detach.</param>
        /// <returns>
        /// <c>true</c> if <c>selectable</c> is detached, otherwise <c>false</c>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException"/>
        public bool Detach(Selectable selectable)
        {
            if (selectable == null)
            {
                throw new System.ArgumentNullException(nameof(selectable));
            }

            return _children.Remove(selectable);
        }

        /// <summary>
        /// Select a <see cref="Selectable"/> child, if it is an attached.
        /// </summary>
        /// <param name="selectable">The <see cref="Selectable"/> to select.</param>
        public void Select(Selectable selectable)
        {
            if (selectable == null)
            {
                Debug.LogWarning($"{nameof(selectable)} is null.");
                return;
            }

            if (!_children.Contains(selectable))
            {
                Debug.LogWarning($"{nameof(selectable)} is not attached.");
                return;
            }

            if (selectable == _selected && _disallowReselection)
            {
                Debug.LogWarning($"{nameof(selectable)} is selected.");
                return;
            }

            if (selectable != _selected)
            {
                Deselect();
                _selected = selectable;
            }

            _selected.Selected();
            OnSelect?.Invoke();
        }

        /// <summary>
        /// Deselect if <see cref="_selected"/> is not <c>null</c>.
        /// </summary>
        public void Deselect()
        {
            if (_selected != null)
            {
                _selected.Deselected();
                _selected = null;

                OnDeselect?.Invoke();
            }
        }

        /// <summary>
        /// Set up the initial state of the <see cref="Selectable"/> children.
        /// </summary>
        /// <remarks>
        /// <b>Warning</b>:
        /// must be called in <see cref="Start"/>.
        /// </remarks>
        private void Setup()
        {
            if (_selected != null && _children.Contains(_selected))
            {
                Select(_selected);
            }
            else
            {
                OnDeselect?.Invoke();
            }

            foreach (Selectable selectable in _children)
            {
                if (selectable == _selected)
                {
                    selectable.Selected();
                }
                else
                {
                    selectable.Deselected();
                }
            }
        }

        /// <remarks>
        /// <b>Warning</b>:
        /// must call <see cref="Setup"/>.
        /// </remarks>
        private void Start()
        {
            Setup();
        }
    }
}
