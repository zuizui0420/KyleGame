using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace Assets.Scripts
{
	public class EnemyCharacterController : BaseEnemyComponent
	{
		private readonly BoolReactiveProperty _isGrounded = new BoolReactiveProperty(true);
		private CharacterController _controller;

		private Vector3 _inputDirection;

		private Rigidbody _rigidbody;

		public IReadOnlyReactiveProperty<bool> IsGrounded
		{
			get { return _isGrounded; }
		}

		public void Move(Vector3 velocity)
		{
			_inputDirection = velocity;
		}

		public void Jump(float power)
		{
			ApplyForce(Vector3.up * power);
		}

		public void Stop()
		{
			_rigidbody.velocity = Vector3.zero;
			_inputDirection = Vector3.zero;
		}

		public void ApplyForce(Vector3 force)
		{
			Observable.NextFrame(FrameCountType.FixedUpdate)
				.Subscribe(_ => _rigidbody.AddForce(force, ForceMode.VelocityChange));
		}

		protected override void OnInitialize()
		{
			_rigidbody = GetComponent<Rigidbody>();

			this.FixedUpdateAsObservable()
				.Subscribe(_ =>
				{
					_rigidbody.AddForce(_inputDirection, ForceMode.Acceleration);
					_isGrounded.Value = CheckGrounded();
				});
		}

		private bool CheckGrounded()
		{
			var ray = new Ray(transform.position + Vector3.up * 0.8f, Vector3.down);
			var result = Physics.SphereCast(ray, 0.68f, 1.0f);

			return result;
		}
	}
}