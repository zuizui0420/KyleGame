using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace KyleGame
{
	[RequireComponent(typeof(CharacterController))]
	public class EnemyCharacterController : BaseEnemyComponent
	{
		private readonly BoolReactiveProperty _isGrounded = new BoolReactiveProperty(true);

		private CharacterController _characterController;

		private readonly float _gravityScale = 20f;

		private Vector3 _inputDirection;

		private readonly float _jumpPower = 8f;

		public IReadOnlyReactiveProperty<bool> IsGrounded
		{
			get { return _isGrounded; }
		}

		public void Move(Vector3 velocity)
		{
			velocity.y = _inputDirection.y;
			_inputDirection = velocity;
		}

		public void Jump()
		{
			_inputDirection.y = _jumpPower;
		}

		public void Stop()
		{
			_inputDirection = Vector3.zero;
		}

		protected override void OnInitialize()
		{
			_characterController = GetComponent<CharacterController>();

			this.UpdateAsObservable()
				.Subscribe(_ =>
				{
					_inputDirection.y -= _gravityScale * Time.deltaTime;

					_characterController.Move(_inputDirection * Time.deltaTime);
					_isGrounded.Value = _characterController.isGrounded;
				});
		}
	}
}