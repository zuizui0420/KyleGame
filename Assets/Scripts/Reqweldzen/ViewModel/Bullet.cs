

using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Zenject;

namespace KyleGame.ViewModel
{
	public class Bullet : MonoBehaviour
	{
		[SerializeField, Header("着弾エフェクト")]
		private GameObject _effectPrefab;

		private Rigidbody _rigidbody;

		private float _bulletPower = 10f;
		
		private void Awake()
		{
			_rigidbody = GetComponent<Rigidbody>();
		}

		private void Start()
		{
			_rigidbody.AddForce(transform.forward * _bulletPower, ForceMode.Impulse);
		}
	}
}