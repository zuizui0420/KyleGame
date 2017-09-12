using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public interface IInputEventProvider
{
	IReadOnlyReactiveProperty<bool> LeftTriggerButton { get; }
	IReadOnlyReactiveProperty<bool> RightTriggerButton { get; }
	IReadOnlyReactiveProperty<Vector3> MoveDirection { get; }
	IReadOnlyReactiveProperty<Vector3> CameraRotation { get; }
}