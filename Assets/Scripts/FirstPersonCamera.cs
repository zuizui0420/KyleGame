using UnityEngine;

public class FirstPersonCamera : SingletonMonoBehaviour<FirstPersonCamera>
{
	private Vector3 _defaultPosition;

	[SerializeField]
	private GameObject _fpsOrigin;

	private Vector3 _fpsOriginPosition;

	private Camera _camera;

	private void Start()
	{
		_camera = GetComponent<Camera>();

		_defaultPosition = transform.localPosition;
		_fpsOriginPosition = _fpsOrigin.transform.localPosition;

		_camera.enabled = false;
	}

	private void Update()
	{
		if (PlayerSystem.instance.IsZooming)
		{
			_camera.enabled = true;
			transform.localPosition = Vector3.MoveTowards(transform.localPosition, _fpsOriginPosition, 0.2f);
		}
		else
		{
			_camera.enabled = false;
			transform.localPosition = Vector3.MoveTowards(transform.localPosition, _defaultPosition, 0.2f);
		}
	}
}