using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage2_Room_Gimmick_Panel : MonoBehaviour
{
    [SerializeField, Header("ID")]
    public int ID;

    //トリガーとなるパネルかどうか
    [HideInInspector]
    public bool ThisTrigger = false;

    //ギミックのトリガー状態
    [HideInInspector]
    public bool GimmickTrigger = false;

    void OnCollisionEnter(Collision col)
    {
        if (ThisTrigger)
        {
            if (col.gameObject.tag == TAGNAME.TAG_PLAYER)
            {
                Debug.Log("接触");
                GimmickTrigger = true;
            };
        }      
    }
}