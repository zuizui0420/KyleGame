using System.Collections;
using UnityEngine;
using UnityEngine.AI;

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

    public void Dead()
    {
        GetComponent<NavMeshAgent>().speed = 0f;
        StartCoroutine(Explosion());       
    }

	private IEnumerator Explosion()
	{
        _sparkEffect.Play();

        yield return new WaitForSeconds(0.5f);

		_explosionEffect.Play();

		yield return new WaitForSeconds(1.0f);

		Destroy(gameObject);
	}
}