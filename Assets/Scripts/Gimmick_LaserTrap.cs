using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ギミック：レーザートラップ
/// </summary>
public class Gimmick_LaserTrap : GimmickBase
{
    [SerializeField, Header("LineRendererのついたLaserTrap")]
    GameObject LaserTrap_LineRenderer;

    [SerializeField, Header("対となるLaserTrap")]
    GameObject LaserTrap;

    [SerializeField, Header("トラップ作動中かどうか")]
    bool TrapWorking = false;

    [SerializeField, Header("移動するレーザーかどうか")]
    bool MoveLaser;

    [SerializeField, Header("レーザートラップの移動先座標")]
    GameObject[] LaserMovePoints;

    [SerializeField, Header("移動速度"), Range(0f, 1f)]
    float MoveLerpSpeed;

    //移動先座標のリスト
    List<Vector3> NextMovePosition = new List<Vector3>();

    //移動先座標に到達したかどうか
    bool Reaching = false;

    LineRenderer L_renderer;

    //移動先座標のリスト番号
    int Index = 0;

	void Start ()
    {
        //初期座標・移動先座標を取得
        NextMovePosition.Add(transform.position);

        foreach(GameObject pos in LaserMovePoints)
        {
            NextMovePosition.Add(pos.transform.position);
        }

        //LineRendererを取得
        L_renderer = GetComponentInChildren<LineRenderer>();

        //頂点数を設定
        L_renderer.positionCount = 2;      
	}
	
	void Update ()
    {
        if (TrapWorking)
        {
            L_renderer.enabled = true;

            L_renderer.SetPosition(0, LaserTrap_LineRenderer.transform.position);
            L_renderer.SetPosition(1, LaserTrap.transform.position);

            RayCastSystem();
        }
        else
        {
            L_renderer.enabled = false;
        }

        if (MoveLaser)
        {
            LaserMove();
        }        
    }

    /// <summary>
    /// あたり判定の処理
    /// </summary>
    private void RayCastSystem()
    {
        RaycastHit hit;

        Debug.DrawLine(LaserTrap_LineRenderer.transform.position, LaserTrap.transform.position, Color.cyan);

        if(Physics.Linecast(LaserTrap_LineRenderer.transform.position, LaserTrap.transform.position, out hit, 1 << LayerMask.NameToLayer("Player")))
        {
            if(hit.collider.tag == TAGNAME.TAG_PLAYER)
            {
                //ダメージ処理
                hit.collider.gameObject.GetComponent<PlayerSystem>().Damage();
            }
        }
    }

    /// <summary>
    /// レーザーの移動処理
    /// </summary>
    private void LaserMove()
    {
        transform.position = Vector3.MoveTowards(transform.position, NextMovePosition[Index], MoveLerpSpeed);

        if (Vector3.Distance(transform.position, NextMovePosition[Index]) <= 0)
        {
            if (!Reaching)
            {
                if (Index == NextMovePosition.Count - 1) { Index--; Reaching = true; }
                else { Index++; }
            }
            else
            {
                if (Index == 0) { Index++; Reaching = false; }
                else { Index--; }
            }
        }
    }

    protected override void GimmickAction_Laser()
    {
        if (TrapWorking)
        {
            TrapWorking = false;
        }
        else
        {
            TrapWorking = true;
        }        
    }
}