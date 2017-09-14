using System;
using GamepadInput;
using UnityEngine;

//このクラスでは、プレイヤーの制御をするものです
public class PlayerSystem : SingletonMonoBehaviour<PlayerSystem>
{
	[Serializable]
	public class Settings
	{
		[Header("LaserLeft")]
		public Attack_Laser LaserLeft;
		
		[Header("RightLaser")]
		public Attack_Laser LaserRight;

		[Header("Electric Mode Effect when AttackPhase")]
		public GameObject ElectricAttackEffect;

		[SerializeField]
		[Header("Electric Mode Effect")]
		public GameObject ElectricEffect;
	}

	[SerializeField]
	private Settings _settings;

	//モード：エレクトリ
	private bool _isElectricMode;

	//モード：レーザー
	private bool _isLaserMode;

	//各モードでの攻撃中かどうか
	private bool _laserAttackFlag, _electricAttackFlag;

	

	//計算後の移動量
	private Vector3 _moveDirection;

	//最終前方
	private float _moveForwardSpeed;

	[Header("移動量")]
	[SerializeField]
	[Range(0, 10)]
	private float _moveSpeed;

	[Header("動いている状態からの回転量")]
	[SerializeField]
	[Range(0, 360)]
	private float _movingTurnSpeed;

	private Animator _playerAnimator;

	//プレイヤーのカメラ
	private Camera _playerCamera;

	

	[Header("回転量")]
	[SerializeField]
	[Range(0, 1)]
	private float _rotateSpeed;

	[Header("静止状態からの回転量")]
	[SerializeField]
	[Range(0, 360)]
	private float _stationaryTurnSpeed;

	//最終角度
	private float _turnAmount;

	[SerializeField]
	[Header("ゲームパッドでプレイをするか")]
	private bool _useGamePad;

	//プレイヤーのコントロール
	[HideInInspector]
	public bool PlayerControl = true;

	private void Start()
	{
		//最初はメインカメラを読み込む
		_playerCamera = Camera.main;

		_playerAnimator = GetComponent<Animator>();

		DATABASE.PlayIsGamePad = _useGamePad;
	}

	private void FixedUpdate()
	{
		PlayerMove();

		PlayerAnimation();

		PlayerCommand();
	}

	private void PlayerMove()
	{
		//カメラの方向ベクトルを取得
		var forward = _playerCamera.transform.TransformDirection(Vector3.forward);
		var right = _playerCamera.transform.TransformDirection(Vector3.right);

		if (DATABASE.PlayIsGamePad)
		{
			if (!_laserAttackFlag && !_electricAttackFlag && PlayerControl)
				_moveDirection = GamePad.GetAxis(GamePad.Axis.LeftStick, GamePad.Index.One, true).x * right +
				                 GamePad.GetAxis(GamePad.Axis.LeftStick, GamePad.Index.One, true).y * forward;
		}
		else
		{
			if (!_laserAttackFlag && !_electricAttackFlag && PlayerControl)
				_moveDirection = InputManager.Horizontal * right + InputManager.Vertical * forward;
		}

		//１以上ならば、正規化(Normalize)をする
		if (_moveDirection.magnitude > 1f) _moveDirection.Normalize();

		//ワールド空間での方向をローカル空間に逆変換する
		//※ワールド空間でのカメラは、JoyStickと逆の方向ベクトルを持つため、Inverseをしなければならない
		var inverseDirection = transform.InverseTransformDirection(_moveDirection);

		//アークタンジェントをもとに、最終的になる角度を求める
		_turnAmount = Mathf.Atan2(inverseDirection.x, inverseDirection.z);

		//最終的な前方に代入する
		_moveForwardSpeed = inverseDirection.z;

		//最終的な前方になるまでの時間を計算する
		var turnSpeed = Mathf.Lerp(_stationaryTurnSpeed, _movingTurnSpeed, _moveForwardSpeed);

		//Y軸を最終的な角度になるようにする
		transform.Rotate(0, _turnAmount * turnSpeed * Time.deltaTime, 0);

		//移動スピードを掛ける
		_moveDirection *= _moveSpeed * Time.deltaTime;

		_moveDirection.y = 0;

		//プレイヤーを移動させる
		transform.position += _moveDirection;
	}

	private void PlayerCommand()
	{
		if (DATABASE.PlayIsGamePad)
			Input_GamePad();
		else
			Input_KeyBoard();
	}

