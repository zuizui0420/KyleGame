using System;
using UnityEngine;
using GamepadInput;
using System.Collections;
using UniRx;
using UniRx.Triggers;

//このクラスでは、プレイヤーの制御をするものです
public class PlayerSystem : SingletonMonoBehaviour<PlayerSystem>
{
	[SerializeField]
	private Camera _thirdPersonCamera;
	[SerializeField]
	private Camera _firstPersonCamera;

    [SerializeField, Header("ゲームパッドでプレイをするか")]
    bool PlayIsGamePad;

    [SerializeField, Header("HealthManager")]
    HealthManager LifeIcon;

    [SerializeField, Header("BatteryControl")]
    BatteryControl BatteryIcon;

	[SerializeField]
	[Range(0, 10)]
	private float _movementSpeed = 5f;

    [SerializeField]
    [Range(0, 1)]
    private float _rotationSpeed;
	
	[SerializeField]
	[Range(0, 360)]
	private float _stationaryTurnSpeed = 360f;
	
    [SerializeField]
    [Range(0, 360)]
    private float _movingTurnSpeed;

    [SerializeField]
    private GameObject _electricEffectWhenPassive;

    [SerializeField]
    private GameObject _electricEffectWhenAttack;

    [SerializeField]
    private GameObject _explosionEffectWhenDead;

    float MoveSpeed;

    //最終角度
    float m_TurnAmount;

    //最終前方
    float m_ForwardAmount;

    //計算後の移動量
    Vector3 moveDirection;

    [SerializeField,Header("プレイヤーの操作")]
    public bool PlayerControle = true;

    [SerializeField, Header("プレイヤーのカメラ操作")]
    public bool PlayerCameraControle = true;

    //プレイヤーのカメラ
    private Camera _currentCamera;

    private Animator _animator;

    //モード：エイム

    public bool IsAimMode { get; set; }

    //モード：放電
    public bool IsSparkMode { get; set; }

    //各モードでの攻撃中かどうか
    public bool LaserAttack = false, SparkAttack = false;

    [SerializeField, Header("デバッグ")]
    bool FirstPerson_DebugTest;

    [SerializeField, Header("電気モード時の移動速度"), Range(1f, 10f)]
    float ElectricModeSpeed;

    public bool IsZooming { get; set; }

    //無敵
	private bool _onVisible;

    private bool _moveFlag = true;

	//private bool _isDead;

	private DamageShader _3rdCameraDamageShader;

	private BoolReactiveProperty _isDead = new BoolReactiveProperty();
	public IReadOnlyReactiveProperty<bool> IsDead { get { return _isDead; } }

    private enum AnimationMode
    {
		/// <summary>
		/// エイム中
		/// </summary>
        Aiming,

		/// <summary>
		/// 放電中
		/// </summary>
        Sparking,
    }

    private void Start()
    {
        //移動速度を設定
        MoveSpeed = _movementSpeed;

        //最初はメインカメラを読み込む
        _currentCamera = Camera.main;

        _animator = GetComponent<Animator>();

        DATABASE.PlayIsGamePad = PlayIsGamePad;

	    _3rdCameraDamageShader = _thirdPersonCamera.GetComponent<DamageShader>();

	    this.UpdateAsObservable().Where(_ => IsAimMode).Subscribe(_ =>
	    {
		    ReticleSystem.Instance.ReticleMove();

		    if (LaserAttack)
		    {
			    AudioManager.Instance.Play(AUDIONAME.SE_LASER, 1, true, 200);
			    AttackLaser(true);
		    }
		    else
		    {
			    AudioManager.Instance.AudioDelete(AUDIONAME.SE_LASER);
			    AttackLaser(false);
		    }
	    });
    }

