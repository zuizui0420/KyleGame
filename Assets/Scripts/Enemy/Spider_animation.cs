using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spider_animation : MonoBehaviour {

    public Animator spider_animator;
    public ParticleSystem spark;
    public ParticleSystem explosion;

    private float speed;
 
    // Use this for initialization
    void Start () {
        Attack();
	}
	
	// Update is called once per frame
	void Update () {
        //ここは任意に使って
        speed = spider_animator.GetFloat("Speed");

        if (speed >= 1.0f)
            Walk();
       
	}
    //移動
    void Walk()
    {
        spider_animator.Play("Walk");
    }

    //攻撃
    void Attack()
    {
        spider_animator.SetTrigger("Attack");
        spark.Play();
        StartCoroutine("Explosion");
    }

    private IEnumerator Explosion()
    {
        // コルーチンの処理  
        // 2秒待つ  
        yield return new WaitForSeconds(2.0f);
        //2秒後に爆発
        explosion.Play();

        yield return new WaitForSeconds(1.0f);
        //爆発後消去
        Destroy(gameObject);
}
}
