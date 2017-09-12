using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

[RequireComponent(typeof(PlayerCharacterController))]
public class PlayerMover : BasePlayerComponent
{
	private BoolReactiveProperty _isRunning = new BoolReactiveProperty();

	private PlayerCharacterController _playerCharacterController;

	protected override void OnInitialize()
	{
		_playerCharacterController = GetComponent<PlayerCharacterController>();
	}
}