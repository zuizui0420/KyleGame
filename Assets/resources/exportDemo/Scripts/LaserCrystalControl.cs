using UnityEngine;
using System.Collections;

public class LaserCrystalControl : MonoBehaviour {

    //レーザーがヒットしているオブジェクトの参照用変数
    private GameObject hitObjectL = null;
    private GameObject hitObjectR = null;
    //Rendererコンポーネント参照用変数
    private Renderer rend;
    //緑と青の値
    private float G = 1.0f;
    private float B = 1.0f;
    //レーザーがヒットしていた時間
    private float hitTime = 0;
    //満タンかチェック
    private bool fullCharge = false;
	// Use this for initialization
	void Start ()
    {
       
	}
	
	// Update is called once per frame
	void Update ()
    {
        //ヒットしているオブジェクトの参照
        hitObjectL  = GameObject.Find("LeftLaser").GetComponent<LaserLeft>().hitObject;
        hitObjectR = GameObject.Find("RightLaser").GetComponent<RightLaser>().hitObject;
        if (Input.GetButton("Fire2"))
        {
            try {
                //ヒットしてたオブジェクトがLaserCrystalのとき
                if (hitObjectL.tag == "LaserCrystal" && hitObjectR.tag == "LaserCrystal")
                {
                    
                    //Time.deltaTimeを10で割って足していく
                    hitTime += Time.deltaTime / 10;
                    //テスト
                    Debug.Log(hitTime);
                    //LaserCrystalのRendererを参照
                    rend = hitObjectL.GetComponent<Renderer>();
                    if (hitTime <= 0.8f)                   
                        rend.material.color = new Color(1.0f, G - hitTime, B - hitTime);                 
                    else if (hitTime > 0.8f)
                        fullCharge = true;

                }
            }
            catch { };//try/catchでエラーを無視
           
          
                
        }
        
          
	
	}
}
