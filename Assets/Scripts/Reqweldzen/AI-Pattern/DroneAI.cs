using System;
using System.Collections;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace KyleGame
{
	public partial class Drone
	{
		private readonly Vector3ReactiveProperty _destination = new Vector3ReactiveProperty();
		private readonly FloatReactiveProperty _movementSpeed = new FloatReactiveProperty();
		
		/// <summary>
		///     ステート：巡回
		/// </summary>
		private class StateWander : WalkerStateBase<Drone>
		{
			private readonly CompositeDisposable _disposableList = new CompositeDisposable();


			public StateWander(Drone owner) : base(owner)
			{
			}

			public override void Enter()
			{
				MovementSpeed = 1.5f;

				ObserveStart();
			}

			public override void Exit()
			{
				_disposableList.Clear();
			}

			private void ObserveStart()
			{
				// プレイヤーが視界に入ったら追跡
				Owner.UpdateAsObservable()
					.Where(_ => OnObjectReflectedInOwnerEyes())
					.TakeUntilDestroy(Owner)
					.Subscribe(_ =>
					{
						Origin = Self.position;
						Owner.ChangeState(DroneState.Pursuit);
					})
					.AddTo(_disposableList);

				Observable.FromCoroutine(WandererCoroutine).TakeUntilDestroy(Owner).Subscribe().AddTo(_disposableList);
			}
		}

		/// <summary>
		///     ステート：追跡
		/// </summary>
		private class StatePursuit : WalkerStateBase<Drone>
		{
			private const float MaxDemarcationDistance = 15f;
			private const float RelativeDistance = 3f;

			private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

			public StatePursuit(Drone owner) : base(owner)
			{
			}

			public override void Enter()
			{
				MovementSpeed = 3f;

				Observable.Return(Unit.Default)
					.Delay(TimeSpan.FromSeconds(3))
					.Subscribe(_ => { Owner.ChangeState(DroneState.Attack); })
					.AddTo(_compositeDisposable);
			}

			public override void Execute()
			{
				if (GetDistance(Origin) >= MaxDemarcationDistance)
				{
					Owner.ChangeState(DroneState.Return);
				}
				else if (!(GetDistance(Player) < RelativeDistance))
				{
					Destination = Player.position;

					var moveDir = GetDirection(Destination) * MovementSpeed;

					Move(moveDir);
				}
			}

			public override void Exit()
			{
				_compositeDisposable.Clear();
			}
		}

		/// <summary>
		///     ステート：攻撃
		/// </summary>
		private class StateAttack : WalkerStateBase<Drone>
		{
			public StateAttack(Drone owner) : base(owner)
			{
			}

			public override void Enter()
			{
				Debug.Log("Attack");
				Observable.NextFrame()
					.Subscribe(_ => Owner.ChangeState(DroneState.Pursuit));
			}
		}

		/// <summary>
		///     ステート：帰投
		/// </summary>
		private class StateReturn : WalkerStateBase<Drone>
		{
			public StateReturn(Drone owner) : base(owner)
			{
			}

			public override void Enter()
			{
				Observable.FromCoroutine(WandererCoroutine)
					.TakeUntilDestroy(Owner)
					.Subscribe(_ => { Owner.ChangeState(DroneState.Wander); });
			}

			private new IEnumerator WandererCoroutine(CancellationToken token)
			{
				Destination = Origin;

				while (!token.IsCancellationRequested)
				{
					var moveDir = GetDirection(Destination) * MovementSpeed;

					Move(moveDir);

					if (GetDistance(Destination) < 1.0f)
						break;

					yield return null;
				}
			}
		}
	}

	public enum DroneState
	{
		Wander,
		Pursuit,
		Attack,
		Return
	}
}