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
				MovementSpeed = 3f;
				Agent.isStopped = false;
				Owner._bossAnimatorControl.MovementSpeed = 0.2f;

				// 追跡座標を10Fごとに更新
				Observable.IntervalFrame(10)
					.Subscribe(_ => UpdateDestination())
					.AddTo(_compositeDisposable);

				// プレイヤーが近い場合すぐに行動する
				Owner.UpdateAsObservable()
					.Where(_ => GetDistance(Player) <= RelativeDistance)
					.FirstOrDefault()
					.Subscribe(_ => Owner.ChangeState(BossState.Laser))
					.AddTo(_compositeDisposable);

				// 最長5秒後に行動を決定する
				Observable.Timer(TimeSpan.FromSeconds(5))
					.TakeUntilDestroy(Owner)
					.Subscribe(_ =>
					{
						Owner._bossAnimatorControl.MovementSpeed = 0;

						// タックル
						if (GetDistance(Player) > 5f)
						{
							Owner.ChangeState(BossState.Tackle);
							return;
						}


						Owner.ChangeState(BossState.Laser);
					})
					.AddTo(_compositeDisposable);
			}

			private void UpdateDestination()
			{
				var relativePos = GetDirection(Player) * RelativeDistance * -1;
				Destination = Player.position + relativePos;
			}

			public override void Exit()
			{
				_compositeDisposable.Clear();
				MovementSpeed = 0;
			}
		}

		private class StateLaser : WalkerStateBase<Boss>
		{
			private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

			public StateLaser(Boss owner) : base(owner)
			{
			}

			public override void Enter()
			{
				Owner._bossAnimatorControl.Beam().TakeUntilDestroy(Owner).Subscribe(_ =>
					{
						Owner.ChangeState(BossState.Pursuit);
					})
					.AddTo(_compositeDisposable);
			}

			public override void Exit()
			{
				_compositeDisposable.Clear();
			}
		}

		private class StateSummonEnemy : WalkerStateBase<Boss>
		{
			public StateSummonEnemy(Boss owner) : base(owner)
			{
			}

			public override void Enter()
			{
				Observable.Timer(TimeSpan.FromSeconds(3))
					.TakeUntilDestroy(Owner)
					.Subscribe(_ => { Owner.ChangeState(BossState.Pursuit); });
			}
		}

		private class StateTackle : WalkerStateBase<Boss>
		{
			private const float RelativeDistance = 1.5f;

			private readonly BossAnimatorControl _animatorControl;

			private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

			public StateTackle(Boss owner) : base(owner)
			{
				_animatorControl = owner._bossAnimatorControl;
			}

			public override void Enter()
			{
				_animatorControl.MovementSpeed = 0;
				Agent.ResetPath();

				Observable.FromCoroutine(Tackle)
					.TakeUntilDestroy(Owner)
					.Subscribe(_ => Owner.ChangeState(BossState.Pursuit))
					.AddTo(_compositeDisposable);
			}

			public override void Exit()
			{
				_animatorControl.TackleRelease();
				_compositeDisposable.Clear();
			}

			private bool _isHit;

			private void Hit()
			{
				_isHit = true;
			}

			// 電気タックル
			private IEnumerator Tackle(CancellationToken token)
			{
				var direction = GetDirection(Player);

				yield return _animatorControl.TackleReady().ToYieldInstruction();

				yield return new WaitForSeconds(0.5f);

				var startTime = Time.timeSinceLevelLoad;

				_isHit = false;
				Owner.OnCollisionEnterAsObservable().Where(x => x.collider.GetComponent<PlayerSystem>()).FirstOrDefault().Subscribe(_ => Hit()).AddTo(_compositeDisposable);

				while (true)
				{
					var time = Time.timeSinceLevelLoad - startTime;
					if (time >= 1f) break;
					if (_isHit) break;

					Agent.Move(direction * 8f * Time.deltaTime);
					yield return null;
				}

				_animatorControl.TackleRelease();

				yield return new WaitForSeconds(1f);
			}
		}

		private class StateFreeze : WalkerStateBase<Boss>
		{
			public StateFreeze(Boss owner) : base(owner)
			{
			}

			public override void Enter()
			{
				MovementSpeed = 0f;
				Owner._bossAnimatorControl.Damage(5).Subscribe(_ =>
					{
						Owner.ChangeState(BossState.Pursuit);
					})
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
				MovementSpeed = 0f;
				Agent.isStopped = true;

				Owner._bossAnimatorControl.Death();
			}
		}
	}
}