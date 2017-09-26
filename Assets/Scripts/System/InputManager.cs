using System;
using GamepadInput;
using UnityEngine;

/// <summary>
///     コントローラー入力検知
/// </summary>
public class InputManager : MonoBehaviour
{
	//キーボード用 Axisインプット変数
	public static float Horizontal, Vertical;

	//キーボード用 Keyインプット変数
	public static bool Key_E;

	//キーボード用 Clickインプット変数
	public static bool click_Right, click_Left;

	//キーボード用 マウスAxisインプット変数
	public static float MouseHorizontal, MouseVertical;

	private void Update()
	{
		if (!DATABASE.PlayIsGamePad)
			KeyInput();

		//GamePadDebugWindow(GamePad.Index.One);
	}

	/// <summary>
	///     キー入力
	/// </summary>
	private void KeyInput()
	{
		//移動
		Horizontal = Input.GetAxisRaw("Horizontal");
		Vertical = Input.GetAxisRaw("Vertical");

		//キー
		Key_E = Input.GetKeyDown(KeyCode.E);

		//クリック
		click_Left = Input.GetMouseButton(0);
		click_Right = Input.GetMouseButton(1);

		//マウス
		MouseHorizontal = Input.GetAxisRaw("Mouse X");
		MouseVertical = Input.GetAxisRaw("Mouse Y");
	}

	/// <summary>
	///     ゲームパッド入力のデバッグ
	/// </summary>
	private void GamePadDebugWindow(GamePad.Index controller)
	{
		//ゲームパッド(プレイヤー)を取得
		var state = GamePad.GetState(controller);

		//Button
		Debug.Log("Gamepad " + controller);
		Debug.Log("[" + controller + "：P]" + "Ａ：" + state.A);
		Debug.Log("[" + controller + "：P]" + "Ｂ：" + state.B);
		Debug.Log("[" + controller + "：P]" + "Ｘ：" + state.X);
		Debug.Log("[" + controller + "：P]" + "Ｙ：" + state.Y);
		Debug.Log("[" + controller + "：P]" + "START：" + state.Start);
		Debug.Log("[" + controller + "：P]" + "Back：" + state.Back);
		Debug.Log("[" + controller + "：P]" + "L1：" + state.LeftShoulder);
		Debug.Log("[" + controller + "：P]" + "R1：" + state.RightShoulder);
		Debug.Log("[" + controller + "：P]" + "POV_Left：" + state.Left);
		Debug.Log("[" + controller + "：P]" + "POV_Rigt：" + state.Right);
		Debug.Log("[" + controller + "：P]" + "POV_Up：" + state.Up);
		Debug.Log("[" + controller + "：P]" + "POV_Down：" + state.Down);
		Debug.Log("[" + controller + "：P]" + "L3：" + state.LeftStick);
		Debug.Log("[" + controller + "：P]" + "R3：" + state.RightStick);

		Debug.Log("-----------------------------");

		//Trigger
		Debug.Log("[" + controller + "：P]" + "L2：" + Math.Round(state.LeftTrigger, 2));
		Debug.Log("[" + controller + "：P]" + "R2：" + Math.Round(state.RightTrigger, 2));

		Debug.Log("-----------------------------");

		//Axis
		Debug.Log("[" + controller + "：P]" + "LStick：" + state.LeftStickAxis);
		Debug.Log("[" + controller + "：P]" + "RStick：" + state.rightStickAxis);
		Debug.Log("[" + controller + "：P]" + "DPad：" + state.dPadAxis);
	}

	//キー入力のデバッグログを出力するメソッド
	private void KeyDebugWindow()
	{
		#region XY Axisのデバッグ処理

		if (Horizontal > 0.0f)
			Debug.Log("Right");
		else if (Horizontal < 0.0f)
			Debug.Log("Left");
		if (Vertical > 0.0f)
			Debug.Log("Up");
		else if (Vertical < 0.0f)
			Debug.Log("Down");

		#endregion

		//クリックのデバッグ処理
		if (click_Left) Debug.Log("左クリック");

		if (click_Right) Debug.Log("右クリック");

		//マウスAxisのデバッグ処理
		Debug.Log("マウスX" + MouseHorizontal);
		Debug.Log("マウスY" + MouseVertical);
	}
}