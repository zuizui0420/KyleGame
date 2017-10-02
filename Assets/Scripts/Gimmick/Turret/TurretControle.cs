using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敵：タレット
/// </summary>
public class TurretControle : GimmickBase
{
    [SerializeField, Header("タレット")]
    GameObject Turret;

    [SerializeField, Header("起動しているかどうか")]
    bool Starting;

    [SerializeField, Header("全方向を検知が可能か")]
    bool AroundCheck;

    [SerializeField, Header("体力(０の場合は無敵)")]
    float Life;

    [SerializeField, Header("攻撃時間")]
    float AttackTime;

    [SerializeField, Header("次の攻撃までの待機時間")]
    float AttackInterval;

    [SerializeField, Header("タレットの回転速度")]
    float turretAngularSpeed;

    [SerializeField, Header("弾丸を生成する座標")]
    GameObject[] ShotPoint;

    [SerializeField, Header("タレット用の弾丸オブジェクト")]
    GameObject Turret_Bullet;

    [SerializeField, Header("エフェクト生成座標")]
    GameObject EffectInsPoint;

    [SerializeField, Header("マズルフラッシュのエフェクト")]
    GameObject Effect_hit;

    //攻撃態勢かどうか
    bool AttackMode = false;

    //攻撃待機状態かどうか
    bool AttackWaiting = false;

    //検知したターゲット
    GameObject Target;   

    //Idle用の初期Ｙ角度
    float DefaultAngle;

    //起動・停止状態の角度
    float SetUpRot = 0f;
    float NotSetUpRot = 30f;

    //初期角度に戻っているかどうか
    bool DefaultAngleBack = false;

    void Start ()
    {
        //初期Y角度を取得
        DefaultAngle = Turret.transform.localEulerAngles.y;

        if (!Starting)
        {
            //起動していない状態にする
            Turret.transform.localEulerAngles = new Vector3(NotSetUpRot, 0f, 0f);

            //起動していないので無敵にしておく
            LIFE = 0f;
        }
        else
        {
            //体力を設定
            LIFE = Life;
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
	}

    /// <summary>
    /// ターゲット検知用Ray
    /// </summary>
    private void AngleRay()
    {
        RaycastHit hit;

        if(Physics.Raycast(Turret.transform.position, Turret.transform.forward, out hit,Mathf.Infinity))
        {
            if(hit.collider.gameObject.tag == TAGNAME.TAG_PLAYER)
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

    /// <summary>
    /// ギミック：作動
    /// </summary>
    protected override void GimmickAction_Enemy()
    {
        //体力を設定
        LIFE = Life;

        StartCoroutine(TurretAnimation(SetUpRot, true));
    }

    /// <summary>
    /// ギミック：破壊
    /// </summary>
    protected override void GimmickBreak()
    {
        //攻撃状態を解除
        AttackMode = false;

        StartCoroutine(TurretAnimation(NotSetUpRot, false));
    }

    /// <summary>
    /// 起動・停止のアニメーション
    /// </summary>
    /// <returns></returns>
    private IEnumerator TurretAnimation(float AnimationAngle, bool starting)
    {
        float currentTime = 2f;

        if (!starting) { Starting = starting; }

        while (currentTime > 0)
        {
            currentTime -= Time.deltaTime;

            Turret.transform.localEulerAngles = new Vector3(
                Mathf.Lerp(Turret.transform.localEulerAngles.x, AnimationAngle, 0.1f), 0f, 0f);

            yield return null;
        }

        if (starting) { Starting = starting; }       
    }

    #region 攻撃
    private IEnumerator Attack()
    {
        //攻撃時間を取得
        float AttackcurrentTime = AttackTime;

        //弾丸生成時間
        float BulletInsTime = 0.5f;

        //ガトリングを発射する
        while(AttackcurrentTime > 0)
        {
            if (!AttackMode)
            {
                break; ;
            }

            AttackcurrentTime -= Time.deltaTime;

            BulletInsTime -= Time.deltaTime;

            if(BulletInsTime < 0f)
            {
                foreach (GameObject Point in ShotPoint)
                {                   
                    GameObject bullet = Instantiate(Turret_Bullet, Point.transform.position, Point.transform.rotation);
                    bullet.SetActive(true);

                    //エフェクトを生成
                    Instantiate(Effect_hit, EffectInsPoint.transform.position, Quaternion.identity);
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

        DefaultAngleBack = false;

        yield return new WaitForSeconds(AttackInterval);

        AttackWaiting = false;              
    }
    #endregion   

    #region 待機状態
    private void IdleTurret()
    {
        if (AroundCheck)
        {
            //回転の間隔
            const float IdleAngle = 20f;

            //Sin波(-IdleAngle～IdleAngleの間)
            float y = Mathf.Sin(Time.time) * IdleAngle;

            Turret.transform.localEulerAngles = new Vector3(0f, DefaultAngle + y, 0f);
        }
        else
        {
            if(!DefaultAngleBack)
            {
                float YAngle = Mathf.MoveTowards(Turret.transform.localEulerAngles.y, 0f, turretAngularSpeed);

                Turret.transform.localEulerAngles = new Vector3(0f, YAngle, 0f);

                if(Turret.transform.localEulerAngles.y == 0)
                {
                    DefaultAngleBack = true;
                }
            }
            else
            {
                //回転の間隔
                const float IdleAngle = 20f;

                //Sin波(-IdleAngle～IdleAngleの間)
                float y = Mathf.Sin(Time.time) * IdleAngle;

                Turret.transform.localEulerAngles = new Vector3(0f, DefaultAngle + y, 0f);
            }
        }       
    }
    #endregion

    /// <summary>
    /// ターゲットの方向に向く処理
    /// </summary>
    private void RotateToTarget()
    {
        Quaternion rot = Quaternion.LookRotation(GetPlayerAngle_NotY());

        Turret.transform.rotation = Quaternion.Slerp(Turret.transform.rotation, rot, Time.deltaTime * turretAngularSpeed);
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