using UnityEngine;

public class Drone : StatefulEnemyComponentBase<Drone, DroneState>
{
	private Transform _playerTransform;

	protected override void OnInitialize()
	{
		_playerTransform = GameObject.Find("Player").transform;

		StateMachine = new StateMachine<Drone>();

		ChangeState(DroneState.Wander);
	}

	private class StateWander : State<Drone>
	{
		public StateWander(Drone owner) : base(owner)
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

public enum DroneState
{
	Wander,
	Pursuit,
	Attack
}