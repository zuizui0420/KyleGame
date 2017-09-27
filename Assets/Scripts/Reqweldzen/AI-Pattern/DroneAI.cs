using System;
using System.Collections;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace KyleGame
{
	public partial class Drone
	{
		/// <summary>
		///     ステート：巡回
		/// </summary>
		private class StateWander : WalkerStateBase<Drone>
		{
			private readonly CompositeDisposable _disposableList = new CompositeDisposable();


			public StateWander(Drone owner) : base(owner)
			{
				ReactionAngle = owner.ReactionAngle;
				ReactionDistance = owner.ReactionDistance;
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

				Observable.FromCoroutine(token => WandererCoroutine(WaitCoroutine, token)).TakeUntilDestroy(Owner).Subscribe().AddTo(_disposableList);
			}

			private IEnumerator WaitCoroutine()
			{
				yield return new WaitForSeconds(2f);
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

				// 3s毎に攻撃
				Observable.Return(Unit.Default)
					.Delay(TimeSpan.FromSeconds(3))
					.Subscribe(_ =>
					{
						Agent.isStopped = true;
						Owner.ChangeState(DroneState.Attack);
					})
					.AddTo(_compositeDisposable);

				// 追跡開始位置から一定距離離れたら追跡中断
				Owner.UpdateAsObservable()
					.Where(_ => GetDistance(Origin) >= MaxDemarcationDistance)
					.Subscribe(_ =>
					{
						Owner.ChangeState(DroneState.Return);
					})
					.AddTo(_compositeDisposable);

				// 追跡座標を10Fごとに更新
				Observable.IntervalFrame(10, FrameCountType.FixedUpdate)
					.Subscribe(_ =>
					{
						var relativePos = GetDirection(Player) * RelativeDistance * -1;
						Destination = Player.position + relativePos;
					})
					.AddTo(_compositeDisposable);
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
					.Subscribe(_ =>
					{
						Agent.isStopped = false;
						Owner.ChangeState(DroneState.Pursuit);
					});
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

			private IEnumerator WandererCoroutine(CancellationToken token)
			{
				Destination = Origin;

				while (!token.IsCancellationRequested)
				{
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