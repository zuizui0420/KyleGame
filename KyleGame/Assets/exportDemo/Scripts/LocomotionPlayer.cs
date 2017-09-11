 /// <summary>
/// 
/// </summary>

using UnityEngine;
using System;
using System.Collections;
  
[RequireComponent(typeof(Animator))]  

//Name of class must be name of file as well

public class LocomotionPlayer : MonoBehaviour {

    protected Animator animator;
    private CharacterController charCon;

    public float speed = 0;
    private float direction = 0;
    private Locomotion locomotion = null;


   // private bool jump = false;
   // private Vector3 height = Vector3.zero;
    public bool modeN = true;
    public bool modeE = false;
    public bool modeQ = false;
    public bool attackE = false;
    private float count = 0.5f;



    // Use this for initialization
    void Start () 
	{
        animator = GetComponent<Animator>();
        locomotion = new Locomotion(animator);
        charCon = GetComponent<CharacterController>();

    }
    
	void Update () 
	{
        if (animator && Camera.main)
		{
            if (!Input.GetButton("Fire1"))
                JoystickToEvents.Do(transform, Camera.main.transform, ref speed, ref direction);
            locomotion.Do(speed * 6, direction * 180);

            if (Input.GetButton("Fire1"))
            {
                speed = 0;
                direction = 0;
            }
		}

        //if (Input.GetButton("Jump"))
        //{
        //    animator.SetBool("Jump", true);
        //    jump = true;
        //    height.y = 7.0f;
        //    charCon.Move(height * Time.deltaTime);
            
            
        //}
        //if (jump == true)
        //{
        //    height.y += -9.81f * Time.deltaTime;
        //    charCon.Move(height * Time.deltaTime);
        //}
        //if(transform.position.y <= 0.15)
        //{
        //    height.y = 0;
        //    jump = false;
        //    animator.SetBool("Jump", false);
        //}

        if (Input.GetKeyDown(KeyCode.E))
        {
          
            if (modeE == false)
            {
                modeE = true;
                modeN = false;
                modeQ = false;
            }

            else if(modeE == true)
            {
                modeE = false;
                modeN = true;
            }
            
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            if(modeQ == false)
            {
                modeQ = true;
                modeE = false;
            }
            else  if (modeQ == true)
            {
                modeQ = false;
                modeN = true;
            }
        }
        if (Input.GetButton("Fire1") && Input.GetButton("Fire2"))
            animator.SetBool("Spark", true);

        if(modeE == false&&!Input.GetButton("Fire1"))
            animator.SetBool("Spark", false);

        if (modeE == true && Input.GetButton("Fire2"))
        {
            animator.SetBool("Spark", true);
            count -= Time.deltaTime;
            if(count<=0)
            attackE = true;
        }



        if (Input.GetButtonUp("Fire2"))
        {
            animator.SetBool("Spark", false);
            attackE = false;
            count = 0.5f;
        }

    }
}
