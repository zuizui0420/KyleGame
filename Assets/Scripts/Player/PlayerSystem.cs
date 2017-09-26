using GamepadInput;
using UnityEngine;

//このクラスでは、プレイヤーの制御をするものです
public class PlayerSystem : SingletonMonoBehaviour<PlayerSystem>
{
	[SerializeField]
	[Header("エレクトロモード用エフェクト")]
	private GameObject Effect_Electric;

	[SerializeField]
	[Header("エレクトロモード用攻撃エフェクト")]
	private GameObject Effect_Electric_Attack;

	[SerializeField]
	[Header("LaserLeft")]
	private Attack_Laser l_Laser;

	//各モードでの攻撃中かどうか
	private bool LaserAttacking, ElectricAttacking;

	//最終前方
	private float m_ForwardAmount;

	[Header("動いている状態からの回転量")]
	[SerializeField]
	[Range(0, 360)]
	private float m_MovingTurnSpeed;

	[Header("静止状態からの回転量")]
	[SerializeField]
	[Range(0, 360)]
	private float m_StationaryTurnSpeed;

	//最終角度
	private float m_TurnAmount;

	//モード：エレクトリ
	private bool Mode_Electric;

	//モード：レーザー
	private bool Mode_Laser;

	//計算後の移動量
	private Vector3 moveDirection;

	[Header("移動量")]
	[SerializeField]
	[Range(0, 10)]
	private float MoveSpeed;

	private Animator PlayerAnim;

	//プレイヤーのカメラ
	private Camera PlayerCam;

	//プレイヤーのコントロール
	[HideInInspector]
	public bool PlayerControle = true;

	[SerializeField]
	[Header("ゲームパッドでプレイをするか")]
	private bool PlayIsGamePad;

	[SerializeField]
	[Header("RightLaser")]
	private Attack_Laser r_Laser;

	[Header("回転量")]
	[SerializeField]
	[Range(0, 1)]
	private float rotateSpeed;

	private void Start()
	{
		//最初はメインカメラを読み込む
		PlayerCam = Camera.main;

		PlayerAnim = GetComponent<Animator>();

		DATABASE.PlayIsGamePad = PlayIsGamePad;
	}

	private void FixedUpdate()
	{
		PlayerMove();

		PlayerAnimation();

		PlayerCommand();
	}

	#region 移動入力

	private void PlayerMove()
	{
		//カメラの方向ベクトルを取得
		var forward = PlayerCam.transform.TransformDirection(Vector3.forward);
		var right = PlayerCam.transform.TransformDirection(Vector3.right);

		if (DATABASE.PlayIsGamePad)
		{
			if (!LaserAttacking && !ElectricAttacking && PlayerControle)
				moveDirection = GamePad.GetAxis(GamePad.Axis.LeftStick, GamePad.Index.One, true).x * right +
				                GamePad.GetAxis(GamePad.Axis.LeftStick, GamePad.Index.One, true).y * forward;
		}
		else
		{
			if (!LaserAttacking && !ElectricAttacking && PlayerControle)
				moveDirection = InputManager.Horizontal * right + InputManager.Vertical * forward;
		}

		//１以上ならば、正規化(Normalize)をする
		if (moveDirection.magnitude > 1f) moveDirection.Normalize();

		//ワールド空間での方向をローカル空間に逆変換する
		//※ワールド空間でのカメラは、JoyStickと逆の方向ベクトルを持つため、Inverseをしなければならない
		var C_move = transform.InverseTransformDirection(moveDirection);

		//アークタンジェントをもとに、最終的になる角度を求める
		m_TurnAmount = Mathf.Atan2(C_move.x, C_move.z);

		//最終的な前方に代入する
		m_ForwardAmount = C_move.z;

		//最終的な前方になるまでの時間を計算する
		var turnSpeed = Mathf.Lerp(m_StationaryTurnSpeed, m_MovingTurnSpeed, m_ForwardAmount);

		//Y軸を最終的な角度になるようにする
		transform.Rotate(0, m_TurnAmount * turnSpeed * Time.deltaTime, 0);

		//移動スピードを掛ける
		moveDirection *= MoveSpeed * Time.deltaTime;

		moveDirection.y = 0;

		//プレイヤーを移動させる
		transform.position += moveDirection;
	}

	#endregion

	private void PlayerCommand()
	{
		if (DATABASE.PlayIsGamePad)
			Input_GamePad();
		else
			Input_KeyBoard();
	}

