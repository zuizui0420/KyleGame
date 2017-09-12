using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GamepadInput;

public class ReticleSystem : SingletonMonoBehaviour<ReticleSystem>
{
    [SerializeField, Header("")]
    Camera camera_FirstPerson;

    float RayDistance = 100f;

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
        float X = 0f;
        float Y = 0f;

        Vector2 Pos = transform.localPosition;

        if (DATABASE.PlayIsGamePad)
        {
            X = GamePad.GetAxis(GamePad.Axis.LeftStick, GamePad.Index.One, true).x;
            Y = GamePad.GetAxis(GamePad.Axis.LeftStick, GamePad.Index.One, true).y;

            Pos.x += X * 0.01f;
            Pos.y += Y * 0.01f;

            Pos.x = Mathf.Clamp(Pos.x, -0.4f, 0.4f);
            Pos.y = Mathf.Clamp(Pos.y, -0.2f, 0.2f);
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

        if(Physics.Raycast(transform.position, transform.forward, out hit, RayDistance))
        {           
            Debug.Log(hit.point);
        }
        else
        {
            Debug.Log(RayDistance);
        }
    }
}