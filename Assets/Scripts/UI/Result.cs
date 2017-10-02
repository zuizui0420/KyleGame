using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GamepadInput;

/// <summary>
/// リザルト
/// </summary>
public class Result : MonoBehaviour
{
    [SerializeField, Header("経過時間")]
    Text Timer_Text;

    [SerializeField, Header("ランク")]
    Text PlayerRank_Text;

    [SerializeField, Header("セレクト")]
    Image SelectImage;

    [SerializeField, Header("")]
    GameObject SelectTitlePos, SelectReplayPos;

    float ClearTime = 0f;

    bool SelectMode = false;

    Vector3 DefaultRankScale;

    //[0] リプレイ [1] タイトル
    int selectID = 0;

	void Start ()
    {
        //クリア時間を取得
        ClearTime = DATABASE.ResultTimer;

        DefaultRankScale = PlayerRank_Text.transform.localScale;

        PlayerRank_Text.transform.localScale = Vector3.zero;

        StartCoroutine(TimeCountAnimation());
	}

    void Update()
    {       
        if (SelectMode)
        { 
            //ゲームパッドのAxisを取得(右)
            if (GamePad.GetAxis(GamePad.Axis.LeftStick, GamePad.Index.One, true).x > 0)
            {
                SelectImage.transform.localPosition = SelectTitlePos.transform.localPosition ;
                selectID = 1;
            }
            //ゲームパッドのAxisを取得(左)
            else if (GamePad.GetAxis(GamePad.Axis.LeftStick, GamePad.Index.One, true).x < 0)
            {
                SelectImage.transform.localPosition = SelectReplayPos.transform.localPosition;
                selectID = 0;
            }

            //Xボタンを押した場合
            if (GamePad.GetButtonDown(GamePad.Button.X, GamePad.Index.One))
            {
                SelectMode = false;

                switch (selectID)
                {
                    case 0:
                        SceneFader.Instance.LoadLevel(SCENENAME.SCENE_STAGE1);
                        break;

                    case 1:
                        SceneFader.Instance.LoadLevel(SCENENAME.SCENE_TITLE);
                        break;
                }
            }
        }             
    }

    private IEnumerator TimeCountAnimation()
    {
        int currentTime = 0;

        while (currentTime < ClearTime)
        {
            currentTime++;

            //分数がある場合
            if(currentTime / 60 != 0)
            {
                Timer_Text.text = (currentTime / 60).ToString() + ":" + (currentTime - 60).ToString();               
            }
            else
            {
                Timer_Text.text = currentTime.ToString();
            }

            yield return null;
        }

        StartCoroutine(RankAnimation());
    }

    private IEnumerator RankAnimation()
    {
        while (PlayerRank_Text.transform.localScale.x < DefaultRankScale.x)
        {
            PlayerRank_Text.transform.localScale = Vector3.MoveTowards(PlayerRank_Text.transform.localScale, DefaultRankScale, 0.1f);

            yield return null;
        }

        SelectMode = true;    
    }
}