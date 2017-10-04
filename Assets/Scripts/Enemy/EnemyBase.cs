using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敵：ベース
/// </summary>
public class EnemyBase : MonoBehaviour
{
    [SerializeField, Header("敵の体力(０の場合は無敵)")]
    float LIFE;

    //死亡フラグ
    bool Dead = false;

    /// <summary>
    /// 敵：ダメージ処理
    /// </summary>
    public void EnemyDamage()
    {
        if (!Dead && LIFE != 0)
        {
            LIFE -= Time.deltaTime;

            if (LIFE <= 0f)
            {
                Debug.Log(gameObject.name + "を撃破");

				AudioManager.Instance.Play(AUDIONAME.SE_EXPLOSION_1);

				Dead = true;

                EnemyDead();
            }
        }        
    }

    /// <summary>
    /// 敵：死亡処理
    /// </summary>
    protected virtual void EnemyDead(){}
}