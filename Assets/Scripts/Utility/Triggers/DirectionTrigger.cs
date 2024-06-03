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

namespace Utility
{
    /// <summary>
    /// Invoke events by comparing the dot product of two forward-vectors to a
    /// threshold, if it is greater than or less than, or they are equal.
    /// </summary>
    [AddComponentMenu("Utility/DirectionTrigger")]
    public class DirectionTrigger : MonoBehaviour
    {
        /// <summary>
        /// The left-hand side of the dot operation.
        /// </summary>
        [SerializeField]
        private Transform _leftHandSide = null;

        /// <summary>
        /// The right-hand side of the dot operation.
        /// </summary>
        [SerializeField]
        private Transform _rightHandSide = null;

        /// <summary>
        /// Compared to the dot product to invoke events. 
        /// </summary>
        [Range(-1.0f, 1.0f)]
        [SerializeField]
        private float _threshold = 0.0f;

        /// <summary>
        /// Event invoked if the dot product is equal to the threshold.
        /// </summary>
        public UnityEvent OnEqual = null;

        /// <summary>
        /// Event invoked if the dot product is greater than the threshold.
        /// </summary>
        public UnityEvent OnGreaterThan = null;

        /// <summary>
        /// Event invoked if the dot product is less than the theshold.
        /// </summary>
        public UnityEvent OnLessThan = null;

        /// <summary>
        /// Internal states.
        /// </summary>
        private enum State
        {
            /// <summary>
            /// Default state.
            /// </summary>
            NIL,

            /// <summary>
            /// Equal state.
            /// </summary>
            EQ,

            /// <summary>
            /// Greater-than state.
            /// </summary>
            GT,

            /// <summary>
            /// Less-than state.
            /// </summary>
            LT
        }

        /// <summary>
        /// Internal state.
        /// </summary>
        private State _state = State.NIL;

        /// <summary>
        /// Compare the dot product to the threshold and invoke events if the state
        /// is not the same as the internal state.
        /// </summary>
        private void Compare()
        {
            if (_leftHandSide == null || _rightHandSide == null)
            {
                _state = State.NIL;
                return;
            }

            float dot = Vector3.Dot(_leftHandSide.forward, _rightHandSide.forward);

            if (dot > _threshold)
            {
                if (_state != State.GT)
                {
                    _state = State.GT;

                    OnGreaterThan?.Invoke();
                }
            }
            else if (dot < _threshold)
            {
                if (_state != State.LT)
                {
                    _state = State.LT;

                    OnLessThan?.Invoke();
                }
            }
            else // dot == threshold
            {
                if (_state != State.EQ)
                {
                    _state = State.EQ;

                    OnEqual?.Invoke();
                }
            }
        }

        /// <remarks>
        /// Must call <see cref="Compare"/>.
        /// </remarks>
        void LateUpdate()
        {
            Compare();
        }
    }
}
