using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class BossAnimationControle : MonoBehaviour
{
	private Animator _animator;

	private float debug_speed;

	private readonly int _tackleHash = Animator.StringToHash("Base Layer.Tackle");

	private BoolReactiveProperty _isBeamingReactiveProperty = new BoolReactiveProperty();

	private void Start()
	{
		_animator = GetComponent<Animator>();

		var animatorStateStream = this.UpdateAsObservable().Select(_ => _animator.GetCurrentAnimatorStateInfo(0));

		animatorStateStream.Where(x => x.fullPathHash == _tackleHash)
			.Where(x => x.normalizedTime > 0.3f)
			.Subscribe(_ =>
			{
				Lock();
			});

		this.UpdateAsObservable().Where(_ => Input.GetKeyDown(KeyCode.B))
			.Subscribe(_ => IsBeam = !IsBeam);

		this.UpdateAsObservable().Where(_ => Input.GetKey(KeyCode.N))
			.Subscribe(_ =>
			{
				debug_speed += 0.01f;

				Mathf.Clamp(debug_speed, 0f, 1f);

				MovementSpeed = debug_speed;
			});

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

	public float MovementSpeed
	{
		set { _animator.SetFloat("Speed", value); }
	}

	/// <summary>
	///     アニメーション：ビーム攻撃
	/// </summary>
	public bool IsBeam
	{
		get
		{
			return _animator.GetBool("Beam");
			
		}
		set
		{
			_animator.SetBool("Beam", value);
		}
	}
	/// <summary>
	///     アニメーション：死亡
	/// </summary>
	public void Death()
	{
		_animator.SetTrigger("Death");
	}

	public bool IsTackle
	{
		set { _animator.SetBool("Tackle", value); }
	}
	/// <summary>
	///     アニメーション：タックル
	/// </summary>
	public void Animation_Damage()
	{
		_animator.SetBool("Damage", true);
	}

	/// <summary>
	/// タックル構え
	/// </summary>
	/// <returns>リリース</returns>
	public Action TackleReady()
	{
		_animator.SetBool("Tackle", true);

		return () => _animator.SetBool("Damage", true);
	}
}