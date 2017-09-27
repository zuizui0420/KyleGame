using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敵：タレット
/// </summary>
public class TurretControle : GimmickBase
{
    [SerializeField, Header("起動しているかどうか")]
    bool Starting;

    [SerializeField, Header("タレット")]
    GameObject Turret;

    [SerializeField, Header("弾丸を生成する座標")]
    GameObject[] ShotPoint;

    [SerializeField, Header("タレット用の弾丸オブジェクト")]
    GameObject Turret_Bullet;

    [SerializeField, Header("全方向検知")]
    bool AroundCheck;

    //攻撃時間
    float AttackTime = 5f;

    //次攻撃までのインターバル
    float AttackInterval = 2f;

    //攻撃態勢かどうか
    bool AttackMode = false;

    //攻撃待機状態かどうか
    bool AttackWaiting = false;

    //検知したターゲット
    GameObject Target;

    //タレットの回転速度
    float turretAngularSpeed = 20f;

    //Idle用の初期Ｙ角度
    float DefaultAngle;

    void Start ()
    {
        //初期Y角度を取得
        DefaultAngle = Turret.transform.localEulerAngles.y;

        if (!Starting)
        {
            //初期は起動していない状態にする
            Turret.transform.localEulerAngles = new Vector3(30f, 0f, 0f);
        }       
	}
	
	void Update ()
    {
        //攻撃態勢
        if (AttackMode && !AttackWaiting && Starting)
        {
            //検知したターゲットの方向を向き続ける
            RotateToTarget();
        }
        else if(!AttackMode && !AttackWaiting && Starting)
        {
            AngleRay();

            IdleTurret();
        }

        //デバッグ用：タレット起動スイッチ
        if (Input.GetKeyDown(KeyCode.P))
        {
            GimmickAction_Enemy();
        }      
	}

    /// <summary>
    /// ターゲット検知用Ray
    /// </summary>
    private void AngleRay()
    {
        RaycastHit hit;

        if(Physics.Raycast(Turret.transform.position,Turret.transform.forward, out hit))
        {
            if(hit.collider.gameObject.tag == "Player")
            {
                //ターゲットを設定
                Target = hit.collider.gameObject;

                //攻撃態勢に移行
                AttackMode = true;

                //攻撃を開始
                StartCoroutine(Attack());
            }
        }
    }

    protected override void GimmickAction_Enemy()
    {
        Debug.Log("タレット起動");

        StartCoroutine(StartUpAnimation());
    }

    /// <summary>
    /// 起動時のアニメーション
    /// </summary>
    /// <returns></returns>
    private IEnumerator StartUpAnimation()
    {
        float currentTime = 2f;

        while (currentTime > 0)
        {
            currentTime -= Time.deltaTime;

            Quaternion rot = Quaternion.AngleAxis(0f, transform.right);
            Turret.transform.rotation = Quaternion.RotateTowards(Turret.transform.rotation, rot, 1f);

            yield return null;
        }

        Turret.transform.rotation = Quaternion.identity;

        Starting = true;
    }

    /// <summary>
    /// 攻撃
    /// </summary>
    /// <returns></returns>
    private IEnumerator Attack()
    {
        //攻撃時間を取得
        float AttackcurrentTime = AttackTime;

        //弾丸生成時間
        float BulletInsTime = 0.5f;

        //ガトリングを発射する
        while(AttackcurrentTime > 0)
        {
            AttackcurrentTime -= Time.deltaTime;

            BulletInsTime -= Time.deltaTime;

            if(BulletInsTime < 0f)
            {
                foreach (GameObject Point in ShotPoint)
                {
                    Instantiate(Turret_Bullet, Point.transform.position, Point.transform.rotation);
                }

                BulletInsTime = 0.5f;
            }

            yield return null;               
        }

        if (AroundCheck)
        {
            DefaultAngle = Turret.transform.localEulerAngles.y;
        }        

        AttackMode = false;

        AttackWaiting = true;

        yield return new WaitForSeconds(AttackInterval);

        AttackWaiting = false;

        Debug.Log("再度検知を開始");       
    }

    private void RotateToTarget()
    {
        Quaternion rot = Quaternion.LookRotation(GetPlayerAngle_NotY());

        Turret.transform.rotation = Quaternion.Slerp(Turret.transform.rotation, rot, Time.deltaTime * turretAngularSpeed);
    }

    /// <summary>
    /// 待機状態のタレットの回転
    /// </summary>
    private void IdleTurret()
    {
        if (AroundCheck)
        {
            //回転の間隔
            const float IdleAngle = 10f;

            //Sin波(-IdleAngle～IdleAngleの間)
            float y = Mathf.Sin(Time.time) * IdleAngle;

            Turret.transform.localEulerAngles = new Vector3(0f, DefaultAngle + y, 0f);
        }
        else
        {
            if(Turret.transform.localEulerAngles.y != 0)
            {
                float YAngle = Mathf.Lerp(Turret.transform.localEulerAngles.y, DefaultAngle, turretAngularSpeed);

                Turret.transform.localEulerAngles = new Vector3(0f, YAngle, 0f);
            }
            else
            {
                //回転の間隔
                const float IdleAngle = 10f;

                //Sin波(-IdleAngle～IdleAngleの間)
                float y = Mathf.Sin(Time.time) * IdleAngle;

                Turret.transform.localEulerAngles = new Vector3(0f, DefaultAngle + y, 0f);
            }
        }       
    }

    /// <summary>
    /// ターゲットへの方向ベクトルを取得
    /// </summary>
    /// <returns></returns>
    private Vector3 GetPlayerAngle()
    {
        Vector3 myPos = Turret.transform.position;
        Vector3 targetPos = Target.transform.position;        

        return (targetPos - myPos);
    }

    /// <summary>
    /// ターゲットへの方向ベクトルを取得(Y軸なし)
    /// </summary>
    /// <returns></returns>
    private Vector3 GetPlayerAngle_NotY()
    {
        Vector3 myPos = new Vector3(Turret.transform.position.x, 0.0f, Turret.transform.position.z);
        Vector3 targetPos = new Vector3(Target.transform.position.x, 0.0f, Target.transform.position.z);

        return (targetPos - myPos);
    }
}