using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GamepadInput;
using KyleGame;

public class ReticleSystem : SingletonMonoBehaviour<ReticleSystem>
{
    [SerializeField, Header("FirstPersonCamera")]
    private Camera _firstPersonCamera;
   
    Vector3 HitPoint;

	private PlayerSystem _playerSystem;

    float Z;

    //ヒットしたゲームオブジェクト
    GameObject hitObject;

	private SpriteRenderer _spriteRenderer;

    private void Start()
    {
	    _spriteRenderer = GetComponent<SpriteRenderer>();


	    _spriteRenderer.enabled = false;

        Z = transform.localPosition.z;
    }

    public void ResetPosition()
    {
        transform.localPosition = new Vector3(0f, 0f, Z);
    }

    public void ReticleEnable(bool enable)
    {
	    _spriteRenderer.enabled = enable;       
    }

	private void GamePadMove()
	{
		var localPosition = transform.localPosition;

		var x = -GamePad.GetAxis(GamePad.Axis.RightStick, GamePad.Index.One, true).y * 1.5f;
		var y = -GamePad.GetAxis(GamePad.Axis.RightStick, GamePad.Index.One, true).x * 1.5f;

		localPosition.x += x;
		localPosition.y += y;
		localPosition.z = Z;

		localPosition.x = Mathf.Clamp(localPosition.x, -50f, 50f);
		localPosition.y = Mathf.Clamp(localPosition.y, -25f, 25f);
		transform.localPosition = localPosition;
	}

	private void KeyboardMove()
	{
		var mousePosition = Input.mousePosition;

		mousePosition.z = Z;

		var screenToWorldPoint = _firstPersonCamera.ScreenToWorldPoint(mousePosition);

		screenToWorldPoint.x = Mathf.Clamp(screenToWorldPoint.x, -50f, 50f);
		screenToWorldPoint.y = Mathf.Clamp(screenToWorldPoint.y, -25f, 25f);

		transform.position = screenToWorldPoint;
	}

    public void ReticleMove()
    {
	    if (DATABASE.PlayIsGamePad)
		    GamePadMove();
	    else
		    KeyboardMove();

	    ReticleTargetRay();
    }

    private void ReticleTargetRay()
    {
        Debug.DrawRay(_firstPersonCamera.transform.position, transform.position - _firstPersonCamera.transform.position, Color.blue);

	    RaycastHit hit;
		if (Physics.Raycast(_firstPersonCamera.transform.position, transform.position - _firstPersonCamera.transform.position, out hit, Mathf.Infinity))
        {
            HitPoint = hit.point;

	        var laserDrivenObj = hit.collider.GetComponent<ILaserDrivenObject>();
	        var sparkDrivenObj = hit.collider.GetComponent<ISparkDrivenObject>();

	        if (laserDrivenObj != null)
		        laserDrivenObj.OnHitLaser();

			if (sparkDrivenObj != null)
				sparkDrivenObj.OnHitSpark();


			HitTagCheck(hit.collider.gameObject, hit.collider.tag);
        }
        else
        {
            HitTagCheck(hitObject);
        }      
    }

    /// <summary>
    /// Rayで検知したタグのチェック
    /// </summary>
    /// <param name="tagName"></param>
    private void HitTagCheck(GameObject obj = null, string tagName = "")
    {
        if (tagName == TAGNAME.TAG_GIMMICK_LASER)
        {
            if (_playerSystem.LaserAttack)
            {
                Debug.Log("タグヒット：GIMMICK_LASER");
                obj.GetComponent<GimmickBase>().GimmickAction(true);
                hitObject = obj;
            }            
        }

        else if (tagName == TAGNAME.TAG_GIMMICK_SPARK)
        {
            Debug.Log("タグヒット：GIMMICK_SPARK");
            obj.GetComponent<GimmickBase>().GimmickAction(true);
            hitObject = obj;
        }

        else
        {
            Debug.Log("タグヒット：NONE");

            if(hitObject != null)
            {
                hitObject.GetComponent<GimmickBase>().GimmickAction(false);
            }            
        }
    }
}