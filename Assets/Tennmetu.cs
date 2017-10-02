using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Tennmetu : MonoBehaviour {
    public float flashingIntarval;

    float delta = 0;


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        this.delta += Time.deltaTime;

        if (this.delta > this.flashingIntarval)
        {
            float alpha = GetComponent<CanvasRenderer>().GetAlpha();

            if (alpha == 1.0f)
            {

                GetComponent<CanvasRenderer>().SetAlpha(0.0f);

            }
            else {

                GetComponent<CanvasRenderer>().SetAlpha(1.0f);

            }

            this.delta = 0;

        }

    }
}

	

