using MaggieTools.Grapher;
using UnityEngine;
using TMPro;

public class Point : MonoBehaviour
{ 
	public float x;
	public float y;

	public float pathCovered { get { return CalculatePathCovered(); } }

	public float slope { get { return y/x; } }
	public float angle { get { return Mathf.Atan2(y, x) * Mathf.Rad2Deg; } }

	public bool selfDestruct = false;

	[Range(0f, 5f)]
	public float selfDestructTime = 0f;

	private float pointAliveTime;

	[SerializeField] private Transform _infoPane;
	[SerializeField] private TextMeshProUGUI _coordinateText;
	[SerializeField] private TextMeshProUGUI _slopeText;

	private void Awake()
	{
		_infoPane.gameObject.SetActive(false);
	}

	void Update()
	{
		pointAliveTime += Time.deltaTime;

		if (selfDestruct)
		{
			if (pointAliveTime >= selfDestructTime)
				Destroy(gameObject);
		}
	}

	private void OnMouseEnter()
	{
		_infoPane.gameObject.SetActive(true);
		_coordinateText.text = $"X: {x.ToString("0.00")} | Y: {y.ToString("0.00")}";
		_slopeText.text = $"A: {angle.ToString("0.00")} | dy/dx: {slope.ToString("0.00")}";
	}

	private void OnMouseExit()
	{
		_infoPane.gameObject.SetActive(false);
	}

	private float CalculatePathCovered() => Mathf.Abs(Grids.grid.lx - x) / Grids.grid.gridSize;
}
