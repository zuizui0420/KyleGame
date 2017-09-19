using System.Collections;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace KyleGame
{
	public partial class Drone : StatefulEnemyComponentBase<Drone, DroneState>
	{
		/// <summary>
		/// ドローン本体
		/// </summary>
		[SerializeField]
		private Transform _body;

		/// <summary>
		/// 巡回ポイント
		/// </summary>
		[SerializeField]
		private Transform[] _destinationAnchorList;

		/// <summary>
		/// Enemy body height
		/// </summary>
		[SerializeField]
		private float _height;

		private Transform _playerTransform;

		private CharacterController _characterController;

		private int _anchorNum;
		
		protected override void OnInitialize()
		{
			_playerTransform = GameObject.Find("Player").transform;
			_characterController = GetComponent<CharacterController>();

			StateList.Add(new StateWander(this));
			StateList.Add(new StatePursuit(this));
			StateList.Add(new StateAttack(this));
			StateList.Add(new StateReturn(this));

			StateMachine = new StateMachine<Drone>();

			ChangeState(DroneState.Wander);
		}
	}
}
