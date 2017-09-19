using UniRx;
using UnityEngine;

namespace KyleGame
{
	public interface IInputEventProvider
	{
		IReadOnlyReactiveProperty<bool> LeftTriggerButton { get; }
		IReadOnlyReactiveProperty<bool> RightTriggerButton { get; }
		IReadOnlyReactiveProperty<Vector3> MoveDirection { get; }
		IReadOnlyReactiveProperty<Vector3> CameraRotation { get; }
	}
}