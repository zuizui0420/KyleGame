using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCI_FI_WallLamp : MonoBehaviour
{
    [SerializeField, Header("BarrelOffset")]
    GameObject BarrelOffset;

    [SerializeField, Header("TriBarrelsOffset")]
    GameObject TriBarrelsOffset;

    [SerializeField,Header("MeshRenderer_Object")]
    MeshRenderer[] m_renderer;

    [SerializeField, Header("色変化速度"), Range(1f, 10f)]
    float ColorLerpSpeed;

    [SerializeField, Header("回転速度"), Range(0f, 1f)]
    float RotateSpeed;

    [SerializeField, Header("ランプ起動")]
    bool LampOn;

    [SerializeField, Header("初期色")]
    Color DefaultEmissionColor;

    [SerializeField, Header("次色")]
    Color NextEmissionColor;   

    float colorValue = 0f;

    void Start()
    {
        foreach(MeshRenderer renderer in m_renderer)
        {
            //EmissionColorの使用を可能にする
            renderer.material.EnableKeyword("_EMISSION");

            //初期EmissionColorを設定
            renderer.material.SetColor("_EmissionColor", DefaultEmissionColor);
        }       
    }

    void Update()
    {
        //カラー遷移
        LampColorMove(LampOn);

        //Z軸回転
        Vector3 BarrelRotZ = BarrelOffset.transform.localEulerAngles;
        Vector3 TriBarrelRotZ = TriBarrelsOffset.transform.localEulerAngles;

        BarrelRotZ.z += RotateSpeed;
        TriBarrelRotZ.z += RotateSpeed;

        BarrelOffset.transform.localEulerAngles = BarrelRotZ;
        TriBarrelsOffset.transform.localEulerAngles = TriBarrelRotZ;
    }

    private void LampColorMove(bool flg)
    {
        if (flg) colorValue = Mathf.MoveTowards(colorValue, 1f, (ColorLerpSpeed / 10) * Time.deltaTime);

        else colorValue = Mathf.MoveTowards(colorValue, 0f, (ColorLerpSpeed / 10) * Time.deltaTime);

        if (colorValue > 1) colorValue = 1f;
        if (colorValue < 0) colorValue = 0f;

        foreach(MeshRenderer renderer in m_renderer)
        {
            renderer.material.SetColor("_EmissionColor", Color.Lerp(DefaultEmissionColor, NextEmissionColor, colorValue));
        }   
    }
}