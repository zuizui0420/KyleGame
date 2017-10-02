using System;
using System.Collections;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace KyleGame
{
	public partial class Boss
	{
		private class StateIdle : State<Boss>
		{
			public StateIdle(Boss owner) : base(owner)
			{
			}
		}

		private class StatePursuit : WalkerStateBase<Boss>
		{
			private const float RelativeDistance = 1.5f;

			private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

			public StatePursuit(Boss owner) : base(owner)
			{
			}

			public override void Enter()
			{
				MovementSpeed = 1f;

				// 追跡座標を10Fごとに更新
				Observable.IntervalFrame(10 /*, FrameCountType.FixedUpdate*/)
					.Subscribe(_ =>
					{
						var relativePos = GetDirection(Player) * RelativeDistance * -1;
						Destination = Player.position + relativePos;
					})
					.AddTo(_compositeDisposable);

				// プレイヤーが近い場合すぐに行動する
				Owner.UpdateAsObservable()
					.Where(_ => GetDistance(Player) <= RelativeDistance)
					.FirstOrDefault()
					.Do(_ => Owner.ChangeState(BossState.Tackle))
					.Subscribe()
					.AddTo(_compositeDisposable);

				// 最長5秒後に行動を決定する
				Observable.Timer(TimeSpan.FromSeconds(3))
					.TakeUntilDestroy(Owner)
					.Subscribe(_ =>
					{
						// タックル
						if (GetDistance(Player) > 5f)
							Owner.ChangeState(BossState.Tackle);
					})
					.AddTo(_compositeDisposable);
			}

			public override void Exit()
			{
				_compositeDisposable.Clear();
			}
		}

		private class StateCallEnemy : WalkerStateBase<Boss>
		{
			public StateCallEnemy(Boss owner) : base(owner)
			{
			}

			public override void Enter()
			{
				Observable.Timer(TimeSpan.FromSeconds(3))
					.TakeUntilDestroy(Owner)
					.Subscribe(_ => { Owner.ChangeState(BossState.Pursuit); });
			}
		}

		private class StateLaser : WalkerStateBase<Boss>
		{
			public StateLaser(Boss owner) : base(owner)
			{
			}

			public override void Enter()
			{
				Observable.FromCoroutine(Owner.LaserCoroutine).TakeUntilDestroy(Owner).Subscribe(_ =>
				{
					Owner.ChangeState(BossState.Pursuit);
				});
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

		private class StateSummonEnemy : WalkerStateBase<Boss>
		{
			public StateSummonEnemy(Boss owner) : base(owner)
			{
			}

			public override void Enter()
			{
				
			}
		}

		private class StateTackle : WalkerStateBase<Boss>
		{
			private const float RelativeDistance = 1.5f;

			public StateTackle(Boss owner) : base(owner)
			{
			}

			public override void Enter()
			{
				Observable.FromCoroutine(_ => Tackle())
					.TakeUntilDestroy(Owner)
					.Subscribe(_ => Owner.ChangeState(BossState.Pursuit));
			}

			// 電気タックル
			private IEnumerator Tackle()
			{
				Agent.isStopped = true;
				MovementSpeed = 20f;

				var relativePos = GetDirection(Player) * RelativeDistance * -1;
				Destination = Player.position + relativePos;

				yield return new WaitForSeconds(0.5f);


				Agent.isStopped = false;

				yield return new WaitForSeconds(2f);

				Agent.isStopped = true;

				yield return new WaitForSeconds(0.5f);

				Agent.isStopped = false;
			}
		}

		private class StateFreeze : WalkerStateBase<Boss>
		{
			public StateFreeze(Boss owner) : base(owner)
			{
			}

			public override void Enter()
			{
				Observable.Timer(TimeSpan.FromSeconds(5))
					.Subscribe(_ => { Owner.ChangeState(BossState.Pursuit); })
					.AddTo(Owner);
			}
		}

		private class StateDead : WalkerStateBase<Boss>
		{
			public StateDead(Boss owner) : base(owner)
			{
			}

			public override void Enter()
			{
			}
		}
	}
}