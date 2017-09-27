using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// SCI_FI_Switch
/// </summary>
public class SCI_FI_Switch : MonoBehaviour
{
    [SerializeField, Header("Barrel")]
    GameObject Barrel;

    [SerializeField, Header("スイッチが押されているかどうか")]
    public bool SwitchOn = false;

    [SerializeField, Header("色が切り替わる速度")]
    float ColorLerpSpeed = 0.1f;

    [SerializeField, Header("スイッチの回転速度")]
    float RotateSpeed = 0.1f;

    float MoveLerpSpeed = 0.1f;

    //移動先座標
    Vector3 DefaultPoint, DownPoint;  

    MeshRenderer m_renderer;

    Color DefaultEmissionColor, MoveEmissionColor;

    float currentColorTime = 0f;

    float Color_b = 0f;

    void Start ()
    {
        DefaultPoint = Barrel.transform.localPosition;
        DownPoint = new Vector3(DefaultPoint.x, -0.74f, DefaultPoint.z);

        m_renderer = Barrel.GetComponent<MeshRenderer>();

        //EmissionColorの使用を可能にする
        m_renderer.material.EnableKeyword("_EMISSION");

        //初期EmissionColorを取得
        DefaultEmissionColor = m_renderer.material.GetColor("_EmissionColor");

        //遷移するEmissionColorを設定
        MoveEmissionColor = new Color(0f, 0.4f, 1f);
	}

	void Update ()
    {
        //カラー遷移
        ColorMove();

        if (SwitchOn)
        {
            //Y軸移動
            Barrel.transform.localPosition = Vector3.Lerp(Barrel.transform.localPosition, DownPoint, MoveLerpSpeed);
        }
        else
        {
            //Y軸移動
            Barrel.transform.localPosition = Vector3.Lerp(Barrel.transform.localPosition, DefaultPoint, MoveLerpSpeed);

            //Y軸回転
            Vector3 Rot = Barrel.transform.localEulerAngles;
            Rot.y += RotateSpeed;
            Barrel.transform.localEulerAngles = Rot;
        }
	}

    private void ColorMove()
    {
        const float MinColorValue = 0f, MaxColorValue = 0.4f;

        currentColorTime += Time.deltaTime;

        if(currentColorTime > 4f)
        {
            currentColorTime = 0f;
        }
        else if (currentColorTime > 2f)
        {
            Color_b = Mathf.Lerp(Color_b, MinColorValue, ColorLerpSpeed);
        }
        else
        {
            Color_b = Mathf.Lerp(Color_b, MaxColorValue, ColorLerpSpeed);
        }

        m_renderer.material.SetColor("_EmissionColor", new Color(0f, Color_b, 1f));
    }

    void OnTriggerStay(Collider col)
    {
        if (col.gameObject.tag == TAGNAME.TAG_PLAYER)
        {
            SwitchOn = true;
        }       
    }

    private void OnTriggerExit(Collider col)
    {
        if (col.gameObject.tag == TAGNAME.TAG_PLAYER)
        {
            SwitchOn = false;
        }        
    }
}