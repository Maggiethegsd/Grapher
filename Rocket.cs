using UnityEngine;

public class Rocket : MonoBehaviour
{
	[SerializeField, Range(.5f, 200f)]
	private float thrust;

	private Rigidbody2D _rigidbody;

	private float _dt => Time.fixedDeltaTime;

	void Start() => _rigidbody = GetComponent<Rigidbody2D>();

	void FixedUpdate()
	{
		float _x = Input.GetAxis("Horizontal");
		float _y = Input.GetAxis("Vertical");

		float _torque = -_x;
		float _velocityY = _y * thrust;

		Vector2 _thrustVector = transform.up * _velocityY;

		_rigidbody.AddForce(_thrustVector);
		_rigidbody.AddTorque(_torque);
	}
}
