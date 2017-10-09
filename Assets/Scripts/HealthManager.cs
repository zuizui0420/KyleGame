using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <inheritdoc />
/// <summary>
/// プレイヤーの体力UI
/// </summary>
public class HealthManager : MonoBehaviour
{
    [SerializeField,Header("体力用画像群")]
    Sprite[] LifeImages;

	[SerializeField, Header("死亡")]
	public bool Dead;

    float rate = 0.5f;

    private Image _lifeImage;

	private Sprite LifeImageSprite
	{
		get { return _lifeImage.sprite; }
		set { _lifeImage.sprite = value; }
	}

	private void Start ()
    {
        _lifeImage = GetComponent<Image>();

        //初期画像を設定
        _lifeImage.sprite = LifeImages[DATABASE.Life];
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
	/// <param name="index"></param>
	/// <returns></returns>
	private IEnumerator SpriteBlink(int index)
    {
        if (!Dead)
        {
            float duration = 1f;

            float startTime = Time.timeSinceLevelLoad;

			//Imageの差し替え
	        LifeImageSprite = LifeImages[index];

            while (true)
            {
                float time = Time.timeSinceLevelLoad - startTime;
                if (time >= duration) break;

                // 現在のAlpha値を取得
                float alpha = _lifeImage.color.a;

                // Alphaが0 または 1になったら増減値を反転
                if (alpha < 0 || alpha > 1)
                {
                    rate = rate * -1;
                }

                // Alpha値を増減させてセット
                _lifeImage.color = new Color(1, 1, 1, alpha + rate);

                yield return null;
            }

            _lifeImage.color = Color.white;
        }        
    }
}