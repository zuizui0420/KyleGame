using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GamepadInput;

public class ReticleSystem : SingletonMonoBehaviour<ReticleSystem>
{
    [SerializeField, Header("FirstPersonCamera")]
    Camera camera_FirstPerson;

    [SerializeField, Header("PlayerSystem")]
    PlayerSystem player;
   
    Vector3 HitPoint;

    float Z;

    //ヒットしたゲームオブジェクト
    GameObject hitObject;


    private void Start()
    {
        GetComponent<SpriteRenderer>().enabled = false;

        Z = transform.localPosition.z;
    }

    public void ResetPosition()
    {
        transform.localPosition = new Vector3(0f, 0f, Z);
    }

    public void ReticleEnable(bool enable)
    {
        GetComponent<SpriteRenderer>().enabled = enable;       
    }

    public void ReticleMove()
    {
        if (DATABASE.PlayIsGamePad)
        {
            float X = 0f;
            float Y = 0f;

            Vector3 Pos = transform.localPosition;

            X = -GamePad.GetAxis(GamePad.Axis.RightStick, GamePad.Index.One, true).y;
            Y = -GamePad.GetAxis(GamePad.Axis.RightStick, GamePad.Index.One, true).x;

            Pos.x += X;
            Pos.y += Y;
            Pos.z = Z;

            Pos.x = Mathf.Clamp(Pos.x, -50f, 50f);
            Pos.y = Mathf.Clamp(Pos.y, -25f, 25f);
            transform.localPosition = Pos;
        }
        else
        {
            Vector3 MousePos = Input.mousePosition;

            MousePos.z = Z;

            Vector3 screenToWorldPoint = camera_FirstPerson.ScreenToWorldPoint(MousePos);

            screenToWorldPoint.x = Mathf.Clamp(screenToWorldPoint.x, -50f, 50f);
            screenToWorldPoint.y = Mathf.Clamp(screenToWorldPoint.y, -25f, 25f);

            transform.position = screenToWorldPoint;
        }

        ReticleTargetRay();
    }

    private void ReticleTargetRay()
    {
        RaycastHit hit;

        Debug.DrawRay(camera_FirstPerson.transform.position, transform.position - camera_FirstPerson.transform.position, Color.blue);

        if (Physics.Raycast(camera_FirstPerson.transform.position, transform.position - camera_FirstPerson.transform.position, out hit, Mathf.Infinity))
        {
            HitPoint = hit.point;

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
            if (player.LaserAttack)
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