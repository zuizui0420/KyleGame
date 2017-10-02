using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Taitol : MonoBehaviour {
 

    // Use this for initialization
  
	
	// Update is called once per frame
	void Update () {

        if (Input.GetMouseButtonDown(0))
        {

            SceneManager.LoadScene("setumei");


        }
        
    }
}
