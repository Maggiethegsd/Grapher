using UnityEngine;
using System.Linq;

[RequireComponent(typeof(Rigidbody2D))]
public class Charge : MonoBehaviour
{
	[Range(-1f, 1f)]
	public int charge;

	public const float k = 8.99f;

	public static Charge[] charges;

	[SerializeField]
	private Vector2 initialVelocity;

	private Rigidbody2D _rigidbody => GetComponent<Rigidbody2D>();

	void Awake() => _rigidbody.velocity = initialVelocity;
	void Start() => Mathf.Clamp(charge, -1f, 1f);

	void OnEnable() => charges = (from c in GameObject.FindGameObjectsWithTag("Charge")
										  select c.GetComponent<Charge>()).ToArray();
	void Update()
	{
		foreach (Charge c in charges)
		{
			if (c == this)
				continue;

			Vector3 dir = transform.position - c.transform.position;
			float dist = dir.magnitude;

			float coloumbicForce = (k * charge * c.charge) / (dist * dist);
			_rigidbody.AddForce(dir * coloumbicForce * Time.deltaTime);
		}
	}
}
