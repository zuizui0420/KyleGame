using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricalDoor : MonoBehaviourExtension
{
    [SerializeField, Header("SCI_FI_Motor")]
    SCI_FI_Motor[] Motors;

    [SerializeField, Header("PlayerSystem")]
    PlayerSystem _player;

    [SerializeField, Header("扉が閉じる時間")]
    float CloseTime;

    private Door _door;

    bool AreaStay = false;

    bool DoorOpened = false;

	void Start ()
    {
        _door = GetComponent<Door>();
	}
	
	void Update ()
    {
        //範囲内にいる状態で放電をした場合
        if (AreaStay && _player.SparkAttack)
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
        }
        else
        {
            foreach (SCI_FI_Motor motor in Motors)
            {
                motor.SpinOn = false;
            }
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

    private void OnTriggerStay(Collider col)
    {
        if(col.tag == "Player")
        {
            AreaStay = true;
        }
    }

    private void OnTriggerExit()
    {
        AreaStay = false;
    }
}