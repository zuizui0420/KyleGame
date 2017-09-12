using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameContol : MonoBehaviour {
    public enum PHASE
    {
        PLAY,
        GAMEOVER,
        CLEAR,
    };
    public PHASE phase;

    public float time = 180;             //ゲーム時間
    public int score = 0;                //スコア
    public float playerBaseHP = 1000;    //自陣HP
    public bool isCrear = false;         //クリア判定

	// Use this for initialization
	void Start () {
        phase = PHASE.PLAY;
	
	}
	
	// Update is called once per frame
	void Update () {
        switch (phase)
        {
            case PHASE.PLAY:
                if (time <= 0 || playerBaseHP <= 0)
                    phase = PHASE.GAMEOVER;
                if (isCrear == true)
                    phase = PHASE.CLEAR;
                break;
            case PHASE.GAMEOVER:
                break;

            case PHASE.CLEAR:
                break;
        }
	
	}
}
