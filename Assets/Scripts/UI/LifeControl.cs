using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LifeControl : MonoBehaviour
{
	private Sprite lifeImage;

	[SerializeField]
	private Sprite[] lifeImages;

	private float rate = 0.5f;


	// Use this for initialization
	private void Start()
	{
		//ダメージに応じて変える
		lifeImage = lifeImages[3];

		Invoke("StartBlink", 1f);
	}

	private void StartBlink()
	{
		StartCoroutine(SpriteBlink(1f));
	}

	// Update is called once per frame
	private void Update()
	{
	}


	private IEnumerator SpriteBlink(float duration)
	{
		var startTime = Time.timeSinceLevelLoad;
		var image = GetComponent<Image>();

		//imegeの差し替え
		GetComponent<Image>().sprite = lifeImage;
		image.sprite = lifeImage;

		while (true)
		{
			var time = Time.timeSinceLevelLoad - startTime;
			if (time >= duration) break;

			// 現在のAlpha値を取得
			var alpha = image.color.a;
			// Alphaが0 または 1になったら増減値を反転
			if (alpha < 0 || alpha > 1)
				rate = rate * -1;
			// Alpha値を増減させてセット
			image.color = new Color(1, 1, 1, alpha + rate);

			yield return null;
		}

		image.color = Color.white;
	}
}