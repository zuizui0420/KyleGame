using System.Collections;
using UniRx;
using UnityEngine;

public class DroneAnimation : MonoBehaviour
{
	private Animator _animator;

	[SerializeField]
	private ParticleSystem _explosionEffect;

	[SerializeField]
	private ParticleSystem _sparkEffect;

	public float Speed { get; set; }

	private void Awake()
	{
		_animator = GetComponentInChildren<Animator>();
	}

	//攻撃
	public IObservable<Unit> Suicide()
	{
		_animator.SetTrigger("Break");
		_sparkEffect.Play();

		return Observable.FromCoroutine<Unit>(Explosion);
	}

	private IEnumerator Explosion(IObserver<Unit> observer)
	{
		yield return new WaitForSeconds(1f);

		observer.OnNext(Unit.Default);
		_explosionEffect.Play();
		Debug.Log("Play Effect");

		yield return new WaitForSeconds(_explosionEffect.main.duration);

		observer.OnCompleted();
	}
}