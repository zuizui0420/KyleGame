using System.Collections;
using UnityEngine;

/// <summary>
///     弾丸：タレット
/// </summary>
public class TurretBullet : MonoBehaviour
{
	private readonly float BulletPower = 10f;

	[SerializeField]
	[Header("着弾時のエフェクト")]
	private GameObject Effect_Hit;

	private Rigidbody rig;

	private void Start()
	{
		rig = GetComponent<Rigidbody>();

		//弾丸を前方に飛ばす
		rig.AddForce(transform.forward * BulletPower, ForceMode.Impulse);

		StartCoroutine(BulletDestroy());
	}

	private void OnTriggerEnter()
	{
		//Instantiate(Effect_Hit, transform.position, Quaternion.identity);

		Destroy(gameObject);
	}

	private IEnumerator BulletDestroy()
	{
		yield return new WaitForSeconds(2f);

		Destroy(gameObject);
	}
}