using UnityEngine;
using System.Collections;

public class QuickEffectControl : MonoBehaviour {

    private GameObject player;
    public bool modeCheck;
    private ParticleSystem particle;

    // Use this for initialization
    void Start ()
    {
        player = GameObject.Find("Player");
        particle = this.GetComponent<ParticleSystem>();

        particle.Stop();

    }
	
	// Update is called once per frame
	void Update ()
    {
        modeCheck = player.GetComponent<LocomotionPlayer>().modeQ;

        if (modeCheck == true)
            particle.Play();
        if (modeCheck == false)
            particle.Stop();

    }
}
