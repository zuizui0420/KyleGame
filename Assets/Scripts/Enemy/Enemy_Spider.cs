using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敵：スパイダー
/// </summary>
public class Enemy_Spider : EnemyBase
{
    [SerializeField, Header("敵の体力(０の場合は無敵)")]
    float Life;

	void Start ()
    {
        LIFE = Life;	
	}
	
    /// <summary>
    /// 敵：死亡処理
    /// </summary>
    protected override void EnemyDead()
    {
        base.EnemyDead();
    }
}