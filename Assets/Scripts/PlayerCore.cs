using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class PlayerCore : MonoBehaviour
{
	private readonly AsyncSubject<Unit> _onInitializeAsyncSubject = new AsyncSubject<Unit>();
	public IObservable<Unit> OnInitializeAsync { get { return _onInitializeAsyncSubject; } }

	private void Awake()
	{
		_onInitializeAsyncSubject.Subscribe(_ =>
		{
		});
	}

	public void Initialize()
	{
		_onInitializeAsyncSubject.OnNext(Unit.Default);
		_onInitializeAsyncSubject.OnCompleted();
	}
}