	private void Input_GamePad()
	{
		//RBボタンで攻撃
		if (GamePad.GetButton(GamePad.Button.RightShoulder, GamePad.Index.One))
		{
			//レーザー
			if (_isLaserMode && !_electricAttackFlag)
			{
				_laserAttackFlag = true;
				LazerAttack(true);
			}

			//エレクトロ
			if (_isElectricMode && !_laserAttackFlag && !_isLaserMode)
			{
				_electricAttackFlag = true;
				ElectricAttack(true);
			}
		}
		else
		{
			//レーザー
			if (_isLaserMode && _laserAttackFlag)
			{
				_laserAttackFlag = false;
				LazerAttack(false);
			}

			//エレクトロ
			if (_isElectricMode && _electricAttackFlag && !_isLaserMode)
			{
				_electricAttackFlag = false;
				ElectricAttack(false);
			}
		}

		//LBボタンでエイム
		if (GamePad.GetButton(GamePad.Button.LeftShoulder, GamePad.Index.One))
		{
			if (!_electricAttackFlag)
			{
				PlayerControl = false;

				_isLaserMode = true;

				ReticleSystem.Instance.ReticleEnable(true);

				ReticleSystem.Instance.ReticleMove();
			}
		}
		else
		{
			if (!_electricAttackFlag && _isLaserMode)
			{
				PlayerControl = true;

				_isLaserMode = false;

				ReticleSystem.Instance.ReticleEnable(false);

				LazerAttack(false);

				ReticleSystem.Instance.ResetPosition();
			}
		}

		//モード切替
		if (GamePad.GetButtonDown(GamePad.Button.X, GamePad.Index.One))
			if (!_isElectricMode && !_electricAttackFlag)
			{
				foreach (var effect in _settings.ElectricEffect.GetComponentsInChildren<ParticleSystem>())
					effect.Play();

				_isElectricMode = true;
			}
			else if (_isElectricMode && !_electricAttackFlag)
			{
				foreach (var effect in _settings.ElectricEffect.GetComponentsInChildren<ParticleSystem>())
					effect.Stop();

				_isElectricMode = false;
			}
	}

	private void Input_KeyBoard()
	{
		//右クリックで攻撃
		if (InputManager.click_Right)
		{
			//レーザー
			if (_isLaserMode && !_electricAttackFlag)
			{
				_laserAttackFlag = true;
				LazerAttack(true);
			}

			//エレクトロ
			if (_isElectricMode && !_laserAttackFlag && !_isLaserMode)
			{
				_electricAttackFlag = true;
				ElectricAttack(true);
			}
		}
		else
		{
			//レーザー
			if (_isLaserMode && _laserAttackFlag)
			{
				_laserAttackFlag = false;
				LazerAttack(false);
			}

			//エレクトロ
			if (_isElectricMode && _electricAttackFlag && !_isLaserMode)
			{
				_electricAttackFlag = false;
				ElectricAttack(false);
			}
		}

		//左クリックでエイム
		if (InputManager.click_Left)
		{
			if (!_electricAttackFlag)
			{
				PlayerControl = false;

				_isLaserMode = true;

				ReticleSystem.Instance.ReticleEnable(true);

				ReticleSystem.Instance.ReticleMove();
			}
		}
		else
		{
			if (!_electricAttackFlag && _isLaserMode)
			{
				PlayerControl = true;

				_isLaserMode = false;

				ReticleSystem.Instance.ReticleEnable(false);

				LazerAttack(false);

				ReticleSystem.Instance.ResetPosition();
			}
		}

		//モード切替
		if (InputManager.Key_E)
			if (!_isElectricMode && !_electricAttackFlag)
			{
				foreach (var effect in _settings.ElectricEffect.GetComponentsInChildren<ParticleSystem>())
					effect.Play();

				_isElectricMode = true;
			}
			else if (_isElectricMode && !_electricAttackFlag)
			{
				foreach (var effect in _settings.ElectricEffect.GetComponentsInChildren<ParticleSystem>())
					effect.Stop();

				_isElectricMode = false;
			}
	}

	/// <summary>
	///     レーザー攻撃
	/// </summary>
	/// <param name="atk"></param>
	private void LazerAttack(bool atk)
	{
		_settings.LaserLeft.fire = atk;
		_settings.LaserRight.fire = atk;
	}

	/// <summary>
	///     電撃攻撃
	/// </summary>
	/// <param name="atk"></param>
	private void ElectricAttack(bool atk)
	{
		//攻撃
		_playerAnimator.SetBool("ElectricAttack", atk);

		WaitAfter(0.3f, () =>
		{
			if (atk)
				foreach (var effect in _settings.ElectricAttackEffect.GetComponentsInChildren<ParticleSystem>())
					effect.Play();
			else
				foreach (var effect in _settings.ElectricAttackEffect.GetComponentsInChildren<ParticleSystem>())
					effect.Stop();
		});
	}

	/// <summary>
	///     アニメーション制御
	/// </summary>
	private void PlayerAnimation()
	{
		//移動
		_playerAnimator.SetFloat("Speed", _moveDirection.magnitude * 10);
	}
}