using System;
using System.Collections;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace KyleGame
{
	public partial class Spider
	{
		private readonly Vector3ReactiveProperty _destination = new Vector3ReactiveProperty();
		private readonly FloatReactiveProperty _movementSpeed = new FloatReactiveProperty();

		/// <summary>
		///     ステート：巡回
		/// </summary>
		private class StateWander : WalkerStateBase<Spider>
		{
			private readonly CompositeDisposable _disposableList = new CompositeDisposable();


			public StateWander(Spider owner) : base(owner)
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
					.Subscribe(_ => { Owner.ChangeState(SpiderState.Pursuit); })
					.AddTo(_disposableList);

				Observable.FromCoroutine(WandererCoroutine).TakeUntilDestroy(Owner).Subscribe().AddTo(_disposableList);
			}
		}

		/// <summary>
		///     ステート：追跡
		/// </summary>
		private class StatePursuit : WalkerStateBase<Spider>
		{
			public StatePursuit(Spider owner) : base(owner)
			{
			}

			public override void Enter()
			{
				MovementSpeed = 5f;
				if (Owner._spiderType == SpiderType.InstantSpark)
					Owner._spiderAnimation.Spark();
			}

			public override void Execute()
			{
				if (!(GetDistance(Player) < 2f))
				{
					Destination = Player.position;

					var moveDir = GetDirection(Destination) * MovementSpeed;

					Move(moveDir);
				}
				else
				{
					Owner.ChangeState(SpiderState.Explode);
				}
			}

			public override void Exit()
			{
				Stop();
			}
		}

		/// <summary>
		///     ステート：自爆
		/// </summary>
		private class StateExplosion : WalkerStateBase<Spider>
		{
			public StateExplosion(Spider owner) : base(owner)
			{
			}

			public override void Enter()
			{
				switch (Owner._spiderType)
				{
					case SpiderType.InstantSpark:
						Owner._spiderAnimation.Suicide();
						break;
					case SpiderType.TimerBomb:
						Owner._spiderAnimation.Spark();
						Owner._spiderAnimation.Suicide();
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
		}
	}
}