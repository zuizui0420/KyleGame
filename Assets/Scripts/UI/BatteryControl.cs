using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BatteryControl : MonoBehaviour {

    private float battery = 1;
    private float G, B;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void BatteryUse()
    {
        float startTime = Time.timeSinceLevelLoad;
        while (true)
        {
            float time = (Time.timeSinceLevelLoad - startTime);
            if (time >= 100.0f) break;            
            battery = time / 100;
            GetComponent<Image>().color = new Color(1, battery, battery, 1);
        }
    }
}
