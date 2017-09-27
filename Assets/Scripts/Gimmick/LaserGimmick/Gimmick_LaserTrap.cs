using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ギミック：レーザートラップ
/// </summary>
public class Gimmick_LaserTrap : GimmickBase
{
    [SerializeField, Header("対となるLaserTrap")]
    GameObject LaserTrap;

    [SerializeField, Header("トラップ作動中かどうか")]
    bool TrapWorking = false;

    LineRenderer L_renderer;

	void Start ()
    {
        L_renderer = GetComponent<LineRenderer>();

        L_renderer.positionCount = 2;      
	}
	
	void Update ()
    {
        if (TrapWorking)
        {
            L_renderer.enabled = true;

            L_renderer.SetPosition(0, transform.position);
            L_renderer.SetPosition(1, LaserTrap.transform.position);

            RayCastSystem();
        }
        else
        {
            L_renderer.enabled = false;
        }  
    }

    private void RayCastSystem()
    {
        RaycastHit hit;

        Debug.DrawRay(transform.position, LaserTrap.transform.position - transform.position, Color.cyan);

        if(Physics.Raycast(transform.position,LaserTrap.transform.position - transform.position,out hit,
            Vector3.Distance(transform.position, LaserTrap.transform.position)))
        {
            if(hit.collider.tag == TAGNAME.TAG_PLAYER)
            {
                //ダメージ処理
                hit.collider.gameObject.GetComponent<PlayerSystem>().Damage();
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