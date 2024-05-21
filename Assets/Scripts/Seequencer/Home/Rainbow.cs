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

namespace See.Home
{

/// <summary>
/// Cycle through the colors of a rainbow.
/// </summary>
[AddComponentMenu("Seequencer/Home/Rainbow")]
public class Rainbow : MonoBehaviour
{
	/// <summary>
	/// The renderer with the material to set the color property.
	/// </summary>
	[SerializeField]
	private Renderer _renderer = null;

	/// <summary>
	/// The name for the color property of the material used by the renderer.
	/// </summary>
	[SerializeField]
	private string _property = "_Color";

	/// <summary>
	/// Index of the applied color.
	/// </summary>
	[SerializeField]
	private int _index = 0;

	/// <summary>
	/// The default color.
	/// </summary>
	[SerializeField]
	private Color _defaultColor = Color.white;

	/// <summary>
	/// The colors of the rainbow.
	/// </summary>
	[SerializeField]
	private List<Color> _colors = new List<Color>
	{
		Color.red,
		Color.yellow,
		Color.green,
		Color.cyan,
		Color.blue,
		Color.magenta
	};

	/// <summary>
	/// Iterate to the next color in the rainbow (circular).
	/// </summary>
	public void Cycle()
	{
		_index = (_index + 1) % _colors.Count;

		SetColor(_colors[_index]);
	}

	/// <summary>
	/// Set the color of the renderer's material.
	/// </summary>
	/// <param name="color">The color to set.</param>
	private void SetColor(Color color)
	{
		if (_renderer != null && _renderer.material != null)
		{
			_renderer.material.SetColor(_property, color);
		}
	}

	/// <remarks>
	/// Try to get <see cref="_renderer"/> if it is <c>null</c>.
	/// </remarks>
	private void Awake()
	{
		if (_renderer == null && !TryGetComponent(out _renderer))
		{
			Debug.LogWarning($"{nameof(_renderer)} is null.");
		}
	}

	/// <remarks>
	/// Set the color of the renderer's material to the current color.
	/// </remarks>
	private void OnEnable()
	{
		SetColor(_colors[_index]);
	}

	/// <remarks>
	/// Reset the color of the renderer's material to the default color.
	/// </remarks>
	private void OnDisable()
	{
		SetColor(_defaultColor);
	}
}

}
