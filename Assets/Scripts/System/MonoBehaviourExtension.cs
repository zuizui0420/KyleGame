using System.Collections;
using System;
using UnityEngine;

/// <summary>
/// Monobehaviour拡張
/// </summary>
public class MonoBehaviourExtension : MonoBehaviour
{
    bool Break = false;

    /// <summary>
    /// 待機時間を生成する機能
    /// </summary>
    /// <param name="_wait"></param>
    /// <param name="_act"></param>
    protected void WaitAfter(float _wait, Action _act)
    {
        if (_act != null)
        {
            StartCoroutine(_WaitAfter(_wait, _act));
        }
    }

    IEnumerator _WaitAfter(float _wait, Action _act)
    {
        while(_wait > 0)
        {
            _wait -= Time.deltaTime;

            if (Break)
            {
                Debug.Log("Break");
                Break = false;
                yield break;
            }

            yield return null;
        }

        _act();
    }

    /// <summary>
    /// 現在実行中のWaitAfterを全て破棄する
    /// </summary>
    public void WaitAfterBreak()
    {
        Break = true;
    }
}