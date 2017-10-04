using UnityEngine;
using GamepadInput;
using System.Collections;

//このクラスでは、プレイヤーの制御をするものです
public class PlayerSystem : SingletonMonoBehaviour<PlayerSystem>
{
    [SerializeField, Header("ゲームパッドでプレイをするか")]
    bool PlayIsGamePad;

    [SerializeField, Header("3人称カメラ")]
    Camera ThirdPerson_Cam;

    [SerializeField, Header("1人称カメラ")]
    Camera FirstPerson_Cam;

    [SerializeField, Header("LifeControl")]
    LifeControl LifeIcon;

    [SerializeField, Header("BatteryControl")]
    BatteryControl BatteryIcon;

    [Header("移動量")]
    [SerializeField]
    [Range(0, 10)]
    float DefaultSpeed;

    [Header("回転量")]
    [SerializeField]
    [Range(0, 1)]
    float rotateSpeed;

    [Header("静止状態からの回転量")]
    [SerializeField]
    [Range(0, 360)]
    float m_StationaryTurnSpeed;

    [Header("動いている状態からの回転量")]
    [SerializeField]
    [Range(0, 360)]
    float m_MovingTurnSpeed;

    [SerializeField, Header("Attack_Laser")]
    Attack_Laser LaserSystem;

    [SerializeField, Header("エレクトロモード用エフェクト")]
    GameObject Effect_Electric;

    [SerializeField, Header("エレクトロモード用攻撃エフェクト")]
    GameObject Effect_Electric_Attack;

    [SerializeField, Header("死亡時の爆発エフェクト")]
    GameObject Effect_Explosion;

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
    Camera PlayerCam;

    Animator PlayerAnim;

    //モード：エイム
    public bool Mode_Aim = false;

    //モード：放電
    public bool Mode_Spark = false;

    //各モードでの攻撃中かどうか
    public bool LaserAttack = false, SparkAttack = false;

    [SerializeField, Header("デバッグ")]
    bool FirstPerson_DebugTest;

    [SerializeField, Header("電気モード時の移動速度"), Range(1f, 10f)]
    float ElectricModeSpeed;

    public bool Zooming = false;

    //無敵
    bool OnVisible = false;

    bool MoveFlg = true;

    enum ANIMATION_MODE
    {
        AIM,
        SPARK,
    }

    void Start()
    {
        //移動速度を設定
        MoveSpeed = DefaultSpeed;

        //最初はメインカメラを読み込む
        PlayerCam = Camera.main;

        PlayerAnim = GetComponent<Animator>();

        DATABASE.PlayIsGamePad = PlayIsGamePad;
    }

    void Update()
    {
        if (BatteryIcon.OverHeat && MoveFlg && !BatteryIcon.DummyOverHeat)
        {
            BatteryIcon.BatteryOverHeat();
            MoveSpeed = DefaultSpeed;

            Mode_Spark = false;

            if (BatteryIcon.ResetFlg)
            {
                Damage();

                foreach (ParticleSystem effect in Effect_Electric.GetComponentsInChildren<ParticleSystem>())
                {
                    effect.Stop();
                }

                SparkAttack = false;
                AttackAnimation(false, ANIMATION_MODE.SPARK);

                BatteryIcon.ResetFlg = false;         
            }
        }
        else if (!BatteryIcon.OverHeat && MoveFlg && BatteryIcon.DummyOverHeat)
        {
            BatteryIcon.BatteryDummyOverHeat();
            MoveSpeed = ElectricModeSpeed;
        }
        else if (Mode_Spark && MoveFlg)
        {
            BatteryIcon.BatteryUse();
            MoveSpeed = ElectricModeSpeed;
        }
        else if(MoveFlg)
        {
            BatteryIcon.BatteryCharge();
            MoveSpeed = DefaultSpeed;
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
        Vector3 forward = PlayerCam.transform.TransformDirection(Vector3.forward);
        Vector3 right = PlayerCam.transform.TransformDirection(Vector3.right);

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
        float turnSpeed = Mathf.Lerp(m_StationaryTurnSpeed, m_MovingTurnSpeed, m_ForwardAmount);

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
        if (Mode_Aim)
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
        }
    }

