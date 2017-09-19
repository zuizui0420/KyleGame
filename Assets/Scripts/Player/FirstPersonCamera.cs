﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonCamera : SingletonMonoBehaviour<FirstPersonCamera>
{
    [SerializeField,Header("1人称になる座標")]
    GameObject FirstPerson_AnglePoint;

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
        if (!PlayerSystem.instance.PlayerControle)
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