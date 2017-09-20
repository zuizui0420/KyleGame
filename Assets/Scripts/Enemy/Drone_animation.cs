using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drone_animation : MonoBehaviour {
    public Animator drone_animation;
    public ParticleSystem spark;
    public ParticleSystem explosion;

    // Use this for initialization
    void Start () {
 
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    //攻撃
    void Destroy()
    {
        drone_animation.SetTrigger("Break");
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
