using UnityEngine;

public class FirstPersonCamera : SingletonMonoBehaviour<FirstPersonCamera>
{
	private Vector3 _defaultFirstPersonPosition;
	private Vector3 _defaultThirdPersonPosition;

	[SerializeField]
	[Header("1人称になる座標")]
	private GameObject FirstPerson_AnglePoint;

	private void Start()
	{
		_defaultThirdPersonPosition = transform.localPosition;
		_defaultFirstPersonPosition = FirstPerson_AnglePoint.transform.localPosition;

		GetComponent<Camera>().enabled = false;
	}

	private void Update()
	{
		if (!PlayerSystem.instance.PlayerControl)
		{
			GetComponent<Camera>().enabled = true;
			transform.localPosition = Vector3.MoveTowards(transform.localPosition, _defaultFirstPersonPosition, 0.3f);
		}
		else
		{
			GetComponent<Camera>().enabled = false;
			transform.localPosition = Vector3.MoveTowards(transform.localPosition, _defaultThirdPersonPosition, 0.3f);
		}
	}
}