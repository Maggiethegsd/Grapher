using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class GravitationalBody : MonoBehaviour
{
	[Range(1f, 250f)]
	public float mass;

	private Rigidbody2D _rb;
	private GravitationalBody[] _bodies => FindObjectsByType<GravitationalBody>(FindObjectsSortMode.None); 

	private const float G = 10f;

	void Start() => _rb = GetComponent<Rigidbody2D>();

	void Update() => _rb.mass = mass;

	void FixedUpdate()
	{
		foreach (GravitationalBody body in _bodies)
		{
			if (body == this)
				continue;

			Vector2 disp = (transform.position - body.transform.position);
			float mag = (G * mass * body.mass) / (disp.magnitude * disp.magnitude);

			Vector2 F = disp.normalized * mag;
			Vector2 fPerp = new Vector2(F.y, -F.x);
			
			body.GetAttracted(F + fPerp/3);
		}
	}

	public void GetAttracted(Vector3 force) => _rb.AddForce(force);
}
