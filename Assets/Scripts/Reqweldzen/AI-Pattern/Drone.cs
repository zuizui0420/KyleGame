using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using KyleGame.ViewModel;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Zenject;

namespace KyleGame
{
	public partial class Drone : StatefulWalker<Drone, DroneState>
	{
		[SerializeField]
		private GameObject _bulletPrefab;

		private DroneAnimation _droneAnimation;

		[SerializeField]
		private bool _isIdleMode;

		private Transform _playerTransform;

		[SerializeField, Header("弾丸を生成する座標")]
		private Transform[] _shotPointList;

		[SerializeField, Header("Damage Area")]
		private GameObject _damageArea;

		protected override Transform PlayerTransform
		{
			get { return _playerTransform; }
		}

		protected override void OnInitialize()
		{
			base.OnInitialize();

			_playerTransform = GameObject.Find("Player").transform;

			_droneAnimation = GetComponent<DroneAnimation>();

			_playerTransform = GameObject.Find("Player").transform;

			if (_shotPointList == null || !_shotPointList.Any())
			{
				_shotPointList = new[] {transform};
;			}

			MovementSpeed.TakeUntilDestroy(this)
				.Subscribe(x => _droneAnimation.Speed = x);

			StateList.Add(new StateWander(this));
			StateList.Add(new StateIdle(this));
			StateList.Add(new StatePursuit(this));
			StateList.Add(new StateAttack(this));
			StateList.Add(new StateReturn(this));
			StateList.Add(new StateExplode(this));

			StateMachine = new StateMachine<Drone>();

			ChangeState(_isIdleMode ? DroneState.Idle : DroneState.Wander);
		}

		private IEnumerator ShotBullet(CancellationToken token)
		{
			var duration = 3;
			var burstRate = 200;
			var burstCount = 3f;
			var shotIntervalTime = 200;

			for (var j = 0; j < duration; j++)
			{
				for (var i = 0; i < burstCount; i++)
				{
					foreach (var shotPoint in _shotPointList)
					{
						var bullet = Instantiate(_bulletPrefab, shotPoint.position, shotPoint.rotation);

						bullet.OnTriggerEnterAsObservable().AsUnitObservable()
							.Merge(Observable.Timer(TimeSpan.FromSeconds(2)).AsUnitObservable())
							.TakeUntilDestroy(this)
							.Take(1)
							.Subscribe(_ =>
							{
								Observable.NextFrame()
									.Subscribe(__ => Destroy(bullet));

							});
					}

					yield return this.Wait(TimeSpan.FromMilliseconds(burstRate)).ToYieldInstruction();
				}

				yield return this.Wait(TimeSpan.FromMilliseconds(shotIntervalTime)).ToYieldInstruction();
			}
		}

		public void Dead()
		{
			ChangeState(DroneState.Explode);
		}
	}
}