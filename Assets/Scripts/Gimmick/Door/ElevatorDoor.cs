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

                GetComponent<Door>().Close();                

                WaitAfter(3.0f, () =>
                {
                    AudioManager.Instance.BGMFadeOut(AUDIONAME.BGM_STAGE, 0.5f);

                    //シーン遷移
                    SceneFader.Instance.LoadLevel(StageManager.Instance.ReturnNextSceneName());

                    //エレベーターを動かす
                    StartCoroutine(ElevatorMove());

					//音
					AudioManager.Instance.Play(AUDIONAME.SE_ELEVATOR,0.8f,false,128);


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
