using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// アステロイド群を回転する
/// </summary>
public class AsteroidRotate : MonoBehaviour
{
	void Update ()
    {
        Vector3 Rot = transform.eulerAngles;
        Rot.y += 0.1f;
        transform.eulerAngles = Rot;
	}
}