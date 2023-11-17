using UnityEngine;

public class CameraController : MonoBehaviour
{
	[SerializeField, Range(.1f, 10f)]
	private float moveSpeed;

	void Update()
	{
		Camera.main.orthographicSize -= Input.GetAxis("Mouse ScrollWheel");

		if (!Input.GetMouseButton(0)) 
			return;

		transform.Translate(-Input.GetAxis("Mouse X") * moveSpeed * 10f * Time.deltaTime, -Input.GetAxis("Mouse Y") * moveSpeed * 10f * Time.deltaTime, 0f);
	}
}
