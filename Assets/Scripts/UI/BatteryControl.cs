using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// プレイヤーのバッテリーUI
/// </summary>
public class BatteryControl : MonoBehaviour
{
    [SerializeField,Header("バッテリー使用中")]
    bool useBattery;

    [SerializeField, Header("バッテリーが使える時間"), Range(1, 30)]
    float BatteryLimitCount = 20f;

    [SerializeField,Header("オーバーヒート")]
    public bool OverHeat = false;

    [HideInInspector]
    public bool ResetFlg = false;

    //バッテリー残量
    float CurrentBattery;   

    float rate = 0.25f;

    Image BatteryImage;

    [SerializeField]
    public bool DummyOverHeat = false;

    float DummyBattery = 3f;

    void Start()
    {
        BatteryImage = GetComponent<Image>();

        BatteryLimitCount = 20f;

        //バッテリー残量を設定
        CurrentBattery = BatteryLimitCount;         
    }

    /// <summary>
    /// バッテリーを制御する
    /// </summary>
    public void BatteryUse()
    {
        BatteryImage.enabled = true;

        CurrentBattery -= Time.deltaTime;

        //消費するバッテリー値
        float lossBatteryValue = 0f;

        //1秒に消費するバッテリー値を算出
        lossBatteryValue = (1 / BatteryLimitCount) * CurrentBattery;

        if (CurrentBattery > 0)
        {
            BatteryImage.color = new Color(1f, lossBatteryValue, lossBatteryValue, 1f);
        }
        else
        {
            //オーバーヒートする
            OverHeat = true;

            DummyOverHeat = false;

            DummyBattery = 3f;

            ResetFlg = true;

            AudioManager.Instance.AudioDelete(AUDIONAME.SE_SPARK_1);
        }
    }

    /// <summary>
    /// バッテリーを回復する
    /// </summary>
    public void BatteryCharge()
    {
        BatteryImage.enabled = false;

        CurrentBattery += Time.deltaTime;

        //消費するバッテリー値
        float lossBatteryValue = 0f;

        //1秒に消費するバッテリー値を算出
        lossBatteryValue = (1 / BatteryLimitCount) * CurrentBattery;

        if (CurrentBattery < BatteryLimitCount)
        {
            BatteryImage.color = new Color(1, lossBatteryValue, lossBatteryValue, 1);
        }
    }

    /// <summary>
    /// オーバーヒート時の処理
    /// </summary>
    public void BatteryOverHeat()
    {
        BatteryImage.enabled = true;

        CurrentBattery += Time.deltaTime;

        //消費するバッテリー値
        float lossBatteryValue = 0f;

        //1秒に消費するバッテリー値を算出
        lossBatteryValue = (1 / BatteryLimitCount) * CurrentBattery;

        if (CurrentBattery < BatteryLimitCount)
        {
            // 現在のAlpha値を取得
            float alpha = BatteryImage.color.a;

            // Alphaが0 または 1になったら増減値を反転
            if (alpha < 0 || alpha > 1) { rate = rate * -1; }

            // Alpha値を増減させてセット
            BatteryImage.color = new Color(1f, lossBatteryValue, lossBatteryValue, alpha + rate);
        }
        else
        {
            // Alpha値をリセット
            BatteryImage.color = new Color(1f, lossBatteryValue, lossBatteryValue, 1f);

            OverHeat = false;
        }
    }

    /// <summary>
    /// 3秒間使用できなくする疑似的なオーバーヒート
    /// </summary>
    public void BatteryDummyOverHeat()
    {
        if (!OverHeat)
        {
            BatteryImage.enabled = true;            

            DummyBattery -= Time.deltaTime;

            CurrentBattery -= Time.deltaTime;

            //消費するバッテリー値
            float lossBatteryValue = 0f;

            //1秒に消費するバッテリー値を算出
            lossBatteryValue = (1 / BatteryLimitCount) * CurrentBattery;

            if (DummyBattery > 0f)
            {
                // 現在のAlpha値を取得
                float alpha = BatteryImage.color.a;

                // Alphaが0 または 1になったら増減値を反転
                if (alpha < 0 || alpha > 1) { rate = rate * -1; }

                // Alpha値を増減させてセット
                BatteryImage.color = new Color(1f, lossBatteryValue, lossBatteryValue, alpha + rate);
            }
            else
            {
                // Alpha値をリセット
                BatteryImage.color = new Color(1f, lossBatteryValue, lossBatteryValue, 1f);

                DummyBattery = 3f;

                DummyOverHeat = false;
            }
        }  
    }
}