using System.Collections;
using UniRx;
using UnityEngine;

namespace KyleGame
{
	public partial class Drone : StatefulWalker<Drone, DroneState>
	{
		private Enemy_Drone _droneAnimation;

		private Transform _playerTransform;

		[SerializeField, Header("弾丸を生成する座標")]
		private Transform[] _shotPointList;

		[SerializeField, Header("弾丸オブジェクト")]
		private TurretBullet _bulletPrefab;

		protected override Transform PlayerTransform
		{
			get { return _playerTransform; }
		}

		protected override void OnInitialize()
		{
			base.OnInitialize();

			_playerTransform = GameObject.Find("Player").transform;
			_droneAnimation = GetComponent<Enemy_Drone>();

			MovementSpeed.TakeUntilDestroy(this)
				.Subscribe(x => _droneAnimation.Speed = x);

			StateList.Add(new StateWander(this));
			StateList.Add(new StatePursuit(this));
			StateList.Add(new StateAttack(this));
			StateList.Add(new StateReturn(this));

			StateMachine = new StateMachine<Drone>();

			ChangeState(DroneState.Wander);
		}

		private IEnumerator ShotBullet(CancellationToken token)
		{
			var attackDuration = 2f;
			var bulletRate = 0.2f;
			var bulletContinuity = 3f;
			var bulletInterval = 0.4f;

			int count = 0;

			var startTime = Time.timeSinceLevelLoad;

			while (!token.IsCancellationRequested)
			{
				var diff = Time.timeSinceLevelLoad - startTime;
				if (diff >= attackDuration) break;

				if (count >= bulletContinuity)
				{
					yield return new WaitForSeconds(bulletInterval);
					count = 0;
				}

				for (var i = 0; i < _shotPointList.Length; i++)
				{
					Instantiate(_bulletPrefab, _shotPointList[i].position, _shotPointList[i].rotation);
				}

				count++;
				yield return new WaitForSeconds(bulletRate);
			}
		}
	}
}