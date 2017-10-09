using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class AutoCensorDoor : MonoBehaviour
{
	[SerializeField]
	private float _censorDistance = 3f;

	private Door _door;
	private Transform _player;

	private void Start()
	{
		_player = GameObject.Find("Player").transform;
		_door = GetComponent<Door>();

		this.UpdateAsObservable()
			.Subscribe(_ => DoorCensor());
	}

	private void DoorCensor()
	{
		var distance = Vector3.Distance(_door.transform.position, _player.position);

		if (distance < _censorDistance)
		{
			_door.Open();
		}
		else
		{
			_door.Close();
		}
	}
}