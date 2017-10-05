using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class TitleManager : MonoBehaviour
{
	// Use this for initialization
	private void Start()
	{
		this.UpdateAsObservable()
			.Where(_ => Input.anyKeyDown)
			.Take(1)
			.Subscribe(_ => SceneFader.Instance.LoadLevel(SCENENAME.SCENE_STAGE1));
	}
}