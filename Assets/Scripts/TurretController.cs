using System;
using KyleGame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

/// <summary>
/// 敵：タレット
/// </summary>
public class TurretController : GimmickBase, ILaserDrivenObject
{
    [SerializeField, Header("タレット")]
    GameObject Turret;

    [SerializeField, Header("起動しているかどうか")]
    private bool _isActive;

    [SerializeField, Header("全方向を検知が可能か")]
    bool AroundCheck;

    [SerializeField, Header("体力(０の場合は無敵)")]
    float Life;

    [SerializeField, Header("攻撃時間")]
    float AttackTime;

    [SerializeField, Header("次の攻撃までの待機時間")]
    float AttackInterval;

	[SerializeField]
	private FloatReactiveProperty _headRotationSpeed = new FloatReactiveProperty();

    [SerializeField, Header("弾丸を生成する座標")]
    GameObject[] ShotPoint;

    [SerializeField, Header("タレット用の弾丸オブジェクト")]
    GameObject Turret_Bullet;

    [SerializeField, Header("エフェクト生成座標")]
    GameObject EffectInsPoint;

    [SerializeField, Header("マズルフラッシュのエフェクト")]
    GameObject Effect_hit;

	[SerializeField, Header("故障のエフェクト")]
	GameObject Effect_destroy;

	[SerializeField, Header("電気のエフェクト")]
	GameObject Effect_elect;

	//攻撃態勢かどうか
	private bool _isAttack;

    //攻撃待機状態かどうか
    private bool _isAttackReady;

    //検知したターゲット
    GameObject Target;   

    //Idle用の初期Ｙ角度
    private float _startAngle;

    //起動・停止状態の角度
	private const float TiltWhenActive = 0f;

	private const float TiltWhenNonActive = 30f;

	//初期角度に戻っているかどうか
    bool DefaultAngleBack = false;

	[SerializeField]
	private bool _isImmortal;

    private void Start ()
    {
        //初期Y角度を取得
        _startAngle = Turret.transform.localEulerAngles.y;

        if (!_isActive)
        {
            //起動していない状態にする
            Turret.transform.localEulerAngles = new Vector3(TiltWhenNonActive, 0f, 0f);

            //起動していないので無敵にしておく
            LIFE = 0f;
        }
        else
        {
            //体力を設定
            LIFE = Life;
        }     
	}
	
	private void Update ()
    {
        //攻撃態勢
        if (_isAttack && !_isAttackReady && _isActive)
        {
            //検知したターゲットの方向を向き続ける
            RotateToTarget();
        }
        else if(!_isAttack && !_isAttackReady && _isActive)
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
            if(hit.collider.gameObject.CompareTag(TAGNAME.TAG_PLAYER))
            {
                //ターゲットを設定
                Target = hit.collider.gameObject;

                //攻撃態勢に移行
                _isAttack = true;

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

        StartCoroutine(TurretAnimation(TiltWhenActive, true));
    }

    /// <summary>
    /// ギミック：破壊
    /// </summary>
    protected override void GimmickBreak()
    {
        //攻撃状態を解除
        _isAttack = false;

		var trigger = Instantiate(Effect_destroy, EffectInsPoint.transform.position, Quaternion.identity).GetComponent<ParticleSystemTrigger>();
		trigger.IsFinished.Subscribe(_ => Destroy(trigger.gameObject));

		AudioManager.Instance.Play(AUDIONAME.SE_EXPLOSION_1, 0.5f, false, 110, false);

		Effect_elect.SetActive(true);

		StartCoroutine(TurretAnimation(TiltWhenNonActive, false));
    }

    /// <summary>
    /// 起動・停止のアニメーション
    /// </summary>
    /// <returns></returns>
    private IEnumerator TurretAnimation(float AnimationAngle, bool starting)
    {
		AudioManager.Instance.AudioDelete(AUDIONAME.SE_EXPLOSION_1);

	    var currentTime = 0f;

        if (!starting) { _isActive = starting; }

        while (currentTime < 1)
        {
            currentTime += Time.deltaTime;

	        var x = Mathf.Lerp(Turret.transform.localEulerAngles.x, AnimationAngle, 0.1f);

			Turret.transform.localEulerAngles = new Vector3(x, 0f, 0f);
			
			yield return null;
        }

		Effect_elect.SetActive(false);

        if (starting)
		{			
			_isActive = starting;
		}       
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
            if (!_isAttack)
            {
                break;
            }

            AttackcurrentTime -= Time.deltaTime;

            BulletInsTime -= Time.deltaTime;

            if(BulletInsTime < 0f)
            {
                foreach (var point in ShotPoint)
                {                   
                    GameObject bullet = Instantiate(Turret_Bullet, point.transform.position, point.transform.rotation);
                    bullet.SetActive(true);

                    //エフェクトを生成
                    Instantiate(Effect_hit, EffectInsPoint.transform.position, Quaternion.identity);

					//サウンド再生
					AudioManager.Instance.Play(AUDIONAME.SE_GUNSHOT, 0.1f, false, 110, false);
                }

                BulletInsTime = 0.5f;
            }

            yield return null;               
        }

        if (AroundCheck)
        {
            _startAngle = Turret.transform.localEulerAngles.y;
        }        

        _isAttack = false;

        _isAttackReady = true;

        DefaultAngleBack = false;

		yield return new WaitForSeconds(AttackInterval);

		_isAttackReady = false;

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

            Turret.transform.localEulerAngles = new Vector3(0f, _startAngle + y, 0f);
        }
        else
        {
            if(!DefaultAngleBack)
            {
                float YAngle = Mathf.MoveTowards(Turret.transform.localEulerAngles.y, 0f, _headRotationSpeed.Value);

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

                Turret.transform.localEulerAngles = new Vector3(0f, _startAngle + y, 0f);
            }
        }       
    }
    #endregion

    /// <summary>
    /// ターゲットの方向に向く処理
    /// </summary>
    private void RotateToTarget()
    {
        var rot = Quaternion.LookRotation(GetPlayerAngle_NotY());

        Turret.transform.rotation = Quaternion.Slerp(Turret.transform.rotation, rot, Time.deltaTime * _headRotationSpeed.Value);
    }

    /// <summary>
    /// ターゲットへの方向ベクトルを取得
    /// </summary>
    /// <returns></returns>
    private Vector3 GetPlayerAngle()
    {
        var myPos = Turret.transform.position;
        var targetPos = Target.transform.position;        

        return targetPos - myPos;
    }

    /// <summary>
    /// ターゲットへの方向ベクトルを取得(Y軸なし)
    /// </summary>
    /// <returns></returns>
    private Vector3 GetPlayerAngle_NotY()
    {
	    var direction = GetPlayerAngle();
	    direction.y = 0f;

        return direction;
    }

	public bool IsActive { get; set; }
	public void OnHitLaser()
	{
		throw new System.NotImplementedException();
	}
}