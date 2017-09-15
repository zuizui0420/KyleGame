using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spider_animation : MonoBehaviour {

    public Animator spider_animator;
    public ParticleSystem spark;

    private float speed;
 
    // Use this for initialization
    void Start () {

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
    }
}
