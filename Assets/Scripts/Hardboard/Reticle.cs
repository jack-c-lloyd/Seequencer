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

namespace See.Hardboard
{
    /// <summary>
    /// Based on the reticle-pointer from the Google Cardboard XR Plugin for Unity.
    /// </summary>
    /// <remarks>
    /// <b>Reference</b>: 
    /// <see href="https://github.com/googlevr/cardboard-xr-plugin"/>
    /// </remarks>
    [AddComponentMenu("Seequencer/Interaction/Reticle")]
    [RequireComponent(typeof(SkinnedMeshRenderer))]
    public class Reticle : MonoBehaviour
    {
        /// <summary>
        /// Projector used by the reticle.
        /// </summary>
        private Projector _projector;

        /// <summary>
        /// Used for the arrow projection.
        /// </summary>
        [SerializeField]
        private Interaction.Interactor _interactor = null;

        /// <summary>
        /// Skinned mesh used by the reticle.
        /// </summary>
        private SkinnedMeshRenderer _renderer = null;

        /// <summary>
        /// Index of the <see cref="BlendShape"/> to close the reticle.
        /// </summary>
        private const int _BLENDSHAPE_INDEX = 0;

        /// <summary>
        /// Update the mesh based on the reticle properties.
        /// </summary>
        private void UpdateWeights()
        {
            float weight = _interactor.Current?.Percentage ?? 0.0f;

            _renderer.SetBlendShapeWeight(_BLENDSHAPE_INDEX, weight);
        }

        /// <remarks>
        /// Get <see cref="_renderer"/>.
        /// </remarks>
        private void Awake()
        {
            if (!TryGetComponent(out _renderer))
            {
                Debug.LogError($"{nameof(_renderer)} is null.");
            }

            _projector = new(_renderer);
        }

        /// <remarks>
        /// Refer to <see cref="UpdateWeights"/>.
        /// </remarks>
        private void LateUpdate()
        {
            float distance = _interactor.Distance;
            bool expand = _interactor.Current != null;

            _projector.SetParams(distance, expand);
            _projector.UpdateDiameters();

            UpdateWeights();
        }
    }
}
