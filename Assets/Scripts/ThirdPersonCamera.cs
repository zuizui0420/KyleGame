//
// Unityちゃん用の三人称カメラ
// 
// 2013/06/07 N.Kobyasahi
//
using UnityEngine;
using System.Collections;


public class ThirdPersonCamera : MonoBehaviour
{
    float worldTime = 0;

	public float smooth =3f;		// カメラモーションのスムーズ化用変数
	Transform standardPos;			// the usual position for the camera, specified by a transform in the game
	Transform fpsPos;			// Front Camera locater
    Transform processPos;
    Transform jumpPos;			// Jump Camera locater

    GameObject player;
    Vector3 plyerPos;
 

   
    // スムーズに繋がない時（クイック切り替え）用のブーリアンフラグ
    bool bQuickSwitch = false;	//Change Camera Anchor Quickly
    
	void Start()
	{
		// 各参照の初期化
		standardPos = GameObject.Find ("CamPos").transform;
		
		if(GameObject.Find ("FpsPos"))
			fpsPos = GameObject.Find ("FpsPos").transform;

		if(GameObject.Find ("JumpPos"))
			jumpPos = GameObject.Find ("JumpPos").transform;

        if (GameObject.Find("ProcessPos"))
            processPos = GameObject.Find("ProcessPos").transform;

        player = GameObject.Find("Player");
        plyerPos = player.transform.position;

        //カメラをスタートする
            transform.position = standardPos.position;	
			transform.forward = standardPos.forward;	
	}
    void Update()
    {
        worldTime += Time.deltaTime;
       
      
    }
	
	void FixedUpdate ()	// このカメラ切り替えはFixedUpdate()内でないと正常に動かない
	{

        if (Input.GetButton("Fire1"))   // left Ctlr
        {
            // Change Front Camera
            setCameraPositionFrontView();
        }
       
        else if (Input.GetButton("Fire2"))  //Alt
        {
            // Change Jump Camera
            //setCameraPositionJumpView();
            setCameraPositionNormalView();
        }

        else
        {
            // return the camera to standard position and direction
            setCameraPositionNormalView();
        }
	}

	void setCameraPositionNormalView()
	{
       
		if(bQuickSwitch == false){
            // the camera to standard position and direction
            transform.position = Vector3.Lerp(transform.position, standardPos.position, Time.fixedDeltaTime * smooth);
            transform.forward = Vector3.Lerp(transform.forward, standardPos.forward, Time.fixedDeltaTime * smooth);
		}
		else{
			// the camera to standard position and direction / Quick Change
			transform.position = standardPos.position;	
			transform.forward = standardPos.forward;
			bQuickSwitch = false;
		}
	}

	
	void setCameraPositionFrontView()
	{
		// Change Front Camera
		bQuickSwitch = true;
        transform.position = Vector3.Lerp(transform.position, processPos.position, 0.2f);
        if (Mathf.Abs(player.transform.position.z - transform.localPosition.z) <= 1.3f&&Mathf.Abs(player.transform.position.x-transform.position.x)<=1.2f)
        {
            transform.position = fpsPos.position;
        }
        transform.forward = Vector3.Lerp(transform.forward, processPos.forward, 0.1f);
       
       
    }

	void setCameraPositionJumpView()
	{
        //また後で使う
		// Change Jump Camera
		//bQuickSwitch = false;
		//		transform.position = Vector3.Lerp(transform.position, jumpPos.position, Time.fixedDeltaTime * smooth);	
		//		transform.forward = Vector3.Lerp(transform.forward, jumpPos.forward, Time.fixedDeltaTime * smooth);		
	}
}
