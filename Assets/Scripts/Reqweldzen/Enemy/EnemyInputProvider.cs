using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace KyleGame
{
	public class EnemyInputProvider : BaseEnemyComponent, IInputEventProvider
	{
		private readonly Vector3ReactiveProperty _cameraRotationReactiveProperty = new Vector3ReactiveProperty();
		private readonly BoolReactiveProperty _leftTriggerButtonReactiveProperty = new BoolReactiveProperty();
		private readonly Vector3ReactiveProperty _moveDirectionReactiveProperty = new Vector3ReactiveProperty();
		private readonly BoolReactiveProperty _rightTriggerButtonReactiveProperty = new BoolReactiveProperty();

		public IReadOnlyReactiveProperty<bool> Fire1Button { get; private set; }
		public IReadOnlyReactiveProperty<bool> Fire2Button { get; private set; }

		public IReadOnlyReactiveProperty<bool> LeftTriggerButton
		{
			get { return _leftTriggerButtonReactiveProperty; }
		}

		public IReadOnlyReactiveProperty<bool> RightTriggerButton
		{
			get { return _rightTriggerButtonReactiveProperty; }
		}

		public IReadOnlyReactiveProperty<Vector3> MoveDirection
		{
			get { return _moveDirectionReactiveProperty; }
		}

		public IReadOnlyReactiveProperty<Vector3> CameraRotation
		{
			get { return _cameraRotationReactiveProperty; }
		}

		protected override void OnInitialize()
		{
			this.UpdateAsObservable()
				.Select(_ => Input.GetButton("Fire2"))
				.Subscribe(x => _leftTriggerButtonReactiveProperty.Value = x);

			this.UpdateAsObservable()
				.Select(_ => Input.GetButton("Fire1"))
				.Subscribe(x => _rightTriggerButtonReactiveProperty.Value = x);

			this.UpdateAsObservable()
				.Select(_ => new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")))
				.Subscribe(x => _moveDirectionReactiveProperty.SetValueAndForceNotify(x));
		}
	}
}