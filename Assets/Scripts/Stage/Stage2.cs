using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// シーン：ステージ２
/// </summary>
public class Stage2 : MonoBehaviour
{
	void Start ()
    {
        AudioManager.Instance.PlayAudio(AUDIONAME.BGM_STAGE, 1, true, 128);
	}
}