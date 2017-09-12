using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class PlayerInputProvider : BasePlayerComponent, IInputEventProvider
{
	private readonly BoolReactiveProperty _leftTriggerButtonReactiveProperty = new BoolReactiveProperty();
	private readonly BoolReactiveProperty _rightTriggerButtonReactiveProperty = new BoolReactiveProperty();
	private readonly Vector3ReactiveProperty _moveDirectionReactiveProperty = new Vector3ReactiveProperty();
	private readonly Vector3ReactiveProperty _cameraRotationReactiveProperty = new Vector3ReactiveProperty();

	protected override void OnInitialize()
	{
		
	}

	public IReadOnlyReactiveProperty<bool> LeftTriggerButton { get { return _leftTriggerButtonReactiveProperty; } }
	public IReadOnlyReactiveProperty<bool> RightTriggerButton { get { return _rightTriggerButtonReactiveProperty; } }
	public IReadOnlyReactiveProperty<Vector3> MoveDirection { get { return _moveDirectionReactiveProperty; } }
	public IReadOnlyReactiveProperty<Vector3> CameraRotation { get { return _cameraRotationReactiveProperty; } }
}