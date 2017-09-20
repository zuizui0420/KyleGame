using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCI_FI_WallLamp : MonoBehaviour
{
    [SerializeField, Header("Barrel")]
    GameObject Barrel;

    [SerializeField, Header("色が切り替わる速度"), Range(0.1f, 1f)]
    float ColorLerpSpeed;

    [SerializeField, Header("回転速度"), Range(0f, 1f)]
    float RotateSpeed;

    [SerializeField, Header("")]
    Color DefaultEmissionColor;

    [SerializeField]
    Color MoveEmissionColor;

    MeshRenderer m_renderer;

    float currentColorTime = 0f;

    Color color;

    void Start()
    {
        m_renderer = Barrel.GetComponent<MeshRenderer>();

        //EmissionColorの使用を可能にする
        m_renderer.material.EnableKeyword("_EMISSION");

        //初期EmissionColorを設定
        m_renderer.material.SetColor("_EmissionColor", DefaultEmissionColor);
    }

    void Update()
    {
        //カラー遷移
        ColorMove();

        //Y軸回転
        Vector3 Rot = Barrel.transform.localEulerAngles;
        Rot.y += RotateSpeed;
        Barrel.transform.localEulerAngles = Rot;
    }

    private void ColorMove()
    {
        currentColorTime += Time.deltaTime;

        if (currentColorTime > 4f)
        {
            currentColorTime = 0f;
        }
        else if (currentColorTime > 2f)
        {
            color = Color.Lerp(DefaultEmissionColor, MoveEmissionColor, ColorLerpSpeed);
        }
        else
        {
            color = Color.Lerp(MoveEmissionColor, DefaultEmissionColor, ColorLerpSpeed);
        }

        m_renderer.material.SetColor("_EmissionColor", color);
    }
}