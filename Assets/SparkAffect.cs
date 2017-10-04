using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class SparkAffect : MonoBehaviour
{
	private PlayerSystem _playerSystem;
	private BoolReactiveProperty _laserAttackReactiveProperty = new BoolReactiveProperty();

	[SerializeField]
	private GameObject _sparkDamageArea;

	private void Awake()
	{
		_playerSystem = GetComponent<PlayerSystem>();
	}

	private void Start()
	{
		this.UpdateAsObservable().Select(_ => _playerSystem.LaserAttack)
			.Subscribe(x => _laserAttackReactiveProperty.Value = x);

		_laserAttackReactiveProperty.TakeUntilDestroy(this).Subscribe(_sparkDamageArea.SetActive);

		_sparkDamageArea.OnEnableAsObservable().Subscribe(_ =>
		{
			var colEnterObservable = _sparkDamageArea.OnCollisionEnterAsObservable().TakeUntilDisable(_sparkDamageArea);

			colEnterObservable.Where(x => x.collider.CompareTag(TAGNAME.TAG_GIMMICK_ENEMY))
				.Subscribe(x => x.collider.GetComponent<GimmickBase>().GimmickDamage());
			colEnterObservable.Where(x => x.collider.CompareTag(TAGNAME.TAG_ENEMY))
				.Subscribe(x => x.collider.GetComponent<EnemyBase>().EnemyDamage());
		});
	}
}