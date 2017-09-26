using UniRx;
using UnityEngine;

namespace KyleGame
{
	[RequireComponent(typeof(PlayerCharacterController))]
	public class PlayerMover : BasePlayerComponent
	{
		private readonly BoolReactiveProperty _isRunning = new BoolReactiveProperty();

		private PlayerCharacterController _playerCharacterController;

		protected override void OnInitialize()
		{
			_playerCharacterController = GetComponent<PlayerCharacterController>();

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