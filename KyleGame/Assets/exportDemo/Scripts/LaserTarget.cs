using UnityEngine;
using System.Collections;

public class LaserTarget : MonoBehaviour {
    private Vector3 Pos;//移動量変数
    private GameObject frontPos;//ゲームオブジェクト(FrontPos)の参照
    private float distanceX = -0.5f;//進んだ距離の変数、初期値を0にすると右がずれるので初期値を-0.5に設定
    private float distanceY = 0;//進んだ距離Yの変数
 

    // Use this for initialization
    void Start () {
        
        //ゲームオブジェクト(FrontPos)をセット
        frontPos = GameObject.Find("FrontPos");
     
       

    }
	
	// Update is called once per frame
	void Update ()
    {
        
        //視点移動時
        if (Input.GetButtonUp("Fire1"))
        {
            transform.position = frontPos.transform.position;//初期位置の設定
            //初期値に戻す
            distanceX = -0.5f;
            distanceY = 0;
        }

        //移動量設定
        if (Input.GetButton("Fire1") && Input.GetKey(KeyCode.D))
        {
            Pos.x += 1.0f;
            distanceX += 1.0f;
        }
        if (Input.GetButton("Fire1") && Input.GetKey(KeyCode.A))
        {
            Pos.x -= 1.0f;
            distanceX -= 1.0f;
        }
        if (Input.GetButton("Fire1") && Input.GetKey(KeyCode.W))
        {
            Pos.y += 1.0f;
            distanceY += 1.0f;
        }
        if (Input.GetButton("Fire1") && Input.GetKey(KeyCode.S))
        {
            Pos.y -= 1.0f;
            distanceY -= 1.0f;
        }

        //移動量の限界を設定
        if (Mathf.Abs(distanceX) >= 50.0f)
        {
            Pos.x = 0;
            if (distanceX >= 50.0f)
                distanceX = 50.0f;
            if (distanceX <= -50.0f)
                distanceX = -50.0f;
        }
        if (Mathf.Abs(distanceY) >= 15.0f)
        {
            Pos.y = 0;
            if (distanceY >= 15.0f)
                distanceY = 15.0f;
            if (distanceY <= -15.0f)
                distanceY = -15.0f;
        }

        //移動実行
        transform.localPosition += Pos;
        //移動量リセット
        Pos = new Vector3(0, 0, 0);
        
    }
}
