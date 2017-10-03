using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace KyleGame
{
	public class ParticleSystemTrigger : MonoBehaviour
	{
		private readonly AsyncSubject<Unit> _isFinishedSubject = new AsyncSubject<Unit>();
		public IObservable<Unit> IsFinished { get { return _isFinishedSubject; } }

		private ParticleSystem _particleSystem;

		private void Awake()
		{
			_particleSystem = GetComponent<ParticleSystem>();
		}

		private void Start()
		{
			Observable.Return(Unit.Default).Delay(TimeSpan.FromSeconds(_particleSystem.main.duration))
				.TakeUntilDestroy(this)
				.Subscribe(_isFinishedSubject);
		}
	}
}