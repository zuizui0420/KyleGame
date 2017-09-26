using UnityEngine;

public class Attack_Laser : MonoBehaviour
{
	//レーザーが発射しているとかどうか
	[HideInInspector]
	public bool fire;

	//レーザーがヒットしたかどうか
	private bool hit;

	[SerializeField]
	[Header("レーザーヒット時のエフェクト")]
	private GameObject HitFX;

	//ヒットしたゲームオブジェクト
	private GameObject hitObject;

	//レーザーがヒットしている座標
	private Vector3 hitPos;

	private LineRenderer LaserLine;

	private bool modeCheck;

	//LineRendererの頂点の数
	private Vector3[] points = new Vector3[2];

	//レーザーの頂点の数
	private int positions = 2;

	[SerializeField]
	[Header("Ray照射ターゲット")]
	private GameObject RayTarget;

	//目標座標
	private Vector3 targetPos;

	private void Start()
	{
		//目標座標の設定
		targetPos = RayTarget.transform.position;

		//LineRendererコンポーネントの参照
		LaserLine = GetComponent<LineRenderer>();

		LaserLine.positionCount = 2;
	}

	private void Update()
	{
		//頂点0の設定
		LaserLine.SetPosition(0, transform.position);

		if (fire)
		{
			//頂点1の設定
			LaserLine.SetPosition(1, RayTarget.transform.position);

			RaycastHit_System();
		}
		else
		{
			//頂点1を頂点0と同じにすることでレンダリングをさせない
			LaserLine.SetPosition(1, transform.position);

			//ヒットした座標の初期化、初期化しなければ次の発射時に一瞬だけ前に座標が入る
			hitPos = new Vector3(0, 10000, 0);
		}
	}

	private void RaycastHit_System()
	{
		RaycastHit hit;

		if (Physics.Raycast(transform.position, RayTarget.transform.position - transform.position, out hit, 100f))
			Instantiate(HitFX, hit.point, Quaternion.identity);

		//if (Physics.Raycast(transform.position, targetPos - transform.position, out hitInfo))
		//{
		//    hit = true;
		//    hitPos = hitInfo.point;            
		//    hitObject = hitInfo.collider.gameObject;
		//}
		//else
		//{
		//    hit = false;
		//    hitPos = hitInfo.point;
		//    hitObject = null;
		//}
	}
}