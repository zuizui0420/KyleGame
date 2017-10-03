﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// オーディオを再生
/// </summary>
public class AudioManager : MonoBehaviourExtension
{
    //シングルトンを作成
    #region Singleton
    //遷移先のシーンでこのスクリプトを使用するためのインスタンスを保存するためのもの
    private static AudioManager instance;

    //現在のシーンから外部で使用するインスタンスを作成
    public static AudioManager Instance
    {
        get
        {
            //送るインスタンスがnullの場合
            if (instance == null)
            {
                //シーンからSceneFaderを探して取得する
                instance = (AudioManager)FindObjectOfType(typeof(AudioManager));

                //もしシーン上にない場合
                if (instance == null)
                {
                    //エラーメッセージをログに出力する
                    Debug.LogError(typeof(AudioManager) + "is nothing");
                }
            }

            //送るインスタンスを返す
            return instance;
        }
    }

    #endregion Singleton

    [SerializeField]
    AudioClip[] Audio_Clips;

    [SerializeField]
    List<GameObject> GenerateAudioClips = new List<GameObject>();

    private Dictionary<string, GameObject> _generatedAudioClips = new Dictionary<string, GameObject>();

    public void Play(string name, float volume = 1.0f, bool loop = false, int priority = 128, bool isRecord = true)
    {
		var clipName = name + "_Audio";
       
		if (isRecord)
		{
			if (_generatedAudioClips.ContainsKey(clipName)) return;
		}

        //AudioSource生成
        var audioObj = new GameObject(clipName);
		var audioSource = audioObj.AddComponent<AudioSource>();

        //GenerateAudioClips.Add(AudioObject);
		if (isRecord && loop) _generatedAudioClips.Add(clipName, audioObj);

		audioSource.clip = Audio_Clips.FirstOrDefault(x => x.name.Equals(name));

		//ボリューム設定
		audioSource.volume = volume;

		//ループ設定
		audioSource.loop = loop;
		if (!loop) WaitAfter(2.0f, () => Destroy(audioObj));

		//優先度設定
		audioSource.priority = priority;

		//再生
		audioSource.Play();
    }

    /// <summary>
    /// オーディオの再生
    /// </summary>
    /// <param name="_name">オーディオの名前</param>
    /// <param name="_volume">ボリューム</param>
    /// <param name="_loop">ループ</param>
    /// <param name="_priority">プライオリティ</param>
    public void Play2(string _name, float _volume = 1.0f, bool _loop = false, int _priority = 128)
    {
        foreach(GameObject obj in GenerateAudioClips)
        {
            //同一ファイルが既に存在している場合
            if(obj.name == _name + "_Audio")
            {
                return;
            }
        }

        //AudioSource生成
        GameObject AudioObject = new GameObject(_name + "_Audio");
        AudioObject.AddComponent<AudioSource>();
        AudioSource audio = AudioObject.GetComponent<AudioSource>();

        GenerateAudioClips.Add(AudioObject);

        //Cilp設定
        foreach (AudioClip _cilp in Audio_Clips)
        {
            if (_cilp.name == _name)
            {
                audio.clip = _cilp;
            }
            else
            {
                //Debug.Log("ファイルなし");
            }
        }

        //ボリューム設定
        audio.volume = _volume;

        //ループ設定
        audio.loop = _loop;

        //優先度設定
        audio.priority = _priority;

        //再生
        audio.Play();

        //ループではない
        if (!_loop)
        {
            //破棄する
            WaitAfter(2.0f, () =>
            {
                Destroy(AudioObject);
            });
        }
    }

    /// <summary>
    /// BGMのフェードアウトをします
    /// </summary>
    /// <param name="_name">BGM名</param>
    /// <param name="_speed">速度</param>
    public IEnumerator BGMFadeOut(string _name, float _speed = 0.1f)
    {
        //AudioSourceを検索
        GameObject AudioObject = GameObject.Find(_name + "_Audio");

        AudioSource audio = AudioObject.GetComponent<AudioSource>();

        float volume = audio.volume;

        while (true)
        {
            volume -= _speed * Time.deltaTime;

            if(volume < 0)
            {
                audio.volume = 0;
                break;
            }

            audio.volume = volume;

            yield return null;
        } 
    }

    /// <summary>
    /// オーディオを削除
    /// </summary>
    /// <param name="_name"></param>
    public void AudioDelete(string _name)
    {
		var clipName = _name + "_Audio";

		//GenerateAudioClips.Remove(GameObject.Find(_name + "_Audio"));

		if (_generatedAudioClips.ContainsKey(clipName))
		{
			var obj = _generatedAudioClips[clipName];

			Destroy(obj);

			_generatedAudioClips.Remove(clipName);

			//AudioSourceを検索
		}
    }
}