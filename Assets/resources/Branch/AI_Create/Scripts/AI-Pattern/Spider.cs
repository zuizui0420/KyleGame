using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.AI;

namespace KyleGame
{
	public partial class Spider : StatefulEnemyComponentBase<Spider, SpiderState>
	{
		private NavMeshAgent _agent;

		private Transform _playerTransform;

		[SerializeField] private Transform[] _destinationAnchorList;

		private int _anchorNum;

		protected override void OnInitialize()
		{
			_agent = GetComponent<NavMeshAgent>();

			_playerTransform = GameObject.Find("Player").transform;

			StateList.Add(new StateWander(this));
			StateList.Add(new StatePursuit(this));
			StateList.Add(new StateExplosion(this));

			StateMachine = new StateMachine<Spider>();

			ChangeState(SpiderState.Wander);
		}
	}

	public enum SpiderState
	{
		Wander,
		Pursuit,
		Explode
	}
}