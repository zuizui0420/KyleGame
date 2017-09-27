using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ギミック：ベース
/// </summary>
public class GimmickBase : MonoBehaviour
{
    [SerializeField, Header("ギミックの種類")]
    GIMMICK GIMMICK_TYPE;

    [SerializeField, Header("再起動する時間(０の場合は再起動しない)")]
    float ReStartTime;

    [HideInInspector]
    public bool GimmickActive = false;

    //ギミックの体力 (０の場合は無敵)
    protected float LIFE;

    //破壊フラグ
    bool BreakFlg = false;

    public enum GIMMICK
    {
        LASER,
        SPARK,
        ENEMY,
        DOOR,
    }

    /// <summary>
    /// 各種ギミックの作動処理
    /// </summary>
    /// <param name="active"></param>
    public void GimmickAction(bool active = false)
    {
        GimmickActive = active;

        switch (GIMMICK_TYPE)
        {
            case GIMMICK.LASER:
                GimmickAction_Laser();
                break;

            case GIMMICK.SPARK:
                GimmickAction_Spark();
                break;

            case GIMMICK.ENEMY:
                GimmickAction_Enemy();
                break;

            case GIMMICK.DOOR:
                GimmickAction_Door();
                break;
        }
    }

    /// <summary>
    /// ギミック：ダメージ処理
    /// </summary>
    public void GimmickDamage()
    {
        //無敵の場合は処理を実行しない
        if(LIFE != 0 && !BreakFlg)
        {
            LIFE -= Time.deltaTime;

            if(LIFE <= 0)
            {
                Debug.Log(gameObject.name + "を破壊");

                BreakFlg = true;
                GimmickBreak();

                if(ReStartTime != 0) { StartCoroutine(ReStart()); }
            }
        }
    }

    /// <summary>
    /// ギミック：破壊処理
    /// </summary>
    protected virtual void GimmickBreak(){}

    /// <summary>
    /// ギミック：再起動処理
    /// </summary>
    /// <returns></returns>
    IEnumerator ReStart()
    {
        yield return new WaitForSeconds(ReStartTime);

        BreakFlg = false;

        GimmickAction();
    }

    #region 各ギミックのアクションメソッド
    protected virtual void GimmickAction_Laser(){}

    protected virtual void GimmickAction_Spark(){}

    protected virtual void GimmickAction_Enemy(){}

    protected virtual void GimmickAction_Door(){}
    #endregion

}