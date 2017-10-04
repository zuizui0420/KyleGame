using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace KyleGame
{
	public class ElectricDamageable : MonoBehaviour
	{
		private readonly Subject<Unit> _takeDamageSubject = new Subject<Unit>();
		public IObservable<Unit> DamageEvent { get { return _takeDamageSubject; } }

		private void Start()
		{
			this.OnTriggerEnterAsObservable()
				.Where(x => x.CompareTag(TAGNAME.TAG_ELECTRICWIRE))
				.ThrottleFirst(TimeSpan.FromSeconds(1))
				.AsUnitObservable()
				.Subscribe(_takeDamageSubject);
		}
	}
}