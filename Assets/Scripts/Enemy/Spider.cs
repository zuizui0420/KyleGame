using System;
using UniRx;
using UnityEngine;
using UnityEngine.AI;

public class Spider : StatefulEnemyComponentBase<Spider, SpiderState>
{
	private NavMeshAgent _agent;

	private Transform _playerTransform;

	[SerializeField]
	private SearchArea _searchArea;

	protected override void OnInitialize()
	{
		_agent = GetComponent<NavMeshAgent>();

		_playerTransform = GameObject.Find("Player").transform;

		StateList.Add(new StateWander(this));
		StateList.Add(new StatePursuit(this));

		StateMachine = new StateMachine<Spider>();

		ChangeState(SpiderState.Wander);
	}

	private class StateWander : State<Spider>
	{
		private IDisposable _subscription;

		public StateWander(Spider owner) : base(owner)
		{
		}

		public override void Enter()
		{
			_subscription = Owner._searchArea.OnPlayerInAreaEvent
				.Where(x => x.CompareTag("Player"))
				.TakeUntilDestroy(Owner)
				.Subscribe(_ => { Owner.ChangeState(SpiderState.Pursuit); });
		}

		public override void Execute()
		{
			base.Execute();
		}

		public override void Exit()
		{
			_subscription.Dispose();
			_subscription = null;
		}
	}

	private class StatePursuit : State<Spider>
	{
		public StatePursuit(Spider owner) : base(owner)
		{
		}

		public override void Enter()
		{
			base.Enter();
		}

		public override void Execute()
		{
			base.Execute();
		}

		public override void Exit()
		{
			base.Exit();
		}
	}

	private class StateExplosion : State<Spider>
	{
		public StateExplosion(Spider owner) : base(owner)
		{
		}

		public override void Enter()
		{
			base.Enter();
		}

		public override void Execute()
		{
			base.Execute();
		}

		public override void Exit()
		{
			base.Exit();
		}
	}
}

public enum SpiderState
{
	Wander,
	Pursuit,
	Explode
}