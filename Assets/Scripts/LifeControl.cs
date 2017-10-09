using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// プレイヤーの体力UI
/// </summary>
public class LifeControl : MonoBehaviour
{
    [SerializeField,Header("体力用画像群")]
    Sprite[] LifeImages;

    [SerializeField, Header("死亡")]
    public bool Dead = false;

    float rate = 0.5f;

    Image LifeImage;

	void Start ()
    {
        LifeImage = GetComponent<Image>();

        //初期画像を設定
        LifeImage.sprite = LifeImages[DATABASE.Life];
    }

    /// <summary>
    /// アイコンの点滅
    /// </summary>
    public void StartBlink(int PlayerLife)
    {
        StartCoroutine(SpriteBlink(PlayerLife));
    }

    /// <summary>
    /// アイコンの点滅処理
    /// </summary>
    /// <param name="duration"></param>
    /// <returns></returns>
    private IEnumerator SpriteBlink(int array)
    {
        if (!Dead)
        {
            float duration = 1f;

            float startTime = Time.timeSinceLevelLoad;

            //Imageの差し替え
            LifeImage.sprite = LifeImages[array];

            while (true)
            {
                float time = Time.timeSinceLevelLoad - startTime;
                if (time >= duration) break;

                // 現在のAlpha値を取得
                float alpha = LifeImage.color.a;

                // Alphaが0 または 1になったら増減値を反転
                if (alpha < 0 || alpha > 1)
                {
                    rate = rate * -1;
                }

                // Alpha値を増減させてセット
                LifeImage.color = new Color(1, 1, 1, alpha + rate);

                yield return null;
            }

            LifeImage.color = Color.white;
        }        
    }
}