using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour {
    public GameObject target;

    private float life_time = 1.0f;
    private Vector3 move_pos;
  

	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {

        target = GameObject.Find("Target_boss");


        transform.position = Vector3.Lerp(transform.position,target.transform.position,0.2f);
        
        Destroy(this.gameObject,life_time);

    }
}
