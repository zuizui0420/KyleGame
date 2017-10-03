using System.Collections;
using UniRx;
using UnityEngine;

public class SlowSpiderAnimation : MonoBehaviour, ISpiderAnimation
{
	private Animator _animator;

	[SerializeField]
	private ParticleSystem _explosionEffect;

	[SerializeField]
	private ParticleSystem _sparkEffect;

	/// <inheritdoc />
	/// <summary>
	///     移動スピード
	/// </summary>
	public float Speed
	{
		set { _animator.SetFloat("Speed", value); }
	}

	public void Spark()
	{
		_sparkEffect.Play();
		_animator.SetTrigger("Attack");
	}

	//攻撃
	public IObservable<Unit> Suicide()
	{
		return Observable.FromCoroutine<Unit>(Explosion);
	}

	public IObservable<Unit> Dead()
	{
		_sparkEffect.Play();
		return Observable.FromCoroutine<Unit>(Explosion);
	}

	private void Awake()
	{
		_animator = GetComponentInChildren<Animator>();
	}

	private IEnumerator Explosion(IObserver<Unit> observer)
	{
		yield return new WaitForSeconds(0.5f);

		observer.OnNext(Unit.Default);
		_explosionEffect.Play();

		yield return new WaitForSeconds(_explosionEffect.main.duration);

		observer.OnCompleted();
	}
}