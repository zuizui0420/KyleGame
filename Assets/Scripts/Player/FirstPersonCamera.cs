using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonCamera : SingletonMonoBehaviour<FirstPersonCamera>
{
    [SerializeField,Header("1人称になる座標")]
    GameObject FirstPerson_AnglePoint;

    [SerializeField, Header("PlayerSystem")]
    PlayerSystem playerSystem;

    Vector3 DefaultPointPos;
    Vector3 FirstPersonPointPos;

	void Start ()
    {
        DefaultPointPos = transform.localPosition;
        FirstPersonPointPos = FirstPerson_AnglePoint.transform.localPosition;

        GetComponent<Camera>().enabled = false;
    }
	
	void Update ()
    {
        if (playerSystem.Mode_Laser)
        {
            GetComponent<Camera>().enabled = true;
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, FirstPersonPointPos, 0.3f);
        }
        else
        {
            GetComponent<Camera>().enabled = false;
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, DefaultPointPos, 0.3f);
        }        
	}
}