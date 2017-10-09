using UnityEngine;
using System.Collections;

public class ElectricalEffectControl : MonoBehaviour {
    private GameObject player;
    private bool modeCheck;
    private float playerSpeed;
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
        modeCheck = player.GetComponent<LocomotionPlayer>().attackE;
        playerSpeed = player.GetComponent<LocomotionPlayer>().speed;

        if (modeCheck == true && !Input.GetButton("Fire1"))
        { 
            particle.Play();
        }
            
        if (modeCheck == false||Input.GetButton("Fire2")||playerSpeed !=0)
            particle.Stop();
    }

    
}
