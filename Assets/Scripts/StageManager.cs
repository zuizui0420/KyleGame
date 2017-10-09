using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// ステージ情報を管理
/// </summary>
public class StageManager : SingletonMonoBehaviour<StageManager>
{
    [SerializeField, Header("次に遷移するシーン名")]
    private SceneName NextSceneName;

    void Start()
    {
        AudioManager.Instance.Play(AUDIONAME.BGM_STAGE, 0.5f, true, 128);

        DATABASE.SaveScene = SceneManager.GetActiveScene().name;    
    }

    void Update()
    {
        DATABASE.ResultTimer += Time.deltaTime;
    }

    public string ReturnNextSceneName()
    {

	    return Enum.GetName(typeof(SceneName), NextSceneName);

        //switch (NextSceneName)
        //{
        //    case STAGE.Stage1:

        //        name = SceneName.Stage1;

        //        break;

        //    case STAGE.Stage2:

        //        name = SceneName.Stage2;

        //        break;

        //    case STAGE.Stage3:

        //        name = SceneName.Stage3;

        //        break;

        //    case STAGE.Stage_Boss:

        //        name = SceneName.Stage_Boss;

        //        break;

        //    case STAGE.Result:

        //        name = SceneName.Result;

        //        break;
        //}
    }
}