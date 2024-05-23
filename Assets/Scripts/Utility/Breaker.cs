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

namespace Utility
{

    /// <summary>
    /// Acts as a circuit breaker in a one-way tree.
    /// </summary>
    [AddComponentMenu("Utility/Breaker")]
    public class Breaker : MonoBehaviour, IObserver<Breaker>, ISubject<Breaker>
    {
        /// <summary>
        /// The parent breaker.
        /// </summary>
        [SerializeField]
        private Breaker _parent = null;

        /// <summary>
        /// Public-safe access to <see cref="_parent"/>.
        /// </summary>
        public Breaker Subject => _parent;

        /// <summary>
        /// Set of children.
        /// </summary>
        private HashSet<Breaker> _children = new();

        /// <summary>
        /// Public-safe access to <see cref="_children"/>.
        /// </summary>
        public IReadOnlyCollection<Breaker> Observers => _children;

        [Header("Close/Open")]

        /// <summary>
        /// <c>true</c> if is is closed, otherwise <c>false</c>.
        /// </summary>
        [SerializeField]
        private bool _isClosed = false;

        /// <summary>
        /// Public-safe access to <see cref="_isClosed"/>.
        /// </summary>
        public bool IsClosed => _isClosed;

        /// <summary>
        /// Event invoked if it is closed.
        /// </summary>
        [SerializeField]
        private UnityEvent OnClose = null;

        /// <summary>
        /// Event invoked if it is opened.
        /// </summary>
        [SerializeField]
        private UnityEvent OnOpen = null;

        [Header("Power")]

        /// <summary>
        /// <c>true</c> if is is powered, otherwise <c>false</c>.
        /// </summary>
        [SerializeField]
        private bool _isPowered = false;

        /// <summary>
        /// Public-safe access to <see cref="_isPowered"/>.
        /// </summary>
        public bool IsPowered => _isPowered;

        /// <summary>
        /// Event invoked if it is powered.
        /// </summary>
        [SerializeField]
        private UnityEvent OnPower = null;

        /// <summary>
        /// Event invoked if it is de-powered.
        /// </summary>
        [SerializeField]
        private UnityEvent OnDepower = null;

        /// <summary>
        /// Close the breaker, if and only if it is open.
        /// </summary>
        public void Close()
        {
            if (!_isClosed)
            {
                _isClosed = true;

                OnClose.Invoke();

                Notify();
            }
        }

        /// <summary>
        /// Open the breaker, if and only if it is closed.
        /// </summary>
        public void Open()
        {
            if (_isClosed)
            {
                _isClosed = false;

                OnOpen.Invoke();

                Notify();
            }
        }

        /// <summary>
        /// Power the breaker, if and only if it is not powered.
        /// </summary>
        private void Power()
        {
            if (!_isPowered)
            {
                _isPowered = true;

                OnPower?.Invoke();
            }
        }

        /// <summary>
        /// Depower the breaker, if and only if it is powered.
        /// </summary>
        private void Depower()
        {
            if (_isPowered)
            {
                _isPowered = false;

                OnDepower?.Invoke();
            }
        }

        /// <summary>
        /// Called if state has mutated in itself or up the tree.
        /// </summary>
        private void Notified()
        {
            if (_isClosed)
            {
                if (_parent == null || _parent.IsPowered)
                {
                    Power();
                }
                else
                {
                    Depower();
                }
            }
            else
            {
                Depower();
            }
        }

        /// <summary>
        /// Attach a child.
        /// </summary>
        /// <param name="dependent">The child to attach.</param>
        /// <returns>
        /// <c>true</c> if <c>child</c> is attached, otherwise <c>false</c>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public bool Attach(Breaker child)
        {
            if (child == null)
            {
                throw new System.ArgumentNullException(nameof(child));
            }

            return _children.Add(child);
        }

        /// <summary>
        /// Detach a child.
        /// </summary>
        /// <param name="dependent">The child to detach.</param>
        /// <returns>
        /// <c>true</c> if <c>child</c> is detached, otherwise <c>false</c>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public bool Detach(Breaker child)
        {
            if (child == null)
            {
                throw new System.ArgumentNullException(nameof(child));
            }

            return _children.Remove(child);
        }

        /// <summary>
        /// Notify <see cref="Observers"/>.
        /// </summary>
        private void Notify()
        {
            Notified();

            foreach (Breaker child in _children)
            {
                child.Notify();
            }
        }

        /// <remarks>
        /// <b>Warning</b>:
        /// must call <see cref="Notify"/>, if and only if it is the root.
        /// </remarks>
        private void Start()
        {
            if (Subject == null)
            {
                Notify();
            }
        }

        /// <remarks>
        /// <b>Warning</b>:
        /// must call <see cref="Attach"/>.
        /// </remarks>
        public void OnEnable()
        {
            if (Subject != null && !Subject.Attach(this))
            {
                Debug.LogError($"{this} could not attach to {Subject}.");
            }
        }

        /// <remarks>
        /// <b>Warning</b>:
        /// must call <see cref="Detach"/>.
        /// </remarks>
        public void OnDisable()
        {
            if (Subject != null && !Subject.Detach(this))
            {
                Debug.LogError($"{this} could not detach from {Subject}.");
            }
        }

        /// <remarks>
        /// Valid if there is not a circular dependency on itself.
        /// </remarks>
        private void OnValidate()
        {
            for (Breaker root = Subject; root != null; root = root.Subject)
            {
                if (root == this)
                {
                    Debug.LogError($"{this} has a circular dependency on itself.");
                    return;
                }
            }
        }
    }

}
