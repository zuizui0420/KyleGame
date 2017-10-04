using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class ElectricWireElement : MonoBehaviour
{
	public IObservable<Collision> CollisionEnterObservable
	{
		get { return this.OnCollisionEnterAsObservable(); }
	}
}