using UniRx;
using UnityEngine;

namespace KyleGame
{
	public class EnemyMover : BaseEnemyComponent
	{
		private readonly BoolReactiveProperty _isRunning = new BoolReactiveProperty();

		protected override void OnInitialize()
		{
			InputEventProvider.MoveDirection
				.Subscribe(x =>
				{
					var value = x.normalized * 10f;
					//_playerCharacterController.Move(value);
				});

			InputEventProvider.MoveDirection
				.Subscribe(x => { _isRunning.Value = x.magnitude >= 0.1f; });
		}
	}
}