    /// <summary>
    /// 攻撃
    /// </summary>
    /// <param name="atk"></param>
    private void Attack(bool atk)
    {               
        if (!Zooming && !Mode_Aim && Mode_Spark && !BatteryIcon.DummyOverHeat) //放電
        {
            if (atk)
            {
                SparkAttack = true;
                AttackAnimation(true, ANIMATION_MODE.SPARK);
            }
            else
            {
                SparkAttack = false;
                AttackAnimation(false, ANIMATION_MODE.SPARK);
            }
        }
        else if(Zooming && Mode_Aim) //エイム
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
            if (aim) { AttackAnimation(true, ANIMATION_MODE.AIM); }
            else { AttackAnimation(false, ANIMATION_MODE.AIM); AudioManager.Instance.AudioDelete(AUDIONAME.SE_LASER); }
        }
    }

    /// <summary>
    /// モード切替
    /// </summary>
    private void ModeChange()
    {
        if(!Mode_Spark && !SparkAttack && !BatteryIcon.OverHeat)
        {
            AudioManager.Instance.Play(AUDIONAME.SE_SPARK_2, 0.6f, true, 180);

            foreach (ParticleSystem effect in Effect_Electric.GetComponentsInChildren<ParticleSystem>())
            {
                effect.Play();
            }

            Mode_Spark = true;            
        }
        else if(!SparkAttack)
        {
            AudioManager.Instance.AudioDelete(AUDIONAME.SE_SPARK_2);

            foreach (ParticleSystem effect in Effect_Electric.GetComponentsInChildren<ParticleSystem>())
            {
                effect.Stop();
            }

            Mode_Spark = false;
        }
    }

    /// <summary>
    /// レーザー攻撃
    /// </summary>
    /// <param name="atk"></param>
    private void AttackLaser(bool atk)
    {      
        LaserSystem.fire = atk;
    }

    #region 構えるモーション
    private void AttackAnimation(bool anim, ANIMATION_MODE mode)
    {
        //アニメーション
        PlayerAnim.SetBool("ElectricAttack", anim);

        if (anim)
        {
            PlayerControle = false;

            switch (mode)
            {
                case ANIMATION_MODE.AIM:

                    PlayerCameraControle = false;

                    Mode_Aim = true;

                    if (Mode_Aim)
                    {

                        Zooming = true;
                        ReticleSystem.Instance.ReticleEnable(true);
                    }

                    break;

                case ANIMATION_MODE.SPARK:

                    Mode_Spark = true;

                    AudioManager.Instance.Play(AUDIONAME.SE_SPARK_1, 0.7f, true, 130);

                    foreach (ParticleSystem effect in Effect_Electric_Attack.GetComponentsInChildren<ParticleSystem>())
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
                case ANIMATION_MODE.AIM:

                    Mode_Aim = false;

                    ReticleSystem.Instance.ReticleEnable(false);

                    ReticleSystem.Instance.ResetPosition();

                    LaserAttack = false;

                    AttackLaser(false);

                    Zooming = false;

                    MoveFlg = false;

                    MoveSpeed = 0f;                  

                    WaitAfter(0.3f, () =>
                    {
                        MoveSpeed = DefaultSpeed;
                        MoveFlg = true;
                    });

                    if(DATABASE.Life != 0)
                    {
                        PlayerControle = true;
                    }
                    
                    PlayerCameraControle = true;

                    break;

                case ANIMATION_MODE.SPARK:

                    AudioManager.Instance.AudioDelete(AUDIONAME.SE_SPARK_1);
                    AudioManager.Instance.AudioDelete(AUDIONAME.SE_SPARK_2);

                    if (!BatteryIcon.OverHeat)
                    {
                        BatteryIcon.DummyOverHeat = true;
                    }                   

                    foreach (ParticleSystem effect in Effect_Electric_Attack.GetComponentsInChildren<ParticleSystem>())
                    {
                        effect.Stop();
                    }

                    MoveFlg = false;

                    MoveSpeed = 0f;

                    WaitAfter(0.3f, () =>
                    {
                        MoveSpeed = DefaultSpeed;
                        MoveFlg = true;

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
            //PlayerAnim.SetFloat("Speed", Mathf.Clamp(moveDirection.magnitude * 10, 0f, 1f));

            WaitAfter(0.05f, () =>
            {
                PlayerAnim.SetFloat("Speed", Mathf.Clamp(moveDirection.magnitude * 10, 0f, 1f));
            });
        }
        else
        {
            PlayerAnim.SetFloat("Speed", Mathf.Clamp(moveDirection.magnitude * 10, 0f, 1f));
        }           
    }

    public void Damage()
    {
        if (!OnVisible)
        {
            StartCoroutine(DamageCorutine());
        }
    }

    private IEnumerator DamageCorutine()
    {
        //ダメージエフェクトを表示する
        ThirdPerson_Cam.GetComponent<DamageShader>().damageRatio = 0.5f;

		//音
		AudioManager.Instance.Play(AUDIONAME.SE_EXPLOSION_1);

		//体力減少
		DATABASE.Life--;      

        //体力UIを点滅させる
        LifeIcon.StartBlink(DATABASE.Life);

        //体力がない場合は死亡する
        if (DATABASE.Life == 0)
        {
            gameObject.tag = TAGNAME.TAG_UNTAGGED;
            gameObject.layer = LayerMask.NameToLayer("Default");

            Debug.Log("死亡");

            LifeIcon.Dead = true;

            Effect_Explosion.GetComponent<ParticleSystem>().Play();

			AudioManager.Instance.Play(AUDIONAME.SE_EXPLOSION_2, 0.8f, false, 180);

			PlayerAnim.SetBool("Death", true);

            PlayerControle = false;
            PlayerCameraControle = false;

            AudioManager.Instance.BGMFadeOut(AUDIONAME.BGM_STAGE);

            WaitAfter(3f, () =>
            {
                SceneFader.Instance.LoadLevel(SCENENAME.SCENE_GAMEOVER);
				AudioManager.Instance.AudioDelete(AUDIONAME.SE_EXPLOSION_2);
            });

        }

        //無敵状態にする
        OnVisible = true;

        //徐々にダメージエフェクトを消す
        while(ThirdPerson_Cam.GetComponent<DamageShader>().damageRatio > 0)
        {
            ThirdPerson_Cam.GetComponent<DamageShader>().damageRatio -= Time.deltaTime;

            yield return null;
        }

        //初期値に設定
        ThirdPerson_Cam.GetComponent<DamageShader>().damageRatio = 0f;

		AudioManager.Instance.AudioDelete(AUDIONAME.SE_EXPLOSION_1);

		//無敵状態を解除
		WaitAfter(1f, () =>
        {
            OnVisible = false;          
        });     
    }


}