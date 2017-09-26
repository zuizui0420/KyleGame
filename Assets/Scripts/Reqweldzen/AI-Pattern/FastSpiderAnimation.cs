using System.Collections;
using UnityEngine;

public class FastSpiderAnimation : MonoBehaviour, ISpiderAnimation
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
	}

	//攻撃
	public void Suicide()
	{
		_animator.SetTrigger("Attack");
		StartCoroutine(Explosion());
	}

	private void Awake()
	{
		_animator = GetComponentInChildren<Animator>();
	}

	private IEnumerator Explosion()
	{
		// コルーチンの処理  
		// 2秒待つ  
		//yield return new WaitForSeconds(1.0f);
		//2秒後に爆発
		_explosionEffect.Play();

		yield return new WaitForSeconds(1.0f);
		//爆発後消去
		Destroy(gameObject);
	}
}