using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserSoundControl : MonoBehaviour {

    [SerializeField] private AudioSource audioSources;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        if (Input.GetMouseButtonDown(1))
            audioSources.Play();
        else if(Input.GetMouseButtonUp(1))
            audioSources.Stop();
		
	}

}
