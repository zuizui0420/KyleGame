using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ギミック：ベース
/// </summary>
public class GimmickBase : MonoBehaviour
{
    [SerializeField, Header("ギミックが作動中かどうか")]
    public bool GimmickActive = false;

    [SerializeField, Header("ギミックの種類")]
    GIMMICK GIMMICK_TYPE;

    public enum GIMMICK
    {
        LASER,
        SPARK,
        ENEMY,
        DOOR,
    }

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

    protected virtual void GimmickAction_Laser()
    {
    }

    protected virtual void GimmickAction_Spark()
    {

    }

    protected virtual void GimmickAction_Enemy()
    {

    }

    protected virtual void GimmickAction_Door()
    {

    }
}