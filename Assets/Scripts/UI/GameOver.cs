using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GamepadInput;

/// <summary>
/// ゲームオーバー
/// </summary>
public class GameOver : MonoBehaviourExtension
{
    [SerializeField, Header("セレクト")]
    Image SelectImage;

    [SerializeField, Header("")]
    GameObject SelectTitlePos, SelectReplayPos;

    bool SelectMode = false;

    //[0] コンティニュー [1] タイトル
    int selectID = 0;

    void Start()
    {
        WaitAfter(1.0f, () =>
        {
            SelectMode = true;
        });
    }

    void Update()
    {
        if (SelectMode)
        {
            //ゲームパッドのAxisを取得(右)
            if (GamePad.GetAxis(GamePad.Axis.LeftStick, GamePad.Index.One, true).x > 0)
            {
                SelectImage.transform.localPosition = SelectTitlePos.transform.localPosition;
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
}