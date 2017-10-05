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

    string[] RankNum = { "D", "C", "B", "A", "S" };

    //[0] リプレイ [1] タイトル
    int selectID = 0;

    int deadCount = 0;

	void Start ()
    {
        //クリア時間を取得
        ClearTime = DATABASE.ResultTimer;

        if(DATABASE.DeadCount == 0)
        {
            deadCount = 4;
        }
        else if(DATABASE.DeadCount <= 1)
        {
            deadCount = 3;
        }
        else if(DATABASE.DeadCount <= 3)
        {
            deadCount = 2;
        }
        else if(DATABASE.DeadCount <= 5)
        {
            deadCount = 1;
        }
        else if(DATABASE.DeadCount <= 7)
        {
            deadCount = 0;
        }

        //死亡回数
        deadCount = DATABASE.DeadCount;

        DefaultRankScale = PlayerRank_Text.transform.localScale;

        PlayerRank_Text.transform.localScale = Vector3.zero;

        StartCoroutine(TimeCountAnimation());

        //データベースを初期化
        DATABASE.ResultTimer = 0f;
        DATABASE.DeadCount = 0;
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
                        SceneFader.Instance.LoadLevel(SceneName.Stage1);
                        break;

                    case 1:
                        SceneFader.Instance.LoadLevel(SceneName.Title);
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
            if(currentTime > 60)
            {
	            var minute = currentTime / 60;
	            var seconds = currentTime % 60;
	            Timer_Text.text = string.Format("{0}:{1}", minute, seconds);
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
        PlayerRank_Text.text = RankNum[deadCount];

        while (PlayerRank_Text.transform.localScale.x < DefaultRankScale.x)
        {
            PlayerRank_Text.transform.localScale = Vector3.MoveTowards(PlayerRank_Text.transform.localScale, DefaultRankScale, 0.1f);

            yield return null;
        }

        SelectMode = true;    
    }
}