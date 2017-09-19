using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GamepadInput;

public class ReticleSystem : SingletonMonoBehaviour<ReticleSystem>
{
    [SerializeField, Header("")]
    Camera camera_FirstPerson;

    float RayDistance = 100f;

    private void Start()
    {
        GetComponent<SpriteRenderer>().enabled = false;
    }

    public void ResetPosition()
    {
        transform.localPosition = new Vector3(0f, 0f, 0.5f);
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

            X = GamePad.GetAxis(GamePad.Axis.LeftStick, GamePad.Index.One, true).x;
            Y = GamePad.GetAxis(GamePad.Axis.LeftStick, GamePad.Index.One, true).y;

            Pos.x += X;
            Pos.y += Y;
            Pos.z = 50.0f;

            Pos.x = Mathf.Clamp(Pos.x, -50f, 50f);
            Pos.y = Mathf.Clamp(Pos.y, -25f, 25f);
            transform.localPosition = Pos;
        }
        else
        {
            Vector3 MousePos = Input.mousePosition;

            MousePos.z = 50.0f;

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

        Debug.DrawRay(transform.position, transform.forward, Color.blue);

        //if(Physics.Raycast(transform.position, transform.forward, out hit, RayDistance))
        //{           
        //    Debug.Log(hit.point);
        //}
        //else
        //{
        //    Debug.Log(RayDistance);
        //}
    }
}