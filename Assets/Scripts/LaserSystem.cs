using System.Collections.Generic;
using System.Linq;
using KyleGame;
using UniRx;
using UnityEngine;

public class LaserSystem : MonoBehaviour
{
	private readonly BoolReactiveProperty _isFire = new BoolReactiveProperty();

	//LineRendererの管理リスト
	private readonly List<LineRenderer> _lineRenderers = new List<LineRenderer>();

	[SerializeField]
	private GameObject _rayDestination;

	[SerializeField]
	private GameObject _hitEffect;

	[SerializeField]
	private GameObject[] _laserObjects;

	//目標座標
	private Vector3 targetPos;

	public IReadOnlyReactiveProperty<bool> IsFire
	{
		get { return _isFire; }
	}

	private void Start()
	{
		//目標座標の設定
		targetPos = _rayDestination.transform.position;

		_lineRenderers.AddRange(_laserObjects.Select(x => x.GetComponent<LineRenderer>()));

		//LineRendererの各種設定
		foreach (var lineRenderer in _lineRenderers)
		{
			lineRenderer.positionCount = 2;
			lineRenderer.SetPosition(0, lineRenderer.transform.position);
		}
	}

	private void Update()
	{
		if (IsFire.Value)
		{
			//頂点１の設定
			foreach (var lineRenderer in _lineRenderers)
			{
				lineRenderer.enabled = true;
				lineRenderer.SetPosition(0, lineRenderer.transform.position);
				lineRenderer.SetPosition(1, RaycastHit_System());
			}

			//エフェクト生成
			Instantiate(_hitEffect, RaycastHit_System(), Quaternion.identity);
		}
		else
		{
			foreach (var lineRenderer in _lineRenderers)
				lineRenderer.enabled = false;
		}
	}

	/// <summary>
	///     RayCastの制御・判定
	/// </summary>
	/// <returns></returns>
	private Vector3 RaycastHit_System()
	{
		RaycastHit hit;

		Vector3 hitPoint;

		Debug.DrawRay(transform.position, _rayDestination.transform.position - transform.position, Color.yellow);

		if (Physics.Raycast(transform.position, _rayDestination.transform.position - transform.position, out hit,
			Mathf.Infinity))
		{
			hitPoint = hit.point;

			//ギミック・敵の場合は、ダメージを与える
			if (hit.collider.CompareTag(TAGNAME.TAG_GIMMICK_ENEMY))
				hit.collider.gameObject.GetComponent<GimmickBase>().GimmickDamage();
			else if (hit.collider.CompareTag(TAGNAME.TAG_ENEMY))
				hit.collider.gameObject.GetComponent<EnemyBase>().EnemyDamage();

			var laserDrivenObj = hit.collider.GetComponent<ILaserDrivenObject>();
			if (laserDrivenObj != null)
				laserDrivenObj.OnHitLaser();
		}
		else
		{
			hitPoint = _rayDestination.transform.position;
		}

		return hitPoint;
	}
}