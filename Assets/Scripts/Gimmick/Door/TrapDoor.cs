using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ギミック：トラップドア
/// </summary>
public class TrapDoor : GimmickBase
{
    [SerializeField, Header("ギミック作動のトリガーかどうか")]
    bool TriggerGimmick;

    [SerializeField, Header("連動するギミック")]
    GimmickBase Gimmick;

    [SerializeField, Header("通行禁止用のコライダー")]
    GameObject NoEntryCollider;

    [SerializeField, Header("")]
    Gimmick_Bar[] Bars;

    [SerializeField, Header("通行禁止状態")]
    bool NO_ENTRY_Mode;

    void Start()
    {
        if (NO_ENTRY_Mode)
        {
            foreach (Gimmick_Bar bar in Bars)
            {
                bar.BarShuftUp();
            }

            GetComponent<Door>().Close();

            NoEntryCollider.SetActive(true);
        }
        else
        {
            foreach (Gimmick_Bar bar in Bars)
            {
                bar.BarShuftDown();
            }

            GetComponent<Door>().Open();

            NoEntryCollider.SetActive(false);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            foreach(Gimmick_Bar bar in Bars)
            {
                bar.BarShuftUp();
            }

            GetComponent<Door>().Close();
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            foreach (Gimmick_Bar bar in Bars)
            {
                bar.BarShuftDown();
            }

            GetComponent<Door>().Open();
        }
    }

    protected override void GimmickAction_Door()
    {
        NoEntryCollider.SetActive(false);

        NO_ENTRY_Mode = false;

        GetComponent<Door>().Open();

        foreach (Gimmick_Bar bar in Bars)
        {
            bar.BarShuftDown();
        }       
    }

    private void OnTriggerEnter(Collider col)
    {
        if (TriggerGimmick)
        {
            if(Gimmick != null)
            {
                if (col.gameObject.tag == TAGNAME.TAG_PLAYER)
                {
                    NoEntryCollider.SetActive(true);

                    NO_ENTRY_Mode = true;

                    GetComponent<Door>().Close();

                    foreach (Gimmick_Bar bar in Bars)
                    {
                        bar.BarShuftUp();
                    }

                    Gimmick.GimmickAction();
                }
            }            
        }        
    }  
}