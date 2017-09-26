using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserSoundControl : MonoBehaviour {

    [SerializeField] private AudioSource[] audioSources = new AudioSource[2];

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        StartCoroutine("LaserSoundPlay");
		
	}

    private IEnumerator LaserSoundPlay()
    {
        float startTime = Time.timeSinceLevelLoad;
        audioSources[0].Play();

        while (true)
        {
            float time = Time.timeSinceLevelLoad - startTime;
            if(time >= 2)
            {
                audioSources[1].Play();
                break;
            }
        }

        yield return null;
    }
}
