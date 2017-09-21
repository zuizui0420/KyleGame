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

    //目標座標
    Vector3 targetPos;    

    //レーザーの頂点の数
    int positions = 2;

    //レーザーがヒットしたかどうか
    bool hit;

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
            LaserLine.SetPosition(1, RaycastHit_System());

            //エフェクト生成
            Instantiate(HitFX, RaycastHit_System(), Quaternion.identity);
        }
        else
        {
            //頂点1を頂点0と同じにすることでレンダリングをさせない
            LaserLine.SetPosition(1, transform.position);
        }
    }

    private Vector3 RaycastHit_System()
    {
        RaycastHit hit;

        Vector3 HitPoint = Vector3.zero;

        Debug.DrawRay(transform.position, RayTarget.transform.position - transform.position, Color.yellow);

        if (Physics.Raycast(transform.position, RayTarget.transform.position - transform.position, out hit, 100f))
        {
            HitPoint = hit.point;
        }
        else
        {
            HitPoint = RayTarget.transform.position;
        }

        return HitPoint;
    }
}