using UniRx;
using UnityEngine;

namespace KyleGame
{
	[RequireComponent(typeof(EnemyCharacterController))]
	public class EnemyMover : BaseEnemyComponent
	{
		private readonly BoolReactiveProperty _isRunning = new BoolReactiveProperty();

		private EnemyCharacterController _playerCharacterController;

		protected override void OnInitialize()
		{
			_playerCharacterController = GetComponent<EnemyCharacterController>();

			InputEventProvider.MoveDirection
				.Subscribe(x =>
				{
					var value = x.normalized * 10f;
					_playerCharacterController.Move(value);
				});

			InputEventProvider.MoveDirection
				.Subscribe(x => { _isRunning.Value = x.magnitude >= 0.1f; });
		}
	}
}