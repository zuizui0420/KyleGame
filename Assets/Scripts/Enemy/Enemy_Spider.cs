using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敵：スパイダー
/// </summary>
public class Enemy_Spider : EnemyBase
{
    [SerializeField, Header("FastSpiderAnimation")]
    FastSpiderAnimation SpiderAnimation;

    /// <summary>
    /// 敵：死亡処理
    /// </summary>
    protected override void EnemyDead()
    {
        SpiderAnimation.Dead();
        base.EnemyDead();
    }
}