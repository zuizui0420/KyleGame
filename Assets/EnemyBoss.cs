using System;
using System.Collections;
using System.Collections.Generic;
using KyleGame;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class EnemyBoss : EnemyBase
{
	protected override void EnemyDead()
	{
		GetComponent<Boss>().ChangeState(BossState.Dead);
	}
}