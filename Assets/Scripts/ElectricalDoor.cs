using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricalDoor : MonoBehaviourExtension
{
    [SerializeField, Header("SCI_FI_Switch")]
    SCI_FI_Switch[] Switchs;

    [SerializeField, Header("SCI_FI_Motor")]
    SCI_FI_Motor[] Motors;

    [SerializeField, Header("EmmisionCable")]
    MeshRenderer[] Cables;

    [SerializeField, Header("扉が閉じる時間")]
    float CloseTime;

    private Door _door;

    bool DoorOpened = false;

    MeshRenderer[] m_renderer;

    [SerializeField, Header("初期カラー")]
    Color DefaultColor;

    [SerializeField, Header("遷移カラー")]
    Color EmmisiveColor;

    float ColorLerpSpeed = 0.1f;

    PlayerSystem playerSystem;

	void Start ()
    {
        _door = GetComponent<Door>();

        foreach(MeshRenderer renderer in Cables)
        {
            //EmissionColorの使用を可能にする
            renderer.material.EnableKeyword("_EMISSION");

            //初期EmissionColorを設定
            renderer.material.SetColor("_EmissionColor", DefaultColor);
        }

        //Playerを検索
        playerSystem = GameObject.FindGameObjectWithTag(TAGNAME.TAG_PLAYER).GetComponent<PlayerSystem>();
    }
	
	void Update ()
    {
        //スイッチを踏んでいる状態で放電をした場合
        if ((Switchs[0].SwitchOn || Switchs[1].SwitchOn) && playerSystem.SparkAttack)
        {
            foreach (SCI_FI_Motor motor in Motors)
            {
                motor.SpinOn = true;                
            }

            if(Motors[0].SpinSpeed > 10f)
            {
                _door.Open();
                DoorOpened = true;
            }

            ColorMove(true);       
        }
        else
        {
            foreach (SCI_FI_Motor motor in Motors)
            {
                motor.SpinOn = false;
            }

            ColorMove(false);
        }

        if (DoorOpened)
        {
            if (Motors[0].SpinSpeed == 0f)
            {
                DoorOpened = false;

                _door.Close();
            }
        }   
	}

    private void ColorMove(bool flg)
    {
        if (flg)
        {
            foreach (MeshRenderer renderer in Cables)
            {
                //Colorを設定
                renderer.material.SetColor("_EmissionColor", Color.Lerp(renderer.material.GetColor("_EmissionColor"), EmmisiveColor, ColorLerpSpeed));
            }
        }
        else
        {
            foreach (MeshRenderer renderer in Cables)
            {
                //Colorを設定
                renderer.material.SetColor("_EmissionColor", Color.Lerp(renderer.material.GetColor("_EmissionColor"), DefaultColor, ColorLerpSpeed));
            }
        }
    }
}