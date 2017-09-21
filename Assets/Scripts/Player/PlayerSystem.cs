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
    float MoveSpeed;

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

    //最終角度
    float m_TurnAmount;

    //最終前方
    float m_ForwardAmount;

    //計算後の移動量
    Vector3 moveDirection;

    //プレイヤーのコントロール
    [HideInInspector]
    public bool PlayerControle = true;

    //プレイヤーのカメラ
    Camera PlayerCam;

    Animator PlayerAnim;

    //モード：レーザー
    [HideInInspector]
    public bool Mode_Laser = false;

    //モード：エレクトリ
    bool Mode_Electric = false;

    //各モードでの攻撃中かどうか
    bool LaserAttacking = false, ElectricAttacking = false;

    [SerializeField, Header("デバッグ")]
    bool FirstPerson_DebugTest;

    void Start()
    {
        //最初はメインカメラを読み込む
        PlayerCam = Camera.main;

        PlayerAnim = GetComponent<Animator>();

        DATABASE.PlayIsGamePad = PlayIsGamePad;
    }

    void FixedUpdate()
    {
        PlayerMove();

        PlayerAnimation();

        PlayerCommand();
    }

    #region 移動入力
    private void PlayerMove()
    {
        //カメラの方向ベクトルを取得
        Vector3 forward = PlayerCam.transform.TransformDirection(Vector3.forward);
        Vector3 right = PlayerCam.transform.TransformDirection(Vector3.right);

        if (DATABASE.PlayIsGamePad)
        {
            if (!LaserAttacking && !ElectricAttacking && PlayerControle)
            {
                //Axisにカメラの方向ベクトルを掛ける
                moveDirection = GamePad.GetAxis(GamePad.Axis.LeftStick, GamePad.Index.One, true).x * right +
                                GamePad.GetAxis(GamePad.Axis.LeftStick, GamePad.Index.One, true).y * forward;
            }           
        }
        else
        {
            if (!LaserAttacking && !ElectricAttacking && PlayerControle)
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
        if (GamePad.GetButton(GamePad.Button.LeftShoulder, GamePad.Index.One) || FirstPerson_DebugTest)
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
        {
            if (!Mode_Electric && !ElectricAttacking)
            {
                foreach (ParticleSystem effect in Effect_Electric.GetComponentsInChildren<ParticleSystem>())
                {
                    effect.Play();
                }

                Mode_Electric = true;
            }
            else if (Mode_Electric && !ElectricAttacking)
            {
                foreach (ParticleSystem effect in Effect_Electric.GetComponentsInChildren<ParticleSystem>())
                {
                    effect.Stop();
                }

                Mode_Electric = false;
            }
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
        if (InputManager.click_Left || FirstPerson_DebugTest)
        {
            if (!ElectricAttacking && !Mode_Laser)
            {
                AimMode(true);
            }
            else if (!ElectricAttacking)
            {
                ReticleSystem.Instance.ReticleMove();
            }
        }
        else
        {
            if (!ElectricAttacking && Mode_Laser)
            {
                AimMode(false);
            }
        }
        #endregion

        #region モード切替
        //モード切替
        if (InputManager.Key_E)
        {
            if (!Mode_Electric && !ElectricAttacking)
            {
                foreach (ParticleSystem effect in Effect_Electric.GetComponentsInChildren<ParticleSystem>())
                {
                    effect.Play();
                }

                Mode_Electric = true;
            }
            else if (Mode_Electric && !ElectricAttacking)
            {
                foreach (ParticleSystem effect in Effect_Electric.GetComponentsInChildren<ParticleSystem>())
                {
                    effect.Stop();
                }

                Mode_Electric = false;
            }
        }
        #endregion
    }

    private void AimMode(bool aim)
    {
        //攻撃準備
        PlayerAnim.SetBool("ElectricAttack", aim);

        if (aim)
        {
            PlayerControle = false;

            WaitAfter(0.3f, () =>
            {
                Mode_Laser = true;

                ReticleSystem.Instance.ReticleEnable(true);         
            });          
        }
        else
        {
            Mode_Laser = false;

            ReticleSystem.Instance.ReticleEnable(false);

            ReticleSystem.Instance.ResetPosition();

            LazerAttack(false);

            WaitAfter(0.3f, () =>
            {
                PlayerControle = true;
            });  
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
    /// 電撃攻撃
    /// </summary>
    /// <param name="atk"></param>
    private void ElectricAttack(bool atk)
    {
        //攻撃
        PlayerAnim.SetBool("ElectricAttack", atk);        

        WaitAfter(0.3f, () =>
        {
            if (atk)
            {
                PlayerControle = false;

                foreach (ParticleSystem effect in Effect_Electric_Attack.GetComponentsInChildren<ParticleSystem>())
                {
                    effect.Play();
                }
            }
            else
            {
                PlayerControle = true;

                foreach (ParticleSystem effect in Effect_Electric_Attack.GetComponentsInChildren<ParticleSystem>())
                {
                    effect.Stop();
                }
            }
        });
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