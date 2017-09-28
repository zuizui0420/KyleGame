using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ギミック：エレベータードア
/// </summary>
public class ElevatorDoor : MonoBehaviourExtension
{
    [SerializeField, Header("移動先座標")]
    GameObject MovePoint;

    bool ElevatorMoving = false;

    void OnTriggerEnter(Collider col)
    {
        if (!ElevatorMoving)
        {
            if (col.gameObject.tag == TAGNAME.TAG_PLAYER)
            {
                col.gameObject.transform.parent = transform;
                //col.gameObject.GetComponent<Rigidbody>().isKinematic = true;

                GetComponent<Door>().Close();

                WaitAfter(3.0f, () =>
                {
                    //シーン遷移
                    SceneFader.Instance.LoadLevel(StageManager.Instance.ReturnNextSceneName());

                    //エレベーターを動かす
                    StartCoroutine(ElevatorMove());
                });               
            }
        }        
    }

    private IEnumerator ElevatorMove()
    {
        float CurrentTime = 5f;

        while (CurrentTime > 0f)
        {
            CurrentTime -= Time.deltaTime;

            transform.position = Vector3.MoveTowards(transform.position, MovePoint.transform.position, 0.1f);

            yield return null;
        }
    }
}
