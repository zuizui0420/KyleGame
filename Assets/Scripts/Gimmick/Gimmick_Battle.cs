using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ギミック：強制戦闘
/// </summary>
public class Gimmick_Battle : GimmickBase
{
    [SerializeField, Header("バトルに参加する敵")]
    List<GameObject> Enemies = new List<GameObject>();

    [SerializeField, Header("連動するギミック")]
    GimmickBase Gimmick;

    //破壊した敵の数
    int CountDestroyEnemy = 0;

    //敵の合計
    int EnemySum;

    //ギミックが作動しているかどうか
    bool GimmickStart = false;

    //敵を全滅させたかどうか
    bool EnemyDefeat = false;

	void Awake ()
    {
        //敵の数を取得
        EnemySum = Enemies.Count;

        foreach (GameObject enemy in Enemies)
        {
            enemy.SetActive(false);
        }
    }
	
	void Update ()
    {
        if (GimmickStart)
        {
            if (!EnemyDefeat)
            {
                foreach (GameObject enemy in Enemies)
                {
                    if (enemy == null)
                    {
                        CountDestroyEnemy++;
                        Enemies.Remove(enemy);

                        //敵を全滅させた場合
                        if (CountDestroyEnemy == EnemySum)
                        {
                            EnemyDefeat = true;

                            //ギミック作動
                            Gimmick.GimmickAction();
                        }
                    }
                }
            }
        }        
    }

    protected override void GimmickAction_Enemy()
    {
        foreach (GameObject enemy in Enemies)
        {
            enemy.SetActive(true);
        }

        GimmickStart = true;
    }
}