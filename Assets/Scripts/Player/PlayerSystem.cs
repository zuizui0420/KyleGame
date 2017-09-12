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

    [SerializeField, Header("CameraSystem")]
    CameraSystem C_System;

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

    //モード：レーザー攻撃
    bool Mode_Laser = false;

    private bool modeN = true;
    private bool modeE = false;
    private bool modeQ = false;
    private bool attackE = false;
    private float count = 0.5f;

    void Start()
    {
        //最初はメインカメラを読み込む
        PlayerCam = Camera.main;

        PlayerAnim = GetComponent<Animator>();

        DATABASE.PlayIsGamePad = PlayIsGamePad;
    }

    void FixedUpdate()
    {
        PlayerInput();
    }

    /// <summary>
    /// プレイヤーの入力
    /// </summary>
    private void PlayerInput()
    {
        if (PlayerControle)
        {
            PlayerMove();

            PlayerAnimation();
        }    

        PlayerCommand();        
    }

    /// <summary>
    /// 移動入力
    /// </summary>
    private void PlayerMove()
    {
        //カメラの方向ベクトルを取得
        Vector3 forward = PlayerCam.transform.TransformDirection(Vector3.forward);
        Vector3 right = PlayerCam.transform.TransformDirection(Vector3.right);

        if (DATABASE.PlayIsGamePad)
        {
            //Axisにカメラの方向ベクトルを掛ける
            moveDirection = GamePad.GetAxis(GamePad.Axis.LeftStick, GamePad.Index.One, true).x * right +
                            GamePad.GetAxis(GamePad.Axis.LeftStick, GamePad.Index.One, true).y * forward;
        }
        else
        {
            //Axisにカメラの方向ベクトルを掛ける
            moveDirection = InputManager.Horizontal * right + InputManager.Vertical * forward;
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

    private void PlayerCommand()
    {
        if (PlayIsGamePad)
        {
            //RTボタンで発射
            if (GamePad.GetButton(GamePad.Button.RightShoulder, GamePad.Index.One))
            {
                Debug.Log("レーザー攻撃");

                LazerAttack(true);
            }
            else
            {
                LazerAttack(false);
            }

            //LTボタンで発射準備
            if (GamePad.GetButton(GamePad.Button.LeftShoulder, GamePad.Index.One))
            {
                PlayerControle = false;

                Mode_Laser = true;

                ReticleSystem.Instance.ReticleEnable(true);

                ReticleSystem.Instance.ReticleMove();         
            }
            else
            {
                PlayerControle = true;

                Mode_Laser = false;

                ReticleSystem.Instance.ReticleEnable(false);

                ReticleSystem.Instance.ResetPosition();
            }
        }
        else
        {
            //右クリックで発射
            if (InputManager.click_Right)
            {
                Debug.Log("レーザー攻撃");

                LazerAttack(true);
            }
            else
            {
                LazerAttack(false);
            }

            //左クリックで発射準備
            if (InputManager.click_Left)
            {
                PlayerControle = false;

                Mode_Laser = true;

                ReticleSystem.Instance.ReticleEnable(true);

                ReticleSystem.Instance.ReticleMove();
            }
            else
            {
                PlayerControle = true;

                Mode_Laser = false;

                ReticleSystem.Instance.ReticleEnable(false);

                ReticleSystem.Instance.ResetPosition();
            }
        }
    }

    private void LazerAttack(bool fire)
    {
        r_Laser.fire = fire;
        l_Laser.fire = fire;
    }

    /// <summary>
    /// アニメーション制御
    /// </summary>
    private void PlayerAnimation()
    {
        //移動
        PlayerAnim.SetFloat("Speed", moveDirection.magnitude * 10);

        //if (Input.GetKeyDown(KeyCode.E))
        //{

        //    if (modeE == false)
        //    {
        //        modeE = true;
        //        modeN = false;
        //        modeQ = false;
        //    }

        //    else if (modeE == true)
        //    {
        //        modeE = false;
        //        modeN = true;
        //    }

        //}

        //if (Input.GetKeyDown(KeyCode.R))
        //{
        //    if (modeQ == false)
        //    {
        //        modeQ = true;
        //        modeE = false;
        //    }
        //    else if (modeQ == true)
        //    {
        //        modeQ = false;
        //        modeN = true;
        //    }
        //}

        //if (Input.GetButton("Fire1") && Input.GetButton("Fire2"))
        //    PlayerAnim.SetBool("Spark", true);

        //if (modeE == false && !Input.GetButton("Fire1"))
        //    PlayerAnim.SetBool("Spark", false);

        //if (modeE == true && Input.GetButton("Fire2"))
        //{
        //    PlayerAnim.SetBool("Spark", true);
        //    count -= Time.deltaTime;
        //    if (count <= 0)
        //        attackE = true;
        //}

        //if (Input.GetButtonUp("Fire2"))
        //{
        //    PlayerAnim.SetBool("Spark", false);
        //    attackE = false;
        //    count = 0.5f;
        //}
    }
}