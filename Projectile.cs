using MaggieTools.Grapher;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

using UnityEditor;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
	[SerializeField, Range(0f, 180f)]
	public int angleOfProjection;

	[SerializeField, Range(.1f, 100f)]
	private float initialVelocity;

	[SerializeField, Range(.01f, 1f)]
	private float threshold;
	
	#region Working Variables

	[SerializeField]
	private Rigidbody2D _rigidbody;


	private Vector3 initialPos;

	private float a { get { return Physics.gravity.magnitude; } }
	private float d;
	private float s { get { return Vector2.Distance(transform.position, initialPos); } }
	private float t { get { return Time.time; } }
	private float v { get { return _rigidbody.velocity.magnitude; } }

	private float tMaxHeight = 0f;
	private float tReturn = 0f;

	private Vector2 posPreviousCall;

	List<Vector2> data = new List<Vector2>();

	bool maxHeightReached = false;
	bool returned = false;

	#endregion

	void Start()
	{
		_rigidbody = GetComponent<Rigidbody2D>();

		initialPos = transform.position;
		posPreviousCall = transform.position;
	}

	void OnValidate()
	{
		//set angle of projection
		transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, angleOfProjection));
	}

	public void PredictTrajectory()
	{
		data.Clear();

		float _timeOfFlight = GetTimeOfFlight(initialVelocity, angleOfProjection);
		Debug.Log($"Time of flight: {_timeOfFlight}");

		for (int i = 0; i < Grids.grid.graphResolution; i++)
		{
			float timeDivision = i / Grids.grid.graphResolution * _timeOfFlight;

			float x = GetX(initialVelocity, timeDivision, angleOfProjection);
			x += initialPos.x;

			float y = GetY(initialVelocity, timeDivision, angleOfProjection);
			y += initialPos.y;

			data.Add(new Vector2(x, y));
		}

		Grids.Plotter.PlotData(data.ToArray());
	}

	//get x displacement of projectile at time t with 
	public static float GetX(float u, float t, float theta)
	{
		//s = ut + 1/2 (at^2)
		//s = ut
		//since this is an ideal system with no air resistance,
		//we take horizontal acceleration as 0
		float x = u * Mathf.Cos(theta * Mathf.Deg2Rad) * t;

		return x;
	}

	//get y displacement of projectile at time t with 
	public static float GetY(float u, float t, float theta)
	{
		//s = ut + 1/2 (at^2)
		//here, a = -g

		u *= Mathf.Sin(theta * Mathf.Deg2Rad);
		float y = (u * t) + (0.5f * -9.81f * t * t);

		return y;
	}

	//get y displacement using equation of trajectory instead of Sy
	public static float GetYEOF(float x, float u, float t, float theta)
	{
		//y = tantheta * x - 0.5 * (g/ucostheta^2) x^2
		float a = Mathf.Tan(theta);
		float b = -0.5f * (9.81f / Mathf.Pow(u * Mathf.Cos(theta), 2));

		float y = a * x + b * x * x;
		return y;
	}

	//get time of flight for projectile
	public static float GetTimeOfFlight(float u, float theta)
	{
		float timeOfFlight = 2 * u * Mathf.Sin(Mathf.Deg2Rad * theta) / 9.81f;

		return timeOfFlight;
	}

	//only start registering data from when projectile is launched
	public void LaunchProjectile()
	{
		_rigidbody.velocity = transform.right * initialVelocity;
	}

	void FixedUpdate()
	{
		#region Give maximum height reached
		if (maxHeightReached && returned)
			return; 
		
		d += Vector3.Distance(posPreviousCall, transform.position);
		posPreviousCall = transform.position;

		//data.Add(new Vector2(t, d));

		if (!maxHeightReached)
		{
			if (Mathf.Abs(v) <= threshold)
			{
				tMaxHeight = t;
				maxHeightReached = true;
			}
		}

		if (maxHeightReached && !returned)
		{
			tReturn += Time.deltaTime;

			//time of flight = time of return
			if (tReturn >= tMaxHeight) {
				//Graph.graph.data = data.ToArray();
				returned = true;
				Debug.Log("Time of flight = " + (tMaxHeight + tReturn));
			}
		}
		#endregion
	}
}

[CustomEditor(typeof(Projectile))]
public class ProjectileEditor : Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		Projectile p = (Projectile)target;

		if (GUILayout.Button("Launch"))
		{
			p.LaunchProjectile();
		}

		if (GUILayout.Button("Predict Trajectory"))
		{
			p.PredictTrajectory();
		}
	}

}
