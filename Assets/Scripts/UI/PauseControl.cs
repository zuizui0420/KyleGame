using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PauseControl : MonoBehaviour
{

    [SerializeField] GameObject TextController;
    [SerializeField] GameObject[] Areas = new GameObject[3];
   

    // Use this for initialization
    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerStay(Collider other)
    {
        TextControl textControl = TextController.GetComponent<TextControl>();

        if (other.gameObject.name == "Player" && textControl.canvasCount<3)
        {
            textControl.DisplayText();
            StartCoroutine(InputWait(() =>
            {
                textControl.HideText();

                gameObject.transform.position = Areas[textControl.canvasCount].gameObject.transform.position;

            }));
        }
    }

    private IEnumerator InputWait(Action action)
    {
        yield return new WaitForSecondsRealtime(3.0f);
      
        while (true)
        {
            if (Input.anyKeyDown)
            {
                action();
                break;
            }

            yield return null;
        }
    }
}