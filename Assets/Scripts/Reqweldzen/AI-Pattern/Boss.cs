using System;
using System.Collections;
using System.Linq;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Zenject;

namespace KyleGame
{
	public partial class Boss : StatefulWalker<Boss, BossState>
	{
		private readonly Subject<Unit> _callMobEventSubject = new Subject<Unit>();

		[SerializeField]
		private BossBarrier _bossBarrier;

		private BossAnimatorControl _bossAnimatorControl;

		private ElectricDamageable _electricDamageable;

		[Inject]
		private PlayerSystem _player;

		private Transform _playerTransform;

		public IObservable<Unit> CallMobEvent
		{
			get { return _callMobEventSubject; }
		}

		protected override Transform PlayerTransform
		{
			get { return _playerTransform; }
		}

		protected override void OnInitialize()
		{
			base.OnInitialize();

			_playerTransform = GameObject.Find("Player").transform;

			_bossAnimatorControl = GetComponent<BossAnimatorControl>();
			_electricDamageable = GetComponent<ElectricDamageable>();

			WeaponSetup();
			EventSubscribe();

			StateList.Add(new StateIdle(this));
			StateList.Add(new StatePursuit(this));
			StateList.Add(new StateLaser(this));
			StateList.Add(new StateSummonEnemy(this));
			StateList.Add(new StateTackle(this));
			StateList.Add(new StateFreeze(this));
			StateList.Add(new StateDead(this));
			StateMachine = new StateMachine<Boss>();
			ChangeState(BossState.Pursuit);
		}

		private void EventSubscribe()
		{
			_electricDamageable.DamageEvent
				.Where(_ => !(StateMachine.CurrentState is StateFreeze))
				.TakeUntilDestroy(this).Subscribe(_ =>
			{
				ChangeState(BossState.Freeze);
			});
		}

		private void WeaponSetup()
		{
			var orbIsBroken = Observable.Merge(FindObjectsOfType<EnergyOrb>().Select(x => x.IsOrbBroken));

			// バリアが消えたら硬直させる
			_bossBarrier.IsBrokeEvent.TakeUntilDestroy(this).Subscribe(_ =>
			{
				this.OnCollisionEnterAsObservable().Where(x => x.collider.CompareTag(TAGNAME.TAG_ELECTRICWIRE));

				ChangeState(BossState.Freeze);
			});

			// 2つめの時にスパークを表示
			var barrierHalfBroken = orbIsBroken.Skip(1).FirstOrDefault().Do(_ =>
			{
				Debug.Log("2 break.");
				_bossBarrier.HalfBroken();
			});

			// 4つめの時にバリアを消す
			var barrierBroken = orbIsBroken.Skip(3).FirstOrDefault().Do(_ =>
			{
				Debug.Log("4 break.");
				_bossBarrier.Broke();
			});

			Observable.Merge(barrierHalfBroken, barrierBroken).TakeUntilDestroy(this).Subscribe();
		}

		private IEnumerator SummonCoroutine()
		{
			yield return null;
		}

		private void OnDestroy()
		{
			_callMobEventSubject.OnCompleted();
		}
	}

	public enum BossState
	{
		Idle,
		Pursuit,
		Laser,
		SummonEnemy,
		Tackle,
		Freeze,
		Dead
	}
}