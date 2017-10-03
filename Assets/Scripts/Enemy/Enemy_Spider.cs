using System.Collections;
using System.Collections.Generic;
using KyleGame;
using UnityEngine;

/// <summary>
/// 敵：スパイダー
/// </summary>
public class Enemy_Spider : EnemyBase
{
    /// <summary>
    /// 敵：死亡処理
    /// </summary>
    protected override void EnemyDead()
    {
        GetComponent<Spider>().Dead();
    }
}