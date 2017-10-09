using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

/// <inheritdoc />
/// <summary>
///     弾丸：タレット
/// </summary>
public class TurretBullet : MonoBehaviour
{
	[SerializeField]
	private float _bulletPower = 50;

	[SerializeField]
	private GameObject _hitEffect;

	private void Start()
	{
		var rb = GetComponent<Rigidbody>();

		Observable.NextFrame(FrameCountType.FixedUpdate)
			.TakeUntilDestroy(this)
			.Subscribe(_ => { rb.AddForce(transform.forward * _bulletPower, ForceMode.Impulse); });

		var first = this.OnTriggerEnterAsObservable().Do(OnObjectHit).AsUnitObservable();
		var second = Observable.Timer(TimeSpan.FromSeconds(2)).AsUnitObservable();

		first.Merge(second).Take(1).Subscribe(_ => { }, () => { Destroy(gameObject); }).AddTo(this);
	}

	private void OnObjectHit(Collider col)
	{
		if (col.CompareTag(TAGNAME.TAG_PLAYER))
			col.GetComponent<PlayerSystem>().Damage();

		Instantiate(_hitEffect, transform.position, Quaternion.identity);
	}
}