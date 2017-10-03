using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextControl : MonoBehaviour {

    [SerializeField] GameObject[] canvas = new GameObject[3];
    
    public int canvasCount = 0;


    // Use this for initialization
    void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    public void DisplayText()
    {
        canvas[canvasCount].SetActive(true);

        Time.timeScale = 0;
    }

    public void HideText()
    {
        canvas[canvasCount].SetActive(false);
        canvasCount++;

        Time.timeScale = 1;
    }
}
