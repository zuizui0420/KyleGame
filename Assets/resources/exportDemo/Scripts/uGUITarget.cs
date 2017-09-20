using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class uGUITarget : MonoBehaviour {

    private Transform targetPos;
    private RectTransform mark;

    Canvas canvas;


    private Image image;
   
	// Use this for initialization
	void Start () {
      
        mark = GameObject.Find("TargetUI").GetComponent<RectTransform>();

        canvas = GetComponent<Graphic>().canvas;

        image = GetComponent<Image>();
        image.enabled = false;

    }

    // Update is called once per frame
    void Update () {

        if (Input.GetButton("Fire1"))
            image.enabled = true;

        targetPos = GameObject.Find("Target").GetComponent<Transform>();      
       
        var pos = Vector2.zero;
        var worldCamera = Camera.main;
        var canvasRect = canvas.GetComponent<RectTransform>();



        var screenPos = worldCamera.WorldToScreenPoint( targetPos.position);
        
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPos, null, out pos);
        mark.localPosition = pos;





        //mark.localPosition = targetRect.position;


        if (!Input.GetButton("Fire1"))
            image.enabled = false;


    }

   
}
