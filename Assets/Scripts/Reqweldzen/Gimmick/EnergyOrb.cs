using System.Collections;
using KyleGame;
using UniRx;
using UnityEngine;

public class EnergyOrb : MonoBehaviour
{
	private readonly Subject<Unit> _isOrbBrokenSubject = new Subject<Unit>();

	private FloatReactiveProperty _currentHealth;

	[SerializeField]
	[Header("敵の体力(０の場合は無敵)")]
	private float _endurance;

	[SerializeField]
	private GameObject _explosionEffect;

	[SerializeField]
	private GameObject _orbObject;

	private ParticleSystemTrigger _particleTrigger;

	public IObservable<Unit> IsOrbBroken
	{
		get { return _isOrbBrokenSubject; }
	}

	private void Start()
	{
		_particleTrigger = _explosionEffect.GetComponent<ParticleSystemTrigger>();
		_currentHealth = new FloatReactiveProperty(_endurance);
		_currentHealth.Where(x => x <= 0).Take(1).Subscribe(_ => { Observable.FromCoroutine(DamageCoroutine).Subscribe(); })
			.AddTo(this);
	}

	public void Damage()
	{
		_currentHealth.Value -= Time.deltaTime;
	}

	private IEnumerator DamageCoroutine()
	{
		_orbObject.SetActive(false);

		_explosionEffect.SetActive(true);

		_isOrbBrokenSubject.OnNext(Unit.Default);
		_isOrbBrokenSubject.OnCompleted();

		yield return _particleTrigger.Play().FirstOrDefault().ToYieldInstruction().AddTo(this);

		Destroy(gameObject);
	}
}