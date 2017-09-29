using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAnimationControle : MonoBehaviour
{
    Animator BossAnimator;

    float debug_speed = 0f;


    void Start ()
    {
        BossAnimator = GetComponent<Animator>();
	}
	
    void Update()
    {
        AnimatorStateInfo state = BossAnimator.GetCurrentAnimatorStateInfo(0);
        if (state.fullPathHash == Animator.StringToHash("Base Layer.Tackle"))
        {
            if (state.normalizedTime > 0.3f)
            {
                BossAnimator.speed = 0;
            }
        }
        else BossAnimator.speed = 1;

        if (Input.GetKeyDown(KeyCode.V))
        {
            Animation_Death();
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            if (BossAnimator.GetBool("Beam"))
            {
                Animation_Beam(false);
            }
            else
            {
                Animation_Beam(true);
            }            
        }

        if (Input.GetKey(KeyCode.N))
        {
            debug_speed += 0.01f;

            Mathf.Clamp(debug_speed, 0f, 1f);

            Animation_Move(debug_speed);
        }
    }

    /// <summary>
    /// アニメーション：移動
    /// </summary>
    /// <param name="speed">移動量</param>
    public void Animation_Move(float speed)
    {
        BossAnimator.SetFloat("Speed", speed);
    }

    /// <summary>
    /// アニメーション：ビーム攻撃
    /// </summary>
    /// <param name="state">ステート</param>
    public void Animation_Beam(bool parametor)
    {
        BossAnimator.SetBool("Beam", parametor);
    }

    /// <summary>
    /// アニメーション：死亡
    /// </summary>
    public void Animation_Death()
    {
        BossAnimator.SetTrigger("Death");
    }

    /// <summary>
    /// アニメーション：タックル
    /// </summary>
    public void Animation_Tackle()
    {
        BossAnimator.SetBool("Tackle", true);
    }

    /// <summary>
    /// アニメーション：タックル
    /// </summary>
    public void Animation_Damage()
    {
        BossAnimator.SetBool("Damage", true);
    }
}