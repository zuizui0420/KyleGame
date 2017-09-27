using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

//このクラスでは、シーン遷移をしたときに暗転をするなどの画面効果を出力するためのものです
public class SceneFader : MonoBehaviour
{
    //シングルトンを作成
    #region Singleton
    //遷移先のシーンでこのスクリプトを使用するためのインスタンスを保存するためのもの
    private static SceneFader instance;

    //現在のシーンから外部で使用するインスタンスを作成
    public static SceneFader Instance
    {
        get
        {
            //送るインスタンスがnullの場合
            if (instance == null)
            {
                //シーンからSceneFaderを探して取得する
                instance = (SceneFader)FindObjectOfType(typeof(SceneFader));

                //もしシーン上にない場合
                if (instance == null)
                {
                    //エラーメッセージをログに出力する
                    Debug.LogError(typeof(SceneFader) + "is nothing");
                }
            }

            //送るインスタンスを返す
            return instance;
        }
    }

    #endregion Singleton

    //フェード中の透明度
    private float FadeAlpha = 0;
    
    //フェード中かどうかの判定
    private bool isFading = false;

    //フェード中の時間
    public float interval;

    //フェードの色
    Color fadeColor = Color.black;

    //非同期動作で使用するAsyncOperation
    AsyncOperation async;

    //ローディング中かどうかの判定
    bool NowLoading = false;

    //Async用プログレス
    private float AsyncProgress = 0.0f; 

    //開始時に読むこむ
    public void Awake()
    {
        //ほかのインスタンスが入っている場合
        if (this != Instance)
        {
            //インスタンスをリセットする
            Destroy(gameObject);
            return;
        }

        //取得したインスタンスをシーン上に作成する
        DontDestroyOnLoad(gameObject);
    }

    //画面に貼り付ける画像(GUI)の設定
    public void OnGUI()
    {
        //フェード中に色と透明度を更新して白テクスチャを描画
        if (isFading)
        {          
            //透明度を取得
            fadeColor.a = FadeAlpha;

            //GUIの色をfadeColorから取得する
            GUI.color = fadeColor;

            //白テクスチャを画面の比率に合わせて描画する
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);
        }       
    }

    //外部で呼んで使用するシーン遷移をするメソッド
    //LoadLevel(シーン名,暗転にかかる時間
    public void LoadLevel(string scene)
    {
        StartCoroutine(TransScene(scene));
    }

    /// <summary>
    /// フェードオン
    /// </summary>
    public void FadeOn()
    {
        StartCoroutine(TransScene());
    }

    /// <summary>
    /// Asyncでのシーン遷移をする
    /// </summary>
    /// <param name="scene">シーン名</param>
    /// <returns></returns>
    public void LoadAsyncLevel(string scene)
    {
        //読み込み開始
        StartCoroutine(AsyncLoading(scene));
    }

    /// <summary>
    /// Asyncでの読み込みを開始する
    /// </summary>
    public void AsyncLoadStart()
    {
        StartCoroutine(TransAsyncScene());
    }

    /// <summary>
    /// シーン遷移用コルーチン
    /// </summary>
    /// <param name="scene">シーン名</param>
    /// <returns></returns>
    private IEnumerator TransScene(string scene = "")
    {
        //画面をだんだん暗くするようにする
        //フェード中の判定をTrueにする
        isFading = true;

        //intervalと比較する時間
        float time = 0;

        //intervalよりtimeが下回っている間
        while (time <= interval)
        {
            //フェード中の透明度を０～１の間で変更していく
            FadeAlpha = Mathf.Lerp(0f, 1f, time / interval);

            //timeを1フレームづつ増加させていく
            time += Time.deltaTime;

            yield return null;
        }

        //シーン名がない場合は遷移をしない
        if(scene != "")
        {
            //シーンの切り替え
            SceneManager.LoadScene(scene);
        }
        
        //画面をだんだん明るくするようにする
        //比較時間をリセットする
        time = 0;

        //intervalよりtimeが下回っている間
        while (time <= interval)
        {
            //フェード中の透明度を1～0の間で変更していく
            FadeAlpha = Mathf.Lerp(1f, 0f, time / interval);

            //timeを１フレームづつ増加させていく
            time += Time.deltaTime;

            yield return null;
        }

        //フェード中の判定をFalseにする
        isFading = false;
    }

    /// <summary>
    /// Asyncでの事前Loadingをする
    /// </summary>
    /// <param name="scene">シーン名</param>
    /// <returns></returns>
    private IEnumerator AsyncLoading(string scene)
    {
        //シーンの事前読み込みをする
        async = SceneManager.LoadSceneAsync(scene);

        //勝手にシーン遷移しないようにする
        async.allowSceneActivation = false;

        while (!async.isDone)
        {
            if (async.progress < 0.9f)
            {
                NowLoading = false;

                //進行状況を取得
                AsyncProgress = async.progress;
            }
            else
            {
                NowLoading = true;

                //進行状況を取得
                AsyncProgress = async.progress;

                break;
            }

            yield return null;
        }
    }

    /// <summary>
    /// 暗転しつつAsyncLoadをする
    /// </summary>
    /// <returns></returns>
    private IEnumerator TransAsyncScene()
    {
        //画面をだんだん暗くするようにする
        //フェード中の判定をTrueにする
        isFading = true;

        //intervalと比較する時間
        float time = 0;

        //intervalよりtimeが下回っている間
        while (time <= interval)
        {
            //フェード中の透明度を０～１の間で変更していく
            FadeAlpha = Mathf.Lerp(0f, 1f, time / interval);

            //timeを1フレームづつ増加させていく
            time += Time.deltaTime;

            yield return null;
        }

        NowLoading = false;

        //Asyncしたシーンに遷移
        async.allowSceneActivation = true;

        //画面をだんだん明るくするようにする
        //比較時間をリセットする
        time = 0;

        //intervalよりtimeが下回っている間
        while (time <= interval)
        {
            //フェード中の透明度を1～0の間で変更していく
            FadeAlpha = Mathf.Lerp(1f, 0f, time / interval);

            //timeを１フレームづつ増加させていく
            time += Time.deltaTime;

            yield return null;
        }

        //フェード中の判定をFalseにする
        isFading = false;
    }

    /// <summary>
    /// ローディング中かどうかを返す
    /// </summary>
    /// <returns></returns>
    public bool ReturnLoading()
    {
        return NowLoading;
    }

    /// <summary>
    /// Asyncの進行具合を返す
    /// </summary>
    /// <returns></returns>
    public float ReturnProgress()
    {
        return AsyncProgress;
    }
}