using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack_Laser : MonoBehaviour
{
    //レーザーが発射しているとかどうか
    [HideInInspector]
    public bool fire = false;

    [SerializeField, Header("Ray照射ターゲット")]
    GameObject RayTarget;

    [SerializeField, Header("レーザーヒット時のエフェクト")]
    GameObject HitFX;

    private bool modeCheck;

    //目標座標
    Vector3 targetPos;    

    //レーザーの頂点の数
    int positions = 2;

    //レーザーがヒットしたかどうか
    bool hit;

    //レーザーがヒットしている座標
    Vector3 hitPos;

    //ヒットしたゲームオブジェクト
    GameObject hitObject;   

    //LineRendererの頂点の数
    Vector3[] points = new Vector3[2];

    LineRenderer LaserLine;

    private void Start()
    {
        //目標座標の設定
        targetPos = RayTarget.transform.position;

        //LineRendererコンポーネントの参照
        LaserLine = GetComponent<LineRenderer>();

        LaserLine.positionCount = 2;        
    }

    void Update()
    {
        //頂点0の設定
        LaserLine.SetPosition(0, transform.position);

        if (fire)
        {
            //頂点1の設定
            LaserLine.SetPosition(1, RayTarget.transform.position);

            RaycastHit_System();
        }
        else
        {
            //頂点1を頂点0と同じにすることでレンダリングをさせない
            LaserLine.SetPosition(1, transform.position);

            //ヒットした座標の初期化、初期化しなければ次の発射時に一瞬だけ前に座標が入る
            hitPos = new Vector3(0, 10000, 0);
        }
    }

    private void RaycastHit_System()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, RayTarget.transform.position - transform.position, out hit, 100f))
        {
            Instantiate(HitFX, hit.point, Quaternion.identity);
        }

        //if (Physics.Raycast(transform.position, targetPos - transform.position, out hitInfo))
        //{
        //    hit = true;
        //    hitPos = hitInfo.point;            
        //    hitObject = hitInfo.collider.gameObject;
        //}
        //else
        //{
        //    hit = false;
        //    hitPos = hitInfo.point;
        //    hitObject = null;
        //}
    }
}