using System;
using System.Collections;
using System.Collections.Generic;
using KyleGame;
using UniRx;
using UnityEngine;

public class EnergyOrb : MonoBehaviour
{
	[SerializeField]
	private GameObject _orbObject;

	[SerializeField]
	private GameObject _explosionEffect;

	private ParticleSystemTrigger _particleTrigger;

	private readonly Subject<Unit> _isOrbBrokenSubject = new Subject<Unit>();
	public IObservable<Unit> IsOrbBroken { get { return _isOrbBrokenSubject; } }

	private void Start()
	{
		_particleTrigger = _explosionEffect.GetComponent<ParticleSystemTrigger>();
	}

	public void Damage()
	{
		Observable.FromCoroutine(DamageCoroutine).Subscribe();
	}

	private IEnumerator DamageCoroutine()
	{
		_orbObject.SetActive(false);
		_explosionEffect.SetActive(true);

		yield return null;
		
		_isOrbBrokenSubject.OnCompleted();

		yield return _particleTrigger.IsFinished.ToYieldInstruction();

		Destroy(gameObject);
	}
}