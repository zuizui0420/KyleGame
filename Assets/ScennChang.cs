
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace InariSystem
{
    public class ScennChang : MonoBehaviour {
        

        // Use this for initialization
      
        public void Update()
        {

            if (Input.GetMouseButtonDown(0))
            {
                ScenFader.Instance.LoadLevel("DemoScene");

            }
        }
    }
    // Update is called once per frame
    
}
