using System;
using System.Collections;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.AI;

namespace KyleGame
{
	public partial class Spider
	{
		/// <summary>
		///     ステート：巡回
		/// </summary>
		private class StateWander : WalkerStateBase<Spider>
		{
			private readonly CompositeDisposable _disposableList = new CompositeDisposable();


			public StateWander(Spider owner) : base(owner)
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
					.Subscribe(_ => {  })
					.AddTo(_disposableList);

				Owner.UpdateAsObservable()
					.Where(_ => OnObjectReflectedInOwnerEyes())
					.ThrottleFirst(TimeSpan.FromMilliseconds(50))
					.Where(_ => CanSetDestination())
					.TakeUntilDestroy(Owner)
					.Subscribe(_ =>
					{
						Owner.ChangeState(SpiderState.Pursuit);
					})
					.AddTo(_disposableList);

				Observable.FromCoroutine(token => WandererCoroutine(WaitCoroutine, token)).TakeUntilDestroy(Owner).Subscribe().AddTo(_disposableList);
			}

			private IEnumerator WaitCoroutine()
			{
				yield return new WaitForSeconds(4f);
			}

			private bool CanSetDestination()
			{
				var path = new NavMeshPath();
				return Agent.CalculatePath(Player.position, path);
			}
		}

		private class StateIdle : State<Spider>
		{
			public StateIdle(Spider owner) : base(owner)
			{
			}
		}

		/// <summary>
		///     ステート：追跡
		/// </summary>
		private class StatePursuit : WalkerStateBase<Spider>
		{
			private const float RelativeDistance = 2f;

			private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

			public StatePursuit(Spider owner) : base(owner)
			{
			}

			public override void Enter()
			{
				MovementSpeed = 5f;

				// 追跡座標を10Fごとに更新
				Observable.IntervalFrame(10, FrameCountType.FixedUpdate)
					.Subscribe(_ =>
					{
						Destination = Player.position;
					})
					.AddTo(_compositeDisposable);

				Owner.UpdateAsObservable()
					.Where(_ => GetDistance(Player) < RelativeDistance)
					.Take(1)
					.Subscribe(_ =>
					{
						Owner.ChangeState(SpiderState.Explode);
					})
					.AddTo(_compositeDisposable);

				if (Owner._spiderType == SpiderType.InstantSpark)
					Owner._spiderAnimation.Spark();
			}

			public override void Exit()
			{
				Agent.ResetPath();
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
				MovementSpeed = 0;

				switch (Owner._spiderType)
				{
					case SpiderType.InstantSpark:
						Owner._spiderAnimation.Suicide().Subscribe(_ =>
							{
								Owner._damageArea.SetActive(true);
							},
							() =>
							{
								Destroy(Owner.gameObject);
							});
						break;
					case SpiderType.TimerBomb:
						Owner._spiderAnimation.Spark();
						Owner._spiderAnimation.Suicide().Subscribe(_ =>
							{
								Owner._damageArea.SetActive(true);
							},
							() =>
							{
								Destroy(Owner.gameObject);
							});
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
		}
	}
}