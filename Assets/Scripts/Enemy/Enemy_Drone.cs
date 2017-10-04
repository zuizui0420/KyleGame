using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 敵：ドローン
/// </summary>
public class Enemy_Drone : EnemyBase
{   
    [SerializeField, Header("Animator")]
    Animator EnemyAnimation;

    [SerializeField,Header("漏電エフェクト")]
    ParticleSystem Effect_Spark;

    [SerializeField, Header("爆発エフェクト")]
    ParticleSystem Effect_Explosion;

    public float Speed { get; set; }

    protected override void EnemyDead()
    {
        GetComponent<NavMeshAgent>().speed = 0f;
        Destroy();       
    }

    private void Destroy()
    {
        EnemyAnimation.SetTrigger("Break");
        Effect_Spark.Play();
        StartCoroutine("Explosion");
    }

    private IEnumerator Explosion()
    {
        Effect_Spark.Play();

        yield return new WaitForSeconds(0.5f);

        Effect_Explosion.Play();

		AudioManager.Instance.Play(AUDIONAME.SE_EXPLOSION_2,0.6f,false,120);

		yield return new WaitForSeconds(1f);

		AudioManager.Instance.AudioDelete(AUDIONAME.SE_EXPLOSION_2);
		Destroy(gameObject);
    }
}