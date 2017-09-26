using GamepadInput;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

#endif

public class CameraSystem : MonoBehaviour
{
	public int C_LerpSpeed, CameraSpeed;

	//回転ベクトル
	private Vector3 C_Rotate;

	//カメラオフセットの空オブジェクト
	private GameObject CameraOffset;

	//カメラの原点となるからオブジェクト
	private GameObject CameraOrigin;

	public GameObject Target;

	public float XMaxRadian, XMinRadian;

	private void Start()
	{
		//カメラオフセットを作成
		CameraOffset = new GameObject("CameraOffset");

		//CameraOriginを作成
		CameraOrigin = new GameObject("CameraOrigin");

		//オフセット座標をターゲットの座標にする
		CameraOffset.transform.position = new Vector3(Target.transform.position.x, Target.transform.position.y + 1.0f,
			Target.transform.position.z);

		//カメラの原点にする
		CameraOrigin.transform.position = Camera.main.transform.position;

		//自身をOffsetの子にする
		transform.parent = CameraOffset.transform;

		//CameraOriginをOffsetの子にする
		CameraOrigin.transform.parent = CameraOffset.transform;
	}

	private void Update()
	{
		//プレイヤーの操作ができる場合のみ
		if (PlayerSystem.Instance.PlayerControle)
		{
			OffsetMove();

			CameraAngle();

			RaySystem();
		}
	}

	//カメラのアングル制御
	private void CameraAngle()
	{
		var CalcRotate = Vector3.zero;

		if (DATABASE.PlayIsGamePad)
			CalcRotate = new Vector2(-GamePad.GetAxis(GamePad.Axis.RightStick, GamePad.Index.One, true).x,
				GamePad.GetAxis(GamePad.Axis.RightStick, GamePad.Index.One, true).y);
		else
			CalcRotate = new Vector2(-InputManager.MouseVertical, InputManager.MouseHorizontal);

		//回転角度を加算
		C_Rotate += CalcRotate * CameraSpeed;

		//角度の制限
		C_Rotate.x = Mathf.Clamp(C_Rotate.x, XMinRadian, XMaxRadian);

		//カメラの回転
		CameraOffset.transform.eulerAngles = C_Rotate;
	}

	//オフセットの移動 
	private void OffsetMove()
	{
		var TargetPos = new Vector3(Target.transform.position.x, Target.transform.position.y + 1.0f,
			Target.transform.position.z);
		//オフセットの移動
		CameraOffset.transform.position = Vector3.Lerp(CameraOffset.transform.position, TargetPos, C_LerpSpeed);
	}

	private void RaySystem()
	{
		RaycastHit hit;

		//オフセットからRayTargetの方向を取得
		var direction = CameraOrigin.transform.position - CameraOffset.transform.position;

		//デバッグ用(Rayの描画)
		Debug.DrawRay(CameraOffset.transform.position, CameraOrigin.transform.position - CameraOffset.transform.position,
			Color.red, 1.0f);

		//OffsetからRayTargetまでの距離を計算
		var distance = Vector3.Distance(CameraOrigin.transform.position, CameraOffset.transform.position);

		//Rayを作成
		var ray = new Ray(CameraOffset.transform.position, direction);

		if (Physics.Raycast(ray, out hit, distance, 1 << LayerMask.NameToLayer("Wall")))
		{
			//Rayが衝突した場所を取得して、少し調整する
			var avoidPos = hit.point - direction.normalized * 0.01f;

			//カメラを衝突した場所に移動させる
			transform.position = Vector3.Lerp(transform.position, avoidPos, C_LerpSpeed);
		}
		else
		{
			//カメラをカメラ原点に移動させる
			transform.position = Vector3.Lerp(transform.position, CameraOrigin.transform.position, C_LerpSpeed);
		}
	}
}

#if UNITY_EDITOR
[CustomEditor(typeof(CameraSystem))]
public class CameraSystemEditor : Editor
{
	private int XMin, XMax;

	public override void OnInspectorGUI()
	{
		//インスタンス化
		var Edit = target as CameraSystem;

		EditorGUILayout.LabelField("カメラ追従させるターゲット");
		Edit.Target = (GameObject) EditorGUILayout.ObjectField("Target", Edit.Target, typeof(Object), true);

		EditorGUILayout.Space();

		EditorGUILayout.LabelField("カメラの移動速度");
		Edit.C_LerpSpeed = EditorGUILayout.IntSlider("0～10", Edit.C_LerpSpeed, 0, 10);

		EditorGUILayout.Space();

		EditorGUILayout.LabelField("カメラの回転速度");
		Edit.CameraSpeed = EditorGUILayout.IntSlider("0～10", Edit.CameraSpeed, 0, 10);

		EditorGUILayout.Space();

		EditorGUILayout.MinMaxSlider(new GUIContent("カメラのX角度を制限する"), ref Edit.XMinRadian, ref Edit.XMaxRadian, -60.0f, 90.0f);

		Edit.XMinRadian = Mathf.Clamp(Edit.XMinRadian, -60.0f, 0.0f);
		Edit.XMaxRadian = Mathf.Clamp(Edit.XMaxRadian, 0.0f, 90.0f);

		XMin = (int) Edit.XMinRadian;
		XMax = (int) Edit.XMaxRadian;

		EditorGUILayout.LabelField("-60～0", XMin.ToString());
		EditorGUILayout.LabelField("0～90", XMax.ToString());
	}
}
#endif