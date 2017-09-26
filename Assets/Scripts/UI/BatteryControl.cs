using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BatteryControl : MonoBehaviour {

    private float battery = 1;
    public bool useBattery;
    private float rate = 0.25f;
    private float duration = 0;

    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        StartCoroutine("BatteryUse",duration);
        
	}


    private IEnumerator BatteryUse(float timeUse)
    {
        float startTime = Time.timeSinceLevelLoad;
        var image = GetComponent<Image>();

        while (true)
        {
            float time = (Time.timeSinceLevelLoad - startTime)+timeUse;
            if (useBattery == false || battery <= 0)
            {
                duration = time;
                break;
            }
           

            battery = (1 - 0.05f * time);

            if (time <= 15)
            {                
                image.color = new Color(1, battery, battery, 1);
            }

            else
            {
                // 現在のAlpha値を取得
                float alpha = image.color.a;
                // Alphaが0 または 1になったら増減値を反転
                if (alpha < 0 || alpha > 1)
                {
                    rate = rate * -1;
                }
                // Alpha値を増減させてセット
                image.color = new Color(1, battery, battery, alpha + rate);
            }

            yield return null;
        }
        if (battery <= 0)
        {
            image.color = new Color(1, battery, battery, 1);
         
        }
        else
        {
            image.color = new Color(1, battery, battery, 0);
        }
      
    }

    void BatteryOverheat()
    {

    }
}
