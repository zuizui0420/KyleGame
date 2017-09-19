using UnityEngine;

namespace KyleGame
{
	/// <inheritdoc />
	/// <summary>
	///     The turret.
	/// </summary>
	public class Turret : StatefulEnemyComponentBase<Turret, TurretState>
	{
		/// <summary>
		///     The censor distance.
		/// </summary>
		[Space]
		[SerializeField]
		private float _censorDistance = 5;

		/// <summary>
		///     Rotation head.
		/// </summary>
		[SerializeField]
		private Transform _head;

		private Transform _playerTransform;

		/// <inheritdoc />
		protected override void OnInitialize()
		{
			_playerTransform = GameObject.Find("Player").transform;

			StateList.Add(new StateIdle(this));
			StateList.Add(new StateAttack(this));

			StateMachine = new StateMachine<Turret>();

			ChangeState(TurretState.Idle);
		}

		/// <summary>
		///     The state attack.
		/// </summary>
		private class StateAttack : State<Turret>
		{
			public StateAttack(Turret owner)
				: base(owner)
			{
			}

			public override void Execute()
			{
				var distance = Vector3.Distance(Owner.transform.position, Owner._playerTransform.position);

				if (!(distance < Owner._censorDistance))
				{
					Owner.ChangeState(TurretState.Idle);
					return;
				}

				var direction = Owner._playerTransform.position - Owner.transform.position;
				direction.y = 0;

				var rookRotation = Quaternion.LookRotation(direction);
				Owner._head.rotation = Quaternion.Slerp(Owner._head.rotation, rookRotation, 3f * Time.deltaTime);
			}
		}

		/// <summary>
		///     The state idle.
		/// </summary>
		private class StateIdle : State<Turret>
		{
			public StateIdle(Turret owner)
				: base(owner)
			{
			}

			public override void Execute()
			{
				var distance = Vector3.Distance(Owner.transform.position, Owner._playerTransform.position);

				if (distance < Owner._censorDistance)
				{
					Owner.ChangeState(TurretState.Attack);
					return;
				}

				Owner._head.rotation = Quaternion.Slerp(
					Owner._head.rotation,
					Owner.transform.rotation,
					3f * Time.deltaTime);
			}
		}
	}
}