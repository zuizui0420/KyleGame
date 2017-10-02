using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ステージ情報を管理
/// </summary>
public class StageManager : SingletonMonoBehaviour<StageManager>
{
    [SerializeField, Header("次に遷移するシーン名")]
    STAGE NextSceneName;

    public enum STAGE
    {
        Stage1,
        Stage2,
        Stage3,
        Stage_Boss,
        Result,
    }

    void Start()
    {
        AudioManager.Instance.PlayAudio(AUDIONAME.BGM_STAGE, 1, true, 128);
    }

    void Update()
    {
        DATABASE.ResultTimer += Time.deltaTime;
    }

    public string ReturnNextSceneName()
    {
        string name = "";

        switch (NextSceneName)
        {
            case STAGE.Stage1:

                name = SCENENAME.SCENE_STAGE1;

                break;

            case STAGE.Stage2:

                name = SCENENAME.SCENE_STAGE2;

                break;

            case STAGE.Stage3:

                name = SCENENAME.SCENE_STAGE3;

                break;

            case STAGE.Stage_Boss:

                name = SCENENAME.SCENE_STAGE_BOSS;

                break;

            case STAGE.Result:

                name = SCENENAME.SCENE_RESULT;

                break;
        }

        return name;
    }
}