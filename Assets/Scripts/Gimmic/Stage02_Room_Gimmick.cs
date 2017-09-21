using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// STAGE2_ROOM02 ギミック：電気床
/// </summary>
public class Stage02_Room_Gimmick : MonoBehaviour
{
    [SerializeField,Header("SCI_FI_Switch")]
    SCI_FI_Switch Switch;

    [SerializeField, Header("Panel群")]
    GameObject[] Panel;

    List<MeshRenderer> m_renderer = new List<MeshRenderer>();

    Color DefaultColor = new Color(1, 1, 1), MoveColor = new Color(1, 0, 0);

    float ColorLerpSpeed = 0.1f;

    //[0] : 通行可能 [1] : 通行不可能
    int[,] Panel_ID =   { 
                            { 1, 1, 1, 0, 0, 1, 1, 1 },
                            { 1, 0, 0, 0, 0, 0, 0, 1 },
                            { 1, 0, 1, 1, 1, 1, 0, 1 },
                            { 1, 0, 0, 0, 0, 0, 0, 1 },
                            { 1, 1, 1, 0, 0, 1, 1, 1 },
                        };

    void Start ()
    {
        int num = 0;

        foreach (int id in Panel_ID)
        {
            //ID確認
            if (num == Panel[num].GetComponent<Stage2_Room_Gimmick_Panel>().ID)
            {
                //通行不可能かを確認
                if (id == 1)
                {
                    m_renderer.Add(Panel[num].GetComponent<MeshRenderer>());
                }
            }

            num++;
        }
    }

	void Update ()
    {
        ColorMove(Switch.SwitchOn);
	}

    private void ColorMove(bool flg)
    {
        if (flg)
        {
            foreach (MeshRenderer renderer in m_renderer)
            {
                renderer.material.color = Color.Lerp(renderer.material.color, MoveColor, ColorLerpSpeed);
            }
        }
        else
        {
            foreach (MeshRenderer renderer in m_renderer)
            {
                renderer.material.color = Color.Lerp(renderer.material.color, DefaultColor, ColorLerpSpeed);
            }
        }
    }
}