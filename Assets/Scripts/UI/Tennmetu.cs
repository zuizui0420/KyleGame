using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tennmetu : MonoBehaviour
{
    public float flashingIntarval;

    float delta = 0;
	
	void Update ()
    {
        delta += Time.deltaTime;

        if (delta > flashingIntarval)
        {
            float alpha = GetComponent<CanvasRenderer>().GetAlpha();

            if (alpha == 1.0f)
            {

                GetComponent<CanvasRenderer>().SetAlpha(0.0f);

            }
            else
            {

                GetComponent<CanvasRenderer>().SetAlpha(1.0f);

            }

            delta = 0;
        }
    }
}