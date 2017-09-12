using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public abstract class BasePlayerComponent : MonoBehaviour
{
	private IInputEventProvider _inputEventProvider;

	protected IInputEventProvider InputEventProvider { get { return _inputEventProvider; } }
	protected PlayerCore Core;

	private void Start()
	{
		Core = GetComponent<PlayerCore>();
		_inputEventProvider = GetComponent<IInputEventProvider>();

		OnStart();
	}

	protected virtual void OnStart()
	{
		Core.OnInitializeAsync.Subscribe(_ => OnInitialize());
	}

	protected abstract void OnInitialize();
}