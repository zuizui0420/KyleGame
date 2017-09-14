using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class Door : MonoBehaviour
{
	[SerializeField]
	private Transform _doorLeft;

	[SerializeField]
	private Transform _doorRight;

	private Animator _animator;

	[SerializeField]
	private BoolReactiveProperty _isDoorOpenReactiveProperty = new BoolReactiveProperty();
	public IReadOnlyReactiveProperty<bool> OnDoorStateChange { get { return _isDoorOpenReactiveProperty; } }

	private void Start()
	{
		_animator = GetComponent<Animator>();

		_isDoorOpenReactiveProperty
			.TakeUntilDestroy(this)
			.Subscribe(x =>
			{
				_animator.SetBool("DoorState", x);
			});
	}

	public void Open()
	{
		_isDoorOpenReactiveProperty.Value = true;
	}

	public void Close()
	{
		_isDoorOpenReactiveProperty.Value = false;
	}
}