    void Update()
    {
	    if (_isDead.Value) return;

	    if (BatteryIcon.OverHeat && _moveFlag && !BatteryIcon.DummyOverHeat)
	    {
		    BatteryIcon.BatteryOverHeat();
		    MoveSpeed = _movementSpeed;

		    IsSparkMode = false;

		    if (BatteryIcon.ResetFlg)
		    {
			    Damage();

			    foreach (var effect in _electricEffectWhenPassive.GetComponentsInChildren<ParticleSystem>())
			    {
				    effect.Stop();
			    }

			    SparkAttack = false;
			    AttackAnimation(false, AnimationMode.Sparking);

			    BatteryIcon.ResetFlg = false;
		    }
	    }
	    else if (!BatteryIcon.OverHeat && _moveFlag && BatteryIcon.DummyOverHeat)
	    {
		    BatteryIcon.BatteryDummyOverHeat();
		    MoveSpeed = ElectricModeSpeed;
	    }
	    else if (IsSparkMode && _moveFlag)
	    {
		    BatteryIcon.BatteryUse();
		    MoveSpeed = ElectricModeSpeed;
	    }
	    else if (_moveFlag)
	    {
		    BatteryIcon.BatteryCharge();
		    MoveSpeed = _movementSpeed;
	    }

	    PlayerAnimation();

	    PlayerCommand();

	    AimMode();
    }

    private void FixedUpdate()
    {
        PlayerMove();
    }

    #region 移動入力
    private void PlayerMove()
    {
        //カメラの方向ベクトルを取得
        Vector3 forward = _currentCamera.transform.TransformDirection(Vector3.forward);
        Vector3 right = _currentCamera.transform.TransformDirection(Vector3.right);

        if (DATABASE.PlayIsGamePad)
        {
            if (!LaserAttack && !SparkAttack && PlayerControle)
            {
                //Axisにカメラの方向ベクトルを掛ける
                moveDirection = GamePad.GetAxis(GamePad.Axis.LeftStick, GamePad.Index.One, true).x * right +
                                GamePad.GetAxis(GamePad.Axis.LeftStick, GamePad.Index.One, true).y * forward;
            }           
        }
        else
        {
            if (!LaserAttack && !SparkAttack && PlayerControle)
            {
                //Axisにカメラの方向ベクトルを掛ける
                moveDirection = InputManager.Horizontal * right + InputManager.Vertical * forward;
            }           
        }

        //１以上ならば、正規化(Normalize)をする
        if (moveDirection.magnitude > 1f) moveDirection.Normalize();

        //ワールド空間での方向をローカル空間に逆変換する
        //※ワールド空間でのカメラは、JoyStickと逆の方向ベクトルを持つため、Inverseをしなければならない
        Vector3 C_move = transform.InverseTransformDirection(moveDirection);

        //アークタンジェントをもとに、最終的になる角度を求める
        m_TurnAmount = Mathf.Atan2(C_move.x, C_move.z);

        //最終的な前方に代入する
        m_ForwardAmount = C_move.z;

        //最終的な前方になるまでの時間を計算する
        float turnSpeed = Mathf.Lerp(_stationaryTurnSpeed, _movingTurnSpeed, m_ForwardAmount);

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
        {
            Input_GamePad();
        }
        else
        {
            Input_KeyBoard();
        }
    }

    private void Input_GamePad()
    {
        //動作が不安定------------------------------------------
        //RBボタンで攻撃
        if (GamePad.GetButtonDown(GamePad.Button.RightShoulder, GamePad.Index.One))
        {
            Attack(true);
        }

        if (GamePad.GetButtonUp(GamePad.Button.RightShoulder, GamePad.Index.One))
        {
            Attack(false);
        }

        //LBボタンでエイム
        if (GamePad.GetButtonDown(GamePad.Button.LeftShoulder, GamePad.Index.One))
        {
            Aim(true);
        }

        if (GamePad.GetButtonUp(GamePad.Button.LeftShoulder, GamePad.Index.One))
        {
            Aim(false);
        }

        //モード切替
        if (GamePad.GetButtonDown(GamePad.Button.X, GamePad.Index.One))
        {
            ModeChange();
        }
    }

    private void Input_KeyBoard()
    {
        //右クリックで攻撃
        if (InputManager.click_Right)
        {
            Attack(true);
        }
        else
        {
            Attack(false);
        }

        //左クリックでエイム
        if (InputManager.click_Left || FirstPerson_DebugTest)
        {
            Aim(true);
        }
        else
        {
            Aim(false);
        }

        //モード切替
        if (InputManager.Key_E)
        {
            ModeChange();
        }
    }

    /// <summary>
    /// エイム状態
    /// </summary>
    private void AimMode()
    {
	    
    }

