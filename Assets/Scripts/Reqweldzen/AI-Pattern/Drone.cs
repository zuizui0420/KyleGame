using System.Collections;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace KyleGame
{
	public partial class Drone : StatefulEnemyComponentBase<Drone, DroneState>
	{
		/// <summary>
		/// 巡回ポイント
		/// </summary>
		[SerializeField]
		private Transform[] _destinationAnchorList;

		private Transform _playerTransform;

		private EnemyCharacterController _characterController;

		private DroneAnimation _droneAnimation;

		private int _anchorNum;

		protected override void OnInitialize()
		{
			_playerTransform = GameObject.Find("Player").transform;
			_droneAnimation = GetComponent<DroneAnimation>();
			_characterController = GetComponent<EnemyCharacterController>();

			_destination.TakeUntilDestroy(this)
				.Subscribe(x =>
				{
					var orig = transform.rotation;
					var direction = x - transform.position;
					direction.y = 0;
					var dest = Quaternion.LookRotation(direction);

					transform.rotation = dest/*Quaternion.Slerp(orig, dest, Time.deltaTime)*/;
				});
			_movementSpeed.TakeUntilDestroy(this)
				.Subscribe(x => _droneAnimation.Speed = x);

			StateList.Add(new StateWander(this));
			StateList.Add(new StatePursuit(this));
			StateList.Add(new StateAttack(this));
			StateList.Add(new StateReturn(this));

			StateMachine = new StateMachine<Drone>();

			ChangeState(DroneState.Wander);
		}

		private Vector3 NextDestination()
		{
			var ret = _destinationAnchorList[_anchorNum].position;
			_anchorNum = ++_anchorNum >= _destinationAnchorList.Length ? 0 : _anchorNum;

			return ret;
		}
	}
}
