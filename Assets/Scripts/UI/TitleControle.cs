using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class TitleControle : MonoBehaviour
{
	public SceneName SceneName;

	private void Start()
	{
		this.UpdateAsObservable()
			.Where(_ => Input.anyKeyDown)
			.Take(1)
			.Subscribe(_ =>
			{
				SceneFader.Instance.LoadLevel(SceneName);
			});
	}
}