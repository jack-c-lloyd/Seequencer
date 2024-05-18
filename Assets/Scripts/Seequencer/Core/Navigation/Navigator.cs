

using UnityEngine;
using UnityEngine.Events;

[AddComponentMenu("Seequencer/Navigation/Navigator")]
public class Navigator : MonoBehaviour
{
	[SerializeField]
	private Transform _root = null;

	[SerializeField]
	private Transform _target = null;

	[Range(-1.0f, 1.0f)]
	[SerializeField]
	private float _threshold = 0.0f;

	[SerializeField]
	private UnityEvent<bool> OnChange = null;

	private bool _state = false;

	/// <summary>
	/// Set the target.
	/// </summary>
	/// <param name="target">Transform to target.</param>
	public void SetTarget(Transform target)
	{
		_target = target;
	}

	/// <summary>
	/// Reset the target.
	/// </summary>
	public void ResetTarget()
	{
		_target = null;
	}

	/// <summary>
	/// Project the target onto a plane a rotate towards it.
	/// </summary>
	private void Rotate()
	{
		Vector3 projection = Vector3.ProjectOnPlane(_target.position, _root.forward);
		Vector3 inverseProjection = Quaternion.Inverse(_root.rotation) * projection;

		float angle = Mathf.Atan2(inverseProjection.y, inverseProjection.x) * Mathf.Rad2Deg;

		transform.localRotation = Quaternion.AngleAxis(angle, Vector3.forward);
	}

	private void LateUpdate()
	{
		if (_target == null)
		{
			OnChange.Invoke(false);
			return;
		}

		Vector3 direction = (_target.position - _root.position).normalized;
		bool result = Vector3.Dot(_root.forward, direction) < _threshold;

		OnChange.Invoke(result);

		if (result)
		{
			Rotate();
		}
	}
}
