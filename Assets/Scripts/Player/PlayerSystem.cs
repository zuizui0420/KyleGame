using UnityEngine;
using GamepadInput;
using System.Collections;

//このクラスでは、プレイヤーの制御をするものです
public class PlayerSystem : SingletonMonoBehaviour<PlayerSystem>
{
    [SerializeField, Header("ゲームパッドでプレイをするか")]
    bool PlayIsGamePad;

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

    [SerializeField, Header("RightLaser")]
    Attack_Laser r_Laser;

    [SerializeField, Header("LaserLeft")]
    Attack_Laser l_Laser;

    [SerializeField, Header("エレクトロモード用エフェクト")]
    GameObject Effect_Electric;

    [SerializeField, Header("エレクトロモード用攻撃エフェクト")]
    GameObject Effect_Electric_Attack;

    float MoveSpeed;

    //最終角度
    float m_TurnAmount;

    //最終前方
    float m_ForwardAmount;

    //計算後の移動量
    Vector3 moveDirection;

    //プレイヤーのコントロール
    [SerializeField]
    public bool PlayerControle = true;

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

    void FixedUpdate()
    {
        if (Mode_Spark) { MoveSpeed = ElectricModeSpeed; }
        else { MoveSpeed = DefaultSpeed; }

        PlayerMove();

        PlayerAnimation();

        PlayerCommand();        

        AimMode();
        //SparkMode();
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
                LazerAttack(true);
            }
            else
            {
                LazerAttack(false);
            }           
        }
    }

    /// <summary>
    /// 放電状態
    /// </summary>
    private void SparkMode()
    {
        if (Mode_Spark)
        {
            if (SparkAttack)
            {
                AttackAnimation(true,ANIMATION_MODE.SPARK);
            }

            if(!SparkAttack)
            {
                AttackAnimation(false, ANIMATION_MODE.SPARK);
            }
        }
    }

    /// <summary>
    /// 攻撃
    /// </summary>
    /// <param name="atk"></param>
    private void Attack(bool atk)
    {               
        if (Mode_Spark && !Mode_Aim) //放電
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
        else if(Mode_Aim) //エイム
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
            else { AttackAnimation(false, ANIMATION_MODE.AIM); }
        }
    }

    /// <summary>
    /// モード切替
    /// </summary>
    private void ModeChange()
    {
        if(!Mode_Spark && !SparkAttack)
        {
            foreach (ParticleSystem effect in Effect_Electric.GetComponentsInChildren<ParticleSystem>())
            {
                effect.Play();
            }

            Mode_Spark = true;            
        }
        else
        {
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
    private void LazerAttack(bool atk)
    {
        r_Laser.fire = atk;
        l_Laser.fire = atk;
    }

    /// <summary>
    /// 構えるアニメーション
    /// </summary>
    private void AttackAnimation(bool anim, ANIMATION_MODE mode)
    {
        //アニメーション
        PlayerAnim.SetBool("ElectricAttack", anim);

        Debug.Log(PlayerControle);

        if (anim)
        {
            PlayerControle = false;

            switch (mode)
            {
                case ANIMATION_MODE.AIM:

                    Debug.Log("エイム中");                   

                    Mode_Aim = true;

                    WaitAfter(0.3f, () => 
                    {
                        if (Mode_Aim)
                        {
                            Zooming = true;
                            ReticleSystem.Instance.ReticleEnable(true);
                        }           
                    });

                    break;

                case ANIMATION_MODE.SPARK:

                    Mode_Spark = true;

                    WaitAfter(0.3f, () =>
                    {
                        foreach (ParticleSystem effect in Effect_Electric_Attack.GetComponentsInChildren<ParticleSystem>())
                        {
                            effect.Play();
                        }
                    });                   

                    break;
            }
        }
        else
        {
            switch (mode)
            {
                case ANIMATION_MODE.AIM:

                    Debug.Log("エイム解除");

                    Mode_Aim = false;

                    ReticleSystem.Instance.ReticleEnable(false);

                    ReticleSystem.Instance.ResetPosition();

                    LazerAttack(false);

                    Zooming = false;

                    WaitAfter(0.3f, () =>
                    {
                        PlayerControle = true;
                    });

                    break;

                case ANIMATION_MODE.SPARK:

                    Mode_Spark = true;

                    foreach (ParticleSystem effect in Effect_Electric_Attack.GetComponentsInChildren<ParticleSystem>())
                    {
                        effect.Stop();
                    }

                    WaitAfter(0.3f, () =>
                    {
                        PlayerControle = true;
                    });                   

                    break;
            }
        }
    }

    /// <summary>
    /// アニメーション制御
    /// </summary>
    private void PlayerAnimation()
    {
        //移動速度０の場合は、Animatorの即時ステートを防ぐために、0.1秒のインターバルを設ける
        if ((moveDirection.magnitude * 10) == 0)
        {
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
}