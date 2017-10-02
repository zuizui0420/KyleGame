using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;
using System.Linq;
using UnityEngine;

public class ScenFader : MonoBehaviour {
    #region Singleton
    private static ScenFader instance;
    public static ScenFader Instance
    {
        get
        {
            if (instance == null)
            {
                instance = (ScenFader)FindObjectOfType(typeof(ScenFader));
                if (instance == null)
                {
                    Debug.LogError(typeof(ScenFader) + "is nothing");
                }
            }
            return instance;
        }
    }

    #endregion Singleton
    private float FadeAlpha = 0;
    private bool isFading = false;
    public float interval;
    Color fadeColor = Color.black;
    public void Awake()
    {
        if (this != Instance)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }
    public void OnGUI()
    {
        if (isFading)
        {
            fadeColor.a = FadeAlpha;
            GUI.color = fadeColor;
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);
        }
    }
    public void LoadLevel(string scene)
    {
        StartCoroutine(TransScene(scene, interval));
    }
    private IEnumerator TransScene(string scene, float interval)
    {
        isFading = true;
        float time = 0;
        while (time <= interval)
        {
            FadeAlpha = Mathf.Lerp(0f, 1f, time / interval);
            time += Time.deltaTime;

            yield return null;
        }
        SceneManager.LoadScene(scene);
     
        time = 0;
        while (time <= interval)
        {
            FadeAlpha = Mathf.Lerp(1f, 0f, time / interval);
            time += Time.deltaTime;

            yield return null;
        }
        isFading = false;
    }
}

// Use this for  private static SceneFader instance;

