using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ドア：スイッチ連動
/// </summary>
public class SwitchDoor : GimmickBase
{
    [SerializeField, Header("連動するギミック")]
    GimmickBase Gimmick;

    protected override void GimmickAction_Door()
    {
        GetComponent<Door>().Open();

        if(Gimmick != null)
        {
            Gimmick.GimmickAction();
        }
    }
}