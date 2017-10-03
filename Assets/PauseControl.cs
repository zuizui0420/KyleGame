using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseControl : MonoBehaviour {

    [SerializeField] GameObject TextController;

	// Use this for initialization
	void Start () {
        
        
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        TextControl textControl = TextController.GetComponent<TextControl>();

        if (other.gameObject.name == "Player")
        {
            textControl.DisplayText();
        }
    }
}
