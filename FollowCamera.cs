using UnityEngine;

public class FollowCamera : MonoBehaviour
{
	[SerializeField]
	private Transform target;

	[SerializeField]
	private Vector2 offset;

	[SerializeField, Range(1f, 10f)]
	private float followSpeed;

	[SerializeField, Range(.1f, 1f)]
	private float cameraZoom;

	private float _initialZ;
	private Camera _camera;

	void Start()
	{
		_initialZ = transform.position.z;
		_camera = Camera.main;
	}

	void Update()
	{
		Vector3 _target = new Vector3(target.position.x + offset.x, target.position.y + offset.y, _initialZ);

		Vector3 _smoothPosition = Vector3.Slerp(transform.position, _target, Time.deltaTime * followSpeed);
		transform.position = _smoothPosition; 
		
		_camera.orthographicSize = 5 / cameraZoom;
	}

}
