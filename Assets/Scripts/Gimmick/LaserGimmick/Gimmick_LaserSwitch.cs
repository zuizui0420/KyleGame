using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gimmick_LaserSwitch : GimmickBase
{
    [SerializeField, Header("作動させるギミック")]
    GimmickBase Gimmick;

    [SerializeField, Header("SCI_FI_WallLamp")]
    SCI_FI_WallLamp Lamp;

    [SerializeField, Header("レーザーを当て続ける時間")]
    float Laser_HitTime;

    float Laser_currentTime = 0f;

    bool LampOn = false;

    protected override void GimmickAction_Laser()
    {
        //ランプが起動していない時のみ
        if (!LampOn)
        {
            if (GimmickActive)
            {
                Laser_currentTime += Time.deltaTime;

                if (Laser_currentTime > Laser_HitTime)
                {
                    Lamp.LampSwitch(true);
                    LampOn = true;
                    Gimmick.GimmickAction();
                }
            }
            else
            {
                Lamp.LampSwitch(false);
                Laser_currentTime -= Time.deltaTime;
            }
        }        
    }
}