using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UniRx;
using UniRx.Triggers;

namespace KyleGame
{
	[Serializable]
	public class PauseElement
	{
		public GameObject Canvas;
		public Transform Anchor;
	}

	public class PauseControl : MonoBehaviour
	{
		[SerializeField]
		private PauseElement[] _pauseElements;

		private void Start()
		{
			if (_pauseElements == null) return;

			Observable.FromCoroutine(TutorialCoroutine).Subscribe();
		}

		private IEnumerator TutorialCoroutine()
		{

			foreach (var element in _pauseElements)
			{
				transform.position = element.Anchor.position;

				yield return this.OnTriggerEnterAsObservable()
					.Where(x => x.CompareTag(TAGNAME.TAG_PLAYER))
					.FirstOrDefault()
					.ToYieldInstruction();

				element.Canvas.SetActive(true);
				Time.timeScale = 0;

				yield return Observable.Timer(TimeSpan.FromSeconds(3)).ToYieldInstruction();

				while (true)
				{
					if (Input.anyKeyDown)
					{
						break;
					}

					yield return null;
				}

				element.Canvas.SetActive(false);
				Time.timeScale = 1;
			}

			yield return null;
		}
	}

}