    /// <summary>
    /// 攻撃
    /// </summary>
    /// <param name="atk"></param>
    private void Attack(bool atk)
    {               
        if (!IsZooming && !IsAimMode && IsSparkMode && !BatteryIcon.DummyOverHeat) //放電
        {
            if (atk)
            {
                SparkAttack = true;
                AttackAnimation(true, AnimationMode.Sparking);
            }
            else
            {
                SparkAttack = false;
                AttackAnimation(false, AnimationMode.Sparking);
            }
        }
        else if(IsZooming && IsAimMode) //エイム
        {
            if (atk)
            {
                LaserAttack = true;
            }
            else
            {
                LaserAttack = false;
            }
        }
    }

    /// <summary>
    /// エイム
    /// </summary>
    /// <param name="aim"></param>
    private void Aim(bool aim)
    {
        if (!SparkAttack)
        {
            if (aim) { AttackAnimation(true, AnimationMode.Aiming); }
            else { AttackAnimation(false, AnimationMode.Aiming); AudioManager.Instance.AudioDelete(AUDIONAME.SE_LASER); }
        }
    }

    /// <summary>
    /// モード切替
    /// </summary>
    private void ModeChange()
    {
        if(!IsSparkMode && !SparkAttack && !BatteryIcon.OverHeat)
        {
            AudioManager.Instance.Play(AUDIONAME.SE_SPARK_2, 0.6f, true, 180);

            foreach (ParticleSystem effect in _electricEffectWhenPassive.GetComponentsInChildren<ParticleSystem>())
            {
                effect.Play();
            }

            IsSparkMode = true;            
        }
        else if(!SparkAttack)
        {
            AudioManager.Instance.AudioDelete(AUDIONAME.SE_SPARK_2);

            foreach (ParticleSystem effect in _electricEffectWhenPassive.GetComponentsInChildren<ParticleSystem>())
            {
                effect.Stop();
            }

            IsSparkMode = false;
        }
    }

    /// <summary>
    /// レーザー攻撃
    /// </summary>
    /// <param name="atk"></param>
    private void AttackLaser(bool atk)
    {
	    _isFireReactiveProperty.Value = atk;
    }

	private readonly BoolReactiveProperty _isFireReactiveProperty = new BoolReactiveProperty();
	public IReadOnlyReactiveProperty<bool> IsFire { get { return _isFireReactiveProperty; } }

    #region 構えるモーション
    private void AttackAnimation(bool anim, AnimationMode mode)
    {
        //アニメーション
        _animator.SetBool("ElectricAttack", anim);

        if (anim)
        {
            PlayerControle = false;

            switch (mode)
            {
                case AnimationMode.Aiming:

                    PlayerCameraControle = false;

                    IsAimMode = true;

                    if (IsAimMode)
                    {

                        IsZooming = true;
                        ReticleSystem.Instance.ReticleEnable(true);
                    }

                    break;

                case AnimationMode.Sparking:

                    IsSparkMode = true;

                    AudioManager.Instance.Play(AUDIONAME.SE_SPARK_1, 0.7f, true, 130);

                    foreach (ParticleSystem effect in _electricEffectWhenAttack.GetComponentsInChildren<ParticleSystem>())
                    {
                        effect.Play();
                    }               

                    break;
            }
        }
        else
        {
            switch (mode)
            {
                case AnimationMode.Aiming:

                    IsAimMode = false;

                    ReticleSystem.Instance.ReticleEnable(false);

                    ReticleSystem.Instance.ResetPosition();

                    LaserAttack = false;

                    AttackLaser(false);

                    IsZooming = false;

                    _moveFlag = false;

                    MoveSpeed = 0f;                  

                    WaitAfter(0.3f, () =>
                    {
                        MoveSpeed = _movementSpeed;
                        _moveFlag = true;
                    });

                    if(DATABASE.Life != 0)
                    {
                        PlayerControle = true;
                    }
                    
                    PlayerCameraControle = true;

                    break;

                case AnimationMode.Sparking:

                    AudioManager.Instance.AudioDelete(AUDIONAME.SE_SPARK_1);
                    AudioManager.Instance.AudioDelete(AUDIONAME.SE_SPARK_2);

                    if (!BatteryIcon.OverHeat)
                    {
                        BatteryIcon.DummyOverHeat = true;
                    }                   

                    foreach (ParticleSystem effect in _electricEffectWhenAttack.GetComponentsInChildren<ParticleSystem>())
                    {
                        effect.Stop();
                    }

                    _moveFlag = false;

                    MoveSpeed = 0f;

                    WaitAfter(0.3f, () =>
                    {
                        MoveSpeed = _movementSpeed;
                        _moveFlag = true;

                    });

                    if (DATABASE.Life != 0)
                    {
                        PlayerControle = true;
                    }

                    break;
            }
        }
    }
    #endregion

