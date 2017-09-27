using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 弾丸：タレット
/// </summary>
public class TurretBullet : MonoBehaviour
{
    Rigidbody rig;

    [SerializeField, Header("着弾時のエフェクト")]
    GameObject Effect_Hit;

    float BulletPower = 10f;

	void Start ()
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