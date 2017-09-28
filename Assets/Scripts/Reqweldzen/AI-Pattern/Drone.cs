using UniRx;
using UnityEngine;

namespace KyleGame
{
	public partial class Drone : StatefulWalker<Drone, DroneState>
	{
		private Enemy_Drone _droneAnimation;

		private Transform _playerTransform;

		protected override Transform PlayerTransform
		{
			get { return _playerTransform; }
		}

		protected override void OnInitialize()
		{
			base.OnInitialize();

			_playerTransform = GameObject.Find("Player").transform;
			_droneAnimation = GetComponent<Enemy_Drone>();

			MovementSpeed.TakeUntilDestroy(this)
				.Subscribe(x => _droneAnimation.Speed = x);

			StateList.Add(new StateWander(this));
			StateList.Add(new StatePursuit(this));
			StateList.Add(new StateAttack(this));
			StateList.Add(new StateReturn(this));

			StateMachine = new StateMachine<Drone>();

			ChangeState(DroneState.Wander);
		}
	}
}