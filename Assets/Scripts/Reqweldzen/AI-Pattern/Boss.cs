using System;
using System.Collections;
using System.Linq;
using UniRx;
using UnityEngine;
using Zenject;

namespace KyleGame
{
	public partial class Boss : StatefulWalker<Boss, BossState>
	{
		private readonly Subject<Unit> _callMobEventSubject = new Subject<Unit>();

		[SerializeField]
		private BossBarrier _bossBarrier;

		/// <summary>
		/// 
		/// </summary>
		private bool _isBarrierBroken;

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

			_playerTransform = _player.transform;

			WeaponSetup();

			StateList.Add(new StateIdle(this));
			StateList.Add(new StatePursuit(this));
			StateList.Add(new StateTackle(this));
			StateList.Add(new StateFreeze(this));
			StateList.Add(new StateDead(this));
			StateMachine = new StateMachine<Boss>();
			ChangeState(BossState.Pursuit);
		}

		private void WeaponSetup()
		{
			var orbIsBroken = FindObjectsOfType<EnergyOrb>().Select(x => x.IsOrbBroken).Merge();

			// バリアが消えたら硬直させる
			_bossBarrier.IsBrokeEvent.TakeUntilDestroy(this).Subscribe(_ => ChangeState(BossState.Freeze));

			// 2つめの時にスパークを表示
			var barrierHalfBroken = orbIsBroken.Skip(1).FirstOrDefault().Do(_ => { _bossBarrier.HalfBroken(); });

			// 4つめの時にバリアを消す
			var barrierBroken = orbIsBroken.Skip(3).FirstOrDefault().Do(_ => { _bossBarrier.Broke(); });

			barrierHalfBroken.Merge(barrierBroken).TakeUntilDestroy(this).Subscribe();
		}

		private IEnumerator LaserCoroutine()
		{
			yield return null;
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
		Tackle,
		Freeze,
		Dead
	}
}