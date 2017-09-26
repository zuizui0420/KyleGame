using UnityEngine;

public class FirstPersonCamera : SingletonMonoBehaviour<FirstPersonCamera>
{
	private Vector3 DefaultPointPos;

	[SerializeField]
	[Header("1人称になる座標")]
	private GameObject FirstPerson_AnglePoint;

	private Vector3 FirstPersonPointPos;

	private void Start()
	{
		DefaultPointPos = transform.localPosition;
		FirstPersonPointPos = FirstPerson_AnglePoint.transform.localPosition;

		GetComponent<Camera>().enabled = false;
	}

	private void Update()
	{
		if (!PlayerSystem.instance.PlayerControle)
		{
			GetComponent<Camera>().enabled = true;
			transform.localPosition = Vector3.MoveTowards(transform.localPosition, FirstPersonPointPos, 0.3f);
		}
		else
		{
			GetComponent<Camera>().enabled = false;
			transform.localPosition = Vector3.MoveTowards(transform.localPosition, DefaultPointPos, 0.3f);
		}
	}
}