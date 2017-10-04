using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace KyleGame
{
	public class ParticleSystemTrigger : MonoBehaviour
	{
		private ParticleSystem _particleSystem;

		private void Awake()
		{
			_particleSystem = GetComponent<ParticleSystem>();
		}

		public IObservable<Unit> Play()
		{
			_particleSystem.Play();
			return Observable.Timer(TimeSpan.FromSeconds(_particleSystem.main.duration)).AsUnitObservable();
		}
	}
}