    /// <summary>
    /// アニメーション制御
    /// </summary>
    private void PlayerAnimation()
    {
        //移動速度０の場合は、Animatorの即時ステートを防ぐために、0.1秒のインターバルを設ける
        if ((moveDirection.magnitude * 10) == 0)
        {
            //_animator.SetFloat("Speed", Mathf.Clamp(moveDirection.magnitude * 10, 0f, 1f));

            WaitAfter(0.05f, () =>
            {
                _animator.SetFloat("Speed", Mathf.Clamp(moveDirection.magnitude * 10, 0f, 1f));
            });
        }
        else
        {
            _animator.SetFloat("Speed", Mathf.Clamp(moveDirection.magnitude * 10, 0f, 1f));
        }           
    }

    public void Damage()
    {
        if (!_onVisible)
        {
            StartCoroutine(DamageCoroutine());
        }
    }

    private IEnumerator DamageCoroutine()
    {
		//ダメージエフェクトを表示する
	    _3rdCameraDamageShader.damageRatio = 0.5f;

		//音
		AudioManager.Instance.Play(AUDIONAME.SE_EXPLOSION_1);

		//体力減少
		DATABASE.Life--;      

        //体力UIを点滅させる
        LifeIcon.StartBlink(DATABASE.Life);

        //体力がない場合は死亡する
        if (DATABASE.Life == 0)
        {
			_isDead.Value = true;

            gameObject.tag = TAGNAME.TAG_UNTAGGED;
            gameObject.layer = LayerMask.NameToLayer("Default");

            ResetValue();

            Debug.Log("死亡");

            LifeIcon.Dead = true;

            _explosionEffectWhenDead.GetComponent<ParticleSystem>().Play();

			AudioManager.Instance.Play(AUDIONAME.SE_EXPLOSION_2, 0.8f, false, 180);

			_animator.SetBool("Death", true);

            PlayerControle = false;
            PlayerCameraControle = false;

            AudioManager.Instance.BGMFadeOut(AUDIONAME.BGM_STAGE);

            WaitAfter(3f, () =>
            {
                SceneFader.Instance.LoadLevel(SceneName.GameOver);
				AudioManager.Instance.AudioDelete(AUDIONAME.SE_EXPLOSION_2);
            });

        }

        //無敵状態にする
        _onVisible = true;

        //徐々にダメージエフェクトを消す
        while(_3rdCameraDamageShader.damageRatio > 0)
        {
	        _3rdCameraDamageShader.damageRatio -= Time.deltaTime;

            yield return null;
        }

		//初期値に設定
	    _3rdCameraDamageShader.damageRatio = 0f;

		AudioManager.Instance.AudioDelete(AUDIONAME.SE_EXPLOSION_1);

	    Observable.Timer(TimeSpan.FromSeconds(1)).Subscribe(_ => _onVisible = false);
    }

    /// <summary>
    /// 全ての状態をリセットする
    /// </summary>
    private void ResetValue()
    {
	    foreach (var effect in _electricEffectWhenPassive.GetComponentsInChildren<ParticleSystem>())
		    effect.Stop();

	    ReticleSystem.Instance.ReticleEnable(false);

		ReticleSystem.Instance.ResetPosition();

		AttackLaser(false);

		IsZooming = false;

		_moveFlag = false;

		IsAimMode = false;
        IsSparkMode = false;

        LaserAttack = false;
        SparkAttack = false;

        AudioManager.Instance.AudioDelete(AUDIONAME.SE_LASER);
        AudioManager.Instance.AudioDelete(AUDIONAME.SE_SPARK_1);
        AudioManager.Instance.AudioDelete(AUDIONAME.SE_SPARK_2);
    }
}