using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ギミック：バー
/// </summary>
public class Gimmick_Bar : MonoBehaviour
{
    [SerializeField, Header("通行を妨げるバー(順番に設定して)")]
    GameObject[] Bars;

    [SerializeField, Header("Lerp速度"), Range(0, 1)]
    float LerpMoveSpeed;

    //バーの初期座標リスト
    List<Vector3> DefaultBarPos = new List<Vector3>();

    //バーの移動先の座標リスト
    List<Vector3> NextBarPos = new List<Vector3>();

    //バーのY軸上下移動時の増減値
    float YMoveValue = 0.5f;

    //一番下のバーの最終的なY座標
    float LastYPos = -1.9f;

    void Awake ()
    {
        //初期座標・移動先座標を設定
        for (int i = 0; i < Bars.Length; i++)
        {
            Vector3 BarPos = Bars[0].transform.localPosition;
            BarPos.y = LastYPos;

            DefaultBarPos.Add(new Vector3(BarPos.x, BarPos.y + (i * YMoveValue), BarPos.z));
            NextBarPos.Add(new Vector3(BarPos.x, BarPos.y + YMoveValue + (i * YMoveValue), BarPos.z));
        }      
    }

    public void BarShuftUp()
    {
        StartCoroutine(EntryBarAnimation(true));
    }

    public void BarShuftDown()
    {
        StartCoroutine(EntryBarAnimation(false));
    }

    /// <summary>
    /// バーのアニメーション
    /// </summary>
    /// <returns></returns>
    private IEnumerator EntryBarAnimation(bool ban)
    {
        //通行禁止にする
        if (ban)
        {
            for (int i = 0; i < Bars.Length; i++)
            {
                Bars[i].SetActive(true);

                while (true)
                {
                    Bars[i].transform.localPosition = Vector3.MoveTowards(Bars[i].transform.localPosition, NextBarPos[i], LerpMoveSpeed);

                    if (Vector3.Distance(Bars[i].transform.localPosition, NextBarPos[i]) <= 0)
                    {
                        break;
                    }

                    yield return null;
                }
            }
        }
        else
        {
            for (int i = Bars.Length - 1; i >= 0; i--)
            {
                while (true)
                {
                    Bars[i].transform.localPosition = Vector3.MoveTowards(Bars[i].transform.localPosition, DefaultBarPos[i], LerpMoveSpeed);

                    if (Vector3.Distance(Bars[i].transform.localPosition, DefaultBarPos[i]) <= 0)
                    {
                        Bars[i].SetActive(false);
                        break;
                    }

                    yield return null;
                }
            }

            Bars[0].SetActive(true);
        }
    }
}