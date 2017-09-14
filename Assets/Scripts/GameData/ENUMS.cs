using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// enumの列挙型データを保管する場所
/// </summary>
public class ENUMS : MonoBehaviour
{
    public enum PLAYER
    {
        ONE,    //1P
        TWO,    //2P
        THREE,  //3P
        FOUR,   //4P
    }

    public enum COMMAND
    {
        AXIS,   //Axis
        TRIGGER,    //Trigger
    }

    public enum FOOD
    {
        Cheese, //チーズ
        Tomato, //トマト
        Lettuce,    //レタス
        Bacon,  //ベーコン
        Steak,  //ステーキ
        Ham,    //ハム
        Egg,    //タマゴ
    }

    public enum EFFECT
    {
        GetFood,    //具材を挟んだ時のエフェクト
    }

    public enum AUDIO
    {
        BGM,    //BGM
        SE, //SE
    }

    public enum AI_STR
    {
        EASY,   //弱い
        NORMAL, //普通
        HARD,   //強い
    }
}