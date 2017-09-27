using System.Collections;
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
	public void Suicide()
	{
		_animator.SetTrigger("Break");
		_sparkEffect.Play();

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