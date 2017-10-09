using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

/// <inheritdoc />
/// <summary>
///     ギミック：エレベータードア
/// </summary>
public class Elevator : MonoBehaviour
{
	private Door _door;

	[SerializeField]
	[Header("移動先座標")]
	private GameObject _movePoint;

	private void Start()
	{
		_door = GetComponent<Door>();

		this.OnTriggerEnterAsObservable()
			.Where(x => x.CompareTag(TAGNAME.TAG_PLAYER))
			.Take(1)
			.Subscribe(
				player =>
				{
					player.transform.SetParent(transform);

					_door.Close();

					Observable.Timer(TimeSpan.FromSeconds(3)).Subscribe(_ =>
					{
						// 音
						AudioManager.Instance.BGMFadeOut(AUDIONAME.BGM_STAGE, 0.5f);
						AudioManager.Instance.Play(AUDIONAME.SE_ELEVATOR, 0.8f);

						// シーン遷移
						SceneFader.Instance.LoadLevel(StageManager.Instance.ReturnNextSceneName());

						// エレベーターを動かす
						ElevatorMove();
					});
				})
			.AddTo(this);
	}

	private void ElevatorMove()
	{
		this.UpdateAsObservable()
			.TakeUntil(Observable.Timer(TimeSpan.FromSeconds(5)))
			.Subscribe(_ =>
			{
				transform.position = Vector3.MoveTowards(transform.position, _movePoint.transform.position, 0.1f);
			});
	}
}