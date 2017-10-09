using UnityEngine;
using System.Collections;

public class HitEffectControl : MonoBehaviour {

    GameObject RightLaser;
    GameObject LeftLaser;
   

    public GameObject effct;

	// Use this for initialization
	void Start ()
    {
        RightLaser = GameObject.Find("RightLaser");
        LeftLaser = GameObject.Find("LeftLaser");
	
	}
	
	// Update is called once per frame
	void Update()
    {
        Vector3 LaserPosR;
        Vector3 LaserPosL;
        bool fireR;
        bool fireL;
        bool hitR;
        bool hitL;


        fireR = RightLaser.GetComponent<RightLaser>().fire;
        fireL = LeftLaser.GetComponent<LaserLeft>().fire;
        hitR = RightLaser.GetComponent<RightLaser>().hit;
        hitL = LeftLaser.GetComponent<LaserLeft>().hit;

       
        

        if (fireR == true && hitR == true)
        {
            LaserPosR = RightLaser.GetComponent<RightLaser>().hitPos;
            Instantiate(effct, LaserPosR, Quaternion.identity);
        }
        if (fireL == true && hitL == true)
        {
            LaserPosL = LeftLaser . GetComponent<LaserLeft>().hitPos;
            Instantiate(effct, LaserPosL, Quaternion.identity);
        }


    }
}
