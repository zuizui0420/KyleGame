using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossLaserControl : MonoBehaviour {

    public GameObject eyesPos,targetPos,laser;//Laserの発射座標と着弾座標
    public bool fire;
  
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {


        //レーザー照射
        if (fire)
        {
            Instantiate(laser, transform.position, Quaternion.identity);
            fire = false;
          
        }
        //照射していない時
        else
        {
            
        }


    }

    private void FixedUpdate()
    {
        RaycastHit hitInfo;//ヒットしたオブジェクトの情報
        if(fire)
        Physics.Raycast(eyesPos.transform.position, targetPos.transform.position - eyesPos.transform.position,out hitInfo);
    }
}
