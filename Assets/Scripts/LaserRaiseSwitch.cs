using System.Collections;
using System.Collections.Generic;
using KyleGame;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class LaserRaiseSwitch : GimmickBase, ILaserDrivenObject
{
    [SerializeField, Header("作動させるギミック")]
    GimmickBase Gimmick;

    [SerializeField, Header("SCI_FI_WallLamp")]
    SCI_FI_WallLamp Lamp;

    [SerializeField, Header("レーザーを当て続ける時間")]
    float Laser_HitTime;
	
    bool LampOn = false;

	private ReadOnlyReactiveProperty<bool> _isRaised;

	public bool IsActive { get; set; }

	private FloatReactiveProperty _currentTime = new FloatReactiveProperty();

	[SerializeField]
	private SCI_FI_WallLamp _wallLamp;
	private float RotateSpeed
	{
		get { return _wallLamp.RotateSpeed; }
		set { _wallLamp.RotateSpeed = value; }
	}

	private void Start()
	{
		_isRaised = _currentTime.Select(x => x > Laser_HitTime).ToReadOnlyReactiveProperty();

		_isRaised.Where(x => x).Take(1).Subscribe(_ =>
		{
			Lamp.LampSwitch(true);
			Gimmick.GimmickAction();
		});
	}

	public void OnHitLaser()
	{
		if (!_isRaised.Value)
		{
			if (GimmickActive)
			{

				_currentTime.Value += Time.deltaTime;

				RotateSpeed += 0.1f;

				RotateSpeed = Mathf.Clamp(RotateSpeed, 0f, 5f);
			}
			else
			{
				Lamp.LampSwitch(false);
				_currentTime.Value -= Time.deltaTime;
			}
		}
		else
		{
			RotateSpeed -= 0.1f;

			RotateSpeed = Mathf.Clamp(RotateSpeed, 0f, 5f);
		}
	}
}