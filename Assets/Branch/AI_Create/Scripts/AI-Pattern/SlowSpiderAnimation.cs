using System.Collections;
using KyleGame;
using UnityEngine;

public class SlowSpiderAnimation : MonoBehaviour, ISpiderAnimation
{
	[SerializeField]
	private ParticleSystem _explosionEffect;

	[SerializeField]
	private ParticleSystem _sparkEffect;

	private Animator _animator;

	/// <inheritdoc />
	/// <summary>
	/// 移動スピード
	/// </summary>
	public float Speed
	{
		set
		{
			_animator.SetFloat("Speed", value);
		}
	}

	private void Awake()
	{
		_animator = GetComponentInChildren<Animator>();
	}

	public void Spark()
	{
		_animator.SetTrigger("Attack");
		_sparkEffect.Play();
	}


	//攻撃
	public void Suicide()
	{
		StartCoroutine(Explosion());
	}

	private IEnumerator Explosion()
	{
		// コルーチンの処理  
		// 2秒待つ  
		yield return new WaitForSeconds(2.0f);
		//2秒後に爆発
		_explosionEffect.Play();

		yield return new WaitForSeconds(1.0f);
		//爆発後消去
		Destroy(gameObject);
	}
}