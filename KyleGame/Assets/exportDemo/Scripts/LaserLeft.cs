using UnityEngine;
using System.Collections;

public class LaserLeft : MonoBehaviour
{
    //目標座標
    Vector3 targetPos;
    //レーザーが発射しているとかどうか
    public bool fire = false;
    //レーザー左
    GameObject leftLaser;
    //レーザーの頂点の数
    public int positions = 2;
    //レーザーがヒットしたかどうか
    public bool hit;
    //レーザーがヒットしている座標
    public  Vector3 hitPos;
    //ヒットしたゲームオブジェクト
    public GameObject hitObject;

    private GameObject player;
    private bool modeCheck;

    // Use this for initialization
    void Start()
    {
        //ゲームオブジェクト参照
        leftLaser = GameObject.Find("LeftLaser");
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        //modeCheck = player.GetComponent<LocomotionPlayer>().modeN;

        //目標座標の設定
        targetPos = GameObject.Find("Target").GetComponent<Transform>().transform.position;
        //LineRendererコンポーネントの参照
        LineRenderer leftLaser = GetComponent<LineRenderer>();
        //LineRendererの頂点の数
        Vector3[] points = new Vector3[positions];
        //頂点0の設定
        points[0] = transform.position;

        //レーザー左照射
        if (Input.GetButton("Fire1") && Input.GetButton("Fire2"))
        {
            fire = true;
            points[1] = targetPos;
        }
        //照射していない時
        else {
            points[1] = transform.position;
            fire = false;
        }

        if (Input.GetButtonUp("Fire2"))
            hitPos = new Vector3(0,10000,0);//ヒットした座標の初期化、初期化しなければ次の発射時に一瞬だけ前に座標が入る

        //LineRendererのpositionsを設定
        leftLaser.SetPositions(points);


    }

    //コライダーをスクリプトやアニメーションで移動させる場合、物理ライブラリが更新されて Raycast が新しい位置に当たるよう最低でも1度 FixedUpdate の実行を待つ必要がありる
    public void FixedUpdate()
    {

        //RaycastHit参照用変数???
        RaycastHit hitInfo;
        if (fire == true)
        {
            if (Physics.Raycast(transform.position, targetPos - transform.position, out hitInfo))
            {               
                hitPos = hitInfo.point;
                this.hit = true;
                hitObject = hitInfo.collider.gameObject;
            }
            if (!Physics.Raycast(transform.position, targetPos - transform.position, out hitInfo))
            {
                this.hit = false;
                hitObject = null;
            }

           
        }

    }
}
