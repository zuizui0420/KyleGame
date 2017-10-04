using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack_Laser : MonoBehaviour
{
    [SerializeField,Header("レーザー照射中")]
    public bool fire = false;

    [SerializeField, Header("レーザー")]
    GameObject[] Lasers;

    [SerializeField, Header("Ray照射ターゲット")]
    GameObject RayTarget;

    [SerializeField, Header("レーザーヒット時のエフェクト")]
    GameObject HitFX;

    //目標座標
    Vector3 targetPos;    

    //LineRendererの管理リスト
    List<LineRenderer> L_Renderer = new List<LineRenderer>();

    private void Start()
    {
        //目標座標の設定
        targetPos = RayTarget.transform.position;

        //LineRendererコンポーネントの参照
        foreach(GameObject renderer in Lasers)
        {
            L_Renderer.Add(renderer.GetComponent<LineRenderer>());
        }

        //LineRendererの各種設定
        foreach(LineRenderer renderer in L_Renderer)
        {
            renderer.positionCount = 2;
            renderer.SetPosition(0, renderer.transform.position);
        }      
    }

    void Update()
    {
        if (fire)
        {
            //頂点１の設定
            foreach (LineRenderer renderer in L_Renderer)
            {
                renderer.enabled = true;
                renderer.SetPosition(0, renderer.transform.position);
                renderer.SetPosition(1, RaycastHit_System());
            }

            //エフェクト生成
            Instantiate(HitFX, RaycastHit_System(), Quaternion.identity);
        }
        else
        {
            foreach (LineRenderer renderer in L_Renderer)
            {
                renderer.enabled = false;
            }
        }
    }

    /// <summary>
    /// RayCastの制御・判定
    /// </summary>
    /// <returns></returns>
    private Vector3 RaycastHit_System()
    {
        RaycastHit hit;

        Vector3 hitPoint;

        Debug.DrawRay(transform.position, RayTarget.transform.position - transform.position, Color.yellow);

        if (Physics.Raycast(transform.position, RayTarget.transform.position - transform.position, out hit, Mathf.Infinity))
        {
            hitPoint = hit.point;

            //ギミック・敵の場合は、ダメージを与える
            if(hit.collider.CompareTag(TAGNAME.TAG_GIMMICK_ENEMY))
            {
                hit.collider.gameObject.GetComponent<GimmickBase>().GimmickDamage();
            }
            else if(hit.collider.CompareTag(TAGNAME.TAG_ENEMY))
            {
                hit.collider.gameObject.GetComponent<EnemyBase>().EnemyDamage();
            }
        }
        else
        {
            hitPoint = RayTarget.transform.position;
        }

        return hitPoint;
    }   
}