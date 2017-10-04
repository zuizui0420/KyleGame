using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionEffect : MonoBehaviour
{
	[SerializeField]
	private GameObject _damageAreaObject;
	
	// Use this for initialization
	private void Start()
	{
		_damageAreaObject.SetActive(true);
	}
}