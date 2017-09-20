using UnityEngine;
using System.Collections;

public class MagneticEffectControll : MonoBehaviour {

    private GameObject player;
    private bool modeCheck;
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
        modeCheck = player.GetComponent<LocomotionPlayer>().modeE;

        if (modeCheck == true)
            particle.Play();
        if (modeCheck == false)
            particle.Stop();

    }
}
