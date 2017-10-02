using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleControle : MonoBehaviour
{
	void Update ()
    {
        if (Input.anyKeyDown)
        {
            SceneFader.Instance.LoadLevel(SCENENAME.SCENE_STAGE1);
        }       
    }
}