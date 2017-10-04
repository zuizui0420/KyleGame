using System;
using System.Collections;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class BossAnimatorControl : MonoBehaviour
{
	private readonly int _tackleHash = Animator.StringToHash("Base Layer.Tackle");
	private Animator _animator;

	private BossLaserControl _bossLaserControl;

	private BoolReactiveProperty _isBeamingReactiveProperty = new BoolReactiveProperty();

	public float MovementSpeed
	{
		get { return _animator.GetFloat("Speed"); }
		set { _animator.SetFloat("Speed", value); }
	}

	/// <summary>
	///     アニメーション：ビーム攻撃
	/// </summary>
	public bool IsBeam
	{
		get { return _animator.GetBool("Beam"); }
		set { _animator.SetBool("Beam", value); }
	}

	public bool IsTackle
	{
		set { _animator.SetBool("Tackle", value); }
	}

	private void Awake()
	{
		_animator = GetComponentInChildren<Animator>();
		_bossLaserControl = GetComponentInChildren<BossLaserControl>();
	}

	private void Start()
	{
		this.UpdateAsObservable().Where(_ => Input.GetKeyDown(KeyCode.B))
			.Subscribe(_ => IsBeam = !IsBeam);

		this.UpdateAsObservable().Where(_ => Input.GetKeyDown(KeyCode.V))
			.Subscribe(_ => Death());
	}

	private void Lock()
	{
		_animator.speed = 0;
	}

	private void Release()
	{
		_animator.speed = 1;
	}

	public IObservable<Unit> Beam()
	{
		return Observable.FromCoroutine(BeamCoroutine);
	}

	private IEnumerator BeamCoroutine()
	{
		IsBeam = true;

		yield return new WaitForSeconds(0.5f);

		for (var i = 0; i < 3; i++)
		{
			_bossLaserControl.Shot();

			yield return new WaitForSeconds(0.3f);
		}

		IsBeam = false;

		yield return new WaitForSeconds(1f);
	}

	/// <summary>
	///     アニメーション：死亡
	/// </summary>
	public void Death()
	{
		_animator.SetTrigger("Death");
	}

	/// <summary>
	///     アニメーション：タックル
	/// </summary>
	public void Animation_Damage()
	{
		_animator.SetBool("Damage", true);
	}
	


	private void Tackle()
	{
		_animator.Play(_tackleHash);
	}

	public IObservable<Unit> TackleReady()
	{
		return Observable.Create<Unit>(observer =>
		{
			Tackle();
			var disposable = Observable.Return(Unit.Default).Delay(TimeSpan.FromMilliseconds(220)).Subscribe(_ =>
			{
				_animator.speed = 0;
				observer.OnNext(Unit.Default);
				observer.OnCompleted();
			});

			return disposable;
		});
	}

	public void TackleRelease()
	{
		_animator.speed = 1;
	}
}