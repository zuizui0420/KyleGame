using UnityEngine;
using System.Collections;

public class EnemyBase : MonoBehaviour
{
    Vector3 pos;
    bool pointZ = true;
    bool pointY = true;
    private void Start()
    {
        
    }

    private void Update()
    {
        towerPosY();
        towerPosZ();
        pos = transform.position;
    }
    //Y方向に動く(浮遊)
    public void towerPosY()
    {
        if (pointY == true)
        {
            transform.position += new Vector3(0, 0.01f, 0);
            if (pos.y > 24)
                pointY = false;
        }
        else if (pointY == false)
        {
            transform.position -= new Vector3(0, 0.01f, 0);
            if (pos.y < 22)
                pointY = true;
        }
    }
    //Z方向に動く(ゆらゆら)
    public void towerPosZ()
    {
        if (pointZ == true)
        {
            transform.position += new Vector3(0, 0, 0.003f);
            if (pos.z > 95.5f)
                pointZ = false;
        }
        else if (pointZ == false)
        {
            transform.position -= new Vector3(0, 0, 0.003f);
            if (pos.z < 94.5f)
                pointZ = true;
        }
    }
}