	private void Input_GamePad()
	{
		#region 攻撃

		//RBボタンで攻撃
		if (GamePad.GetButton(GamePad.Button.RightShoulder, GamePad.Index.One))
		{
			//レーザー
			if (Mode_Laser && !ElectricAttacking)
			{
				LaserAttacking = true;
				LazerAttack(true);
			}

			//エレクトロ
			if (Mode_Electric && !LaserAttacking && !Mode_Laser)
			{
				ElectricAttacking = true;
				ElectricAttack(true);
			}
		}
		else
		{
			//レーザー
			if (Mode_Laser && LaserAttacking)
			{
				LaserAttacking = false;
				LazerAttack(false);
			}

			//エレクトロ
			if (Mode_Electric && ElectricAttacking && !Mode_Laser)
			{
				ElectricAttacking = false;
				ElectricAttack(false);
			}
		}

		#endregion

		#region エイム

		//LBボタンでエイム
		if (GamePad.GetButton(GamePad.Button.LeftShoulder, GamePad.Index.One))
		{
			if (!ElectricAttacking)
			{
				PlayerControle = false;

				Mode_Laser = true;

				ReticleSystem.Instance.ReticleEnable(true);

				ReticleSystem.Instance.ReticleMove();
			}
		}
		else
		{
			if (!ElectricAttacking && Mode_Laser)
			{
				PlayerControle = true;

				Mode_Laser = false;

				ReticleSystem.Instance.ReticleEnable(false);

				LazerAttack(false);

				ReticleSystem.Instance.ResetPosition();
			}
		}

		#endregion

		#region モード切替

		//モード切替
		if (GamePad.GetButtonDown(GamePad.Button.X, GamePad.Index.One))
			if (!Mode_Electric && !ElectricAttacking)
			{
				foreach (var effect in Effect_Electric.GetComponentsInChildren<ParticleSystem>())
					effect.Play();

				Mode_Electric = true;
			}
			else if (Mode_Electric && !ElectricAttacking)
			{
				foreach (var effect in Effect_Electric.GetComponentsInChildren<ParticleSystem>())
					effect.Stop();

				Mode_Electric = false;
			}

		#endregion
	}

	private void Input_KeyBoard()
	{
		#region 攻撃

		//右クリックで攻撃
		if (InputManager.click_Right)
		{
			//レーザー
			if (Mode_Laser && !ElectricAttacking)
			{
				LaserAttacking = true;
				LazerAttack(true);
			}

			//エレクトロ
			if (Mode_Electric && !LaserAttacking && !Mode_Laser)
			{
				ElectricAttacking = true;
				ElectricAttack(true);
			}
		}
		else
		{
			//レーザー
			if (Mode_Laser && LaserAttacking)
			{
				LaserAttacking = false;
				LazerAttack(false);
			}

			//エレクトロ
			if (Mode_Electric && ElectricAttacking && !Mode_Laser)
			{
				ElectricAttacking = false;
				ElectricAttack(false);
			}
		}

		#endregion

		#region エイム

		//左クリックでエイム
		if (InputManager.click_Left)
		{
			if (!ElectricAttacking)
			{
				PlayerControle = false;

				Mode_Laser = true;

				ReticleSystem.Instance.ReticleEnable(true);

				ReticleSystem.Instance.ReticleMove();
			}
		}
		else
		{
			if (!ElectricAttacking && Mode_Laser)
			{
				PlayerControle = true;

				Mode_Laser = false;

				ReticleSystem.Instance.ReticleEnable(false);

				LazerAttack(false);

				ReticleSystem.Instance.ResetPosition();
			}
		}

		#endregion

		#region モード切替

		//モード切替
		if (InputManager.Key_E)
			if (!Mode_Electric && !ElectricAttacking)
			{
				foreach (var effect in Effect_Electric.GetComponentsInChildren<ParticleSystem>())
					effect.Play();

				Mode_Electric = true;
			}
			else if (Mode_Electric && !ElectricAttacking)
			{
				foreach (var effect in Effect_Electric.GetComponentsInChildren<ParticleSystem>())
					effect.Stop();

				Mode_Electric = false;
			}

		#endregion
	}

	/// <summary>
	///     レーザー攻撃
	/// </summary>
	/// <param name="atk"></param>
	private void LazerAttack(bool atk)
	{
		r_Laser.fire = atk;
		l_Laser.fire = atk;
	}

	/// <summary>
	///     電撃攻撃
	/// </summary>
	/// <param name="atk"></param>
	private void ElectricAttack(bool atk)
	{
		//攻撃
		PlayerAnim.SetBool("ElectricAttack", atk);

		WaitAfter(0.3f, () =>
		{
			if (atk)
				foreach (var effect in Effect_Electric_Attack.GetComponentsInChildren<ParticleSystem>())
					effect.Play();
			else
				foreach (var effect in Effect_Electric_Attack.GetComponentsInChildren<ParticleSystem>())
					effect.Stop();
		});
	}

	/// <summary>
	///     アニメーション制御
	/// </summary>
	private void PlayerAnimation()
	{
		//移動
		PlayerAnim.SetFloat("Speed", moveDirection.magnitude * 10);
	}
}