using System;
using UniRx;
using UnityEngine;

namespace KyleGame
{
	static class UniRxExtension
	{
		public static IObservable<Unit> Wait(this Component component, TimeSpan timeSpan)
		{
			return Observable.Timer(timeSpan)
				.TakeUntilDisable(component)
				.AsUnitObservable();
		}
	}
}