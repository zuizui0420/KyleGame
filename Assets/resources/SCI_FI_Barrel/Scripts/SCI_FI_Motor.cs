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

    [SerializeField, Header("回転速度の限界値")]
    float MaxSpinSpeed;

	private void Update ()
    {
        if (SpinOn)
        {
            if(SpinSpeed < MaxSpinSpeed)
            {
                Vector3 Rot = Barrel.transform.localEulerAngles;
                Rot.y += SpinSpeed;
                Barrel.transform.localEulerAngles = Rot;

                SpinSpeed += 0.1f;
            }
            else
            {
                Vector3 Rot = Barrel.transform.localEulerAngles;
                Rot.y += SpinSpeed;
                Barrel.transform.localEulerAngles = Rot;

                SpinSpeed = MaxSpinSpeed;
            }
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