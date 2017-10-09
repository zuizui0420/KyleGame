using KyleGame;
using UniRx;
using UnityEngine;

namespace Reqweldzen.InputEventProvider
{
	public class KeyboardInputProvider : MonoBehaviour, IInputEventProvider
	{
		private readonly BoolReactiveProperty _fire1Button = new BoolReactiveProperty();
		private readonly BoolReactiveProperty _fire2Button = new BoolReactiveProperty();
		private readonly BoolReactiveProperty _leftTriggerButton = new BoolReactiveProperty();
		private readonly BoolReactiveProperty _rightTriggerButton = new BoolReactiveProperty();
		private readonly Vector3ReactiveProperty _moveDirection = new Vector3ReactiveProperty();
		private readonly Vector3ReactiveProperty _cameraRotation = new Vector3ReactiveProperty();

		public IReadOnlyReactiveProperty<bool> Fire1Button { get { return _fire1Button; } }
		public IReadOnlyReactiveProperty<bool> Fire2Button { get { return _fire2Button; } }
		public IReadOnlyReactiveProperty<bool> LeftTriggerButton { get { return _leftTriggerButton; } }
		public IReadOnlyReactiveProperty<bool> RightTriggerButton { get { return _rightTriggerButton; } }
		public IReadOnlyReactiveProperty<Vector3> MoveDirection { get { return _moveDirection; } }
		public IReadOnlyReactiveProperty<Vector3> CameraRotation { get { return _cameraRotation; } }
	}
}