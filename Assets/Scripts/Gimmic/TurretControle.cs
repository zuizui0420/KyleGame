using System.Collections;
using UnityEngine;

/// <summary>
///     敵：タレット
/// </summary>
public class TurretControle : MonoBehaviour
{
	//次攻撃までのインターバル
	private readonly float AttackInterval = 2f;

	//攻撃態勢かどうか
	private bool AttackMode;

	//攻撃時間
	private readonly float AttackTime = 5f;

	//攻撃待機状態かどうか
	private bool AttackWaiting;

	//Idle用の初期Ｙ角度
	private float DefaultAngle;

	[SerializeField]
	[Header("弾丸を生成する座標")]
	private GameObject[] ShotPoint;

	[SerializeField]
	[Header("起動しているかどうか")]
	private bool Starting;

	//検知したターゲット
	private GameObject Target;

	[SerializeField]
	[Header("タレット")]
	private GameObject Turret;

	[SerializeField]
	[Header("タレット用の弾丸オブジェクト")]
	private GameObject Turret_Bullet;

	//タレットの回転速度
	private readonly float turretAngularSpeed = 20f;

	private void Start()
	{
		if (!Starting)
			Turret.transform.localEulerAngles = new Vector3(30f, 0f, 0f);
	}

	private void Update()
	{
		//攻撃態勢
		if (AttackMode && !AttackWaiting && Starting)
		{
			//検知したターゲットの方向を向き続ける
			RotateToTarget();
		}
		else if (!AttackMode && !AttackWaiting && Starting)
		{
			AngleRay();

			IdleTurret();
		}

		//デバッグ用：タレット起動スイッチ
		if (Input.GetKeyDown(KeyCode.P))
			Mode_StartUp();
	}

	/// <summary>
	///     ターゲット検知用Ray
	/// </summary>
	private void AngleRay()
	{
		RaycastHit hit;

		if (Physics.Raycast(Turret.transform.position, Turret.transform.forward, out hit))
			if (hit.collider.gameObject.tag == "Player")
			{
				//ターゲットを設定
				Target = hit.collider.gameObject;

				//攻撃態勢に移行
				AttackMode = true;

				//攻撃を開始
				StartCoroutine(Attack());
			}
	}

	/// <summary>
	///     タレット起動
	/// </summary>
	public void Mode_StartUp()
	{
		Debug.Log("起動");

		StartCoroutine(StartUpAnimation());
	}

	/// <summary>
	///     起動時のアニメーション
	/// </summary>
	/// <returns></returns>
	private IEnumerator StartUpAnimation()
	{
		var currentTime = 2f;

		while (currentTime > 0)
		{
			currentTime -= Time.deltaTime;

			var rot = Quaternion.AngleAxis(0f, transform.right);
			Turret.transform.rotation = Quaternion.RotateTowards(Turret.transform.rotation, rot, 1f);

			yield return null;
		}

		Turret.transform.rotation = Quaternion.identity;

		Starting = true;
	}

	/// <summary>
	///     攻撃
	/// </summary>
	/// <returns></returns>
	private IEnumerator Attack()
	{
		//攻撃時間を取得
		var AttackcurrentTime = AttackTime;

		//弾丸生成時間
		var BulletInsTime = 0.5f;

		//ガトリングを発射する
		while (AttackcurrentTime > 0)
		{
			AttackcurrentTime -= Time.deltaTime;

			BulletInsTime -= Time.deltaTime;

			if (BulletInsTime < 0f)
			{
				foreach (var Point in ShotPoint)
					Instantiate(Turret_Bullet, Point.transform.position, Point.transform.rotation);

				BulletInsTime = 0.5f;
			}

			yield return null;
		}

		DefaultAngle = Turret.transform.localEulerAngles.y;

		AttackMode = false;

		AttackWaiting = true;

		yield return new WaitForSeconds(AttackInterval);

		AttackWaiting = false;

		Debug.Log("再度検知を開始");
	}

	private void RotateToTarget()
	{
		var rot = Quaternion.LookRotation(GetPlayerAngle_NotY());

		Turret.transform.rotation = Quaternion.Slerp(Turret.transform.rotation, rot, Time.deltaTime * turretAngularSpeed);
	}

	/// <summary>
	///     待機状態のタレットの回転
	/// </summary>
	private void IdleTurret()
	{
		//回転の間隔
		const float IdleAngle = 10f;

		//Sin波(-IdleAngle～IdleAngleの間)
		var y = Mathf.Sin(Time.time) * IdleAngle;

		Turret.transform.eulerAngles = new Vector3(0f, DefaultAngle + y, 0f);
	}

	/// <summary>
	///     ターゲットへの方向ベクトルを取得
	/// </summary>
	/// <returns></returns>
	private Vector3 GetPlayerAngle()
	{
		var myPos = Turret.transform.position;
		var targetPos = Target.transform.position;

		return targetPos - myPos;
	}

	/// <summary>
	///     ターゲットへの方向ベクトルを取得(Y軸なし)
	/// </summary>
	/// <returns></returns>
	private Vector3 GetPlayerAngle_NotY()
	{
		var myPos = new Vector3(Turret.transform.position.x, 0.0f, Turret.transform.position.z);
		var targetPos = new Vector3(Target.transform.position.x, 0.0f, Target.transform.position.z);

		return targetPos - myPos;
	}
}