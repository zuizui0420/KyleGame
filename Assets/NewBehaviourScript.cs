using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

public class NewBehaviourScript : MonoBehaviour
{
	public Button _button;

	// Use this for initialization
	private void Start()
	{
		var first = _button.OnClickAsObservable().Do(_ => Debug.Log("Click"));
		var second = Observable.Timer(TimeSpan.FromSeconds(3)).Do(_ => Debug.Log("Timer")).AsUnitObservable();

		first.Merge(second).Take(1).Subscribe(_ => Debug.Log("OnNext"), () =>
		{
			Debug.Log("OnCompleted");
			Destroy(gameObject);
		}).AddTo(this);

		this.OnDestroyAsObservable().Subscribe(_ => Debug.Log("Destroyed"));
	}
}