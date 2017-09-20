using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCI_FI_Motor : MonoBehaviour
{
    [SerializeField, Header("Barrel")]
    GameObject Barrel;

    [SerializeField, Header("回転中かどうか")]
    public bool SpinOn;

    [SerializeField, Header("回転速度")]
    public float SpinSpeed;

	private void Update ()
    {
        if (SpinOn)
        {
            Vector3 Rot = Barrel.transform.localEulerAngles;
            Rot.y += SpinSpeed;
            Barrel.transform.localEulerAngles = Rot;
        }
        else
        {
            if(SpinSpeed > 0)
            {
                Vector3 Rot = Barrel.transform.localEulerAngles;
                Rot.y += SpinSpeed;
                Barrel.transform.localEulerAngles = Rot;

                SpinSpeed -= 0.1f;
            }
            else
            {
                SpinSpeed = 0f;
            }
        }      
    }
}